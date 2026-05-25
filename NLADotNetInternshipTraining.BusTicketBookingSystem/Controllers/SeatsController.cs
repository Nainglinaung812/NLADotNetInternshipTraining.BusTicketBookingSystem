using Microsoft.AspNetCore.Mvc;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Models;

namespace NLADotNetInternshipTraining.WebApi.Controllers;

[ApiController]
[Route("api/Bus/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly AppDbContext _db = new AppDbContext();


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


    [HttpGet("available/{scheduleId}")]
    public IActionResult GetAvailableSeats(Guid scheduleId)
    {
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