namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Models;

public class BusModel
{
    public Guid Id { get; set; }
    public string BusNumber { get; set; } = null!;
    public int TotalSeats { get; set; }
    public string BusType { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

public class BusCreateRequestModel
{
    public string BusNumber { get; set; } = null!;
    public int TotalSeats { get; set; }
    public string BusType { get; set; } = null!;
    public string? CreatedBy { get; set; }
}

public class BusCreateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}

public class BusUpdateRequestModel
{
    public string BusNumber { get; set; } = null!;
    public int TotalSeats { get; set; }
    public string BusType { get; set; } = null!;
    public string? ModifiedBy { get; set; }
}

public class BusUpdateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public BusModel? Data { get; set; }
}

public class BusPatchRequestModel
{
    public string? BusNumber { get; set; }
    public int? TotalSeats { get; set; }
    public string? BusType { get; set; }
    public string? ModifiedBy { get; set; }
}

public class BusPatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public BusModel? Data { get; set; }
}

public class BusDeleteRequestModel
{
    public string? DeletedBy { get; set; }
}

public class BusDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}