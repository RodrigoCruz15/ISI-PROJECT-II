using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Enums;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.Services.Soap.Services
{
    /// <summary>
    /// Implementacao do servico SOAP para gestao de regras de alerta
    /// Atua como camada de integracao entre a API REST e a Infrastructure
    /// </summary>
    public class AlertRuleSoapService : IAlertRuleSoapService
    {
        private readonly IAlertRuleRepository _alertRuleRepository;
        private readonly ISensorRepository _sensorRepository;

        public AlertRuleSoapService(IAlertRuleRepository alertRuleRepository, ISensorRepository sensorRepository)
        {
            _alertRuleRepository = alertRuleRepository;
            _sensorRepository = sensorRepository;
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
                var rule = await _alertRuleRepository.GetByIdAsync(id);

                if (rule == null)
                {
                    return new SoapResponse<AlertRuleDto>
                    {
                        Success = false,
                        Message = $"Regra com ID {id} nao encontrada"
                    };
                }

                return new SoapResponse<AlertRuleDto>
                {
                    Success = true,
                    Message = "Regra encontrada com sucesso",
                    Data = MapToDto(rule)
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
                var rules = await _alertRuleRepository.GetAllAsync();
                var ruleDtos = rules.Select(MapToDto).ToList();

                return new SoapResponse<List<AlertRuleDto>>
                {
                    Success = true,
                    Message = $"{ruleDtos.Count} regra(s) encontrada(s)",
                    Data = ruleDtos
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
        /// Obtem regras de um sensor especifico
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertRuleDto>>> GetAlertRulesBySensorIdAsync(Guid sensorId)
        {
            try
            {
                var rules = await _alertRuleRepository.GetBySensorIdAsync(sensorId);
                var ruleDtos = rules.Select(MapToDto).ToList();

                return new SoapResponse<List<AlertRuleDto>>
                {
                    Success = true,
                    Message = $"{ruleDtos.Count} regra(s) encontrada(s) para o sensor",
                    Data = ruleDtos
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
                // Verificar se o sensor existe
                var sensor = await _sensorRepository.GetByIdAsync(request.SensorId);
                if (sensor == null)
                {
                    return new SoapResponse<AlertRuleDto>
                    {
                        Success = false,
                        Message = $"Sensor com ID {request.SensorId} nao encontrado"
                    };
                }

                // Validacoes basicas
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return new SoapResponse<AlertRuleDto>
                    {
                        Success = false,
                        Message = "Nome da regra e obrigatorio"
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return new SoapResponse<AlertRuleDto>
                    {
                        Success = false,
                        Message = "Mensagem do alerta e obrigatoria"
                    };
                }

                // Criar regra
                var alertRule = new AlertRule
                {
                    SensorId = request.SensorId,
                    Name = request.Name.Trim(),
                    Condition = request.Condition,
                    Threshold = request.Threshold,
                    Severity = request.Severity,
                    Message = request.Message.Trim(),
                    IsActive = true
                };

                var createdRule = await _alertRuleRepository.CreateAsync(alertRule);

                return new SoapResponse<AlertRuleDto>
                {
                    Success = true,
                    Message = "Regra criada com sucesso",
                    Data = MapToDto(createdRule)
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
                var existingRule = await _alertRuleRepository.GetByIdAsync(id);

                if (existingRule == null)
                {
                    return new SoapResponse<bool>
                    {
                        Success = false,
                        Message = $"Regra com ID {id} nao encontrada"
                    };
                }

                // Atualizar campos fornecidos
                if (!string.IsNullOrWhiteSpace(request.Name))
                    existingRule.Name = request.Name.Trim();

                if (request.Condition.HasValue)
                    existingRule.Condition = request.Condition.Value;

                if (request.Threshold.HasValue)
                    existingRule.Threshold = request.Threshold.Value;

                if (request.Severity.HasValue)
                    existingRule.Severity = request.Severity.Value;

                if (!string.IsNullOrWhiteSpace(request.Message))
                    existingRule.Message = request.Message.Trim();

                if (request.IsActive.HasValue)
                    existingRule.IsActive = request.IsActive.Value;

                var result = await _alertRuleRepository.UpdateAsync(id, existingRule);

                return new SoapResponse<bool>
                {
                    Success = result,
                    Message = result ? "Regra atualizada com sucesso" : "Falha ao atualizar regra",
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
                var result = await _alertRuleRepository.DeleteAsync(id);

                return new SoapResponse<bool>
                {
                    Success = result,
                    Message = result ? "Regra removida com sucesso" : "Regra nao encontrada",
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

        /// <summary>
        /// Mapeia AlertRule para AlertRuleDto
        /// </summary>
        /// <param name="alertRule"></param>
        /// <returns></returns>
        private static AlertRuleDto MapToDto(AlertRule alertRule)
        {
            return new AlertRuleDto
            {
                Id = alertRule.Id,
                SensorId = alertRule.SensorId,
                Name = alertRule.Name,
                Condition = alertRule.Condition,
                Threshold = alertRule.Threshold,
                Severity = alertRule.Severity,
                Message = alertRule.Message,
                IsActive = alertRule.IsActive,
                CreatedAt = alertRule.CreatedAt
            };
        }
    }
}