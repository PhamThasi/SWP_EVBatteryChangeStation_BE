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
    BatteryQuality NVARCHAR(100)
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
    OldBatteryID INT NOT NULL FOREIGN KEY REFERENCES Battery(BatteryID),
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
