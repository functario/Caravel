namespace Caravel.Core.UnitTests.Tests.NodeAction.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNodeAction)]
[Trait(Domain, NodeDomain)]
public sealed class AtomicAllowsToAddMetadata : IDisposable
{
    private readonly CancellationTokenSource _localTokenSource30mins;

    public AtomicAllowsToAddMetadata()
    {
        _localTokenSource30mins = new CancellationTokenSource(TimeSpan.FromMinutes(30));
    }

    public void Dispose()
    {
        _localTokenSource30mins?.Dispose();
    }

    [Fact(DisplayName = "When action returns the current node")]
    public async Task Test1()
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
        var sut = await journey
            .DoAsync<Node1, EnrichedNode<Node1>>(
                (journey, node1, ct) =>
                {
                    var actionMetadata = new ActionMetaData("My custom metadata");
                    var enrichedNode = new EnrichedNode<Node1>(node1, actionMetadata);
                    return Task.FromResult(enrichedNode);
                },
                CancellationToken.None
            )
            .GotoAsync<Node2>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }

    [Fact(DisplayName = "When action returns a different node")]
    public async Task Test2()
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
        var map = builder.Map;

        // Act
        var sut = await journey
            .DoAsync<Node1, EnrichedNode<Node2>>(
                (journey, node1, ct) =>
                {
                    var actionMetadata = new ActionMetaData("My custom metadata");
                    var enrichedNode = new EnrichedNode<Node2>(map.NodeSpy2, actionMetadata);
                    return Task.FromResult(enrichedNode);
                },
                CancellationToken.None
            )
            .GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }
}
