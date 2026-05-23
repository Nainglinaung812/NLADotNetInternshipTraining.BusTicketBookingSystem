using Microsoft.AspNetCore.Mvc;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;
using NLADotNetInternshipTraining.BusTicketBookingSystem.Models;
using Route = NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels.Route;

namespace NLADotNetInternshipTraining.BusTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly AppDbContext _db = new AppDbContext();
    // ၁။ GET: api/Routes (အရှင်ရှိနေသည့် လမ်းကြောင်းအားလုံးပြရန်)
    [HttpGet]
    public IActionResult GetRoutes()
    {
        // Soft Delete (IsDelete == false) ဖြစ်နေတာတွေကိုပဲ ယူမယ်
        var lst = _db.Routes
            .Where(x => x.IsDelete == false)
            .Select(x => new RouteModel
            {
                Id = x.Id,
                DepartureStation = x.DepartureStation,
                ArrivalStation = x.ArrivalStation,
                DistanceKM = x.DistanceKm,
                CreatedBy = x.CreatedBy,
                CreatedAt = Convert.ToDateTime(x.CreatedAt),
                ModifiedBy = x.ModifiedBy,
                ModifiedAt = x.ModifiedAt
            })
            .ToList();

        return Ok(lst);
    }

    // ၂။ GET: api/Routes/{id} (သီးသန့် လမ်းကြောင်းတစ်ခုကို ရှာဖွေရန်)
    [HttpGet("{id}")]
    public IActionResult GetRoute(Guid id)
    {
        var item = _db.Routes.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound("Routes not found");
        }

        var routeData = new RouteModel
        {
            Id = item.Id,
            DepartureStation = item.DepartureStation,
            ArrivalStation = item.ArrivalStation,
            DistanceKM = item.DistanceKm,
            CreatedBy = item.CreatedBy,
            CreatedAt = Convert.ToDateTime(item.CreatedAt),
            ModifiedBy = item.ModifiedBy,
            ModifiedAt = item.ModifiedAt
        };

        return Ok(routeData);
    }

    // ၃။ POST: api/Routes (လမ်းကြောင်းအသစ်ဆောက်ရန်)
    [HttpPost]
    public IActionResult CreateRoute(RouteCreateRequestModel request)
    {

        _db.Routes.Add(new Route
        {
            Id = Guid.NewGuid(), // UNIQUEIDENTIFIER မို့လို့ New Guid ထုတ်ပေးရပါမယ်
            DepartureStation = request.DepartureStation,
            ArrivalStation = request.ArrivalStation,
            DistanceKm = request.DistanceKM,
            CreatedBy = request.CreatedBy ?? "Admin-Staff",
            IsDelete = false // Default အရှင်ဆောက်ခြင်း
        });


        int result = _db.SaveChanges();

        return StatusCode(201, new RouteCreateResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "Saving Route Successful" : "Saving Route Failed"
        });
    }

    // ၄။ PUT: api/Routes/{id} (လမ်းကြောင်းတစ်ခုလုံးကို အစားထိုးပြင်ရန်)
    [HttpPut("{id}")]
    public IActionResult UpdateRoute(Guid id, RouteUpdateRequestModel request)
    {
        var item = _db.Routes.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new RouteUpdateResponseModel
            {
                IsSuccess = false,
                Message = "Route not found"
            });
        }

        // ဒေတာတစ်ခုလုံးကို အစားထိုးပြင်ဆင်ခြင်း
        item.DepartureStation = request.DepartureStation;
        item.ArrivalStation = request.ArrivalStation;
        item.DistanceKm = request.DistanceKM;
        item.ModifiedBy = request.ModifiedBy ?? "Admin-Modifier";
        item.ModifiedAt = DateTime.Now; // မြန်မာစံတော်ချိန် ဝင်ပါမည်

        int result = _db.SaveChanges();

        return Ok(new RouteUpdateResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "Route Update Successfully" : "Route Update Failed",
            Data = new RouteModel
            {
                Id = item.Id,
                DepartureStation = item.DepartureStation,
                ArrivalStation = item.ArrivalStation,
                DistanceKM = item.DistanceKm,
                CreatedBy = item.CreatedBy,
                CreatedAt = Convert.ToDateTime(item.CreatedAt),
                ModifiedBy = item.ModifiedBy,
                ModifiedAt = item.ModifiedAt
            }
        });
    }

    // ၅။ HTTP PATCH: api/Routes/{id} (ကော်လံတစ်ခုချင်းစီကို ကွက်တိ Patch ရန်)
    [HttpPatch("{id}")]
    public IActionResult PatchRoute(Guid id, RoutePatchRequestModel request)
    {
        var item = _db.Routes.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new RoutePatchResponseModel
            {
                IsSuccess = false,
                Message = "Route not found"
            });
        }

        int count = 0;
        if (!string.IsNullOrEmpty(request.DepartureStation))
        {
            count++;
            item.DepartureStation = request.DepartureStation;
        }
        if (!string.IsNullOrEmpty(request.ArrivalStation))
        {
            count++;
            item.ArrivalStation = request.ArrivalStation;
        }
        if (request.DistanceKM.HasValue && request.DistanceKM.Value > 0)
        {
            count++;
            item.DistanceKm = request.DistanceKM.Value;
        }

        if (count == 0)
        {
            return BadRequest(new RoutePatchResponseModel
            {
                IsSuccess = false,
                Message = "No fields need to update"
            });
        }

        // ပြင်ဆင်မှု ခြေရာခံခြင်း
        item.ModifiedBy = request.ModifiedBy ?? "Admin-Patch-User";
        item.ModifiedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new RoutePatchResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "Route Patch Successfully" : "Route Patch Failed",
            Data = new RouteModel
            {
                Id = item.Id,
                DepartureStation = item.DepartureStation,
                ArrivalStation = item.ArrivalStation,
                DistanceKM = item.DistanceKm,
                CreatedBy = item.CreatedBy,
                CreatedAt = Convert.ToDateTime(item.CreatedAt),
                ModifiedBy = item.ModifiedBy,
                ModifiedAt = item.ModifiedAt
            }
        });
    }

    // ၆။ DELETE: api/Routes/{id} (ဆရာ့လိုအပ်ချက်အရ အပြီးမဖျက်ဘဲ Soft Delete လုပ်ရန်)
    [HttpDelete("{id}")]
    public IActionResult DeleteRoute(Guid id, [FromBody] RouteDeleteRequestModel request)
    {
        var item = _db.Routes.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new RouteDeleteResponseModel
            {
                IsSuccess = false,
                Message = "Route not found or already deleted"
            });
        }

        // ဒေတာဘေ့စ်ထဲမှ အပြီးမဖြုတ်ပါဘူး၊ Soft Delete အနေနဲ့ပဲ အလံထောင်ကွယ်ပါမယ်
        item.IsDelete = true;
        item.DeletedBy = request.DeletedBy ?? "Admin-Delete-User";
        item.DeletedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new RouteDeleteResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "Route Soft-Deleted Successfully" : "Route Soft-Delete Failed"
        });
    }
}