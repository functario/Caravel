namespace Caravel.Core.UnitTests.Tests.NodeAction.Chained;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNodeAction)]
[Trait(Domain, NodeDomain)]
public class ChainedChangesTheCurrentNode
{
    [Fact(DisplayName = "When action returns a different node")]
    public async Task Test1()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node4>()
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node5>()
            .Done();

        var journey = builder.Build();
        var map = builder.Map;
        // Act
        // csharpier-ignore
        var sut = await journey
            .GotoAsync<Node2>()
            .DoAsync<Node2, Node3>((node, ct) => Task.FromResult(map.NodeSpy3))
            .DoAsync<Node3, Node4>(
                (journey, node, ct)
                => Task.FromResult(journey.OfType<SmartJourney>().Map.NodeSpy4)
            )
            .GotoAsync<Node5>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }
}
