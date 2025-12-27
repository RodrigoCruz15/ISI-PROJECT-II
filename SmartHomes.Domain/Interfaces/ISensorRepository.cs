using SmartHomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.Interfaces
{
    public interface ISensorRepository
    {
        
        Task<Sensor?> GetByIdAsync(Guid id);
        Task<IEnumerable<Sensor>> GetAllAsync();
        Task<IEnumerable<Sensor>> GetByHomeAsync(Guid homeId);
        Task<IEnumerable<Sensor>> GetActiveByHomeIdAsync(Guid homeId);
        Task<Sensor> CreateAsync(Sensor sensor);
        Task<bool> UpdateAsync(Guid id, Sensor sensor);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateLastReadingAsync(Guid sensorId, DateTime timestamp);
    }
}
