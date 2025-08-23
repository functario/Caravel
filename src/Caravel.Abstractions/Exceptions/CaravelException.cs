namespace Caravel.Abstractions.Exceptions;

public class CaravelException : Exception
{
    public CaravelException(string message)
        : base(message) { }
}
