using System.Text;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding
                            .UTF8.GetBytes(builder.Configuration["TokenKey"])),
                       ValidateIssuer = false,
                       ValidateAudience = false
                   };

               });

if (builder.Environment.IsDevelopment())
{

    builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(
     builder.Configuration.GetConnectionString("DefaultConnection")
     ));
    //connString = builder.Configuration.GetConnectionString("DefaultConnection");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(builder => builder
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()
     .WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
