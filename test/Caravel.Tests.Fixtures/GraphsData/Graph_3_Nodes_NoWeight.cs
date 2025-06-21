using System.Collections.Frozen;
using System.Collections.Generic;
using AutoFixture;
using Caravel.Abstractions;

namespace Caravel.Tests.Fixtures.GraphsData;

public sealed class Graph_3_Nodes_NoWeight
{
    private readonly Graph _graph;

    public Graph_3_Nodes_NoWeight()
    {
        var fixture = CaravelDataAttribute.CreateFixture();
        var node1 = fixture.Create<Node>();
        var node2 = fixture.Create<Node>();
        var node3 = fixture.Create<Node>();

        var map = new Dictionary<Node, List<Edge>>
        {
            { node1, [new(node1, node2)] },
            { node2, [new(node2, node3)] },
            { node3, [new(node3, node2)] },
        };

        _graph = new Graph(map.ToFrozenDictionary());
    }

    public Graph Graph => _graph;
    //public string ShortestPath = "";
}
