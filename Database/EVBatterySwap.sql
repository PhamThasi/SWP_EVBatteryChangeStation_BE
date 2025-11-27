-- ========================================
-- DATABASE: EVBatterySwap (GUID VERSION)
-- ========================================

create DATABASE EVBatterySwap;
GO
USE EVBatterySwap;
GO

-- ========================
-- Table: Role
-- ========================
CREATE TABLE Role (
    RoleID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    RoleName NVARCHAR(100) NOT NULL,
    Status BIT NOT NULL,
    CreateDate DATETIME DEFAULT GETDATE(),
    UpdateDate DATETIME NULL
);

-- ========================
-- Table: Station
-- ========================
CREATE TABLE Station (
    StationID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Address NVARCHAR(255),
    PhoneNumber NVARCHAR(20),
    Status BIT,
    StationName NVARCHAR(100),
    BatteryQuantity INT
);

-- ========================
-- Table: Account
-- ========================
CREATE TABLE Account (
    AccountID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    AccountName NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(150),
    Password NVARCHAR(255) NOT NULL,
    Email NVARCHAR(150) UNIQUE,
    Gender NVARCHAR(10),
    Address NVARCHAR(255),
    PhoneNumber NVARCHAR(20),
    DateOfBirth DATE,
    Status BIT NOT NULL,
    CreateDate DATETIME DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    RoleID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Role(RoleID),
    StationID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Station(StationID)
);

-- ========================
-- Table: Subscription
-- ========================
CREATE TABLE Subscription (
    SubscriptionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    ExtraFee DECIMAL(18,2),
    Description NVARCHAR(255),
    DurationPackage INT,
    IsActive BIT DEFAULT 1,
    CreateDate DATETIME DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    -- Ngày bắt đầu và ngày hết hạn của gói khi gắn với một Account cụ thể
    StartDate DATETIME NULL,
    EndDate DATETIME NULL,
    -- Số lượt swap còn lại (nếu gói có giới hạn lượt)
    RemainingSwaps INT NULL,
    AccountID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Account(AccountID)
);

-- ========================
-- Table: Car
-- ========================
CREATE TABLE Car (
    VehicleID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Model NVARCHAR(100),
    BatteryType NVARCHAR(100),
	Images Nvarchar(1000),
    Producer NVARCHAR(100),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Available',
    CreateDate DATETIME DEFAULT GETDATE()
);

-- ========================
-- Table: Battery
-- ========================
CREATE TABLE Battery (
    BatteryID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Capacity DECIMAL(10,2),
    LastUsed DATETIME,
    Status BIT,
    StateOfHealth DECIMAL(5,2),
    PercentUse DECIMAL(5,2),
    TypeBattery NVARCHAR(100),
    BatterySwapDate DATETIME,
    InsuranceDate DATE,
    StationID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Station(StationID)
);

-- ========================
-- Table: Booking
-- ========================
CREATE TABLE Booking (
    BookingID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    DateTime DATETIME NOT NULL,
    Notes NVARCHAR(100),
	IsApproved Nvarchar(50) not null,
    CreatedDate DATETIME DEFAULT GETDATE(),
    StationID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Station(StationID),
    VehicleID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Car(VehicleID),
    AccountID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Account(AccountID),
    BatteryID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Battery(BatteryID)
);

-- ========================
-- Table: SwappingTransaction
-- ========================
CREATE TABLE SwappingTransaction (
    TransactionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Notes NVARCHAR(255),
    StaffID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Account(AccountID),
    VehicleID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Car(VehicleID),
    NewBatteryID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Battery(BatteryID),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
    CreateDate DATETIME DEFAULT GETDATE()
);

-- ========================
-- Table: Payment
-- ========================
CREATE TABLE Payment (
    PaymentID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Price DECIMAL(18,2),
    Method NVARCHAR(50),
    Status Nvarchar(100) not null default 'Pending',
	PaymentGateId BigInt,
    CreateDate DATETIME DEFAULT GETDATE(),
    SubscriptionID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Subscription(SubscriptionID),
    TransactionID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES SwappingTransaction(TransactionID),
    -- AccountId để lưu user mua subscription (bắt buộc nếu có SubscriptionId)
    AccountID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES Account(AccountID)
);

-- Tạo filtered unique index cho TransactionID (chỉ áp dụng UNIQUE cho giá trị không NULL)
-- Cho phép nhiều NULL nhưng mỗi giá trị khác NULL phải unique
CREATE UNIQUE NONCLUSTERED INDEX UQ__Payment__55433A4A4B1B216D 
ON Payment(TransactionID) 
WHERE TransactionID IS NOT NULL;

-- ========================
-- Table: SupportRequest
-- ========================
CREATE TABLE SupportRequest (
    RequestID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    IssueType NVARCHAR(100),
    Description NVARCHAR(255),
    CreateDate DATETIME DEFAULT GETDATE(),
    Status BIT,
    AccountID UNIQUEIDENTIFIER NOT NULL,
    StaffID UNIQUEIDENTIFIER NULL,
    ResponseText NVARCHAR(255) NULL,
    ResponseDate DATETIME NULL,
    CONSTRAINT FK_SupportRequest_User FOREIGN KEY (AccountID) REFERENCES Account(AccountID),
    CONSTRAINT FK_SupportRequest_Staff FOREIGN KEY (StaffID) REFERENCES Account(AccountID)
);

-- ========================
-- Table: Feedback
-- ========================
CREATE TABLE Feedback (
    FeedbackID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(255),
    CreateDate DATETIME DEFAULT GETDATE(),
    AccountID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Account(AccountID),
    BookingID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Booking(BookingID)
);
GO

-- ========================
-- TRIGGERS
-- ========================

-- Khi thêm pin
CREATE TRIGGER trg_AfterInsert_Battery
ON Battery
AFTER INSERT
AS
BEGIN
    UPDATE Station
    SET BatteryQuantity = BatteryQuantity + 1
    WHERE StationId IN (SELECT StationId FROM Inserted);
END
GO

-- Khi xóa pin
CREATE TRIGGER trg_AfterDelete_Battery
ON Battery
AFTER DELETE
AS
BEGIN
    UPDATE Station
    SET BatteryQuantity = BatteryQuantity - 1
    WHERE StationId IN (SELECT StationId FROM Deleted);
END
GO

-- ========================
-- DỮ LIỆU MẪU
-- ========================


-- Role
DECLARE @roleAdmin UNIQUEIDENTIFIER = NEWID();
DECLARE @roleStaff UNIQUEIDENTIFIER = NEWID();
DECLARE @roleCustomer UNIQUEIDENTIFIER = NEWID();

INSERT INTO Role (RoleID, RoleName, Status)
VALUES
(@roleAdmin, N'Admin', 1),
(@roleStaff, N'Staff', 1),
(@roleCustomer, N'Customer', 1);

-- Station
DECLARE @stationHN UNIQUEIDENTIFIER = NEWID();
DECLARE @stationHCM UNIQUEIDENTIFIER = NEWID();
DECLARE @stationDN UNIQUEIDENTIFIER = NEWID();
DECLARE @stationHP UNIQUEIDENTIFIER = NEWID();

INSERT INTO Station (StationID, Address, PhoneNumber, Status, StationName, BatteryQuantity)
VALUES
(@stationHN,  N'115 Hữu Nghị, Ngọc Hà, Ba Đình, Hà Nội', '0901234567', 1, N'StationHN01', 2),
(@stationHCM, N'53 Nguyễn Huệ, Bến Nghé, Quận 1, TP. Hồ Chí Minh',  '0902345678', 1, N'StationHCM01', 1),
(@stationDN,  N'167 Trần Phú, Hải Châu, Đà Nẵng', '0903456789', 1, N'StationDN01', 0),
(@stationHP,  N'207 Lạch Tray, Ngô Quyền, Hải Phòng', '0904567890', 1, N'StationHP01', 0);

-- Account
DECLARE @admin UNIQUEIDENTIFIER = NEWID();
DECLARE @staffHN UNIQUEIDENTIFIER = NEWID();
DECLARE @staffHCM UNIQUEIDENTIFIER = NEWID();
DECLARE @customer UNIQUEIDENTIFIER = NEWID();
DECLARE @staffDN UNIQUEIDENTIFIER = NEWID();
DECLARE @staffHP UNIQUEIDENTIFIER = NEWID();
DECLARE @customer2 UNIQUEIDENTIFIER = NEWID();
DECLARE @customer3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Account (AccountID, AccountName, FullName, Password, Email, Gender, Address, PhoneNumber, DateOfBirth, Status, RoleID, StationID)
VALUES
(@admin, N'admin01', N'Nguyễn Văn Admin', 'admin@123', 'admin01@gmail.com', N'Nam', N'Hà Nội', '0911111111', '1980-01-01', 1, @roleAdmin, NULL),
(@staffHN, N'staffHN', N'Lê Thị Staff', 'staff@123', 'staffHN@gmail.com', N'Nữ', N'Hà Nội', '0922222222', '1990-05-10', 1, @roleStaff, @stationHN),
(@staffHCM, N'staffHCM', N'Trần Văn Staff', 'staff@123', 'staffHCM@gmail.com', N'Nam', N'TP.HCM', '0933333333', '1992-07-15', 1, @roleStaff, @stationHCM),
(@customer, N'customer01', N'Phạm Minh Khách', 'cus@123', 'customer01@gmail.com', N'Nam', N'Hà Nội', '0944444444', '2000-02-20', 1, @roleCustomer, NULL),
(@staffDN, N'staffDN', N'Nguyễn Văn Staff DN', 'staff@123', 'staffDN@gmail.com', N'Nam', N'Đà Nẵng', '0951111111', '1988-03-12', 1, @roleStaff, @stationDN),
(@staffHP, N'staffHP', N'Lê Thị Staff HP', 'staff@123', 'staffHP@gmail.com', N'Nữ', N'Hải Phòng', '0952222222', '1991-09-20', 1, @roleStaff, @stationHP),
(@customer2, N'customer02', N'Nguyễn Minh Khách 2', 'cus@123', 'customer02@gmail.com', N'Nam', N'Đà Nẵng', '0963333333', '2001-04-10', 1, @roleCustomer, NULL),
(@customer3, N'customer03', N'Phạm Thị Khách 3', 'cus@123', 'customer03@gmail.com', N'Nữ', N'Hải Phòng', '0974444444', '1999-08-22', 1, @roleCustomer, NULL);

-- Subscription
DECLARE @subBasic UNIQUEIDENTIFIER = NEWID();
DECLARE @subPremium UNIQUEIDENTIFIER = NEWID();
DECLARE @subSave UNIQUEIDENTIFIER = NEWID();
INSERT INTO Subscription (SubscriptionID, Name, Price, ExtraFee, Description, DurationPackage, IsActive, AccountID)
VALUES
(@subBasic, N'Thanh toán trực tiếp', 4500000, 50000, N'Trả tiền thuê pin', 30, 1, null),
(@subSave, N'Gói Tiết kiệm', 2990000, 350000, N'Thuê pin theo tháng, giá thấp hơn 20–30% so với gói cơ bản. Bao gồm 10–15 lượt swap miễn phí mỗi tháng. Ưu tiên tại trạm khi đổi pin trong giờ cao điểm. Theo dõi số lần swap còn lại ngay trên app. Thanh toán tự động – có thể gia hạn gói dễ dàng', 120, 1, null),
(@subPremium, N'Gói nâng cao', 10000000, 100000, N'Swap không giới hạn – đổi pin bất cứ lúc nào. Bảo dưỡng & kiểm tra tình trạng pin định kỳ miễn phí. Ưu tiên hàng đầu tại mọi trạm trên hệ thống. Hỗ trợ kỹ thuật 24/7 qua hotline hoặc app. Giảm giá 10–15% khi đăng ký thêm phương tiện khác', 30, 1, null);

-- Car
DECLARE @carE34 UNIQUEIDENTIFIER = NEWID();
DECLARE @carTesla UNIQUEIDENTIFIER = NEWID();
DECLARE @carVin UNIQUEIDENTIFIER = NEWID();
DECLARE @carBYD UNIQUEIDENTIFIER = NEWID();
DECLARE @carSolid UNIQUEIDENTIFIER = NEWID();

INSERT INTO Car (VehicleID, Model, BatteryType, Producer)
VALUES

(@carE34, N'VinFast E34', N'Lithium-ion', N'VinFast'),
(@carTesla, N'Tesla Model 3', N'Lithium-ion', N'Tesla'),
(@carVin, N'VinFast VF8', N'Lithium-ion', N'VinFast'),
(@carBYD, N'BYD Atto 3', N'Lithium-ion', N'BYD'),
(@carSolid, N'SolidCar X1', N'Solid-state', N'SolidCar');
-- Battery
DECLARE @batt1 UNIQUEIDENTIFIER = NEWID();
DECLARE @batt2 UNIQUEIDENTIFIER = NEWID();
DECLARE @batt3 UNIQUEIDENTIFIER = NEWID();
DECLARE @batt4 UNIQUEIDENTIFIER = NEWID();
DECLARE @batt5 UNIQUEIDENTIFIER = NEWID();
DECLARE @batt6 UNIQUEIDENTIFIER = NEWID();
DECLARE @batt7 UNIQUEIDENTIFIER = NEWID();
DECLARE @battA UNIQUEIDENTIFIER = NEWID();
DECLARE @battB UNIQUEIDENTIFIER = NEWID();
DECLARE @battC UNIQUEIDENTIFIER = NEWID();
DECLARE @battD UNIQUEIDENTIFIER = NEWID();
INSERT INTO Battery (BatteryID, Capacity, LastUsed, Status, StateOfHealth, PercentUse, TypeBattery, BatterySwapDate, InsuranceDate, StationID)
VALUES

(@batt1, 50.0, GETDATE(), 1, 95.5, 70.2, N'Lithium-ion', GETDATE(), '2026-01-01', @stationHN),
(@batt2, 60.0, GETDATE(), 1, 97.0, 80.1, N'Lithium-ion', GETDATE(), '2026-01-01', @stationHN),
(@batt3, 55.0, GETDATE(), 1, 90.0, 65.0, N'Lithium-ion', GETDATE(), '2026-01-01', @stationHCM),
(@batt4, 45.0, GETDATE(), 1, 92.0, 50.0, N'Lithium-ion', GETDATE(), '2026-01-01', @stationDN),
(@batt5, 55.0, GETDATE(), 1, 95.0, 30.0, N'Lithium-ion', GETDATE(), '2026-01-01', @stationDN),
(@batt6, 60.0, GETDATE(), 1, 97.0, 20.0, N'Lithium-ion', GETDATE(), '2026-01-01', @stationHP),
(@batt7, 50.0, GETDATE(), 1, 90.0, 40.0, N'Lithium-ion', GETDATE(), '2026-01-01', @stationHP),
(@battA, 55.0, GETDATE(), 1, 100.0, 0.0, N'Solid-state', GETDATE(), '2026-01-01', @stationHN),
(@battB, 60.0, GETDATE(), 1, 100.0, 0.0, N'Solid-state', GETDATE(), '2026-01-01', @stationHN),
(@battC, 50.0, GETDATE(), 1, 100.0, 0.0, N'Solid-state', GETDATE(), '2026-01-01', @stationHN),
(@battD, 45.0, GETDATE(), 1, 100.0, 0.0, N'Solid-state', GETDATE(), '2026-01-01', @stationHN);

-- Booking
DECLARE @book1 UNIQUEIDENTIFIER = NEWID();
DECLARE @book2 UNIQUEIDENTIFIER = NEWID();
DECLARE @book3 UNIQUEIDENTIFIER = NEWID();
DECLARE @book4 UNIQUEIDENTIFIER = NEWID();
DECLARE @book5 UNIQUEIDENTIFIER = NEWID();
DECLARE @book6 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Booking (BookingID, DateTime, Notes, IsApproved, StationID, VehicleID, AccountID, BatteryID)
VALUES
(@book1, GETDATE(), N'Đổi pin lần 1', N'Approve', @stationHN, @carE34, @customer, @batt1),
(@book2, GETDATE(), N'Đổi pin lần 2', N'Approve', @stationHCM, @carTesla, @customer, @batt3),
(@book3, GETDATE(), N'Đổi pin VF8', N'Approve', @stationDN, @carVin, @customer2, @batt4),
(@book4, GETDATE(), N'Đổi pin BYD', N'Approve', @stationHP, @carBYD, @customer3, @batt6),
(@book5, GETDATE(), N'Đổi pin VinFast E34', N'Approve', @stationDN, @carE34, @customer2, @batt5),
(@book6, GETDATE(), N'Đổi pin Tesla Model 3', N'Approve', @stationHP, @carTesla, @customer3, @batt7);

-- SupportRequest
INSERT INTO SupportRequest (IssueType, Description, Status, AccountID, StaffID, ResponseText, ResponseDate)
VALUES
(N'Vấn đề thanh toán', N'Tôi bị trừ tiền 2 lần', 1, @customer, @staffHN, N'Đã hoàn tiền', GETDATE()),
(N'Booking lỗi', N'Không thể đặt lịch', 1, @customer, @staffHCM, N'Đã xử lý, mời thử lại', GETDATE()),
(N'Pin lỗi', N'Pin không sạc được', 1, @customer, @staffDN, N'Đã đổi pin mới', GETDATE()),
(N'Vấn đề app', N'App không hiển thị lịch sử', 1, @customer, @staffHP, N'Đã xử lý', GETDATE());


-- Feedback
INSERT INTO Feedback (Rating, Comment, AccountID, BookingID)
VALUES
(5, N'Dịch vụ rất tốt', @customer, @book1),
(4, N'Ổn nhưng cần cải thiện tốc độ xử lý', @customer, @book2),
(5, N'Rất hài lòng', @customer2, @book3),
(4, N'Tốt nhưng cần cải thiện', @customer3, @book4);

/*
-- SwappingTransaction
DECLARE @trans1 UNIQUEIDENTIFIER = NEWID();
DECLARE @trans2 UNIQUEIDENTIFIER = NEWID();
DECLARE @trans3 UNIQUEIDENTIFIER = NEWID();
DECLARE @trans4 UNIQUEIDENTIFIER = NEWID();
DECLARE @trans5 UNIQUEIDENTIFIER = NEWID();
DECLARE @trans6 UNIQUEIDENTIFIER = NEWID();
INSERT INTO SwappingTransaction (TransactionID, Notes, StaffID, VehicleID, NewBatteryID)
VALUES
(@trans1, N'Đổi pin thành công', @staffHN, @carE34, @batt2),
(@trans2, N'Đổi pin nhanh chóng', @staffHCM, @carTesla, @batt1),
(@trans3, N'Đổi pin VF8 thành công', @staffDN, @carVin, @batt5),
(@trans4, N'Đổi pin BYD thành công', @staffHP, @carBYD, @batt7),
(@trans5, N'Đổi pin VinFast E34 thành công', @staffDN, @carE34, @batt4),
(@trans6, N'Đổi pin Tesla Model 3 thành công', @staffHP, @carTesla, @batt6);
*/
-- Payment liên kết Transaction
/*
INSERT INTO Payment (PaymentID, Price, Method, Status, SubscriptionID, TransactionID, PaymentGateId)
VALUES 
(NEWID(), 500000, N'Credit Card', 'Pending', null, @trans1, ABS(CHECKSUM(NEWID())) % 10000000000),
(NEWID(), 1000000, N'Momo', 'Pending', @subPremium, @trans2, ABS(CHECKSUM(NEWID())) % 10000000000),
(NEWID(), 600000, N'Momo', 'Pending', null, @trans4, ABS(CHECKSUM(NEWID())) % 10000000000),
(NEWID(), 700000, N'VNPAY', 'Pending', @subPremium, @trans5, ABS(CHECKSUM(NEWID())) % 10000000000),
(NEWID(), 800000, N'Credit Card', 'Pending', @subPremium, @trans6, ABS(CHECKSUM(NEWID())) % 10000000000);
*/
/*
USE master;
GO
ALTER DATABASE EVBatterySwap SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
DROP DATABASE EVBatterySwap;
GO
*/
