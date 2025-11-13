using Microsoft.EntityFrameworkCore;
using PorductManager.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace PorductManager.Infrastructure.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                logger.LogInformation("Banco de dados já possui dados. Pulando inicialização.");
                return;
            }

            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
            var managerRole = context.Roles.FirstOrDefault(r => r.Name == "Manager");
            var userRole = context.Roles.FirstOrDefault(r => r.Name == "User");

            if (adminRole == null || managerRole == null || userRole == null)
            {
                logger.LogWarning("Roles não encontradas. Certifique-se de que o seed foi executado.");
                return;
            }

            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@productmanager.com",
                PasswordHash = HashPassword("Admin123!"),
                CreatedAt = DateTime.UtcNow
            };

            var managerUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "manager",
                Email = "manager@productmanager.com",
                PasswordHash = HashPassword("Manager123!"),
                CreatedAt = DateTime.UtcNow
            };

            var regularUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "user",
                Email = "user@productmanager.com",
                PasswordHash = HashPassword("User123!"),
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(adminUser, managerUser, regularUser);
            context.SaveChanges();

            var now = DateTime.UtcNow;
            context.UserRoles.AddRange(
                new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id, AssignedAt = now },
                new UserRole { UserId = managerUser.Id, RoleId = managerRole.Id, AssignedAt = now },
                new UserRole { UserId = regularUser.Id, RoleId = userRole.Id, AssignedAt = now }
            );
            context.SaveChanges();

            logger.LogInformation("Usuários padrão criados com sucesso:");
            logger.LogInformation("Admin - Username: admin, Password: Admin123!");
            logger.LogInformation("Manager - Username: manager, Password: Manager123!");
            logger.LogInformation("User - Username: user, Password: User123!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao inicializar banco de dados");
            throw;
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

