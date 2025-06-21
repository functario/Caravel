//using System.Collections.Frozen;
//using System.Collections.Generic;
//using AutoFixture;
//using Caravel.Abstractions;
//using Caravel.Core;

//namespace Caravel.Tests.Fixtures.GraphsData;

//public sealed class Graph_3_Nodes_NoWeight
//{
//    private readonly Graph _graph;

//    public Graph_3_Nodes_NoWeight()
//    {
//        var fixture = CaravelDataAttribute.CreateFixture();
//        var node1 = fixture.Create<INode>();
//        var node2 = fixture.Create<INode>();
//        var node3 = fixture.Create<INode>();

//        var map = new Dictionary<INode, List<IEdge>>
//        {
//            { node1, [new(node1, node2)] },
//            { node2, [new(node2, node3)] },
//            { node3, [new(node3, node2)] },
//        };

//        _graph = new Graph(map.ToFrozenDictionary());
//    }

//    public Graph Graph => _graph;
//    //public string ShortestPath = "";
//}
