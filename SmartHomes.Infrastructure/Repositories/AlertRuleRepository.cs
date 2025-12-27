using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Enums;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Infrastructure.Data;

namespace SmartHomes.Infrastructure.Repositories
{
    /// <summary>
    /// Implementacao do repositorio de regras de alerta com acesso direto ao PostgreSQL
    /// </summary>
    public class AlertRuleRepository : IAlertRuleRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public AlertRuleRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Obtem uma regra pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AlertRule?> GetByIdAsync(Guid id)
        {
            const string query = @"
                SELECT id, sensor_id, name, condition, threshold, severity, message, is_active, created_at
                FROM alert_rules
                WHERE id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapToAlertRule(reader);
        }

        /// <summary>
        /// Obtem todas as regras registadas
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AlertRule>> GetAllAsync()
        {
            const string query = @"
                SELECT id, sensor_id, name, condition, threshold, severity, message, is_active, created_at
                FROM alert_rules
                ORDER BY created_at DESC";

            var rules = new List<AlertRule>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                rules.Add(MapToAlertRule(reader));
            }

            return rules;
        }

        /// <summary>
        /// Obtem todas as regras de um sensor especifico
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AlertRule>> GetBySensorIdAsync(Guid sensorId)
        {
            const string query = @"
                SELECT id, sensor_id, name, condition, threshold, severity, message, is_active, created_at
                FROM alert_rules
                WHERE sensor_id = @SensorId
                ORDER BY name";

            var rules = new List<AlertRule>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@SensorId", sensorId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                rules.Add(MapToAlertRule(reader));
            }

            return rules;
        }

        /// <summary>
        /// Obtem apenas as regras ativas de um sensor
        /// Usado para verificar se uma leitura dispara alertas
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AlertRule>> GetActiveBySensorIdAsync(Guid sensorId)
        {
            const string query = @"
                SELECT id, sensor_id, name, condition, threshold, severity, message, is_active, created_at
                FROM alert_rules
                WHERE sensor_id = @SensorId AND is_active = TRUE
                ORDER BY name";

            var rules = new List<AlertRule>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@SensorId", sensorId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                rules.Add(MapToAlertRule(reader));
            }

            return rules;
        }

        /// <summary>
        /// Cria uma nova regra de alerta
        /// </summary>
        /// <param name="alertRule"></param>
        /// <returns></returns>
        public async Task<AlertRule> CreateAsync(AlertRule alertRule)
        {
            const string query = @"
                INSERT INTO alert_rules (id, sensor_id, name, condition, threshold, severity, message, is_active, created_at)
                VALUES (@Id, @SensorId, @Name, @Condition, @Threshold, @Severity, @Message, @IsActive, @CreatedAt)
                RETURNING id, sensor_id, name, condition, threshold, severity, message, is_active, created_at";

            alertRule.Id = Guid.NewGuid();
            alertRule.CreatedAt = DateTime.UtcNow;

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", alertRule.Id);
            command.Parameters.AddWithValue("@SensorId", alertRule.SensorId);
            command.Parameters.AddWithValue("@Name", alertRule.Name);
            command.Parameters.AddWithValue("@Condition", (int)alertRule.Condition);
            command.Parameters.AddWithValue("@Threshold", alertRule.Threshold);
            command.Parameters.AddWithValue("@Severity", (int)alertRule.Severity);
            command.Parameters.AddWithValue("@Message", alertRule.Message);
            command.Parameters.AddWithValue("@IsActive", alertRule.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", alertRule.CreatedAt);

            await using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            return MapToAlertRule(reader);
        }

        /// <summary>
        /// Atualiza uma regra existente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alertRule"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(Guid id, AlertRule alertRule)
        {
            const string query = @"
                UPDATE alert_rules
                SET name = @Name,
                    condition = @Condition,
                    threshold = @Threshold,
                    severity = @Severity,
                    message = @Message,
                    is_active = @IsActive
                WHERE id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", alertRule.Name);
            command.Parameters.AddWithValue("@Condition", (int)alertRule.Condition);
            command.Parameters.AddWithValue("@Threshold", alertRule.Threshold);
            command.Parameters.AddWithValue("@Severity", (int)alertRule.Severity);
            command.Parameters.AddWithValue("@Message", alertRule.Message);
            command.Parameters.AddWithValue("@IsActive", alertRule.IsActive);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        /// <summary>
        /// Remove uma regra
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM alert_rules WHERE id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        /// <summary>
        /// Mapeia um reader do PostgreSQL para entidade AlertRule
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static AlertRule MapToAlertRule(NpgsqlDataReader reader)
        {
            return new AlertRule
            {
                Id = reader.GetGuid(0),
                SensorId = reader.GetGuid(1),
                Name = reader.GetString(2),
                Condition = (AlertConditionEnum)reader.GetInt32(3),
                Threshold = reader.GetDecimal(4),
                Severity = (AlertSeverityEnum)reader.GetInt32(5),
                Message = reader.GetString(6),
                IsActive = reader.GetBoolean(7),
                CreatedAt = reader.GetDateTime(8)
            };
        }
    }
}