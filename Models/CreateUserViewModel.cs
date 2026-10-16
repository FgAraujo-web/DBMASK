
using System.ComponentModel.DataAnnotations;

namespace DBMASK.Web.Models;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Introduza um email válido.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
    [DataType(DataType.Password)]
    [MinLength(12, ErrorMessage = "A palavra-passe deve ter pelo menos 12 caracteres.")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Selecione um perfil.")]
    public string Role { get; set; } = "";
}
