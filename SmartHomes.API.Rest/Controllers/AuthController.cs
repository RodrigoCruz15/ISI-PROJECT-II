using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHomes.API.Rest.Clients;
using SmartHomes.Domain.DTO;

namespace SmartHomes.API.Rest.Controllers;

/// <summary>
/// Controller para autenticacao e registo de utilizadores
/// Comunica com o servico SOAP
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthSoapClient _authSoapClient;

    public AuthController(AuthSoapClient authSoapClient)
    {
        _authSoapClient = authSoapClient;
    }

    /// <summary>
    /// Regista um novo utilizador
    /// </summary>
    /// <param name="request">Dados de registo</param>
    /// <returns>Utilizador criado com JWT token</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _authSoapClient.RegisterAsync(request);

        if (!response.Success)
            return BadRequest(new { message = response.Message });

        return CreatedAtAction(nameof(Register), response.Data);
    }

    /// <summary>
    /// Autentica um utilizador
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Dados do utilizador com JWT token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authSoapClient.LoginAsync(request);

        if (!response.Success)
            return Unauthorized(new { message = response.Message });

        return Ok(response.Data);
    }

    /// <summary>
    /// Endpoint de teste para verificar autenticacao
    /// Requer token JWT valido
    /// </summary>
    /// <returns>Dados do utilizador autenticado</returns>
    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        return Ok(new
        {
            userId,
            email,
            name,
            message = "Autenticado com sucesso!"
        });
    }
}