namespace Caravel.Core.UnitTests.Tests.FormatGraph;

[Trait(TestType, Unit)]
[Trait(FeatureUnderTest, FeatureFormatGraph)]
public class AsMermaidMarkdown
{
    [Theory(DisplayName = "Journey of 2 routes displays graph with weights")]
    [InlineData(50, 100)]
    [InlineData(100, 50)]
    public async Task Test1(int node3Weight, int node4Weight)
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .Done()
            .AddNode<NodeSpy2>()
            .WithEdge<NodeSpy3>(node3Weight) // The weight setting the route
            .WithEdge<NodeSpy4>(node4Weight) // The weight setting the route
            .Done()
            .AddNode<NodeSpy3>()
            .WithEdge<NodeSpy5>()
            .Done()
            .AddNode<NodeSpy4>()
            .WithEdge<NodeSpy5>()
            .Done()
            .AddNode<NodeSpy5>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = journey.ToMermaidGraph();

        // Assert
        await sut.VerifyMermaidMarkdownAsync(node3Weight, node4Weight);
    }
}
