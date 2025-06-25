using Caravel.Core.Extensions;
using Caravel.Tests.Fixtures;
using Caravel.Tests.Fixtures.NodeSpies;

namespace Caravel.Core.UnitTests.Tests.FormatGraph;

[Trait(TestType, Unit)]
[Trait(FeatureUnderTest, FeatureFormatGraph)]
public class AsMermaidGraph
{
    private const string GraphNode4Win =
        """
        graph TD
        NodeSpy1 -->|0| NodeSpy2
        NodeSpy2 -->|100| NodeSpy3
        NodeSpy2 -->|50| NodeSpy4
        NodeSpy3 -->|0| NodeSpy5
        NodeSpy4 -->|0| NodeSpy5
        """;

    private const string GraphNode3Win =
        """
        graph TD
        NodeSpy1 -->|0| NodeSpy2
        NodeSpy2 -->|50| NodeSpy3
        NodeSpy2 -->|100| NodeSpy4
        NodeSpy3 -->|0| NodeSpy5
        NodeSpy4 -->|0| NodeSpy5
        """;

    [Theory(DisplayName = "Journey of 2 routes displays graph with weights")]
    [InlineData(50, 100, GraphNode3Win)]
    [InlineData(100, 50, GraphNode4Win)]
    public void Test1(int node3Weight, int node4Weight, string expectedGraph)
    {
        // csharpier-ignore-start
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
        var sut = journey.ToMermaidGraph().ReplaceLineEndings();

        // Assert
        sut.Should().BeEquivalentTo(expectedGraph.ReplaceLineEndings());
        // csharpier-ignore-end
    }
}
