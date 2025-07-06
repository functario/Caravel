using AwesomeAssertions;

namespace Caravel.Core.UnitTests.Tests.NodeAction.Chained;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNodeAction)]
[Trait(Domain, NodeDomain)]
public sealed class ChainedHasDefaultJourneyAndLocalCancellationTokensToNone : IDisposable
{
    private readonly CancellationTokenSource _localTokenSource30mins;

    public ChainedHasDefaultJourneyAndLocalCancellationTokensToNone()
    {
        _localTokenSource30mins = new CancellationTokenSource(TimeSpan.FromMinutes(30));
    }

    public void Dispose()
    {
        _localTokenSource30mins?.Dispose();
    }

    [Fact(DisplayName = "When journey CancellationToken and local are default")]
    public async Task Test1()
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

        // CancellationToken not set
        var journey = builder.Build();

        // Act
        var sut = await journey
            .GotoAsync<Node2>()
            .DoAsync<Node2>((node, ct) => Task.FromResult(node)) // CancellationToken not set
            .GotoAsync<Node3>();

        // Assert
        journey.JourneyCancellationToken.IsCancellationRequested.Should().BeFalse();
        _localTokenSource30mins.IsCancellationRequested.Should().BeFalse();
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }
}
