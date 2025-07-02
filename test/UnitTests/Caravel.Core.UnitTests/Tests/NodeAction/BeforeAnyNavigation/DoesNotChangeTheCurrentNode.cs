namespace Caravel.Core.UnitTests.Tests.NodeAction.BeforeAnyNavigation;

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
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .Done()
            .AddNode<NodeSpy2>()
            .WithEdge<NodeSpy3>()
            .Done()
            .AddNode<NodeSpy3>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = await journey
            .DoAsync<NodeSpy1>((node, ct) => Task.FromResult(node))
            .GotoAsync<NodeSpy2>()
            .GotoAsync<NodeSpy3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagram(isDescriptionDisplayed: true);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When action (with journey) is done on current node")]
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

        // Act
        var sut = await journey
            .DoAsync<NodeSpy1>((journey, node, ct) => Task.FromResult(node))
            .GotoAsync<NodeSpy2>()
            .GotoAsync<NodeSpy3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagram(isDescriptionDisplayed: true);
        await result.VerifyMermaidMarkdownAsync();
    }
}
