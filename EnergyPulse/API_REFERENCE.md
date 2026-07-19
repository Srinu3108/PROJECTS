# EnergyPulse - Complete API Reference

## API Endpoints Summary

### 📊 Dashboard Endpoints

#### Get Summary Report

```
GET /api/report/summary/{siteId}
Response: ReportDto
```

Returns site overview with performance metrics

---

### 📄 Report & Maintenance Endpoints

#### Get Report Summary

```
GET /api/report/summary/{siteId}
Response: ReportDto
```

#### Generate Maintenance PDF

```
GET /api/report/pdf/maintenance/{siteId}
Response: Binary PDF file
```

Downloads professional maintenance report for site

#### Generate Device PDF

```
GET /api/report/pdf/device/{deviceId}
Response: Binary PDF file
```

Downloads device-specific performance report

#### List Maintenance Records

```
GET /api/report/maintenance-records
Query Parameters:
  - siteId: int (optional)
  - status: string (optional) - Pending, In Progress, Completed
Response: List<MaintenanceRecordDto>
```

#### Create Maintenance Record

```
POST /api/report/maintenance-records
Body: CreateMaintenanceRecordDto
Response: MaintenanceRecordDto (201 Created)
```

Required fields:

- deviceId: int
- title: string
- description: string
- priority: string (Low, Medium, High, Critical)

#### Update Maintenance Record

```
PUT /api/report/maintenance-records/{id}
Body: UpdateMaintenanceRecordDto
Response: MaintenanceRecordDto (200 OK)
```

#### Delete Maintenance Record

```
DELETE /api/report/maintenance-records/{id}
Response: 204 No Content
```

---

### 🚨 Alert Management Endpoints

#### List All Alerts

```
GET /api/alerts
Query Parameters:
  - siteId: int (optional)
  - severity: string (optional) - Low, Medium, High, Critical
  - status: string (optional) - Open, In Review, Resolved, Ignored
  - pageNumber: int (default: 1)
  - pageSize: int (default: 20)
Response: { totalCount, pageNumber, pageSize, alerts: List<AlertDto> }
```

#### Get Critical Alerts for Site

```
GET /api/alerts/critical/site/{siteId}
Response: List<AlertDto>
```

Returns only Critical severity alerts that are Open

#### Get Alert Summary

```
GET /api/alerts/summary/site/{siteId}
Response: AlertSummaryDto
```

Returns count by severity and average impact

#### Get Device Alerts

```
GET /api/alerts/device/{deviceId}
Query Parameters:
  - status: string (default: "Open")
Response: List<AlertDto>
```

#### Create Alert

```
POST /api/alerts
Body: CreateAlertDto
Response: AlertDto (201 Created)
```

Fields:

- deviceId: int (required)
- alertType: string (Performance, Hardware, Sensor, etc.)
- message: string (required)
- severity: string (Low, Medium, High, Critical)
- impactKW: double (optional)

#### Resolve Alert

```
PUT /api/alerts/{id}/resolve
Response: { message: "Alert resolved successfully" }
```

#### Update Alert Status

```
PUT /api/alerts/{id}/status
Body: { status: string }
Response: AlertDto (200 OK)
```

#### Delete Alert

```
DELETE /api/alerts/{id}
Response: 204 No Content
```

---

### 📈 Performance Analytics Endpoints

#### Get Device Performance Trend

```
GET /api/performance/trends/device/{deviceId}
Query Parameters:
  - daysBack: int (default: 30)
Response: List<PerformanceTrendDto>
```

Returns daily average efficiency over specified period

#### Get Site Device Comparison

```
GET /api/performance/comparison/site/{siteId}
Response: List<DeviceComparisonDto>
```

Compares all devices in a site

#### Calculate Revenue Loss

```
GET /api/performance/revenue-loss/site/{siteId}
Query Parameters:
  - pricePerKWh: double (default: 50.0)
Response: RevenueLossDto
```

Calculates financial impact of underperformance

#### Get Maintenance Recommendations

```
GET /api/performance/recommendations/site/{siteId}
Response: List<MaintenanceRecommendationDto>
```

AI-style recommendations based on trends

#### Get Device Reliability Score

```
GET /api/performance/reliability-score/device/{deviceId}
Response: { deviceId, reliabilityScore: double, grade: string }
```

Returns score 0-100 and letter grade

---

## DTOs Reference

### ReportDto

```csharp
{
  "site": "string",
  "totalDevices": "int",
  "activeDevices": "int",
  "averageEfficiency": "double",
  "systemPerformanceIndex": "double",
  "totalAlerts": "int",
  "highAlerts": "int",
  "generatedAt": "datetime",
  "underperformingDevices": "List<DeviceIssueDto>"
}
```

### AlertDto

```csharp
{
  "id": "int",
  "deviceId": "int",
  "deviceName": "string",
  "alertType": "string",
  "message": "string",
  "createdAt": "datetime",
  "resolvedAt": "datetime|null",
  "severity": "string",
  "status": "string",
  "impactKW": "double|null",
  "estimatedCost": "double|null"
}
```

### MaintenanceRecordDto

```csharp
{
  "id": "int",
  "deviceId": "int",
  "deviceName": "string",
  "title": "string",
  "description": "string",
  "reportDate": "datetime",
  "completedDate": "datetime|null",
  "status": "string",
  "priority": "string",
  "estimatedCost": "double|null",
  "actualCost": "double|null",
  "notes": "string"
}
```

### PerformanceTrendDto

```csharp
{
  "date": "datetime",
  "efficiencyPercentage": "double",
  "actualOutput": "double",
  "targetOutput": "double"
}
```

### RevenueLossDto

```csharp
{
  "totalLossKW": "double",
  "estimatedCostUSD": "double",
  "affectedDevices": "int",
  "periodDays": "string"
}
```

### MaintenanceRecommendationDto

```csharp
{
  "deviceName": "string",
  "recommendation": "string",
  "priority": "string"
}
```

### AlertSummaryDto

```csharp
{
  "criticalAlerts": "int",
  "highAlerts": "int",
  "mediumAlerts": "int",
  "totalOpenAlerts": "int",
  "averageImpactKW": "double",
  "estimatedTotalCost": "double"
}
```

---

## HTTP Status Codes

| Code | Meaning      | Example                       |
| ---- | ------------ | ----------------------------- |
| 200  | OK           | Successfully retrieved data   |
| 201  | Created      | Resource created successfully |
| 204  | No Content   | Successful deletion           |
| 400  | Bad Request  | Invalid parameters            |
| 404  | Not Found    | Resource doesn't exist        |
| 500  | Server Error | Internal error                |

---

## Error Response Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-..."
}
```

---

## Example Requests with cURL

### Get Critical Alerts

```bash
curl -X GET "https://localhost:5001/api/alerts/critical/site/1" \
  -H "Accept: application/json"
```

### Generate PDF Report

```bash
curl -X GET "https://localhost:5001/api/report/pdf/maintenance/1" \
  -H "Accept: application/pdf" \
  -o report.pdf
```

### Create Maintenance Record

```bash
curl -X POST "https://localhost:5001/api/report/maintenance-records" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": 1,
    "title": "Quarterly Inspection",
    "description": "Routine maintenance check",
    "priority": "Medium",
    "estimatedCost": 500
  }'
```

### Get Performance Trends

```bash
curl -X GET "https://localhost:5001/api/performance/trends/device/1?daysBack=30" \
  -H "Accept: application/json"
```

### Get Revenue Loss

```bash
curl -X GET "https://localhost:5001/api/performance/revenue-loss/site/1?pricePerKWh=50" \
  -H "Accept: application/json"
```

---

## Pagination Example

```
GET /api/alerts?pageNumber=2&pageSize=10

Response:
{
  "totalCount": 45,
  "pageNumber": 2,
  "pageSize": 10,
  "alerts": [...]
}
```

---

## Filtering Examples

### Get High Severity Open Alerts

```
GET /api/alerts?severity=High&status=Open
```

### Get Maintenance Records by Status

```
GET /api/report/maintenance-records?status=Pending&siteId=1
```

### Get Device Alerts (Resolved)

```
GET /api/alerts/device/1?status=Resolved
```

---

## Response Time Expectations

| Endpoint       | Complexity | Expected Time |
| -------------- | ---------- | ------------- |
| List Alerts    | Low        | <100ms        |
| Get Trends     | Medium     | 100-500ms     |
| Generate PDF   | High       | 500-2000ms    |
| Get Comparison | High       | 200-800ms     |
| Alert Summary  | Low        | <50ms         |

---

## Authentication (Future)

When authentication is added:

```
Authorization: Bearer {jwt_token}
```

All endpoints will require valid JWT token.

---

**API Version:** 2.0  
**Last Updated:** July 12, 2026  
**Status:** Production Ready
