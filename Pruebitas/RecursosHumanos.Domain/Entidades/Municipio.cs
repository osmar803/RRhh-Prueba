using RecursosHumanos.Domain.Exceptions;

namespace RecursosHumanos.Domain;

public class Municipio
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; }
    public Guid DepartamentoId { get; private set; }

    private Municipio() { }

    public Municipio(string nombre, Guid departamentoId)
    {
        CambiarNombre(nombre);
        CambiarDepartamento(departamentoId);
        Id = Guid.NewGuid();
    }

    public void CambiarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaNegocioException("El nombre del municipio es obligatorio");

        Nombre = nombre.Trim();
    }

    private void CambiarDepartamento(Guid departamentoId)
    {
        if (departamentoId == Guid.Empty)
            throw new ReglaNegocioException("El departamento es obligatorio");

        DepartamentoId = departamentoId;
    }
}