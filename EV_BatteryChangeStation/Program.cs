using EV_BatteryChangeStation.Services;
using EV_BatteryChangeStation_Common.Configuration;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Service.Service;
using Microsoft.EntityFrameworkCore;
using static EV_BatteryChangeStation_Common.Configuration.EmailSettings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Đăng ký DbContext với DI
builder.Services.AddDbContext<EvbatterySwapContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký cấu hình EmailSettings (đọc từ appsettings.json)
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
// Đăng ký EmailService
builder.Services.AddScoped<EmailService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
