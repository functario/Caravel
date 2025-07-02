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
        var pastJourney = await journey.GotoAsync<NodeSpy5>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagram();

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
        var pastJourney = await journey.GotoAsync<NodeSpy3>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagram();

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
        var pastJourney = await journey.GotoAsync<NodeSpy5>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagram();

        // Assert
        await sut.VerifyMermaidMarkdownAsync(node3Weight, node4Weight);
    }

    [Fact(DisplayName = "History shows all navigations")]
    public async Task Test4()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>(description: "Open Node2")
            .Done()
            .AddNode<NodeSpy2>()
            .WithEdge<NodeSpy3>()
            .Done()
            .AddNode<NodeSpy3>()
            .WithEdge<NodeSpy4>()
            .Done()
            .AddNode<NodeSpy4>()
            .WithEdge<NodeSpy5>()
            .WithEdge<NodeSpy1>()
            .Done()
            .AddNode<NodeSpy5>()
            .WithEdge<NodeSpy4>()
            .Done();

        var journey = builder.Build();
        // csharpier-ignore
        var pastJourney = await journey
            .GotoAsync<NodeSpy5>()
            .GotoAsync<NodeSpy1>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagram(true);

        // Assert
        await sut.VerifyMermaidMarkdownAsync();
    }
}
