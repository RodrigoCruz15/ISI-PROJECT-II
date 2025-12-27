using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace SmartHomes.Application.Services;

/// <summary>
/// Servico de integracao com OpenWeatherMap API
/// </summary>
public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

    public WeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeather:ApiKey"] ?? throw new ArgumentNullException("OpenWeather API Key não configurada");
    }

    /// <summary>
    /// Obtem dados meteorologicos para coordenadas especificas
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <returns>Dados meteorologicos ou null se erro</returns>
    public async Task<WeatherDto?> GetWeatherByCoordinatesAsync(decimal latitude, decimal longitude)
    {
        try
        {
            // Construir URL com parametros
            var url = $"{BaseUrl}?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric&lang=pt";

            // Fazer pedido HTTP GET
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            // Ler resposta JSON
            var jsonContent = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<OpenWeatherMapResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (weatherData == null)
                return null;

            // Mapear para WeatherDto
            return MapToDto(weatherData);
        }
        catch (Exception)
        {
            // Log erro (pode adicionar logging aqui)
            return null;
        }
    }

    // Classes internas para deserializar resposta da API
    private class OpenWeatherMapResponse
    {
        public MainData Main { get; set; } = new();
        public WeatherInfo[] Weather { get; set; } = Array.Empty<WeatherInfo>();
        public WindData Wind { get; set; } = new();
        public string Name { get; set; } = string.Empty;
    }

    private class MainData
    {
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
    }

    private class WeatherInfo
    {
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    private class WindData
    {
        public double Speed { get; set; }
    }

    private static WeatherDto MapToDto(OpenWeatherMapResponse data)
    {
        return new WeatherDto
        {
            Temperature = (decimal)data.Main.Temp,
            FeelsLike = (decimal)data.Main.FeelsLike,
            Humidity = data.Main.Humidity,
            Description = data.Weather[0].Description,
            Icon = data.Weather[0].Icon,
            WindSpeed = (decimal)data.Wind.Speed,
            City = data.Name
        };
    }
}