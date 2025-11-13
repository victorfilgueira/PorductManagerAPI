using Microsoft.Extensions.Logging;
using PorductManager.Application.DTOs;
using PorductManager.Application.Interfaces;
using PorductManager.Domain.Entities;
using PorductManager.Domain.Interfaces;

namespace PorductManager.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Buscando produto por ID: {ProductId}", id);
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Produto não encontrado. ID: {ProductId}", id);
            }
            return product == null ? null : MapToDto(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto por ID: {ProductId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Buscando todos os produtos");
            var products = await _productRepository.GetAllAsync();
            var productDtos = products.Select(MapToDto).ToList();
            _logger.LogInformation("Retornando {Count} produtos", productDtos.Count);
            return productDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os produtos");
            throw;
        }
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
    {
        try
        {
            _logger.LogInformation("Criando novo produto: {ProductName}", createProductDto.Name);
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                CreatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.CreateAsync(product);
            _logger.LogInformation("Produto criado com sucesso. ID: {ProductId}", createdProduct.Id);
            return MapToDto(createdProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto: {ProductName}", createProductDto.Name);
            throw;
        }
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto updateProductDto)
    {
        try
        {
            _logger.LogInformation("Atualizando produto. ID: {ProductId}", id);
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Produto não encontrado para atualização. ID: {ProductId}", id);
                return null;
            }

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.Price = updateProductDto.Price;
            product.Stock = updateProductDto.Stock;
            product.UpdatedAt = DateTime.UtcNow;

            var updatedProduct = await _productRepository.UpdateAsync(product);
            _logger.LogInformation("Produto atualizado com sucesso. ID: {ProductId}", id);
            return MapToDto(updatedProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto. ID: {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Deletando produto. ID: {ProductId}", id);
            var result = await _productRepository.DeleteAsync(id);
            if (!result)
            {
                _logger.LogWarning("Produto não encontrado para deleção. ID: {ProductId}", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar produto. ID: {ProductId}", id);
            throw;
        }
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}

