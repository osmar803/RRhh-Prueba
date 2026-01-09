namespace RecursosHumanos.Domain.Exceptions;

public class ReglaNegocioException : ExcepcionDominio
{
    public ReglaNegocioException(string mensaje) : base(mensaje) { }
}