using System.Collections.Frozen;
using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Abstractions.Configurations;
using Caravel.Abstractions.Exceptions;

namespace Caravel.Graph.Dijkstra;

public sealed class DijkstraGraph : IGraph
{
    private readonly FrozenDictionary<Type, INode> _registeredNodes;
    private readonly IRouteFactory _routeFactory;
    private readonly IEdgeFactory _edgeFactory;

    public DijkstraGraph(
        IEnumerable<INode> nodes,
        IRouteFactory routeFactory,
        IEdgeFactory edgeFactory
    )
    {
        _registeredNodes = nodes.ToDictionary(n => n.GetType(), n => n).ToFrozenDictionary();
        _routeFactory = routeFactory;
        _edgeFactory = edgeFactory;
    }

    /// <inheritdoc />
    public FrozenDictionary<Type, INode> Nodes => _registeredNodes;
    public IRouteFactory RouteFactory => _routeFactory;
    public IEdgeFactory EdgeFactory => _edgeFactory;

    /// <inheritdoc />
    public IRoute GetSelfRoute(INode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        var selfEdge = _edgeFactory.CreteSelfEdge(node);
        return _routeFactory.CreateRoute([selfEdge]);
    }

    /// <inheritdoc />
    public IRoute GetRoute(
        Type origin,
        Type destination,
        IWaypoints waypoints,
        IExcludedWaypoints excludedWaypoints
    )
    {
        ArgumentNullException.ThrowIfNull(origin);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(waypoints);
        ArgumentNullException.ThrowIfNull(excludedWaypoints);
        ValidateOriginAndDestinationNotExcluded(origin, destination, excludedWaypoints);
        ValidateWaypointsNotExcluded(waypoints, excludedWaypoints);

        var allEdges = new List<IEdge>();
        var current = origin;

        foreach (var waypoint in waypoints)
        {
            var segment = Dijkstra(current, waypoint, excludedWaypoints);
            allEdges.AddRange(segment);
            current = waypoint;
        }

        var finalSegment = Dijkstra(current, destination, excludedWaypoints);
        allEdges.AddRange(finalSegment);

        return _routeFactory.CreateRoute([.. allEdges]);
    }

    private static void ValidateWaypointsNotExcluded(
        IWaypoints waypoints,
        IExcludedWaypoints excludedWaypoints
    )
    {
        var invalidWaypoints = excludedWaypoints.Where(x => waypoints.Contains(x)).ToArray();
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
        IExcludedWaypoints excludedWaypoints
    )
    {
        var isOriginExcluded = IsExcluded(origin, excludedWaypoints);
        var isdestinationExcluded = IsExcluded(destination, excludedWaypoints);
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

    private static bool IsExcluded(Type node, IExcludedWaypoints excludedWaypoints) =>
        excludedWaypoints.Any(x => x == node);

    private List<IEdge> Dijkstra(Type origin, Type destination, IExcludedWaypoints excludedWaypoints)
    {
        var distances = new Dictionary<Type, int> { [origin] = 0 };
        var previous = new Dictionary<Type, IEdge>();
        var visited = new HashSet<Type>();

        var queue = new PriorityQueue<Type, int>();
        queue.Enqueue(origin, 0);

        while (queue.TryDequeue(out var nodeVisited, out _))
        {
            if (IsExcluded(nodeVisited, excludedWaypoints))
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
