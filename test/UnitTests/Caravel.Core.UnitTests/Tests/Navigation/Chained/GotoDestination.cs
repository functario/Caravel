namespace Caravel.Core.UnitTests.Tests.Navigation.Chained;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, GotoDomain)]
public class GotoDestination
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
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .Done()
            .Build();
        // Act
        // csharpier-ignore
        var sut = await journey
            .GotoAsync<Node2>()
            .GotoAsync<Node2>() // self reference
            .GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithQuadrant);
        await result.VerifyMermaidMarkdownAsync();
    }
}
