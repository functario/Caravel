namespace Caravel.Core.UnitTests.Tests.Navigation.Atomic;

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
            .Done()
            .Build();

        // Act
        var sut = await journey.GotoAsync<Node1>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithQuadrant);
        await result.VerifyMermaidMarkdownAsync();
    }
}
