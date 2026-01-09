namespace RecursosHumanos.Domain.Repositories;

public interface IColaboradorRepository
{
    Task<Colaborador?> ObtenerPorIdAsync(Guid id);
    Task<List<Colaborador>> ObtenerTodosAsync();

    Task AgregarAsync(Colaborador colaborador);
    Task ActualizarAsync(Colaborador colaborador);
    Task EliminarAsync(Guid id);
}