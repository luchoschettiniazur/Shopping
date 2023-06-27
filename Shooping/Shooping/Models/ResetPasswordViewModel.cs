using System.ComponentModel.DataAnnotations;

namespace Shooping.Models;

public class ResetPasswordViewModel
{

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Debes ingresar un correo válido.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string UserName { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
    public string Password { get; set; } = null!;

    [Compare("Password", ErrorMessage = "La nueva contraseña y la confirmación no son iguales.")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Token { get; set; } = null!;


}
