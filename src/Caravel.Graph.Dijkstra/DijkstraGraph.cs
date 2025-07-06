using System.Collections.Frozen;
using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Abstractions.Exceptions;
using Caravel.Core;

namespace Caravel.Graph.Dijkstra;

public sealed class DijkstraGraph : IGraph
{
    private readonly FrozenDictionary<Type, INode> _registeredNodes;

    public DijkstraGraph(IEnumerable<INode> nodes)
    {
        _registeredNodes = nodes.ToDictionary(n => n.GetType(), n => n).ToFrozenDictionary();
    }

    /// <inheritdoc />
    public FrozenDictionary<Type, INode> Nodes => _registeredNodes;

    /// <inheritdoc />
    public IRoute GetSelfRoute(INode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        var type = node.GetType();
        var selfEdge = new Edge(type, type, new NeighborNavigator((_, _) => Task.FromResult(node)));
        return new Route([selfEdge]);
    }

    /// <inheritdoc />
    public IRoute GetRoute(
        Type origin,
        Type destination,
        IWaypoints waypoints,
        IExcludedNodes excludedNodes
    )
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

    private static void ValidateWaypointsNotExcluded(
        IWaypoints waypoints,
        IExcludedNodes excludedNodes
    )
    {
        var invalidWaypoints = excludedNodes.Where(x => waypoints.Contains(x)).ToArray();
        if (invalidWaypoints.Length > 0)
        {
            throw new InvalidWaypointsException(
                InvalidWaypointReasons.WaypointsInExcludedNodes,
                invalidWaypoints
            );
        }
    }

    private static void ValidateOriginAndDestinationNotExcluded(
        Type origin,
        Type destination,
        IExcludedNodes excludedNodes
    )
    {
        var isOriginExcluded = IsExcluded(origin, excludedNodes);
        var isdestinationExcluded = IsExcluded(destination, excludedNodes);
        InvalidRouteReasons? reason = (isOriginExcluded, isdestinationExcluded) switch
        {
            (true, true) => InvalidRouteReasons.ExtremityNodesExcluded,
            (false, true) => InvalidRouteReasons.DestinationNodeExcluded,
            (true, false) => InvalidRouteReasons.OriginNodeExcluded,
            (false, false) => null,
        };

        if (reason is null)
        {
            return;
        }

        throw new InvalidRouteException((InvalidRouteReasons)reason!, origin, destination);
    }

    private static bool IsExcluded(Type node, IExcludedNodes excludedNodes) =>
        excludedNodes.Any(x => x == node);

    private List<IEdge> Dijkstra(Type origin, Type destination, IExcludedNodes excludedNodes)
    {
        var distances = new Dictionary<Type, int> { [origin] = 0 };
        var previous = new Dictionary<Type, IEdge>();
        var visited = new HashSet<Type>();

        var queue = new PriorityQueue<Type, int>();
        queue.Enqueue(origin, 0);

        while (queue.TryDequeue(out var nodeVisited, out _))
        {
            if (IsExcluded(nodeVisited, excludedNodes))
                continue;

            if (visited.Contains(nodeVisited))
                continue;

            if (nodeVisited == destination)
                break;

            visited.Add(nodeVisited);

            if (!_registeredNodes.TryGetValue(nodeVisited, out var currentNode))
                throw new UnknownNodeException(nodeVisited);

            var edges = OrderedEdges(currentNode.GetEdges());
            ThrowIfDuplicatedEdges(edges);

            foreach (var edge in edges)
            {
                if (edge is null)
                    throw new InvalidEdgeException(InvalidEdgeReasons.Null);

                var neighbor = edge.Neighbor;
                var newDistance = distances[nodeVisited] + edge.Weight;

                if (
                    !distances.TryGetValue(neighbor, out var knownDistance)
                    || newDistance < knownDistance
                )
                {
                    distances[neighbor] = newDistance;
                    previous[neighbor] = edge;
                    queue.Enqueue(neighbor, newDistance);
                }
            }
        }

        if (!previous.ContainsKey(destination))
            throw new RouteNotFoundException(origin, destination);

        // Reconstruct path
        var path = new List<IEdge>();
        var node = destination;

        while (node != origin)
        {
            var edge = previous[node];
            path.Insert(0, edge);
            node = edge.Origin;
        }

        return path;
    }

    // Edges have to be ordered to ensure a deterministic route behavior
    private static List<IEdge> OrderedEdges(ImmutableHashSet<IEdge> edges) =>
        [.. edges.OrderBy(e => e.Neighbor.FullName)];

    private static void ThrowIfDuplicatedEdges(ICollection<IEdge> edges)
    {
        var duplicates = edges
            .GroupBy(x => (x.Origin, x.Neighbor, x.Weight))
            .Where(g => g.Count() > 1)
            .Select(g => g.First())
            .ToList();

        if (duplicates.Count > 0)
        {
            throw new DuplicatedEdgesException(duplicates);
        }
    }
}
