using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PorductManager.Domain.Entities;
using PorductManager.Domain.Interfaces;
using PorductManager.Infrastructure.Data;

namespace PorductManager.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoleRepository> _logger;

    public RoleRepository(ApplicationDbContext context, ILogger<RoleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Buscando role com ID: {RoleId}", id);
            var role = await _context.Roles.FindAsync(id);
            return role;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar role com ID: {RoleId}", id);
            throw;
        }
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        try
        {
            _logger.LogInformation("Buscando role com nome: {RoleName}", name);
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name);
            return role;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar role com nome: {RoleName}", name);
            throw;
        }
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Buscando todas as roles");
            var roles = await _context.Roles.ToListAsync();
            _logger.LogInformation("Encontradas {Count} roles", roles.Count);
            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todas as roles");
            throw;
        }
    }
}

