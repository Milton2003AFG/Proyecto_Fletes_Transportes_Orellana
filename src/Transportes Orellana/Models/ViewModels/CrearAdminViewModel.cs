using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Transportes_Orellana.Models.ViewModels;

public class CrearAdminViewModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un formato de correo válido")]
    [Display(Name = "Correro Electrónico")]
    public string Email {get; set;} = string.Empty;

    [Required(ErrorMessage = "La contraseña temporal es requerida")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 12)]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña Temporal")]
    public string Password {get; set;} = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coindicen")]
    public string ConfirmPassword {get; set;} = string.Empty;

    [Phone(ErrorMessage = "Ingrese un número de teléfono válido")]
    [Display(Name = "Teléfono de Contacto")]
    public string? Telefono {get; set;}
}