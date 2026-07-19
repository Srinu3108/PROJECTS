using EnergyPulse.Models;
using EnergyPulse.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<PerformanceService>();
builder.Services.AddLogging();

builder.Services.AddControllers();

// Add JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Register AuthService
builder.Services.AddScoped<AuthService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
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

// 🔥 ADD THIS - Logging middleware to see what's happening
app.Use(async (context, next) =>
{
    Console.WriteLine($"📡 Request: {context.Request.Method} {context.Request.Path}");
    await next();
});

app.UseCors("AllowAll");
app.UseAuthentication();
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
            Location = "Chennai, India",
            Type = "Solar",
            TotalCapacityKW = 5000,
            EstablishedDate = DateTime.UtcNow.AddYears(-2),
            PerformanceTarget = 85.0
        };

        context.Sites.Add(site);
        context.SaveChanges();

        var device1 = new Device
        {
            Name = "Inverter 1",
            Status = "Active",
            DeviceType = "Inverter",
            SiteId = site.Id,
            Capacity = 2000,
            InstalledDate = DateTime.UtcNow.AddYears(-2),
            LastMaintenanceDate = DateTime.UtcNow.AddDays(-45)
        };
        var device2 = new Device
        {
            Name = "Inverter 2",
            Status = "Active",
            DeviceType = "Inverter",
            SiteId = site.Id,
            Capacity = 2000,
            InstalledDate = DateTime.UtcNow.AddYears(-2),
            LastMaintenanceDate = DateTime.UtcNow.AddDays(-30)
        };
        var device3 = new Device
        {
            Name = "Inverter 3",
            Status = "Fault",
            DeviceType = "Inverter",
            SiteId = site.Id,
            Capacity = 1000,
            InstalledDate = DateTime.UtcNow.AddYears(-1),
            LastMaintenanceDate = DateTime.UtcNow.AddDays(-120)
        };

        context.Devices.AddRange(device1, device2, device3);
        context.SaveChanges();

        context.PowerReadings.AddRange(
            new PowerReading { DeviceId = device1.Id, Timestamp = DateTime.UtcNow, ActualOutputKW = 1800, TargetOutputKW = 2000 },
            new PowerReading { DeviceId = device2.Id, Timestamp = DateTime.UtcNow, ActualOutputKW = 1850, TargetOutputKW = 2000 },
            new PowerReading { DeviceId = device3.Id, Timestamp = DateTime.UtcNow, ActualOutputKW = 300, TargetOutputKW = 1000 }
        );

        context.Alerts.AddRange(
            new Alert
            {
                DeviceId = device1.Id,
                AlertType = "Performance",
                Message = "Device efficiency below target",
                CreatedAt = DateTime.UtcNow,
                Severity = "Medium",
                Status = "Open"
            },
            new Alert
            {
                DeviceId = device3.Id,
                AlertType = "Performance",
                Message = "Critical efficiency drop detected",
                CreatedAt = DateTime.UtcNow,
                Severity = "Critical",
                Status = "Open",
                ImpactKW = 700,
                EstimatedCost = 35000
            }
        );

        context.SaveChanges();
    }
}

// 🔥 ADD THIS - Health check endpoint before app.Run()
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    database = "Connected"
}));

app.Run();
