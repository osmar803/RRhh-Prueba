using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Domain;
using RecursosHumanos.Domain.Repositories;
using RecursosHumanos.Infrastructure.Data;

namespace RecursosHumanos.Infrastructure.Repositories;

public class ColaboradorRepository : IColaboradorRepository
{
    private readonly AppDbContext _context;

    public ColaboradorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Colaborador?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Colaboradores
            .Include(c => c.Empresas)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Colaborador>> ObtenerTodosAsync()
    {
        return await _context.Colaboradores
            .OrderBy(c => c.NombreCompleto)
            .ToListAsync();
    }

    public async Task AgregarAsync(Colaborador colaborador)
    {
        await _context.Colaboradores.AddAsync(colaborador);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Colaborador colaborador)
    {
        _context.Colaboradores.Update(colaborador);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Guid id)
    {
        var colab = await ObtenerPorIdAsync(id);
        if (colab != null)
        {
            _context.Colaboradores.Remove(colab);
            await _context.SaveChangesAsync();
        }
    }
}