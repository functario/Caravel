namespace Caravel.Abstractions.Exceptions;

/// <summary>
/// Exception throws when an invalid <see cref="IRoute"/> is detected.
/// </summary>
public class InvalidRouteException : CaravelException
{
    private static string MessageWithOriginAndDestination(
        InvalidRouteReasons reason,
        Type? origin,
        Type? destination
    )
    {
        var isOriginNull = origin is null;
        var isDestinationNull = destination is null;
        var sub = (isOriginNull, isDestinationNull) switch
        {
            (true, true) => "",
            (false, true) => $" (origin: {origin?.FullName})",
            (true, false) => $" (destination: {destination?.FullName})",
            (false, false) =>
                $" (origin: '{origin?.FullName}', destination: '{destination?.FullName}')",
        };

        return $"Invalid {nameof(IRoute)} detected with reason '{Enum.GetName(reason)}'{sub}.";
    }

    public InvalidRouteException(InvalidRouteReasons reason, Type? origin, Type? destination)
        : base(MessageWithOriginAndDestination(reason, origin, destination)) { }
}

public enum InvalidRouteReasons
{
    ExtremityNodesExcluded = 0,
    OriginNodeExcluded,
    DestinationNodeExcluded,
}
