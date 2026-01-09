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

    public async Task<Empresa?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Empresas
            .Include(e => e.Colaboradores) // Incluimos la relación
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Empresa?> ObtenerPorNitAsync(string nit)
    {
        return await _context.Empresas.FirstOrDefaultAsync(e => e.Nit == nit);
    }

    public async Task<List<Empresa>> ObtenerTodosAsync()
    {
        // Opcionalmente podrías incluir Municipio y Departamento aquí si los necesitas para mostrar en la grilla
        return await _context.Empresas.OrderBy(e => e.RazonSocial).ToListAsync();
    }

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
        var empresa = await ObtenerPorIdAsync(id);
        if (empresa != null)
        {
            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();
        }
    }
}