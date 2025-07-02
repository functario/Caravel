namespace Caravel.Core.UnitTests.Tests.NodeAction.BeforeAnyNavigation;

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
        ArgumentNullException.ThrowIfNull(map);

        // Act
        var sut = await journey
            .DoAsync<NodeSpy1, NodeSpy2>((node, ct) => Task.FromResult(map.NodeSpy2))
            .GotoAsync<NodeSpy3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagram(isDescriptionDisplayed: true);
        await result.VerifyMermaidMarkdownAsync();
    }
}
