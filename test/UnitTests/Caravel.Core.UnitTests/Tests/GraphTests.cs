using System.Collections.Frozen;
using Caravel.Core.Extensions;
using Caravel.Tests.Fixtures.GraphsData;
using Caravel.Tests.Fixtures.GraphsData.Nodes;

namespace Caravel.Core.UnitTests.Tests;

[Trait(TestType, Unit)]
public class GraphTests
{
    [Fact(DisplayName = "Get the shortest route without waypoints or weights")]
    public void Test1()
    {
        // Arrange
        var graphData = new Graph_3_Nodes_NoWeight();
        var graph = graphData.Graph;
        var edgeAtoB = graphData.Nodes.GetEdge<NodeA, NodeB>();
        var edgeBtoC = graphData.Nodes.GetEdge<NodeB, NodeC>();
        var origin = typeof(NodeA);
        var destination = typeof(NodeC);
        var edges = FrozenSet.Create(edgeAtoB, edgeBtoC);
        var expectedRoute = new Route(edges);

        // Act
        var sut = graph.GetShortestRoute(origin, destination);

        // Assert
        sut.Should().BeEquivalentTo(expectedRoute);
    }

    [Fact(DisplayName = "Run")]
    public void Test2()
    {
        // Arrange
        var graphData = new Graph_3_Nodes_NoWeight();
        var graph = graphData.Graph;
        var edgeAtoB = graphData.Nodes.GetEdge<NodeA, NodeB>();
        var edgeBtoC = graphData.Nodes.GetEdge<NodeB, NodeC>();
        var origin = typeof(NodeA);
        var destination = typeof(NodeC);
        var edges = FrozenSet.Create(edgeAtoB, edgeBtoC);
        var expectedRoute = new Route(edges);
        var route = graph.GetShortestRoute(origin, destination);

        // Act
        var sut = route.Edges.Select(e => e.GetNext);
        foreach (var next in sut)
        {
            next(CancellationToken.None);
        }

        // Assert
        //sut.Should().BeEquivalentTo(expectedRoute);
    }
}
