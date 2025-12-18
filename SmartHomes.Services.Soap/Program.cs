using SoapCore;
using SmartHomes.Services.Soap.Services;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Infrastructure.Data;
using SmartHomes.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Obter connection string do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("Postgres");

// Registar serviços
builder.Services.AddSingleton(new DatabaseConnection(connectionString!));
builder.Services.AddScoped<IHomeRepository, HomeRepository>();
builder.Services.AddScoped<IHomeSoapService, HomeSoapService>();

// Configurar SoapCore
builder.Services.AddSoapCore();

var app = builder.Build();

// Expor o serviço SOAP no endpoint /HomeSoapService.asmx
app.UseSoapEndpoint<IHomeSoapService>("/HomeSoapService.asmx", new SoapEncoderOptions());

app.Run();