using AwesomeAssertions.Execution;
using Caravel.Abstractions;
using Caravel.Core.Extensions;
using Caravel.Tests.Fixtures.GraphsData;
using Caravel.Tests.Fixtures.GraphsData.Nodes;

namespace Caravel.Core.UnitTests.Tests.GraphTests;

[Trait(TestType, Unit)]
public class WaypointsTests
{
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
        var graphData = new Graph_5_Nodes_WithWeight();
        var graph = graphData.Graph;
        var nodeA = new NodeA();
        var journey = new Journey(nodeA, graph, CancellationToken.None);
        using var localCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(10));

        // Act
        var sut = await journey
            .GotoAsync<NodeC>(waypoints: [typeof(NodeD)], localCancellationTokenSource.Token);

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
    [Fact(DisplayName = "GotoAsync with waypoints and excluded nodes")]
    public async Task Test7()
    {
        // csharpier-ignore-start
        // Arrange
        var graphData = new Graph_5_Nodes_WithWeight();
        var graph = graphData.Graph;
        var nodeA = new NodeA();
        var journey = new Journey(nodeA, graph, CancellationToken.None);
        using var localCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        Type[] expectedHistory = [typeof(NodeA),
                typeof(NodeD),
                typeof(NodeC),
                typeof(NodeE),
                typeof(NodeA),
                typeof(NodeD),
                typeof(NodeC)];

        // Act
        var sut = await journey
            .GotoAsync<NodeC>([typeof(NodeD)], [typeof(NodeB)], localCancellationTokenSource.Token)
            .GotoAsync<NodeA>([typeof(NodeE)], [typeof(NodeB)], localCancellationTokenSource.Token)
            .GotoAsync<NodeC>(waypoints: [typeof(NodeD)], localCancellationTokenSource.Token);

        // Assert
        using var scope = new AssertionScope();
        sut.Log.History.Count.Should()
            .Be(expectedHistory.Length);

        sut.Log.History.Should()
            .ContainInConsecutiveOrder(expectedHistory);
        // csharpier-ignore-end
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Usage",
    "xUnit1051:Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken",
    Justification = "<Pending>"
)]
    [Fact(DisplayName = "GotoAsync with excluded nodes")]
    public async Task Test3()
    {
        // csharpier-ignore-start
        // Arrange
        var graphData = new Graph_5_Nodes_WithWeight();
        var graph = graphData.Graph;
        var nodeA = new NodeA();
        var journey = new Journey(nodeA, graph, CancellationToken.None);
        using var localCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(10));

        // Act
        var sut = await journey
            .GotoAsync<NodeC>(excludedNodes: [typeof(NodeB)], localCancellationTokenSource.Token);

        // Assert
        sut.Log.History.Should()
            .ContainInConsecutiveOrder(
                [typeof(NodeA),
                typeof(NodeD),
                typeof(NodeC)]
            );
        // csharpier-ignore-end
    }



    [Fact(DisplayName = "Throws exception when excluded nodes contains waypoints")]
    public async Task Test8()
    {
        // csharpier-ignore-start
        // Arrange
        var graphData = new Graph_5_Nodes_WithWeight();
        var graph = graphData.Graph;
        var nodeA = new NodeA();
        var journey = new Journey(nodeA, graph, CancellationToken.None);
        using var localCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        // Act
        Func<Task<IJourney>> sut = async () => await journey
            .GotoAsync<NodeC>(waypoints: [typeof(NodeD)], excludeNodes: [typeof(NodeD)])
            .ConfigureAwait(false);

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Some waypoints are excluded.");
        // csharpier-ignore-end
    }
}
