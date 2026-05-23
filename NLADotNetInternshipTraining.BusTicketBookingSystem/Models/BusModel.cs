namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Models;

// ပင်မ ဒေတာပြသရန် Model
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

// 1. Create Models
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

// 2. Update Models
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

// 3. Patch Models
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

// 4. Delete Models
public class BusDeleteRequestModel
{
    public string? DeletedBy { get; set; }
}

public class BusDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}