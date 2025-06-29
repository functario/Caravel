namespace Caravel.Core.UnitTests.Tests.FormatGraph;

[Trait(TestType, Unit)]
[Trait(FeatureUnderTest, FeatureFormatGraph)]
public class AsMermaidHtml
{
    [Fact(DisplayName = "Graph of 2 routes displays as html file")]
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
        var sut = journey.Graph.ToMermaidHtml();

        // Assert
        await sut.VerifyMermaidHtmlAsync();
    }

    [Fact(DisplayName = "JourneyLog displays the shortest of 2 routes as html file")]
    public async Task Test2()
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
        var pastJourney = await journey.GotoAsync<NodeSpy5>();

        // Act
        var sut = await pastJourney.ToMermaidHtml(CancellationToken.None);

        // Assert
        await sut.VerifyMermaidHtmlAsync();
    }

    [Fact(DisplayName = "JourneyLog of 2 JourneyHistories displays as html file")]
    public async Task Test3()
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
            .WithEdge<NodeSpy1>()
            .Done()
            .AddNode<NodeSpy5>()
            .WithEdge<NodeSpy4>()
            .Done();

        var journey = builder.Build();
        // csharpier-ignore
        var pastJourney = await journey
            .GotoAsync<NodeSpy5>()
            .GotoAsync<NodeSpy1>();

        // Act
        var sut = await pastJourney.ToManyMermaidHtml(CancellationToken.None);

        // Assert
        await sut.VerifyMermaidHtmlAsync();
    }
}
