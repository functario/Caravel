using Caravel.Tests.Fixtures.FixedJourneys;

namespace Caravel.Core.UnitTests.Tests.Navigation.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, GotoDomain)]
public class AtomicGotoDestination
{
    [Fact(
        DisplayName = $"When origin is also destination without {nameof(Waypoints)} or explicit self reference edge"
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
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = $"When origin is also destination with {nameof(Waypoints)}")]
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
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = $"Whith {nameof(Waypoints)} and {nameof(ExcludedWaypoints)}")]
    public async Task Test3()
    {
        // Arrange
        // csharpier-ignore
        Waypoints waypoints = [typeof(Node6), typeof(Node14)];
        ExcludedWaypoints excludedWaypoints = [typeof(Node3), typeof(Node7)];
        var journey = JourneyFixtures.JourneyWithJoinRightFractalGraph3Levels.Build();

        // Act
        var sut = await journey.GotoAsync<Node15>(waypoints, excludedWaypoints);

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync();
    }
}
