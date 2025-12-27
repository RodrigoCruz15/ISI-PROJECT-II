using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de negócio com alertas
    /// </summary>
    public interface IAlertService
    {
        /// <summary>
        /// Obtém um alerta pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AlertDto?> GetAlertByIdAsync(Guid id);

        /// <summary>
        /// Obtém alertas de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<AlertDto>> GetAlertsBySensorIdAsync(Guid sensorId, int limit = 100);

        /// <summary>
        /// Obtém alertas de uma casa
        /// </summary>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<AlertDto>> GetAlertsByHomeIdAsync(Guid homeId, int limit = 100);

        /// <summary>
        /// Obtém alertas não reconhecidos (pendentes)
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        Task<IEnumerable<AlertDto>> GetUnacknowledgedAlertsAsync(Guid? homeId = null);

        /// <summary>
        /// Obtém alertas com detalhes enriquecidos
        /// </summary>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<AlertWithDetailsDto>> GetAlertsWithDetailsByHomeIdAsync(Guid homeId, int limit = 50);

        /// <summary>
        /// Marca um alerta como reconhecido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> AcknowledgeAlertAsync(Guid id);

        /// <summary>
        /// Chamado automaticamente ao criar uma leitura de sensor
        /// </summary>
        /// <param name="reading">Leitura recém-criada</param>
        /// <returns>Lista de alertas criados (se houver)</returns>
        Task<IEnumerable<Alert>> CheckAndCreateAlertsAsync(SensorReading reading);

    }
}
