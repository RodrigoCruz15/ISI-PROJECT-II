using SmartHomes.API.Rest.Clients;
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

// Adicionar serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Obter URL base do serviço SOAP do appsettings.json
var soapServiceUrl = builder.Configuration["SoapService:Url"]
    ?? "http://localhost:5001";

// Registar clientes SOAP como Singleton
builder.Services.AddSingleton(new HomeSoapClient($"{soapServiceUrl}/HomeSoapService.asmx"));
builder.Services.AddSingleton(new SensorSoapClient($"{soapServiceUrl}/SensorSoapService.asmx"));
builder.Services.AddSingleton(new SensorReadingSoapClient($"{soapServiceUrl}/SensorReadingSoapService.asmx"));
builder.Services.AddSingleton(new AlertRuleSoapClient($"{soapServiceUrl}/AlertRuleSoapService.asmx"));
builder.Services.AddSingleton(new AlertSoapClient($"{soapServiceUrl}/AlertSoapService.asmx"));

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