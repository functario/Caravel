namespace Caravel.Core.UnitTests.Tests.Navigation.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, GotoDomain)]
public class AtomicGotoDestination
{
    [Fact(
        DisplayName = "When origin is also destination without waypoints or explicit self reference edge"
    )]
    public async Task Test1()
    {
        // Arrange
        // csharpier-ignore
        // Warning: A node cannot reference itself via a Edge (throws InvalidEdgeException)!
        var journey = new JourneyBuilder()
            .AddNode<Node1>()
            .Done()
            .Build();

        // Act
        var sut = await journey.GotoAsync<Node1>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithQuadrant);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When origin is also destination with waypoints")]
    public async Task Test2()
    {
        // Arrange
        // csharpier-ignore
        Waypoints waypoints = [typeof(Node2), typeof(Node3)];
        var journey = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node1>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node2>()
            .Done()
            .Build();

        // Act
        var sut = await journey.GotoAsync<Node1>(waypoints);

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithQuadrant);
        await result.VerifyMermaidMarkdownAsync();
    }
}
