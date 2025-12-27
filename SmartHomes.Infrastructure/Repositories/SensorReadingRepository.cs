using SmartHomes.Domain.Interfaces;
using SmartHomes.Infrastructure.Data;
using SmartHomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace SmartHomes.Infrastructure.Repositories
{
    public class SensorReadingRepository : ISensorReadingRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public SensorReadingRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<SensorReading?> GetByIdAsync(Guid id)
        {
            const string query = @"Select id, sensor_id, value, timestamp, triggered_alert, sensor From sensor_readings Where id = @Id";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) 
                return null;

            return MapToSensorReading(reader);
        }

        /// <summary>
        /// Obtem as ultimas leituras de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SensorReading>> GetBySensorIdAsync(Guid sensorId, int limit = 100)
        {
            const string query = @"SELECT id, sensor_id, value, timestamp FROM sensor_readings WHERE sensor_id = @SensorId
                ORDER BY timestamp DESC
                LIMIT @Limit";

            var readings = new List<SensorReading>();

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@SensorId", sensorId);
            command.Parameters.AddWithValue("@Limit", limit);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                readings.Add(MapToSensorReading(reader));
            }

            return readings;
        }

        /// <summary>
        /// Obtem a ultima leitura de um sensor especifico
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public async Task<SensorReading?> GetLatestBySensorIdAsync(Guid sensorId)
        {
            const string query = @"
                SELECT id, sensor_id, value, timestamp
                FROM sensor_readings
                WHERE sensor_id = @SensorId
                ORDER BY timestamp DESC
                LIMIT 1";

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@SensorId", sensorId);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapToSensorReading(reader);
        }

        /// <summary>
        /// Cria uma nova leitura
        /// </summary>
        /// <param name="reading"></param>
        /// <returns></returns>
        public async Task<SensorReading> CreateAsync(SensorReading reading)
        {
            const string query = @"
                INSERT INTO sensor_readings (id, sensor_id, value, timestamp)
                VALUES (@Id, @SensorId, @Value, @Timestamp)
                RETURNING id, sensor_id, value, timestamp";

            reading.Id = Guid.NewGuid();
            reading.Timestamp = DateTime.UtcNow;

            await using var connection = _dbConnection.CreateConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", reading.Id);
            command.Parameters.AddWithValue("@SensorId", reading.SensorId);
            command.Parameters.AddWithValue("@Value", reading.Value);
            command.Parameters.AddWithValue("@Timestamp", reading.Timestamp);

            await using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            return MapToSensorReading(reader);
        }


        /// <summary>
        /// Mapeia um reader do PostgreSQL para entidade SensorReading
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static SensorReading MapToSensorReading(NpgsqlDataReader reader)
        {
            return new SensorReading
            {
                Id = reader.GetGuid(0),
                SensorId = reader.GetGuid(1),
                Value = reader.GetDecimal(2),
                Timestamp = reader.GetDateTime(3)
            };
        }

    }
}
