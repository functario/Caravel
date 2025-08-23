using Caravel.Abstractions;

namespace Caravel.Graph.Dijkstra.Extensions;

public static class GraphExtensions
{
    public static IRoute GetShortestRoute(this IGraph graph, Type origin, Type destination)
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        return graph.GetRoute(origin, destination, IWaypoints.Empty(), IExcludedWaypoints.Empty());
    }

    public static IRoute GetShortestRoute(
        this IGraph graph,
        Type origin,
        Type destination,
        IExcludedWaypoints excludedWaypoints
    )
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        return graph.GetRoute(origin, destination, IWaypoints.Empty(), excludedWaypoints);
    }

    public static IRoute GetShortestRoute(
        this IGraph graph,
        Type origin,
        Type destination,
        IWaypoints waypoints
    )
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        return graph.GetRoute(origin, destination, waypoints, IExcludedWaypoints.Empty());
    }
}
