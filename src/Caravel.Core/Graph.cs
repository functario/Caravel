using System.Collections.Frozen;
using Caravel.Abstractions;

namespace Caravel.Core;
public sealed class Graph : IGraph
{
    private readonly FrozenDictionary<Type, INode> _nodes;

    public Graph(ICollection<INode> nodes)
    {
        _nodes = nodes.ToDictionary(n => n.GetType(), n => n).ToFrozenDictionary();
    }

    public IRoute GetShortestRoute(Type origin, Type destination, IWaypoints waypoints, IExcludedNodes excludedNodes)
    {
        ArgumentNullException.ThrowIfNull(origin);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(waypoints);
        ArgumentNullException.ThrowIfNull(excludedNodes);
        ValidateOriginAndDestinationNotExcluded(origin, destination, excludedNodes);
        ValidateWaypointsNotExcluded(waypoints, excludedNodes);

        var allEdges = new List<IEdge>();
        var current = origin;

        foreach (var waypoint in waypoints)
        {
            var segment = Dijkstra(current, waypoint, excludedNodes);
            allEdges.AddRange(segment);
            current = waypoint;
        }

        var finalSegment = Dijkstra(current, destination, excludedNodes);
        allEdges.AddRange(finalSegment);

        return new Route([.. allEdges]);
    }

    private static void ValidateWaypointsNotExcluded(IWaypoints waypoints, IExcludedNodes excludedNodes)
    {
        if (excludedNodes.Any(x => waypoints.Contains(x)))
        {
            throw new InvalidOperationException("Some waypoints are excluded.");
        }
    }

    private static void ValidateOriginAndDestinationNotExcluded(Type origin, Type destination, IExcludedNodes excludedNodes)
    {
        var isOriginExcluded = IsExcluded(origin, excludedNodes);
        var isdestinationExcluded = IsExcluded(destination, excludedNodes);
        var _ = (isOriginExcluded, isdestinationExcluded) switch
        {
            (true, true) => throw new NotSupportedException("Origin and Destination should not be excluded."),
            (false, true) => throw new NotSupportedException("Origin should not be excluded."),
            (true, false) => throw new NotSupportedException("Destination should not be excluded."),
            (false, false) => false,
        };
    }

    private static bool IsExcluded(Type node, IExcludedNodes excludedNodes)
    => excludedNodes.Any(x => x == node);

    private List<IEdge> Dijkstra(Type start, Type end, IExcludedNodes excludedNodes)
    {
        var distances = new Dictionary<Type, int> { [start] = 0 };
        var previous = new Dictionary<Type, IEdge>();
        var visited = new HashSet<Type>();

        var queue = new PriorityQueue<Type, int>();
        queue.Enqueue(start, 0);

        while (queue.TryDequeue(out var current, out _))
        {
            if (IsExcluded(current, excludedNodes))
                continue;

            if (visited.Contains(current))
                continue;

            if (current == end)
                break;

            visited.Add(current);

            if (!_nodes.TryGetValue(current, out var currentNode))
                throw new InvalidOperationException($"Node of type {current.Name} not found in graph.");

            foreach (var edge in currentNode.GetEdges())
            {
                var neighbor = edge.Neighbor;
                var newDistance = distances[current] + edge.Weight;

                if (!distances.TryGetValue(neighbor, out var knownDistance) || newDistance < knownDistance)
                {
                    distances[neighbor] = newDistance;
                    previous[neighbor] = edge;
                    queue.Enqueue(neighbor, newDistance);
                }
            }
        }

        if (!previous.ContainsKey(end))
            throw new InvalidOperationException($"No route found from {start.Name} to {end.Name}");

        // Reconstruct path
        var path = new List<IEdge>();
        var node = end;

        while (node != start)
        {
            var edge = previous[node];
            path.Insert(0, edge);
            node = edge.Origin;
        }

        return path;
    }
}
