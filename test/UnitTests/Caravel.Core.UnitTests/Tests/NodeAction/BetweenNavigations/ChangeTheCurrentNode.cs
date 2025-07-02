namespace Caravel.Core.UnitTests.Tests.NodeAction.BetweenNavigations;

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
            .WithEdge<NodeSpy4>()
            .Done()
            .AddNode<NodeSpy4>()
            .Done();

        var journey = builder.Build();
        var map = builder.Map;
        ArgumentNullException.ThrowIfNull(map);

        // Act
        var sut = await journey
            .GotoAsync<NodeSpy2>()
            .DoAsync<NodeSpy2, NodeSpy3>((node, ct) => Task.FromResult(map.NodeSpy3))
            .GotoAsync<NodeSpy4>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagram(isDescriptionDisplayed: true);
        await result.VerifyMermaidMarkdownAsync();
    }
}
