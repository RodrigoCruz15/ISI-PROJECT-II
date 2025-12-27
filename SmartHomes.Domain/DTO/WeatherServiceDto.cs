namespace SmartHomes.Domain.DTO;

/// <summary>
/// DTO com dados meteorologicos
/// </summary>
public class WeatherDto
{
    /// <summary>
    /// Temperatura atual em Celsius
    /// </summary>
    public decimal Temperature { get; set; }

    /// <summary>
    /// Sensacao termica em Celsius
    /// </summary>
    public decimal FeelsLike { get; set; }

    /// <summary>
    /// Humidade relativa (%)
    /// </summary>
    public int Humidity { get; set; }

    /// <summary>
    /// Descricao do clima (ex: "Clear sky", "Cloudy")
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Icone do clima (codigo OpenWeather)
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Velocidade do vento (m/s)
    /// </summary>
    public decimal WindSpeed { get; set; }

    /// <summary>
    /// Cidade/local
    /// </summary>
    public string City { get; set; } = string.Empty;
}