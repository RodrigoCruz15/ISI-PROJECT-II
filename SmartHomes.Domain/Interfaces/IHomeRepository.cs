using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHomes.Domain.Entities;

namespace SmartHomes.Domain.Interfaces
{
    public interface IHomeRepository
    {
        Task<Home?> GetByIdAsync(Guid id);
        Task<IEnumerable<Home>> GetAllAsync();
        Task<Home> CreateAsync(Home home);
        Task<bool> UpdateAsync(Guid id, Home home);
        Task<bool> DeleteAsync(Guid id);
    }
}
