using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Enums;
using SmartHomes.Domain.Interfaces;

namespace SmartHomes.Application.Services
{
    /// <summary>
    /// Implementacao da logica de negocio para gestao de alertas
    /// Responsavel por: consultar alertas, verificar regras, criar alertas automaticamente
    /// </summary>
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IAlertRuleRepository _alertRuleRepository;

        public AlertService(IAlertRepository alertRepository, IAlertRuleRepository alertRuleRepository)
        {
            _alertRepository = alertRepository;
            _alertRuleRepository = alertRuleRepository;
        }

        /// <summary>
        /// Obtem um alerta pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AlertDto?> GetAlertByIdAsync(Guid id)
        {
            var alert = await _alertRepository.GetByIdAsync(id);

            if (alert == null)
                return null;

            return MapToDto(alert);
        }

        /// <summary>
        /// Obtem alertas de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AlertDto>> GetAlertsBySensorIdAsync(Guid sensorId, int limit = 100)
        {
            var alerts = await _alertRepository.GetBySensorIdAsync(sensorId, limit);
            return alerts.Select(MapToDto);
        }

        /// <summary>
        /// Obtem alertas de uma casa
        /// </summary>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AlertDto>> GetAlertsByHomeIdAsync(Guid homeId, int limit = 100)
        {
            var alerts = await _alertRepository.GetByHomeIdAsync(homeId, limit);
            return alerts.Select(MapToDto);
        }

        /// <summary>
        /// Obtem alertas nao reconhecidos (pendentes)
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AlertDto>> GetUnacknowledgedAlertsAsync(Guid? homeId = null)
        {
            var alerts = await _alertRepository.GetUnacknowledgedAsync(homeId);
            return alerts.Select(MapToDto);
        }

        /// <summary>
        /// Obtem alertas com detalhes enriquecidos
        /// TODO: Implementar quando necessario para UI
        /// </summary>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Task<IEnumerable<AlertWithDetailsDto>> GetAlertsWithDetailsByHomeIdAsync(Guid homeId, int limit = 50)
        {
            // Por agora, retorna lista vazia
            // Pode ser implementado depois com JOIN entre tabelas
            throw new NotImplementedException("Funcionalidade GetAlertsWithDetailsByHomeIdAsync ainda nao implementada");
        }

        /// <summary>
        /// Marca um alerta como reconhecido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> AcknowledgeAlertAsync(Guid id)
        {
            return await _alertRepository.AcknowledgeAsync(id);
        }

        /// <summary>
        /// METODO CHAVE: Verifica se uma leitura dispara alertas
        /// Chamado automaticamente ao criar uma leitura de sensor
        /// </summary>
        /// <param name="reading"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Alert>> CheckAndCreateAlertsAsync(SensorReading reading)
        {
            // 1. Buscar regras ativas para este sensor
            var activeRules = await _alertRuleRepository.GetActiveBySensorIdAsync(reading.SensorId);

            var triggeredAlerts = new List<Alert>();

            // 2. Verificar cada regra
            foreach (var rule in activeRules)
            {
                // Verificar se a leitura viola a condicao da regra
                bool ruleViolated = CheckCondition(reading.Value, rule.Condition, rule.Threshold);

                if (ruleViolated)
                {
                    // 3. Criar alerta
                    var alert = new Alert
                    {
                        AlertRuleId = rule.Id,
                        SensorReadingId = reading.Id,
                        SensorId = reading.SensorId,
                        Value = reading.Value,
                        Threshold = rule.Threshold,
                        Severity = rule.Severity,
                        Message = rule.Message,
                        IsAcknowledged = false
                    };

                    var createdAlert = await _alertRepository.CreateAsync(alert);
                    triggeredAlerts.Add(createdAlert);
                }
            }

            return triggeredAlerts;
        }

        /// <summary>
        /// Verifica se um valor viola uma condicao
        /// </summary>
        /// <param name="value"></param>
        /// <param name="condition"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static bool CheckCondition(decimal value, AlertConditionEnum condition, decimal threshold)
        {
            return condition switch
            {
                AlertConditionEnum.Equals => value == threshold,
                AlertConditionEnum.GreaterThan => value > threshold,
                AlertConditionEnum.LessThan => value < threshold,
                AlertConditionEnum.GreaterThanOrEqual => value >= threshold,
                AlertConditionEnum.LessThanOrEqual => value <= threshold,
                _ => false
            };
        }

        /// <summary>
        /// Converte entidade Alert para AlertDto
        /// </summary>
        /// <param name="alert"></param>
        /// <returns></returns>
        private static AlertDto MapToDto(Alert alert)
        {
            return new AlertDto
            {
                Id = alert.Id,
                AlertRuleId = alert.AlertRuleId,
                SensorReadingId = alert.SensorReadingId,
                SensorId = alert.SensorId,
                Value = alert.Value,
                Threshold = alert.Threshold,
                Severity = alert.Severity,
                Message = alert.Message,
                TriggeredAt = alert.TriggeredAt,
                IsAcknowledged = alert.IsAcknowledged,
                AcknowledgedAt = alert.AcknowledgedAt
            };
        }
    }
}