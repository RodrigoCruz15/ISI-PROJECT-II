using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Interfaces;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.Services.Soap.Services
{
    /// <summary>
    /// Implementacao do servico SOAP para gestao de casas
    /// Atua como camada de integracao entre a API REST e a Application
    /// </summary>
    public class HomeSoapService : IHomeSoapService
    {
        private readonly IHomeService _homeService;

        public HomeSoapService(IHomeService homeService)
        {
            _homeService = homeService;
        }

        /// <summary>
        /// Obtem uma casa pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<HomeDto>> GetHomeByIdAsync(Guid id)
        {
            try
            {
                var home = await _homeService.GetHomeByIdAsync(id);

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
                    Data = home
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
        /// Obtem todas as casas registadas
        /// </summary>
        /// <returns></returns>
        public async Task<SoapResponse<List<HomeDto>>> GetAllHomesAsync()
        {
            try
            {
                var homes = await _homeService.GetAllHomesAsync();
                var homesList = homes.ToList();

                return new SoapResponse<List<HomeDto>>
                {
                    Success = true,
                    Message = $"{homesList.Count} casa(s) encontrada(s)",
                    Data = homesList
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
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<HomeDto>> CreateHomeAsync(CreateHomeRequest request)
        {
            try
            {
                var createdHome = await _homeService.CreateHomeAsync(request);

                return new SoapResponse<HomeDto>
                {
                    Success = true,
                    Message = "Casa criada com sucesso",
                    Data = createdHome
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
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> UpdateHomeAsync(Guid id, UpdateHomeRequest request)
        {
            try
            {
                var result = await _homeService.UpdateHomeAsync(id, request);

                return new SoapResponse<bool>
                {
                    Success = result,
                    Message = result ? "Casa atualizada com sucesso" : "Casa não encontrada",
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
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<bool>> DeleteHomeAsync(Guid id)
        {
            try
            {
                var result = await _homeService.DeleteHomeAsync(id);

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
        /// Obtem casa com dados meteorologicos
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SoapResponse<HomeWithWeatherDto>> GetHomeWithWeatherAsync(Guid id)
        {
            try
            {
                var homeWithWeather = await _homeService.GetHomeWithWeatherAsync(id);

                if (homeWithWeather == null)
                {
                    return new SoapResponse<HomeWithWeatherDto>
                    {
                        Success = false,
                        Message = $"Casa com ID {id} não encontrada"
                    };
                }

                return new SoapResponse<HomeWithWeatherDto>
                {
                    Success = true,
                    Message = "Casa e dados meteorológicos obtidos com sucesso",
                    Data = homeWithWeather
                };
            }
            catch (Exception ex)
            {
                return new SoapResponse<HomeWithWeatherDto>
                {
                    Success = false,
                    Message = $"Erro ao obter casa com clima: {ex.Message}"
                };
            }
        }
    }
}