using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Graph.Dijkstra;
using Caravel.Tests.Fixtures.GraphsData.Nodes;

namespace Caravel.Tests.Fixtures.GraphsData;

public sealed class Graph_5_Nodes_WithWeight
{
    private readonly DijkstraGraph _graph;
    private readonly ImmutableHashSet<INode> _nodes;

    public Graph_5_Nodes_WithWeight()
    {
        _nodes = [new NodeA(), new NodeB(), new NodeC(), new NodeD(), new NodeE()];

        _graph = new DijkstraGraph(_nodes);
    }

    public DijkstraGraph Graph => _graph;
    public ImmutableHashSet<INode> Nodes => _nodes;
}
