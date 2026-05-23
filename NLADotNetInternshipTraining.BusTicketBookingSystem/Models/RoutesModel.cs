namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Models;

// ပင်မ ဒေတာပြသရန် Model
public class RouteModel
{
    public Guid Id { get; set; }
    public string DepartureStation { get; set; } = null!;
    public string ArrivalStation { get; set; } = null!;
    public decimal DistanceKM { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

// 1. Create Models
public class RouteCreateRequestModel
{
    public string DepartureStation { get; set; } = null!;
    public string ArrivalStation { get; set; } = null!;
    public decimal DistanceKM { get; set; }
    public string? CreatedBy { get; set; }
}

public class RouteCreateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}

// 2. Update Models
public class RouteUpdateRequestModel
{
    public string DepartureStation { get; set; } = null!;
    public string ArrivalStation { get; set; } = null!;
    public decimal DistanceKM { get; set; }
    public string? ModifiedBy { get; set; }
}

public class RouteUpdateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public RouteModel? Data { get; set; }
}

// 3. Patch Models
public class RoutePatchRequestModel
{
    public string? DepartureStation { get; set; }
    public string? ArrivalStation { get; set; }
    public decimal? DistanceKM { get; set; }
    public string? ModifiedBy { get; set; }
}

public class RoutePatchResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public RouteModel? Data { get; set; }
}

// 4. Delete Models
public class RouteDeleteRequestModel
{
    public string? DeletedBy { get; set; }
}

public class RouteDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}