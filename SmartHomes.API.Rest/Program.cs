using SmartHomes.API.Rest.Clients;
using SmartHomes.Application.Services;
using SmartHomes.Domain.Interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Enums aparecem como strings no JSON (ex: "Temperature" em vez de 1)
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Swagger mostra enums como strings também
    c.UseInlineDefinitionsForEnums();
});

// Obter URL base do serviço SOAP do appsettings.json
var soapServiceUrl = builder.Configuration["SoapService:Url"]
    ?? "http://localhost:5001";

// Registar clientes SOAP como Singleton
builder.Services.AddSingleton(new HomeSoapClient($"{soapServiceUrl}/HomeSoapService.asmx"));
builder.Services.AddSingleton(new SensorSoapClient($"{soapServiceUrl}/SensorSoapService.asmx"));
builder.Services.AddSingleton(new SensorReadingSoapClient($"{soapServiceUrl}/SensorReadingSoapService.asmx"));
builder.Services.AddSingleton(new AlertRuleSoapClient($"{soapServiceUrl}/AlertRuleSoapService.asmx"));
builder.Services.AddSingleton(new AlertSoapClient($"{soapServiceUrl}/AlertSoapService.asmx"));
builder.Services.AddSingleton(new AuthSoapClient($"{soapServiceUrl}/AuthSoapService.asmx"));


// Configurar HttpClient para WeatherService
//builder.Services.AddHttpClient<IWeatherService, WeatherService>((serviceProvider, client) =>
//{
//    var config = serviceProvider.GetRequiredService<IConfiguration>();
//    var apiKey = config["OpenWeather:ApiKey"] ?? "d80ffaee2cee46bf65d80759cbb39afe";
//    client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
//});

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();