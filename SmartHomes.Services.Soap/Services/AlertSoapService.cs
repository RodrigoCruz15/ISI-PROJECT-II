using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.Services.Soap.Services
{
    /// <summary>
    /// Implementacao do servico SOAP para gestao de alertas disparados
    /// Atua como camada de integracao para consultar alertas
    /// </summary>
    public class AlertSoapService : IAlertSoapService
    {
        private readonly IAlertRepository _alertRepository;

        public AlertSoapService(IAlertRepository alertRepository)
        {
            _alertRepository = alertRepository;
        }

        /// <summary>
        /// Obtem um alerta pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<AlertDto>> GetAlertByIdAsync(Guid id)
        {
            try
            {
                var alert = await _alertRepository.GetByIdAsync(id);

                if (alert == null)
                {
                    return new SoapResponse<AlertDto>
                    {
                        Success = false,
                        Message = $"Alerta com ID {id} nao encontrado"
                    };
                }

                return new SoapResponse<AlertDto>
                {
                    Success = true,
                    Message = "Alerta encontrado com sucesso",
                    Data = MapToDto(alert)
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<AlertDto>
                {
                    Success = false,
                    Message = $"Erro ao obter alerta: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtem alertas de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertDto>>> GetAlertsBySensorIdAsync(Guid sensorId, int limit)
        {
            try
            {
                var alerts = await _alertRepository.GetBySensorIdAsync(sensorId, limit);
                var alertDtos = alerts.Select(MapToDto).ToList();

                return new SoapResponse<List<AlertDto>>
                {
                    Success = true,
                    Message = $"{alertDtos.Count} alerta(s) encontrado(s)",
                    Data = alertDtos
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<List<AlertDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter alertas: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtem alertas de uma casa
        /// </summary>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertDto>>> GetAlertsByHomeIdAsync(Guid homeId, int limit)
        {
            try
            {
                var alerts = await _alertRepository.GetByHomeIdAsync(homeId, limit);
                var alertDtos = alerts.Select(MapToDto).ToList();

                return new SoapResponse<List<AlertDto>>
                {
                    Success = true,
                    Message = $"{alertDtos.Count} alerta(s) encontrado(s) na casa",
                    Data = alertDtos
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<List<AlertDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter alertas: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtem alertas nao reconhecidos
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertDto>>> GetUnacknowledgedAlertsAsync(Guid? homeId)
        {
            try
            {
                var alerts = await _alertRepository.GetUnacknowledgedAsync(homeId);
                var alertDtos = alerts.Select(MapToDto).ToList();

                return new SoapResponse<List<AlertDto>>
                {
                    Success = true,
                    Message = $"{alertDtos.Count} alerta(s) nao reconhecido(s)",
                    Data = alertDtos
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<List<AlertDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter alertas: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Marca um alerta como reconhecido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> AcknowledgeAlertAsync(Guid id)
        {
            try
            {
                var result = await _alertRepository.AcknowledgeAsync(id);

                return new SoapResponse<bool>
                {
                    Success = result,
                    Message = result ? "Alerta reconhecido com sucesso" : "Alerta nao encontrado",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<bool>
                {
                    Success = false,
                    Message = $"Erro ao reconhecer alerta: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Mapeia Alert para AlertDto
        /// </summary>
        /// <param name="alert"></param>
        /// <returns></returns>
        private static AlertDto MapToDto(Domain.Entities.Alert alert)
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