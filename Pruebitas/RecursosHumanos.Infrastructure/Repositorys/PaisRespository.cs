using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Domain;
using RecursosHumanos.Domain.Repositories;
using RecursosHumanos.Infrastructure.Data;

namespace RecursosHumanos.Infrastructure.Repositories;

public class PaisRepository : IPaisRepository
{
    private readonly AppDbContext _context;

    public PaisRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pais?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Paises.FindAsync(id);
    }

    public async Task<Pais?> ObtenerPorNombreAsync(string nombre)
    {
        return await _context.Paises.FirstOrDefaultAsync(p => p.Nombre == nombre);
    }

    public async Task<List<Pais>> ObtenerTodosAsync()
    {
        return await _context.Paises.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task CrearAsync(Pais pais)
    {
        await _context.Paises.AddAsync(pais);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Pais pais)
    {
        _context.Paises.Update(pais);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Pais pais)
    {
        _context.Paises.Remove(pais);
        await _context.SaveChangesAsync();
    }
}