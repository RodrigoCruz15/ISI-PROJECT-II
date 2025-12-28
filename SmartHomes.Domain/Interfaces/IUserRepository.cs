using System;
using System.Threading.Tasks;
using SmartHomes.Domain.Entities;

namespace SmartHomes.Domain.Interfaces;

/// <summary>
/// Define o contrato para operacoes de persistencia de utilizadores
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Obtem um utilizador pelo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtem um utilizador pelo email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Cria um novo utilizador
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Verifica se um email ja existe
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<bool> EmailExistsAsync(string email);
}