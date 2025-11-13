using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PorductManager.Application.DTOs;
using PorductManager.Application.Interfaces;

namespace PorductManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            _logger.LogInformation("Tentativa de registro para o usuário: {Username}", registerDto.Username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dados de registro inválidos para o usuário: {Username}", registerDto.Username);
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);
            if (result == null)
            {
                _logger.LogWarning("Falha no registro. Username ou email já existe: {Username}", registerDto.Username);
                return BadRequest(new { message = "Username ou email já está em uso" });
            }

            _logger.LogInformation("Registro bem-sucedido para o usuário: {Username}", registerDto.Username);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar registro para o usuário: {Username}", registerDto.Username);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("Tentativa de login para o usuário: {Username}", loginDto.Username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dados de login inválidos para o usuário: {Username}", loginDto.Username);
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
            {
                _logger.LogWarning("Login falhou para o usuário: {Username}", loginDto.Username);
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            _logger.LogInformation("Login bem-sucedido para o usuário: {Username}", loginDto.Username);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login para o usuário: {Username}", loginDto.Username);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpPost("create-user")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthResponseDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            _logger.LogInformation("Tentativa de criação de usuário por admin: {Username} com role: {Role}", 
                createUserDto.Username, createUserDto.Role);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dados de criação de usuário inválidos");
                return BadRequest(ModelState);
            }

            var result = await _authService.CreateUserAsync(createUserDto);
            if (result == null)
            {
                _logger.LogWarning("Falha na criação de usuário. Username/email já existe ou role inválida: {Username}", 
                    createUserDto.Username);
                return BadRequest(new { message = "Username ou email já está em uso, ou role inválida" });
            }

            _logger.LogInformation("Usuário criado com sucesso por admin: {Username} com role: {Role}", 
                createUserDto.Username, createUserDto.Role);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário: {Username}", createUserDto.Username);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
}

