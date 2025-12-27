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
    /// Controller REST para gestao de regras de alerta
    /// Comunica com o servico SOAP para todas as operacoes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AlertRulesController : ControllerBase
    {
        private readonly AlertRuleSoapClient _soapClient;

        public AlertRulesController(AlertRuleSoapClient soapClient)
        {
            _soapClient = soapClient;
        }

        /// <summary>
        /// Obtem todas as regras de alerta
        /// </summary>
        /// <returns>Lista de regras</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<AlertRuleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAlertRules()
        {
            var response = await _soapClient.GetAllAlertRulesAsync();

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtem uma regra de alerta pelo ID
        /// </summary>
        /// <param name="id">ID da regra</param>
        /// <returns>Dados da regra</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AlertRuleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAlertRuleById(Guid id)
        {
            var response = await _soapClient.GetAlertRuleByIdAsync(id);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Obtem regras de alerta de um sensor especifico
        /// </summary>
        /// <param name="sensorId">ID do sensor</param>
        /// <returns>Lista de regras do sensor</returns>
        [HttpGet("sensor/{sensorId}")]
        [ProducesResponseType(typeof(List<AlertRuleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAlertRulesBySensorId(Guid sensorId)
        {
            var response = await _soapClient.GetAlertRulesBySensorIdAsync(sensorId);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(response.Data);
        }

        /// <summary>
        /// Cria uma nova regra de alerta
        /// </summary>
        /// <param name="request">Dados da regra a criar</param>
        /// <returns>Regra criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AlertRuleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAlertRule([FromBody] CreateAlertRuleRequest request)
        {
            var response = await _soapClient.CreateAlertRuleAsync(request);

            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return CreatedAtAction(
                nameof(GetAlertRuleById),
                new { id = response.Data!.Id },
                response.Data
            );
        }

        /// <summary>
        /// Atualiza uma regra de alerta existente
        /// </summary>
        /// <param name="id">ID da regra a atualizar</param>
        /// <param name="request">Novos dados da regra</param>
        /// <returns>Resultado da operacao</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAlertRule(Guid id, [FromBody] UpdateAlertRuleRequest request)
        {
            var response = await _soapClient.UpdateAlertRuleAsync(id, request);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return NoContent();
        }

        /// <summary>
        /// Remove uma regra de alerta
        /// </summary>
        /// <param name="id">ID da regra a remover</param>
        /// <returns>Resultado da operacao</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAlertRule(Guid id)
        {
            var response = await _soapClient.DeleteAlertRuleAsync(id);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return NoContent();
        }
    }
}