using SmartHomes.Application.Services;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Infrastructure.Data;
using SmartHomes.Infrastructure.Repositories;
using SmartHomes.Services.Soap.Services;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Obter connection string do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("Postgres");

// Validar que a connection string existe
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'Postgres' não encontrada no appsettings.json");
}

// Registar DatabaseConnection
builder.Services.AddSingleton(new DatabaseConnection(connectionString));

// Registar Repositories
builder.Services.AddScoped<IHomeRepository, HomeRepository>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
builder.Services.AddScoped<IAlertRuleRepository, AlertRuleRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "zGOhnLACeFOmnBEyAGNB9IKUcvjDquzf";
var jwtExpirationMinutes = int.Parse(builder.Configuration["Jwt:ExpirationMinutes"] ?? "60");

// Registar AuthService
builder.Services.AddScoped<IAuthService>(provider =>
{
    var userRepo = provider.GetRequiredService<IUserRepository>();
    return new AuthService(userRepo, jwtSecret, jwtExpirationMinutes);
});

// Registar Application Services (logica de negocio)
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<ISensorReadingService, SensorReadingService>();
builder.Services.AddScoped<IAlertRuleService, AlertRuleService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();

// Registar Serviços SOAP
builder.Services.AddScoped<IHomeSoapService, HomeSoapService>();
builder.Services.AddScoped<ISensorSoapService, SensorSoapService>();
builder.Services.AddScoped<ISensorReadingSoapService, SensorReadingSoapService>();
builder.Services.AddScoped<IAlertRuleSoapService, AlertRuleSoapService>();
builder.Services.AddScoped<IAlertSoapService, AlertSoapService>();
builder.Services.AddScoped<IAuthSoapService, AuthSoapService>();

// Configurar SoapCore
builder.Services.AddSoapCore();

var app = builder.Build();

// Expor serviços SOAP nos endpoints
app.UseSoapEndpoint<IHomeSoapService>("/HomeSoapService.asmx", new SoapEncoderOptions());
app.UseSoapEndpoint<ISensorSoapService>("/SensorSoapService.asmx", new SoapEncoderOptions());
app.UseSoapEndpoint<ISensorReadingSoapService>("/SensorReadingSoapService.asmx", new SoapEncoderOptions());
app.UseSoapEndpoint<IAlertRuleSoapService>("/AlertRuleSoapService.asmx", new SoapEncoderOptions());
app.UseSoapEndpoint<IAlertSoapService>("/AlertSoapService.asmx", new SoapEncoderOptions());
app.UseSoapEndpoint<IAuthSoapService>("/AuthSoapService.asmx", new SoapEncoderOptions());



app.Run();