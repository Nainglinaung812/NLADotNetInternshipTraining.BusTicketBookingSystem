namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Models;

// ပင်မ ဒေတာပြသရန် Model
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

    public string? DepartureStation { get; set; } // ဥပမာ- Yangon
    public string? ArrivalStation { get; set; }   // ဥပမာ- Mandalay
    public decimal? DistanceKM { get; set; }
    public string? BusNumber { get; set; }        // ဥပမာ- YGN-VIP-8888
    public string? BusType { get; set; }          // ဥပမာ- Scania VIP (2+1)
}

// 1. Create Models
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

// 2. Update Models
public class ScheduleUpdateRequestModel
{
    public Guid RouteId { get; set; }
    public Guid BusId { get; set; }
    public DateTime DepartureTime { get; set; }
    public decimal TicketPrice { get; set; }
    public string? ModifiedBy { get; set; }
    // public string DepartureStation {get;set;}

    //     "departureStation": null,
    // "arrivalStation": null,
    // "distanceKM": null,
    // "busNumber": null,
    // "busType": null
}

public class ScheduleUpdateResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public ScheduleModel? Data { get; set; }
}

// 3. Patch Models
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

// 4. Delete Models
public class ScheduleDeleteRequestModel
{
    public string? DeletedBy { get; set; }
}

public class ScheduleDeleteResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}