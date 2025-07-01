namespace Caravel.Abstractions.Exceptions;

/// <summary>
/// Exception throws when the <see cref="INode"/> is not registered in <see cref="IGraph"/>.
/// </summary>
public class UnknownNodeException : CaravelException
{
    private static string DefaultMessage(Type nodeType) =>
        $"The {nameof(INode)} '{nodeType.FullName}' is unknown.";

    public UnknownNodeException(Type nodeType)
        : base(DefaultMessage(nodeType)) { }
}
