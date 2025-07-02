using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static partial class JourneyExtensions
{
    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));

        return await journey
            .GotoAsync<TDestination>(
                Waypoints.Empty(),
                ExcludedNodes.Empty(),
                localCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        ExcludedNodes excludedNodes,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .GotoAsync<TDestination>(Waypoints.Empty(), excludedNodes, localCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        Waypoints waypoints,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .GotoAsync<TDestination>(waypoints, ExcludedNodes.Empty(), localCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        Waypoints waypoints,
        ExcludedNodes excludeNodes,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .GotoAsync<TDestination>(waypoints, excludeNodes, localCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode>(
        this IJourney journey,
        Func<TCurrentNode, CancellationToken, Task<TCurrentNode>> func,
        CancellationToken localCancellationToken = default
    )
        where TCurrentNode : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await Task.FromResult(journey)
            .DoAsync<TCurrentNode>(func, localCancellationToken)
            .ConfigureAwait(false);
    }
}
