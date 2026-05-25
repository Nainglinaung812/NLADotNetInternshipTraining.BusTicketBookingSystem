// using NLADotNetInternshipTraining.BusTicketBookingSystem.Database.AppDbContextModels;
// using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // .NET ရဲ့ အလိုအလျောက် Nullability စစ်ဆေးပြီး လည်ထွက်သွားတဲ့ စနစ်ကို ပိတ်ပစ်တာ ဖြစ်ပါတယ်
        options.JsonSerializerOptions.TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver
        {
            // ရလဒ်အနေနဲ့ အပေါ်က Error ကြီး လုံးဝ ပျောက်သွားပါလိမ့်မယ်
        };
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
