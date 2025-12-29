using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHomes.API.Rest.Clients;
using SmartHomes.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHomes.API.Rest.Controllers
{
    /// <summary>
    /// Controller REST para gestao de alertas disparados
    /// Comunica com o servico SOAP para todas as operacoes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class AlertsController : ControllerBase
    {
        private readonly AlertSoapClient _soapClient;

        public AlertsController(AlertSoapClient soapClient)
        {
            _soapClient = soapClient;
        }

        /// <summary>
        /// Obtem um alerta pelo ID
        /// </summary>
        /// <param name="id">ID do alerta</param>
        /// <returns>Dados do alerta</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AlertDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAlertById(Guid id)
        {
            var response = await _soapClient.GetAlertByIdAsync(id);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtem alertas de um sensor
        /// </summary>
        /// <param name="sensorId">ID do sensor</param>
        /// <param name="limit">Numero maximo de alertas (padrao: 100)</param>
        /// <returns>Lista de alertas</returns>
        [HttpGet("sensor/{sensorId}")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAlertsBySensorId(Guid sensorId, [FromQuery] int limit = 100)
        {
            var response = await _soapClient.GetAlertsBySensorIdAsync(sensorId, limit);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtem alertas de uma casa
        /// </summary>
        /// <param name="homeId">ID da casa</param>
        /// <param name="limit">Numero maximo de alertas (padrao: 100)</param>
        /// <returns>Lista de alertas</returns>
        [HttpGet("home/{homeId}")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAlertsByHomeId(Guid homeId, [FromQuery] int limit = 100)
        {
            var response = await _soapClient.GetAlertsByHomeIdAsync(homeId, limit);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtem alertas nao reconhecidos (pendentes)
        /// </summary>
        /// <param name="homeId">ID da casa (opcional)</param>
        /// <returns>Lista de alertas nao reconhecidos</returns>
        [HttpGet("unacknowledged")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnacknowledgedAlerts([FromQuery] Guid? homeId = null)
        {
            var response = await _soapClient.GetUnacknowledgedAlertsAsync(homeId);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Marca um alerta como reconhecido/lido
        /// </summary>
        /// <param name="id">ID do alerta</param>
        /// <returns>Resultado da operacao</returns>
        [HttpPatch("{id}/acknowledge")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AcknowledgeAlert(Guid id)
        {
            var response = await _soapClient.AcknowledgeAlertAsync(id);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return NoContent();
        }
    }
}