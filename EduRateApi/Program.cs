
using EduRateApi.Implementation;
using EduRateApi.Interfaces;
using EduRateApi.Models;
using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

var policy = "MyPolicy";
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton(FirebaseApp.Create());

builder.Services.AddScoped<IFirebaseConnectingService, FirebaseConnectingService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IFundraisingService, FundraisingService>();

builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddScoped<IShopService, ShopService>();

builder.Services.AddScoped<IMailService, MailServer>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policy, policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors(policy);
app.Run();
