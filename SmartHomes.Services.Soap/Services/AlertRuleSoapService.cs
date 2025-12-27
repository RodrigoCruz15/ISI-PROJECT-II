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
    /// Implementacao do servico SOAP para gestao de regras de alerta
    /// Atua como camada de integracao entre a API REST e a Application
    /// </summary>
    public class AlertRuleSoapService : IAlertRuleSoapService
    {
        private readonly IAlertRuleService _alertRuleService;

        public AlertRuleSoapService(IAlertRuleService alertRuleService)
        {
            _alertRuleService = alertRuleService;
        }

        /// <summary>
        /// Obtem uma regra pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<AlertRuleDto>> GetAlertRuleByIdAsync(Guid id)
        {
            try
            {
                var alertRule = await _alertRuleService.GetAlertRuleByIdAsync(id);

                if (alertRule == null)
                {
                    return new SoapResponse<AlertRuleDto>
                    {
                        Success = false,
                        Message = $"Regra com ID {id} não encontrada"
                    };
                }

                return new SoapResponse<AlertRuleDto>
                {
                    Success = true,
                    Message = "Regra encontrada com sucesso",
                    Data = alertRule
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<AlertRuleDto>
                {
                    Success = false,
                    Message = $"Erro ao obter regra: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtem todas as regras
        /// </summary>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertRuleDto>>> GetAllAlertRulesAsync()
        {
            try
            {
                var alertRules = await _alertRuleService.GetAllAlertRulesAsync();
                var rulesList = alertRules.ToList();

                return new SoapResponse<List<AlertRuleDto>>
                {
                    Success = true,
                    Message = $"{rulesList.Count} regra(s) encontrada(s)",
                    Data = rulesList
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<List<AlertRuleDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter regras: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtem regras de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertRuleDto>>> GetAlertRulesBySensorIdAsync(Guid sensorId)
        {
            try
            {
                var alertRules = await _alertRuleService.GetAlertRulesBySensorIdAsync(sensorId);
                var rulesList = alertRules.ToList();

                return new SoapResponse<List<AlertRuleDto>>
                {
                    Success = true,
                    Message = $"{rulesList.Count} regra(s) encontrada(s) para o sensor",
                    Data = rulesList
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<List<AlertRuleDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter regras: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Cria uma nova regra
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<AlertRuleDto>> CreateAlertRuleAsync(CreateAlertRuleRequest request)
        {
            try
            {
                var createdRule = await _alertRuleService.CreateAlertRuleAsync(request);

                return new SoapResponse<AlertRuleDto>
                {
                    Success = true,
                    Message = "Regra criada com sucesso",
                    Data = createdRule
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<AlertRuleDto>
                {
                    Success = false,
                    Message = $"Erro ao criar regra: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Atualiza uma regra existente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> UpdateAlertRuleAsync(Guid id, UpdateAlertRuleRequest request)
        {
            try
            {
                var result = await _alertRuleService.UpdateAlertRuleAsync(id, request);

                return new SoapResponse<bool>
                {
                    Success = result,
                    Message = result ? "Regra atualizada com sucesso" : "Regra não encontrada",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<bool>
                {
                    Success = false,
                    Message = $"Erro ao atualizar regra: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Remove uma regra
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> DeleteAlertRuleAsync(Guid id)
        {
            try
            {
                var result = await _alertRuleService.DeleteAlertRuleAsync(id);

                return new SoapResponse<bool>
                {
                    Success = result,
                    Message = result ? "Regra removida com sucesso" : "Regra não encontrada",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<bool>
                {
                    Success = false,
                    Message = $"Erro ao remover regra: {ex.Message}"
                };
            }
        }
    }
}