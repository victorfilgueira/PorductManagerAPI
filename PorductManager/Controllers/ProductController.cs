using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PorductManager.Application.DTOs;
using PorductManager.Application.Interfaces;

namespace PorductManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        try
        {
            _logger.LogInformation("Requisição GET para produto com ID: {ProductId}", id);
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Produto não encontrado. ID: {ProductId}", id);
                return NotFound(new { message = "Produto não encontrado" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto com ID: {ProductId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        try
        {
            _logger.LogInformation("Requisição GET para listar todos os produtos");
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar produtos");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto createProductDto)
    {
        try
        {
            _logger.LogInformation("Requisição POST para criar produto: {ProductName}", createProductDto.Name);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dados inválidos para criar produto");
                return BadRequest(ModelState);
            }

            var product = await _productService.CreateAsync(createProductDto);
            _logger.LogInformation("Produto criado com sucesso. ID: {ProductId}", product.Id);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto: {ProductName}", createProductDto.Name);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto updateProductDto)
    {
        try
        {
            _logger.LogInformation("Requisição PUT para atualizar produto. ID: {ProductId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dados inválidos para atualizar produto. ID: {ProductId}", id);
                return BadRequest(ModelState);
            }

            var product = await _productService.UpdateAsync(id, updateProductDto);
            if (product == null)
            {
                _logger.LogWarning("Produto não encontrado para atualização. ID: {ProductId}", id);
                return NotFound(new { message = "Produto não encontrado" });
            }

            _logger.LogInformation("Produto atualizado com sucesso. ID: {ProductId}", id);
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto. ID: {ProductId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("Requisição DELETE para produto. ID: {ProductId}", id);
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Produto não encontrado para deleção. ID: {ProductId}", id);
                return NotFound(new { message = "Produto não encontrado" });
            }

            _logger.LogInformation("Produto deletado com sucesso. ID: {ProductId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar produto. ID: {ProductId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
}

