namespace Caravel.Core.UnitTests.Tests.NodeAction.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNodeAction)]
[Trait(Domain, NodeDomain)]
public class DoesNotChangeTheCurrentNode
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
            .DoAsync<Node1>((node, ct) => Task.FromResult(node))
            .GotoAsync<Node2>()
            .GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When action (with journey) is done on current node")]
    public async Task Test2()
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
            .DoAsync<Node1>((journey, node, ct) => Task.FromResult(node))
            .GotoAsync<Node2>()
            .GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }
}
