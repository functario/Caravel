namespace Caravel.Core.UnitTests.Tests.Navigation.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, GotoDomain)]
public class AtomicOverrideStartingNode
{
    [Fact(DisplayName = "When starting the journey")]
    public async Task Test1()
    {
        // Arrange
        // csharpier-ignore
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
        // OverrideStartingNode is only available when starting the Journey!
        var sut = await journey
            .OverrideStartingNode(journeyBuilder.Map.NodeSpy2)
            .GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithQuadrant);
        await result.VerifyMermaidMarkdownAsync();
    }
}
