# 📋 PHÂN TÍCH NGHIỆP VỤ - QUẢN LÝ PIN VÀ CHI NHÁNH

## 🎯 CÁC YÊU CẦU BẮT BUỘC (PHẢI LÀM)

### 1. ✅ QUẢN LÍ PIN ĐẾN MỨC ID CỦA TỪNG PIN
**Hiện trạng:**
- Database đã có `Battery.BatteryID` (UNIQUEIDENTIFIER) ✅
- Đã có các API quản lý pin theo ID ✅
- **THIẾU:** Chưa có validation đầy đủ khi chọn pin cụ thể

**Cần làm:**
- ✅ Pin đã được quản lý theo ID (đã có sẵn)
- ⚠️ Cần thêm validation: Pin phải tồn tại, phải available, phải đúng loại xe

---

### 2. 🔴 KHI BOOK/THAY PIN: CHỌN ĐÚNG PIN THEO DÒNG XE + ID PIN CÓ TRONG KHO
**Hiện trạng:**
- ❌ `BookingCreateDTO` KHÔNG có field `BatteryId` 
- ❌ `CreateSwappingDto` có `NewBatteryId` nhưng KHÔNG validate:
  - Pin có đúng loại với xe không? (`Car.BatteryType` vs `Battery.TypeBattery`)
  - Pin có đang trong kho của Station không?
  - Pin có available không? (Status = true, StateOfHealth > 80)

**Cần làm:**
1. **Sửa Booking:**
   - Thêm `BatteryId?` vào `BookingCreateDTO` (optional khi book, required khi swap)
   - Validate trong `BookingService.CreateAsync()`:
     - Pin phải tồn tại
     - Pin phải thuộc Station được book
     - Pin phải đúng loại với `Car.BatteryType`
     - Pin phải available (Status = true, StateOfHealth > 80)

2. **Sửa Swapping:**
   - Validate trong `SwappingService.CreateTransactionAsync()`:
     - Pin phải đúng loại với xe
     - Pin phải available
     - Pin phải thuộc Station mà Staff đang làm việc

---

### 3. 🔴 GÁN NHÂN VIÊN VỀ ĐÚNG CHI NHÁNH, CHỈ ĐƯỢC LÀM Ở ĐÚNG CHI NHÁNH
**Hiện trạng:**
- ✅ `Account.StationID` đã có (Staff đã được gán Station)
- ❌ **THIẾU:** Validation khi Staff thực hiện swap:
  - Staff chỉ được swap ở Station của mình
  - Booking chỉ được approve bởi Staff của đúng Station

**Cần làm:**
1. **Sửa SwappingService:**
   - Validate `StaffId.StationId` == `Booking.StationId` (hoặc pin phải thuộc Station của Staff)
   - Nếu không đúng → trả lỗi "Staff không được phép làm việc ở Station này"

2. **Sửa BookingService:**
   - Khi approve booking, check Staff phải thuộc đúng Station

---

### 4. 🔴 TRANG THỐNG KÊ THEO CHI NHÁNH, THEO HỆ THỐNG
**Hiện trạng:**
- ❌ Chưa có API thống kê

**Cần làm:**
1. Tạo `StatisticsController` với các endpoint:
   - `GET /api/statistics/by-station/{stationId}` - Thống kê theo chi nhánh
   - `GET /api/statistics/system` - Thống kê toàn hệ thống

2. Dữ liệu thống kê cần có:
   - Số lượng pin theo trạng thái (Available, In Use, Maintenance)
   - Số lượng booking theo trạng thái (Pending, Approved, Completed, Canceled)
   - Số lượng swap đã thực hiện
   - Doanh thu theo tháng
   - Top xe được swap nhiều nhất
   - Pin có StateOfHealth thấp cần thay thế

---

## 🟡 CÁC YÊU CẦU TỰ CHỌN (OPTION)

### 5. ⚪ PIN THUỘC VỀ TỪNG CHI NHÁNH
**Hiện trạng:**
- ✅ `Battery.StationID` đã có → Pin đã thuộc về Station
- ✅ Đã có trigger tự động update `Station.BatteryQuantity`

**Kết luận:** ✅ ĐÃ CÓ SẴN, không cần làm gì thêm

---

### 6. ⚪ PHÂN NHẬN PIN THU HỒI (VỚI RANDOM % PIN CÒN LẠI)
**Hiện trạng:**
- ❌ Chưa có logic xử lý pin thu hồi
- `SwappingTransaction` chỉ có `NewBatteryId`, không có `OldBatteryId`

**Cần làm (nếu chọn):**
1. Thêm `OldBatteryId` vào `SwappingTransaction` (nullable)
2. Khi swap:
   - Lấy pin cũ từ xe (cần thêm bảng `CarCurrentBattery` hoặc field trong `Car`)
   - Tính % còn lại của pin cũ (dựa vào `PercentUse`)
   - Random % (ví dụ: 70-90% của giá trị thực tế) để tránh gian lận
   - Lưu pin cũ vào kho với `Status = true`, `PercentUse = random%`

---

## 📊 PHÂN TÍCH DATABASE

### ✅ CÁC BẢNG ĐÃ ĐỦ:
- `Battery` - Quản lý pin theo ID ✅
- `Station` - Chi nhánh ✅
- `Account` - Staff đã có `StationID` ✅
- `Booking` - Đặt lịch ✅
- `SwappingTransaction` - Giao dịch đổi pin ✅
- `Car` - Xe có `BatteryType` ✅

### ⚠️ CẦN BỔ SUNG:
1. **SwappingTransaction:**
   - Thêm `OldBatteryId` (nếu làm tính năng pin thu hồi)
   - Thêm `BookingId` (để link với booking)

2. **Booking:**
   - Thêm `BatteryId` (optional) - Pin dự kiến sẽ swap

---

## 🗺️ HƯỚNG ĐI THỰC HIỆN

### **BƯỚC 1: SỬA DATABASE (Nếu cần)**
```sql
-- Thêm OldBatteryId vào SwappingTransaction (nếu làm pin thu hồi)
ALTER TABLE SwappingTransaction
ADD OldBatteryId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Battery(BatteryID);

-- Thêm BookingId vào SwappingTransaction (để link)
ALTER TABLE SwappingTransaction
ADD BookingId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Booking(BookingID);

-- Thêm BatteryId vào Booking (pin dự kiến)
ALTER TABLE Booking
ADD BatteryId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Battery(BatteryID);
```

### **BƯỚC 2: SỬA DTOs**
- `BookingCreateDTO`: Thêm `BatteryId?`
- `CreateSwappingDto`: Thêm `OldBatteryId?`, `BookingId?`

### **BƯỚC 3: SỬA SERVICE LAYER (QUAN TRỌNG NHẤT)**
- `BookingService.CreateAsync()`: Validate pin
- `SwappingService.CreateTransactionAsync()`: Validate pin + Staff + Station
- Tạo `StatisticsService` mới

### **BƯỚC 4: SỬA CONTROLLER**
- `BookingController`: Nhận `BatteryId` từ request
- `SwappingController`: Validate trước khi swap
- Tạo `StatisticsController` mới

### **BƯỚC 5: SỬA REPOSITORY (Nếu cần query mới)**
- Thêm method query pin available theo Station + BatteryType
- Thêm method thống kê

---

## 🎯 THỨ TỰ ƯU TIÊN THỰC HIỆN

### **PRIORITY 1 (BẮT BUỘC):**
1. ✅ Validate pin khi Booking (đúng loại xe, có trong kho)
2. ✅ Validate pin khi Swapping (đúng loại xe, available)
3. ✅ Validate Staff chỉ làm việc ở đúng Station
4. ✅ Tạo API thống kê theo Station và System

### **PRIORITY 2 (NẾU CÓ THỜI GIAN):**
5. ⚪ Tính năng pin thu hồi với random %

---

## 📝 CHECKLIST THỰC HIỆN

### Database:
- [ ] Thêm `BatteryId` vào `Booking` (optional)
- [ ] Thêm `OldBatteryId` vào `SwappingTransaction` (nếu làm pin thu hồi)
- [ ] Thêm `BookingId` vào `SwappingTransaction`

### DTOs:
- [ ] Sửa `BookingCreateDTO` - thêm `BatteryId?`
- [ ] Sửa `CreateSwappingDto` - thêm `OldBatteryId?`, `BookingId?`
- [ ] Tạo DTOs cho Statistics

### Service Layer:
- [ ] `BookingService.CreateAsync()` - Validate pin
- [ ] `SwappingService.CreateTransactionAsync()` - Validate pin + Staff + Station
- [ ] Tạo `StatisticsService` với các method thống kê

### Repository:
- [ ] Thêm method `GetAvailableBatteriesByStationAndType(StationId, BatteryType)`
- [ ] Thêm method thống kê

### Controller:
- [ ] Sửa `BookingController` - nhận `BatteryId`
- [ ] Sửa `SwappingController` - validate
- [ ] Tạo `StatisticsController`

---

## 🔍 CÁC CÂU HỎI CẦN LÀM RÕ

1. **Khi Booking:** 
   - User có cần chọn pin ngay khi book không? Hay chỉ chọn khi đến swap?
   - → **Đề xuất:** Optional khi book, bắt buộc khi swap

2. **Pin thu hồi:**
   - Có cần track pin cũ không? Hay chỉ cần pin mới?
   - → **Đề xuất:** Nếu không làm tính năng thu hồi thì không cần `OldBatteryId`

3. **Thống kê:**
   - Cần thống kê theo khoảng thời gian nào? (ngày, tuần, tháng, năm)
   - → **Đề xuất:** Theo tháng là đủ, có thể filter theo date range

