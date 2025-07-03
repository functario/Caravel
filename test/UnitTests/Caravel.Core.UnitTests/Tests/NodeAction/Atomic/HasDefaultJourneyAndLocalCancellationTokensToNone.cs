using AwesomeAssertions;

namespace Caravel.Core.UnitTests.Tests.NodeAction.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNodeAction)]
[Trait(Domain, NodeDomain)]
public sealed class HasDefaultJourneyAndLocalCancellationTokensToNone : IDisposable
{
    private readonly CancellationTokenSource _localTokenSource30mins;

    public HasDefaultJourneyAndLocalCancellationTokensToNone()
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
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .Done()
            .AddNode<NodeSpy2>()
            .Done();

        // CancellationToken not set
        var journey = builder.Build();

        // Act
        var sut = await journey
            .DoAsync<NodeSpy1>((node, ct) => Task.FromResult(node))
            .GotoAsync<NodeSpy2>(); // CancellationToken not set

        // Assert
        journey.JourneyCancellationToken.IsCancellationRequested.Should().BeFalse();
        _localTokenSource30mins.IsCancellationRequested.Should().BeFalse();
        var result = await sut.ToMermaidSequenceDiagram(isDescriptionDisplayed: true);
        await result.VerifyMermaidMarkdownAsync();
    }
}
