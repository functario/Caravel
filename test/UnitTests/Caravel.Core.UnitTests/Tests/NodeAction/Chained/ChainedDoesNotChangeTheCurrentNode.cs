namespace Caravel.Core.UnitTests.Tests.NodeAction.Chained;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNodeAction)]
[Trait(Domain, NodeDomain)]
public class ChainedDoesNotChangeTheCurrentNode
{
    [Fact(DisplayName = "When action is done on current node")]
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
            .Done();

        var journey = builder.Build();

        // Act
        var sut = await journey
            .GotoAsync<Node2>()
            .DoAsync<Node2>((node, ct) => Task.FromResult(node))
            .DoAsync<Node2>((journey, node, ct) => Task.FromResult(node))
            .GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }
}
