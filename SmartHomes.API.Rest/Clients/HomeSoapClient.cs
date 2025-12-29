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
    /// Cliente para comunicacao com o servico SOAP de casas
    /// </summary>
    public class HomeSoapClient
    {
        private readonly IHomeSoapService _soapClient;

        public HomeSoapClient(string soapServiceUrl)
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
            var channelFactory = new ChannelFactory<IHomeSoapService>(binding, endpoint);

            // O erro ocorria aqui porque o .NET validava o esquema antes de criar o canal
            _soapClient = channelFactory.CreateChannel();
        }

        /// <summary>
        /// Obtem uma casa pelo ID via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<HomeDto>> GetHomeByIdAsync(Guid id)
        {
            return await _soapClient.GetHomeByIdAsync(id);
        }

        /// <summary>
        /// Obtem todas as casas via SOAP
        /// </summary>
        /// <returns></returns>
        public async Task<SoapResponse<List<HomeDto>>> GetAllHomesAsync()
        {
            return await _soapClient.GetAllHomesAsync();
        }

        /// <summary>
        /// Obtem casas de um utilizador via SOAP
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<HomeDto>>> GetHomesByUserIdAsync(Guid userId)
        {
            return await _soapClient.GetHomesByUserIdAsync(userId);
        }

        /// <summary>
        /// Obtem casa com dados meteorologicos via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<HomeWithWeatherDto>> GetHomeWithWeatherAsync(Guid id)
        {
            return await _soapClient.GetHomeWithWeatherAsync(id);
        }

        /// <summary>
        /// Cria uma nova casa via SOAP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<HomeDto>> CreateHomeAsync(CreateHomeRequest request)
        {
            return await _soapClient.CreateHomeAsync(request);
        }

        /// <summary>
        /// Atualiza uma casa via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> UpdateHomeAsync(Guid id, UpdateHomeRequest request)
        {
            return await _soapClient.UpdateHomeAsync(id, request);
        }

        /// <summary>
        /// Remove uma casa via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> DeleteHomeAsync(Guid id, Guid userId)
        {
            return await _soapClient.DeleteHomeAsync(id, userId);
        }
    }
}