namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Models;


public class BookingModel
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public DateTime BookingDate { get; set; }
    public string? CreatedBy { get; set; }

    public List<string> BookedSeats { get; set; } = new();
}
public class BookingCreateRequestModel
{
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    
    public string DepartureStation { get; set; } = null!; 
    public string ArrivalStation { get; set; } = null!;   
    public string BusNumber { get; set; } = null!;        
    
    public List<string> SeatNumbers { get; set; } = new(); 
    public string? CreatedBy { get; set; }
}

public class BookingResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public Guid? BookingId { get; set; }
}