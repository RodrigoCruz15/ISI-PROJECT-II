namespace SmartHomes.Domain.DTO;

/// <summary>
/// DTO para autenticacao de utilizador
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Email do utilizador
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}