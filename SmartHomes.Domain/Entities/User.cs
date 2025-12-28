using System;

namespace SmartHomes.Domain.Entities;

/// <summary>
/// Representa um utilizador do sistema
/// </summary>
public class User
{
    /// <summary>
    /// Identificador unico do utilizador
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo do utilizador
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email (usado para login)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash da password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Data de criacao da conta
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indica se a conta esta ativa
    /// </summary>
    public bool IsActive { get; set; } = true;
}