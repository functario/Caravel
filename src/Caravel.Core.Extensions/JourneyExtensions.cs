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
                EmptyExcludedWaypoints.Create(),
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
                EmptyExcludedWaypoints.Create(),
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

    public static async Task<IJourney> DoAsync<TCurrentNode, TTargetNode>(
        this IJourney journey,
        Func<TCurrentNode, CancellationToken, Task<TTargetNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .DoAsync<TCurrentNode, TTargetNode>(
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

    public static async Task<IJourney> GotoDoAsync<TOriginNode, TTargetNode>(
        this IJourney journey,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TOriginNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var waypoints = EmptyWaypoints.Create();
        var excludedWaypoints = EmptyExcludedWaypoints.Create();

        return await journey
            .GotoDoAsync(func, waypoints, excludedWaypoints, scopedCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoDoAsync<TOriginNode, TTargetNode>(
        this IJourney journey,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
        IWaypoints waypoints,
        CancellationToken scopedCancellationToken = default
    )
        where TOriginNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var excludedWaypoints = EmptyExcludedWaypoints.Create();

        return await journey
            .GotoDoAsync(func, waypoints, excludedWaypoints, scopedCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoDoAsync<TOriginNode, TTargetNode>(
        this IJourney journey,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
        IExcludedWaypoints excludedWaypoints,
        CancellationToken scopedCancellationToken = default
    )
        where TOriginNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var waypoints = EmptyWaypoints.Create();
        return await journey
            .GotoDoAsync(func, waypoints, excludedWaypoints, scopedCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoDoAsync<TOriginNode, TTargetNode>(
        this IJourney journey,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
        IWaypoints waypoints,
        IExcludedWaypoints excludedWaypoints,
        CancellationToken scopedCancellationToken = default
    )
        where TOriginNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        ArgumentNullException.ThrowIfNull(waypoints, nameof(waypoints));
        ArgumentNullException.ThrowIfNull(excludedWaypoints, nameof(excludedWaypoints));
        var originType = typeof(TOriginNode);
        var targetType = typeof(TTargetNode);

        await journey
            .GotoAsync(originType, waypoints, excludedWaypoints, scopedCancellationToken)
            .ConfigureAwait(false);

        return await journey.DoAsync(func, scopedCancellationToken).ConfigureAwait(false);
    }
}
