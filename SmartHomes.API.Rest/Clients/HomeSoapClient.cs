using SmartHomes.Domain.DTO;
using System.ServiceModel;
using SmartHomes.Services.Soap.Services;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.API.Rest.Clients
{
    public class HomeSoapClient
    {
        private readonly IHomeSoapService _soapClient;

        public HomeSoapClient(string soapServiceUrl)
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(soapServiceUrl);
            var channelFactory = new ChannelFactory<IHomeSoapService>(binding, endpoint);
            _soapClient = channelFactory.CreateChannel();
        }

        /// <summary>
        /// Obtém uma casa pelo ID via SOAP
        /// </summary>
        public async Task<SoapResponse<HomeDto>> GetHomeByIdAsync(Guid id)
        {
            return await _soapClient.GetHomeByIdAsync(id);
        }

        /// <summary>
        /// Obtém todas as casas via SOAP
        /// </summary>
        public async Task<SoapResponse<List<HomeDto>>> GetAllHomesAsync()
        {
            return await _soapClient.GetAllHomesAsync();
        }

        /// <summary>
        /// Cria uma nova casa via SOAP
        /// </summary>
        public async Task<SoapResponse<HomeDto>> CreateHomeAsync(CreateHomeRequest request)
        {
            return await _soapClient.CreateHomeAsync(request);
        }

        /// <summary>
        /// Atualiza uma casa via SOAP
        /// </summary>
        public async Task<SoapResponse<bool>> UpdateHomeAsync(Guid id, UpdateHomeRequest request)
        {
            return await _soapClient.UpdateHomeAsync(id, request);
        }

        /// <summary>
        /// Remove uma casa via SOAP
        /// </summary>
        public async Task<SoapResponse<bool>> DeleteHomeAsync(Guid id)
        {
            return await _soapClient.DeleteHomeAsync(id);
        }
    }
}
