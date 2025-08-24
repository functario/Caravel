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
    /// <param name="journey">
    /// The <see cref="IJourney"/> to be operated on.
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
    /// <param name="journey">
    /// The <see cref="IJourney"/> to be operated on.
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
        this IJourney journey,
        IWaypoints waypoints,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TOriginNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var excludedWaypoints = EmptyExcludedWaypoints.Create();

        return await journey
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
    /// <param name="journey">
    /// The <see cref="IJourney"/> to be operated on.
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
        this IJourney journey,
        IExcludedWaypoints excludedWaypoints,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
        CancellationToken scopedCancellationToken = default
    )
        where TOriginNode : INode
        where TTargetNode : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var waypoints = EmptyWaypoints.Create();
        return await journey
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
    /// <param name="journey">
    /// The <see cref="IJourney"/> to be operated on.
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
        this IJourney journey,
        IWaypoints waypoints,
        IExcludedWaypoints excludedWaypoints,
        Func<IJourney, TOriginNode, CancellationToken, Task<TTargetNode>> func,
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
