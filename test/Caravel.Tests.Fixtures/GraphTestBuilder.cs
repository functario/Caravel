using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Graph.Dijkstra;
using NSubstitute;

namespace Caravel.Tests.Fixtures;

public sealed class GraphTestBuilder
{
    private readonly Dictionary<INode, List<IEdge>> _edgesByNode = [];
    private readonly Dictionary<INode, Task<bool>> _audits = [];
    private readonly List<INode> _nodes = [];

    public INode AddNode<T>() where T : class
    {
        var node = Substitute.For<INode, T>();

        // Initially empty edges
        node.GetEdges()
            .Returns(_ => [.. (_edgesByNode.TryGetValue(node, out var list) ? list : [])]);

        _nodes.Add(node);
        _edgesByNode[node] = [];
        return node;
    }

    public IAuditableNode AddAuditableNode<T>() where T : class
    {
        var node = Substitute.For<IAuditableNode, T>();
        node.Audit(Arg.Any<IJourney>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
                _audits.TryGetValue(node, out var result) ? result : Task.FromResult(true)
            );

        // Initially empty edges
        node.GetEdges()
            .Returns(_ => [.. (_edgesByNode.TryGetValue(node, out var list) ? list : [])]);

        _nodes.Add(node);
        _edgesByNode[node] = [];
        return node;
    }

    public GraphTestBuilder SetAuditFor(IAuditableNode node, bool result)
    {
        _audits[node] = Task.FromResult(result);
        return this;
    }

    public GraphTestBuilder SetEdges(INode origin, INode neighbor, int weight)
    {
        ArgumentNullException.ThrowIfNull(origin, nameof(origin));
        if (!_edgesByNode.ContainsKey(origin))
            throw new InvalidOperationException(
                $"Node '{origin.GetType().DeclaringType}' was not created with CreateNode."
            );

        var edge = AddEdge(origin, neighbor, weight);

        _edgesByNode[origin] = [edge];

        origin.GetEdges().Returns(_ => [.. _edgesByNode[origin]]);

        return this;
    }

    public static IEdge AddEdge(INode origin, INode neighbor, int weight = 1)
    {
        ArgumentNullException.ThrowIfNull(origin, nameof(origin));
        ArgumentNullException.ThrowIfNull(neighbor, nameof(neighbor));
        var edge = Substitute.For<IEdge>();
        edge.Origin.Returns(origin.GetType());
        edge.Neighbor.Returns(neighbor.GetType());
        edge.Weight.Returns(weight);
        edge.MoveNext.Returns((IJourney _, CancellationToken _) => Task.FromResult(neighbor));
        return edge;
    }

    public IJourney CreateJourney(INode start, CancellationToken cancellationToken = default)
    {
        var journey = new Journey(start, CreateGraph(), cancellationToken);
        return journey;
    }

    public IGraph CreateGraph()
    {
        var graph = new DijkstraGraph(_nodes);
        return graph;
    }
}
