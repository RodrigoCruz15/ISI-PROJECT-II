using SmartHomes.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.Interfaces
{
    public interface ISensorService
    {

        Task<SensorDto?> GetSensorByIdAsync(Guid id);
        Task<IEnumerable<SensorDto>> GetAllSensorAsync();
        Task<IEnumerable<SensorDto>> GetSensorsByHomeIdAsync(Guid homeId);
        Task<IEnumerable<SensorDto>> GetActiveSensorsByHomeIdAsync(Guid homeId);
        Task<SensorDto> CreateSensorAsync(CreateSensorRequest request);
        Task<bool> UpdateSensorAsync(Guid id, UpdateSensorRequest request);
        Task<bool> DeleteSensorAsync(Guid id);

    }
}
