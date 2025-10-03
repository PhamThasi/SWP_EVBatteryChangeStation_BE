using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.InternalService.IService;
using EV_BatteryChangeStation_Service.InternalService.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EV_BatteryChangeStation_Service.ExternalService.IService;
using EV_BatteryChangeStation_Service.ExternalService.Service;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Đăng kí service
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasher<EV_BatteryChangeStation_Repository.Entities.Account>, PasswordHasher<EV_BatteryChangeStation_Repository.Entities.Account>>();
builder.Services.AddScoped<IAuthenService, AuthenService>();
builder.Services.AddScoped<IJWTService, JWTService>();

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
});

// Đăng kí unit of work
builder.Services.AddScoped<UnitOfWork>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cấu hình email
var email = builder.Configuration["EmailSettings:Email"];
var appPassword = builder.Configuration["EmailSettings:AppPassword"];

//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.Events = new JwtBearerEvents
//        {
//            OnMessageReceived = context =>
//            {
//                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

//                // Lấy service để check token revoke
//                var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthenService>();
//                if (authService.IsTokenRevoked(token))
//                {
//                    context.Fail("Token đã bị thu hồi (logout).");
//                }

//                return Task.CompletedTask;
//            }
//        };
//    });

app.UseHttpsRedirection();

app.UseAuthentication();  
app.UseAuthorization();

app.MapControllers();

app.Run();
