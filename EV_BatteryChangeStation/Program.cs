using EV_BatteryChangeStation_Repository.UnitOfWork;
using EV_BatteryChangeStation_Service.InternalService.IService;
using EV_BatteryChangeStation_Service.InternalService.Service;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//đăng kí service
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasher<EV_BatteryChangeStation_Repository.Entities.Account>, PasswordHasher<EV_BatteryChangeStation_Repository.Entities.Account>>();
builder.Services.AddScoped<IAuthenService, AuthenService>();

//đăng kí unit of work
builder.Services.AddScoped<UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.UseAuthorization();

app.MapControllers();

app.Run();
