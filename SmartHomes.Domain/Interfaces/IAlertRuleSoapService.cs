using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks; // Adicionado para suportar Task
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Models; 

namespace SmartHomes.Domain.Interfaces
{
    /// <summary>
    /// Define o contrato SOAP para operacoes sobre regras de alerta
    /// </summary>
    [ServiceContract]
    public interface IAlertRuleSoapService
    {
        [OperationContract]
        Task<SoapResponse<AlertRuleDto>> GetAlertRuleByIdAsync(Guid id);

        [OperationContract]
        Task<SoapResponse<List<AlertRuleDto>>> GetAllAlertRulesAsync();

        [OperationContract]
        Task<SoapResponse<List<AlertRuleDto>>> GetAlertRulesBySensorIdAsync(Guid sensorId);

        [OperationContract]
        Task<SoapResponse<AlertRuleDto>> CreateAlertRuleAsync(CreateAlertRuleRequest request);

        [OperationContract]
        Task<SoapResponse<bool>> UpdateAlertRuleAsync(Guid id, UpdateAlertRuleRequest request);

        [OperationContract]
        Task<SoapResponse<bool>> DeleteAlertRuleAsync(Guid id);
    }
}