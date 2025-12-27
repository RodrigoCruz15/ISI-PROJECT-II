using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Interfaces;

namespace SmartHomes.Application.Services
{
    /// <summary>
    /// Implementacao da logica de negocio para gestao de leituras de sensores
    /// Responsavel por: criar leituras, buscar historico, atualizar timestamp, verificar alertas
    /// </summary>
    public class SensorReadingService : ISensorReadingService
    {
        private readonly ISensorReadingRepository _readingRepository;
        private readonly ISensorRepository _sensorRepository;
        private readonly IAlertService _alertService;

        public SensorReadingService(ISensorReadingRepository readingRepository, ISensorRepository sensorRepository, IAlertService alertService)
        {
            _readingRepository = readingRepository;
            _sensorRepository = sensorRepository;
            _alertService = alertService;
        }

        /// <summary>
        /// Obtem uma leitura pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SensorReadingDto?> GetReadingByIdAsync(Guid id)
        {
            var reading = await _readingRepository.GetByIdAsync(id);

            if (reading == null)
                return null;

            return MapToDto(reading);
        }

        /// <summary>
        /// Obtem as ultimas N leituras de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SensorReadingDto>> GetReadingsBySensorIdAsync(Guid sensorId, int limit = 100)
        {
            var readings = await _readingRepository.GetBySensorIdAsync(sensorId, limit);
            return readings.Select(MapToDto);
        }

        /// <summary>
        /// Obtem a ultima leitura de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public async Task<SensorReadingDto?> GetLatestReadingAsync(Guid sensorId)
        {
            var reading = await _readingRepository.GetLatestBySensorIdAsync(sensorId);

            if (reading == null)
                return null;

            return MapToDto(reading);
        }

        /// <summary>
        /// Cria uma nova leitura com validacoes
        /// Atualiza automaticamente a data da ultima leitura no sensor
        /// Verifica e cria alertas automaticamente se regras forem violadas
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SensorReadingDto> CreateReadingAsync(CreateSensorReadingRequest request)
        {
            //Validacao: verificar se o sensor existe e esta ativo
            var sensor = await _sensorRepository.GetByIdAsync(request.SensorId);
            if (sensor == null)
                throw new ArgumentException($"Sensor com ID {request.SensorId} nao encontrado");

            if (!sensor.IsActive)
                throw new InvalidOperationException($"Sensor {sensor.Name} esta inativo");

            ValidateReadingValue(request.Value);

            var reading = new SensorReading
            {
                SensorId = request.SensorId,
                Value = request.Value
            };

            var createdReading = await _readingRepository.CreateAsync(reading);

            //Atualiza a data da ultima leitura no sensor
            await _sensorRepository.UpdateLastReadingAsync(request.SensorId, createdReading.Timestamp);

            //Verifica e criar alertas automaticamente
            await _alertService.CheckAndCreateAlertsAsync(createdReading);

            
            return MapToDto(createdReading);
        }

        /// <summary>
        /// Valida o valor da leitura
        /// </summary>
        /// <param name="value"></param>
        private static void ValidateReadingValue(decimal value)
        {
            // Validacao basica - pode ser expandida conforme o tipo de sensor
            if (value < -273.15m) // Zero absoluto para temperatura
                throw new ArgumentException("Valor de leitura fisicamente impossivel");

            if (value > 1000000) // Limite arbitrario razoavel
                throw new ArgumentException("Valor de leitura alto");
        }

        /// <summary>
        /// Converte entidade SensorReading para SensorReadingDto
        /// </summary>
        /// <param name="reading"></param>
        /// <returns></returns>
        private static SensorReadingDto MapToDto(SensorReading reading)
        {
            return new SensorReadingDto
            {
                Id = reading.Id,
                SensorId = reading.SensorId,
                Value = reading.Value,
                Timestamp = reading.Timestamp
            };
        }
    }
}