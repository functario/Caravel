namespace Caravel.Core.UnitTests.Tests.FormatGraph;

[Trait(TestType, Unit)]
[Trait(FeatureUnderTest, FeatureFormatGraph)]
public class AsMermaidGraphHtml
{

    [Fact(DisplayName = "Journey of 2 routes displays as html file")]
    public async Task Test1()
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
        var sut = journey.AsMermaidHtml();

        // Assert
        await sut.VerifyMermaidGraphHtmlAsync();
    }
}
