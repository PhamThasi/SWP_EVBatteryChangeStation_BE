using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EV_BatteryChangeStation_Repository.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Car",
                columns: table => new
                {
                    VehicleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BatteryType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Producer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Car__476B54B2FBCD2BC6", x => x.VehicleID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__8AFACE3A71AC50DB", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Station",
                columns: table => new
                {
                    StationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BatteryQuality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Station__E0D8A6DD21EBD158", x => x.StationID);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    StationID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account__349DA586C236EBB6", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK__Account__RoleID__3E52440B",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID");
                    table.ForeignKey(
                        name: "FK__Account__Station__3F466844",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID");
                });

            migrationBuilder.CreateTable(
                name: "Battery",
                columns: table => new
                {
                    BatteryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacity = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    LastUsed = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    StateOfHealth = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    PercentUse = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TypeBattery = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BatterySwapDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InsuranceDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Battery__5710803E6A6769CC", x => x.BatteryID);
                    table.ForeignKey(
                        name: "FK__Battery__Station__4E88ABD4",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    BookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    StationID = table.Column<int>(type: "int", nullable: false),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Booking__73951ACDE8C52343", x => x.BookingID);
                    table.ForeignKey(
                        name: "FK__Booking__Account__5441852A",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK__Booking__Station__52593CB8",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "StationID");
                    table.ForeignKey(
                        name: "FK__Booking__Vehicle__534D60F1",
                        column: x => x.VehicleID,
                        principalTable: "Car",
                        principalColumn: "VehicleID");
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    SubscriptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExtraFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DurationPackage = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    AccountID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subscrip__9A2B24BDAB8E8A64", x => x.SubscriptionID);
                    table.ForeignKey(
                        name: "FK__Subscript__Accou__440B1D61",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "SupportRequest",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    ResponseText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ResponseDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SupportR__33A8519A9D6769AC", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_SupportRequest_Staff",
                        column: x => x.StaffID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK_SupportRequest_User",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "SwappingTransaction",
                columns: table => new
                {
                    TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StaffID = table.Column<int>(type: "int", nullable: false),
                    OldBatteryID = table.Column<int>(type: "int", nullable: false),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    NewBatteryID = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Swapping__55433A4BF6A8C536", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK__SwappingT__NewBa__6383C8BA",
                        column: x => x.NewBatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID");
                    table.ForeignKey(
                        name: "FK__SwappingT__Staff__619B8048",
                        column: x => x.StaffID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK__SwappingT__Vehic__628FA481",
                        column: x => x.VehicleID,
                        principalTable: "Car",
                        principalColumn: "VehicleID");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    BookingID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Feedback__6A4BEDF6F3665690", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK__Feedback__Accoun__5DCAEF64",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK__Feedback__Bookin__5EBF139D",
                        column: x => x.BookingID,
                        principalTable: "Booking",
                        principalColumn: "BookingID");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    SubscriptionID = table.Column<int>(type: "int", nullable: true),
                    TransactionID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__9B556A585E4E527C", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK__Payment__Subscri__48CFD27E",
                        column: x => x.SubscriptionID,
                        principalTable: "Subscription",
                        principalColumn: "SubscriptionID");
                    table.ForeignKey(
                        name: "FK__Payment__Transac__6754599E",
                        column: x => x.TransactionID,
                        principalTable: "SwappingTransaction",
                        principalColumn: "TransactionID");
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleID", "RoleName", "Status", "UpdateDate" },
                values: new object[,]
                {
                    { 1, "Admin", false, null },
                    { 2, "Staff", false, null },
                    { 3, "Customer", false, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleID",
                table: "Account",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Account_StationID",
                table: "Account",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "UQ__Account__A9D105343631B7EE",
                table: "Account",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Battery_StationID",
                table: "Battery",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_AccountID",
                table: "Booking",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_StationID",
                table: "Booking",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_VehicleID",
                table: "Booking",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_AccountID",
                table: "Feedback",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_BookingID",
                table: "Feedback",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "UQ__Payment__55433A4A74209A6B",
                table: "Payment",
                column: "TransactionID",
                unique: true,
                filter: "[TransactionID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Payment__9A2B24BC99BDB0F9",
                table: "Payment",
                column: "SubscriptionID",
                unique: true,
                filter: "[SubscriptionID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_AccountID",
                table: "Subscription",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequest_AccountID",
                table: "SupportRequest",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequest_StaffID",
                table: "SupportRequest",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_SwappingTransaction_NewBatteryID",
                table: "SwappingTransaction",
                column: "NewBatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_SwappingTransaction_StaffID",
                table: "SwappingTransaction",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_SwappingTransaction_VehicleID",
                table: "SwappingTransaction",
                column: "VehicleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "SupportRequest");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "SwappingTransaction");

            migrationBuilder.DropTable(
                name: "Battery");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Car");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Station");
        }
    }
}
