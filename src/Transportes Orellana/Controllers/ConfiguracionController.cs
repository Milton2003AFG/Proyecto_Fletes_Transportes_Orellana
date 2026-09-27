using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Transportes_Orellana.Models.ViewModels;
using Transportes_Orellana.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Transportes_Orellana.Controllers;

[Authorize(Roles = "Admin")]
public class ConfiguracionController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly UserManager<IdentityUser> _userManager;

    public ConfiguracionController(IUsuarioService usuarioService, UserManager<IdentityUser> userManager)
    {
        _usuarioService = usuarioService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuariosIdentity = await _userManager.Users.ToListAsync();
        var listaUsuarios = new List<UsuarioConfiguracionViewModel>();

        foreach(var u in usuariosIdentity)
        {
            var roles = await _userManager.GetRolesAsync(u);
            var estaBloqueado = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow;

            listaUsuarios.Add(new UsuarioConfiguracionViewModel
            {
                Id = u.Id,
                Email = u.Email ?? u.UserName ?? "Sin correo",
                Telefono = u.PhoneNumber ?? "No registrado",
                Rol = roles.FirstOrDefault() ?? "Sin rol",
                EmailConfirmado = u.EmailConfirmed,
                EstaBloqueado = estaBloqueado
            });
        }
        return View(listaUsuarios);

    }

    [HttpGet]
    public IActionResult CrearAdmin()
    {
        return View(new CrearAdminViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearAdmin(CrearAdminViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (exito, error, _) = await _usuarioService.RegistrarUsuarioAsync(
            model.Email,
            model.Password,
            "Admin",
            model.Telefono
        );

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al procesar el registro");
            return View(model);
        }

        TempData["SuccessMessage"] = $"El administrador '{model.Email}' fue registrado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(string id, bool bloquear)
    {
        var usuarioActualId = _userManager.GetUserId(User);
        if(id == usuarioActualId && bloquear)
        {
            TempData["ErrorMessage"] = "No puedes suspender tu propia cuenta de administrador";
            return RedirectToAction(nameof(Index));
        }

        var (exito, error) = await _usuarioService.CambiarEstadoUsuarioAsync(id, bloquear);
        if (!exito)
        {
            TempData["ErrorMessage"] = error;
        }
        else
        {
            TempData["SuccessMessage"] = bloquear
                ? "Acceso del usuario suspendido correctamente"
                : "Usuario reactivado con éxito";
        }

        return RedirectToAction(nameof(Index));
    }
}