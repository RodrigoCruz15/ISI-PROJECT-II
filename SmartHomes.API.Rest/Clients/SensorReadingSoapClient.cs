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
    /// Cliente para comunicação com o serviço SOAP de leituras de sensores
    /// </summary>
    public class SensorReadingSoapClient
    {
        private readonly ISensorReadingSoapService _soapClient;

        public SensorReadingSoapClient(string soapServiceUrl)
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
            var channelFactory = new ChannelFactory<ISensorReadingSoapService>(binding, endpoint);

            // O erro ocorria aqui porque o .NET validava o esquema antes de criar o canal
            _soapClient = channelFactory.CreateChannel();
        }

        /// <summary>
        /// Obtém uma leitura pelo ID via SOAP
        /// </summary>
        public async Task<SoapResponse<SensorReadingDto>> GetReadingByIdAsync(Guid id)
        {
            return await _soapClient.GetReadingByIdAsync(id);
        }

        /// <summary>
        /// Obtém leituras de um sensor via SOAP
        /// </summary>
        public async Task<SoapResponse<List<SensorReadingDto>>> GetReadingsBySensorIdAsync(Guid sensorId, int limit)
        {
            return await _soapClient.GetReadingsBySensorIdAsync(sensorId, limit);
        }

        /// <summary>
        /// Obtém a última leitura de um sensor via SOAP
        /// </summary>
        public async Task<SoapResponse<SensorReadingDto>> GetLatestReadingAsync(Guid sensorId)
        {
            return await _soapClient.GetLatestReadingAsync(sensorId);
        }

        /// <summary>
        /// Cria uma nova leitura via SOAP
        /// </summary>
        public async Task<SoapResponse<SensorReadingDto>> CreateReadingAsync(CreateSensorReadingRequest request)
        {
            return await _soapClient.CreateReadingAsync(request);
        }
    }
}