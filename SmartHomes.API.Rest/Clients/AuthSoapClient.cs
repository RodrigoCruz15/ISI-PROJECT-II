using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Models;
using SmartHomes.Domain.Interfaces;

namespace SmartHomes.API.Rest.Clients;

/// <summary>
/// Cliente para comunicacao com o servico SOAP de autenticacao
/// </summary>
public class AuthSoapClient
{
    private readonly IAuthSoapService _soapClient;

    public AuthSoapClient(string soapServiceUrl)
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
        var channelFactory = new ChannelFactory<IAuthSoapService>(binding, endpoint);

        // O erro ocorria aqui porque o .NET validava o esquema antes de criar o canal
        _soapClient = channelFactory.CreateChannel();
    }

    /// <summary>
    /// Regista um novo utilizador via SOAP
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<SoapResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        return await _soapClient.RegisterAsync(request);
    }

    /// <summary>
    /// Autentica um utilizador via SOAP
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<SoapResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        return await _soapClient.LoginAsync(request);
    }
}