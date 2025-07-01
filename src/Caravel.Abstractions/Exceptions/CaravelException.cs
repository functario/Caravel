namespace Caravel.Abstractions.Exceptions;

public class CaravelException : Exception
{
    public CaravelException() { }

    public CaravelException(string message)
        : base(message) { }

    public CaravelException(string message, Exception innerException)
        : base(message, innerException) { }
}
