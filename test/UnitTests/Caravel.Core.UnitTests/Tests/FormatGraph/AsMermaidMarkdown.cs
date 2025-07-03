namespace Caravel.Core.UnitTests.Tests.FormatGraph;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureFormatGraph)]
public class AsMermaidMarkdown
{
    [Theory(DisplayName = "Journey of 2 routes displays graph with weights")]
    [InlineData(50, 100)]
    [InlineData(100, 50)]
    public async Task Test1(int node3Weight, int node4Weight)
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>(description: "Open Node2")
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>(node3Weight) // The weight setting the route
            .WithEdge<Node4>(node4Weight) // The weight setting the route
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node5>(description: "Open Node5")
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node5>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = journey.ToMermaidMarkdown(WithDescription);

        // Assert
        await sut.VerifyMermaidMarkdownAsync(node3Weight, node4Weight);
    }
}
