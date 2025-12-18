using SmartHomes.API.Rest.Clients;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Obter URL do serviço SOAP
var soapServiceUrl = builder.Configuration["SoapService:Url"]
    ?? "http://localhost:5001/HomeSoapService.asmx";

// Registar cliente SOAP
builder.Services.AddSingleton(new HomeSoapClient(soapServiceUrl));

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