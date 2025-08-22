namespace Caravel.Abstractions.Events;

public interface IJourneyLegReader
{
    public Task<IEnumerable<IJourneyLeg>> GetCompletedJourneyLegsAsync(
        CancellationToken cancellationToken = default
    );
}
