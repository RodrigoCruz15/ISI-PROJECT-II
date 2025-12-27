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
    /// Implementacao do repositorio de alertas disparados com acesso direto ao PostgreSQL
    /// </summary>
    public class AlertRepository : IAlertRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public AlertRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Obtem um alerta pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Alert?> GetByIdAsync(Guid id)
        {
            const string query = @"
                SELECT id, alert_rule_id, sensor_reading_id, sensor_id, value, threshold, 
                       severity, message, triggered_at, is_acknowledged, acknowledged_at
                FROM alerts
                WHERE id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapToAlert(reader);
        }

        /// <summary>
        /// Obtem todos os alertas de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Alert>> GetBySensorIdAsync(Guid sensorId, int? limit = null)
        {
            var query = @"
                SELECT id, alert_rule_id, sensor_reading_id, sensor_id, value, threshold, 
                       severity, message, triggered_at, is_acknowledged, acknowledged_at
                FROM alerts
                WHERE sensor_id = @SensorId
                ORDER BY triggered_at DESC";

            if (limit.HasValue)
                query += " LIMIT @Limit";

            var alerts = new List<Alert>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@SensorId", sensorId);
            if (limit.HasValue)
                command.Parameters.AddWithValue("@Limit", limit.Value);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                alerts.Add(MapToAlert(reader));
            }

            return alerts;
        }

        /// <summary>
        /// Obtem todos os alertas de uma casa
        /// </summary>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Alert>> GetByHomeIdAsync(Guid homeId, int? limit = null)
        {
            var query = @"
                SELECT a.id, a.alert_rule_id, a.sensor_reading_id, a.sensor_id, a.value, a.threshold, 
                       a.severity, a.message, a.triggered_at, a.is_acknowledged, a.acknowledged_at
                FROM alerts a
                INNER JOIN sensors s ON a.sensor_id = s.id
                WHERE s.home_id = @HomeId
                ORDER BY a.triggered_at DESC";

            if (limit.HasValue)
                query += " LIMIT @Limit";

            var alerts = new List<Alert>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@HomeId", homeId);
            if (limit.HasValue)
                command.Parameters.AddWithValue("@Limit", limit.Value);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                alerts.Add(MapToAlert(reader));
            }

            return alerts;
        }

        /// <summary>
        /// Obtem alertas nao reconhecidos (pendentes)
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Alert>> GetUnacknowledgedAsync(Guid? homeId = null)
        {
            string query;

            if (homeId.HasValue)
            {
                query = @"
                    SELECT a.id, a.alert_rule_id, a.sensor_reading_id, a.sensor_id, a.value, a.threshold, 
                           a.severity, a.message, a.triggered_at, a.is_acknowledged, a.acknowledged_at
                    FROM alerts a
                    INNER JOIN sensors s ON a.sensor_id = s.id
                    WHERE a.is_acknowledged = FALSE AND s.home_id = @HomeId
                    ORDER BY a.triggered_at DESC";
            }
            else
            {
                query = @"
                    SELECT id, alert_rule_id, sensor_reading_id, sensor_id, value, threshold, 
                           severity, message, triggered_at, is_acknowledged, acknowledged_at
                    FROM alerts
                    WHERE is_acknowledged = FALSE
                    ORDER BY triggered_at DESC";
            }

            var alerts = new List<Alert>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            if (homeId.HasValue)
                command.Parameters.AddWithValue("@HomeId", homeId.Value);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                alerts.Add(MapToAlert(reader));
            }

            return alerts;
        }

        /// <summary>
        /// Obtem alertas por gravidade
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Alert>> GetBySeverityAsync(int severity, Guid? homeId = null, int? limit = null)
        {
            string query;

            if (homeId.HasValue)
            {
                query = @"
                    SELECT a.id, a.alert_rule_id, a.sensor_reading_id, a.sensor_id, a.value, a.threshold, 
                           a.severity, a.message, a.triggered_at, a.is_acknowledged, a.acknowledged_at
                    FROM alerts a
                    INNER JOIN sensors s ON a.sensor_id = s.id
                    WHERE a.severity = @Severity AND s.home_id = @HomeId
                    ORDER BY a.triggered_at DESC";
            }
            else
            {
                query = @"
                    SELECT id, alert_rule_id, sensor_reading_id, sensor_id, value, threshold, 
                           severity, message, triggered_at, is_acknowledged, acknowledged_at
                    FROM alerts
                    WHERE severity = @Severity
                    ORDER BY triggered_at DESC";
            }

            if (limit.HasValue)
                query += " LIMIT @Limit";

            var alerts = new List<Alert>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Severity", severity);
            if (homeId.HasValue)
                command.Parameters.AddWithValue("@HomeId", homeId.Value);
            if (limit.HasValue)
                command.Parameters.AddWithValue("@Limit", limit.Value);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                alerts.Add(MapToAlert(reader));
            }

            return alerts;
        }

        /// <summary>
        /// Cria um novo alerta
        /// Chamado automaticamente quando uma leitura viola uma regra
        /// </summary>
        /// <param name="alert"></param>
        /// <returns></returns>
        public async Task<Alert> CreateAsync(Alert alert)
        {
            const string query = @"
                INSERT INTO alerts (id, alert_rule_id, sensor_reading_id, sensor_id, value, threshold, 
                                   severity, message, triggered_at, is_acknowledged, acknowledged_at)
                VALUES (@Id, @AlertRuleId, @SensorReadingId, @SensorId, @Value, @Threshold, 
                        @Severity, @Message, @TriggeredAt, @IsAcknowledged, @AcknowledgedAt)
                RETURNING id, alert_rule_id, sensor_reading_id, sensor_id, value, threshold, 
                          severity, message, triggered_at, is_acknowledged, acknowledged_at";

            alert.Id = Guid.NewGuid();
            alert.TriggeredAt = DateTime.UtcNow;

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", alert.Id);
            command.Parameters.AddWithValue("@AlertRuleId", alert.AlertRuleId);
            command.Parameters.AddWithValue("@SensorReadingId", alert.SensorReadingId);
            command.Parameters.AddWithValue("@SensorId", alert.SensorId);
            command.Parameters.AddWithValue("@Value", alert.Value);
            command.Parameters.AddWithValue("@Threshold", alert.Threshold);
            command.Parameters.AddWithValue("@Severity", (int)alert.Severity);
            command.Parameters.AddWithValue("@Message", alert.Message);
            command.Parameters.AddWithValue("@TriggeredAt", alert.TriggeredAt);
            command.Parameters.AddWithValue("@IsAcknowledged", alert.IsAcknowledged);
            command.Parameters.AddWithValue("@AcknowledgedAt",
                alert.AcknowledgedAt.HasValue ? alert.AcknowledgedAt.Value : DBNull.Value);

            await using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            return MapToAlert(reader);
        }

        /// <summary>
        /// Marca um alerta como reconhecido/lido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> AcknowledgeAsync(Guid id)
        {
            const string query = @"
                UPDATE alerts
                SET is_acknowledged = TRUE,
                    acknowledged_at = @AcknowledgedAt
                WHERE id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@AcknowledgedAt", DateTime.UtcNow);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        /// <summary>
        /// Conta alertas nao reconhecidos de uma casa
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<int> CountUnacknowledgedByHomeIdAsync(Guid homeId)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM alerts a
                INNER JOIN sensors s ON a.sensor_id = s.id
                WHERE a.is_acknowledged = FALSE AND s.home_id = @HomeId";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@HomeId", homeId);

            var count = await command.ExecuteScalarAsync();
            return Convert.ToInt32(count);
        }

        /// <summary>
        /// Mapeia um reader do PostgreSQL para entidade Alert
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Alert MapToAlert(NpgsqlDataReader reader)
        {
            return new Alert
            {
                Id = reader.GetGuid(0),
                AlertRuleId = reader.GetGuid(1),
                SensorReadingId = reader.GetGuid(2),
                SensorId = reader.GetGuid(3),
                Value = reader.GetDecimal(4),
                Threshold = reader.GetDecimal(5),
                Severity = (AlertSeverityEnum)reader.GetInt32(6),
                Message = reader.GetString(7),
                TriggeredAt = reader.GetDateTime(8),
                IsAcknowledged = reader.GetBoolean(9),
                AcknowledgedAt = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
            };
        }
    }
}