using AwesomeAssertions;

namespace Caravel.Core.UnitTests.Tests.NodeAction.BeforeAnyNavigation;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNodeAction)]
[Trait(Domain, NodeDomain)]
public sealed class AllowsToMergeLocalAndJourneyCancellationTokens : IDisposable
{
    private readonly CancellationTokenSource _localTokenSource30mins;

    public AllowsToMergeLocalAndJourneyCancellationTokens()
    {
        _localTokenSource30mins = new CancellationTokenSource(TimeSpan.FromMinutes(30));
    }

    public void Dispose()
    {
        _localTokenSource30mins?.Dispose();
    }

    [Fact(DisplayName = "When journey CancellationToken is default and local is set")]
    public async Task Test1()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .Done()
            .AddNode<NodeSpy2>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = async () =>
            await journey
                .DoAsync<NodeSpy1>(
                    async (node, ct) =>
                    {
                        // cancel only local token
                        await _localTokenSource30mins.CancelAsync();
                        return node;
                    },
                    _localTokenSource30mins.Token
                )
                .GotoAsync<NodeSpy2>();

        // Assert
        await sut.Should().ThrowExactlyAsync<OperationCanceledException>();
        journey.JourneyCancellationToken.IsCancellationRequested.Should().BeFalse();
        _localTokenSource30mins.IsCancellationRequested.Should().BeTrue();
    }

    [Fact(DisplayName = "When journey CancellationToken is default and local is set")]
    public async Task Test2()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .Done()
            .AddNode<NodeSpy2>()
            .Done();

        using var journeyTokenSource = new CancellationTokenSource();
        var journey = builder.Build(ct: journeyTokenSource.Token);

        // Act
        var sut = async () =>
            await journey
                .DoAsync<NodeSpy1>(
                    async (node, ct) =>
                    {
                        // cancel only journey token
                        await journeyTokenSource.CancelAsync();
                        return node;
                    }
                )
                .GotoAsync<NodeSpy2>();

        // Assert
        await sut.Should().ThrowExactlyAsync<OperationCanceledException>();
        journey.JourneyCancellationToken.IsCancellationRequested.Should().BeTrue();
        _localTokenSource30mins.IsCancellationRequested.Should().BeFalse();
    }

    [Fact(DisplayName = "When journey CancellationToken and local are CancellationToken.None")]
    public async Task Test3()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .Done()
            .AddNode<NodeSpy2>()
            .Done();

        var journey = builder.Build(ct: CancellationToken.None);

        // Act
        var sut = await journey
            .DoAsync<NodeSpy1>((node, ct) => Task.FromResult(node), CancellationToken.None)
            .GotoAsync<NodeSpy2>();

        // Assert
        journey.JourneyCancellationToken.IsCancellationRequested.Should().BeFalse();
        _localTokenSource30mins.IsCancellationRequested.Should().BeFalse();
        var result = await sut.ToMermaidSequenceDiagram();
        await result.VerifyMermaidMarkdownAsync();
    }
}
