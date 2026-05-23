using Microsoft.AspNetCore.Mvc;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Models;


namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusesController : ControllerBase
{
    private readonly AppDbContext _db = new AppDbContext();



    // ၁။ GET: api/Buses (အသုံးပြုနေဆဲ ကားစာရင်းအားလုံး ထုတ်ပြရန်)
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

        return Ok(lst);
    }

    // ၂။ GET: api/Buses/{id} (ကားတစ်စီးချင်းစီ၏ Specifications ကို ကြည့်ရန်)
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

    // ၃။ POST: api/Buses (ကားအသစ်စာရင်းသွင်းရန်)
    [HttpPost]
    public IActionResult CreateBus(BusCreateRequestModel request)
    {
        // ကားနံပါတ် တူနေရင် ပေးမသွင်းဘဲ ကာကွယ်ရန် (Unique Validation)
        var isDuplicate = _db.Buses.Any(x => x.BusNumber == request.BusNumber && x.IsDelete == false);
        if (isDuplicate)
        {
            return BadRequest(new BusCreateResponseModel { IsSuccess = false, Message = "ဒီကားနံပါတ်က စနစ်ထဲမှာ ရှိနေပြီးသား ဖြစ်ပါတယ်ဗျာ။" });
        }

        // Entity Framework Core Model က အနည်းကိန်း 'Bus' ဖြစ်နေတာ သတိပြုပါ ညီလေး
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
            Message = result > 0 ? "Saving Bus Successful" : "Saving Bus Failed"
        });
    }

    // ၄။ PUT: api/Buses/{id} (ကားအချက်အလက်အားလုံးကို အစအဆုံး အစားထိုးပြင်ရန်)
    [HttpPut("{id}")]
    public IActionResult UpdateBus(Guid id, BusUpdateRequestModel request)
    {
        var item = _db.Buses.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new BusUpdateResponseModel { IsSuccess = false, Message = "Bus not found" });
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
            Message = result > 0 ? "Bus Update Successfully" : "Bus Update Failed",
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

    // ၅။ PATCH: api/Buses/{id} (ကားနံပါတ် သို့မဟုတ် Specification တစ်ခုတည်းကို ကွက်တိပြင်ရန်)
    [HttpPatch("{id}")]
    public IActionResult PatchBus(Guid id, BusPatchRequestModel request)
    {
        var item = _db.Buses.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new BusPatchResponseModel { IsSuccess = false, Message = "Bus not found" });
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
            return BadRequest(new BusPatchResponseModel { IsSuccess = false, Message = "No fields need to update" });
        }

        item.ModifiedBy = request.ModifiedBy ?? "Admin-Patch-User";
        item.ModifiedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new BusPatchResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "Bus Patch Successfully" : "Bus Patch Failed",
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

    // ၆။ DELETE: api/Buses/{id} (ကားအိုကြီးများကို Soft Delete လုပ်ရန်)
    [HttpDelete("{id}")]
    public IActionResult DeleteBus(Guid id, BusDeleteRequestModel request)
    {
        var item = _db.Buses.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new BusDeleteResponseModel { IsSuccess = false, Message = "Bus not found or already deleted" });
        }

        item.IsDelete = true;
        item.DeletedBy = request.DeletedBy ?? "Admin-Delete-User";
        item.DeletedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new BusDeleteResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "Bus Soft-Deleted Successfully" : "Bus Soft-Delete Failed"
        });
    }
}