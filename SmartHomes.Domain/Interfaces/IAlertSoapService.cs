using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks; // Adicionado para suportar Task
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Models;

namespace SmartHomes.Domain.Interfaces
{
    /// <summary>
    /// Define o contrato SOAP para operacoes sobre alertas disparados
    /// </summary>
    [ServiceContract]
    public interface IAlertSoapService
    {
        /// <summary>
        /// Obtem um alerta pelo ID via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<AlertDto>> GetAlertByIdAsync(Guid id);

        /// <summary>
        /// Obtem alertas de um sensor via SOAP
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<List<AlertDto>>> GetAlertsBySensorIdAsync(Guid sensorId, int limit);

        /// <summary>
        /// Obtem alertas de uma casa via SOAP
        /// </summary>
        /// <param name="homeId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<List<AlertDto>>> GetAlertsByHomeIdAsync(Guid homeId, int limit);

        /// <summary>
        /// Obtem alertas nao reconhecidos via SOAP
        /// </summary>
        /// <param name="homeId"></param>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<List<AlertDto>>> GetUnacknowledgedAlertsAsync(Guid? homeId);

        /// <summary>
        /// Marca um alerta como reconhecido via SOAP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Task<SoapResponse<bool>> AcknowledgeAlertAsync(Guid id);
    }
}