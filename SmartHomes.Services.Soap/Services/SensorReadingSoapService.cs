using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Services.Soap.Services;
using SmartHomes.Domain.Models;
using SmartHomes.Domain.Interfaces;

namespace SmartHomes.Services.Soap.Services
{
    /// <summary>
    /// Implementacao do servico SOAP para gestao de leituras de sensores
    /// Atua como camada de integracao entre a API REST e a Application
    /// </summary>
    public class SensorReadingSoapService : ISensorReadingSoapService
    {
        private readonly ISensorReadingService _readingService;

        public SensorReadingSoapService(ISensorReadingService readingService)
        {
            _readingService = readingService;
        }

        /// <summary>
        /// Obtem uma leitura pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<SensorReadingDto>> GetReadingByIdAsync(Guid id)
        {
            try
            {
                var reading = await _readingService.GetReadingByIdAsync(id);

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
                    Data = reading
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
        /// Obtem leituras de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<SensorReadingDto>>> GetReadingsBySensorIdAsync(Guid sensorId, int limit)
        {
            try
            {
                var readings = await _readingService.GetReadingsBySensorIdAsync(sensorId, limit);
                var readingsList = readings.ToList();

                return new SoapResponse<List<SensorReadingDto>>
                {
                    Success = true,
                    Message = $"{readingsList.Count} leitura(s) encontrada(s)",
                    Data = readingsList
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
        /// Obtem a ultima leitura de um sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<SensorReadingDto>> GetLatestReadingAsync(Guid sensorId)
        {
            try
            {
                var reading = await _readingService.GetLatestReadingAsync(sensorId);

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
                    Data = reading
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
        /// Verifica alertas automaticamente
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<SensorReadingDto>> CreateReadingAsync(CreateSensorReadingRequest request)
        {
            try
            {
                var createdReading = await _readingService.CreateReadingAsync(request);

                return new SoapResponse<SensorReadingDto>
                {
                    Success = true,
                    Message = "Leitura criada com sucesso",
                    Data = createdReading
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
    }
}