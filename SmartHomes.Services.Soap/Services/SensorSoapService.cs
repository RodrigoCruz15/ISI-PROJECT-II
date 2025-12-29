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
    /// Implementacao do servico SOAP para gestao de sensores
    /// Atua como camada de integracao entre a API REST e a Application
    /// </summary>
    public class SensorSoapService : ISensorSoapService
    {
        private readonly ISensorService _sensorService;

        public SensorSoapService(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        /// <summary>
        /// Obtem um sensor pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<SensorDto>> GetSensorByIdAsync(Guid id)
        {
            try
            {
                var sensor = await _sensorService.GetSensorByIdAsync(id);

                if (sensor == null)
                {
                    return new SoapResponse<SensorDto>
                    {
                        Success = false,
                        Message = $"Sensor com ID {id} não encontrado"
                    };
                }

                return new SoapResponse<SensorDto>
                {
                    Success = true,
                    Message = "Sensor encontrado com sucesso",
                    Data = sensor
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<SensorDto>
                {
                    Success = false,
                    Message = $"Erro ao obter sensor: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtem todos os sensores
        /// </summary>
        /// <returns></returns>
        public async Task<SoapResponse<List<SensorDto>>> GetAllSensorsAsync()
        {
            try
            {
                var sensors = await _sensorService.GetAllSensorAsync();
                var sensorsList = sensors.ToList();

                return new SoapResponse<List<SensorDto>>
                {
                    Success = true,
                    Message = $"{sensorsList.Count} sensor(es) encontrado(s)",
                    Data = sensorsList
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<List<SensorDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter sensores: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtem sensores de uma casa especifica
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<SensorDto>>> GetSensorsByHomeIdAsync(Guid homeId)
        {
            try
            {
                var sensors = await _sensorService.GetSensorsByHomeIdAsync(homeId);
                var sensorsList = sensors.ToList();

                return new SoapResponse<List<SensorDto>>
                {
                    Success = true,
                    Message = $"{sensorsList.Count} sensor(es) encontrado(s) na casa",
                    Data = sensorsList
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<List<SensorDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter sensores: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtem sensores ativos de uma casa
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<SensorDto>>> GetActiveSensorsByHomeIdAsync(Guid homeId)
        {
            try
            {
                var sensors = await _sensorService.GetActiveSensorsByHomeIdAsync(homeId);
                var sensorsList = sensors.ToList();

                return new SoapResponse<List<SensorDto>>
                {
                    Success = true,
                    Message = $"{sensorsList.Count} sensor(es) ativo(s) encontrado(s)",
                    Data = sensorsList
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<List<SensorDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter sensores ativos: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Cria um novo sensor
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<SensorDto>> CreateSensorAsync(CreateSensorRequest request)
        {
            try
            {
                var createdSensor = await _sensorService.CreateSensorAsync(request);

                return new SoapResponse<SensorDto>
                {
                    Success = true,
                    Message = "Sensor criado com sucesso",
                    Data = createdSensor
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<SensorDto>
                {
                    Success = false,
                    Message = $"Erro ao criar sensor: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Atualiza um sensor existente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> UpdateSensorAsync(Guid id, UpdateSensorRequest request)
        {
            try
            {
                var result = await _sensorService.UpdateSensorAsync(id, request);

                return new SoapResponse<bool>
                {
                    Success = result,
                    Message = result ? "Sensor atualizado com sucesso" : "Sensor não encontrado",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<bool>
                {
                    Success = false,
                    Message = $"Erro ao atualizar sensor: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Remove um sensor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> DeleteSensorAsync(Guid id)
        {
            try
            {
                var result = await _sensorService.DeleteSensorAsync(id);

                return new SoapResponse<bool>
                {
                    Success = result,
                    Message = result ? "Sensor removido com sucesso" : "Sensor não encontrado",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<bool>
                {
                    Success = false,
                    Message = $"Erro ao remover sensor: {ex.Message}"
                };
            }
        }
    }
}