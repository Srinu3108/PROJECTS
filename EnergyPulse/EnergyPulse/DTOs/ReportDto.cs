using System;
using System.Collections.Generic;

namespace EnergyPulse.DTOs
{
    public class ReportDto
    {
        public string Site { get; set; } = string.Empty;
        public int TotalDevices { get; set; }
        public int ActiveDevices { get; set; }
        public double AverageEfficiency { get; set; }
        public double SystemPerformanceIndex { get; set; }
        public int TotalAlerts { get; set; }
        public int HighAlerts { get; set; }
        public DateTime GeneratedAt { get; set; }
        public List<DeviceIssueDto> UnderperformingDevices { get; set; } = new();
        public int MaintenanceIssues => UnderperformingDevices.Count;
    }

    public class DeviceIssueDto
    {
        public string DeviceName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double PerformanceIndex { get; set; }
        public string Issue { get; set; } = string.Empty;
    }
}
