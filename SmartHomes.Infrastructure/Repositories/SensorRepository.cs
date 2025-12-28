using SmartHomes.Domain.Interfaces;
using SmartHomes.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartHomes.Infrastructure.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public SensorRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Obtem sensor pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Sensor?> GetByIdAsync(Guid id)
        {
            const string query = @"Select id, home_id, type, unit, name, is_active, created_at, last_reading_at from sensors where id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query,connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapToSensor(reader);

        }

        /// <summary>
        /// Obtem todos os sensores
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Sensor>> GetAllAsync()
        {
            const string query = @"Select id, home_id, type, unit, name, is_active, created_at, last_reading_at from sensors Order by created_at DESC";

            var sensors = new List<Sensor>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sensors.Add(MapToSensor(reader));
            }
            return sensors;
        }


        /// <summary>
        /// Obtem sensores de uma casa especifica
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Sensor>> GetByHomeAsync(Guid homeId)
        {
            const string query = @"Select id, home_id, type, unit, name, is_active, created_at, last_reading_at from sensors Where home_id = @HomeId";

            var sensors = new List<Sensor>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue($"@HomeId", homeId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sensors.Add(MapToSensor(reader));
            }

            return sensors;

        }

        /// <summary>
        /// Obtem sensores ativos numa casa especifica
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Sensor>> GetActiveByHomeIdAsync(Guid homeId)
        {
            const string query = @"Select id, home_id, type, unit, name, is_active, created_at, last_reading_at from sensors Where home_id = @Home_Id AND is_active = TRUE ";

            var sensors = new List<Sensor>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query,connection);
            command.Parameters.AddWithValue("@HomeId", homeId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sensors.Add(MapToSensor(reader));
            }
            return sensors;
        }


        /// <summary>
        /// Cria um novo sensor
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public async Task<Sensor> CreateAsync(Sensor sensor)
        {
            const string query = @"Insert into sensors (id, home_id, type, unit, name, is_active, created_at,last_reading_at)
                                    Values (@Id, @HomeId, @Type, @Unit, @Name, @IsActive, @CreatedAt, @LastReadingAt)
                                    RETURNING id, home_id, type, unit, name, is_active, created_at, last_reading_at";

            sensor.Id = Guid.NewGuid();
            sensor.CreatedAt = DateTime.UtcNow;

            await using var connection = _dbConnection.CreateConnection();

            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", sensor.Id);
            command.Parameters.AddWithValue("@HomeId", sensor.HomeId);
            command.Parameters.AddWithValue("@Type", (int)sensor.Type);
            command.Parameters.AddWithValue("@Unit", (int)sensor.Unit);
            command.Parameters.AddWithValue("@Name", sensor.Name);
            command.Parameters.AddWithValue("@IsActive", sensor.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", sensor.CreatedAt);
            command.Parameters.AddWithValue("@LastReadingAt",
                sensor.LastReadingAt.HasValue ? sensor.LastReadingAt.Value : DBNull.Value);

            await using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            return MapToSensor(reader);
        }

        /// <summary>
        /// Atualiza um sensor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(Guid id, Sensor sensor)
        {
            const string query = @"
                UPDATE sensors
                SET name = @Name,
                    type = @Type,
                    unit = @Unit,
                    is_active = @IsActive
                WHERE id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", sensor.Name);
            command.Parameters.AddWithValue("@Type", (int)sensor.Type);
            command.Parameters.AddWithValue("@Unit", (int)sensor.Unit);
            command.Parameters.AddWithValue("@IsActive", sensor.IsActive);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        /// <summary>
        /// Remove um sensor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM sensors WHERE id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        /// <summary>
        /// Atualiza a data/hora da ultima leitura
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public async Task<bool> UpdateLastReadingAsync(Guid sensorId, DateTime timestamp)
        {
            const string query = @"
                UPDATE sensors
                SET last_reading_at = @Timestamp
                WHERE id = @SensorId";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@SensorId", sensorId);
            command.Parameters.AddWithValue("@Timestamp", timestamp);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        private static Sensor MapToSensor(NpgsqlDataReader reader)
        {
            return new Sensor
            {
                Id = reader.GetGuid(0),
                HomeId = reader.GetGuid(1),
                Type = (SensorTypeEnum)reader.GetInt32(2),
                Unit = (UnitEnum)reader.GetInt32(3),
                Name = reader.GetString(4),
                IsActive = reader.GetBoolean(5),
                CreatedAt = reader.GetDateTime(6),
                LastReadingAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            };
        }
    }
}
