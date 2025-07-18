﻿using AwesomeAssertions;

namespace Caravel.Core.UnitTests.Tests.NodeAction.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNodeAction)]
[Trait(Domain, NodeDomain)]
public sealed class AtomicAllowsToMergeLocalAndJourneyCancellationTokens : IDisposable
{
    private readonly CancellationTokenSource _localTokenSource30mins;

    public AtomicAllowsToMergeLocalAndJourneyCancellationTokens()
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
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = async () =>
            await journey
                .DoAsync<Node1>(
                    async (node, ct) =>
                    {
                        // cancel only local token
                        await _localTokenSource30mins.CancelAsync();
                        return node;
                    },
                    _localTokenSource30mins.Token
                )
                .GotoAsync<Node2>();

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
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .Done();

        using var journeyTokenSource = new CancellationTokenSource();
        var journey = builder.Build(ct: journeyTokenSource.Token);

        // Act
        var sut = async () =>
            await journey
                .DoAsync<Node1>(
                    async (node, ct) =>
                    {
                        // cancel only journey token
                        await journeyTokenSource.CancelAsync();
                        return node;
                    }
                )
                .GotoAsync<Node2>();

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
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .Done();

        var journey = builder.Build(ct: CancellationToken.None);

        // Act
        var sut = await journey
            .DoAsync<Node1>((node, ct) => Task.FromResult(node), CancellationToken.None)
            .GotoAsync<Node2>();

        // Assert
        journey.JourneyCancellationToken.IsCancellationRequested.Should().BeFalse();
        _localTokenSource30mins.IsCancellationRequested.Should().BeFalse();
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithDescription);
        await result.VerifyMermaidMarkdownAsync();
    }
}
