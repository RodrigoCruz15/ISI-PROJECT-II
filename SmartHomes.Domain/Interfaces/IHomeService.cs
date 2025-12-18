using SmartHomes.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.Interfaces
{
    public interface IHomeService
    {
        /// <summary>
        /// Obtém uma casa pelo seu identificador
        /// </summary>
        /// <param name="id">Identificador único da casa</param>
        /// <returns>Dados da casa ou null se não existir</returns>
        Task<HomeDto?> GetHomeByIdAsync(Guid id);

        /// <summary>
        /// Obtém todas as casas registadas no sistema
        /// </summary>
        /// <returns>Lista de casas</returns>
        Task<IEnumerable<HomeDto>> GetAllHomesAsync();

        /// <summary>
        /// Cria uma nova casa no sistema
        /// </summary>
        /// <param name="request">Dados da casa a criar</param>
        /// <returns>Casa criada com ID gerado</returns>
        Task<HomeDto> CreateHomeAsync(CreateHomeRequest request);

        /// <summary>
        /// Atualiza os dados de uma casa existente
        /// </summary>
        /// <param name="id">Identificador da casa a atualizar</param>
        /// <param name="request">Novos dados da casa</param>
        /// <returns>True se atualizada com sucesso, False caso contrário</returns>
        Task<bool> UpdateHomeAsync(Guid id, UpdateHomeRequest request);

        /// <summary>
        /// Remove uma casa do sistema
        /// </summary>
        /// <param name="id">Identificador da casa a remover</param>
        /// <returns>True se removida com sucesso, False caso contrário</returns>
        Task<bool> DeleteHomeAsync(Guid id);

    }
}
