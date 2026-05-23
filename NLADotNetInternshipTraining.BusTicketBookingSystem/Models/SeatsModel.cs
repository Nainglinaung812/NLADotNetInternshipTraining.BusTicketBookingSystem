namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Models;

public class SeatResponseModel
{
    public int Id { get; set; }
    public Guid ScheduleId { get; set; }
    public string SeatNumber { get; set; } = null!;
    public bool IsBooked { get; set; }
    public Guid? BookingId { get; set; }
}