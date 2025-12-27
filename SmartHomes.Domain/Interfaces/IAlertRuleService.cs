using SmartHomes.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de negócio com regras de alerta
    /// </summary>
    public interface IAlertRuleService
    {
        /// <summary>
        /// Obtém uma regra pelo ID
        /// </summary>
        Task<AlertRuleDto?> GetAlertRuleByIdAsync(Guid id);

        /// <summary>
        /// Obtém todas as regras
        /// </summary>
        Task<IEnumerable<AlertRuleDto>> GetAllAlertRulesAsync();

        /// <summary>
        /// Obtém regras de um sensor específico
        /// </summary>
        Task<IEnumerable<AlertRuleDto>> GetAlertRulesBySensorIdAsync(Guid sensorId);

        /// <summary>
        /// Cria uma nova regra com validações
        /// Valida: sensor existe, threshold válido, mensagem não vazia, etc.
        /// </summary>
        Task<AlertRuleDto> CreateAlertRuleAsync(CreateAlertRuleRequest request);

        /// <summary>
        /// Atualiza uma regra existente
        /// </summary>
        Task<bool> UpdateAlertRuleAsync(Guid id, UpdateAlertRuleRequest request);

        /// <summary>
        /// Remove uma regra
        /// </summary>
        Task<bool> DeleteAlertRuleAsync(Guid id);
    }
}
