using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Services.Soap.Services;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.API.Rest.Clients
{
    /// <summary>
    /// Cliente para comunicacao com o servico SOAP de regras de alerta
    /// </summary>
    public class AlertRuleSoapClient
    {
        private readonly IAlertRuleSoapService _soapClient;

        public AlertRuleSoapClient(string soapServiceUrl)
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(soapServiceUrl);
            var channelFactory = new ChannelFactory<IAlertRuleSoapService>(binding, endpoint);
            _soapClient = channelFactory.CreateChannel();
        }

        /// <summary>
        /// Obtem uma regra pelo ID via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<AlertRuleDto>> GetAlertRuleByIdAsync(Guid id)
        {
            return await _soapClient.GetAlertRuleByIdAsync(id);
        }

        /// <summary>
        /// Obtem todas as regras via SOAP
        /// </summary>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertRuleDto>>> GetAllAlertRulesAsync()
        {
            return await _soapClient.GetAllAlertRulesAsync();
        }

        /// <summary>
        /// Obtem regras de um sensor via SOAP
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertRuleDto>>> GetAlertRulesBySensorIdAsync(Guid sensorId)
        {
            return await _soapClient.GetAlertRulesBySensorIdAsync(sensorId);
        }

        /// <summary>
        /// Cria uma nova regra via SOAP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<AlertRuleDto>> CreateAlertRuleAsync(CreateAlertRuleRequest request)
        {
            return await _soapClient.CreateAlertRuleAsync(request);
        }

        /// <summary>
        /// Atualiza uma regra via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> UpdateAlertRuleAsync(Guid id, UpdateAlertRuleRequest request)
        {
            return await _soapClient.UpdateAlertRuleAsync(id, request);
        }

        /// <summary>
        /// Remove uma regra via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> DeleteAlertRuleAsync(Guid id)
        {
            return await _soapClient.DeleteAlertRuleAsync(id);
        }
    }
}