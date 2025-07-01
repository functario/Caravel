namespace Caravel.Core.UnitTests.Tests.Navigation;

[Trait(TestType, Unit)]
[Trait(FeatureUnderTest, FeatureGotoAsync)]
public class NavigationTests
{
    [Fact(DisplayName = "Navigation prefers the edge with smallest weight")]
    public async Task Test1()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>(3)
            .WithEdge<NodeSpy2>(1)
            .WithEdge<NodeSpy2>(2)
            .Done()
            .AddNode<NodeSpy2>()
            .Done();

        var journey = builder.Build();
        var pastJourney = await journey.GotoAsync<NodeSpy2>();

        // Act
        var sut = await pastJourney.ToMermaidSequenceDiagram();

        // Assert
        await sut.VerifyMermaidMarkdownAsync();
    }
}
