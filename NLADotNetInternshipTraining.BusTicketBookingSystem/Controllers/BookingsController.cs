using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Models;

namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db = new AppDbContext();

    // ===================================================================
    // ၁။ GET: api/Bookings (ဝယ်ယူထားသည့် ဘောင်ချာမှတ်တမ်းအားလုံး ကြည့်ရန်)
    // ===================================================================
    [HttpGet]
    public IActionResult GetBookings()
    {
        // Include(b => b.Seats) ကိုသုံးပြီး ဘောင်ချာနဲ့တွဲရက် ခုံတွေကိုပါ ဆွဲထုတ်မယ်
        var bookings = _db.Bookings
            .Include(b => b.Seats)
            .Where(b => b.IsDelete == false)
            .Select(b => new BookingModel
            {
                Id = b.Id,
                CustomerName = b.CustomerName,
                CustomerPhone = b.CustomerPhone,
                TotalPrice = b.TotalPrice,
                BookingDate = Convert.ToDateTime(b.BookingDate),
                CreatedBy = b.CreatedBy,
                // ဒီ Booking ID နဲ့ ငြိနေတဲ့ ခုံနံပါတ်တွေကိုပဲ စစ်ထုတ်ပြီး List ဖွဲ့ပြမယ်
                BookedSeats = b.Seats.Select(s => s.SeatNumber).ToList()
            })
            .ToList();

        return Ok(bookings);
    }

    // ===================================================================
    // ၂။ GET: api/Bookings/{id} (ဘောင်ချာ ID ဖြင့် သီးသန့် တစ်စောင်ချင်းစီ ရှာကြည့်ရန်)
    // ===================================================================
    [HttpGet("{id}")]
    public IActionResult GetBooking(Guid id)
    {
        var booking = _db.Bookings
            .Include(b => b.Seats)
            .FirstOrDefault(b => b.Id == id && b.IsDelete == false);

        if (booking == null)
        {
            return NotFound("your requesting (Voucher) do not found");
        }

        var result = new BookingModel
        {
            Id = booking.Id,
            CustomerName = booking.CustomerName,
            CustomerPhone = booking.CustomerPhone,
            TotalPrice = booking.TotalPrice,
            BookingDate = Convert.ToDateTime(booking.BookingDate),
            CreatedBy = booking.CreatedBy,
            BookedSeats = booking.Seats.Select(s => s.SeatNumber).ToList()
        };

        return Ok(result);
    }

    // ===================================================================
    // ၃။ POST: api/Bookings (လက်မှတ်ဝယ်ယူခြင်း Core Transaction API)
    // ===================================================================

    [HttpPost]
    public IActionResult CreateBooking(BookingCreateRequestModel request)
    {
        // ၁။ Validation: ခုံနံပါတ် ပါ/မပါ အရင်စစ်မယ်
        if (request.SeatNumbers == null || !request.SeatNumbers.Any())
        {
            return BadRequest(new BookingResponseModel { IsSuccess = false, Message = "Please choose at least one seat." });
        }

        // ၂။ 🔥 [အဓိကအကွက်] မြို့အမည်များနှင့် ကားနံပါတ်ကို သုံးပြီး နောက်ကွယ်ကနေ Schedule ID ကို လှမ်းရှာခြင်း
        var schedule = _db.Schedules
            .Include(s => s.Route)
            .Include(s => s.Bus)
            .FirstOrDefault(s => s.IsDelete == false
                              && s.Route.DepartureStation.Contains(request.DepartureStation)
                              && s.Route.ArrivalStation.Contains(request.ArrivalStation)
                              && s.Bus.BusNumber == request.BusNumber);

        // အကယ်၍ ရှာမတွေ့ရင် စမတ်ကျကျ Error ပြန်ထုတ်ပေးမယ်
        if (schedule == null)
        {
            return BadRequest(new BookingResponseModel
            {
                IsSuccess = false,
                Message = $"ခရီးစဉ် ရှာမတွေ့ပါဗျာ။ ({request.DepartureStation} မှ {request.ArrivalStation} သို့ သွားမည့် ကားနံပါတ် {request.BusNumber} အချိန်စာရင်း မရှိပါ)"
            });
        }

        // ၃။ ရှာတွေ့ထားတဲ့ schedule ရဲ့ လက်မှတ်ဈေးနှုန်းနဲ့ စုစုပေါင်း ကျသင့်ငွေ တွက်ချက်ခြင်း
        decimal totalAmount = request.SeatNumbers.Count * schedule.TicketPrice;

        // ၄။ Database Transaction စတင် အသုံးပြုခြင်း
        using var transaction = _db.Database.BeginTransaction();

        try
        {
            // (က) ပင်မ Voucher (Booking) သိမ်းဆည်းခြင်း
            var bookingId = Guid.NewGuid();
            var newBooking = new Booking
            {
                Id = bookingId,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                TotalPrice = totalAmount,
                BookingDate = DateTime.Now,
                CreatedBy = request.CreatedBy ?? "Passenger-Self",
                IsDelete = false
            };
            _db.Bookings.Add(newBooking);

            // (ခ) ရှာတွေ့ထားတဲ့ schedule.Id အောက်က ခုံတွေကို Loop ပတ် စစ်ဆေး/Update လုပ်ခြင်း
            foreach (var seatNo in request.SeatNumbers)
            {
                // 💡 schedule.Id ကို သုံးပြီး ကွက်တိ ရှာသွားပါတယ်
                var seat = _db.Seats.FirstOrDefault(s => s.ScheduleId == schedule.Id
                                                      && s.SeatNumber == seatNo
                                                      && s.IsDelete == false);

                if (seat == null)
                {
                    return BadRequest(new BookingResponseModel { IsSuccess = false, Message = $"Seat number ({seatNo}) does not exist for this route." });
                }

                if (seat.IsBooked)
                {
                    return BadRequest(new BookingResponseModel { IsSuccess = false, Message = $"Sorry, seat number ({seatNo}) is already sold out." });
                }

                // ခုံကို ဝယ်ပြီးကြောင်း အမှတ်အသားလုပ်ပြီး BookingId ချိတ်မယ်
                seat.IsBooked = true;
                seat.BookingId = bookingId;
                seat.ModifiedBy = request.CreatedBy ?? "Passenger-Self";
                seat.ModifiedAt = DateTime.Now;
            }

            _db.SaveChanges();
            transaction.Commit(); // အားလုံး အောင်မြင်မှ ဒေတာဘေ့စ်ထဲ တကယ် သိမ်းမယ်

            return StatusCode(201, new BookingResponseModel
            {
                IsSuccess = true,
                Message = $"လက်မှတ်ဝယ်ယူမှု အောင်မြင်ပါပြီဗျာ။ စုစုပေါင်းကျသင့်ငွေ - {totalAmount} ကျပ်။",
                BookingId = bookingId
            });
        }
        catch (Exception ex)
        {
            transaction.Rollback(); // Error တက်ရင် နဂိုအတိုင်း ပြန်ဆုတ်မယ်
            return StatusCode(500, new { isSuccess = false, message = "Your ticket purchase failed and transaction was rolled back.", error = ex.Message });
        }
    }
}