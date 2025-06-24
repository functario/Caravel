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
        var graphData = new Graph_4_Nodes_WithWeight();
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
    [Fact(DisplayName = "GotoAsync with waypoints")]
    public async Task Test2()
    {
        // csharpier-ignore-start
        // Arrange
        var graphData = new Graph_4_Nodes_WithWeight();
        var graph = graphData.Graph;
        var nodeA = new NodeA();
        var journey = new Journey(nodeA, graph, TestContext.Current.CancellationToken);
        using var localCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(10));

        // Act
        var sut = await journey
            .GotoAsync<NodeC>([typeof(NodeD)], localCancellationTokenSource.Token);

        // Assert
        sut.Log.History.Should()
            .ContainInConsecutiveOrder(
                [typeof(NodeA),
                typeof(NodeD),
                typeof(NodeC)]
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
        var graphData = new Graph_4_Nodes_WithWeight();
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Usage",
        "xUnit1051:Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken",
        Justification = "<Pending>"
    )]
    [Fact(DisplayName = "GotoAsync")]
    public async Task Test4()
    {
        // csharpier-ignore-start
        // Arrange
        var graphData = new Graph_4_Nodes_WithWeight();
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

    [Fact(
        DisplayName = $"IAuditableNode GotoAsync throws exception if does not exist in the current context of journey."
    )]
    public async Task Test5()
    {
        // Arrange
        var graphData = new Graph_4_Nodes_WithWeight();
        var graph = graphData.Graph;
        var nodeA = new NodeA(false);
        var journey = new Journey(nodeA, graph, TestContext.Current.CancellationToken);

        // Act
        var sut = async () =>
        {
            await journey
                .GotoAsync<NodeB>(TestContext.Current.CancellationToken)
                .ConfigureAwait(false);
        };

        await sut.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("The Node audit has failed");
    }

    [Fact(
        DisplayName = $"IAuditableNode DoAsync throws exception if does not exist in the current context of journey."
    )]
    public async Task Test6()
    {
        // Arrange
        var graphData = new Graph_4_Nodes_WithWeight();
        var graph = graphData.Graph;
        var nodeA = new NodeA();
        var journey = new Journey(nodeA, graph, TestContext.Current.CancellationToken);

        // Act
        var sut = async () =>
        {
            await journey
                .GotoAsync<NodeC>(TestContext.Current.CancellationToken)
                .DoAsync<NodeC>(
                    (n, _) =>
                    {
                        return Task.FromResult(new NodeC(false));
                    }
                )
                .ConfigureAwait(false);
        };

        await sut.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("The Node audit has failed");
    }
}
