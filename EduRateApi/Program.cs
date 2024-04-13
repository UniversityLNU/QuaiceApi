
using EduRateApi.Implementation;
using EduRateApi.Interfaces;
using EduRateApi.Models;
using FirebaseAdmin;
using FirebaseAdminAuthentication.DependencyInjection.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

var policy = "MyPolicy";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, (o) => { });

builder.Services.AddSingleton(FirebaseApp.Create());
builder.Services.AddScoped<FirebaseAuthenticationFunctionHandler>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IFundraisingService, FundraisingService>();

builder.Services.AddScoped<IPostService, PostService>();
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
