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

        public HomeService(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
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
                OwnerId = request.OwnerId,
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
        /// Remove uma casa do sistema
        /// </summary>
        public async Task<bool> DeleteHomeAsync(Guid id)
        {
            // Verificar se existe antes de remover
            var existingHome = await _homeRepository.GetByIdAsync(id);
            if (existingHome == null)
                return false;

            return await _homeRepository.DeleteAsync(id);
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
                OwnerId = home.OwnerId,
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
