using System;
using System.Collections.Generic;

namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;

public partial class Route
{
    public Guid Id { get; set; }

    public string DepartureStation { get; set; } = null!;

    public string ArrivalStation { get; set; } = null!;

    public decimal DistanceKm { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsDelete { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
