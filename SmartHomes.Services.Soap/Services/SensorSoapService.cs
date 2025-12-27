
using SmartHomes.Domain.Interfaces;
using SmartHomes.Services.Soap.Models;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.DTO;
using System.Data.SqlTypes;

namespace SmartHomes.Services.Soap.Services
{
    public class SensorSoapService : ISensorSoapService
    {

        private readonly ISensorRepository _sensorRepository;
        private readonly IHomeRepository _homeRepository;

        public SensorSoapService(ISensorRepository sensorRepository, IHomeRepository homeRepository)
        {
            _sensorRepository = sensorRepository;
            _homeRepository = homeRepository;
        }

        public async Task<SoapResponse<SensorDto>> GetSensorByIdAsync(Guid id)
        {
            try
            {
                var sensor = await _sensorRepository.GetByIdAsync(id);

                if (sensor == null)
                {
                    return new SoapResponse<SensorDto>
                    {
                        Success = false,
                        Message = $"Sensor com ID {id} nao encontrada"
                    };
                }

                return new SoapResponse<SensorDto>
                {
                    Success = true,
                    Message = "Sensor encontrado com sucesso",
                    Data = MapToDto(sensor)
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
                var sensors = await _sensorRepository.GetAllAsync();
                var sensorDtos = sensors.Select(MapToDto).ToList();

                return new SoapResponse<List<SensorDto>>
                {
                    Success = true,
                    Message = $"{sensorDtos.Count} sensor(es) encontrados",
                    Data = sensorDtos
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
                var sensors = await _sensorRepository.GetByHomeAsync(homeId);
                var sensorDtos = sensors.Select(MapToDto).ToList();

                return new SoapResponse<List<SensorDto>>
                {
                    Success = true,
                    Message = $"{sensorDtos.Count} sensor(es) encontrados na casa com ID:{homeId}",
                    Data = sensorDtos
                };
            }catch (Exception ex)
            {
                return new SoapResponse<List<SensorDto>>
                {
                    Success = false,
                    Message = $"Erro ao obter sensores: {ex.Message}"
                };
            }
        }


        /// <summary>
        /// Obtem todos os sensores ativos de uma casa especifica
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        public async Task<SoapResponse<List<SensorDto>>> GetActiveSensorsByHomeIdAsync(Guid homeId)
        {
            try
            {
                var sensors = await _sensorRepository.GetActiveByHomeIdAsync(homeId);
                var sensorDtos = sensors.Select(MapToDto).ToList();

                return new SoapResponse<List<SensorDto>>
                {
                    Success = true,
                    Message = $"{sensorDtos.Count} sensor(es) ativos encontrados na casa com ID:{homeId}",
                    Data = sensorDtos
                };
            }catch (Exception ex)
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

                var homeExists = await _homeRepository.GetByIdAsync(request.HomeId);
                if (homeExists == null)
                {
                    return new SoapResponse<SensorDto>
                    {
                        Success = false,
                        Message = $"Casa com ID {request.HomeId} não encontrada"
                    };
                }
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return new SoapResponse<SensorDto>
                    {
                        Success = false,
                        Message = "Nome do sensor e obrigatório"
                    };
                }

                var sensor = new Sensor
                {
                    HomeId = request.HomeId,
                    Type = request.Type,
                    Unit = request.Unit,
                    Name = request.Name,
                    IsActive = true
                };

                var createdSensor = await _sensorRepository.CreateAsync(sensor);

                return new SoapResponse<SensorDto>
                {
                    Success = true,
                    Message = "Sensor criado com sucesso",
                    Data = MapToDto(createdSensor)
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
                var existingSensor = await _sensorRepository.GetByIdAsync(id);

                if (existingSensor == null)
                {
                    return new SoapResponse<bool>
                    {
                        Success = false,
                        Message = $"Sensor com ID {id} não encontrado"
                    };
                }

                // Atualizar campos fornecidos
                if (!string.IsNullOrWhiteSpace(request.Name))
                    existingSensor.Name = request.Name.Trim();

                if (request.Type.HasValue)
                    existingSensor.Type = request.Type.Value;

                if (request.Unit.HasValue)
                    existingSensor.Unit = request.Unit.Value;

                if (request.IsActive.HasValue)
                    existingSensor.IsActive = request.IsActive.Value;

                var result = await _sensorRepository.UpdateAsync(id, existingSensor);

                return new SoapResponse<bool>
                {
                    Success = result,
                    Message = result ? "Sensor atualizado com sucesso" : "Falha ao atualizar sensor",
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
                var result = await _sensorRepository.DeleteAsync(id);

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



        /// <summary>
        /// Mapeia a entity Sensor para SensorDto
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
