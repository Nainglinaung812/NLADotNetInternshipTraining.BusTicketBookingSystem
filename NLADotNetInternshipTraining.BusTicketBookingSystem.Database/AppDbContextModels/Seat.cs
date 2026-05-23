using System;
using System.Collections.Generic;

namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;

public partial class Seat
{
    public int Id { get; set; }

    public Guid ScheduleId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public bool IsBooked { get; set; }

    public Guid? BookingId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsDelete { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Schedule Schedule { get; set; } = null!;
}
