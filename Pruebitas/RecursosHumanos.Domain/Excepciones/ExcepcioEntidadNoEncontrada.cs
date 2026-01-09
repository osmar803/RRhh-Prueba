namespace RecursosHumanos.Domain.Exceptions;

public class EntidadNoEncontradaException : ExcepcionDominio
{
    public EntidadNoEncontradaException(string entidad, object id)
        : base($"{entidad} con id '{id}' no fue encontrada") { }
}