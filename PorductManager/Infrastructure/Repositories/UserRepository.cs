using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PorductManager.Domain.Entities;
using PorductManager.Domain.Interfaces;
using PorductManager.Infrastructure.Data;

namespace PorductManager.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Buscando usuário com ID: {UserId}", id);
            var user = await _context.Users.FindAsync(id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário com ID: {UserId}", id);
            throw;
        }
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        try
        {
            _logger.LogInformation("Buscando usuário com username: {Username}", username);
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário com username: {Username}", username);
            throw;
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            _logger.LogInformation("Buscando usuário com email: {Email}", email);
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário com email: {Email}", email);
            throw;
        }
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Buscando usuário com ID e roles: {UserId}", id);
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário com ID e roles: {UserId}", id);
            throw;
        }
    }

    public async Task<User?> GetByUsernameWithRolesAsync(string username)
    {
        try
        {
            _logger.LogInformation("Buscando usuário com username e roles: {Username}", username);
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário com username e roles: {Username}", username);
            throw;
        }
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        try
        {
            _logger.LogInformation("Verificando existência de usuário com username: {Username}", username);
            var exists = await _context.Users.AnyAsync(u => u.Username == username);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência de usuário com username: {Username}", username);
            throw;
        }
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        try
        {
            _logger.LogInformation("Verificando existência de usuário com email: {Email}", email);
            var exists = await _context.Users.AnyAsync(u => u.Email == email);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência de usuário com email: {Email}", email);
            throw;
        }
    }

    public async Task<User> CreateAsync(User user)
    {
        try
        {
            _logger.LogInformation("Criando novo usuário: {Username}", user.Username);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Usuário criado com sucesso. ID: {UserId}", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário: {Username}", user.Username);
            throw;
        }
    }

    public async Task<User> UpdateAsync(User user)
    {
        try
        {
            _logger.LogInformation("Atualizando usuário com ID: {UserId}", user.Id);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Usuário atualizado com sucesso. ID: {UserId}", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário com ID: {UserId}", user.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Deletando usuário com ID: {UserId}", id);
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado para deletar. ID: {UserId}", id);
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Usuário deletado com sucesso. ID: {UserId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar usuário com ID: {UserId}", id);
            throw;
        }
    }

    public async Task AddRoleToUserAsync(Guid userId, Guid roleId)
    {
        try
        {
            _logger.LogInformation("Adicionando role {RoleId} ao usuário {UserId}", roleId, userId);
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Role adicionada com sucesso ao usuário {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar role ao usuário {UserId}", userId);
            throw;
        }
    }
}

