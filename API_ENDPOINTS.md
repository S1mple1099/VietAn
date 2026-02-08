# API Endpoints - Postman Collection

## Base URL
- **HTTP**: `http://localhost:5189`
- **HTTPS**: `https://localhost:7068`

---

## 🔐 Authentication APIs

### 1. Login
**POST** `/api/auth/login`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "fullName": "Quản Trị Viên",
  "expiresAt": "2026-01-18T05:52:26Z",
  "permissions": ["VIEW_DASHBOARD", "VIEW_MONITOR", "VIEW_HISTORY", "EXPORT_DATA", "MANAGE_USER", "VIEW_LOGIN_LOG"]
}
```

**Ví dụ Postman:**
```
POST http://localhost:5189/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

---

## 📊 Monitor APIs

### 1. Get Monitor Data

#### Option 1: GET với Query String
**GET** `/api/monitor/data`

**Query Parameters:**
- `pumpId` (optional, default: "pump1") - ID của bơm (pump1, pump2, pump3)
- `fromDate` (optional, default: today) - Ngày bắt đầu (format: yyyy-MM-dd)
- `toDate` (optional, default: today) - Ngày kết thúc (format: yyyy-MM-dd)
- `page` (optional, default: 1) - Số trang (bắt đầu từ 1)
- `pageSize` (optional, default: 15) - Số bản ghi mỗi trang (tối đa: 1000)

**Ví dụ:**
```
GET http://localhost:5189/api/monitor/data?pumpId=pump1&fromDate=2026-01-17&toDate=2026-01-17&page=1&pageSize=15
```

**Ví dụ với default values:**
```
GET http://localhost:5189/api/monitor/data
```

#### Option 2: POST với Request Body (Khuyến nghị)
**POST** `/api/monitor/data`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "pumpId": "pump1",
  "fromDate": "2026-01-17",
  "toDate": "2026-01-17",
  "page": 1,
  "pageSize": 15
}
```

**Ví dụ Postman:**
```
POST http://localhost:5189/api/monitor/data
Content-Type: application/json

{
  "pumpId": "pump1",
  "fromDate": "2026-01-17",
  "toDate": "2026-01-17",
  "page": 1,
  "pageSize": 15
}
```

**Ví dụ với default values (chỉ cần gửi {}):**
```
POST http://localhost:5189/api/monitor/data
Content-Type: application/json

{}
```

**Response:**
```json
{
  "items": [
    {
      "pumpId": "pump1",
      "timestamp": "2026-01-17T06:00:00",
      "date": "17/01/2026",
      "time": "06:00:00",
      "tempA": "55.0",
      "tempB": "56.0",
      "tempC": "54.0",
      "vrs": "380",
      "vst": "379",
      "vtr": "381",
      "currentR": "12.0",
      "currentS": "11.5",
      "currentT": "12.2",
      "runtime": "00:00",
      "tankOut": "35",
      "tankIn": "47"
    }
  ],
  "totalCount": 1440,
  "page": 1,
  "pageSize": 15,
  "totalPages": 96,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### 2. Get Pumps List
**GET** `/api/monitor/pumps`

**Ví dụ:**
```
GET http://localhost:5189/api/monitor/pumps
```

**Response:**
```json
[
  {
    "id": "pump1",
    "name": "Bom 1"
  },
  {
    "id": "pump2",
    "name": "Bom 2"
  },
  {
    "id": "pump3",
    "name": "Bom 3"
  }
]
```

### 3. Export Monitor Data (Excel)

#### Option 1: GET với Query String
**GET** `/api/monitor/export`

**Query Parameters:**
- `pumpId` (optional, default: "pump1")
- `fromDate` (optional, default: today)
- `toDate` (optional, default: today)

**Ví dụ:**
```
GET http://localhost:5189/api/monitor/export?pumpId=pump1&fromDate=2026-01-17&toDate=2026-01-17
```

#### Option 2: POST với Request Body
**POST** `/api/monitor/export`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "pumpId": "pump1",
  "fromDate": "2026-01-17",
  "toDate": "2026-01-17"
}
```

**Ví dụ Postman:**
```
POST http://localhost:5189/api/monitor/export
Content-Type: application/json

{
  "pumpId": "pump1",
  "fromDate": "2026-01-17",
  "toDate": "2026-01-17"
}
```

**Response:** File Excel (.xlsx)

---

## 📜 History APIs

### 1. Get History Data

#### Option 1: GET với Query String
**GET** `/api/history`

**Query Parameters:**
- `eventType` (optional, default: "all") - Loại sự kiện: "all", "login", "error", "ok", "warn"
- `searchText` (optional) - Từ khóa tìm kiếm
- `fromDate` (optional, default: today) - Ngày bắt đầu (format: yyyy-MM-dd)
- `toDate` (optional, default: today) - Ngày kết thúc (format: yyyy-MM-dd)
- `includeLoginLogs` (optional, default: true) - Bao gồm login logs
- `page` (optional, default: 1) - Số trang (bắt đầu từ 1)
- `pageSize` (optional, default: 15) - Số bản ghi mỗi trang (tối đa: 1000)

**Ví dụ:**
```
GET http://localhost:5189/api/history?eventType=all&fromDate=2026-01-06&toDate=2026-01-06&includeLoginLogs=true&page=1&pageSize=15
```

**Ví dụ với search:**
```
GET http://localhost:5189/api/history?eventType=error&searchText=PLC&fromDate=2026-01-06&toDate=2026-01-17
```

**Ví dụ với default values:**
```
GET http://localhost:5189/api/history
```

#### Option 2: POST với Request Body (Khuyến nghị)
**POST** `/api/history/search`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "eventType": "all",
  "searchText": "",
  "fromDate": "2026-01-06",
  "toDate": "2026-01-06",
  "includeLoginLogs": true,
  "page": 1,
  "pageSize": 15
}
```

**Ví dụ Postman:**
```
POST http://localhost:5189/api/history/search
Content-Type: application/json

{
  "eventType": "all",
  "fromDate": "2026-01-06",
  "toDate": "2026-01-06",
  "includeLoginLogs": true,
  "page": 1,
  "pageSize": 15
}
```

**Ví dụ với search:**
```
POST http://localhost:5189/api/history/search
Content-Type: application/json

{
  "eventType": "error",
  "searchText": "PLC",
  "fromDate": "2026-01-06",
  "toDate": "2026-01-06",
  "includeLoginLogs": true,
  "page": 1,
  "pageSize": 15
}
```

**Ví dụ với default values (chỉ cần gửi {}):**
```
POST http://localhost:5189/api/history/search
Content-Type: application/json

{}
```

**Response:**
```json
{
  "items": [
    {
      "id": "01",
      "time": "06/01/2026 14:23:20",
      "device": "",
      "account": "Technical",
      "type": "Đăng Nhập",
      "description": "Technical đăng nhập",
      "errorCode": null,
      "processingTime": null
    },
    {
      "id": "02",
      "time": "06/01/2026 14:26:20",
      "device": "Bom 1",
      "account": "Admin",
      "type": "Lỗi",
      "description": "Quá momen",
      "errorCode": "PUMP_OVERMOMENT",
      "processingTime": "15s"
    }
  ],
  "totalCount": 130,
  "page": 1,
  "pageSize": 15,
  "totalPages": 9,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### 2. Get Event Types
**GET** `/api/history/event-types`

**Ví dụ:**
```
GET http://localhost:5189/api/history/event-types
```

**Response:**
```json
[
  {
    "id": "all",
    "name": "Tất cả sự kiện"
  },
  {
    "id": "login",
    "name": "Đăng Nhập"
  },
  {
    "id": "error",
    "name": "Lỗi"
  },
  {
    "id": "ok",
    "name": "Hoạt Động Tốt"
  },
  {
    "id": "warn",
    "name": "Cảnh Báo"
  }
]
```

### 3. Export History Data (Excel)

#### Option 1: GET với Query String
**GET** `/api/history/export`

**Query Parameters:**
- `eventType` (optional, default: "all")
- `searchText` (optional)
- `fromDate` (optional, default: today)
- `toDate` (optional, default: today)
- `includeLoginLogs` (optional, default: true)

**Ví dụ:**
```
GET http://localhost:5189/api/history/export?eventType=all&fromDate=2026-01-06&toDate=2026-01-06
```

#### Option 2: POST với Request Body
**POST** `/api/history/export`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "eventType": "all",
  "searchText": "",
  "fromDate": "2026-01-06",
  "toDate": "2026-01-06",
  "includeLoginLogs": true
}
```

**Ví dụ Postman:**
```
POST http://localhost:5189/api/history/export
Content-Type: application/json

{
  "eventType": "all",
  "fromDate": "2026-01-06",
  "toDate": "2026-01-06",
  "includeLoginLogs": true
}
```

**Response:** File Excel (.xlsx)

---

## 👥 User Management APIs

### 1. Get All Users
**GET** `/api/user`

**Headers (khi có login):**
```
Authorization: Bearer {token}
```

**Ví dụ:**
```
GET http://localhost:5189/api/user
```

**Response:**
```json
[
  {
    "id": "guid-here",
    "username": "admin",
    "fullName": "Quản Trị Viên",
    "email": "admin@monitoring.local",
    "isActive": true,
    "roles": ["ADMIN"]
  }
]
```

### 2. Get User By ID
**GET** `/api/user/{id}`

**Ví dụ:**
```
GET http://localhost:5189/api/user/123e4567-e89b-12d3-a456-426614174000
```

### 3. Create User
**POST** `/api/user`

**Body (JSON):**
```json
{
  "username": "newuser",
  "password": "password123",
  "fullName": "Người Dùng Mới",
  "email": "newuser@monitoring.local",
  "roleIds": ["role-guid-1", "role-guid-2"]
}
```

**Ví dụ:**
```
POST http://localhost:5189/api/user
Content-Type: application/json

{
  "username": "newuser",
  "password": "password123",
  "fullName": "Người Dùng Mới",
  "email": "newuser@monitoring.local",
  "roleIds": []
}
```

### 4. Update User
**PUT** `/api/user/{id}`

**Body (JSON):**
```json
{
  "fullName": "Tên Mới",
  "email": "newemail@monitoring.local",
  "isActive": true,
  "roleIds": ["role-guid-1"]
}
```

### 5. Delete User
**DELETE** `/api/user/{id}`

**Ví dụ:**
```
DELETE http://localhost:5189/api/user/123e4567-e89b-12d3-a456-426614174000
```

### 6. Change Password
**POST** `/api/user/{id}/change-password`

**Body (JSON):**
```json
{
  "currentPassword": "oldpassword",
  "newPassword": "newpassword123"
}
```

### 7. Get All Roles
**GET** `/api/user/roles`

**Ví dụ:**
```
GET http://localhost:5189/api/user/roles
```

### 8. Get Role By ID
**GET** `/api/user/roles/{id}`

---

## 📝 Test Accounts

### Default Users (sau khi seed):
- **Admin**: `admin` / `admin123`
- **Monitor**: `monitor` / `monitor123`
- **Operator**: `operator` / `operator123`
- **Technical**: `technical` / `technical123`
- **Viewer**: `viewer` / `viewer123`

---

## 🔍 Quick Test Examples

### Test History API (đơn giản nhất):
```
GET http://localhost:5189/api/history
```

### Test Monitor API:
```
GET http://localhost:5189/api/monitor/data?pumpId=pump1&fromDate=2026-01-17&toDate=2026-01-17&page=1&pageSize=15
```

### Test Login:
```
POST http://localhost:5189/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

---

## ⚠️ Lưu ý

1. **Base URL**: Kiểm tra port trong `launchSettings.json` hoặc console output khi chạy app
2. **Authentication**: Hiện tại `[Authorize]` đã được tạm thời disable cho development
3. **Date Format**: Sử dụng format `yyyy-MM-dd` (ví dụ: `2026-01-17`)
4. **Response Format**: Tất cả responses đều là JSON, trừ export endpoints (trả về file Excel)
