using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHomes.API.Rest.Clients;
using SmartHomes.Domain.DTO;

namespace SmartHomes.API.Rest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomesController : ControllerBase
    {
        private readonly HomeSoapClient _soapClient;

        public HomesController(HomeSoapClient soapClient)
        {
            _soapClient = soapClient;
        }

        /// <summary>
        /// Obtém todas as casas
        /// </summary>
        /// <returns>Lista de casas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<HomeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllHomes()
        {
            var response = await _soapClient.GetAllHomesAsync();

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtém uma casa pelo ID
        /// </summary>
        /// <param name="id">ID da casa</param>
        /// <returns>Dados da casa</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HomeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHomeById(Guid id)
        {
            var response = await _soapClient.GetHomeByIdAsync(id);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Cria uma nova casa
        /// </summary>
        /// <param name="request">Dados da casa a criar</param>
        /// <returns>Casa criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(HomeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateHome([FromBody] CreateHomeRequest request)
        {
            var response = await _soapClient.CreateHomeAsync(request);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return CreatedAtAction(
                nameof(GetHomeById),
                new { id = response.Data!.Id },
                response.Data
            );
        }

        /// <summary>
        /// Atualiza uma casa existente
        /// </summary>
        /// <param name="id">ID da casa a atualizar</param>
        /// <param name="request">Novos dados da casa</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateHome(Guid id, [FromBody] UpdateHomeRequest request)
        {
            var response = await _soapClient.UpdateHomeAsync(id, request);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return NoContent();
        }

        /// <summary>
        /// Remove uma casa
        /// </summary>
        /// <param name="id">ID da casa a remover</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteHome(Guid id)
        {
            var response = await _soapClient.DeleteHomeAsync(id);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return NoContent();
        }
    }
}
