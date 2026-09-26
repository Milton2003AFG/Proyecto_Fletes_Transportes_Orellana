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

    public MotoristaService(
        AppDbContext context,
        IUsuarioService usuarioService)
    {
        _context = context;
        _usuarioService = usuarioService;
    }

    public async Task<IEnumerable<Motorista>> ObtenerTodosAsync()
        => await _context.Motoristas
            .AsNoTracking()
            .OrderByDescending(m => m.FechaRegistro)
            .ToListAsync();

    public async Task<Motorista?> ObtenerPorIdAsync(int id)
        => await _context.Motoristas
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<(bool Exito, string? Error)> RegistrarMotoristaAsync(
        RegistrarMotoristaViewModel model)
    {
        var (exito, error, userId) =
            await _usuarioService.RegistrarUsuarioAsync(
                model.Correo,
                model.Password,
                "Motorista",
                model.Telefono);

        if (!exito) return (false, error);

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
            _context.Motoristas.Add(motorista);
            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al guardar el motorista: {ex.Message}");
        }
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(
        Motorista motorista)
    {
        var motoristaDb = await _context.Motoristas.FindAsync(motorista.Id);

        if (motoristaDb == null)
            return (false, "El motorista no existe.");

        var correoDuplicado = await _context.Motoristas.AnyAsync(m =>
            m.Correo.ToLower() == motorista.Correo.Trim().ToLower()
            && m.Id != motorista.Id);

        if (correoDuplicado)
            return (false, "El correo ya está asignado a otro motorista.");

        motoristaDb.Nombre = motorista.Nombre;
        motoristaDb.Apellido = motorista.Apellido;
        motoristaDb.Direccion = motorista.Direccion;
        motoristaDb.Correo = motorista.Correo;
        motoristaDb.Telefono = motorista.Telefono;
        motoristaDb.PerfilSocial = motorista.PerfilSocial;
        motoristaDb.Curriculum = motorista.Curriculum;

        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<bool> CambiarEstadoAsync(int id, string nuevoEstado)
    {
        var motorista = await _context.Motoristas.FindAsync(id);

        if (motorista == null) return false;

        motorista.Estado = nuevoEstado;
        await _context.SaveChangesAsync();

        return true;
    }
}