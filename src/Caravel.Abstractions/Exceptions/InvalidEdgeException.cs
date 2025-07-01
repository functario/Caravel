namespace Caravel.Abstractions.Exceptions;

/// <summary>
/// Exception throws when an invalid <see cref="IEdge"/> is detected.
/// </summary>
public class InvalidEdgeException : CaravelException
{
    private static string DefaultMessage(InvalidEdgeReasons reason) =>
        $"Invalid {nameof(IEdge)} detected with reason '{Enum.GetName(reason)}'.";

    public InvalidEdgeException(InvalidEdgeReasons reason)
        : base(DefaultMessage(reason)) { }
}

public enum InvalidEdgeReasons
{
    Null = 0,
}
