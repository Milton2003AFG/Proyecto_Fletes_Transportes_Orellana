using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Models;
using Transportes_Orellana.Models.ViewModels;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Services.Implementations;

public class MotoristaService : IMotoristaService
{   
    private readonly AppDbContext _context;
    private readonly IUsuarioService _usuarioService;
    private readonly IWebHostEnvironment _env;

    public MotoristaService(
        AppDbContext context,
        IUsuarioService usuarioService,
        IWebHostEnvironment env
    )
    {
        _context = context;
        _usuarioService = usuarioService;
        _env = env;
    }

    public async Task<(bool Exito, string? Error)>RegistrarMotoristaAsync(RegistrarMotoristaViewModel model)
    {
        var (exitoUser, errorUser, userId) = await _usuarioService.RegistrarUsuarioAsync(
            model.Correo,
            model.Password,
            "Motorista",
            model.Telefono
        );

        if (!exitoUser)
        {
            return (false, errorUser);
        }

        var motorista = new Motorista
        {
            UsuarioId = userId,
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            Direccion = model.Direccion,
            Correo = model.Correo,
            Telefono = model.Telefono,
            PerfilSocial = model.PerfilSocial,
            Curriculum = model.Curriculum,
            Estado = "activo",
            FechaRegistro = DateTime.UtcNow
        };

        try
        {
            _context.Add(motorista);
            await _context.SaveChangesAsync();
            return(true, null); 
        }catch (Exception ex)
        {
            return(false, $"Error al guardar el motorista en la base de datos: {ex.Message}");
        }
    }
}