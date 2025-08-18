namespace Caravel.Abstractions.Exceptions;

/// <summary>
/// Exception throws when the expected <see cref="INode"/> is not the one detected.
/// </summary>
public class UnexpectedNodeException : CaravelException
{
    private static string DefaultMessage(Type expected, Type current) =>
        $"The {nameof(INode)} '{current.FullName}' is not the expected '{expected.FullName}'.";

    public UnexpectedNodeException(Type node, Type current)
        : base(DefaultMessage(node, current)) { }
}
