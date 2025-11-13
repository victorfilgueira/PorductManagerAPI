using PorductManager.Application.DTOs;

namespace PorductManager.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto?> CreateUserAsync(CreateUserDto createUserDto);
}

