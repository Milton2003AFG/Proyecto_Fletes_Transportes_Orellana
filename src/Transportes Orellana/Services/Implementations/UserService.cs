using Microsoft.AspNetCore.Identity;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Services.Implementations;

public class UserService : IUsuarioService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<(bool Exito, string? Error, string? UsuarioId)> RegistrarUsuarioAsync(
        string email, 
        string password, 
        string rol, 
        string? telefono = null)
    {
        var usuarioExiste = await _userManager.FindByEmailAsync(email);
        if(usuarioExiste != null)
        {   
            return(false, "El correo ya se encuentra registrado", null);
        }

        if(!await _roleManager.RoleExistsAsync(rol))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(rol));
            if (!roleResult.Succeeded)
            {
                return(false, "No se pudo inicializar el rol del usuario", null);
            }
        }

        var nuevoUsuario = new IdentityUser
        {
            UserName = email,
            Email = email,
            PhoneNumber = telefono,
            EmailConfirmed = true
        };

        var resultadoCreacion = await _userManager.CreateAsync(nuevoUsuario, password);
        if (!resultadoCreacion.Succeeded)
        {
            var error = resultadoCreacion.Errors.FirstOrDefault()?.Description ?? "Error al registrar credenciales";
            return(false, error, null);
        }

        var resultado = await _userManager.AddToRoleAsync(nuevoUsuario, rol);
        if (!resultado.Succeeded)
        {
            await _userManager.DeleteAsync(nuevoUsuario);
            return(false, "Error al asociar el rol al usuario", null);
        }

        return(true, null, nuevoUsuario.Id);
    }
}