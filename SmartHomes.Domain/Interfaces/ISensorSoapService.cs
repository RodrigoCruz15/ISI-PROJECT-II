using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks; // Adicionado para suportar Task
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Models;

namespace SmartHomes.Domain.Interfaces

{
    [ServiceContract]
    public interface ISensorSoapService
    {

        [OperationContract]
        Task<SoapResponse<SensorDto>> GetSensorByIdAsync(Guid id);

        [OperationContract]
        Task<SoapResponse<List<SensorDto>>> GetAllSensorsAsync();

        [OperationContract]
        Task<SoapResponse<List<SensorDto>>> GetSensorsByHomeIdAsync(Guid homeId);

        [OperationContract]
        Task<SoapResponse<List<SensorDto>>> GetActiveSensorsByHomeIdAsync(Guid homeId);

        [OperationContract]
        Task<SoapResponse<SensorDto>> CreateSensorAsync(CreateSensorRequest request);

        [OperationContract]
        Task<SoapResponse<bool>> UpdateSensorAsync(Guid id, UpdateSensorRequest request);

        [OperationContract]
        Task<SoapResponse<bool>> DeleteSensorAsync(Guid id);

    }
}
