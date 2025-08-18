using Caravel.Abstractions;
using Caravel.Abstractions.Exceptions;

namespace Caravel.Core.Extensions;

public static partial class JourneyExtensions
{
    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        CancellationToken scopedCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);

        return await journey
            .GotoAsync<TDestination>(
                EmptyWaypoints.Create(),
                EmptyExcludedNodes.Create(),
                scopedCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        IExcludedNodes excludedNodes,
        CancellationToken scopedCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey
            .GotoAsync<TDestination>(
                EmptyWaypoints.Create(),
                excludedNodes,
                scopedCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        IWaypoints waypoints,
        CancellationToken scopedCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey
            .GotoAsync<TDestination>(
                waypoints,
                EmptyExcludedNodes.Create(),
                scopedCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        IWaypoints waypoints,
        IExcludedNodes excludeNodes,
        CancellationToken scopedCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);

        return await journey
            .GotoAsync<TDestination>(waypoints, excludeNodes, scopedCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode>(
        this Task<IJourney> journeyTask,
        Func<TCurrentNode, CancellationToken, Task<TCurrentNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey
            .DoAsync<TCurrentNode, TCurrentNode>(func, scopedCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode, TNodeOut>(
        this Task<IJourney> journeyTask,
        Func<TCurrentNode, CancellationToken, Task<TNodeOut>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
        where TNodeOut : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey.DoAsync(func, scopedCancellationToken).ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode>(
        this Task<IJourney> journeyTask,
        Func<IJourney, TCurrentNode, CancellationToken, Task<TCurrentNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey.DoAsync(func, scopedCancellationToken).ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode, TNodeOut>(
        this Task<IJourney> journeyTask,
        Func<IJourney, TCurrentNode, CancellationToken, Task<TNodeOut>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
        where TNodeOut : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey.DoAsync(func, scopedCancellationToken).ConfigureAwait(false);
    }

    internal static void ThrowIfNotCurrentNode(this Type current, Type expected)
    {
        if (current != expected)
        {
            throw new UnexpectedNodeException(current, expected);
        }
    }
}
