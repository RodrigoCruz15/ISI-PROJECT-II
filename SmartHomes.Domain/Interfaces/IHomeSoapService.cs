using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks; // Adicionado para suportar Task
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Models;

namespace SmartHomes.Domain.Interfaces;

/// <summary>
/// Define o contrato SOAP para operações sobre casas
/// </summary>
[ServiceContract]
public interface IHomeSoapService
{
    /// <summary>
    /// Obtém uma casa pelo ID via SOAP
    /// </summary>
    /// 

    [OperationContract]
    Task<SoapResponse<HomeDto>> GetHomeByIdAsync(Guid id);

    /// <summary>
    /// Obtém casas pelo UserId via SOAP
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [OperationContract]
    Task<SoapResponse<List<HomeDto>>> GetHomesByUserIdAsync(Guid userId);

    /// <summary>
    /// Obtém todas as casas via SOAP
    /// </summary>
    [OperationContract]
    Task<SoapResponse<List<HomeDto>>> GetAllHomesAsync();

    /// <summary>
    /// Cria uma nova casa via SOAP
    /// </summary>
    [OperationContract]
    Task<SoapResponse<HomeDto>> CreateHomeAsync(CreateHomeRequest request);

    /// <summary>
    /// Atualiza uma casa existente via SOAP
    /// </summary>
    [OperationContract]
    Task<SoapResponse<bool>> UpdateHomeAsync(Guid id, UpdateHomeRequest request);

    /// <summary>
    /// Remove uma casa via SOAP
    /// </summary>
    [OperationContract]
    Task<SoapResponse<bool>> DeleteHomeAsync(Guid id, Guid userId);

    /// <summary>
    /// Obtem casa com dados meteorologicos via SOAP
    /// </summary>
    [OperationContract]
    Task<SoapResponse<HomeWithWeatherDto>> GetHomeWithWeatherAsync(Guid id);

}