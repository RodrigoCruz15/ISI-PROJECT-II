using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHomes.API.Rest.Clients;
using SmartHomes.Domain.DTO;

namespace SmartHomes.API.Rest.Controllers
{
    /// <summary>
    /// Controller REST para gestão de sensores
    /// Comunica com o serviço SOAP para todas as operações
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SensorsController : ControllerBase
    {
        private readonly SensorSoapClient _soapClient;

        public SensorsController(SensorSoapClient soapClient)
        {
            _soapClient = soapClient;
        }

        /// <summary>
        /// Obtém todos os sensores
        /// </summary>
        /// <returns>Lista de sensores</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<SensorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSensors()
        {
            var response = await _soapClient.GetAllSensorsAsync();

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtém um sensor pelo ID
        /// </summary>
        /// <param name="id">ID do sensor</param>
        /// <returns>Dados do sensor</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SensorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSensorById(Guid id)
        {
            var response = await _soapClient.GetSensorByIdAsync(id);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtém sensores de uma casa específica
        /// </summary>
        /// <param name="homeId">ID da casa</param>
        /// <returns>Lista de sensores da casa</returns>
        [HttpGet("home/{homeId}")]
        [ProducesResponseType(typeof(List<SensorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSensorsByHomeId(Guid homeId)
        {
            var response = await _soapClient.GetSensorsByHomeIdAsync(homeId);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtém sensores ativos de uma casa
        /// </summary>
        /// <param name="homeId">ID da casa</param>
        /// <returns>Lista de sensores ativos</returns>
        [HttpGet("home/{homeId}/active")]
        [ProducesResponseType(typeof(List<SensorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveSensorsByHomeId(Guid homeId)
        {
            var response = await _soapClient.GetActiveSensorsByHomeIdAsync(homeId);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Cria um novo sensor
        /// </summary>
        /// <param name="request">Dados do sensor a criar</param>
        /// <returns>Sensor criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SensorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSensor([FromBody] CreateSensorRequest request)
        {
            var response = await _soapClient.CreateSensorAsync(request);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return CreatedAtAction(
                nameof(GetSensorById),
                new { id = response.Data!.Id },
                response.Data
            );
        }

        /// <summary>
        /// Atualiza um sensor existente
        /// </summary>
        /// <param name="id">ID do sensor a atualizar</param>
        /// <param name="request">Novos dados do sensor</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSensor(Guid id, [FromBody] UpdateSensorRequest request)
        {
            var response = await _soapClient.UpdateSensorAsync(id, request);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return NoContent();
        }

        /// <summary>
        /// Remove um sensor
        /// </summary>
        /// <param name="id">ID do sensor a remover</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSensor(Guid id)
        {
            var response = await _soapClient.DeleteSensorAsync(id);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return NoContent();
        }
    }
}