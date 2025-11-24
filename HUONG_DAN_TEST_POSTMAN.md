# 🧪 HƯỚNG DẪN TEST API `/api/booking/staff/my-bookings` TRONG POSTMAN

## ❓ TẠI SAO KHÔNG CẦN TRUYỀN ID?

**Vì API này lấy AccountId từ JWT Token, không cần truyền từ Postman!**

```
┌─────────────────────────────────────────┐
│ 1. Bạn đăng nhập → Nhận JWT Token       │
│    Token chứa AccountId bên trong        │
└─────────────────┬───────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│ 2. Gửi Token trong Header               │
│    Authorization: Bearer {token}        │
└─────────────────┬───────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│ 3. Backend tự động lấy AccountId từ Token│
│    → Query Account → Lấy StationID       │
│    → Query Booking theo StationID       │
└─────────────────────────────────────────┘
```

---

## 📋 CÁCH TEST TRONG POSTMAN

### **BƯỚC 1: Đăng nhập để lấy JWT Token**

**Request:**
```
POST http://localhost:5204/api/Authen/Login
Content-Type: application/json

{
  "keyword": "staffHN",  // AccountName hoặc Email
  "password": "staff@123"
}
```

**Response:**
```json
{
  "status": 200,
  "message": "Login successful",
  "data": {
    "accountId": "guid-here",
    "accountName": "staffHN",
    "email": "staffHN@gmail.com",
    "roleName": "Staff",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."  // ← Copy token này!
  }
}
```

**👉 Copy token từ response!**

---

### **BƯỚC 2: Test API `/api/booking/staff/my-bookings`**

**Request:**
```
GET http://localhost:5204/api/booking/staff/my-bookings
```

**Headers:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**👉 KHÔNG CẦN truyền ID vào URL hay Body!**

---

## 🎯 CHI TIẾT TRONG POSTMAN

### **1. Tạo Request mới:**

1. Method: `GET`
2. URL: `http://localhost:5204/api/booking/staff/my-bookings`
3. **KHÔNG có Parameters** (không cần truyền gì cả)

### **2. Thêm Authorization Header:**

1. Vào tab **Headers**
2. Thêm header:
   - **Key:** `Authorization`
   - **Value:** `Bearer {paste_token_here}`

   Ví dụ:
   ```
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
   ```

### **3. Click "Send"**

---

## ✅ RESPONSE THÀNH CÔNG (200):

```json
{
  "status": 200,
  "message": "Success",
  "data": [
    {
      "bookingId": "123e4567-e89b-12d3-a456-426614174000",
      "dateTime": "2024-01-15T10:00:00",
      "notes": "Đổi pin lần 1",
      "isApproved": "Pending",
      "createdDate": "2024-01-10T08:00:00",
      "stationId": "station-guid-here",
      "vehicleId": "car-guid-here",
      "accountId": "customer-guid-here"
    },
    {
      "bookingId": "another-booking-guid",
      "dateTime": "2024-01-16T14:00:00",
      "notes": "Đổi pin lần 2",
      "isApproved": "Approved",
      "createdDate": "2024-01-11T09:00:00",
      "stationId": "station-guid-here",
      "vehicleId": "car-guid-here",
      "accountId": "customer-guid-here"
    }
  ],
  "errorCode": 0
}
```

---

## ❌ CÁC TRƯỜNG HỢP LỖI:

### **1. Không có Token (401):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### **2. Token không hợp lệ (401):**
```json
{
  "message": "Invalid token",
  "status": 401
}
```

### **3. Không phải Staff (403):**
```json
{
  "status": 403,
  "message": "Only Staff can access this endpoint",
  "data": null,
  "errorCode": 1
}
```

### **4. Staff chưa được gán Station (400):**
```json
{
  "status": 400,
  "message": "Staff is not assigned to any station",
  "data": null,
  "errorCode": 1
}
```

### **5. Không có Booking (404):**
```json
{
  "status": 404,
  "message": "No bookings found for this station",
  "data": null,
  "errorCode": 1
}
```

---

## 🔍 TẠI SAO KHÔNG CẦN TRUYỀN ID?

### **Code trong Controller:**
```csharp
[HttpGet("staff/my-bookings")]
[Authorize] // Yêu cầu đăng nhập
public async Task<IActionResult> GetMyStationBookings()
{
    // Lấy AccountId từ JWT Token (KHÔNG cần truyền từ Postman!)
    var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                      ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    
    // AccountId được lấy từ Token, không phải từ request
    var result = await _bookingService.GetByStaffStationAsync(accountId);
    return StatusCode(result.Status, result);
}
```

**👉 `User.FindFirst()` tự động lấy từ JWT Token trong Header!**

---

## 📊 SO SÁNH 2 LOẠI API:

### **API 1: Cần truyền ID (như `/api/booking/User/{accountId}`)**
```
GET /api/booking/User/123e4567-e89b-12d3-a456-426614174000
```
- ✅ Cần truyền `accountId` vào URL
- ❌ Không an toàn (user có thể fake ID)

### **API 2: Lấy từ Token (như `/api/booking/staff/my-bookings`)**
```
GET /api/booking/staff/my-bookings
Authorization: Bearer {token}
```
- ✅ KHÔNG cần truyền ID
- ✅ An toàn (lấy từ Token, không thể fake)
- ✅ Tự động lấy đúng AccountId của user đang đăng nhập

---

## 🎯 TÓM TẮT:

1. ✅ **Đăng nhập** → Lấy JWT Token
2. ✅ **Copy token** từ response
3. ✅ **Gửi GET request** với Header `Authorization: Bearer {token}`
4. ✅ **KHÔNG CẦN** truyền ID vào URL hay Body
5. ✅ Backend tự động lấy AccountId từ Token

---

## 💡 TIP: Lưu Token trong Postman Environment

1. Tạo Environment trong Postman
2. Thêm variable `token`
3. Sau khi login, set `token = {value từ response}`
4. Dùng `{{token}}` trong Authorization header

**Ví dụ:**
```
Authorization: Bearer {{token}}
```

---

**🎉 Vậy là bạn không cần truyền ID, chỉ cần Token là đủ!**

