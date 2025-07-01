namespace Caravel.Core.UnitTests.Tests.Navigation;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, EdgeDomain)]
public class UsesEdgesWithSmallestWeights
{
    [Fact(DisplayName = "When same edge with different weights")]
    public async Task Test1()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>(3)
            .WithEdge<NodeSpy2>(1)
            .WithEdge<NodeSpy2>(2)
            .Done()
            .AddNode<NodeSpy2>()
            .Done();

        var journey = builder.Build();
        var pastJourney = await journey.GotoAsync<NodeSpy2>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagram();

        // Assert
        await sut.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When edge configured with default values")]
    public async Task Test2()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .Done()
            .AddNode<NodeSpy2>()
            .Done();

        var journey = builder.Build();
        var pastJourney = await journey.GotoAsync<NodeSpy2>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagram();

        // Assert
        await sut.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When edges configured with specific and default values")]
    public async Task Test3()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>(1)
            .WithEdge<NodeSpy2>(99)
            .Done()
            .AddNode<NodeSpy2>()
            .WithEdge<NodeSpy3>(99)
            .WithEdge<NodeSpy3>()
            .Done()
            .AddNode<NodeSpy3>()
            .WithEdge<NodeSpy4>()
            .Done()
            .AddNode<NodeSpy4>()
            .Done();

        var journey = builder.Build();
        var pastJourney = await journey
            .GotoAsync<NodeSpy2>()
            .GotoAsync<NodeSpy3>()
            .GotoAsync<NodeSpy4>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagram();

        // Assert
        await sut.VerifyMermaidMarkdownAsync();
    }
}
