using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Domain;
using RecursosHumanos.Domain.Repositories;
using RecursosHumanos.Infrastructure.Data;

namespace RecursosHumanos.Infrastructure.Repositories;

public class EmpresaRepository : IEmpresaRepository
{
    private readonly AppDbContext _context;

    public EmpresaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Empresa>> ObtenerTodosAsync()
    {
        // Incluimos Municipio para que se vea el nombre en la tabla
        return await _context.Empresas
            .Include(e => e.Municipio) 
            .OrderBy(e => e.NombreComercial)
            .ToListAsync();
    }

    public async Task<Empresa?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Empresas.FindAsync(id);
    }

    // --- ESTE ES EL MÉTODO QUE TE FALTABA ---
    public async Task<Empresa?> ObtenerPorNitAsync(string nit)
    {
        return await _context.Empresas
            .FirstOrDefaultAsync(e => e.Nit == nit);
    }
    // ----------------------------------------

    public async Task AgregarAsync(Empresa empresa)
    {
        await _context.Empresas.AddAsync(empresa);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Empresa empresa)
    {
        _context.Empresas.Update(empresa);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Guid id)
    {
        var entidad = await ObtenerPorIdAsync(id);
        if (entidad != null)
        {
            _context.Empresas.Remove(entidad);
            await _context.SaveChangesAsync();
        }
    }
}