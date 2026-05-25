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

    [HttpGet]
    public IActionResult GetSchedules()
    {
        var lst = _db.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
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


                BusNumber = x.Bus.BusNumber,
                BusType = x.Bus.BusType,

                DepartureStation = x.Route.DepartureStation,
                ArrivalStation = x.Route.ArrivalStation,
                DistanceKM = x.Route.DistanceKm
            })
            .ToList();

        return Ok(lst);
    }

    [HttpGet("{id}")]
    public IActionResult GetSchedule(Guid id)
    {
        var item = _db.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .FirstOrDefault(x => x.Id == id && x.IsDelete == false);

        if (item is null)
        {
            return NotFound("ရွေးချယ်ထားသော ကားထွက်မည့်အချိန်စာရင်းကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။");
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


            BusNumber = item.Bus.BusNumber,
            BusType = item.Bus.BusType,


            DepartureStation = item.Route.DepartureStation,
            ArrivalStation = item.Route.ArrivalStation,
            DistanceKM = item.Route.DistanceKm
        };

        return Ok(scheduleData);
    }

    [HttpGet("search")]
    public IActionResult SearchSchedules([FromQuery] string? from, [FromQuery] string? to)
    {
        if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
        {
            return BadRequest(new ScheduleSearchResponseModel { IsSuccess = false, Message = "Departure (From) နှင့် Arrival (To) နေရာများကို သေချာဖြည့်ပေးပါဗျာ။" });
        }

        var searchResult = _db.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
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


                BusNumber = x.Bus.BusNumber,
                BusType = x.Bus.BusType,

                DepartureStation = x.Route.DepartureStation,
                ArrivalStation = x.Route.ArrivalStation,
                DistanceKM = x.Route.DistanceKm
            })
            .ToList();
        if (searchResult == null || !searchResult.Any())
        {
            return NotFound(new ScheduleSearchResponseModel
            {
                IsSuccess = false,
                Message = $"စိတ်မကောင်းပါဘူးဗျာ... {from} မှ {to} သို့ သွားမည့် ခရီးစဉ်အချိန်စာရင်း ရှာမတွေ့ပါ သို့မဟုတ် ဖျက်သိမ်းထားပြီး ဖြစ်ပါတယ်ဗျာ။"
            });
        }

        return Ok(searchResult);

    }

    [HttpPost]
    public IActionResult CreateSchedule(ScheduleCreateRequestModel request)
    {
        var bus = _db.Buses.FirstOrDefault(b => b.Id == request.BusId && b.IsDelete == false);
        if (bus == null)
        {
            return BadRequest(new ScheduleCreateResponseModel { IsSuccess = false, Message = "ရွေးချယ်လိုက်သော အဝေးပြေးကား ID ကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။" });
        }

        var routeExists = _db.Routes.Any(r => r.Id == request.RouteId && r.IsDelete == false);
        if (!routeExists)
        {
            return BadRequest(new ScheduleCreateResponseModel { IsSuccess = false, Message = "ရွေးချယ်လိုက်သော ခရီးစဉ်လမ်းကြောင်း ID ကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။" });
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

        // int seatCount = 1;
        // char rowLetter = 'A';

        // while (seatCount <= bus.TotalSeats)
        // {
        //     for (int i = 1; i <= seatsPerRow; i++)
        //     {
        //         if (seatCount > bus.TotalSeats) break;

        //         string seatNumber = $"{rowLetter}{i}";

        //         var newSeat = new Seat
        //         {
        //             ScheduleId = scheduleId,
        //             SeatNumber = seatNumber,
        //             IsBooked = false,
        //             BookingId = null,
        //             CreatedBy = "System-Auto",
        //             IsDelete = false
        //         };
        //         _db.Seats.Add(newSeat);
        //         seatCount++;
        //     }
        //     rowLetter++;
        // }

        int seatCount = 1;
        int currentRowNumber = 1;

        while (seatCount <= bus.TotalSeats)
        {

            char rowLetter = (char)('A' + (currentRowNumber - 1));

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

            currentRowNumber++;
        }

        int result = _db.SaveChanges();

        return StatusCode(201, new ScheduleCreateResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? $"ကားထွက်မည့် အချိန်စာရင်းနှင့် ကားခုံ အရေအတွက် ({bus.TotalSeats}) ခုကို အောင်မြင်စွာ ဖန်တီးပြီးပါပြီ။" : "အချိန်စာရင်း ဖန်တီးမှု မအောင်မြင်ပါ။"
        });
    }


    [HttpPut("{id}")]
    public IActionResult UpdateSchedule(Guid id, ScheduleUpdateRequestModel request)
    {
        var item = _db.Schedules.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new ScheduleUpdateResponseModel { IsSuccess = false, Message = "ပြုပြင်လိုသော ကားထွက်မည့်အချိန်စာရင်းကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။" });
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
                Message = result > 0 ? "ကားထွက်မည့် အချိန်စာရင်းကို အောင်မြင်စွာ ပြုပြင်မွမ်းမံပြီးပါပြီ။" : "အချိန်စာရင်း ပြုပြင်မွမ်းမံမှု မအောင်မြင်ပါ။",
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


    [HttpPatch("{id}")]
    public IActionResult PatchSchedule(Guid id, SchedulePatchRequestModel request)
    {
        var item = _db.Schedules.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new SchedulePatchResponseModel { IsSuccess = false, Message = "ပြုပြင်လိုသော ကားထွက်မည့်အချိန်စာရင်းကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။" });
        }

        int count = 0;
        if (request.RouteId.HasValue) { count++; item.RouteId = request.RouteId.Value; }
        if (request.BusId.HasValue) { count++; item.BusId = request.BusId.Value; }
        if (request.DepartureTime.HasValue) { count++; item.DepartureTime = request.DepartureTime.Value; }
        if (request.TicketPrice.HasValue && request.TicketPrice.Value > 0) { count++; item.TicketPrice = request.TicketPrice.Value; }

        if (count == 0)
        {
            return BadRequest(new SchedulePatchResponseModel { IsSuccess = false, Message = "ပြုပြင်မွမ်းမံရန်အတွက် မည်သည့်အချက်အလက်မှ ထည့်သွင်းထားခြင်းမရှိပါဗျာ။" });
        }

        item.ModifiedBy = request.ModifiedBy ?? "Admin-Patch-User";
        item.ModifiedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new SchedulePatchResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "ကားထွက်မည့် အချိန်စာရင်းကို ကွက်တိ အောင်မြင်စွာ ပြုပြင်မွမ်းမံပြီးပါပြီ။" : "အချိန်စာရင်း ကွက်တိပြုပြင်မွမ်းမံမှု မအောင်မြင်ပါ။",
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


    [HttpDelete("{id}")]
    public IActionResult DeleteSchedule(Guid id, ScheduleDeleteRequestModel request)
    {
        var item = _db.Schedules.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new ScheduleDeleteResponseModel { IsSuccess = false, Message = "ရွေးချယ်ထားသော ကားထွက်မည့်အချိန်စာရင်းကို စနစ်ထဲမှာ ရှာမတွေ့ပါ သို့မဟုတ် ဖျက်သိမ်းပြီးသား ဖြစ်နေပါတယ်ဗျာ။" });
        }

        item.IsDelete = true;
        item.DeletedBy = request.DeletedBy ?? "Admin-Delete-User";
        item.DeletedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new ScheduleDeleteResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "ကားထွက်မည့် အချိန်စာရင်းကို စနစ်ထဲမှ အောင်မြင်စွာ ဖျက်သိမ်းပြီးပါပြီ။" : "အချိန်စာရင်း ဖျက်သိမ်းမှု မအောင်မြင်ပါ။"
        });
    }
}