namespace RecursosHumanos.Domain.Exceptions;

public abstract class ExcepcionDominio : Exception
{
    protected ExcepcionDominio(string mensaje) : base(mensaje) { }
}