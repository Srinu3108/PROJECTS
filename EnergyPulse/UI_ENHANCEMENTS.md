# EnergyPulse - UI Enhancement Guide

## Overview

The backend has been significantly enhanced with new services, models, and APIs. This guide outlines recommended UI enhancements for the Blazor frontend.

## Recommended UI Components to Add

### 1. Device Detail Page

**File:** `EnergyPulse.UI/Pages/DeviceDetail.razor`

**Features:**

- Device overview card (name, status, capacity, efficiency)
- Real-time reliability score (0-100 gauge)
- Performance trend chart (last 30 days)
- Recent readings table
- Open alerts list
- Maintenance history timeline
- Action buttons (Generate Report, Create Maintenance Record)

**API Calls:**

- `GET /api/devices/{id}`
- `GET /api/performance/reliability-score/device/{id}`
- `GET /api/performance/trends/device/{id}`
- `GET /api/alerts/device/{id}`
- `GET /api/report/maintenance-records?deviceId={id}`

### 2. Enhanced Dashboard Page

**File:** Update `EnergyPulse.UI/Pages/Dashboard.razor`

**New Components:**

- System Performance Index (Hero Metric)
- Revenue Loss calculator
- Critical alerts widget with red alert status
- Maintenance recommendations panel
- Device comparison mini-charts

**API Calls:**

- `GET /api/report/summary/{siteId}`
- `GET /api/alerts/summary/site/{siteId}`
- `GET /api/performance/revenue-loss/site/{siteId}`
- `GET /api/performance/recommendations/site/{siteId}`

### 3. Performance Analytics Page

**File:** `EnergyPulse.UI/Pages/PerformanceAnalytics.razor`

**Features:**

- Interactive device comparison table
- Site performance trends chart
- Efficiency distribution chart
- Alert frequency chart
- Filter by date range, device type

**API Calls:**

- `GET /api/performance/comparison/site/{siteId}`
- `GET /api/performance/trends/device/{deviceId}`

### 4. Maintenance Management Page

**File:** Update `EnergyPulse.UI/Pages/Report.razor`

**Features:**

- Maintenance records list with status
- Create maintenance record modal
- Update record form
- Maintenance history timeline
- Cost tracking (estimated vs actual)
- Priority color coding

**API Calls:**

- `GET /api/report/maintenance-records`
- `POST /api/report/maintenance-records`
- `PUT /api/report/maintenance-records/{id}`
- `DELETE /api/report/maintenance-records/{id}`

### 5. Alert Management Enhancements

**File:** Update `EnergyPulse.UI/Pages/Alerts.razor`

**Features:**

- Alert filtering (severity, status, date range)
- Alert detail modal
- Bulk resolve actions
- Alert trend graph
- Impact visualization (revenue loss, KW impact)

**API Calls:**

- `GET /api/alerts?severity={severity}&status={status}&pageNumber={page}`
- `GET /api/alerts/summary/site/{siteId}`
- `PUT /api/alerts/{id}/resolve`
- `PUT /api/alerts/{id}/status`

## Recommended Chart Libraries

### Chart.js Integration

Perfect for Blazor with `CurrieTechnologies.Razor.ChartJS`

**Use Cases:**

- Line charts for performance trends
- Bar charts for device comparison
- Pie charts for alert distribution
- Area charts for revenue loss over time

### Installation

```bash
dotnet add package CurrieTechnologies.Razor.ChartJS
```

## Reusable Blazor Components to Create

### 1. PerformanceGauge.razor

```razor
@* Display reliability score 0-100 as circular gauge *@
<Parameters>
    - Score: double
    - DeviceName: string
</Parameters>
```

### 2. AlertBadge.razor

```razor
@* Show alert severity with color coding *@
<Parameters>
    - Severity: string (Critical=Red, High=Orange, Medium=Yellow, Low=Green)
    - Count: int
</Parameters>
```

### 3. EfficiencyChart.razor

```razor
@* Line chart for efficiency trends *@
<Parameters>
    - TrendData: List<PerformanceTrendDto>
    - DeviceName: string
</Parameters>
```

### 4. MaintenanceForm.razor

```razor
@* Reusable form for creating/editing maintenance records *@
<Parameters>
    - DeviceId: int
    - OnSubmit: EventCallback<CreateMaintenanceRecordDto>
</Parameters>
```

### 5. DeviceComparisonTable.razor

```razor
@* Comparison table for all devices in a site *@
<Parameters>
    - SiteId: int
    - OnDeviceClick: EventCallback<int>
</Parameters>
```

## UI/UX Improvements

### 1. Color Scheme Enhancement

```css
--critical: #dc3545 (Red) --high: #fd7e14 (Orange) --medium: #ffc107 (Yellow)
  --low: #28a745 (Green) --excellent: #20c997 (Teal) --good: #17a2b8 (Cyan)
  --warning: #ff6b6b (Light Red);
```

### 2. Navigation Updates

Add menu items:

- Dashboard (Enhanced)
- Devices (with new detail view)
- Performance Analytics (New)
- Maintenance (New)
- Alerts (Enhanced)
- Reports

### 3. Responsive Design

- Mobile-friendly cards
- Touch-friendly buttons
- Collapsible sections on mobile
- Bottom sheet for modals

## Implementation Priority

**Phase 1 (Critical):**

1. Device Detail Page
2. Enhanced Dashboard with KPIs
3. Alerts improvement

**Phase 2 (Important):** 4. Maintenance Management Page 5. Performance Analytics Page 6. Chart integration

**Phase 3 (Nice-to-Have):** 7. Advanced filtering 8. Export to CSV 9. Mobile optimization

## API Integration Pattern

```csharp
// Example in Blazor component
@inject HttpClient Http

private async Task LoadDeviceDetails()
{
    try
    {
        device = await Http.GetFromJsonAsync<DeviceDetailDto>(
            $"api/devices/{DeviceId}");

        performanceTrend = await Http.GetFromJsonAsync<List<PerformanceTrendDto>>(
            $"api/performance/trends/device/{DeviceId}");

        alerts = await Http.GetFromJsonAsync<List<AlertDto>>(
            $"api/alerts/device/{DeviceId}");
    }
    catch (Exception ex)
    {
        errorMessage = $"Error loading device: {ex.Message}";
    }
}
```

## Testing the UI Updates

1. **Verify API responses** using Swagger at `/swagger/index.html`
2. **Test pagination** on alerts and maintenance records
3. **Verify filters** work correctly
4. **Test PDF generation** - download and verify content
5. **Check responsive design** on mobile breakpoints

## Deployment Checklist

- [ ] All backend API endpoints tested
- [ ] Database migrations applied
- [ ] PDF report generation tested
- [ ] Alert auto-generation verified
- [ ] UI components created and tested
- [ ] Navigation links updated
- [ ] Error handling implemented
- [ ] Logging configured
- [ ] Performance optimized
- [ ] Security validated

## Support & Troubleshooting

### Common Issues

**Issue:** PDF reports not generating

- Solution: Ensure itext7 NuGet package is installed
- Command: `dotnet add package itext7`

**Issue:** Alert endpoints returning 404

- Solution: Verify AlertsController.cs is in Controllers folder
- Check namespace matches other controllers

**Issue:** Performance queries timing out

- Solution: Add database indexes on DeviceId, SiteId
- Use `Include()` to eager load relationships

**Issue:** Blazor component not rendering

- Solution: Check \_Imports.razor includes component namespace
- Verify component parameter names match exactly
