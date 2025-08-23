using Caravel.Tests.Fixtures.FixedJourneys;

namespace Caravel.Core.UnitTests.Tests.Navigation.Chained;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, WaypointsDomain)]
public class ChainedGoThroughWaypoints
{
    [Theory(DisplayName = "Whith single waypoint")]
    [InlineData(typeof(Node4))]
    [InlineData(typeof(Node3))]
    public async Task Test1(Type waypoint)
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .WithEdge<Node4>()
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
        Waypoints waypoints = [waypoint];
        // Act
        // csharpier-ignore
        var sut = await journey
            .GotoAsync<Node2>()
            .GotoAsync<Node5>(waypoints);

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync().UseParameters(waypoint.Name);
    }

    [Fact(DisplayName = $"Whith many {nameof(Waypoints)}")]
    public async Task Test2()
    {
        // Arrange
        var journey = JourneyFixtures.JourneyWithJoinRightFractalGraph3Levels.Build();
        Waypoints waypoints = [typeof(Node3), typeof(Node6), typeof(Node12)];
        // Act
        // csharpier-ignore
        var sut = await journey
            .GotoAsync<Node2>()
            .GotoAsync<Node14>(waypoints);

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync();
    }
}
