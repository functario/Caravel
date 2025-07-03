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

    [Fact(DisplayName = "When origin is also destination with waypoints")]
    public async Task Test2()
    {
        // Arrange
        // csharpier-ignore-start
        Waypoints waypoints = [typeof(Node1), typeof(Node3)];
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
        var sut = await journey
            .GotoAsync<Node2>() // In this test, Node2 is origin of the second GotoAsync
            .GotoAsync<Node2>(waypoints); // self reference

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithQuadrant);
        await result.VerifyMermaidMarkdownAsync();
        // csharpier-ignore-stop
    }
}
