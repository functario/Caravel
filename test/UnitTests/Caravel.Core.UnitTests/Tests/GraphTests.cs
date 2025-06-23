using System.Collections.Immutable;
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
        var edges = ImmutableList.Create(edgeAtoB, edgeBtoC);
        var expectedRoute = new Route(edges);

        // Act
        var sut = graph.GetShortestRoute(origin, destination);

        // Assert
        sut.GetPath().Should().BeEquivalentTo(expectedRoute.GetPath());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Usage",
        "xUnit1051:Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken",
        Justification = "<Pending>"
    )]
    [Fact(DisplayName = "GotoAsync")]
    public async Task Test2()
    {
        // csharpier-ignore-start
        // Arrange
        var graphData = new Graph_3_Nodes_NoWeight();
        var graph = graphData.Graph;
        var nodeA = new NodeA();
        var journey = new Journey(nodeA, graph, TestContext.Current.CancellationToken);
        using var localCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(10));

        // Act
        var sut = await journey
            .GotoAsync<NodeC>(localCancellationTokenSource.Token)
            .GotoAsync<NodeB>();

        // Assert
        sut.Log.History.Should()
            .ContainInConsecutiveOrder(
                [typeof(NodeA),
                typeof(NodeB),
                typeof(NodeC),
                typeof(NodeA),
                typeof(NodeB)]
            );
        // csharpier-ignore-end
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Usage",
        "xUnit1051:Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken",
        Justification = "<Pending>"
    )]
    [Fact(DisplayName = "DoAsync")]
    public async Task Test3()
    {
        // csharpier-ignore-start
        // Arrange
        var graphData = new Graph_3_Nodes_NoWeight();
        var graph = graphData.Graph;
        var nodeA = new NodeA();
        var journey = new Journey(nodeA, graph, TestContext.Current.CancellationToken);
        using var localCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(10));

        // Act
        var sut = await journey
            .GotoAsync<NodeC>()
            .DoAsync<NodeC>((n, _) =>
            {
                return Task.FromResult(n);
            }, localCancellationTokenSource.Token);

        // Assert
        sut.Current.GetType().Should().Be<NodeC>();
        // csharpier-ignore-end
    }
}
