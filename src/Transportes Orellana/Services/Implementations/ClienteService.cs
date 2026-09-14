using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Models;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Services.Implementations;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _context;

    public ClienteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
    {
        return await _context.Clientes
            .OrderByDescending(c => c.FechaRegistro)
            .ToListAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(bool Exito, string? Error)> CrearAsync(Cliente cliente)
    {
        // No duplicar clientes con el mismo correo
        bool correoExiste = await _context.Clientes
            .AnyAsync(c => c.Correo.ToLower() == cliente.Correo.Trim().ToLower());

        if (correoExiste)
        {
            return (false, "Ya existe un cliente registrado con este correo electrónico.");
        }

        cliente.FechaRegistro = DateTime.UtcNow;
        cliente.Estado = "activo";

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(Cliente cliente)
    {
        var clienteDb = await _context.Clientes.FindAsync(cliente.Id);
        if (clienteDb == null)
        {
            return (false, "El cliente no existe.");
        }

        // Validar duplicidad de correo con otro cliente
        bool correoDuplicado = await _context.Clientes
            .AnyAsync(c => c.Correo.ToLower() == cliente.Correo.Trim().ToLower() && c.Id != cliente.Id);

        if (correoDuplicado)
        {
            return (false, "El correo ya está asignado a otro cliente.");
        }

        clienteDb.Nombre = cliente.Nombre;
        clienteDb.Apellido = cliente.Apellido;
        clienteDb.Correo = cliente.Correo;
        clienteDb.Telefono = cliente.Telefono;
        clienteDb.Direccion = cliente.Direccion;
        clienteDb.Estado = cliente.Estado;
        
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<bool> CambiarEstadoAsync(int id, string nuevoEstado)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return false;

        cliente.Estado = nuevoEstado;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Clientes.AnyAsync(c => c.Id == id);
    }
}