namespace Caravel.Core.UnitTests.Tests.GetHistory;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
public class WhenDirectJourney
{
    [Fact(DisplayName = "History shows only shortest path to 5 Nodes on 5")]
    public async Task Test1()
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
            .WithEdge<Node4>()
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node5>()
            .Done();

        var journey = builder.Build();
        var pastJourney = await journey.GotoAsync<Node5>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagramMarkdownAsync();

        // Assert
        await sut.VerifyMermaidMarkdownAsync();
    }

    [Fact(
        DisplayName = "History shows only shortest path when destination is the 3rd Node on 5 Nodes"
    )]
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
            .WithEdge<Node4>()
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node5>()
            .Done();

        var journey = builder.Build();
        var pastJourney = await journey.GotoAsync<Node3>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagramMarkdownAsync();

        // Assert
        await sut.VerifyMermaidMarkdownAsync();
    }

    [Theory(DisplayName = "History shows only shortest path when Journey of 2 routes with weights")]
    [InlineData(100, 50)]
    [InlineData(50, 100)]
    public async Task Test3(int node3Weight, int node4Weight)
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>(node3Weight) // The weight setting the route
            .WithEdge<Node4>(node4Weight) // The weight setting the route
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node5>()
            .Done();

        var journey = builder.Build();
        var pastJourney = await journey.GotoAsync<Node5>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagramMarkdownAsync();

        // Assert
        await sut.VerifyMermaidMarkdownAsync(node3Weight, node4Weight);
    }

    [Fact(DisplayName = "History shows all navigations")]
    public async Task Test4()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>(description: "Open Node2")
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node4>()
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node5>()
            .WithEdge<Node1>()
            .Done()
            .AddNode<Node5>()
            .WithEdge<Node4>()
            .Done();

        var journey = builder.Build();
        // csharpier-ignore
        var pastJourney = await journey
            .GotoAsync<Node5>()
            .GotoAsync<Node1>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);

        // Assert
        await sut.VerifyMermaidMarkdownAsync();
    }
}
