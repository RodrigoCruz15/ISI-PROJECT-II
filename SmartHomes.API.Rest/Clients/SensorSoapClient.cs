using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Models;
using SmartHomes.Domain.Interfaces;

namespace SmartHomes.API.Rest.Clients
{
    public class SensorSoapClient
    {
        private readonly ISensorSoapService _soapClient;

        public SensorSoapClient(string soapServiceUrl)
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
            var channelFactory = new ChannelFactory<ISensorSoapService>(binding, endpoint);

            // O erro ocorria aqui porque o .NET validava o esquema antes de criar o canal
            _soapClient = channelFactory.CreateChannel();
        }

        /// <summary>
        /// Obtém um sensor pelo ID via SOAP
        /// </summary>
        public async Task<SoapResponse<SensorDto>> GetSensorByIdAsync(Guid id)
        {
            return await _soapClient.GetSensorByIdAsync(id);
        }

        /// <summary>
        /// Obtém todos os sensores via SOAP
        /// </summary>
        public async Task<SoapResponse<List<SensorDto>>> GetAllSensorsAsync()
        {
            return await _soapClient.GetAllSensorsAsync();
        }

        /// <summary>
        /// Obtém sensores de uma casa via SOAP
        /// </summary>
        public async Task<SoapResponse<List<SensorDto>>> GetSensorsByHomeIdAsync(Guid homeId)
        {
            return await _soapClient.GetSensorsByHomeIdAsync(homeId);
        }

        /// <summary>
        /// Obtém sensores ativos de uma casa via SOAP
        /// </summary>
        public async Task<SoapResponse<List<SensorDto>>> GetActiveSensorsByHomeIdAsync(Guid homeId)
        {
            return await _soapClient.GetActiveSensorsByHomeIdAsync(homeId);
        }

        /// <summary>
        /// Cria um novo sensor via SOAP
        /// </summary>
        public async Task<SoapResponse<SensorDto>> CreateSensorAsync(CreateSensorRequest request)
        {
            return await _soapClient.CreateSensorAsync(request);
        }

        /// <summary>
        /// Atualiza um sensor via SOAP
        /// </summary>
        public async Task<SoapResponse<bool>> UpdateSensorAsync(Guid id, UpdateSensorRequest request)
        {
            return await _soapClient.UpdateSensorAsync(id, request);
        }

        /// <summary>
        /// Remove um sensor via SOAP
        /// </summary>
        public async Task<SoapResponse<bool>> DeleteSensorAsync(Guid id)
        {
            return await _soapClient.DeleteSensorAsync(id);
        }

    }
}
