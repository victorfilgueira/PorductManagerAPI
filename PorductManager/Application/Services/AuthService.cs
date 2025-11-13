using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using PorductManager.Application.DTOs;
using PorductManager.Application.Interfaces;
using PorductManager.Domain.Entities;
using PorductManager.Domain.Interfaces;

namespace PorductManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("Tentativa de login para o usuário: {Username}", loginDto.Username);

            var user = await _userRepository.GetByUsernameWithRolesAsync(loginDto.Username);

            if (user == null)
            {
                _logger.LogWarning("Tentativa de login com usuário inexistente: {Username}", loginDto.Username);
                return null;
            }

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Senha incorreta para o usuário: {Username}", loginDto.Username);
                return null;
            }

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("Login bem-sucedido para o usuário: {Username}", loginDto.Username);

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var primaryRole = roles.FirstOrDefault() ?? "User";

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = primaryRole,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar login para o usuário: {Username}", loginDto.Username);
            throw;
        }
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            _logger.LogInformation("Tentativa de registro para o usuário: {Username}", registerDto.Username);

            if (await _userRepository.ExistsByUsernameAsync(registerDto.Username))
            {
                _logger.LogWarning("Tentativa de registro com username já existente: {Username}", registerDto.Username);
                return null;
            }

            if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
            {
                _logger.LogWarning("Tentativa de registro com email já existente: {Email}", registerDto.Email);
                return null;
            }

            var defaultRole = await _roleRepository.GetByNameAsync("User");
            if (defaultRole == null)
            {
                _logger.LogError("Role 'User' não encontrada no banco de dados");
                throw new InvalidOperationException("Role padrão 'User' não encontrada");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.AddRoleToUserAsync(user.Id, defaultRole.Id);

            var userWithRoles = await _userRepository.GetByIdWithRolesAsync(user.Id);
            if (userWithRoles == null)
            {
                _logger.LogError("Erro ao carregar usuário com roles após criação");
                throw new InvalidOperationException("Erro ao carregar usuário criado");
            }

            _logger.LogInformation("Usuário registrado com sucesso: {Username}", registerDto.Username);

            var token = _jwtService.GenerateToken(userWithRoles);

            var roles = userWithRoles.UserRoles.Select(ur => ur.Role.Name).ToList();
            var primaryRole = roles.FirstOrDefault() ?? "User";

            return new AuthResponseDto
            {
                Token = token,
                Username = userWithRoles.Username,
                Email = userWithRoles.Email,
                Role = primaryRole,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar usuário: {Username}", registerDto.Username);
            throw;
        }
    }

    public async Task<AuthResponseDto?> CreateUserAsync(CreateUserDto createUserDto)
    {
        try
        {
            _logger.LogInformation("Tentativa de criação de usuário por admin: {Username} com role: {Role}", 
                createUserDto.Username, createUserDto.Role);

            if (await _userRepository.ExistsByUsernameAsync(createUserDto.Username))
            {
                _logger.LogWarning("Tentativa de criar usuário com username já existente: {Username}", createUserDto.Username);
                return null;
            }

            if (await _userRepository.ExistsByEmailAsync(createUserDto.Email))
            {
                _logger.LogWarning("Tentativa de criar usuário com email já existente: {Email}", createUserDto.Email);
                return null;
            }

            var role = await _roleRepository.GetByNameAsync(createUserDto.Role);
            if (role == null)
            {
                _logger.LogError("Role '{Role}' não encontrada no banco de dados", createUserDto.Role);
                return null;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = HashPassword(createUserDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.AddRoleToUserAsync(user.Id, role.Id);

            var userWithRoles = await _userRepository.GetByIdWithRolesAsync(user.Id);
            if (userWithRoles == null)
            {
                _logger.LogError("Erro ao carregar usuário com roles após criação");
                throw new InvalidOperationException("Erro ao carregar usuário criado");
            }

            _logger.LogInformation("Usuário criado com sucesso por admin: {Username} com role: {Role}", 
                createUserDto.Username, createUserDto.Role);

            var token = _jwtService.GenerateToken(userWithRoles);

            var roles = userWithRoles.UserRoles.Select(ur => ur.Role.Name).ToList();
            var primaryRole = roles.FirstOrDefault() ?? "User";

            return new AuthResponseDto
            {
                Token = token,
                Username = userWithRoles.Username,
                Email = userWithRoles.Email,
                Role = primaryRole,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário: {Username}", createUserDto.Username);
            throw;
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == passwordHash;
    }
}

