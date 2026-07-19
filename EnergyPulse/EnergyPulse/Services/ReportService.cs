using EnergyPulse.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace EnergyPulse.Services
{
    public class ReportService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReportService> _logger;

        public ReportService(AppDbContext context, ILogger<ReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Generate a comprehensive maintenance report for a site
        /// </summary>
        public async Task<byte[]> GenerateMaintenanceReportPdfAsync(int siteId)
        {
            try
            {
                var site = await _context.Sites
                    .Include(s => s.Devices)
                    .ThenInclude(d => d.Readings)
                    .Include(s => s.Devices)
                    .ThenInclude(d => d.Alerts)
                    .FirstOrDefaultAsync(s => s.Id == siteId);

                if (site == null)
                    throw new Exception($"Site with ID {siteId} not found");

                using (var memoryStream = new MemoryStream())
                {
                    var writer = new PdfWriter(memoryStream);
                    var pdfDocument = new PdfDocument(writer);
                    var document = new Document(pdfDocument);

                    // Title
                    document.Add(new Paragraph($"Maintenance Report - {site.Name}")
                        .SetFontSize(24)
                        .SetBold()
                        .SetMarginBottom(5));

                    document.Add(new Paragraph($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}")
                        .SetFontSize(10)
                        .SetMarginBottom(20));

                    // Site Summary
                    document.Add(new Paragraph("SITE SUMMARY")
                        .SetFontSize(14)
                        .SetBold()
                        .SetMarginTop(15)
                        .SetMarginBottom(10));

                    var siteTable = new Table(2)
                        .SetWidth(UnitValue.CreatePercentValue(100));

                    AddTableCell(siteTable, "Location:", site.Location);
                    AddTableCell(siteTable, "Type:", site.Type);
                    AddTableCell(siteTable, "Total Capacity:", $"{site.TotalCapacityKW:F2} KW");
                    AddTableCell(siteTable, "Performance Target:", $"{site.PerformanceTarget:F2}%");
                    AddTableCell(siteTable, "Established:", site.EstablishedDate.ToString("yyyy-MM-dd"));

                    document.Add(siteTable);

                    // Performance Metrics
                    document.Add(new Paragraph("PERFORMANCE METRICS")
                        .SetFontSize(14)
                        .SetBold()
                        .SetMarginTop(20)
                        .SetMarginBottom(10));

                    var avgEfficiency = site.Devices.Any(d => d.Readings.Any())
                        ? site.Devices
                            .Where(d => d.Readings.Any())
                            .Average(d => d.Readings.Average(r => (r.ActualOutputKW / r.TargetOutputKW) * 100))
                        : 0.0;

                    var activeDevices = site.Devices.Count(d => d.Status == "Active");
                    var faultDevices = site.Devices.Count(d => d.Status == "Fault");
                    var maintenanceDevices = site.Devices.Count(d => d.Status == "Maintenance");
                    var criticalAlerts = site.Devices.SelectMany(d => d.Alerts)
                        .Count(a => a.Severity == "Critical" && a.Status == "Open");

                    var metricsTable = new Table(2)
                        .SetWidth(UnitValue.CreatePercentValue(100));

                    AddTableCell(metricsTable, "Average Efficiency:", $"{avgEfficiency:F2}%");
                    AddTableCell(metricsTable, "Active Devices:", activeDevices.ToString());
                    AddTableCell(metricsTable, "Devices in Fault:", faultDevices.ToString());
                    AddTableCell(metricsTable, "Devices in Maintenance:", maintenanceDevices.ToString());
                    AddTableCell(metricsTable, "Critical Open Alerts:", criticalAlerts.ToString());

                    document.Add(metricsTable);

                    // Underperforming Devices
                    var underperformingDevices = site.Devices
                        .Where(d => d.CurrentEfficiency < site.PerformanceTarget && d.Readings.Any())
                        .OrderBy(d => d.CurrentEfficiency)
                        .ToList();

                    if (underperformingDevices.Any())
                    {
                        document.Add(new Paragraph("UNDERPERFORMING DEVICES")
                            .SetFontSize(14)
                            .SetBold()
                            .SetMarginTop(20)
                            .SetMarginBottom(10));

                        var devicesTable = new Table(5)
                            .SetWidth(UnitValue.CreatePercentValue(100));

                        devicesTable.AddHeaderCell(CreateHeaderCell("Device Name"));
                        devicesTable.AddHeaderCell(CreateHeaderCell("Status"));
                        devicesTable.AddHeaderCell(CreateHeaderCell("Current Efficiency"));
                        devicesTable.AddHeaderCell(CreateHeaderCell("Open Alerts"));
                        devicesTable.AddHeaderCell(CreateHeaderCell("Last Reading"));

                        foreach (var device in underperformingDevices)
                        {
                            var openAlerts = device.Alerts.Count(a => a.Status == "Open");
                            var lastReading = device.Readings.OrderByDescending(r => r.Timestamp).FirstOrDefault();

                            devicesTable.AddCell(new Cell().Add(new Paragraph(device.Name)));
                            devicesTable.AddCell(new Cell().Add(new Paragraph(device.Status)));
                            devicesTable.AddCell(new Cell().Add(new Paragraph($"{device.CurrentEfficiency:F2}%")));
                            devicesTable.AddCell(new Cell().Add(new Paragraph(openAlerts.ToString())));
                            devicesTable.AddCell(new Cell().Add(new Paragraph(
                                lastReading?.Timestamp.ToString("yyyy-MM-dd HH:mm") ?? "N/A")));
                        }

                        document.Add(devicesTable);
                    }

                    // Critical Alerts
                    var criticalAlertsData = site.Devices
                        .SelectMany(d => d.Alerts.Where(a => a.Severity == "Critical" && a.Status == "Open"))
                        .ToList();

                    if (criticalAlertsData.Any())
                    {
                        document.Add(new Paragraph("CRITICAL ALERTS REQUIRING IMMEDIATE ACTION")
                            .SetFontSize(14)
                            .SetBold()
                            .SetMarginTop(20)
                            .SetMarginBottom(10));

                        var alertsTable = new Table(4)
                            .SetWidth(UnitValue.CreatePercentValue(100));

                        alertsTable.AddHeaderCell(CreateHeaderCell("Device"));
                        alertsTable.AddHeaderCell(CreateHeaderCell("Alert Type"));
                        alertsTable.AddHeaderCell(CreateHeaderCell("Message"));
                        alertsTable.AddHeaderCell(CreateHeaderCell("Created"));

                        foreach (var alert in criticalAlertsData.OrderByDescending(a => a.CreatedAt))
                        {
                            alertsTable.AddCell(new Cell().Add(new Paragraph(alert.Device?.Name ?? "Unknown")));
                            alertsTable.AddCell(new Cell().Add(new Paragraph(alert.AlertType)));
                            alertsTable.AddCell(new Cell().Add(new Paragraph(alert.Message)));
                            alertsTable.AddCell(new Cell().Add(new Paragraph(alert.CreatedAt.ToString("yyyy-MM-dd HH:mm"))));
                        }

                        document.Add(alertsTable);
                    }

                    // Recommendations
                    document.Add(new Paragraph("RECOMMENDATIONS")
                        .SetFontSize(14)
                        .SetBold()
                        .SetMarginTop(20)
                        .SetMarginBottom(10));

                    var recommendations = GenerateRecommendations(site, avgEfficiency);
                    var list = new List()
                        .SetSymbolIndent(12);

                    foreach (var rec in recommendations)
                    {
                        list.Add(new ListItem(rec));
                    }

                    document.Add(list);

                    // Footer
                    document.Add(new Paragraph("\nEnd of Report")
                        .SetFontSize(10)
                        .SetMarginTop(20)
                        .SetTextAlignment(TextAlignment.CENTER));

                    document.Close();
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating maintenance report for site {SiteId}", siteId);
                throw;
            }
        }

        /// <summary>
        /// Generate a device-specific report
        /// </summary>
        public async Task<byte[]> GenerateDeviceReportPdfAsync(int deviceId)
        {
            try
            {
                var device = await _context.Devices
                    .Include(d => d.Site)
                    .Include(d => d.Readings)
                    .Include(d => d.Alerts)
                    .Include(d => d.MaintenanceRecords)
                    .FirstOrDefaultAsync(d => d.Id == deviceId);

                if (device == null)
                    throw new Exception($"Device with ID {deviceId} not found");

                using (var memoryStream = new MemoryStream())
                {
                    var writer = new PdfWriter(memoryStream);
                    var pdfDocument = new PdfDocument(writer);
                    var document = new Document(pdfDocument);

                    // Title
                    document.Add(new Paragraph($"Device Performance Report - {device.Name}")
                        .SetFontSize(20)
                        .SetBold()
                        .SetMarginBottom(5));

                    document.Add(new Paragraph($"Site: {device.Site?.Name} | Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}")
                        .SetFontSize(10)
                        .SetMarginBottom(20));

                    // Device Summary
                    document.Add(new Paragraph("DEVICE SUMMARY")
                        .SetFontSize(14)
                        .SetBold()
                        .SetMarginBottom(10));

                    var devTable = new Table(2)
                        .SetWidth(UnitValue.CreatePercentValue(100));

                    AddTableCell(devTable, "Type:", device.DeviceType);
                    AddTableCell(devTable, "Status:", device.Status);
                    AddTableCell(devTable, "Capacity:", $"{device.Capacity:F2} KW");
                    AddTableCell(devTable, "Current Efficiency:", $"{device.CurrentEfficiency:F2}%");
                    AddTableCell(devTable, "Installed:", device.InstalledDate.ToString("yyyy-MM-dd"));
                    AddTableCell(devTable, "Last Maintenance:", device.LastMaintenanceDate?.ToString("yyyy-MM-dd") ?? "Never");

                    document.Add(devTable);

                    // Recent Readings
                    var recentReadings = device.Readings
                        .OrderByDescending(r => r.Timestamp)
                        .Take(10)
                        .OrderBy(r => r.Timestamp)
                        .ToList();

                    if (recentReadings.Any())
                    {
                        document.Add(new Paragraph("RECENT POWER READINGS (Last 10)")
                            .SetFontSize(14)
                            .SetBold()
                            .SetMarginTop(15)
                            .SetMarginBottom(10));

                        var readingsTable = new Table(4)
                            .SetWidth(UnitValue.CreatePercentValue(100));

                        readingsTable.AddHeaderCell(CreateHeaderCell("Timestamp"));
                        readingsTable.AddHeaderCell(CreateHeaderCell("Actual Output (KW)"));
                        readingsTable.AddHeaderCell(CreateHeaderCell("Target Output (KW)"));
                        readingsTable.AddHeaderCell(CreateHeaderCell("Efficiency"));

                        foreach (var reading in recentReadings)
                        {
                            readingsTable.AddCell(new Cell().Add(new Paragraph(reading.Timestamp.ToString("yyyy-MM-dd HH:mm"))));
                            readingsTable.AddCell(new Cell().Add(new Paragraph($"{reading.ActualOutputKW:F2}")));
                            readingsTable.AddCell(new Cell().Add(new Paragraph($"{reading.TargetOutputKW:F2}")));
                            readingsTable.AddCell(new Cell().Add(new Paragraph($"{reading.Efficiency:F2}%")));
                        }

                        document.Add(readingsTable);
                    }

                    // Open Alerts
                    var openAlerts = device.Alerts.Where(a => a.Status == "Open").OrderByDescending(a => a.CreatedAt).ToList();
                    if (openAlerts.Any())
                    {
                        document.Add(new Paragraph("OPEN ALERTS")
                            .SetFontSize(14)
                            .SetBold()
                            .SetMarginTop(15)
                            .SetMarginBottom(10));

                        var alertsTable = new Table(4)
                            .SetWidth(UnitValue.CreatePercentValue(100));

                        alertsTable.AddHeaderCell(CreateHeaderCell("Type"));
                        alertsTable.AddHeaderCell(CreateHeaderCell("Severity"));
                        alertsTable.AddHeaderCell(CreateHeaderCell("Message"));
                        alertsTable.AddHeaderCell(CreateHeaderCell("Created"));

                        foreach (var alert in openAlerts)
                        {
                            alertsTable.AddCell(new Cell().Add(new Paragraph(alert.AlertType)));
                            alertsTable.AddCell(new Cell().Add(new Paragraph(alert.Severity)));
                            alertsTable.AddCell(new Cell().Add(new Paragraph(alert.Message)));
                            alertsTable.AddCell(new Cell().Add(new Paragraph(alert.CreatedAt.ToString("yyyy-MM-dd HH:mm"))));
                        }

                        document.Add(alertsTable);
                    }

                    document.Close();
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating device report for device {DeviceId}", deviceId);
                throw;
            }
        }

        private List<string> GenerateRecommendations(Site site, double avgEfficiency)
        {
            var recommendations = new List<string>();

            if (avgEfficiency < site.PerformanceTarget - 10)
            {
                recommendations.Add($"URGENT: System efficiency is {avgEfficiency:F2}%, which is {(site.PerformanceTarget - avgEfficiency):F2}% below target. Conduct immediate site-wide inspection.");
            }

            var faultDevices = site.Devices.Count(d => d.Status == "Fault");
            if (faultDevices > 0)
            {
                recommendations.Add($"Address {faultDevices} device(s) in Fault status immediately to prevent further performance degradation.");
            }

            var maintenanceOverdue = site.Devices
                .Where(d => d.LastMaintenanceDate.HasValue && (DateTime.UtcNow - d.LastMaintenanceDate.Value).TotalDays > 90)
                .Count();

            if (maintenanceOverdue > 0)
            {
                recommendations.Add($"Schedule preventive maintenance for {maintenanceOverdue} device(s) that are overdue (>90 days).");
            }

            recommendations.Add("Review sensor accuracy and calibration settings.");
            recommendations.Add("Analyze weather impact on performance using historical data.");

            return recommendations;
        }

        private void AddTableCell(Table table, string label, string value)
        {
            var labelCell = new Cell();
            labelCell.Add(new Paragraph(label).SetBold());
            table.AddCell(labelCell);

            table.AddCell(new Cell().Add(new Paragraph(value)));
        }

        private Cell CreateHeaderCell(string text)
        {
            var cell = new Cell();
            cell.Add(new Paragraph(text).SetBold());
            cell.SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY);
            return cell;
        }
    }
}
