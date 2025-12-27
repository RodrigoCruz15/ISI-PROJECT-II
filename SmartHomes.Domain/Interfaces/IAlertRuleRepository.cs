using SmartHomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.Interfaces
{
    public interface IAlertRuleRepository
    {
        /// <summary>
        /// Obtem uma regra de alerta pelo seu identificador unico
        /// </summary>
        /// <param name="id">Identificador da regra de alerta</param>
        /// <returns>Regra de alerta ou null caso nao exista</returns>
        Task<AlertRule?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtem todas as regras de alerta existentes no sistema
        /// </summary>
        /// <returns>Colecao de regras de alerta</returns>
        Task<IEnumerable<AlertRule>> GetAllAsync();

        /// <summary>
        /// Obtem todas as regras de alerta associadas a um sensor especifico
        /// </summary>
        /// <param name="sensorId">Identificador do sensor</param>
        /// <returns>Colecao de regras de alerta do sensor</returns>
        Task<IEnumerable<AlertRule>> GetBySensorIdAsync(Guid sensorId);

        /// <summary>
        /// Obtem apenas as regras de alerta ativas associadas a um sensor
        /// </summary>
        /// <param name="sensorId">Identificador do sensor</param>
        /// <returns>Colecao de regras de alerta ativas</returns>
        Task<IEnumerable<AlertRule>> GetActiveBySensorIdAsync(Guid sensorId);

        /// <summary>
        /// Cria uma nova regra de alerta
        /// </summary>
        /// <param name="alertRule">Objeto com os dados da regra de alerta</param>
        /// <returns>Regra de alerta criada</returns>
        Task<AlertRule> CreateAsync(AlertRule alertRule);

        /// <summary>
        /// Atualiza uma regra de alerta existente
        /// </summary>
        /// <param name="id">Identificador da regra a atualizar</param>
        /// <param name="alertRule">Novos dados da regra de alerta</param>
        /// <returns>True se a atualizacao for bem sucedida, false caso contrario</returns>
        Task<bool> UpdateAsync(Guid id, AlertRule alertRule);

        /// <summary>
        /// Remove uma regra de alerta do sistema
        /// </summary>
        /// <param name="id">Identificador da regra de alerta</param>
        /// <returns>True se a remocao for bem sucedida, false caso contrario</returns>
        Task<bool> DeleteAsync(Guid id);

    }
}
