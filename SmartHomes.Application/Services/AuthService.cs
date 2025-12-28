using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SmartHomes.Domain.DTO;
using SmartHomes.Domain.Entities;
using SmartHomes.Domain.Interfaces;

namespace SmartHomes.Application.Services;

/// <summary>
/// Implementacao da logica de autenticacao e autorizacao
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly string _jwtSecret;
    private readonly int _jwtExpirationMinutes;

    public AuthService(IUserRepository userRepository, string jwtSecret, int jwtExpirationMinutes = 60)
    {
        _userRepository = userRepository;
        _jwtSecret = jwtSecret;
        _jwtExpirationMinutes = jwtExpirationMinutes;
    }

    /// <summary>
    /// Regista um novo utilizador
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Validacoes
        ValidateRegisterRequest(request);

        // Verificar se email ja existe
        if (await _userRepository.EmailExistsAsync(request.Email))
            throw new InvalidOperationException("Email já está registado");

        // Hash da password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Criar utilizador
        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLower(),
            PasswordHash = passwordHash,
            IsActive = true
        };

        var createdUser = await _userRepository.CreateAsync(user);

        // Gerar JWT token
        var token = GenerateJwtToken(createdUser);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes);

        return new AuthResponse
        {
            UserId = createdUser.Id,
            FullName = createdUser.FullName,
            Email = createdUser.Email,
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Autentica um utilizador
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        // Obter utilizador pelo email
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            return null;

        // Verificar password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        // Verificar se conta esta ativa
        if (!user.IsActive)
            throw new InvalidOperationException("Conta desativada");

        // Gerar JWT token
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes);

        return new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Gera um JWT token para o utilizador
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Valida o pedido de registo
    /// </summary>
    /// <param name="request"></param>
    private static void ValidateRegisterRequest(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ArgumentException("Nome completo é obrigatório");

        if (request.FullName.Length < 3)
            throw new ArgumentException("Nome deve ter pelo menos 3 caracteres");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email é obrigatório");

        if (!request.Email.Contains("@"))
            throw new ArgumentException("Email inválido");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password é obrigatória");

        if (request.Password.Length < 6)
            throw new ArgumentException("Password deve ter pelo menos 6 caracteres");
    }
}