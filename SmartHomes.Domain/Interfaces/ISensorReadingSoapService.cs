using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks; // Adicionado para suportar Task
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Models;

namespace SmartHomes.Domain.Interfaces
{
    [ServiceContract]
    public interface ISensorReadingSoapService
    {
        [OperationContract]
        Task<SoapResponse<SensorReadingDto>> GetReadingByIdAsync(Guid id);

        [OperationContract]
        Task<SoapResponse<List<SensorReadingDto>>> GetReadingsBySensorIdAsync(Guid sensorId, int limit);

        [OperationContract]
        Task<SoapResponse<SensorReadingDto>> GetLatestReadingAsync(Guid sensorId);

        [OperationContract]
        Task<SoapResponse<SensorReadingDto>> CreateReadingAsync(CreateSensorReadingRequest request);


    }
}
