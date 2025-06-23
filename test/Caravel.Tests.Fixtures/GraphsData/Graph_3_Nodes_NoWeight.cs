using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Tests.Fixtures.GraphsData.Nodes;

namespace Caravel.Tests.Fixtures.GraphsData;

public sealed class Graph_3_Nodes_NoWeight
{
    private readonly Graph _graph;
    private readonly ImmutableHashSet<INode> _nodes;

    public Graph_3_Nodes_NoWeight()
    {
        _nodes = [
            new NodeA(),
            new NodeB(),
            new NodeC()
            ];

        _graph = new Graph(_nodes);
    }

    public Graph Graph => _graph;
    public ImmutableHashSet<INode> Nodes => _nodes;
}
