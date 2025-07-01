namespace Caravel.Abstractions.Exceptions;

/// <summary>
/// Exception throws when there is no <see cref="IRoute"/> found between two <see cref="INode"/>.
/// </summary>
public class RouteNotFoundException : CaravelException
{
    private static string DefaultMessage(Type origin, Type destination) =>
        $"No {nameof(IRoute)} found between origin {nameof(INode)} '{origin.FullName}' and destination {nameof(INode)} '{destination.FullName}'.";

    public RouteNotFoundException(Type origin, Type destination)
        : base(DefaultMessage(origin, destination)) { }
}
