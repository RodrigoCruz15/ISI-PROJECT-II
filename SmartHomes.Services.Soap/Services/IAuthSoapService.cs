using System.ServiceModel;
using SmartHomes.Domain.DTO;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.Services.Soap.Services;

/// <summary>
/// Define o contrato SOAP para operacoes de autenticacao
/// </summary>
[ServiceContract]
public interface IAuthSoapService
{
    /// <summary>
    /// Regista um novo utilizador via SOAP
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    Task<SoapResponse<AuthResponse>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Autentica um utilizador via SOAP
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    Task<SoapResponse<AuthResponse>> LoginAsync(LoginRequest request);
}