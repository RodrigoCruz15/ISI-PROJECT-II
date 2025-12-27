using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.Services.Soap.Services
{
    /// <summary>
    /// Implementação do serviço SOAP para gestão de leituras de sensores
    /// Atua como camada de integração para registar e consultar leituras
    /// </summary>
    public class SensorReadingSoapService : ISensorReadingSoapService
    {
        private readonly ISensorReadingRepository _readingRepository;
        private readonly ISensorRepository _sensorRepository;

        public SensorReadingSoapService(ISensorReadingRepository readingRepository, ISensorRepository sensorRepository)
        {
            _readingRepository = readingRepository;
            _sensorRepository = sensorRepository;
        }

        /// <summary>
        /// Obtém uma leitura pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<SensorReadingDto>> GetReadingByIdAsync(Guid id)
        {
            try
            {
                var reading = await _readingRepository.GetByIdAsync(id);

                if (reading == null)
                {
                    return new SoapResponse<SensorReadingDto>
                    {
                        Success = false,
                        Message = $"Leitura com ID {id} não encontrada"
                    };
                }

                return new SoapResponse<SensorReadingDto>
                {
                    Success = true,
                    Message = "Leitura encontrada com sucesso",
                    Data = MapToDto(reading)
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<SensorReadingDto>
                {
                    Success = false,
                    Message = $"Erro ao obter leitura: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtém leituras de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<SensorReadingDto>>> GetReadingsBySensorIdAsync(Guid sensorId, int limit)
        {
            try
            {
                var readings = await _readingRepository.GetBySensorIdAsync(sensorId, limit);
                var readingDtos = readings.Select(MapToDto).ToList();

                return new SoapResponse<List<SensorReadingDto>>
                {
                    Success = true,
                    Message = $"{readingDtos.Count} leitura(s) encontrada(s)",
                    Data = readingDtos
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<List<SensorReadingDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter leituras: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtém a última leitura de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<SensorReadingDto>> GetLatestReadingAsync(Guid sensorId)
        {
            try
            {
                var reading = await _readingRepository.GetLatestBySensorIdAsync(sensorId);

                if (reading == null)
                {
                    return new SoapResponse<SensorReadingDto>
                    {
                        Success = false,
                        Message = "Nenhuma leitura encontrada para este sensor"
                    };
                }

                return new SoapResponse<SensorReadingDto>
                {
                    Success = true,
                    Message = "Última leitura obtida com sucesso",
                    Data = MapToDto(reading)
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<SensorReadingDto>
                {
                    Success = false,
                    Message = $"Erro ao obter última leitura: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Cria uma nova leitura
        /// Valida se o sensor existe e está ativo antes de criar
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<SensorReadingDto>> CreateReadingAsync(CreateSensorReadingRequest request)
        {
            try
            {
                // Verificar se o sensor existe
                var sensor = await _sensorRepository.GetByIdAsync(request.SensorId);
                if (sensor == null)
                {
                    return new SoapResponse<SensorReadingDto>
                    {
                        Success = false,
                        Message = $"Sensor com ID {request.SensorId} não encontrado"
                    };
                }

                // Verificar se o sensor está ativo
                if (!sensor.IsActive)
                {
                    return new SoapResponse<SensorReadingDto>
                    {
                        Success = false,
                        Message = $"Sensor '{sensor.Name}' está inativo"
                    };
                }

                // Criar leitura
                var reading = new SensorReading
                {
                    SensorId = request.SensorId,
                    Value = request.Value
                };

                var createdReading = await _readingRepository.CreateAsync(reading);

                // Atualizar timestamp da última leitura no sensor
                await _sensorRepository.UpdateLastReadingAsync(request.SensorId, createdReading.Timestamp);

                return new SoapResponse<SensorReadingDto>
                {
                    Success = true,
                    Message = "Leitura criada com sucesso",
                    Data = MapToDto(createdReading)
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<SensorReadingDto>
                {
                    Success = false,
                    Message = $"Erro ao criar leitura: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Mapeia SensorReading para SensorReadingDto
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