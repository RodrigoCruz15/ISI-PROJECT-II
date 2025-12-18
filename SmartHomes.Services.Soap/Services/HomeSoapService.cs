
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Domain.Enums;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.Services.Soap.Services;
/// <summary>
/// Implementação do serviço SOAP para gestão de casas
/// Atua como camada de integração entre a API REST e a Infrastructure
/// </summary>
public class HomeSoapService : IHomeSoapService
{
    private readonly IHomeRepository _homeRepository;

    public HomeSoapService(IHomeRepository homeRepository)
    {
        _homeRepository = homeRepository;
    }

    /// <summary>
    /// Obtém uma casa pelo ID
    /// </summary>
    public async Task<SoapResponse<HomeDto>> GetHomeByIdAsync(Guid id)
    {
        try
        {
            var home = await _homeRepository.GetByIdAsync(id);

            if (home == null)
            {
                return new SoapResponse<HomeDto>
                {
                    Success = false,
                    Message = $"Casa com ID {id} não encontrada"
                };
            }

            return new SoapResponse<HomeDto>
            {
                Success = true,
                Message = "Casa encontrada com sucesso",
                Data = MapToDto(home)
            };
        }
        catch (Exception ex)
        {
            return new SoapResponse<HomeDto>
            {
                Success = false,
                Message = $"Erro ao obter casa: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtém todas as casas registadas
    /// </summary>
    public async Task<SoapResponse<List<HomeDto>>> GetAllHomesAsync()
    {
        try
        {
            var homes = await _homeRepository.GetAllAsync();
            var homeDtos = homes.Select(MapToDto).ToList();

            return new SoapResponse<List<HomeDto>>
            {
                Success = true,
                Message = $"{homeDtos.Count} casa(s) encontrada(s)",
                Data = homeDtos
            };
        }
        catch (Exception ex)
        {
            return new SoapResponse<List<HomeDto>>
            {
                Success = false,
                Message = $"Erro ao obter casas: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Cria uma nova casa
    /// </summary>
    public async Task<SoapResponse<HomeDto>> CreateHomeAsync(CreateHomeRequest request)
    {
        try
        {
            // Mapear do request para entidade
            var home = new Home
            {
                OwnerId = request.OwnerId,
                Name = request.Name,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Area = request.Area,
                Status = HomeStatus.Active
            };

            var createdHome = await _homeRepository.CreateAsync(home);

            return new SoapResponse<HomeDto>
            {
                Success = true,
                Message = "Casa criada com sucesso",
                Data = MapToDto(createdHome)
            };
        }
        catch (Exception ex)
        {
            return new SoapResponse<HomeDto>
            {
                Success = false,
                Message = $"Erro ao criar casa: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Atualiza uma casa existente
    /// </summary>
    public async Task<SoapResponse<bool>> UpdateHomeAsync(Guid id, UpdateHomeRequest request)
    {
        try
        {
            // Obter casa existente
            var existingHome = await _homeRepository.GetByIdAsync(id);

            if (existingHome == null)
            {
                return new SoapResponse<bool>
                {
                    Success = false,
                    Message = $"Casa com ID {id} não encontrada"
                };
            }

            // Atualizar apenas os campos fornecidos
            if (!string.IsNullOrEmpty(request.Name))
                existingHome.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Address))
                existingHome.Address = request.Address;

            if (request.Latitude.HasValue)
                existingHome.Latitude = request.Latitude.Value;

            if (request.Longitude.HasValue)
                existingHome.Longitude = request.Longitude.Value;

            if (request.Area.HasValue)
                existingHome.Area = request.Area.Value;

            if (request.Status.HasValue)
                existingHome.Status = request.Status.Value;

            var result = await _homeRepository.UpdateAsync(id, existingHome);

            return new SoapResponse<bool>
            {
                Success = result,
                Message = result ? "Casa atualizada com sucesso" : "Falha ao atualizar casa",
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new SoapResponse<bool>
            {
                Success = false,
                Message = $"Erro ao atualizar casa: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Remove uma casa do sistema
    /// </summary>
    public async Task<SoapResponse<bool>> DeleteHomeAsync(Guid id)
    {
        try
        {
            var result = await _homeRepository.DeleteAsync(id);

            return new SoapResponse<bool>
            {
                Success = result,
                Message = result ? "Casa removida com sucesso" : "Casa não encontrada",
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new SoapResponse<bool>
            {
                Success = false,
                Message = $"Erro ao remover casa: {ex.Message}"
            };
        }
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