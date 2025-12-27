using System.ServiceModel;
using SmartHomes.Domain.DTO;
using SmartHomes.Services.Soap.Models;

namespace SmartHomes.Services.Soap.Services

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
