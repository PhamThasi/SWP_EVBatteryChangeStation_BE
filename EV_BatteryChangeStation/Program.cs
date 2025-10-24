using EV_BatteryChangeStation_Repository.IRepositories;
using EV_BatteryChangeStation_Repository.Repositories;
using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.ExternalService.IService;
using EV_BatteryChangeStation_Service.ExternalService.Service;
using EV_BatteryChangeStation_Service.InternalService.IService;
using EV_BatteryChangeStation_Service.InternalService.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using EV_BatteryChangeStation_Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using EV_BatteryChangeStation_Repository.Base;
using VNPAY.NET;


var builder = WebApplication.CreateBuilder(args);
//Dang ki SupportRequest
builder.Services.AddScoped<ISupportRequestService, SupportRequestService>();
//Dang ki Subscription
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
//Dang ki FeedBack
builder.Services.AddScoped<IFeedBackService, FeedBackService>();
//Dang ki Station
builder.Services.AddScoped<IStationService, StationService>();
//Dang ki Booking
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
// Cấu hình DbContext với connection string
builder.Services.AddDbContext<EVBatterySwapContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Đăng kí service
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IBatteryService, BatteryService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ISwappingService, SwappingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPasswordHasher<EV_BatteryChangeStation_Repository.Entities.Account>, PasswordHasher<EV_BatteryChangeStation_Repository.Entities.Account>>();
builder.Services.AddScoped<IAuthenService, AuthenService>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IVnpay, Vnpay>();
// Đăng kí cho JWT service
var jwtSettings = builder.Configuration.GetSection("JwtConfig"); // lấy từ appsettings.json

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthenService>();

            if (authService.IsTokenRevoked(token))
            {
                context.Fail("Token has been revoked (logout).");
            }

            return Task.CompletedTask;
        }
    };
});

// Đăng kí unit of work
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Cấu hình swagger để sử dụng JWT Bearer
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:5205") // URL frontend của bạn
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // nếu cần gửi cookie/token
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cấu hình email
var email = builder.Configuration["EmailSettings:Email"];
var appPassword = builder.Configuration["EmailSettings:AppPassword"];

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();  
app.UseAuthorization();

app.MapControllers();

app.Run();
