namespace Caravel.Abstractions.Exceptions;

/// <summary>
/// Exception throws when invalid <see cref="IWaypoints"/> are detected.
/// </summary>
public class InvalidWaypointsException : CaravelException
{
    private static string DefaultMessage(InvalidWaypointReasons reason, params Type[] types) =>
        $"Invalid {nameof(IWaypoints)} detected with reason {Enum.GetName(reason)}: "
        + $"{string.Join(';', types.Select(x => x.FullName))}.";

    public InvalidWaypointsException(InvalidWaypointReasons reason, params Type[] types)
        : base(DefaultMessage(reason, types)) { }
}

public enum InvalidWaypointReasons
{
    WaypointsInExcludedNodes,
}
