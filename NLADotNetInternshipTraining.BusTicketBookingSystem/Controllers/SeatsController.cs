using Microsoft.AspNetCore.Mvc;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Models;
namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Controllers;
[ApiController]
[Route("api/Bus/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly AppDbContext _db = new AppDbContext();
    [HttpGet("schedule/{scheduleId}")]
    public IActionResult GetSeatsBySchedule(Guid scheduleId)
    {
        var seatsList = _db.Seats
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

        if (!seatsList.Any())
        {
            return NotFound(new { isSuccess = false, message = "ဤခရီးစဉ်အတွက် ထိုင်ခုံဒေတာများ ရှာမတွေ့ပါဗျာ။" });
        }
        var response = new SeatSummaryResponseModel
        {
            TotalSeatsCount = seatsList.Count,
            BookedSeatsCount = seatsList.Count(s => s.IsBooked == true),
            AvailableSeatsCount = seatsList.Count(s => s.IsBooked == false),
            Seats = seatsList 
        };

        return Ok(response);
    }
    [HttpGet("available/{scheduleId}")]
    public IActionResult GetAvailableSeats(Guid scheduleId)
    {
        var allSeatsInDb = _db.Seats
            .Where(s => s.ScheduleId == scheduleId && s.IsDelete == false)
            .ToList();

        if (!allSeatsInDb.Any())
        {
            return NotFound(new { isSuccess = false, message = "ဤခရီးစဉ်အတွက် ထိုင်ခုံဒေတာများ ရှာမတွေ့ပါဗျာ။" });
        }

        var availableSeatsList = allSeatsInDb
            .Where(s => s.IsBooked == false)
            .Select(s => new SeatResponseModel
            {
                Id = s.Id,
                ScheduleId = s.ScheduleId,
                SeatNumber = s.SeatNumber,
                IsBooked = s.IsBooked,
                BookingId = s.BookingId
            })
            .ToList();

        var response = new SeatSummaryResponseModel
        {
            TotalSeatsCount = allSeatsInDb.Count, 
            BookedSeatsCount = allSeatsInDb.Count(s => s.IsBooked == true), 
            AvailableSeatsCount = availableSeatsList.Count, 
            Seats = availableSeatsList 
        };

        return Ok(response);
    }
}