namespace RecursosHumanos.Domain.Exceptions;

public class ConflictoDominioException : ExcepcionDominio
{
    public ConflictoDominioException(string mensaje) : base(mensaje) { }
}