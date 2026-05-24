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


    [HttpGet]
    public IActionResult GetBookings()
    {

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

                BookedSeats = b.Seats.Select(s => s.SeatNumber).ToList()
            })
            .ToList();

        return Ok(bookings);
    }


    [HttpGet("{id}")]
    public IActionResult GetBooking(Guid id)
    {
        var booking = _db.Bookings
            .Include(b => b.Seats)
            .FirstOrDefault(b => b.Id == id && b.IsDelete == false);

        if (booking == null)
        {
            return NotFound("သင်ရှာဖွေနေသော ကားလက်မှတ်ဝယ်ယူမှုမှတ်တမ်း (Voucher) ကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။");
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



    [HttpPost]
    public IActionResult CreateBooking(BookingCreateRequestModel request)
    {

        if (request.SeatNumbers == null || !request.SeatNumbers.Any())
        {
            return BadRequest(new BookingResponseModel { IsSuccess = false, Message = "ကျေးဇူးပြု၍ အနည်းဆုံး ကားခုံတစ်ခုံ ရွေးချယ်ပေးပါဗျာ။" });
        }


        var schedule = _db.Schedules
            .Include(s => s.Route)
            .Include(s => s.Bus)
            .FirstOrDefault(s => s.IsDelete == false
                              && s.Route.DepartureStation.Contains(request.DepartureStation)
                              && s.Route.ArrivalStation.Contains(request.ArrivalStation)
                              && s.Bus.BusNumber == request.BusNumber);


        if (schedule == null)
        {
            return BadRequest(new BookingResponseModel
            {
                IsSuccess = false,
                Message = $"ခရီးစဉ် ရှာမတွေ့ပါဗျာ။ ({request.DepartureStation} မှ {request.ArrivalStation} သို့ သွားမည့် ကားနံပါတ် {request.BusNumber} အချိန်စာရင်း မရှိပါ)"
            });
        }


        decimal totalAmount = request.SeatNumbers.Count * schedule.TicketPrice;


        using var transaction = _db.Database.BeginTransaction();

        try
        {

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

            foreach (var seatNo in request.SeatNumbers)
            {

                var seat = _db.Seats.FirstOrDefault(s => s.ScheduleId == schedule.Id
                                                      && s.SeatNumber == seatNo
                                                      && s.IsDelete == false);

                if (seat == null)
                {
                    return BadRequest(new BookingResponseModel { IsSuccess = false, Message = $"ရွေးချယ်ထားသော ခုံနံပါတ် ({seatNo}) သည် ဤခရီးစဉ်လမ်းကြောင်းပေါ်တွင် မရှိပါဗျာ။" });
                }

                if (seat.IsBooked)
                {
                    return BadRequest(new BookingResponseModel { IsSuccess = false, Message = $"စိတ်မကောင်းပါဘူးဗျာ... ခုံနံပါတ် ({seatNo}) ကတော့ တခြားသူ ဝယ်ယူသွားလို့ လက်မှတ်ကုန်သွားပါပြီ။" });
                }


                seat.IsBooked = true;
                seat.BookingId = bookingId;
                seat.ModifiedBy = request.CreatedBy ?? "Passenger-Self";
                seat.ModifiedAt = DateTime.Now;
            }

            _db.SaveChanges();
            transaction.Commit();

            return StatusCode(201, new BookingResponseModel
            {
                IsSuccess = true,
                Message = $"လက်မှတ်ဝယ်ယူမှု အောင်မြင်ပါပြီဗျာ။ စုစုပေါင်းကျသင့်ငွေ - {totalAmount} ကျပ်။",
                BookingId = bookingId
            });
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return StatusCode(500, new { isSuccess = false, message = "လက်မှတ်ဝယ်ယူမှု မအောင်မြင်ပါသဖြင့် စနစ်မှ ငွေပေးချေမှုကို ပယ်ဖျက်ပြီး မူလအတိုင်း ပြန်လည်ပြင်ဆင်ပေးလိုက်ပါပြီ။", error = ex.Message });
        }
    }
}