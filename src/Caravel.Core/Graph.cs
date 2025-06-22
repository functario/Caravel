using System.Collections.Frozen;
using Caravel.Abstractions;

namespace Caravel.Core;

public class Graph : IGraph
{
    public IRoute GetShortestRoute(INode origin, INode destination)
    {
        return GetShortestRoute(origin, Array.Empty<INode>(), destination);
    }

    public IRoute GetShortestRoute(INode origin, ICollection<INode> waypoints, INode destination)
    {
        ArgumentNullException.ThrowIfNull(origin);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(waypoints);
        var allEdges = new List<IEdge>();
        var current = origin;

        foreach (var waypoint in waypoints)
        {
            var route = Dijkstra(current, waypoint) ?? throw new InvalidOperationException($"No route from {current.Name} to waypoint {waypoint.Name}.");
            allEdges.AddRange(route.Edges);
            current = waypoint;
        }

        var finalRoute = Dijkstra(current, destination) ?? throw new InvalidOperationException($"No route from {current.Name} to destination {destination.Name}.");
        allEdges.AddRange(finalRoute.Edges);

        return new Route(allEdges.ToFrozenSet());
    }

    private static Route Dijkstra(INode start, INode destination)
    {
        var visited = new HashSet<INode>();
        var distances = new Dictionary<INode, int> { [start] = 0 };
        var previous = new Dictionary<INode, IEdge>();

        var queue = new PriorityQueue<INode, int>();
        queue.Enqueue(start, 0);

        while (queue.TryDequeue(out var current, out _))
        {
            if (visited.Contains(current))
                continue;

            if (current == destination)
                break;

            visited.Add(current);

            var edges = current.GetEdges();
            foreach (var edge in edges)
            {
                var neighbor = edge.Neighbor;
                var newDist = distances[current] + edge.Weight;

                if (!distances.TryGetValue(neighbor, out var value) || newDist < value)
                {
                    value = newDist;
                    distances[neighbor] = value;
                    previous[neighbor] = edge;
                    queue.Enqueue(neighbor, newDist);
                }
            }
        }

        // Reconstruct path
        if (!previous.ContainsKey(destination))
            throw new InvalidOperationException("Does not contains node");

        var pathEdges = new List<IEdge>();
        var node = destination;

        while (node != start)
        {
            var edge = previous[node];
            if (edge == null)
                break;

            pathEdges.Insert(0, edge);
            node = edge.Origin;
        }

        return new Route(pathEdges.ToFrozenSet());
    }

}
