using System;

namespace SmartHomes.Domain.DTO;

/// <summary>
/// DTO de resposta apos autenticacao bem-sucedida
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// ID do utilizador
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Nome do utilizador
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email do utilizador
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// JWT Token para usar nas proximas requests
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Data de expiracao do token
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}