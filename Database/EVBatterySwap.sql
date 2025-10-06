CREATE DATABASE EVBatterySwap;
GO

USE EVBatterySwap;
GO

-- ========================
-- Table: Role
-- ========================
CREATE TABLE Role (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(100) NOT NULL,
    Status BIT NOT NULL,
    CreateDate DATETIME DEFAULT GETDATE(),
    UpdateDate DATETIME NULL
);

-- ========================
-- Table: Station
-- ========================
CREATE TABLE Station (
    StationID INT PRIMARY KEY IDENTITY(1,1),
    Address NVARCHAR(255),
    PhoneNumber NVARCHAR(20),
    Status BIT,
    AccountName NVARCHAR(100),
    BatteryQuantity int
);

-- ========================
-- Table: Account
-- ========================
CREATE TABLE Account (
    AccountID INT PRIMARY KEY IDENTITY(1,1),
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
    RoleID INT NOT NULL FOREIGN KEY REFERENCES Role(RoleID),
    StationID INT NULL FOREIGN KEY REFERENCES Station(StationID) -- staff thuộc station nào
);

-- ========================
-- Table: Subscription
-- ========================
CREATE TABLE Subscription (
    SubscriptionID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    ExtraFee DECIMAL(18,2),
    Description NVARCHAR(255),
    DurationPackage INT,
    IsActive BIT DEFAULT 1,
    CreateDate DATETIME DEFAULT GETDATE(),
    UpdateDate DATETIME NULL,
    AccountID INT NOT NULL FOREIGN KEY REFERENCES [Account](AccountID) -- gói thuộc về user
);

-- ========================
-- Table: Payment (1-1 với Subscription)
-- ========================
CREATE TABLE Payment (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    Price DECIMAL(18,2),
    Method NVARCHAR(50),
    Status BIT,
    CreateDate DATETIME DEFAULT GETDATE(),
    SubscriptionID INT UNIQUE FOREIGN KEY REFERENCES Subscription(SubscriptionID)
);

-- ========================
-- Table: Car (Không nối Account trực tiếp)
-- ========================
CREATE TABLE Car (
    VehicleID INT PRIMARY KEY IDENTITY(1,1),
    Model NVARCHAR(100),
    BatteryType NVARCHAR(100),
    Producer NVARCHAR(100),
    CreateDate DATETIME DEFAULT GETDATE()
);

-- ========================
-- Table: Battery (M-1 Station)
-- ========================
CREATE TABLE Battery (
    BatteryID INT PRIMARY KEY IDENTITY(1,1),
    Capacity DECIMAL(10,2),
    LastUsed DATETIME,
    Status BIT,
    StateOfHealth DECIMAL(5,2),
    PercentUse DECIMAL(5,2),
    TypeBattery NVARCHAR(100),
    BatterySwapDate DATETIME,
    InsuranceDate DATE,
    StationID INT NOT NULL FOREIGN KEY REFERENCES Station(StationID)
);

-- ========================
-- Table: Booking (M-1 Station, M-1 Car, M-1 Account)
-- ========================
CREATE TABLE Booking (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    DateTime DATETIME NOT NULL,
    Notes NVARCHAR(100),
    Status BIT,
    CreatedDate DATETIME DEFAULT GETDATE(),
    StationID INT NOT NULL FOREIGN KEY REFERENCES Station(StationID),
    VehicleID INT NOT NULL FOREIGN KEY REFERENCES Car(VehicleID),
    AccountID INT NOT NULL FOREIGN KEY REFERENCES [Account](AccountID) -- ai đặt booking
);

-- ========================
-- Table: SupportRequest (chỉ nối với Account)
-- ========================
CREATE TABLE SupportRequest (
    RequestID INT PRIMARY KEY IDENTITY(1,1),
    IssueType NVARCHAR(100),
    Description NVARCHAR(255),
    CreateDate DATETIME DEFAULT GETDATE(),
    Status BIT, -- 0 = pending, 1 = resolved
    AccountID INT NOT NULL, -- user gửi request
    StaffID INT NULL,       -- staff xử lý
    ResponseText NVARCHAR(255) NULL,
    ResponseDate DATETIME NULL,
    CONSTRAINT FK_SupportRequest_User FOREIGN KEY (AccountID) REFERENCES [Account](AccountID),
    CONSTRAINT FK_SupportRequest_Staff FOREIGN KEY (StaffID) REFERENCES [Account](AccountID)
);

-- ========================
-- Table: Feedback (M-1 Booking, M-1 Account)
-- ========================
CREATE TABLE Feedback (
    FeedbackID INT PRIMARY KEY IDENTITY(1,1),
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(255),
    CreateDate DATETIME DEFAULT GETDATE(),
    AccountID INT NOT NULL FOREIGN KEY REFERENCES [Account](AccountID),
    BookingID INT NOT NULL FOREIGN KEY REFERENCES Booking(BookingID)
);

-- ========================
-- Table: SwappingTransaction
-- ========================
CREATE TABLE SwappingTransaction (
    TransactionID INT PRIMARY KEY IDENTITY(1,1),
    Notes NVARCHAR(255),
    StaffID INT NOT NULL FOREIGN KEY REFERENCES [Account](AccountID),
    OldBatteryID INT NOT NULL,
    VehicleID INT NOT NULL FOREIGN KEY REFERENCES Car(VehicleID),
    NewBatteryID INT NOT NULL FOREIGN KEY REFERENCES Battery(BatteryID),
    CreateDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT CK_Swap_Battery CHECK (OldBatteryID <> NewBatteryID)
);

-- Bổ sung Payment liên kết với Transaction (nếu cần)
ALTER TABLE Payment
ADD TransactionID INT UNIQUE FOREIGN KEY REFERENCES SwappingTransaction(TransactionID);

-- ========================
-- END
-- ========================
/*
USE master;
GO
ALTER DATABASE EVBatterySwap SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
DROP DATABASE EVBatterySwap;
GO
*/

-- Role
select * from Role
INSERT INTO Role (RoleName, Status) VALUES
(N'Admin', 1),
(N'Staff', 1),
(N'Customer', 1);

-- Station
INSERT INTO Station (Address, PhoneNumber, Status, AccountName, BatteryQuantity) VALUES
(N'123 Lê Lợi, Hà Nội', '0901234567', 1, N'StationHN01', N'2'),
(N'456 Nguyễn Huệ, TP.HCM', '0902345678', 1, N'StationHCM01', N'1');

-- Account
INSERT INTO Account (AccountName, FullName, Password, Email, Gender, Address, PhoneNumber, DateOfBirth, Status, RoleID, StationID)
VALUES
(N'admin01', N'Nguyễn Văn Admin', 'admin@123', 'admin01@gmail.com', N'Nam', N'Hà Nội', '0911111111', '1980-01-01', 1, 1, NULL),
(N'staffHN', N'Lê Thị Staff', 'staff@123', 'staffHN@gmail.com', N'Nữ', N'Hà Nội', '0922222222', '1990-05-10', 1, 2, 1),
(N'staffHCM', N'Trần Văn Staff', 'staff@123', 'staffHCM@gmail.com', N'Nam', N'TP.HCM', '0933333333', '1992-07-15', 1, 2, 2),
(N'customer01', N'Phạm Minh Khách', 'cus@123', 'customer01@gmail.com', N'Nam', N'Hà Nội', '0944444444', '2000-02-20', 1, 3, NULL);

-- Subscription
INSERT INTO Subscription (Name, Price, ExtraFee, Description, DurationPackage, IsActive, AccountID)
VALUES
(N'Gói cơ bản', 500000, 50000, N'Dùng 30 ngày, giới hạn 10 lần đổi pin', 30, 1, 4),
(N'Gói nâng cao', 1000000, 100000, N'Dùng 30 ngày, không giới hạn đổi pin', 30, 1, 4);

-- Car
INSERT INTO Car (Model, BatteryType, Producer)
VALUES
(N'VinFast E34', N'Lithium-ion', N'VinFast'),
(N'Tesla Model 3', N'Lithium-ion', N'Tesla');

-- Battery
INSERT INTO Battery (Capacity, LastUsed, Status, StateOfHealth, PercentUse, TypeBattery, BatterySwapDate, InsuranceDate, StationID)
VALUES
(50.0, GETDATE(), 1, 95.5, 70.2, N'Lithium-ion', GETDATE(), '2026-01-01', 1),
(60.0, GETDATE(), 1, 97.0, 80.1, N'Lithium-ion', GETDATE(), '2026-01-01', 1),
(55.0, GETDATE(), 1, 90.0, 65.0, N'Lithium-ion', GETDATE(), '2026-01-01', 2);

-- Booking
INSERT INTO Booking (DateTime, Notes, Status, StationID, VehicleID, AccountID)
VALUES
(GETDATE(), N'Đổi pin lần 1', 1, 1, 1, 4),
(GETDATE(), N'Đổi pin lần 2', 1, 2, 2, 4);

-- SupportRequest
INSERT INTO SupportRequest (IssueType, Description, Status, AccountID, StaffID, ResponseText, ResponseDate)
VALUES
(N'Vấn đề thanh toán', N'Tôi bị trừ tiền 2 lần', 1, 4, 2, N'Đã hoàn tiền', GETDATE()),
(N'Booking lỗi', N'Không thể đặt lịch', 1, 4, 3, N'Đã xử lý, mời thử lại', GETDATE());

-- Feedback
INSERT INTO Feedback (Rating, Comment, AccountID, BookingID)
VALUES
(5, N'Dịch vụ rất tốt', 4, 1),
(4, N'Ổn nhưng cần cải thiện tốc độ xử lý', 4, 2);

-- SwappingTransaction
INSERT INTO SwappingTransaction (Notes, StaffID, OldBatteryID, VehicleID, NewBatteryID)
VALUES
(N'Đổi pin thành công', 2, 1, 1, 2),
(N'Đổi pin nhanh chóng', 3, 3, 2, 1);

-- Cập nhật Payment liên kết với Transaction
UPDATE Payment SET TransactionID = 1 WHERE PaymentID = 1;
UPDATE Payment SET TransactionID = 2 WHERE PaymentID = 2;

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

DROP TRIGGER IF EXISTS trg_AfterDelete_Battery;
GO

DROP TRIGGER IF EXISTS trg_AfterInsert_Battery;
GO
