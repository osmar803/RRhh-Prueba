using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Domain;
using RecursosHumanos.Domain.Repositories;
using RecursosHumanos.Infrastructure.Data;

namespace RecursosHumanos.Infrastructure.Repositories;

public class MunicipioRepository : IMunicipioRepository
{
    private readonly AppDbContext _context;

    public MunicipioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Municipio?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Municipios.FindAsync(id);
    }

    public async Task<List<Municipio>> ObtenerPorDepartamentoAsync(Guid departamentoId)
    {
        return await _context.Municipios
            .Where(m => m.DepartamentoId == departamentoId)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<List<Municipio>> ObtenerTodosAsync()
    {
        return await _context.Municipios.OrderBy(m => m.Nombre).ToListAsync();
    }

    public async Task AgregarAsync(Municipio municipio)
    {
        await _context.Municipios.AddAsync(municipio);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Municipio municipio)
    {
        _context.Municipios.Update(municipio);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Guid id)
    {
        var muni = await ObtenerPorIdAsync(id);
        if (muni != null)
        {
            _context.Municipios.Remove(muni);
            await _context.SaveChangesAsync();
        }
    }
}