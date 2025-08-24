namespace Caravel.Core.UnitTests.Tests.GotoDo.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNodeAction)]
[Trait(Domain, NodeDomain)]
public sealed class AtomicGotoDo : IDisposable
{
    private readonly CancellationTokenSource _localTokenSource30mins;

    public AtomicGotoDo()
    {
        _localTokenSource30mins = new CancellationTokenSource(TimeSpan.FromMinutes(30));
    }

    public void Dispose()
    {
        _localTokenSource30mins?.Dispose();
    }

    [Fact(DisplayName = "When current, origin and target is the same node")]
    public async Task Test1()
    {
        // Arrange
        var builder = new JourneyBuilder().AddNode<Node1>().Done();

        var journey = builder.Build(ct: CancellationToken.None);

        // Act
        var sut = await journey.GotoDoAsync<Node1, Node1>(
            (journey, node1, ct) =>
            {
                return Task.FromResult(node1);
            },
            CancellationToken.None
        );

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(
        DisplayName = "When current, origin and target is the same node is the same, with enriched metadata"
    )]
    public async Task Test2()
    {
        // Arrange
        var builder = new JourneyBuilder().AddNode<Node1>().Done();

        var journey = builder.Build(ct: CancellationToken.None);

        // Act
        var sut = await journey.GotoDoAsync<Node1, EnrichedNode<Node1>>(
            (journey, node1, ct) =>
            {
                var actionMetadata = new ActionMetaData("My custom metadata");
                var enrichedNode = new EnrichedNode<Node1>(node1, actionMetadata);
                return Task.FromResult(enrichedNode);
            },
            CancellationToken.None
        );

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When current and origin is same node but target is different")]
    public async Task Test3()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .Done();

        var journey = builder.Build(ct: CancellationToken.None);

        // Act
        var sut = await journey.GotoDoAsync<Node1, Node2>(
            (journey, node1, ct) =>
            {
                return Task.FromResult(builder.Map.NodeSpy2);
            },
            CancellationToken.None
        );

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(
        DisplayName = "When current and origin is same node but target is different, with enriched metadata"
    )]
    public async Task Test4()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .Done();

        var journey = builder.Build(ct: CancellationToken.None);

        // Act
        var sut = await journey.GotoDoAsync<Node1, EnrichedNode<Node2>>(
            (journey, node1, ct) =>
            {
                var actionMetadata = new ActionMetaData("My custom metadata");
                var enrichedNode = new EnrichedNode<Node2>(builder.Map.NodeSpy2, actionMetadata);
                return Task.FromResult(enrichedNode);
            },
            CancellationToken.None
        );

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When current, origin and target are all different")]
    public async Task Test5()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .Done();

        var journey = builder.Build(ct: CancellationToken.None);

        // Act
        var sut = await journey.GotoDoAsync<Node2, Node3>(
            (journey, node2, ct) =>
            {
                return Task.FromResult(builder.Map.NodeSpy3);
            },
            CancellationToken.None
        );

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(
        DisplayName = "When current, origin and target are all different, with enriched metadata"
    )]
    public async Task Test6()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .Done();

        var journey = builder.Build(ct: CancellationToken.None);

        // Act
        var sut = await journey.GotoDoAsync<Node2, EnrichedNode<Node3>>(
            (journey, node2, ct) =>
            {
                var actionMetadata = new ActionMetaData("My custom metadata");
                var enrichedNode = new EnrichedNode<Node3>(builder.Map.NodeSpy3, actionMetadata);
                return Task.FromResult(enrichedNode);
            },
            CancellationToken.None
        );

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "With waypoints")]
    public async Task Test7()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node4>()
            .Done()
            .AddNode<Node4>()
            .Done();

        var journey = builder.Build(ct: CancellationToken.None);
        Waypoints waypoints = [typeof(Node2)];

        // Act
        var sut = await journey.GotoDoAsync<Node3, Node4>(
            (journey, node3, ct) =>
            {
                return Task.FromResult(builder.Map.NodeSpy4);
            },
            waypoints,
            CancellationToken.None
        );

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "With excluded waypoints")]
    public async Task Test8()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node4>(weight: 99)
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node4>()
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node5>()
            .Done();

        var journey = builder.Build(ct: CancellationToken.None);

        // Since Node2 has a weight of 99, excluding Node3 validate that we force path on Node2.
        ExcludedWaypoints excludedWaypoints = [typeof(Node3)];

        // Act
        var sut = await journey.GotoDoAsync<Node4, Node5>(
            (journey, node4, ct) =>
            {
                return Task.FromResult(builder.Map.NodeSpy5);
            },
            excludedWaypoints,
            CancellationToken.None
        );

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "With included and excluded waypoints")]
    public async Task Test9()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node4>(weight: 99)
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node4>()
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node5>()
            .WithEdge<Node6>()
            .Done()
            .AddNode<Node6>()
            .Done();

        var journey = builder.Build(ct: CancellationToken.None);

        // Since Node2 has a weight of 99, excluding Node3 validate that we force path on Node2.
        ExcludedWaypoints excludedWaypoints = [typeof(Node3)];
        Waypoints waypoints = [typeof(Node4)];

        // Act
        var sut = await journey.GotoDoAsync<Node5, Node6>(
            (journey, node5, ct) =>
            {
                return Task.FromResult(builder.Map.NodeSpy6);
            },
            waypoints,
            excludedWaypoints,
            CancellationToken.None
        );

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }
}
