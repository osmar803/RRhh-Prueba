using RecursosHumanos.Domain.Exceptions;

namespace RecursosHumanos.Domain;

public class Pais
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; }

    private Pais() { }

    public Pais(string nombre)
    {
        CambiarNombre(nombre);
        Id = Guid.NewGuid();
    }

    public void CambiarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaNegocioException("El nombre del país es obligatorio");

        Nombre = nombre.Trim();
    }
}