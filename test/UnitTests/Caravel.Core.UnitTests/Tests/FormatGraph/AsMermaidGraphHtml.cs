using Caravel.Core.Extensions;
using Caravel.Tests.Fixtures;
using Caravel.Tests.Fixtures.Extensions;
using Caravel.Tests.Fixtures.NodeSpies;

namespace Caravel.Core.UnitTests.Tests.FormatGraph;

[Trait(TestType, Unit)]
[Trait(FeatureUnderTest, FeatureFormatGraph)]
public class AsMermaidGraphHtml
{
    private const string HtlmGraph =
        """
        <body>
            <pre class="mermaid">
            graph TD
        NodeSpy1 -->|0| NodeSpy2
        NodeSpy2 -->|50| NodeSpy3
        NodeSpy2 -->|100| NodeSpy4
        NodeSpy3 -->|0| NodeSpy5
        NodeSpy4 -->|0| NodeSpy5
            </pre>
            <script type="module">
                import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
                mermaid.initialize({ startOnLoad: true });
            </script>
        </body>
        """;

    [Fact(DisplayName = "Journey of 2 routes save to html file")]
    public void Test1()
    {
        var builder = new JourneyBuilder()
                .AddNode<NodeSpy1>()
                .WithEdge<NodeSpy2>()
                .Done()
                .AddNode<NodeSpy2>()
                .WithEdge<NodeSpy3>(50) // The weight setting the route
                .WithEdge<NodeSpy4>(100) // The weight setting the route
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
        var sut = journey.AsMermaidHtml().ReplaceLineEndingsToLinux();

        // Assert
        sut.Should().BeEquivalentTo(HtlmGraph.ReplaceLineEndingsToLinux());
    }
}
