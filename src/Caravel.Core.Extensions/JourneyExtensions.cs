using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static partial class JourneyExtensions
{
    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        CancellationToken scopedCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .GotoAsync<TDestination>(
                EmptyWaypoints.Create(),
                EmptyExcludedNodes.Create(),
                scopedCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        IExcludedWaypoints excludedWaypoints,
        CancellationToken scopedCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .GotoAsync<TDestination>(
                EmptyWaypoints.Create(),
                excludedWaypoints,
                scopedCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        IWaypoints waypoints,
        CancellationToken scopedCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .GotoAsync<TDestination>(
                waypoints,
                EmptyExcludedNodes.Create(),
                scopedCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        IWaypoints waypoints,
        IExcludedWaypoints excludeNodes,
        CancellationToken scopedCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .GotoAsync<TDestination>(waypoints, excludeNodes, scopedCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode>(
        this IJourney journey,
        Func<TCurrentNode, CancellationToken, Task<TCurrentNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .DoAsync<TCurrentNode, TCurrentNode>(
                (_, node, token) => func(node, token),
                scopedCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode, TNodeOut>(
        this IJourney journey,
        Func<TCurrentNode, CancellationToken, Task<TNodeOut>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
        where TNodeOut : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .DoAsync<TCurrentNode, TNodeOut>(
                (_, node, token) => func(node, token),
                scopedCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode>(
        this IJourney journey,
        Func<IJourney, TCurrentNode, CancellationToken, Task<TCurrentNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey.DoAsync(func, scopedCancellationToken).ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode, TNodeOut>(
        this IJourney journey,
        Func<IJourney, TCurrentNode, CancellationToken, Task<TNodeOut>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
        where TNodeOut : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey.DoAsync(func, scopedCancellationToken).ConfigureAwait(false);
    }
}
