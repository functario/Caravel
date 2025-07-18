﻿using Caravel.Abstractions;
using Caravel.Abstractions.Exceptions;

namespace Caravel.Core.Extensions;

public static partial class JourneyExtensions
{
    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);

        return await journey
            .GotoAsync<TDestination>(
                Waypoints.Empty(),
                ExcludedNodes.Empty(),
                localCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        ExcludedNodes excludedNodes,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);

        return await journey
            .GotoAsync<TDestination>(Waypoints.Empty(), excludedNodes, localCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        Waypoints waypoints,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);

        return await journey
            .GotoAsync<TDestination>(waypoints, ExcludedNodes.Empty(), localCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        Waypoints waypoints,
        ExcludedNodes excludeNodes,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);

        return await journey
            .GotoAsync<TDestination>(waypoints, excludeNodes, localCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode>(
        this Task<IJourney> journeyTask,
        Func<TCurrentNode, CancellationToken, Task<TCurrentNode>> func,
        CancellationToken localCancellationToken = default
    )
        where TCurrentNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey
            .DoAsync<TCurrentNode, TCurrentNode>(func, localCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode, TNodeOut>(
        this Task<IJourney> journeyTask,
        Func<TCurrentNode, CancellationToken, Task<TNodeOut>> func,
        CancellationToken localCancellationToken = default
    )
        where TCurrentNode : INode
        where TNodeOut : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey.DoAsync(func, localCancellationToken).ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode>(
        this Task<IJourney> journeyTask,
        Func<IJourney, TCurrentNode, CancellationToken, Task<TCurrentNode>> func,
        CancellationToken localCancellationToken = default
    )
        where TCurrentNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey.DoAsync(func, localCancellationToken).ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode, TNodeOut>(
        this Task<IJourney> journeyTask,
        Func<IJourney, TCurrentNode, CancellationToken, Task<TNodeOut>> func,
        CancellationToken localCancellationToken = default
    )
        where TCurrentNode : INode
        where TNodeOut : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey.DoAsync(func, localCancellationToken).ConfigureAwait(false);
    }

    internal static CancellationTokenSource LinkJourneyAndLocalCancellationTokens(
        this IJourney journey,
        CancellationToken localCancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            journey.JourneyCancellationToken,
            localCancellationToken
        );

        return linkedCancellationTokenSource;
    }

    internal static void ThrowIfNotCurrentNode(this Type current, Type expected)
    {
        if (current != expected)
        {
            throw new UnexpectedNodeException(current, expected);
        }
    }
}
