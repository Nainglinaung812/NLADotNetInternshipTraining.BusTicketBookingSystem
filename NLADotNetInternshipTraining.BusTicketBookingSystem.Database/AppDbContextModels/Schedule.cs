using System;
using System.Collections.Generic;

namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;

public partial class Schedule
{
    public Guid Id { get; set; }

    public Guid RouteId { get; set; }

    public Guid BusId { get; set; }

    public DateTime DepartureTime { get; set; }

    public decimal TicketPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsDelete { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual Bus Bus { get; set; } = null!;

    public virtual Route Route { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
