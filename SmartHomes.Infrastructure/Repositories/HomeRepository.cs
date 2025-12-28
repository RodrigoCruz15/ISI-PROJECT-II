using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Enums;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Infrastructure.Data;

namespace SmartHomes.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de casas com acesso direto ao PostgreSQL
    /// </summary>
    public class HomeRepository : IHomeRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public HomeRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Obtém uma casa pelo ID
        /// </summary>
        public async Task<Home?> GetByIdAsync(Guid id)
        {
            const string query = @"
                SELECT id, user_id, name, address, latitude, longitude, area, status, created_at 
                FROM homes 
                WHERE id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapToHome(reader);
        }


        /// <summary>
        /// Obtem todas as casas de um utilizador especifico
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Home>> GetByUserIdAsync(Guid userId)
        {
            const string query = @"
        SELECT id, user_id, name, address, latitude, longitude, area, status, created_at
        FROM homes
        WHERE user_id = @UserId
        ORDER BY created_at DESC";

            var homes = new List<Home>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                homes.Add(MapToHome(reader));
            }

            return homes;
        }

        /// <summary>
        /// Obtém todas as casas
        /// </summary>
        public async Task<IEnumerable<Home>> GetAllAsync()
        {
            const string query = @"
                SELECT id, user_id, name, address, latitude, longitude, area, status, created_at
                FROM homes
                ORDER BY created_at DESC";

            var homes = new List<Home>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                homes.Add(MapToHome(reader));
            }

            return homes;
        }

        /// <summary>
        /// Cria uma nova casa
        /// </summary>
        public async Task<Home> CreateAsync(Home home)
        {
            const string query = @"
                INSERT INTO homes (id, user_id, name, address, latitude, longitude, area, status, created_at)
                VALUES (@Id, @UserId, @Name, @Address, @Latitude, @Longitude, @Area, @Status, @CreatedAt)
                RETURNING id, user_id, name, address, latitude, longitude, area, status, created_at";

            home.Id = Guid.NewGuid();
            home.CreatedAt = DateTime.UtcNow;

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", home.Id);
            command.Parameters.AddWithValue("@UserId", home.UserId);
            command.Parameters.AddWithValue("@Name", home.Name);
            command.Parameters.AddWithValue("@Address", home.Address);
            command.Parameters.AddWithValue("@Latitude", home.Latitude);
            command.Parameters.AddWithValue("@Longitude", home.Longitude);
            command.Parameters.AddWithValue("@Area", home.Area);
            command.Parameters.AddWithValue("@Status", home.Status.ToString());
            command.Parameters.AddWithValue("@CreatedAt", home.CreatedAt);

            await using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            return MapToHome(reader);
        }

        /// <summary>
        /// Atualiza uma casa existente
        /// </summary>
        public async Task<bool> UpdateAsync(Guid id, Home home) 
        {
            const string query = @"
                UPDATE homes
                SET name = @Name,
                    address = @Address,
                    latitude = @Latitude,
                    longitude = @Longitude,
                    area = @Area,
                    status = @Status
                WHERE id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", home.Name);
            command.Parameters.AddWithValue("@Address", home.Address);
            command.Parameters.AddWithValue("@Latitude", home.Latitude);
            command.Parameters.AddWithValue("@Longitude", home.Longitude);
            command.Parameters.AddWithValue("@Area", home.Area);
            command.Parameters.AddWithValue("@Status", home.Status.ToString());

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        /// <summary>
        /// Remove uma casa
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            const string query = "DELETE FROM homes WHERE id = @Id AND user_id = @UserId";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UserId", userId);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        /// <summary>
        /// Mapeia um reader do PostgreSQL para entidade Home
        /// </summary>
        private static Home MapToHome(NpgsqlDataReader reader)
        {
            return new Home
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                Name = reader.GetString(2),
                Address = reader.GetString(3),
                Latitude = reader.GetDecimal(4),
                Longitude = reader.GetDecimal(5),
                Area = reader.GetDecimal(6),
                Status = Enum.Parse<HomeStatus>(reader.GetString(7)),
                CreatedAt = reader.GetDateTime(8)
            };
        }
    }
}