using System.Threading.Tasks;
using SmartHomes.Domain.DTO;

namespace SmartHomes.Domain.Interfaces;

/// <summary>
/// Define o contrato para operacoes de autenticacao
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Regista um novo utilizador
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Autentica um utilizador
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}