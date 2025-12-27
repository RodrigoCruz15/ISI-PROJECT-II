using SmartHomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.Interfaces
{
    public interface ISensorReadingRepository
    {
        /// <summary>
        /// Obtém uma leitura pelo seu identificador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SensorReading?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtém as últimas N leituras de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<SensorReading>> GetBySensorIdAsync(Guid sensorId, int limit);

        /// <summary>
        /// Obtém a última leitura de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        Task<SensorReading?> GetLatestBySensorIdAsync(Guid sensorId);


        /// <summary>
        /// Cria uma nova leitura
        /// </summary>
        /// <param name="reading"></param>
        /// <returns></returns>
        Task<SensorReading> CreateAsync(SensorReading reading);

        

    }
}
