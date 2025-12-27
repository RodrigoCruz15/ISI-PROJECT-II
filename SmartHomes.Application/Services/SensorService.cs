using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Application.Services
{
    public class SensorService : ISensorService
    {

        private readonly ISensorRepository _sensorRepository;
        private readonly IHomeRepository _homeRepository;

        /// <summary>
        /// Obtem todos os sensores
        /// </summary>
        /// <param name="sensorRepository"></param>
        public SensorService(ISensorRepository sensorRepository, IHomeRepository homeRepository)
        {
            _sensorRepository = sensorRepository;
            _homeRepository = homeRepository;
        }

        /// <summary>
        /// Obtem sensor pelo seu id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SensorDto?> GetSensorByIdAsync(Guid id)
        {
            var sensor = await _sensorRepository.GetByIdAsync(id);

            if (sensor == null)
                return null;

            return MapToDto(sensor);
        }

        /// <summary>
        /// Obtem todos os sensores
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SensorDto>> GetAllSensorAsync()
        {
            var sensors = await _sensorRepository.GetAllAsync();
            return sensors.Select(MapToDto);
        }

        /// <summary>
        /// Obtem todos os sensores de uma casa especifica
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SensorDto>> GetSensorsByHomeIdAsync(Guid homeId)
        {

            var sensors = await _sensorRepository.GetByHomeAsync(homeId);

            return sensors.Select(MapToDto);

        }

        /// <summary>
        /// Obtem sensores ativos de uma casa
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SensorDto>> GetActiveSensorsByHomeIdAsync(Guid homeId)
        {
            var sensor = await _sensorRepository.GetActiveByHomeIdAsync(homeId);

            return sensor.Select(MapToDto);
        }

        /// <summary>
        /// Cria um novo sensor com validacoes de negocio
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<SensorDto> CreateSensorAsync(CreateSensorRequest request)
        {

            // Validacao: verificar se a casa existe
            var homeExists = await _homeRepository.GetByIdAsync(request.HomeId);
            if (homeExists == null)
                throw new ArgumentException($"Casa com ID {request.HomeId} nao encontrada");

            ValidateCreateRequest(request);

            // Mapear para entidade
            var sensor = new Sensor
            {
                HomeId = request.HomeId,
                Type = request.Type,
                Unit = request.Unit,
                Name = request.Name.Trim(),
                IsActive = true
            };

            // Criar no repository
            var createdSensor = await _sensorRepository.CreateAsync(sensor);

            return MapToDto(createdSensor);
        }

        /// <summary>
        /// Atualizar um sensor existente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSensorAsync(Guid id, UpdateSensorRequest request)
        {
            // Verificar se o sensor existe
            var existingSensor = await _sensorRepository.GetByIdAsync(id);
            if (existingSensor == null)
                return false;

            // Validações de negócio
            ValidateUpdateRequest(request);

            // Atualizar apenas os campos fornecidos
            if (!string.IsNullOrWhiteSpace(request.Name))
                existingSensor.Name = request.Name.Trim();

            if (request.Type.HasValue)
                existingSensor.Type = request.Type.Value;

            if (request.Unit.HasValue)
                existingSensor.Unit = request.Unit.Value;

            if (request.IsActive.HasValue)
                existingSensor.IsActive = request.IsActive.Value;

            return await _sensorRepository.UpdateAsync(id, existingSensor);
        }

        /// <summary>
        /// Remove um sensor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteSensorAsync(Guid id)
        {
            // Verificar se existe antes de remover
            var existingSensor = await _sensorRepository.GetByIdAsync(id);
            if (existingSensor == null)
                return false;

            return await _sensorRepository.DeleteAsync(id);
        }


        /// <summary>
        /// Valida o pedido de criacao de sensor
        /// </summary>
        private static void ValidateCreateRequest(CreateSensorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("O nome do sensor é obrigatório", nameof(request.Name));

            if (request.Name.Length > 100)
                throw new ArgumentException("O nome do sensor não pode exceder 100 caracteres", nameof(request.Name));

            if (request.Type == SensorTypeEnum.Unknown)
                throw new ArgumentException("Tipo de sensor inválido", nameof(request.Type));

            if (request.Unit == UnitEnum.Unknown)
                throw new ArgumentException("Unidade de medida inválida", nameof(request.Unit));
        }

        /// <summary>
        /// Valida o pedido de atualização de sensor
        /// </summary>
        private static void ValidateUpdateRequest(UpdateSensorRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Name) && request.Name.Length > 100)
                throw new ArgumentException("O nome do sensor não pode exceder 100 caracteres", nameof(request.Name));

            if (request.Type.HasValue && request.Type.Value == SensorTypeEnum.Unknown)
                throw new ArgumentException("Tipo de sensor inválido", nameof(request.Type));

            if (request.Unit.HasValue && request.Unit.Value == UnitEnum.Unknown)
                throw new ArgumentException("Unidade de medida inválida", nameof(request.Unit));
        }

        /// <summary>
        /// Converte entity Sensor para SensorDto
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        private static SensorDto MapToDto(Sensor sensor)
        {
            return new SensorDto
            {
                Id = sensor.Id,
                HomeId = sensor.HomeId,
                Type = sensor.Type,
                Unit = sensor.Unit,
                Name = sensor.Name,
                IsActive = sensor.IsActive,
                CreatedAt = sensor.CreatedAt,
                LastReadingAt = sensor.LastReadingAt
            };
        }

    }
}
