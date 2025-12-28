using System.ServiceModel;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Services.Soap.Services;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.API.Rest.Clients;

/// <summary>
/// Cliente para comunicacao com o servico SOAP de autenticacao
/// </summary>
public class AuthSoapClient
{
    private readonly IAuthSoapService _soapClient;

    public AuthSoapClient(string soapServiceUrl)
    {
        var binding = new BasicHttpBinding();
        var endpoint = new EndpointAddress(soapServiceUrl);
        var channelFactory = new ChannelFactory<IAuthSoapService>(binding, endpoint);
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