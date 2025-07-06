namespace Caravel.Core.UnitTests.Tests.Navigation.Chained;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, EdgeDomain)]
public class ChainedUsesEdgesWithSmallestWeights
{
    [Fact(DisplayName = "When same edge with different weights")]
    public async Task Test1()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>(3)
            .WithEdge<Node2>(1)
            .WithEdge<Node2>(2)
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>(3)
            .WithEdge<Node3>(1)
            .WithEdge<Node3>(2)
            .Done()
            .AddNode<Node3>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = await journey.GotoAsync<Node2>().GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync();
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When edge configured with default values")]
    public async Task Test2()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = await journey.GotoAsync<Node2>().GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync();
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When edges configured with specific and default values")]
    public async Task Test3()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .WithEdge<Node2>(1)
            .WithEdge<Node2>(99)
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>(99)
            .WithEdge<Node3>()
            .WithEdge<Node3>(1)
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node4>(99)
            .WithEdge<Node4>(1)
            .Done()
            .AddNode<Node4>()
            .Done();

        var journey = builder.Build();
        // Act
        // csharpier-ignore
        var sut = await journey
            .GotoAsync<Node2>()
            .GotoAsync<Node4>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync();
        await result.VerifyMermaidMarkdownAsync();
    }
}
