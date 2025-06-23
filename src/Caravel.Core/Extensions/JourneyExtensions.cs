using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class JourneyExtensions
{
    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        CancellationToken methodCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));
        var journey = await journeyTask.ConfigureAwait(false);
        return await journey.GotoAsync<TDestination>(methodCancellationToken).ConfigureAwait(false);
    }
}
