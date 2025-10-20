-- ========================================
-- DATABASE: EVBatterySwap (GUID VERSION)
-- ========================================

CREATE DATABASE EVBatterySwap;
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
    AccountName NVARCHAR(100),
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
    AccountID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Account(AccountID)
);

-- ========================
-- Table: Payment
-- ========================
CREATE TABLE Payment (
    PaymentID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Price DECIMAL(18,2),
    Method NVARCHAR(50),
    Status BIT,
    CreateDate DATETIME DEFAULT GETDATE(),
    SubscriptionID UNIQUEIDENTIFIER UNIQUE FOREIGN KEY REFERENCES Subscription(SubscriptionID)
);

-- ========================
-- Table: Car
-- ========================
CREATE TABLE Car (
    VehicleID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Model NVARCHAR(100),
    BatteryType NVARCHAR(100),
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
    Status BIT,
    CreatedDate DATETIME DEFAULT GETDATE(),
    StationID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Station(StationID),
    VehicleID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Car(VehicleID),
    AccountID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Account(AccountID)
);

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

-- ========================
-- Table: SwappingTransaction
-- ========================
CREATE TABLE SwappingTransaction (
    TransactionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Notes NVARCHAR(255),
    StaffID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Account(AccountID),
    OldBatteryID UNIQUEIDENTIFIER NOT NULL,
    VehicleID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Car(VehicleID),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
    NewBatteryID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Battery(BatteryID),
    CreateDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT CK_Swap_Battery CHECK (OldBatteryID <> NewBatteryID)
);

-- Bổ sung Payment liên kết với Transaction
ALTER TABLE Payment
ADD TransactionID UNIQUEIDENTIFIER UNIQUE FOREIGN KEY REFERENCES SwappingTransaction(TransactionID);
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

INSERT INTO Station (StationID, Address, PhoneNumber, Status, AccountName, BatteryQuantity)
VALUES
(@stationHN, N'123 Lê Lợi, Hà Nội', '0901234567', 1, N'StationHN01', 2),
(@stationHCM, N'456 Nguyễn Huệ, TP.HCM', '0902345678', 1, N'StationHCM01', 1);

-- Account
DECLARE @admin UNIQUEIDENTIFIER = NEWID();
DECLARE @staffHN UNIQUEIDENTIFIER = NEWID();
DECLARE @staffHCM UNIQUEIDENTIFIER = NEWID();
DECLARE @customer UNIQUEIDENTIFIER = NEWID();

INSERT INTO Account (AccountID, AccountName, FullName, Password, Email, Gender, Address, PhoneNumber, DateOfBirth, Status, RoleID, StationID)
VALUES
(@admin, N'admin01', N'Nguyễn Văn Admin', 'admin@123', 'admin01@gmail.com', N'Nam', N'Hà Nội', '0911111111', '1980-01-01', 1, @roleAdmin, NULL),
(@staffHN, N'staffHN', N'Lê Thị Staff', 'staff@123', 'staffHN@gmail.com', N'Nữ', N'Hà Nội', '0922222222', '1990-05-10', 1, @roleStaff, @stationHN),
(@staffHCM, N'staffHCM', N'Trần Văn Staff', 'staff@123', 'staffHCM@gmail.com', N'Nam', N'TP.HCM', '0933333333', '1992-07-15', 1, @roleStaff, @stationHCM),
(@customer, N'customer01', N'Phạm Minh Khách', 'cus@123', 'customer01@gmail.com', N'Nam', N'Hà Nội', '0944444444', '2000-02-20', 1, @roleCustomer, NULL);

-- Subscription
DECLARE @subBasic UNIQUEIDENTIFIER = NEWID();
DECLARE @subPremium UNIQUEIDENTIFIER = NEWID();

INSERT INTO Subscription (SubscriptionID, Name, Price, ExtraFee, Description, DurationPackage, IsActive, AccountID)
VALUES
(@subBasic, N'Gói cơ bản', 500000, 50000, N'Dùng 30 ngày, giới hạn 10 lần đổi pin', 30, 1, @customer),
(@subPremium, N'Gói nâng cao', 1000000, 100000, N'Dùng 30 ngày, không giới hạn đổi pin', 30, 1, @customer);

-- Car
DECLARE @carE34 UNIQUEIDENTIFIER = NEWID();
DECLARE @carTesla UNIQUEIDENTIFIER = NEWID();

INSERT INTO Car (VehicleID, Model, BatteryType, Producer)
VALUES
(@carE34, N'VinFast E34', N'Lithium-ion', N'VinFast'),
(@carTesla, N'Tesla Model 3', N'Lithium-ion', N'Tesla');

-- Battery
DECLARE @batt1 UNIQUEIDENTIFIER = NEWID();
DECLARE @batt2 UNIQUEIDENTIFIER = NEWID();
DECLARE @batt3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Battery (BatteryID, Capacity, LastUsed, Status, StateOfHealth, PercentUse, TypeBattery, BatterySwapDate, InsuranceDate, StationID)
VALUES
(@batt1, 50.0, GETDATE(), 1, 95.5, 70.2, N'Lithium-ion', GETDATE(), '2026-01-01', @stationHN),
(@batt2, 60.0, GETDATE(), 1, 97.0, 80.1, N'Lithium-ion', GETDATE(), '2026-01-01', @stationHN),
(@batt3, 55.0, GETDATE(), 1, 90.0, 65.0, N'Lithium-ion', GETDATE(), '2026-01-01', @stationHCM);

-- Booking
DECLARE @book1 UNIQUEIDENTIFIER = NEWID();
DECLARE @book2 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Booking (BookingID, DateTime, Notes, Status, StationID, VehicleID, AccountID)
VALUES
(@book1, GETDATE(), N'Đổi pin lần 1', 1, @stationHN, @carE34, @customer),
(@book2, GETDATE(), N'Đổi pin lần 2', 1, @stationHCM, @carTesla, @customer);

-- SupportRequest
INSERT INTO SupportRequest (IssueType, Description, Status, AccountID, StaffID, ResponseText, ResponseDate)
VALUES
(N'Vấn đề thanh toán', N'Tôi bị trừ tiền 2 lần', 1, @customer, @staffHN, N'Đã hoàn tiền', GETDATE()),
(N'Booking lỗi', N'Không thể đặt lịch', 1, @customer, @staffHCM, N'Đã xử lý, mời thử lại', GETDATE());

-- Feedback
INSERT INTO Feedback (Rating, Comment, AccountID, BookingID)
VALUES
(5, N'Dịch vụ rất tốt', @customer, @book1),
(4, N'Ổn nhưng cần cải thiện tốc độ xử lý', @customer, @book2);

-- SwappingTransaction
DECLARE @trans1 UNIQUEIDENTIFIER = NEWID();
DECLARE @trans2 UNIQUEIDENTIFIER = NEWID();

INSERT INTO SwappingTransaction (TransactionID, Notes, StaffID, OldBatteryID, VehicleID, NewBatteryID)
VALUES
(@trans1, N'Đổi pin thành công', @staffHN, @batt1, @carE34, @batt2),
(@trans2, N'Đổi pin nhanh chóng', @staffHCM, @batt3, @carTesla, @batt1);

-- Payment liên kết Transaction
INSERT INTO Payment (PaymentID, Price, Method, Status, SubscriptionID, TransactionID)
VALUES 
(NEWID(), 500000, N'Credit Card', 1, @subBasic, @trans1),
(NEWID(), 1000000, N'Momo', 1, @subPremium, @trans2);
GO

/*
USE master;
GO
ALTER DATABASE [TenDatabase] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
DROP DATABASE [TenDatabase];
GO
*/