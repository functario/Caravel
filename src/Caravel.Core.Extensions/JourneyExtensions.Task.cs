using Caravel.Abstractions;

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
                EmptyExcludedWaypoints.Create(),
                scopedCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        IExcludedWaypoints excludedWaypoints,
        CancellationToken scopedCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey
            .GotoAsync<TDestination>(
                EmptyWaypoints.Create(),
                excludedWaypoints,
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
                EmptyExcludedWaypoints.Create(),
                scopedCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        IWaypoints waypoints,
        IExcludedWaypoints excludeNodes,
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

    public static async Task<IJourney> DoAsync<TCurrentNode, TTargetNode>(
        this Task<IJourney> journeyTask,
        Func<TCurrentNode, CancellationToken, Task<TTargetNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
        where TTargetNode : INode
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

    public static async Task<IJourney> DoAsync<TCurrentNode, TTargetNode>(
        this Task<IJourney> journeyTask,
        Func<IJourney, TCurrentNode, CancellationToken, Task<TTargetNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);
        return await journey.DoAsync(func, scopedCancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Moves the current <see cref="IJourney"/> to the specified <typeparamref name="TOriginNode"/>
    /// and then executes <paramref name="func"/> with that node as the context.
    /// </summary>
    /// <typeparam name="TOriginNode">
    /// The type of node that the journey is navigated to before <paramref name="func"/> is invoked.
    /// </typeparam>
    /// <typeparam name="TTargetNode">
    /// The type of node that <paramref name="func"/> returns.
    /// It may be the same as <typeparamref name="TOriginNode"/> or a different node.
    /// </typeparam>
    /// <param name="journeyTask">
    /// A task that resolves to the <see cref="IJourney"/> to be operated on.
    /// </param>
    /// <param name="func">
    /// A function that runs in the context of the node reached by <typeparamref name="TOriginNode"/>.
    /// The function receives the journey, the reached node, and a cancellation token, and returns a node of type <typeparamref name="TTargetNode"/>.
    /// </param>
    /// <param name="scopedCancellationToken">
    /// A cancellation token that applies only to this method.
    /// </param>
    /// <returns>
    /// The same <see cref="IJourney"/> instance, now positioned at the node returned by <paramref name="func"/>.
    /// </returns>
    public static async Task<IJourney> GotoDoAsync<TOriginNode, TTargetNode>(
        this Task<IJourney> journeyTask,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TOriginNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));
        var waypoints = EmptyWaypoints.Create();
        var excludedWaypoints = EmptyExcludedWaypoints.Create();

        return await journeyTask
            .GotoDoAsync(waypoints, excludedWaypoints, func, scopedCancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Moves the current <see cref="IJourney"/> to the specified <typeparamref name="TOriginNode"/>
    /// and then executes <paramref name="func"/> with that node as the context.
    /// </summary>
    /// <typeparam name="TOriginNode">
    /// The type of node that the journey is navigated to before <paramref name="func"/> is invoked.
    /// </typeparam>
    /// <typeparam name="TTargetNode">
    /// The type of node that <paramref name="func"/> returns.
    /// It may be the same as <typeparamref name="TOriginNode"/> or a different node.
    /// </typeparam>
    /// <param name="journeyTask">
    /// A task that resolves to the <see cref="IJourney"/> to be operated on.
    /// </param>
    /// <param name="waypoints">
    /// The <see cref="IWaypoints"/> that the journey must cross to reach <typeparamref name="TOriginNode"/>.
    /// </param>
    /// <param name="func">
    /// A function that runs in the context of the node reached by <typeparamref name="TOriginNode"/>.
    /// The function receives the journey, the reached node, and a cancellation token, and returns a node of type <typeparamref name="TTargetNode"/>.
    /// </param>
    /// <param name="scopedCancellationToken">
    /// A cancellation token that applies only to this method.
    /// </param>
    /// <returns>
    /// The same <see cref="IJourney"/> instance, now positioned at the node returned by <paramref name="func"/>.
    /// </returns>
    public static async Task<IJourney> GotoDoAsync<TOriginNode, TTargetNode>(
        this Task<IJourney> journeyTask,
        IWaypoints waypoints,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TOriginNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));
        var excludedWaypoints = EmptyExcludedWaypoints.Create();

        return await journeyTask
            .GotoDoAsync(waypoints, excludedWaypoints, func, scopedCancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Moves the current <see cref="IJourney"/> to the specified <typeparamref name="TOriginNode"/>
    /// and then executes <paramref name="func"/> with that node as the context.
    /// </summary>
    /// <typeparam name="TOriginNode">
    /// The type of node that the journey is navigated to before <paramref name="func"/> is invoked.
    /// </typeparam>
    /// <typeparam name="TTargetNode">
    /// The type of node that <paramref name="func"/> returns.
    /// It may be the same as <typeparamref name="TOriginNode"/> or a different node.
    /// </typeparam>
    /// <param name="journeyTask">
    /// A task that resolves to the <see cref="IJourney"/> to be operated on.
    /// </param>
    /// <param name="excludedWaypoints">
    /// The <see cref="IExcludedWaypoints"/> that the journey must avoid to reach <typeparamref name="TOriginNode"/>.
    /// </param>
    /// <param name="func">
    /// A function that runs in the context of the node reached by <typeparamref name="TOriginNode"/>.
    /// The function receives the journey, the reached node, and a cancellation token, and returns a node of type <typeparamref name="TTargetNode"/>.
    /// </param>
    /// <param name="scopedCancellationToken">
    /// A cancellation token that applies only to this method.
    /// </param>
    /// <returns>
    /// The same <see cref="IJourney"/> instance, now positioned at the node returned by <paramref name="func"/>.
    /// </returns>
    public static async Task<IJourney> GotoDoAsync<TOriginNode, TTargetNode>(
        this Task<IJourney> journeyTask,
        IExcludedWaypoints excludedWaypoints,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TOriginNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));
        var waypoints = EmptyWaypoints.Create();

        return await journeyTask
            .GotoDoAsync(waypoints, excludedWaypoints, func, scopedCancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Moves the current <see cref="IJourney"/> to the specified <typeparamref name="TOriginNode"/>
    /// and then executes <paramref name="func"/> with that node as the context.
    /// </summary>
    /// <typeparam name="TOriginNode">
    /// The type of node that the journey is navigated to before <paramref name="func"/> is invoked.
    /// </typeparam>
    /// <typeparam name="TTargetNode">
    /// The type of node that <paramref name="func"/> returns.
    /// It may be the same as <typeparamref name="TOriginNode"/> or a different node.
    /// </typeparam>
    /// <param name="journeyTask">
    /// A task that resolves to the <see cref="IJourney"/> to be operated on.
    /// </param>
    /// <param name="waypoints">
    /// The <see cref="IWaypoints"/> that the journey must cross to reach <typeparamref name="TOriginNode"/>.
    /// </param>
    /// <param name="excludedWaypoints">
    /// The <see cref="IExcludedWaypoints"/> that the journey must avoid to reach <typeparamref name="TOriginNode"/>.
    /// </param>
    /// <param name="func">
    /// A function that runs in the context of the node reached by <typeparamref name="TOriginNode"/>.
    /// The function receives the journey, the reached node, and a cancellation token, and returns a node of type <typeparamref name="TTargetNode"/>.
    /// </param>
    /// <param name="scopedCancellationToken">
    /// A cancellation token that applies only to this method.
    /// </param>
    /// <returns>
    /// The same <see cref="IJourney"/> instance, now positioned at the node returned by <paramref name="func"/>.
    /// </returns>
    public static async Task<IJourney> GotoDoAsync<TOriginNode, TTargetNode>(
        this Task<IJourney> journeyTask,
        IWaypoints waypoints,
        IExcludedWaypoints excludedWaypoints,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TOriginNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));
        ArgumentNullException.ThrowIfNull(waypoints, nameof(waypoints));
        ArgumentNullException.ThrowIfNull(excludedWaypoints, nameof(excludedWaypoints));
        var originType = typeof(TOriginNode);
        var targetType = typeof(TTargetNode);
        var journey = await journeyTask.ConfigureAwait(false);

        await journey
            .GotoAsync(originType, waypoints, excludedWaypoints, scopedCancellationToken)
            .ConfigureAwait(false);

        return await journey.DoAsync(func, scopedCancellationToken).ConfigureAwait(false);
    }
}
