using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Graph.Dijkstra;
using Caravel.Tests.Fixtures.NodeSpies;

namespace Caravel.Tests.Fixtures;
public sealed class JourneyBuilder
{
    private readonly Dictionary<Type, NodeBuilder> _nodes = [];
    private Type? _firstNodeType;

    public NodeBuilder AddNode<T>() where T : INodeSpy
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

    public IJourney Build(CancellationToken ct = default)
    {
        var nodeInstances = new Dictionary<Type, INodeSpy>();

        // Create nodes with edges
        foreach (var kvp in _nodes)
        {
            var type = kvp.Key;
            var builder = kvp.Value;
            var edges = builder.BuildEdges();
            var node = builder.CreateNode(edges);
            nodeInstances[type] = node;
        }

        // Link edge MoveNext handlers to return neighbor from map
        foreach (var builder in _nodes.Values)
        {
            builder.ResolveMoveNext(nodeInstances);
        }

        var graph = new DijkstraGraph([.. nodeInstances.Values.Cast<INode>()]);
        var startNode = nodeInstances[_firstNodeType!];

        return new TestingJourney(startNode, graph, ct);
    }

}

public sealed class NodeBuilder
{
    private readonly JourneyBuilder _parent;
    private readonly Type _type;
    private readonly List<(Type neighbor, int weight)> _edges = [];
    private bool _auditValue = true;
    private INodeSpy? _instance;

    public NodeBuilder(JourneyBuilder parent, Type type)
    {
        _parent = parent;
        _type = type;
    }

    public NodeBuilder WithEdge<TNeighbor>(int weight = 0) where TNeighbor : INode =>
        WithEdge(typeof(TNeighbor), weight);

    public NodeBuilder WithEdge(Type neighborType, int weight = 0)
    {
        _edges.Add((neighborType, weight));
        return this;
    }

    public NodeBuilder WithAudit(bool value)
    {
        _auditValue = value;
        return this;
    }

    public INodeSpy CreateNode(ImmutableHashSet<IEdge> edges)
    {
        var ctor = _type.GetConstructor([typeof(ImmutableHashSet<IEdge>), typeof(bool)])
                   ?? throw new InvalidOperationException($"Constructor not found for {_type.Name}");

        _instance = (INodeSpy)ctor.Invoke([edges, _auditValue]);
        return _instance;
    }

    public ImmutableHashSet<IEdge> BuildEdges()
    {
        var origin = _type;
        var edges = _edges.Select(tuple =>
        {
            var (neighbor, weight) = tuple;

            static Task<INode> MoveNext(IJourney _, CancellationToken __) =>
                throw new InvalidOperationException("Edge not resolved yet.");

            return new Edge(origin, neighbor, MoveNext, weight);
        }).ToImmutableHashSet<IEdge>();

        return edges;
    }

    public void ResolveMoveNext(Dictionary<Type, INodeSpy> map)
    {
        ArgumentNullException.ThrowIfNull(map, nameof(map));
        if (_instance is null)
            throw new InvalidOperationException("Node instance not created yet.");

        var newEdges = _instance.InternalEdges
        .Select(e => (IEdge)new Edge(
            e.Origin,
            e.Neighbor,
            (_, _) => Task.FromResult<INode>(map[e.Neighbor]),
            e.Weight
        ))
        .ToImmutableHashSet();

        var ctor = _type.GetConstructor([typeof(ImmutableHashSet<IEdge>), typeof(bool)])
                   ?? throw new InvalidOperationException("Node must have expected constructor.");

        var newNode = (INodeSpy)ctor.Invoke([newEdges, _auditValue]);
        map[_type] = newNode;
    }

    public JourneyBuilder Done() => _parent;
}
