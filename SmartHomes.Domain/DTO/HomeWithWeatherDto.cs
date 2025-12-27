using System;

namespace SmartHomes.Domain.DTO;

/// <summary>
/// DTO que combina dados da casa com informacao meteorologica
/// </summary>
public class HomeWithWeatherDto
{
    // Dados da casa
    public Guid HomeId { get; set; }
    public string HomeName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    // Temperatura interior (media dos sensores de temperatura)
    public decimal? IndoorTemperature { get; set; }

    // Dados meteorologicos externos
    public WeatherDto? Weather { get; set; }

    // Comparacao
    public decimal? TemperatureDifference { get; set; }
    public string? Recommendation { get; set; }
}