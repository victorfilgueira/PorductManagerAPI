using System.ComponentModel.DataAnnotations;

namespace PorductManager.Application.DTOs;

public class UpdateProductDto
{
    [Required(ErrorMessage = "O nome do produto é obrigatório")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "O preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "O estoque é obrigatório")]
    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo")]
    public int Stock { get; set; }
}

