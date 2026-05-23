namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Models;


public class BookingModel
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public DateTime BookingDate { get; set; }
    public string? CreatedBy { get; set; }

    // 🔥 ဒီ Voucher အောက်မှာ ဝယ်ထားတဲ့ ထိုင်ခုံနံပါတ်များစာရင်း (ဥပမာ- ["A1", "A2"])
    public List<string> BookedSeats { get; set; } = new();
}
// ခရီးသည်ဆီကနေ ယူမယ့် အချက်အလက်များ
public class BookingCreateRequestModel
{
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    
    // 🔥 ScheduleId နေရာမှာ ခရီးသည် သိနိုင်မည့် ဒေတာများကို တောင်းလိုက်ခြင်း
    public string DepartureStation { get; set; } = null!; // ဥပမာ- Yangon
    public string ArrivalStation { get; set; } = null!;   // ဥပမာ- Mandalay
    public string BusNumber { get; set; } = null!;        // ဥပမာ- YGN-VIP-8888
    
    public List<string> SeatNumbers { get; set; } = new(); 
    public string? CreatedBy { get; set; }
}

public class BookingResponseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public Guid? BookingId { get; set; }
}