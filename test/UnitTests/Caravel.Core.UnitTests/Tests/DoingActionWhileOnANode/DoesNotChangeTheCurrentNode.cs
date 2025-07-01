namespace Caravel.Core.UnitTests.Tests.DoingActionWhileOnANode;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[Trait(Domain, EdgeDomain)]
public class DoesNotChangeTheCurrentNode
{
    [Fact(DisplayName = "When action is done after a navigation")]
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
        var pastJourney = await journey
            .GotoAsync<NodeSpy2>()
            .DoAsync<NodeSpy2>((node, ct) => Task.FromResult(node))
            .GotoAsync<NodeSpy3>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagram();

        // Assert
        await sut.VerifyMermaidMarkdownAsync();
    }
}
