namespace RecursosHumanos.Domain.Repositories;

public interface IMunicipioRepository
{
    Task<Municipio?> ObtenerPorIdAsync(Guid id);
    Task<List<Municipio>> ObtenerPorDepartamentoAsync(Guid departamentoId);
    Task<List<Municipio>> ObtenerTodosAsync();

    Task AgregarAsync(Municipio municipio);
    Task ActualizarAsync(Municipio municipio);
    Task EliminarAsync(Guid id);
}