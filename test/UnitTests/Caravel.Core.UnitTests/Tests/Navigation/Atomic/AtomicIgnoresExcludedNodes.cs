using Caravel.Tests.Fixtures.FixedJourneys;

namespace Caravel.Core.UnitTests.Tests.Navigation.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, ExcludedNodesDomain)]
public class AtomicIgnoresExcludedNodes
{
    [Theory(DisplayName = "Whith single excluded node")]
    [InlineData(typeof(Node2))]
    [InlineData(typeof(Node3))]
    public async Task Test1(Type excludedNode)
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node4>()
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node4>()
            .Done()
            .AddNode<Node4>()
            .Done();

        var journey = builder.Build();
        ExcludedNodes excludedNodes = [excludedNode];

        // Act
        var sut = await journey.GotoAsync<Node4>(excludedNodes);

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync().UseParameters(excludedNode.Name);
    }

    [Fact(DisplayName = "Whith many excluded nodes")]
    public async Task Test2()
    {
        // Arrange
        var journey = JourneyFixtures.JourneyWithJoinRightFractalGraph3Levels.Build();
        ExcludedNodes excludedNodes = [typeof(Node3), typeof(Node7)];

        // Act
        var sut = await journey.GotoAsync<Node14>(excludedNodes);

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "Whith single weighted excluded nodes")]
    public async Task Test3()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>(99)
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node4>(99)
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node4>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node6>(99)
            .Done()
            .AddNode<Node5>()
            .WithEdge<Node6>()
            .Done()
            .AddNode<Node6>()
            .Done();

        var journey = builder.Build();
        ExcludedNodes excludedNodes = [typeof(Node3)];

        // Act
        var sut = await journey.GotoAsync<Node6>(excludedNodes);

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "Whith many weighted excluded nodes")]
    public async Task Test4()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>(99)
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node4>(99)
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node4>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node6>(99)
            .Done()
            .AddNode<Node5>()
            .WithEdge<Node6>()
            .Done()
            .AddNode<Node6>()
            .Done();

        var journey = builder.Build();
        ExcludedNodes excludedNodes = [typeof(Node3), typeof(Node5)];

        // Act
        var sut = await journey.GotoAsync<Node6>(excludedNodes);

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync();
    }
}
