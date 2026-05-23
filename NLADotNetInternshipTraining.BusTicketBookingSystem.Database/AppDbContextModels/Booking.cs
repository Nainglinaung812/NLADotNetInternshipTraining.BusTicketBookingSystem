using System;
using System.Collections.Generic;

namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;

public partial class Booking
{
    public Guid Id { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public DateTime? BookingDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsDelete { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
