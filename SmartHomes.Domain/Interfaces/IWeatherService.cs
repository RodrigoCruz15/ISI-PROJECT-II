using System.Threading.Tasks;
using SmartHomes.Domain.DTO;

namespace SmartHomes.Domain.Interfaces;

/// <summary>
/// Define o contrato para integracao com API de meteorologia
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Obtem dados meteorologicos para uma localizacao
    /// </summary>
    /// <param name="latitude">Latitude da localizacao</param>
    /// <param name="longitude">Longitude da localizacao</param>
    /// <returns>Dados meteorologicos</returns>
    Task<WeatherDto?> GetWeatherByCoordinatesAsync(decimal latitude, decimal longitude);
}