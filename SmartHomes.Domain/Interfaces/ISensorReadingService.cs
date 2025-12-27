using SmartHomes.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.Interfaces
{
    public interface ISensorReadingService
    {
        /// <summary>
        /// Obtém uma leitura pelo seu identificador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SensorReadingDto?> GetReadingByIdAsync(Guid id);

        /// <summary>
        /// Obtém as leituras mais recentes de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<SensorReadingDto>> GetReadingsBySensorIdAsync(Guid sensorId, int limit = 100);


        /// <summary>
        /// Obtém a última leitura de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        Task<SensorReadingDto?> GetLatestReadingAsync(Guid sensorId);

        /// <summary>
        /// Cria uma nova leitura
        /// </summary>
        /// <param name="requests">Lista de leituras a criar</param>
        /// <returns>Número de leituras criadas</returns>
        Task<SensorReadingDto> CreateReadingAsync(CreateSensorReadingRequest requests);

        
    }
}
