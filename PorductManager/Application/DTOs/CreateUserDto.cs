using System.ComponentModel.DataAnnotations;

namespace PorductManager.Application.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 100 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(200, ErrorMessage = "O email não pode exceder 200 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$",
        ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma minúscula e um número")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A role é obrigatória")]
    [RegularExpression("^(Admin|Manager|User)$", ErrorMessage = "A role deve ser Admin, Manager ou User")]
    public string Role { get; set; } = "User";
}

