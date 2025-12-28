using System;
using System.Threading.Tasks;
using Npgsql;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Infrastructure.Data;

namespace SmartHomes.Infrastructure.Repositories;

/// <summary>
/// Implementacao do repositorio de utilizadores com acesso direto ao PostgreSQL
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly DatabaseConnection _dbConnection;

    public UserRepository(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    /// <summary>
    /// Obtem um utilizador pelo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string query = @"
            SELECT id, full_name, email, password_hash, created_at, is_active
            FROM users
            WHERE id = @Id";

        await using var connection = _dbConnection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapToUser(reader);
    }

    /// <summary>
    /// Obtem um utilizador pelo email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        const string query = @"
            SELECT id, full_name, email, password_hash, created_at, is_active
            FROM users
            WHERE LOWER(email) = LOWER(@Email)";

        await using var connection = _dbConnection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Email", email);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapToUser(reader);
    }

    /// <summary>
    /// Cria um novo utilizador
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<User> CreateAsync(User user)
    {
        const string query = @"
            INSERT INTO users (id, full_name, email, password_hash, created_at, is_active)
            VALUES (@Id, @FullName, @Email, @PasswordHash, @CreatedAt, @IsActive)
            RETURNING id, full_name, email, password_hash, created_at, is_active";

        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;

        await using var connection = _dbConnection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@Email", user.Email.ToLower());
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
        command.Parameters.AddWithValue("@IsActive", user.IsActive);

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        return MapToUser(reader);
    }

    /// <summary>
    /// Verifica se um email ja existe
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<bool> EmailExistsAsync(string email)
    {
        const string query = "SELECT COUNT(*) FROM users WHERE LOWER(email) = LOWER(@Email)";

        await using var connection = _dbConnection.CreateConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Email", email);

        var count = await command.ExecuteScalarAsync();
        return Convert.ToInt32(count) > 0;
    }

    /// <summary>
    /// Mapeia um reader do PostgreSQL para entidade User
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static User MapToUser(NpgsqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(0),
            FullName = reader.GetString(1),
            Email = reader.GetString(2),
            PasswordHash = reader.GetString(3),
            CreatedAt = reader.GetDateTime(4),
            IsActive = reader.GetBoolean(5)
        };
    }
}