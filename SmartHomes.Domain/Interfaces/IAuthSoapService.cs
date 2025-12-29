using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks; // Adicionado para suportar Task
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Models;

namespace SmartHomes.Domain.Interfaces;

/// <summary>
/// Define o contrato SOAP para operacoes de autenticacao
/// </summary>
[ServiceContract(Namespace = "http://tempuri.org/")]
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