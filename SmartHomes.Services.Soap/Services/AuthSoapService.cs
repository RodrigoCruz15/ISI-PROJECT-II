using System;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.Services.Soap.Services;

/// <summary>
/// Implementacao do servico SOAP para autenticacao
/// Atua como camada de integracao entre a API REST e a Application
/// </summary>
public class AuthSoapService : IAuthSoapService
{
    private readonly IAuthService _authService;

    public AuthSoapService(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Regista um novo utilizador
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<SoapResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);

            return new SoapResponse<AuthResponse>
            {
                Success = true,
                Message = "Utilizador registado com sucesso",
                Data = response
            };
        }
        catch (InvalidOperationException ex)
        {
            return new SoapResponse<AuthResponse>
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (ArgumentException ex)
        {
            return new SoapResponse<AuthResponse>
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new SoapResponse<AuthResponse>
            {
                Success = false,
                Message = $"Erro ao registar utilizador: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Autentica um utilizador
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<SoapResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);

            if (response == null)
            {
                return new SoapResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Email ou password incorretos"
                };
            }

            return new SoapResponse<AuthResponse>
            {
                Success = true,
                Message = "Autenticado com sucesso",
                Data = response
            };
        }
        catch (InvalidOperationException ex)
        {
            return new SoapResponse<AuthResponse>
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new SoapResponse<AuthResponse>
            {
                Success = false,
                Message = $"Erro ao autenticar: {ex.Message}"
            };
        }
    }
}