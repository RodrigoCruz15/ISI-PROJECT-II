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
    /// Implementacao da logica de negocio para gestao de regras de alerta
    /// Responsavel por: validacoes, conversoes, orquestracao de repositories
    /// </summary>
    public class AlertRuleService : IAlertRuleService
    {
        private readonly IAlertRuleRepository _alertRuleRepository;
        private readonly ISensorRepository _sensorRepository;

        public AlertRuleService(IAlertRuleRepository alertRuleRepository, ISensorRepository sensorRepository)
        {
            _alertRuleRepository = alertRuleRepository;
            _sensorRepository = sensorRepository;
        }

        /// <summary>
        /// Obtem uma regra pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AlertRuleDto?> GetAlertRuleByIdAsync(Guid id)
        {
            var rule = await _alertRuleRepository.GetByIdAsync(id);

            if (rule == null)
                return null;

            return MapToDto(rule);
        }

        /// <summary>
        /// Obtem todas as regras
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AlertRuleDto>> GetAllAlertRulesAsync()
        {
            var rules = await _alertRuleRepository.GetAllAsync();
            return rules.Select(MapToDto);
        }

        /// <summary>
        /// Obtem regras de um sensor especifico
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AlertRuleDto>> GetAlertRulesBySensorIdAsync(Guid sensorId)
        {
            var rules = await _alertRuleRepository.GetBySensorIdAsync(sensorId);
            return rules.Select(MapToDto);
        }

        /// <summary>
        /// Cria uma nova regra com validacoes de negocio
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<AlertRuleDto> CreateAlertRuleAsync(CreateAlertRuleRequest request)
        {
            // Validacao: verificar se o sensor existe
            var sensor = await _sensorRepository.GetByIdAsync(request.SensorId);
            if (sensor == null)
                throw new ArgumentException($"Sensor com ID {request.SensorId} nao encontrado");

            // Validacoes de negocio
            ValidateCreateRequest(request);

            // Mapear para entidade
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

            // Criar no repository
            var createdRule = await _alertRuleRepository.CreateAsync(alertRule);

            return MapToDto(createdRule);
        }

        /// <summary>
        /// Atualiza uma regra existente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAlertRuleAsync(Guid id, UpdateAlertRuleRequest request)
        {
            // Verificar se a regra existe
            var existingRule = await _alertRuleRepository.GetByIdAsync(id);
            if (existingRule == null)
                return false;

            // Validacoes de negocio
            ValidateUpdateRequest(request);

            // Atualizar apenas os campos fornecidos
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

            return await _alertRuleRepository.UpdateAsync(id, existingRule);
        }

        /// <summary>
        /// Remove uma regra
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAlertRuleAsync(Guid id)
        {
            // Verificar se existe antes de remover
            var existingRule = await _alertRuleRepository.GetByIdAsync(id);
            if (existingRule == null)
                return false;

            return await _alertRuleRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Valida o pedido de criacao de regra de alerta
        /// </summary>
        /// <param name="request"></param>
        private static void ValidateCreateRequest(CreateAlertRuleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("O nome da regra e obrigatorio", nameof(request.Name));

            if (request.Name.Length > 200)
                throw new ArgumentException("O nome da regra nao pode exceder 200 caracteres", nameof(request.Name));

            if (string.IsNullOrWhiteSpace(request.Message))
                throw new ArgumentException("A mensagem do alerta e obrigatoria", nameof(request.Message));

            if (request.Message.Length > 500)
                throw new ArgumentException("A mensagem nao pode exceder 500 caracteres", nameof(request.Message));

            if (request.Threshold < -1000 || request.Threshold > 1000000)
                throw new ArgumentException("Threshold fora do intervalo valido", nameof(request.Threshold));
        }

        /// <summary>
        /// Valida o pedido de atualizacao de regra de alerta
        /// </summary>
        /// <param name="request"></param>
        private static void ValidateUpdateRequest(UpdateAlertRuleRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Name) && request.Name.Length > 200)
                throw new ArgumentException("O nome da regra nao pode exceder 200 caracteres", nameof(request.Name));

            if (!string.IsNullOrWhiteSpace(request.Message) && request.Message.Length > 500)
                throw new ArgumentException("A mensagem nao pode exceder 500 caracteres", nameof(request.Message));

            if (request.Threshold.HasValue && (request.Threshold < -1000 || request.Threshold > 1000000))
                throw new ArgumentException("Threshold fora do intervalo valido", nameof(request.Threshold));
        }

        /// <summary>
        /// Converte entidade AlertRule para AlertRuleDto
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