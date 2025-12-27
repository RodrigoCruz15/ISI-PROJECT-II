using System;
using System.Collections.Generic;
using System.ServiceModel;
using SmartHomes.Domain.DTO;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.Services.Soap.Services
{
    /// <summary>
    /// Define o contrato SOAP para operacoes sobre regras de alerta
    /// </summary>
    [ServiceContract]
    public interface IAlertRuleSoapService
    {
        /// <summary>
        /// Obtem uma regra pelo ID via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<AlertRuleDto>> GetAlertRuleByIdAsync(Guid id);

        /// <summary>
        /// Obtem todas as regras via SOAP
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<List<AlertRuleDto>>> GetAllAlertRulesAsync();

        /// <summary>
        /// Obtem regras de um sensor via SOAP
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<List<AlertRuleDto>>> GetAlertRulesBySensorIdAsync(Guid sensorId);

        /// <summary>
        /// Cria uma nova regra via SOAP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<AlertRuleDto>> CreateAlertRuleAsync(CreateAlertRuleRequest request);

        /// <summary>
        /// Atualiza uma regra via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<bool>> UpdateAlertRuleAsync(Guid id, UpdateAlertRuleRequest request);

        /// <summary>
        /// Remove uma regra via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<bool>> DeleteAlertRuleAsync(Guid id);
    }
}