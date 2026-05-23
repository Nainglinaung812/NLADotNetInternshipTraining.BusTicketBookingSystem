using Microsoft.AspNetCore.Mvc;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Models;

namespace NLADotNetInternshipTraining.WebApi.Controllers;

[ApiController]
// 🔥 Route ကို api/Bus/Seats ဖြစ်သွားအောင် ဒီလို ပြောင်းလိုက်ပါတယ်ဗျာ
[Route("api/Bus/[controller]")] 
public class SeatsController : ControllerBase
{
    private readonly AppDbContext _db = new AppDbContext();

    // ===================================================================
    // ၁။ GET: api/Bus/Seats/schedule/{scheduleId} 
    // (ခရီးစဉ်တစ်ခုလုံး၏ ခုံ Layout အားလုံး - ဝယ်ပြီး/မဝယ်ရသေး ပြရန်)
    // ===================================================================
    [HttpGet("schedule/{scheduleId}")]
    public IActionResult GetSeatsBySchedule(Guid scheduleId)
    {
        var seats = _db.Seats
            .Where(s => s.ScheduleId == scheduleId && s.IsDelete == false)
            .Select(s => new SeatResponseModel
            {
                Id = s.Id,
                ScheduleId = s.ScheduleId,
                SeatNumber = s.SeatNumber,
                IsBooked = s.IsBooked,
                BookingId = s.BookingId
            })
            .ToList();

        if (!seats.Any())
        {
            return NotFound(new { isSuccess = false, message = "ဤခရီးစဉ်အတွက် ထိုင်ခုံဒေတာများ ရှာမတွေ့ပါဗျာ။" });
        }

        return Ok(seats);
    }

    // ===================================================================
    // ၂။ GET: api/Bus/Seats/available/{scheduleId}
    // (ခရီးစဉ်အလိုက် မဝယ်ရသေးဘဲ လွတ်နေသည့် ခုံစာရင်း သီးသန့်ပြရန်)
    // ===================================================================
    [HttpGet("available/{scheduleId}")]
    public IActionResult GetAvailableSeats(Guid scheduleId)
    {
        // 🔥 IsBooked == false ဖြစ်တဲ့ လွတ်နေတဲ့ ခုံတွေကိုပဲ Filter ဖြတ်ထုတ်မယ်
        var availableSeats = _db.Seats
            .Where(s => s.ScheduleId == scheduleId && s.IsBooked == false && s.IsDelete == false)
            .Select(s => new SeatResponseModel
            {
                Id = s.Id,
                ScheduleId = s.ScheduleId,
                SeatNumber = s.SeatNumber,
                IsBooked = s.IsBooked,
                BookingId = s.BookingId
            })
            .ToList();

        return Ok(availableSeats);
    }
}