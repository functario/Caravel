using AwesomeAssertions.Execution;
using Caravel.Core.Extensions;
using Caravel.Tests.Fixtures;
using Caravel.Tests.Fixtures.NodeSpies;

namespace Caravel.Core.UnitTests.Tests.GotoAsync;

[Trait(TestType, Unit)]
[Trait(FeatureUnderTest, FeatureGotoAsync)]
public class WhenNoWaypointsOrExcludedNodes
{
    [Fact(DisplayName = "Journey of 5 Nodes travels the shortest route to the 5th Node")]
    public async Task Test1()
    {
        // csharpier-ignore-start
        // Arrange
        var builder = new JourneyBuilder()
                .AddNode<NodeSpy1>()
                .WithEdge<NodeSpy2>()
                .Done()
                .AddNode<NodeSpy2>()
                .WithEdge<NodeSpy3>()
                .Done()
                .AddNode<NodeSpy3>()
                .WithEdge<NodeSpy4>()
                .Done()
                .AddNode<NodeSpy4>()
                .WithEdge<NodeSpy5>()
                .Done()
                .AddNode<NodeSpy5>()
                .Done();

        var journey = builder.Build();

        // Act
        var sut = await journey.GotoAsync<NodeSpy5>();

        // Assert
        using var _ = new AssertionScope();
        sut.Log.History.Count.Should().Be(5);
        sut.Log.History.Should()
            .ContainInConsecutiveOrder(
                [typeof(NodeSpy1),
                typeof(NodeSpy2),
                typeof(NodeSpy3),
                typeof(NodeSpy4),
                typeof(NodeSpy5)]
            );

        // csharpier-ignore-end
    }

    [Fact(DisplayName = "Journey of 5 Nodes travels the shortest route to the 3rd Node")]
    public async Task Test2()
    {
        // csharpier-ignore-start
        // Arrange
        var builder = new JourneyBuilder()
                .AddNode<NodeSpy1>()
                .WithEdge<NodeSpy2>()
                .Done()
                .AddNode<NodeSpy2>()
                .WithEdge<NodeSpy3>()
                .Done()
                .AddNode<NodeSpy3>()
                .WithEdge<NodeSpy4>()
                .Done()
                .AddNode<NodeSpy4>()
                .WithEdge<NodeSpy5>()
                .Done()
                .AddNode<NodeSpy5>()
                .Done();

        var journey = builder.Build();

        // Act
        var sut = await journey.GotoAsync<NodeSpy3>();

        // Assert
        using var _ = new AssertionScope();
        sut.Log.History.Count.Should().Be(3);
        sut.Log.History.Should()
            .ContainInConsecutiveOrder(
                [typeof(NodeSpy1),
                typeof(NodeSpy2),
                typeof(NodeSpy3)]
            );

        // csharpier-ignore-end
    }

    [Theory(DisplayName = "Journey of 2 routes with weights travels the shortest route to the 5th Node")]
    [InlineData(100, 50, typeof(NodeSpy4))]
    [InlineData(50, 100, typeof(NodeSpy3))]
    public async Task Test3(int node3Weight, int node4Weight, Type expected)
    {
        // csharpier-ignore-start
        // Arrange
        var builder = new JourneyBuilder()
                .AddNode<NodeSpy1>()
                .WithEdge<NodeSpy2>()
                .Done()
                .AddNode<NodeSpy2>()
                .WithEdge<NodeSpy3>(node3Weight) // The weight setting the route
                .WithEdge<NodeSpy4>(node4Weight) // The weight setting the route
                .Done()
                .AddNode<NodeSpy3>()
                .WithEdge<NodeSpy5>()
                .Done()
                .AddNode<NodeSpy4>()
                .WithEdge<NodeSpy5>()
                .Done()
                .AddNode<NodeSpy5>()
                .Done();

        var journey = builder.Build();

        // Act
        var sut = await journey.GotoAsync<NodeSpy5>();

        // Assert
        using var _ = new AssertionScope();
        sut.Log.History.Count.Should().Be(4);
        sut.Log.History.Should()
            .ContainInConsecutiveOrder(
                [typeof(NodeSpy1),
                typeof(NodeSpy2),
                expected,
                typeof(NodeSpy5)]
            );

        // csharpier-ignore-end
    }
}
