namespace Caravel.Abstractions.Events;
public interface IJourneyLegEvent
{
    DateTimeOffset EventDateTimeOffset { get; }
    public IJourneyLeg JourneyLeg { get; }
}
