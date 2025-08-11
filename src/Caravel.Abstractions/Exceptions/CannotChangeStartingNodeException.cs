namespace Caravel.Abstractions.Exceptions;

/// <summary>
/// Exception throws when an invalid <see cref="IRoute"/> is detected.
/// </summary>
public class CannotChangeStartingNodeException : CaravelException
{
    private static string MessageWithOriginAndDestination(
        CannotChangeStartingNodeReasons reason,
        string? originalStartingNode,
        Type? expectedStartingNode
    )
    {
        var isOriginalStartingNodeNull = originalStartingNode is null;
        var isExpectedStartingNodeNull = expectedStartingNode is null;
        var sub = (isOriginalStartingNodeNull, isExpectedStartingNodeNull) switch
        {
            (true, true) => "",
            (false, true) => $" (original: {originalStartingNode})",
            (true, false) => $" (expected: {expectedStartingNode})",
            (false, false) =>
                $" (original: '{originalStartingNode}', expected: '{expectedStartingNode?.FullName}')",
        };

        return $"Invalid {nameof(IRoute)} detected with reason '{Enum.GetName(reason)}'{sub}.";
    }

    public CannotChangeStartingNodeException(
        CannotChangeStartingNodeReasons reason,
        string? originalStartingNode,
        Type? expectedStartingNode
    )
        : base(MessageWithOriginAndDestination(reason, originalStartingNode, expectedStartingNode))
    { }
}

public enum CannotChangeStartingNodeReasons
{
    CanBeChangedOnlyOnce = 0,
    CannotBeChangedAfterJourneyStarted,
}
