namespace Caravel.Core.UnitTests.Tests.NodeAction.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNodeAction)]
[Trait(Domain, NodeDomain)]
public class ChangeTheCurrentNode
{
    [Fact(DisplayName = "When action returns a different node")]
    public async Task Test1()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .Done()
            .AddNode<NodeSpy2>()
            .WithEdge<NodeSpy3>()
            .Done()
            .AddNode<NodeSpy3>()
            .Done();

        var journey = builder.Build();
        var map = builder.Map;

        // Act
        var sut = await journey
            .DoAsync<NodeSpy1, NodeSpy2>((node, ct) => Task.FromResult(map.NodeSpy2))
            .GotoAsync<NodeSpy3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When action (with journey) returns a different node")]
    public async Task Test2()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .Done()
            .AddNode<NodeSpy2>()
            .WithEdge<NodeSpy3>()
            .Done()
            .AddNode<NodeSpy3>()
            .Done();

        var journey = builder.Build();
        var map = builder.Map;
        // Act
        // csharpier-ignore
        var sut = await journey
            .DoAsync<NodeSpy1, NodeSpy2>(
                (journey, node, ct)
                => Task.FromResult(journey.OfType<SmartJourney>().Map.NodeSpy2)
            )
            .GotoAsync<NodeSpy3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }
}
