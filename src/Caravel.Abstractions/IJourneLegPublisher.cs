namespace Caravel.Abstractions;
public interface IJourneLegPublisher
{
    public Task PublishOnJourneyLegCompletedAsync(IJourneyLeg journeyLeg, CancellationToken cancellationToken);
}
