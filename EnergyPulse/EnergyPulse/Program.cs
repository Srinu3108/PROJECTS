using EnergyPulse.Models;
using EnergyPulse.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=SREENU\\SQLEXPRESS;Database=EnergyPulseDb;Trusted_Connection=True;TrustServerCertificate=True"));
builder.Services.AddScoped<DashboardService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AlertService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Sites.Any())
    {
        var site = new Site
        {
            Name = "Solar Plant Alpha",
            Location = "Chennai",
            TotalCapacityKW = 5000
        };

        context.Sites.Add(site);
        context.SaveChanges();

        var device1 = new Device { Name = "Inverter 1", Status = "Active", SiteId = site.Id };
        var device2 = new Device { Name = "Inverter 2", Status = "Active", SiteId = site.Id };
        var device3 = new Device { Name = "Inverter 3", Status = "Offline", SiteId = site.Id };

        context.Devices.AddRange(device1, device2, device3);
        context.SaveChanges();

        context.PowerReadings.AddRange(
            new PowerReading { DeviceId = device1.Id, Timestamp = DateTime.Now, ActualOutputKW = 400, TargetOutputKW = 500 },
            new PowerReading { DeviceId = device2.Id, Timestamp = DateTime.Now, ActualOutputKW = 450, TargetOutputKW = 500 },
            new PowerReading { DeviceId = device3.Id, Timestamp = DateTime.Now, ActualOutputKW = 0, TargetOutputKW = 500 }
        );

        context.Alerts.Add(
            new Alert
            {
                DeviceId = device3.Id,
                Message = "Device Offline",
                CreatedAt = DateTime.Now,
                Severity = "High"   
            });
        context.SaveChanges();
    }
}

app.Run();
