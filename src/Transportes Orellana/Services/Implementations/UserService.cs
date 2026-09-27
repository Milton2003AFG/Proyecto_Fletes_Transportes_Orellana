using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Services.Implementations;

public class UserService : IUsuarioService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _context ;

    public UserService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
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

    public async Task<(bool Exito, string? Error)>CambiarEstadoUsuarioAsync(string usuarioId, bool bloquear)
    {
        var usuario = await _userManager.FindByIdAsync(usuarioId);
        if(usuario == null)
        {
            return(false, "El usuario no fue encontrado");
        }

        if (!usuario.LockoutEnabled)
        {
            await _userManager.SetLockoutEnabledAsync(usuario, true);
        }
        
        // Sincronizar estado operativo si usuario es de tipo Motorista
        var esMotorista = await _userManager.IsInRoleAsync(usuario,"Motorista");
        if (esMotorista)
        {
            var motorista = await _context.Motoristas
                .FirstOrDefaultAsync(m => m.UsuarioId == usuarioId);
            
            if(motorista != null)
            {
                motorista.Estado = bloquear ? "inactivo" : "activo";
                await _context.SaveChangesAsync();
            }
        }

        DateTimeOffset? fechaBloqueo = bloquear ? DateTimeOffset.UtcNow.AddYears(100) : null;
        var resultado = await _userManager.SetLockoutEndDateAsync(usuario, fechaBloqueo);

        if (!resultado.Succeeded)
        {
            var mensaje = bloquear ? "No se pudo suspender al usuario." : "No se pudo reactivar al usuario.";
            return (false, mensaje);
        }

        return (true, null);
    }

}