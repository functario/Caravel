using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core.Configurations;

public readonly record struct DummyJourneyLegReader : IJourneyLegReader
{
    public Task<IEnumerable<IJourneyLeg>> GetCompletedJourneyLegsAsync(
        CancellationToken cancellationToken = default
    ) => Task.FromResult(Enumerable.Empty<IJourneyLeg>());
}
