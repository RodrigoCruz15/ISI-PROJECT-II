using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Models;
using SmartHomes.Domain.Interfaces;

namespace SmartHomes.API.Rest.Clients
{
    /// <summary>
    /// Cliente para comunicacao com o servico SOAP de alertas disparados
    /// </summary>
    public class AlertSoapClient
    {
        private readonly IAlertSoapService _soapClient;

        public AlertSoapClient(string soapServiceUrl)
        {
            // 1. Criar o binding básico
            var binding = new BasicHttpBinding();

            // 2. CORREÇÃO CRÍTICA: Ativar segurança de transporte para HTTPS
            if (soapServiceUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
            }

            var endpoint = new EndpointAddress(soapServiceUrl);

            // 3. Criar a fábrica de canais
            var channelFactory = new ChannelFactory<IAlertSoapService>(binding, endpoint);

            // O erro ocorria aqui porque o .NET validava o esquema antes de criar o canal
            _soapClient = channelFactory.CreateChannel();
        }

        /// <summary>
        /// Obtem um alerta pelo ID via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<AlertDto>> GetAlertByIdAsync(Guid id)
        {
            return await _soapClient.GetAlertByIdAsync(id);
        }

        /// <summary>
        /// Obtem alertas de um sensor via SOAP
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertDto>>> GetAlertsBySensorIdAsync(Guid sensorId, int limit)
        {
            return await _soapClient.GetAlertsBySensorIdAsync(sensorId, limit);
        }

        /// <summary>
        /// Obtem alertas de uma casa via SOAP
        /// </summary>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertDto>>> GetAlertsByHomeIdAsync(Guid homeId, int limit)
        {
            return await _soapClient.GetAlertsByHomeIdAsync(homeId, limit);
        }

        /// <summary>
        /// Obtem alertas nao reconhecidos via SOAP
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<AlertDto>>> GetUnacknowledgedAlertsAsync(Guid? homeId)
        {
            return await _soapClient.GetUnacknowledgedAlertsAsync(homeId);
        }

        /// <summary>
        /// Marca um alerta como reconhecido via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> AcknowledgeAlertAsync(Guid id)
        {
            return await _soapClient.AcknowledgeAlertAsync(id);
        }
    }
}