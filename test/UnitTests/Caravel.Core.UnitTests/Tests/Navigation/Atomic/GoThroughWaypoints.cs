using Caravel.Tests.Fixtures.FixedJourneys;

namespace Caravel.Core.UnitTests.Tests.Navigation.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, WaypointsDomain)]
public class GoThroughWaypoints
{
    [Theory(DisplayName = "Whith single waypoint")]
    [InlineData(typeof(NodeSpy2))]
    [InlineData(typeof(NodeSpy3))]
    public async Task Test1(Type waypoint)
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .WithEdge<NodeSpy3>()
            .Done()
            .AddNode<NodeSpy2>()
            .WithEdge<NodeSpy4>()
            .Done()
            .AddNode<NodeSpy3>()
            .WithEdge<NodeSpy4>()
            .Done()
            .AddNode<NodeSpy4>()
            .Done();

        var journey = builder.Build();
        Waypoints waypoints = [waypoint];

        // Act
        var sut = await journey.GotoAsync<NodeSpy4>(waypoints);

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdown(WithQuadrant);
        await result.VerifyMermaidMarkdownAsync().UseParameters(waypoint.Name);
    }

    [Fact(DisplayName = "Whith many waypoints")]
    public async Task Test2()
    {
        // Arrange
        var journey = JourneyMany.JourneyWithJoinRightFractalGraph3Levels.Build();
        Waypoints waypoints =
        [
            typeof(NodeSpy2),
            typeof(NodeSpy3),
            typeof(NodeSpy6),
            typeof(NodeSpy12),
        ];

        // Act
        var sut = await journey.GotoAsync<NodeSpy14>(waypoints);

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdown(WithQuadrant);
        await result.VerifyMermaidMarkdownAsync();
    }
}
