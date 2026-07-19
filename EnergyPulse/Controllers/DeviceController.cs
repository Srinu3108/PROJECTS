using EnergyPulse.Api.DTOs;
using EnergyPulse.Data;
using EnergyPulse.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace EnergyPulse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DeviceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/device
        [HttpGet]
        public IActionResult GetDevices()
        {
            var latestReadings = _context.PowerReadings
                .OrderByDescending(r => r.Timestamp)
                .AsEnumerable()
                .GroupBy(r => r.DeviceId)
                .ToDictionary(g => g.Key, g => g.First());

            var devices = _context.Devices
                .ToList()
                .Select(d =>
                {
                    latestReadings.TryGetValue(d.Id, out var reading);
                    var efficiency = 100;

                    if (reading != null && reading.TargetOutputKW > 0)
                    {
                        efficiency = (int)Math.Round((reading.ActualOutputKW / reading.TargetOutputKW) * 100);
                    }
                    else if (d.Status == "Offline")
                    {
                        efficiency = 0;
                    }

                    return new DeviceStatusDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Status = d.Status,
                        Efficiency = Math.Clamp(efficiency, 0, 100),
                        Type = d.Name.Contains("Inverter", StringComparison.OrdinalIgnoreCase) ? "Solar Inverter" : "Sensor"
                    };
                })
                .ToList();

            return Ok(devices);
        }

        // POST: api/device
        [HttpPost]
        public IActionResult AddDevice(CreateDeviceDto dto)
        {
            var device = new Device
            {
                Name = dto.Name,
                Status = dto.Status,
                SiteId = dto.SiteId
            };

            _context.Devices.Add(device);
            _context.SaveChanges();

            return Ok("Device Added");
        }

        // PUT: api/device/5
        [HttpPut("{id}")]
        public IActionResult UpdateDevice(int id, Device updatedDevice)
        {
            var device = _context.Devices.Find(id);

            if (device == null)
                return NotFound();

            device.Name = updatedDevice.Name;
            device.Status = updatedDevice.Status;

            _context.SaveChanges();

            return Ok("Device Updated");
        }

        // DELETE: api/device/5
        [HttpDelete("{id}")]
        public IActionResult DeleteDevice(int id)
        {
            var device = _context.Devices.Find(id);

            if (device == null)
                return NotFound();

            _context.Devices.Remove(device);
            _context.SaveChanges();

            return Ok("Device Deleted");
        }
    }
}