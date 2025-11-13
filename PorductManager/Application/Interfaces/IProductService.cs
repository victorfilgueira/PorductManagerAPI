using PorductManager.Application.DTOs;

namespace PorductManager.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
    Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto updateProductDto);
    Task<bool> DeleteAsync(Guid id);
}

