namespace Caravel.Core.UnitTests.Tests.Navigation.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, GotoDomain)]
public class AtomicSetStartingNode
{
    [Fact(DisplayName = "Before starting the journey")]
    public async Task Test1()
    {
        // Arrange
        var journeyBuilder = new JourneyBuilder()
            .AddNode<Node1>() // Node1 does not have edges. It will throws exception if the starting node.
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .Done();

        var journey = journeyBuilder.Build();

        // Act
        var sut = await journey.SetStartingNode(journeyBuilder.Map.NodeSpy2).GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithQuadrant);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "Twice before starting the journey")]
    public async Task Test2()
    {
        // Arrange
        var journeyBuilder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .Done();

        var journey = journeyBuilder.Build();

        // Act
        // Starting Node can be changed as many times as desired
        // until the journey starts.
        var sut = await journey
            .SetStartingNode(journey.Map.NodeSpy2)
            .SetStartingNode(journey.Map.NodeSpy1) // Set back to Node1
            .GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithQuadrant);
        await result.VerifyMermaidMarkdownAsync();
    }
}
