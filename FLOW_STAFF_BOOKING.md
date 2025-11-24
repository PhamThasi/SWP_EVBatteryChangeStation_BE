# 🔐 FLOW: STAFF XEM BOOKING THEO CHI NHÁNH

## ❓ CÂU HỎI CỦA BẠN:
> "Tôi là Staff, tôi có StationID. Tôi cần hiển thị Booking của StationID của tôi. 
> Chỉ được phép thấy những booking được chọn ở các trạm này."
> 
> **Cần validate ở FE hay BE?**

---

## ✅ TRẢ LỜI NGẮN GỌN:

### **VALIDATE Ở ĐÂU?**
- ✅ **BACKEND (BE) - BẮT BUỘC:** Phải validate ở BE để bảo mật
- ⚠️ **FRONTEND (FE) - TÙY CHỌN:** Chỉ để UX tốt hơn, KHÔNG đảm bảo bảo mật

### **TẠI SAO PHẢI VALIDATE Ở BE?**
- ❌ FE có thể bị hack, user có thể sửa code
- ❌ User có thể gọi API trực tiếp (Postman, curl) bỏ qua FE
- ✅ BE là nơi duy nhất đảm bảo bảo mật 100%

---

## 🗺️ FLOW CHI TIẾT

### **SCENARIO: Staff xem danh sách Booking**

```
┌─────────────────────────────────────────────────────────────┐
│ 1. STAFF ĐĂNG NHẬP                                          │
│    - Login với AccountName/Email + Password                 │
│    - Backend trả về JWT Token                               │
│    - Token chứa: AccountId, AccountName, Email, RoleName   │
└──────────────────────┬──────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────────────────┐
│ 2. FRONTEND LƯU TOKEN                                        │
│    - Lưu token vào localStorage/sessionStorage               │
│    - Gửi token trong Header mỗi request:                    │
│      Authorization: Bearer {token}                           │
└──────────────────────┬──────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────────────────┐
│ 3. STAFF XEM DANH SÁCH BOOKING                               │
│    - FE gọi: GET /api/booking/staff/my-bookings               │
│    - Header: Authorization: Bearer {token}                    │
└──────────────────────┬──────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────────────────┐
│ 4. BACKEND XỬ LÝ (QUAN TRỌNG!)                              │
│    a) Lấy AccountId từ JWT Token (từ Claims)                │
│    b) Query Account từ DB để lấy StationID                  │
│    c) Validate: Account phải là Staff (Role = "Staff")      │
│    d) Query Booking WHERE StationId = Account.StationId      │
│    e) Trả về danh sách Booking của Station đó               │
└──────────────────────┬──────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────────────────┐
│ 5. FRONTEND HIỂN THỊ                                         │
│    - Nhận danh sách Booking                                  │
│    - Hiển thị lên UI                                         │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔍 CHI TIẾT TỪNG BƯỚC

### **BƯỚC 1: Lấy thông tin Staff từ JWT Token**

**Trong Controller:**
```csharp
// Lấy AccountId từ JWT Token
var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                  ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
{
    return Unauthorized("Invalid token");
}
```

**Hoặc tạo Helper Method:**
```csharp
// Tạo extension method để dễ dùng
public static Guid? GetCurrentAccountId(this ClaimsPrincipal user)
{
    var accountIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                      ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    
    if (Guid.TryParse(accountIdClaim, out Guid accountId))
        return accountId;
    
    return null;
}
```

---

### **BƯỚC 2: Query Account để lấy StationID**

**Trong Service:**
```csharp
// Lấy Account từ DB
var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
if (account == null)
    return new ServiceResult(404, "Account not found", null);

// Validate: Phải là Staff
if (account.Role?.RoleName != "Staff")
    return new ServiceResult(403, "Only Staff can access this endpoint", null);

// Validate: Staff phải có StationID
if (account.StationId == null)
    return new ServiceResult(400, "Staff is not assigned to any station", null);

var stationId = account.StationId.Value;
```

---

### **BƯỚC 3: Query Booking theo StationID**

**Trong Repository:**
```csharp
public async Task<List<Booking>> GetByStationIdAsync(Guid stationId)
{
    return await _context.Bookings
        .Where(b => b.StationId == stationId)
        .Include(b => b.Station)
        .Include(b => b.Vehicle)
        .Include(b => b.Account)
        .OrderByDescending(b => b.DateTime)
        .ToListAsync();
}
```

---

## 📝 CODE IMPLEMENTATION

### **1. Thêm Method vào IBookingRepository:**
```csharp
// EV_BatteryChangeStation_Repository/IRepositories/IBookingRepository.cs
Task<List<Booking>> GetByStationIdAsync(Guid stationId);
```

### **2. Implement trong BookingRepository:**
```csharp
// EV_BatteryChangeStation_Repository/Repositories/BookingRepository.cs
public async Task<List<Booking>> GetByStationIdAsync(Guid stationId)
{
    return await _context.Bookings
        .Where(b => b.StationId == stationId)
        .Include(b => b.Station)
        .Include(b => b.Vehicle)
        .Include(b => b.Account)
        .OrderByDescending(b => b.DateTime)
        .ToListAsync();
}
```

### **3. Thêm Method vào IBookingService:**
```csharp
// EV_BatteryChangeStation_Service/InternalService/IService/IBookingService.cs
Task<ServiceResult> GetByStaffStationAsync(Guid staffAccountId);
```

### **4. Implement trong BookingService:**
```csharp
// EV_BatteryChangeStation_Service/InternalService/Service/BookingService.cs
public async Task<ServiceResult> GetByStaffStationAsync(Guid staffAccountId)
{
    try
    {
        // 1. Lấy Account của Staff
        var staff = await _unitOfWork.AccountRepository.GetByIdAsync(staffAccountId);
        if (staff == null)
            return new ServiceResult(404, "Staff not found", null, BookingErrorCode.BookingNotFound);

        // 2. Validate: Phải là Staff
        if (staff.Role?.RoleName != "Staff")
            return new ServiceResult(403, "Only Staff can access this endpoint", null, BookingErrorCode.BookingNotFound);

        // 3. Validate: Staff phải có StationID
        if (staff.StationId == null)
            return new ServiceResult(400, "Staff is not assigned to any station", null, BookingErrorCode.BookingNotFound);

        // 4. Lấy Booking theo StationID
        var stationId = staff.StationId.Value;
        var bookings = await _unitOfWork.BookingRepository.GetByStationIdAsync(stationId);
        
        if (bookings == null || !bookings.Any())
            return new ServiceResult(404, "No bookings found for this station", null, BookingErrorCode.BookingNotFound);

        // 5. Map sang DTO và trả về
        var result = bookings.Select(BookingMapper.ToDTO).ToList();
        return new ServiceResult(200, "Success", result, BookingErrorCode.None);
    }
    catch (Exception ex)
    {
        return new ServiceResult(500, "Error fetching bookings", new List<string> { ex.Message }, BookingErrorCode.DatabaseError);
    }
}
```

### **5. Thêm Endpoint vào BookingController:**
```csharp
// EV_BatteryChangeStation/Controllers/BookingController.cs
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

/// <summary>
/// Lấy danh sách booking của Station mà Staff đang làm việc
/// </summary>
[HttpGet("staff/my-bookings")]
[Authorize] // Yêu cầu đăng nhập
public async Task<IActionResult> GetMyStationBookings()
{
    // Lấy AccountId từ JWT Token
    var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                      ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

    if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
    {
        return Unauthorized("Invalid token");
    }

    var result = await _bookingService.GetByStaffStationAsync(accountId);
    return StatusCode(result.Status, result);
}
```

---

## 🎯 CÁC API ENDPOINT CẦN TẠO

### **1. Staff xem Booking của Station mình:**
```
GET /api/booking/staff/my-bookings
Headers: Authorization: Bearer {token}
Response: List<BookingDTO> của Station mà Staff đang làm việc
```

### **2. Admin xem Booking của tất cả Station:**
```
GET /api/booking/SelectAll/
Headers: Authorization: Bearer {token}
Response: List<BookingDTO> của tất cả Station
```

### **3. Admin xem Booking của một Station cụ thể:**
```
GET /api/booking/by-station/{stationId}
Headers: Authorization: Bearer {token}
Response: List<BookingDTO> của Station đó
```

---

## 🔒 VALIDATION RULES

### **KHI STAFF XEM BOOKING:**
1. ✅ Phải đăng nhập (có JWT Token)
2. ✅ Account phải có Role = "Staff"
3. ✅ Staff phải có StationID (không null)
4. ✅ Chỉ trả về Booking của Station mà Staff đang làm việc
5. ❌ KHÔNG được xem Booking của Station khác

### **KHI ADMIN XEM BOOKING:**
1. ✅ Phải đăng nhập (có JWT Token)
2. ✅ Account phải có Role = "Admin"
3. ✅ Có thể xem Booking của TẤT CẢ Station

---

## 🚫 CÁC TRƯỜNG HỢP LỖI

### **1. Staff chưa được gán Station:**
```json
{
  "status": 400,
  "message": "Staff is not assigned to any station",
  "data": null
}
```

### **2. Staff cố gắng xem Booking của Station khác:**
```json
{
  "status": 403,
  "message": "You can only view bookings of your assigned station",
  "data": null
}
```

### **3. User không phải Staff:**
```json
{
  "status": 403,
  "message": "Only Staff can access this endpoint",
  "data": null
}
```

---

## 📊 SO SÁNH: FE vs BE VALIDATION

| Tiêu chí | Frontend (FE) | Backend (BE) |
|----------|---------------|--------------|
| **Bảo mật** | ❌ Có thể bị hack | ✅ Bảo mật 100% |
| **Mục đích** | UX tốt hơn | Bảo mật thực sự |
| **Bắt buộc?** | ⚠️ Không | ✅ Có |
| **Ví dụ** | Filter UI, ẩn button | Validate trong Service |

---

## ✅ CHECKLIST IMPLEMENTATION

### **Repository Layer:**
- [ ] Thêm `GetByStationIdAsync(Guid stationId)` vào `IBookingRepository`
- [ ] Implement method trong `BookingRepository`

### **Service Layer:**
- [ ] Thêm `GetByStaffStationAsync(Guid staffAccountId)` vào `IBookingService`
- [ ] Implement method trong `BookingService` với validation:
  - [ ] Validate Account tồn tại
  - [ ] Validate Role = "Staff"
  - [ ] Validate StationID không null
  - [ ] Query Booking theo StationID

### **Controller Layer:**
- [ ] Thêm endpoint `GET /api/booking/staff/my-bookings`
- [ ] Lấy AccountId từ JWT Token
- [ ] Gọi Service method
- [ ] Trả về kết quả

### **Testing:**
- [ ] Test với Staff có StationID → Trả về Booking đúng
- [ ] Test với Staff không có StationID → Trả về lỗi
- [ ] Test với Customer → Trả về 403
- [ ] Test với Admin → Có thể xem tất cả

---

## 🎓 KẾT LUẬN

1. ✅ **VALIDATE Ở BE LÀ BẮT BUỘC** - Đảm bảo bảo mật
2. ⚠️ **VALIDATE Ở FE LÀ TÙY CHỌN** - Chỉ để UX tốt hơn
3. 🔐 **LUÔN LẤY StationID TỪ DB** - Không tin tưởng data từ FE
4. 🚫 **KHÔNG BAO GIỜ** cho phép Staff xem Booking của Station khác

---

## 📌 LƯU Ý QUAN TRỌNG

> ⚠️ **KHÔNG BAO GIỜ** nhận StationID từ Frontend!
> 
> ❌ SAI: `GET /api/booking?stationId={stationId}` (User có thể fake)
> ✅ ĐÚNG: `GET /api/booking/staff/my-bookings` (Lấy từ JWT Token)

> ⚠️ **LUÔN** validate Role và StationID ở Backend!
> 
> - Lấy AccountId từ JWT Token
> - Query Account từ DB
> - Validate Role = "Staff"
> - Validate StationID không null
> - Query Booking theo StationID từ DB

