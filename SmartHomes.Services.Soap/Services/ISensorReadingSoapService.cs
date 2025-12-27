using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Entities;
using SmartHomes.Services.Soap.Models;
using System.ServiceModel;

namespace SmartHomes.Services.Soap.Services
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
