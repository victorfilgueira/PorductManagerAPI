using System.ComponentModel.DataAnnotations;

namespace PorductManager.Application.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 100 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;
}

