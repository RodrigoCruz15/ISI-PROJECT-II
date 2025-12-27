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
    /// Implementacao do servico SOAP para gestao de alertas
    /// Atua como camada de integracao entre a API REST e a Application
    /// </summary>
    public class AlertSoapService : IAlertSoapService
    {
        private readonly IAlertService _alertService;

        public AlertSoapService(IAlertService alertService)
        {
            _alertService = alertService;
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
                var alert = await _alertService.GetAlertByIdAsync(id);

                if (alert == null)
                {
                    return new SoapResponse<AlertDto>
                    {
                        Success = false,
                        Message = $"Alerta com ID {id} não encontrado"
                    };
                }

                return new SoapResponse<AlertDto>
                {
                    Success = true,
                    Message = "Alerta encontrado com sucesso",
                    Data = alert
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
                var alerts = await _alertService.GetAlertsBySensorIdAsync(sensorId, limit);
                var alertsList = alerts.ToList();

                return new SoapResponse<List<AlertDto>>
                {
                    Success = true,
                    Message = $"{alertsList.Count} alerta(s) encontrado(s)",
                    Data = alertsList
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
                var alerts = await _alertService.GetAlertsByHomeIdAsync(homeId, limit);
                var alertsList = alerts.ToList();

                return new SoapResponse<List<AlertDto>>
                {
                    Success = true,
                    Message = $"{alertsList.Count} alerta(s) encontrado(s) na casa",
                    Data = alertsList
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
                var alerts = await _alertService.GetUnacknowledgedAlertsAsync(homeId);
                var alertsList = alerts.ToList();

                return new SoapResponse<List<AlertDto>>
                {
                    Success = true,
                    Message = $"{alertsList.Count} alerta(s) pendente(s)",
                    Data = alertsList
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<List<AlertDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter alertas pendentes: {ex.Message}"
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
                var result = await _alertService.AcknowledgeAlertAsync(id);

                return new SoapResponse<bool>
                {
                    Success = result,
                    Message = result ? "Alerta marcado como lido" : "Alerta não encontrado",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<bool>
                {
                    Success = false,
                    Message = $"Erro ao marcar alerta: {ex.Message}"
                };
            }
        }
    }
}