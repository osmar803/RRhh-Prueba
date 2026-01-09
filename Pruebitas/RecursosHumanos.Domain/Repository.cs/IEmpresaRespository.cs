namespace RecursosHumanos.Domain.Repositories;

public interface IEmpresaRepository
{
    Task<Empresa?> ObtenerPorIdAsync(Guid id);
    Task<Empresa?> ObtenerPorNitAsync(string nit);
    Task<List<Empresa>> ObtenerTodosAsync();

    Task AgregarAsync(Empresa empresa);
    Task ActualizarAsync(Empresa empresa);
    Task EliminarAsync(Guid id);
}