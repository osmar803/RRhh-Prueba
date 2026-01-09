namespace RecursosHumanos.Domain.Repositories;

public interface IDepartamentoRepository
{
    Task<Departamento?> ObtenerPorIdAsync(Guid id);
    Task<List<Departamento>> ObtenerPorPaisAsync(Guid paisId);
    Task<List<Departamento>> ObtenerTodosAsync();

    Task AgregarAsync(Departamento departamento);
    Task ActualizarAsync(Departamento departamento);
    Task EliminarAsync(Guid id);
}