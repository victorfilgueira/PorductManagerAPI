using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PorductManager.Domain.Entities;
using PorductManager.Domain.Interfaces;
using PorductManager.Infrastructure.Data;

namespace PorductManager.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ApplicationDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Buscando produto com ID: {ProductId}", id);
            var product = await _context.Products.FindAsync(id);
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto com ID: {ProductId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Buscando todos os produtos");
            var products = await _context.Products.ToListAsync();
            _logger.LogInformation("Encontrados {Count} produtos", products.Count);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os produtos");
            throw;
        }
    }

    public async Task<Product> CreateAsync(Product product)
    {
        try
        {
            _logger.LogInformation("Criando novo produto: {ProductName}", product.Name);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Produto criado com sucesso. ID: {ProductId}", product.Id);
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto: {ProductName}", product.Name);
            throw;
        }
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        try
        {
            _logger.LogInformation("Atualizando produto com ID: {ProductId}", product.Id);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Produto atualizado com sucesso. ID: {ProductId}", product.Id);
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto com ID: {ProductId}", product.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Deletando produto com ID: {ProductId}", id);
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Produto não encontrado para deletar. ID: {ProductId}", id);
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Produto deletado com sucesso. ID: {ProductId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar produto com ID: {ProductId}", id);
            throw;
        }
    }
}

