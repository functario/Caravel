namespace Caravel.Abstractions.Exceptions;

/// <summary>
/// Exception throws when an invalid <see cref="IRoute"/> is detected.
/// </summary>
public class CannotChangeStartingNodeException : CaravelException
{
    private static string MessageWithOriginAndDestination(
        string? originalStartingNodeTypeFullName,
        string? requestedStartingNodeTypeFullName
    )
    {
        return $"Cannot change starting {nameof(INode)} once the {nameof(IJourney)} "
            + $"is started (current starting node '{originalStartingNodeTypeFullName}', "
            + $"requested starting node '{requestedStartingNodeTypeFullName}')";
    }

    public CannotChangeStartingNodeException(
        string? originalStartingNodeTypeFullName,
        string? requestedStartingNodeTypeFullName
    )
        : base(
            MessageWithOriginAndDestination(
                originalStartingNodeTypeFullName,
                requestedStartingNodeTypeFullName
            )
        )
    { }
}
