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

    public IRoute GetShortestRoute(Type origin, Type destination)
        => GetShortestRoute(origin, destination, Waypoints.Empty());

    public IRoute GetShortestRoute(Type origin, Type destination, IWaypoints waypoints)
    {
        ArgumentNullException.ThrowIfNull(origin);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(waypoints);

        var allEdges = new List<IEdge>();
        var current = origin;

        foreach (var waypoint in waypoints)
        {
            var segment = Dijkstra(current, waypoint);
            allEdges.AddRange(segment);
            current = waypoint;
        }

        var finalSegment = Dijkstra(current, destination);
        allEdges.AddRange(finalSegment);

        return new Route([.. allEdges]);
    }

    private List<IEdge> Dijkstra(Type start, Type end)
    {
        var distances = new Dictionary<Type, int> { [start] = 0 };
        var previous = new Dictionary<Type, IEdge>();
        var visited = new HashSet<Type>();

        var queue = new PriorityQueue<Type, int>();
        queue.Enqueue(start, 0);

        while (queue.TryDequeue(out var current, out _))
        {
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
