using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Tests.Fixtures.GraphsData.Nodes;

namespace Caravel.Tests.Fixtures.GraphsData;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1065:Do not raise exceptions in unexpected locations",
    Justification = "<Pending>"
)]
public sealed class MapA : Map
{
    private readonly Lazy<HashSet<INode>> _nodes;
    private readonly Lazy<IGraph> _graph;

    public MapA(Lazy<HashSet<INode>> nodes, Lazy<IGraph> graph)
        : base(nodes, graph)
    {
        _nodes = nodes;
        _graph = graph;
    }

    public NodeA NodeA => throw new NotImplementedException();
    public NodeB NodeB => throw new NotImplementedException();
}
