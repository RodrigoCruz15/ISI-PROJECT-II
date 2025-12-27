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
    /// Controller REST para gestão de leituras de sensores
    /// Comunica com o serviço SOAP para todas as operações
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly SensorReadingSoapClient _soapClient;

        public SensorReadingsController(SensorReadingSoapClient soapClient)
        {
            _soapClient = soapClient;
        }

        /// <summary>
        /// Obtém uma leitura pelo ID
        /// </summary>
        /// <param name="id">ID da leitura</param>
        /// <returns>Dados da leitura</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SensorReadingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReadingById(Guid id)
        {
            var response = await _soapClient.GetReadingByIdAsync(id);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtém leituras de um sensor
        /// </summary>
        /// <param name="sensorId">ID do sensor</param>
        /// <param name="limit">Número máximo de leituras (padrão: 100)</param>
        /// <returns>Lista de leituras</returns>
        [HttpGet("sensor/{sensorId}")]
        [ProducesResponseType(typeof(List<SensorReadingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReadingsBySensorId(Guid sensorId, [FromQuery] int limit = 100)
        {
            var response = await _soapClient.GetReadingsBySensorIdAsync(sensorId, limit);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtém a última leitura de um sensor
        /// </summary>
        /// <param name="sensorId">ID do sensor</param>
        /// <returns>Última leitura</returns>
        [HttpGet("sensor/{sensorId}/latest")]
        [ProducesResponseType(typeof(SensorReadingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLatestReading(Guid sensorId)
        {
            var response = await _soapClient.GetLatestReadingAsync(sensorId);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Cria uma nova leitura
        /// </summary>
        /// <param name="request">Dados da leitura a criar</param>
        /// <returns>Leitura criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SensorReadingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateReading([FromBody] CreateSensorReadingRequest request)
        {
            var response = await _soapClient.CreateReadingAsync(request);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return CreatedAtAction(
                nameof(GetReadingById),
                new { id = response.Data!.Id },
                response.Data
            );
        }
    }
}