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
        

        /// <summary>
        /// Obtem casas de um utilizador especifico
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Home>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Remove uma casa (verificando ownership)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
