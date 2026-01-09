using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Domain;
using RecursosHumanos.Domain.Repositories;
using RecursosHumanos.Infrastructure.Data;

namespace RecursosHumanos.Infrastructure.Repositories;

public class DepartamentoRepository : IDepartamentoRepository
{
    private readonly AppDbContext _context;

    public DepartamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Departamento?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Departamentos.FindAsync(id);
    }

    public async Task<List<Departamento>> ObtenerPorPaisAsync(Guid paisId)
    {
        return await _context.Departamentos
            .Where(d => d.PaisId == paisId)
            .OrderBy(d => d.Nombre)
            .ToListAsync();
    }

    // --- AQUÍ ESTÁ EL CAMBIO CLAVE ---
    public async Task<List<Departamento>> ObtenerTodosAsync()
    {
        return await _context.Departamentos
            .Include(d => d.Pais) // <--- ¡ESTA LÍNEA HACE LA MAGIA!
            .OrderBy(d => d.Nombre)
            .ToListAsync();
    }
    // ---------------------------------

    public async Task AgregarAsync(Departamento departamento)
    {
        await _context.Departamentos.AddAsync(departamento);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Departamento departamento)
    {
        _context.Departamentos.Update(departamento);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Guid id)
    {
        var depto = await ObtenerPorIdAsync(id);
        if (depto != null)
        {
            _context.Departamentos.Remove(depto);
            await _context.SaveChangesAsync();
        }
    }
}