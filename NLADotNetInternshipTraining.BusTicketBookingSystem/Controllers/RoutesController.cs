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

    [HttpGet]
    public IActionResult GetRoutes()
    {
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

    [HttpGet("{id}")]
    public IActionResult GetRoute(Guid id)
    {
        var item = _db.Routes.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound("သတ်မှတ်ထားသော ခရီးစဉ်လမ်းကြောင်းကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။");
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


    [HttpPost]
    public IActionResult CreateRoute(RouteCreateRequestModel request)
    {

        _db.Routes.Add(new Route
        {
            Id = Guid.NewGuid(),
            DepartureStation = request.DepartureStation,
            ArrivalStation = request.ArrivalStation,
            DistanceKm = request.DistanceKM,
            CreatedBy = request.CreatedBy ?? "Admin-Staff",
            IsDelete = false
        });


        int result = _db.SaveChanges();

        return StatusCode(201, new RouteCreateResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "ခရီးစဉ်လမ်းကြောင်းအသစ်ကို အောင်မြင်စွာ ထည့်သွင်းပြီးပါပြီ။" : "လမ်းကြောင်းအသစ် ထည့်သွင်းမှု မအောင်မြင်ပါ။"
        });
    }


    [HttpPut("{id}")]
    public IActionResult UpdateRoute(Guid id, RouteUpdateRequestModel request)
    {
        var item = _db.Routes.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new RouteUpdateResponseModel
            {
                IsSuccess = false,
                Message = "ပြုပြင်လိုသော ခရီးစဉ်လမ်းကြောင်းကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။"
            });
        }


        item.DepartureStation = request.DepartureStation;
        item.ArrivalStation = request.ArrivalStation;
        item.DistanceKm = request.DistanceKM;
        item.ModifiedBy = request.ModifiedBy ?? "Admin-Modifier";
        item.ModifiedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new RouteUpdateResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "ခရီးစဉ်လမ်းကြောင်းကို အောင်မြင်စွာ ပြုပြင်မွမ်းမံပြီးပါပြီ။" : "လမ်းကြောင်း ပြုပြင်မွမ်းမံမှု မအောင်မြင်ပါ။",
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


    [HttpPatch("{id}")]
    public IActionResult PatchRoute(Guid id, RoutePatchRequestModel request)
    {
        var item = _db.Routes.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new RoutePatchResponseModel
            {
                IsSuccess = false,
                Message = "ပြုပြင်လိုသော ခရီးစဉ်လမ်းကြောင်းကို စနစ်ထဲမှာ ရှာမတွေ့ပါဗျာ။"
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
                Message = "ပြုပြင်မွမ်းမံရန်အတွက် မည်သည့်အချက်အလက်မှ ထည့်သွင်းထားခြင်းမရှိပါဗျာ။"
            });
        }


        item.ModifiedBy = request.ModifiedBy ?? "Admin-Patch-User";
        item.ModifiedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new RoutePatchResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "ခရီးစဉ်လမ်းကြောင်းကို ကွက်တိ အောင်မြင်စွာ ပြုပြင်မွမ်းမံပြီးပါပြီ။" : "လမ်းကြောင်း ကွက်တိပြုပြင်မွမ်းမံမှု မအောင်မြင်ပါ။",
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


    [HttpDelete("{id}")]
    public IActionResult DeleteRoute(Guid id, [FromBody] RouteDeleteRequestModel request)
    {
        var item = _db.Routes.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
        if (item is null)
        {
            return NotFound(new RouteDeleteResponseModel
            {
                IsSuccess = false,
                Message = "သတ်မှတ်ထားသော ခရီးစဉ်လမ်းကြောင်းကို စနစ်ထဲမှာ ရှာမတွေ့ပါ သို့မဟုတ် ဖျက်သိမ်းပြီးသား ဖြစ်နေပါတယ်ဗျာ။"
            });
        }


        item.IsDelete = true;
        item.DeletedBy = request.DeletedBy ?? "Admin-Delete-User";
        item.DeletedAt = DateTime.Now;

        int result = _db.SaveChanges();

        return Ok(new RouteDeleteResponseModel
        {
            IsSuccess = result > 0,
            Message = result > 0 ? "ခရီးစဉ်လမ်းကြောင်းကို စနစ်ထဲမှ အောင်မြင်စွာ ဖျက်သိမ်းပြီးပါပြီ။" : "ခရီးစဉ်လမ်းကြောင်း ဖျက်သိမ်းမှု မအောင်မြင်ပါ။"
        });
    }
}