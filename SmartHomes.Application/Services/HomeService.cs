using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Enums;
using SmartHomes.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Application.Services
{
    /// <summary>
    /// Implementação da lógica de negócio para gestão de casas
    /// </summary>
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;
        private readonly IWeatherService _weatherService;
        private readonly ISensorReadingRepository _readingRepository;
        private readonly ISensorRepository _sensorRepository;

        public HomeService(IHomeRepository homeRepository, IWeatherService weatherService, ISensorReadingRepository readingRepository, ISensorRepository sensorRepository)
        {
            _homeRepository = homeRepository;
            _weatherService = weatherService;
            _readingRepository = readingRepository;
            _sensorRepository = sensorRepository;
        }

        /// <summary>
        /// Obtém uma casa pelo ID
        /// </summary>
        public async Task<HomeDto?> GetHomeByIdAsync(Guid id)
        {
            var home = await _homeRepository.GetByIdAsync(id);

            if (home == null)
                return null;

            return MapToDto(home);
        }

        /// <summary>
        /// Obtem todas as casas de um utilizador
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<HomeDto>> GetHomesByUserIdAsync(Guid userId)
        {
            var homes = await _homeRepository.GetByUserIdAsync(userId);
            return homes.Select(MapToDto);
        }

        /// <summary>
        /// Obtém todas as casas
        /// </summary>
        public async Task<IEnumerable<HomeDto>> GetAllHomesAsync()
        {
            var homes = await _homeRepository.GetAllAsync();
            return homes.Select(MapToDto);
        }

        /// <summary>
        /// Cria uma nova casa com validações de negócio
        /// </summary>
        public async Task<HomeDto> CreateHomeAsync(CreateHomeRequest request)
        {
            // Validações de negócio
            ValidateCreateRequest(request);

            // Mapear para entidade
            var home = new Home
            {
                UserId = request.UserId,
                Name = request.Name.Trim(),
                Address = request.Address.Trim(),
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Area = request.Area,
                Status = HomeStatus.Active
            };

            var createdHome = await _homeRepository.CreateAsync(home);
            return MapToDto(createdHome);
        }

        /// <summary>
        /// Atualiza uma casa existente
        /// </summary>
        public async Task<bool> UpdateHomeAsync(Guid id, UpdateHomeRequest request)
        {
            // Verificar se existe
            var existingHome = await _homeRepository.GetByIdAsync(id);
            if (existingHome == null)
                return false;

            // Validações de negócio
            ValidateUpdateRequest(request);

            // Atualizar apenas os campos fornecidos
            if (!string.IsNullOrWhiteSpace(request.Name))
                existingHome.Name = request.Name.Trim();

            if (!string.IsNullOrWhiteSpace(request.Address))
                existingHome.Address = request.Address.Trim();

            if (request.Latitude.HasValue)
                existingHome.Latitude = request.Latitude.Value;

            if (request.Longitude.HasValue)
                existingHome.Longitude = request.Longitude.Value;

            if (request.Area.HasValue)
                existingHome.Area = request.Area.Value;

            if (request.Status.HasValue)
                existingHome.Status = request.Status.Value;

            return await _homeRepository.UpdateAsync(id, existingHome);
        }

        /// <summary>
        /// Remove uma casa (verificando ownership)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteHomeAsync(Guid id, Guid userId)
        {
            var home = await _homeRepository.GetByIdAsync(id);
            if (home == null)
                return false;

            // Verificar se pertence ao utilizador
            if (home.UserId != userId)
                throw new UnauthorizedAccessException("Não tem permissão para eliminar esta casa");

            return await _homeRepository.DeleteAsync(id, userId);
        }


        // Adicione este metodo
        /// <summary>
        /// Obtem dados da casa com informacao meteorologica
        /// </summary>
        /// <param name="id">ID da casa</param>
        /// <returns>Casa com clima exterior e temperatura interior</returns>
        public async Task<HomeWithWeatherDto?> GetHomeWithWeatherAsync(Guid id)
        {
            // 1. Obter dados da casa
            var home = await _homeRepository.GetByIdAsync(id);
            if (home == null)
                return null;

            // 2. Obter clima exterior via API
            var weather = await _weatherService.GetWeatherByCoordinatesAsync(home.Latitude, home.Longitude);

            // 3. Calcular temperatura interior media (dos sensores de temperatura)
            decimal? indoorTemp = await GetAverageIndoorTemperatureAsync(id);

            // 4. Criar resposta combinada
            var result = new HomeWithWeatherDto
            {
                HomeId = home.Id,
                HomeName = home.Name,
                Address = home.Address,
                Latitude = home.Latitude,
                Longitude = home.Longitude,
                IndoorTemperature = indoorTemp,
                Weather = weather
            };

            // 5. Calcular diferenca e dar recomendacao
            if (indoorTemp.HasValue && weather != null)
            {
                result.TemperatureDifference = indoorTemp.Value - weather.Temperature;
                result.Recommendation = GenerateRecommendation(indoorTemp.Value, weather.Temperature);
            }

            return result;
        }

        /// <summary>
        /// Calcula temperatura interior media dos sensores de temperatura
        /// </summary>
        /// <param name="homeId">ID da casa</param>
        /// <returns>Temperatura media ou null</returns>
        private async Task<decimal?> GetAverageIndoorTemperatureAsync(Guid homeId)
        {
            try
            {
                // Obter sensores de temperatura da casa
                var sensors = await _sensorRepository.GetActiveByHomeIdAsync(homeId);
                var tempSensors = sensors.Where(s => s.Type == SensorTypeEnum.Temperature).ToList();

                if (!tempSensors.Any())
                    return null;

                // Obter ultima leitura de cada sensor
                var readings = new List<decimal>();
                foreach (var sensor in tempSensors)
                {
                    var lastReading = await _readingRepository.GetLatestBySensorIdAsync(sensor.Id);
                    if (lastReading != null)
                        readings.Add(lastReading.Value);
                }

                return readings.Any() ? readings.Average() : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gera recomendacao baseada na diferenca de temperatura
        /// </summary>
        /// <param name="indoor">Temperatura interior</param>
        /// <param name="outdoor">Temperatura exterior</param>
        /// <returns>Recomendacao</returns>
        private static string GenerateRecommendation(decimal indoor, decimal outdoor)
        {
            var diff = indoor - outdoor;

            if (Math.Abs(diff) < 2)
                return "Temperatura equilibrada";

            if (diff > 5)
                return "Interior muito mais quente. Considere abrir janelas ou ligar ar condicionado";

            if (diff < -5)
                return "Interior muito mais frio. Considere ligar aquecimento";

            if (diff > 0)
                return "Interior ligeiramente mais quente que o exterior";

            return "Interior ligeiramente mais frio que o exterior";
        }

        /// <summary>
        /// Valida o pedido de criação de casa
        /// </summary>
        private static void ValidateCreateRequest(CreateHomeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("O nome da casa é obrigatório", nameof(request.Name));

            if (request.Name.Length > 200)
                throw new ArgumentException("O nome da casa não pode exceder 200 caracteres", nameof(request.Name));

            if (string.IsNullOrWhiteSpace(request.Address))
                throw new ArgumentException("A morada é obrigatória", nameof(request.Address));

            if (request.Address.Length > 500)
                throw new ArgumentException("A morada não pode exceder 500 caracteres", nameof(request.Address));

            if (request.Latitude < -90 || request.Latitude > 90)
                throw new ArgumentException("Latitude deve estar entre -90 e 90", nameof(request.Latitude));

            if (request.Longitude < -180 || request.Longitude > 180)
                throw new ArgumentException("Longitude deve estar entre -180 e 180", nameof(request.Longitude));

            if (request.Area <= 0)
                throw new ArgumentException("A área deve ser maior que zero", nameof(request.Area));
        }

        /// <summary>
        /// Valida o pedido de atualização de casa
        /// </summary>
        private static void ValidateUpdateRequest(UpdateHomeRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Name) && request.Name.Length > 200)
                throw new ArgumentException("O nome da casa não pode exceder 200 caracteres", nameof(request.Name));

            if (!string.IsNullOrWhiteSpace(request.Address) && request.Address.Length > 500)
                throw new ArgumentException("A morada não pode exceder 500 caracteres", nameof(request.Address));

            if (request.Latitude.HasValue && (request.Latitude < -90 || request.Latitude > 90))
                throw new ArgumentException("Latitude deve estar entre -90 e 90", nameof(request.Latitude));

            if (request.Longitude.HasValue && (request.Longitude < -180 || request.Longitude > 180))
                throw new ArgumentException("Longitude deve estar entre -180 e 180", nameof(request.Longitude));

            if (request.Area.HasValue && request.Area <= 0)
                throw new ArgumentException("A área deve ser maior que zero", nameof(request.Area));
        }

        /// <summary>
        /// Mapeia uma entidade Home para HomeDto
        /// </summary>
        private static HomeDto MapToDto(Home home)
        {
            return new HomeDto
            {
                Id = home.Id,
                OwnerId = home.UserId,
                Name = home.Name,
                Address = home.Address,
                Latitude = home.Latitude,
                Longitude = home.Longitude,
                Area = home.Area,
                Status = home.Status,
                CreatedAt = home.CreatedAt
            };
        }

    }
}
