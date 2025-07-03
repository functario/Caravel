namespace Caravel.Core.UnitTests.Tests.FormatGraph;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureFormatGraph)]
public class AsMermaidHtml
{
    [Fact(DisplayName = "Graph of 2 routes displays as html file")]
    public async Task Test1()
    {
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>(description: "Open Node2")
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>(50) // The weight setting the route
            .WithEdge<Node4>(100) // The weight setting the route
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
        var sut = journey.Graph.ToMermaidHtml(WithDescription);

        // Assert
        await sut.VerifyMermaidHtmlAsync();
    }

    [Fact(DisplayName = "JourneyLog displays the shortest of 2 routes as html file")]
    public async Task Test2()
    {
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>(description: "Open Node2")
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>(50) // The weight setting the route
            .WithEdge<Node4>(100) // The weight setting the route
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
        var pastJourney = await journey.GotoAsync<Node5>();

        // Act
        var sut = await pastJourney.ToMermaidHtmlAsync(WithDescription);

        // Assert
        await sut.VerifyMermaidHtmlAsync();
    }

    [Fact(DisplayName = "JourneyLog of 2 JourneyHistories displays as html file")]
    public async Task Test3()
    {
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>(description: "Open Node2")
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>(50, description: "Open Node3") // The weight setting the route
            .WithEdge<Node4>(100, description: "Open Node4") // The weight setting the route
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node5>(description: "Open Node5")
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node5>(description: "Open Node5")
            .WithEdge<Node1>(description: "Open Node1")
            .Done()
            .AddNode<Node5>()
            .WithEdge<Node4>(description: "Open Node4")
            .Done();

        var journey = builder.Build();
        // csharpier-ignore
        var pastJourney = await journey
            .GotoAsync<Node5>()
            .GotoAsync<Node1>();

        // Act
        var sut = await pastJourney.ToManyMermaidHtmlAsync(WithDescription);

        // Assert
        await sut.VerifyMermaidHtmlAsync();
    }
}
