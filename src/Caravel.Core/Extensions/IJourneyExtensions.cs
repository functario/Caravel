using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static partial class IJourneyExtensions
{
    internal static CancellationTokenSource LinkJourneyAndLocalCancellationTokens(
        this IJourney journey,
        CancellationToken scopedCancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            journey.JourneyCancellationToken,
            scopedCancellationToken
        );

        return linkedCancellationTokenSource;
    }
}
