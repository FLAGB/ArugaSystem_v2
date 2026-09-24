using AndroidWebAPI.Data;
using AndroidWebAPI.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;    
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AndroidWebAPI.DTOs;
using AndroidWebAPI.Repositories;
using AndroidWebAPI.Services;
var builder = WebApplication.CreateBuilder(args);

var hash = BCrypt.Net.BCrypt.HashPassword("Admin12345");
Console.WriteLine("ADMIN HASH:");
Console.WriteLine(hash);


// ── Ports ────────────────────────────────────────────────────
builder.WebHost.UseUrls(
    "http://localhost:57147",
    "http://localhost:57148"
);

// ── Services ─────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddScoped<VaccineRepository>();
builder.Services.AddScoped<VaccineDoseRepository>();
builder.Services.AddScoped<VaccineInventoryRepository>();
builder.Services.AddScoped<IVaccinationTimelineRepository, VaccinationTimelineRepository>();
builder.Services.AddScoped<IQueueRepository, QueueRepository>();
builder.Services.AddScoped<IQueueQRSettingRepository, QueueQRSettingRepository>();
builder.Services.AddScoped<IQueueQRCodeRepository, QueueQRCodeRepository>();
builder.Services.AddScoped<IQueueQRCodeRepository, QueueQRCodeRepository>();
builder.Services.AddScoped<IClinicOperatingScheduleRepository, ClinicOperatingScheduleRepository>();

builder.Services.AddScoped<
    IChildParentRelationshipRepository,
    ChildParentRelationshipRepository>();
builder.Services.AddScoped<IVaccinationRecordRepository, VaccinationRecordRepository>();

builder.Services.AddScoped<IVaccinationScheduleRuleRepository, VaccinationScheduleRuleRepository>();

builder.Services.AddScoped<IChildrenRepository, ChildrenRepository>();
builder.Services.AddScoped<ParentRepository>();          // ← only once
builder.Services.AddScoped<IAccountRepository, AccountRepositoryImpl>();
builder.Services.AddDbContext<AppDbContext>(options =>   // ← only once
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddHostedService<NotificationGeneratorService>();  // ← only once
// Queue QR Code
builder.Services.AddScoped<IQueueQRCodeRepository, QueueQRCodeRepository>();
builder.Services.AddScoped<IQueueQRCodeService, QueueQRCodeService>();

// Clinic Schedule
builder.Services.AddScoped<IClinicOperatingScheduleRepository, ClinicOperatingScheduleRepository>();

// ── Build ─────────────────────────────────────────────────────
var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowVueApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();