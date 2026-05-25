namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Models;

public class ScheduleModel
{
    public Guid Id { get; set; }
    public Guid RouteId { get; set; }
    public Guid BusId { get; set; }
    public DateTime DepartureTime { get; set; }
    public decimal TicketPrice { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? DepartureStation { get; set; }
    public string? ArrivalStation { get; set; }
    public decimal? DistanceKM { get; set; }
    public string? BusNumber { get; set; }
    public string? BusType { get; set; }
    public int TotalSeatsCount { get; set; }      
    public int BookedSeatsCount { get; set; }     
    public int AvailableSeatsCount { get; set; }
}
public class ScheduleCreateRequestModel
{
    public Guid RouteId { get; set; }
    public Guid BusId { get; set; }
    public DateTime DepartureTime { get; set; }
    public decimal TicketPrice { get; set; }
    public string? CreatedBy { get; set; }
}

public class ScheduleCreateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}


public class ScheduleUpdateRequestModel
{
    public Guid RouteId { get; set; }
    public Guid BusId { get; set; }
    public DateTime DepartureTime { get; set; }
    public decimal TicketPrice { get; set; }
    public string? ModifiedBy { get; set; }
}

public class ScheduleUpdateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public ScheduleModel? Data { get; set; }
}


public class SchedulePatchRequestModel
{
    public Guid? RouteId { get; set; }
    public Guid? BusId { get; set; }
    public DateTime? DepartureTime { get; set; }
    public decimal? TicketPrice { get; set; }
    public string? ModifiedBy { get; set; }
}

public class SchedulePatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public ScheduleModel? Data { get; set; }
}


public class ScheduleDeleteRequestModel
{
    public string? DeletedBy { get; set; }
}

public class ScheduleDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}
public class ScheduleSearchResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}