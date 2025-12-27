using SmartHomes.Domain.DTO;
using System.ServiceModel;
using SmartHomes.Services.Soap.Services;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.API.Rest.Clients
{
    public class SensorSoapClient
    {
        private readonly ISensorSoapService _soapClient;

        public SensorSoapClient(string soapServiceUrl)
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(soapServiceUrl);
            var channelFactory = new ChannelFactory<ISensorSoapService>(binding, endpoint);
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
