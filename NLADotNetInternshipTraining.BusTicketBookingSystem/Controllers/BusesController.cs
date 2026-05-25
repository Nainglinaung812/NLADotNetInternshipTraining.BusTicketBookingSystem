using Microsoft.AspNetCore.Mvc;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Models;
namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BusesController : ControllerBase
{
    private readonly AppDbContext _db = new AppDbContext();
    [HttpGet]
    public IActionResult GetBuses()
    {
        var lst = _db.Buses
            .Where(x => x.IsDelete == false)
            .Select(x => new BusModel
            {
                Id = x.Id,
                BusNumber = x.BusNumber,
                TotalSeats = x.TotalSeats,
                BusType = x.BusType,
                CreatedBy = x.CreatedBy,
                CreatedAt = Convert.ToDateTime(x.CreatedAt),
                ModifiedBy = x.ModifiedBy,
                ModifiedAt = x.ModifiedAt
            })
            .ToList();
        if (lst is null || !lst.Any())
        {
            return NotFound("အဝေးပြေးကားများကို စနစ်ထဲမှာ ရှာမတွေ့ပါ သို့မဟုတ် ဖျက်သိမ်းထားပြီး ဖြစ်ပါတယ်ဗျာ။");
        }

        return Ok(lst);
    }

    [HttpGet("{id}")]
    public IActionResult GetBus(Guid id)
    {
        var item = _db.Buses.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound("တောင်းဆိုထားသော ကားစာရင်း မရှိပါ သို့မဟုတ် ဖျက်ထားပြီးဖြစ်ပါသည်။");
        }

        var busData = new BusModel
        {
            Id = item.Id,
            BusNumber = item.BusNumber,
            TotalSeats = item.TotalSeats,
            BusType = item.BusType,
            CreatedBy = item.CreatedBy,
            CreatedAt = Convert.ToDateTime(item.CreatedAt),
            ModifiedBy = item.ModifiedBy,
            ModifiedAt = item.ModifiedAt
        };

        return Ok(busData);
    }

    [HttpPost]
    public IActionResult CreateBus(BusCreateRequestModel request)
    {

        var isDuplicate = _db.Buses.Any(x => x.BusNumber == request.BusNumber && x.IsDelete == false);
        if (isDuplicate)
        {
            return BadRequest(new BusCreateResponseModel { IsSuccess = false, Message = "ဒီကားနံပါတ်က စနစ်ထဲမှာ ရှိနေပြီးသား ဖြစ်ပါတယ်ဗျာ။" });
        }
        var newBus = new Bus
        {
            Id = Guid.NewGuid(),
            BusNumber = request.BusNumber,
            TotalSeats = request.TotalSeats,
            BusType = request.BusType,
            CreatedBy = request.CreatedBy ?? "Admin-Staff",
            IsDelete = false
        };

        _db.Buses.Add(newBus);
        int result = _db.SaveChanges();

        return StatusCode(201, new BusCreateResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "ကားအချက်အလက်ကို အောင်မြင်စွာ သိမ်းဆည်းပြီးပါပြီ။" : "ကားအချက်အလက် သိမ်းဆည်းမှု မအောင်မြင်ပါ။"
        });
    }

    [HttpPut("{id}")]
    public IActionResult UpdateBus(Guid id, BusUpdateRequestModel request)
    {
        var item = _db.Buses.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new BusUpdateResponseModel { IsSuccess = false, Message = "ပြုပြင်လိုသော အဝေးပြေးကားကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။" });
        }

        item.BusNumber = request.BusNumber;
        item.TotalSeats = request.TotalSeats;
        item.BusType = request.BusType;
        item.ModifiedBy = request.ModifiedBy ?? "Admin-Modifier";
        item.ModifiedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new BusUpdateResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "ကားအချက်အလက်ကို အောင်မြင်စွာ ပြုပြင်မွမ်းမံပြီးပါပြီ။" : "ကားအချက်အလက် ပြုပြင်မွမ်းမံမှု မအောင်မြင်ပါ။",
            Data = new BusModel
            {
                Id = item.Id,
                BusNumber = item.BusNumber,
                TotalSeats = item.TotalSeats,
                BusType = item.BusType,
                CreatedBy = item.CreatedBy,
                CreatedAt = Convert.ToDateTime(item.CreatedAt),
                ModifiedBy = item.ModifiedBy,
                ModifiedAt = item.ModifiedAt
            }
        });
    }

    [HttpPatch("{id}")]
    public IActionResult PatchBus(Guid id, BusPatchRequestModel request)
    {
        var item = _db.Buses.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new BusPatchResponseModel { IsSuccess = false, Message = "ပြုပြင်လိုသော အဝေးပြေးကားကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။" });
        }

        int count = 0;
        if (!string.IsNullOrEmpty(request.BusNumber))
        {
            count++;
            item.BusNumber = request.BusNumber;
        }
        if (!string.IsNullOrEmpty(request.BusType))
        {
            count++;
            item.BusType = request.BusType;
        }
        if (request.TotalSeats.HasValue && request.TotalSeats.Value > 0)
        {
            count++;
            item.TotalSeats = request.TotalSeats.Value;
        }

        if (count == 0)
        {
            return BadRequest(new BusPatchResponseModel { IsSuccess = false, Message = "ပြုပြင်မွမ်းမံရန်အတွက် မည်သည့်အချက်အလက်မှ ထည့်သွင်းထားခြင်းမရှိပါဗျာ။" });
        }

        item.ModifiedBy = request.ModifiedBy ?? "Admin-Patch-User";
        item.ModifiedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new BusPatchResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "ကားအချက်အလက်ကို ကွက်တိ အောင်မြင်စွာ ပြုပြင်မွမ်းမံပြီးပါပြီ။" : "ကားအချက်အလက် ကွက်တိပြုပြင်မွမ်းမံမှု မအောင်မြင်ပါ။",
            Data = new BusModel
            {
                Id = item.Id,
                BusNumber = item.BusNumber,
                TotalSeats = item.TotalSeats,
                BusType = item.BusType,
                CreatedBy = item.CreatedBy,
                CreatedAt = Convert.ToDateTime(item.CreatedAt),
                ModifiedBy = item.ModifiedBy,
                ModifiedAt = item.ModifiedAt
            }
        });
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteBus(Guid id, BusDeleteRequestModel request)
    {
        var item = _db.Buses.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new BusDeleteResponseModel { IsSuccess = false, Message = "သတ်မှတ်ထားသော အဝေးပြေးကားကို စနစ်ထဲမှာ ရှာမတွေ့ပါ သို့မဟုတ် ဖျက်သိမ်းပြီးသား ဖြစ်နေပါတယ်ဗျာ။" });
        }

        item.IsDelete = true;
        item.DeletedBy = request.DeletedBy ?? "Admin-Delete-User";
        item.DeletedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new BusDeleteResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "ကားအချက်အလက်ကို စနစ်ထဲမှ အောင်မြင်စွာ ဖျက်သိမ်းပြီးပါပြီ။" : "ကားအချက်အလက် ဖျက်သိမ်းမှု မအောင်မြင်ပါ။"
        });
    }
}