using SmartHomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.Interfaces
{
    /// <summary>
    /// Define o contrato para operacoes de persistencia de alertas disparados
    /// </summary>
    public interface IAlertRepository
    {
        /// <summary>
        /// Obtem um alerta pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Alert?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtem todos os alertas de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<Alert>> GetBySensorIdAsync(Guid sensorId, int? limit = null);

        /// <summary>
        /// Obtem todos os alertas de uma casa
        /// Util para dashboard da casa
        /// </summary>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<Alert>> GetByHomeIdAsync(Guid homeId, int? limit = null);

        /// <summary>
        /// Obtém alertas não reconhecidos (pendentes)
        /// Útil para notificações
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        Task<IEnumerable<Alert>> GetUnacknowledgedAsync(Guid? homeId = null);

        /// <summary>
        /// Obtém alertas por gravidade
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<Alert>> GetBySeverityAsync(int severity, Guid? homeId = null, int? limit = null);

        /// <summary>
        /// Cria um novo alerta
        /// Chamado automaticamente quando uma leitura viola uma regra
        /// </summary>
        Task<Alert> CreateAsync(Alert alert);

        /// <summary>
        /// Marca um alerta como reconhecido/lido
        /// </summary>
        Task<bool> AcknowledgeAsync(Guid id);

        

    }
}
