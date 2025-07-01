using Caravel.Abstractions;
using Caravel.Core;

namespace Caravel.Graph.Dijkstra.Extensions;

public static class GraphExtensions
{
    public static IRoute GetShortestRoute(this IGraph graph, Type origin, Type destination)
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        return graph.GetRoute(origin, destination, Waypoints.Empty(), ExcludedNodes.Empty());
    }

    public static IRoute GetShortestRoute(
        this IGraph graph,
        Type origin,
        Type destination,
        IExcludedNodes excludedNodes
    )
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        return graph.GetRoute(origin, destination, Waypoints.Empty(), excludedNodes);
    }

    public static IRoute GetShortestRoute(
        this IGraph graph,
        Type origin,
        Type destination,
        IWaypoints waypoints
    )
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        return graph.GetRoute(origin, destination, waypoints, ExcludedNodes.Empty());
    }
}
