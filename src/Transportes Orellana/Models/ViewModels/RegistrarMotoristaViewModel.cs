using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Transportes_Orellana.Models.ViewModels;

public class RegistrarMotoristaViewModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [Display(Name = "Correo Electrónico (Usuario)")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña inicial es requerida.")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 12)]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña Temporal")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    // --- Datos de Perfil Operativo (Tabla motorista) ---
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección de residencia es obligatoria.")]
    [StringLength(500)]
    [Display(Name = "Dirección de Residencia")]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [Phone(ErrorMessage = "Ingrese un número telefónico válido.")]
    [StringLength(15)]
    [Display(Name = "Teléfono de Contacto")]
    public string Telefono { get; set; } = string.Empty;

    [Url(ErrorMessage = "Ingrese una URL válida (ej. https://linkedin.com/in/...).")]
    [StringLength(2048)]
    [Display(Name = "Perfil Social (LinkedIn o Facebook)")]
    public string? PerfilSocial { get; set; }

    [Url(ErrorMessage = "Ingrese una URL válida (ej. https://linkedin.com/in/...).")]
    [StringLength(2048)]
    [Display(Name = "Currículum Vitae (Enlace)")]
    public string? Curriculum { get; set; }
}