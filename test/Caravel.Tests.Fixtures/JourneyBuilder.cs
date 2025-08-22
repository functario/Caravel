using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Graph.Dijkstra;

namespace Caravel.Tests.Fixtures;

public sealed class JourneyBuilder
{
    private readonly Dictionary<Type, NodeBuilder> _nodes = [];
    private Type? _firstNodeType;
    private Map? _map;

    public NodeBuilder AddNode<T>()
        where T : INodeSpy
    {
        var type = typeof(T);
        if (_nodes.ContainsKey(type))
            throw new InvalidOperationException($"{type.Name} already added.");

        var builder = new NodeBuilder(this, type);
        _nodes[type] = builder;

        _firstNodeType ??= type;
        return builder;
    }

    public ImmutableDictionary<Type, NodeBuilder> Nodes => _nodes.ToImmutableDictionary();
    public Type Node => _firstNodeType!;
    public Map Map => _map!;

    public static IEdgeFactory EdgeFactory => new EdgeFactory();
    public static IRouteFactory RouteFactory => new RouteFactory();

    public SmartJourney Build(TimeProvider? timeProvider = default, CancellationToken ct = default)
    {
        var nodesByType = new Dictionary<Type, INodeSpy>();

        // Create nodes with edges
        foreach (var kvp in _nodes)
        {
            var type = kvp.Key;
            var builder = kvp.Value;
            var edges = builder.BuildEdges();
            var node = builder.CreateNode(edges);
            nodesByType[type] = node;
        }

        // Set Map
        _map = new Map([.. nodesByType.Values]);

        // Link edge MoveNext handlers to return neighbor from map
        foreach (var builder in _nodes.Values)
        {
            builder.ResolveMoveNext(nodesByType);
        }

        var graph = new DijkstraGraph(
            [.. nodesByType.Values.Cast<INode>()],
            RouteFactory,
            EdgeFactory
        );

        var startNode = nodesByType[_firstNodeType!];

        timeProvider ??= TimeProvider.System;
        var journeyFactories = new JourneyFactories();
        return new SmartJourney(startNode, graph, timeProvider, journeyFactories, _map, ct);
    }
}

public sealed class NodeBuilder
{
    private readonly JourneyBuilder _parent;
    private readonly Type _type;
    private readonly List<(Type neighbor, int weight, ActionMetaData? description)> _edges = [];
    private bool _auditValue = true;
    private INodeSpy? _instance;

    public NodeBuilder(JourneyBuilder parent, Type type)
    {
        _parent = parent;
        _type = type;
    }

    public NodeBuilder WithEdge<TNeighbor>(int weight = 0, string description = "")
        where TNeighbor : INode => WithEdge(typeof(TNeighbor), weight, description);

    public NodeBuilder WithEdge(Type neighborType, int weight = 0, string description = "")
    {
        var edgeInfo = string.IsNullOrWhiteSpace(description) ? null : description;
        _edges.Add((neighborType, weight, new ActionMetaData(edgeInfo ?? "")));
        return this;
    }

    public NodeBuilder WithAudit(bool value)
    {
        _auditValue = value;
        return this;
    }

    public INodeSpy CreateNode(ImmutableHashSet<IEdge> edges)
    {
        var ctor =
            _type.GetConstructor([typeof(ImmutableHashSet<IEdge>), typeof(bool)])
            ?? throw new InvalidOperationException($"Constructor not found for {_type.Name}");

        _instance = (INodeSpy)ctor.Invoke([edges, _auditValue]);
        return _instance;
    }

    public ImmutableHashSet<IEdge> BuildEdges()
    {
        var origin = _type;
        var edges = _edges
            .Select(tuple =>
            {
                var (neighbor, weight, metaData) = tuple;

                static Task<INode> MoveNext(IJourney _, CancellationToken __) =>
                    throw new InvalidOperationException("Edge not resolved yet.");

                var neighborNavigator = new NeighborNavigator(MoveNext, metaData);
                return new Edge(origin, neighbor, neighborNavigator, weight);
            })
            .ToImmutableHashSet<IEdge>();

        return edges;
    }

    public void ResolveMoveNext(Dictionary<Type, INodeSpy> nodesByType)
    {
        ArgumentNullException.ThrowIfNull(nodesByType, nameof(nodesByType));
        if (_instance is null)
            throw new InvalidOperationException("Node instance not created yet.");

        var newEdges = _instance
            .InternalEdges.Select(edge =>
                (IEdge)
                    new Edge(
                        edge.Origin,
                        edge.Neighbor,
                        CreateNeighborNavigator(nodesByType, edge),
                        edge.Weight
                    )
            )
            .ToImmutableHashSet();

        var ctor =
            _type.GetConstructor([typeof(ImmutableHashSet<IEdge>), typeof(bool)])
            ?? throw new InvalidOperationException("Node must have expected constructor.");

        var newNode = (INodeSpy)ctor.Invoke([newEdges, _auditValue]);
        nodesByType[_type] = newNode;
    }

    public JourneyBuilder Done() => _parent;

    private static NeighborNavigator CreateNeighborNavigator(
        Dictionary<Type, INodeSpy> nodesByType,
        IEdge edge
    )
    {
        return new NeighborNavigator(
            (_, _) => Task.FromResult<INode>(nodesByType[edge.Neighbor]),
            edge.NeighborNavigator.MetaData
        );
    }
}
