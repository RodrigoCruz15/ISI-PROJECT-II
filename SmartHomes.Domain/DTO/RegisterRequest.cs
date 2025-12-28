namespace SmartHomes.Domain.DTO;

/// <summary>
/// DTO para registo de novo utilizador
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Nome completo
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email (deve ser unico)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password (minimo 6 caracteres)
    /// </summary>
    public string Password { get; set; } = string.Empty;
}