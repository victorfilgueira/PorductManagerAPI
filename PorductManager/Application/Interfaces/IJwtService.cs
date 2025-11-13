using PorductManager.Domain.Entities;

namespace PorductManager.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}

