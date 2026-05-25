namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Models;

public class SeatResponseModel
{
    public int Id { get; set; }
    public Guid ScheduleId { get; set; }
    public string SeatNumber { get; set; } = null!;
    public bool IsBooked { get; set; }
    public Guid? BookingId { get; set; }
    
}
public class SeatSummaryResponseModel
{
    public int TotalSeatsCount { get; set; }
    public int BookedSeatsCount { get; set; }
    public int AvailableSeatsCount { get; set; }
    public List<SeatResponseModel> Seats { get; set; } = new();
}
