using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Models;


namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly AppDbContext _db = new AppDbContext();

    // ၁။ GET: api/Schedules (ခရီးစဉ်အချိန်စာရင်းအားလုံး ပြရန် - Bus ရော Route ပါ တွဲပြထားသည်)
    [HttpGet]
    public IActionResult GetSchedules()
    {
        var lst = _db.Schedules
            .Include(s => s.Bus)    // 🔥 Bus Table ကို ချိတ်ဆက်ဆွဲယူခြင်း
            .Include(s => s.Route)  // 🔥 Route Table ကို ချိတ်ဆက်ဆွဲယူခြင်း
            .Where(x => x.IsDelete == false)
            .Select(x => new ScheduleModel
            {
                Id = x.Id,
                RouteId = x.RouteId,
                BusId = x.BusId,
                DepartureTime = x.DepartureTime,
                TicketPrice = x.TicketPrice,
                CreatedBy = x.CreatedBy,
                CreatedAt = Convert.ToDateTime(x.CreatedAt),
                ModifiedBy = x.ModifiedBy,
                ModifiedAt = x.ModifiedAt,

                // 🚌 Bus Information
                BusNumber = x.Bus.BusNumber,
                BusType = x.Bus.BusType,

                // 🗺️ Route Information
                DepartureStation = x.Route.DepartureStation,
                ArrivalStation = x.Route.ArrivalStation,
                DistanceKM = x.Route.DistanceKm
            })
            .ToList();

        return Ok(lst);
    }

    // ၂။ GET: api/Schedules/{id} (သီးသန့် ခရီးစဉ်တစ်ခုကို ကြည့်ရန် - Bus ရော Route ပါ တွဲပြထားသည်)
    [HttpGet("{id}")]
    public IActionResult GetSchedule(Guid id)
    {
        var item = _db.Schedules
            .Include(s => s.Bus)   // 🔥 Bus Table ကို Join ထားခြင်း
            .Include(s => s.Route) // 🔥 Route Table ကို Join ထားခြင်း
            .FirstOrDefault(x => x.Id == id && x.IsDelete == false);

        if (item is null)
        {
            return NotFound("No schedule data found");
        }

        var scheduleData = new ScheduleModel
        {
            Id = item.Id,
            RouteId = item.RouteId,
            BusId = item.BusId,
            DepartureTime = item.DepartureTime,
            TicketPrice = item.TicketPrice,
            CreatedBy = item.CreatedBy,
            CreatedAt = Convert.ToDateTime(item.CreatedAt),
            ModifiedBy = item.ModifiedBy,
            ModifiedAt = item.ModifiedAt,

            // 🚌 Bus Information
            BusNumber = item.Bus.BusNumber,
            BusType = item.Bus.BusType,

            // 🗺️ Route Information
            DepartureStation = item.Route.DepartureStation,
            ArrivalStation = item.Route.ArrivalStation,
            DistanceKM = item.Route.DistanceKm
        };

        return Ok(scheduleData);
    }

    // ၃။ GET: api/Schedules/search?from=Yangon&to=Mandalay (ရှာဖွေခြင်း - Bus ရော Route ပါ တွဲပြထားသည်)
    [HttpGet("search")]
    public IActionResult SearchSchedules([FromQuery] string from, [FromQuery] string to)
    {
        if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
        {
            return BadRequest(new { isSuccess = false, message = "Departure (From) နှင့် Arrival (To) နေရာများကို သေချာဖြည့်ပေးပါဗျာ။" });
        }

        var searchResult = _db.Schedules
            .Include(s => s.Bus)   // 🔥 Bus Table ကိုပါ Join ခေါ်ယူခြင်း
            .Include(s => s.Route) // 🔥 Route Table ကိုပါ Join ခေါ်ယူခြင်း
            .Where(s => s.IsDelete == false
                     && s.Route.DepartureStation.Contains(from)
                     && s.Route.ArrivalStation.Contains(to))
            .Select(x => new ScheduleModel
            {
                Id = x.Id,
                RouteId = x.RouteId,
                BusId = x.BusId,
                DepartureTime = x.DepartureTime,
                TicketPrice = x.TicketPrice,
                CreatedBy = x.CreatedBy,
                CreatedAt = Convert.ToDateTime(x.CreatedAt),
                ModifiedBy = x.ModifiedBy,
                ModifiedAt = x.ModifiedAt,

                // 🚌 Bus Information
                BusNumber = x.Bus.BusNumber,
                BusType = x.Bus.BusType,

                // 🗺️ Route Information
                DepartureStation = x.Route.DepartureStation,
                ArrivalStation = x.Route.ArrivalStation,
                DistanceKM = x.Route.DistanceKm
            })
            .ToList();

        return Ok(searchResult);
    }

    // ၄။ POST: api/Schedules (ခရီးစဉ်အသစ်ဆောက်သည်နှင့် ကားခုံများပါ Auto ကြိုဆောက်ပေးမည့်နေရာ)
    [HttpPost]
    public IActionResult CreateSchedule(ScheduleCreateRequestModel request)
    {
        var bus = _db.Buses.FirstOrDefault(b => b.Id == request.BusId && b.IsDelete == false);
        if (bus == null)
        {
            return BadRequest(new ScheduleCreateResponseModel { IsSuccess = false, Message = "choosing car id not found" });
        }

        var routeExists = _db.Routes.Any(r => r.Id == request.RouteId && r.IsDelete == false);
        if (!routeExists)
        {
            return BadRequest(new ScheduleCreateResponseModel { IsSuccess = false, Message = "choosing car routes not found" });
        }

        var scheduleId = Guid.NewGuid();
        var newSchedule = new Schedule
        {
            Id = scheduleId,
            RouteId = request.RouteId,
            BusId = request.BusId,
            DepartureTime = request.DepartureTime,
            TicketPrice = request.TicketPrice,
            CreatedBy = request.CreatedBy ?? "Admin-Staff",
            CreatedAt = DateTime.Now,
            IsDelete = false
        };
        _db.Schedules.Add(newSchedule);

        int seatsPerRow = 4;
        if (bus.BusType.Contains("VIP", StringComparison.OrdinalIgnoreCase))
        {
            seatsPerRow = 3;
        }

        int seatCount = 1;
        char rowLetter = 'A';

        while (seatCount <= bus.TotalSeats)
        {
            for (int i = 1; i <= seatsPerRow; i++)
            {
                if (seatCount > bus.TotalSeats) break;

                string seatNumber = $"{rowLetter}{i}";

                var newSeat = new Seat
                {
                    ScheduleId = scheduleId,
                    SeatNumber = seatNumber,
                    IsBooked = false,
                    BookingId = null,
                    CreatedBy = "System-Auto",
                    IsDelete = false
                };
                _db.Seats.Add(newSeat);
                seatCount++;
            }
            rowLetter++;
        }

        int result = _db.SaveChanges();

        return StatusCode(201, new ScheduleCreateResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? $"saving Schedule successfully ({bus.TotalSeats})" : "Saving Schedule Failed"
        });
    }

    // ၅။ PUT: api/Schedules/{id} (ခရီးစဉ်တစ်ခုလုံးကို အစားထိုးပြင်ရန်)
    [HttpPut("{id}")]
    public IActionResult UpdateSchedule(Guid id, ScheduleUpdateRequestModel request)
    {
        var item = _db.Schedules.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new ScheduleUpdateResponseModel { IsSuccess = false, Message = "Schedule not found" });
        }

        item.RouteId = request.RouteId;
        item.BusId = request.BusId;
        item.DepartureTime = request.DepartureTime;
        item.TicketPrice = request.TicketPrice;

        item.ModifiedBy = request.ModifiedBy ?? "Admin-Modifier";
        item.ModifiedAt = DateTime.Now;
        

        try
        {
            int result = _db.SaveChanges();

            return Ok(new ScheduleUpdateResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Schedule Update Successfully" : "Schedule Update Failed",
                Data = new ScheduleModel
                {
                    Id = item.Id,
                    RouteId = item.RouteId,
                    BusId = item.BusId,
                    DepartureTime = item.DepartureTime,
                    TicketPrice = item.TicketPrice,
                    CreatedBy = item.CreatedBy,
                    CreatedAt = Convert.ToDateTime(item.CreatedAt),
                    ModifiedBy = item.ModifiedBy,
                    ModifiedAt = item.ModifiedAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { isSuccess = false, message = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    // ၆။ PATCH: api/Schedules/{id} (လက်မှတ်ဈေးနှုန်း သို့မဟုတ် ကားထွက်ချိန်တစ်ခုတည်းကို ကွက်တိပြင်ရန်)
    [HttpPatch("{id}")]
    public IActionResult PatchSchedule(Guid id, SchedulePatchRequestModel request)
    {
        var item = _db.Schedules.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new SchedulePatchResponseModel { IsSuccess = false, Message = "Schedule not found" });
        }

        int count = 0;
        if (request.RouteId.HasValue) { count++; item.RouteId = request.RouteId.Value; }
        if (request.BusId.HasValue) { count++; item.BusId = request.BusId.Value; }
        if (request.DepartureTime.HasValue) { count++; item.DepartureTime = request.DepartureTime.Value; }
        if (request.TicketPrice.HasValue && request.TicketPrice.Value > 0) { count++; item.TicketPrice = request.TicketPrice.Value; }

        if (count == 0)
        {
            return BadRequest(new SchedulePatchResponseModel { IsSuccess = false, Message = "No fields need to update" });
        }

        item.ModifiedBy = request.ModifiedBy ?? "Admin-Patch-User";
        item.ModifiedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new SchedulePatchResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "Schedule Patch Successfully" : "Schedule Patch Failed",
            Data = new ScheduleModel
            {
                Id = item.Id,
                RouteId = item.RouteId,
                BusId = item.BusId,
                DepartureTime = item.DepartureTime,
                TicketPrice = item.TicketPrice,
                CreatedBy = item.CreatedBy,
                CreatedAt = Convert.ToDateTime(item.CreatedAt),
                ModifiedBy = item.ModifiedBy,
                ModifiedAt = item.ModifiedAt
            }
        });
    }

    // ၇။ DELETE: api/Schedules/{id} (ခရီးစဉ် ဖျက်သိမ်းခြင်း - Soft Delete)
    [HttpDelete("{id}")]
    public IActionResult DeleteSchedule(Guid id, ScheduleDeleteRequestModel request)
    {
        var item = _db.Schedules.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new ScheduleDeleteResponseModel { IsSuccess = false, Message = "Schedule not found or already deleted" });
        }

        item.IsDelete = true;
        item.DeletedBy = request.DeletedBy ?? "Admin-Delete-User";
        item.DeletedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new ScheduleDeleteResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "Schedule Soft-Deleted Successfully" : "Schedule Soft-Delete Failed"
        });
    }
}