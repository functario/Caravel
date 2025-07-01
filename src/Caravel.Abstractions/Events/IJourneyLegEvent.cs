namespace Caravel.Abstractions.Events;

public interface IJourneyLegEvent
{
    DateTimeOffset Timestamp { get; }
    public IJourneyLeg JourneyLeg { get; }
}
