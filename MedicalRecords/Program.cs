using System.Data;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MedicalRecords.Application;
using MedicalRecords.Domain.Contracts;
using MedicalRecords.Domain.Models;
using MedicalRecords.Infrastructure;
using MedicalRecords.Infrastructure.CommonServices;
using MedicalRecords.Infrastructure.Repositories;
using Serilog;
using MedicalRecords.Middlewares;

var builder = WebApplication.CreateBuilder(args);
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
//register DB connetion

builder.Services.AddSingleton<IDbConnection>(_ => new SqliteConnection("Data Source=patiens.db"));
SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//to add authorize inside swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Your API", Version = "v1" });

    // 🔐 Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token in this format: Bearer {your token here}"
    });

    
});
builder.Services.AddControllers();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
//builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IPatientService, PatientService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5050");
});

builder.Services.AddHttpClient("MiniAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5050"); // MiniAPI base URL
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration;
        options.IncludeErrorDetails = true; // Enables detailed error messages

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();





//add serilog
Log.Logger = new LoggerConfiguration()
    // .WriteTo.Console()
    .WriteTo.File("logs/log.txt", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
        retainedFileCountLimit: 7) // Keep logs for the last 7 days
    .CreateLogger();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

builder.Host.UseSerilog();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var records = new List<MedicalRecord>();


app.MapGet("/medicalrecords/{patientId}", (Guid patientId) =>
{
    var result = records.FirstOrDefault(r => r.PatientId == patientId);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapPost("/medicalrecords", (MedicalRecord record) =>
{
    var existing = records.FirstOrDefault(r => r.PatientId == record.PatientId);
    if (existing is not null) records.Remove(existing);
    record.LastUpdated = DateTime.UtcNow;
    records.Add(record);
    return Results.Ok(record);
});


app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();
app.Run();