# EnergyPulse - Enhancement Documentation

## Overview

This document outlines the significant enhancements made to the EnergyPulse application to create a production-ready renewable energy management platform.

## Key Features Implemented

### 1. **System Performance Index (Hero Metric)**

- Real-time calculation of Actual Output vs. Target Output
- Efficiency percentage tracking per device and site-wide
- Automatic performance degradation alerts

### 2. **One-Click Maintenance Report Generation**

**Endpoints:**

- `GET /api/report/pdf/maintenance/{siteId}` - Generate site-wide PDF report
- `GET /api/report/pdf/device/{deviceId}` - Generate device-specific PDF

**Report Contents:**

- Site summary with capacity and performance metrics
- Performance analysis with average efficiency
- Underperforming devices list
- Critical alerts requiring immediate action
- AI-generated recommendations for maintenance

### 3. **Advanced Alert Management**

**Intelligent Auto-Alerts (6 Thresholds):**

1. Device Offline Detection (0 output)
2. Severely Low Efficiency (<50%)
3. Low Efficiency (50-70%)
4. Below Site Performance Target
5. Sensor Anomalies (negative values)
6. Sudden Output Drop (>30% variance)

**Endpoints:**

- `GET /api/alerts` - View all alerts with filtering
- `GET /api/alerts/critical/site/{siteId}` - Critical alerts
- `GET /api/alerts/summary/site/{siteId}` - Alert summary
- `POST /api/alerts` - Create manual alert
- `PUT /api/alerts/{id}/resolve` - Resolve alert

### 4. **Maintenance History & Tracking**

**Endpoints:**

- `GET /api/report/maintenance-records` - List maintenance records
- `POST /api/report/maintenance-records` - Create maintenance record
- `PUT /api/report/maintenance-records/{id}` - Update record
- `DELETE /api/report/maintenance-records/{id}` - Delete record

**Record Details:**

- Device assignment
- Priority levels (Low, Medium, High, Critical)
- Estimated and actual costs
- Status tracking (Pending, In Progress, Completed)

### 5. **Performance Analytics & Trending**

**Endpoints:**

- `GET /api/performance/trends/device/{deviceId}` - Device efficiency trends
- `GET /api/performance/comparison/site/{siteId}` - Multi-device comparison
- `GET /api/performance/revenue-loss/site/{siteId}` - Calculate revenue loss
- `GET /api/performance/recommendations/site/{siteId}` - Maintenance recommendations
- `GET /api/performance/reliability-score/device/{deviceId}` - Device reliability grade

### 6. **Business Metrics**

- **Revenue Loss Calculation**: Quantify financial impact of underperformance
- **Device Reliability Score**: 0-100 grade based on efficiency, alerts, and maintenance
- **Predictive Maintenance**: ML-style trend analysis and recommendations

### 7. **User & Role Management**

**Models:**

- User with Role-Based Access Control
- Admin: Full access
- Technician: View and update assigned tasks

**User Properties:**

- Email and authentication ready
- Site assignment
- Login tracking

## Database Models

### Device (Enhanced)

```csharp
- DeviceType: Solar Panel, Inverter, Battery, etc.
- Capacity: Device capacity in KW
- Status: Active, Inactive, Maintenance, Fault
- CurrentEfficiency: Latest efficiency percentage
- LastMaintenanceDate: Track maintenance schedule
```

### Alert (Enhanced)

```csharp
- AlertType: Performance, Hardware, Sensor, etc.
- Status: Open, In Review, Resolved, Ignored
- Severity: Low, Medium, High, Critical
- ImpactKW: Estimated power loss
- EstimatedCost: Financial impact in USD
```

### MaintenanceRecord (New)

```csharp
- Title, Description: Work details
- Status: Pending, In Progress, Completed
- Priority: Low, Medium, High, Critical
- EstimatedCost, ActualCost: Budget tracking
```

### User (New)

```csharp
- Role: Admin or Technician
- SiteId: Assignment to specific site
- CreatedReports: Track maintenance records
```

## API Endpoints Summary

### Report Generation

- `GET /api/report/summary/{siteId}` - Summary data
- `GET /api/report/pdf/maintenance/{siteId}` - PDF report
- `GET /api/report/pdf/device/{deviceId}` - Device PDF

### Maintenance Records

- `GET /api/report/maintenance-records` - List all
- `POST /api/report/maintenance-records` - Create
- `PUT /api/report/maintenance-records/{id}` - Update
- `DELETE /api/report/maintenance-records/{id}` - Delete

### Alerts Management

- `GET /api/alerts` - All alerts with pagination
- `GET /api/alerts/critical/site/{siteId}` - Critical only
- `GET /api/alerts/summary/site/{siteId}` - Summary stats
- `GET /api/alerts/device/{deviceId}` - Device alerts
- `POST /api/alerts` - Create alert
- `PUT /api/alerts/{id}/resolve` - Resolve
- `PUT /api/alerts/{id}/status` - Update status

### Performance Analytics

- `GET /api/performance/trends/device/{deviceId}` - Trends
- `GET /api/performance/comparison/site/{siteId}` - Comparison
- `GET /api/performance/revenue-loss/site/{siteId}` - Revenue impact
- `GET /api/performance/recommendations/site/{siteId}` - Recommendations
- `GET /api/performance/reliability-score/device/{deviceId}` - Reliability

## Technology Stack

**Backend:**

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server
- iText7 (PDF Generation)

**Services:**

- Dependency Injection
- Async/await patterns
- Comprehensive logging

## Business Value Proposition

**For Operations Managers:**

1. **Dashboard KPI**: System Performance Index shows efficiency at a glance
2. **Quick Actions**: One-click PDF reports for technician dispatch
3. **Cost Visibility**: Revenue loss calculations quantify impact
4. **Maintenance Priority**: AI recommendations focus on critical issues
5. **Device Health**: Reliability scores guide replacement decisions

**Portfolio Strengths:**

- Demonstrates full-stack development
- Complex data relationships (5+ entities)
- Advanced business logic implementation
- Professional reporting capability
- Scalable architecture

## Future Enhancements

1. **UI Charts & Visualization**
   - Trend charts using Chart.js
   - Device comparison visualizations
   - Alert timeline charts

2. **Email Notifications**
   - Auto-email critical alerts
   - Weekly performance summaries
   - PDF report distribution

3. **Advanced Analytics**
   - Machine learning for anomaly detection
   - Weather impact analysis
   - Predictive equipment failure

4. **Mobile App**
   - React Native for iOS/Android
   - Real-time push notifications
   - Offline capabilities

## Getting Started

1. **Restore NuGet packages**: `dotnet restore`
2. **Update database**: `dotnet ef database update`
3. **Run application**: `dotnet run`
4. **Access API**: `https://localhost:5001/swagger`

## Testing the New Features

### Generate PDF Report

```bash
GET http://localhost:5001/api/report/pdf/maintenance/1
```

### Get Performance Trends

```bash
GET http://localhost:5001/api/performance/trends/device/1?daysBack=30
```

### Create Maintenance Record

```bash
POST http://localhost:5001/api/report/maintenance-records
Body: {
  "deviceId": 1,
  "title": "Inverter Inspection",
  "description": "Quarterly maintenance check",
  "priority": "High",
  "estimatedCost": 500
}
```

### Get Critical Alerts

```bash
GET http://localhost:5001/api/alerts/critical/site/1
```

## Client Story

_"I noticed that many small solar farm managers were wasting hours manually checking sensor logs to find why their power output was dropping. I built this .NET solution to provide a Performance Index that flags issues automatically and allows them to generate Maintenance Reports in seconds, reducing their downtime by 15%."_

This implementation delivers on all three pillars of that story.
