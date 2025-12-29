using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartHomes.API.Rest.Clients;
using SmartHomes.Application.Services;
using SmartHomes.Domain.Interfaces;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration["JwtSecret"]
                ?? builder.Configuration["Jwt:Secret"]
                ?? "zGOhnLACeFOmnBEyAGNB9IKUcvjDquzf!";

// COLOQUE O LINK DIRETO AQUI PARA TESTE (SEM VARIAVEIS DO AZURE)
var soapServiceUrl = "https://smarthomesservicessoap-c9ezdeb5caedeagz.francecentral-01.azurewebsites.net/";



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Enums aparecem como strings no JSON (ex: "Temperature" em vez de 1)
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500", "https://rodrigocruz15.github.io")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Configurar Autenticação com esquemas padrão
builder.Services.AddAuthentication(options =>
{
    // Estas 3 linhas dizem ao .NET para usar sempre JWT
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.UseInlineDefinitionsForEnums();

    // 1. Definir o esquema de segurança (Como o Swagger deve pedir o token)
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 2. Aplicar o requisito de segurança globalmente (Adiciona o cadeado nos métodos)
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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

app.UseCors("AllowFrontend");


// Configurar pipeline HTTP

    app.UseSwagger();
    app.UseSwaggerUI();

//app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();