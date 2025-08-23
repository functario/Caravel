using Caravel.Abstractions.Events;

namespace Caravel.Abstractions;

public interface IJourneyConfiguration
{
    public IJourneyFactories JourneyFactories { get; }
    public IJourneyLegEventFactory JourneyLegEventFactory { get; }
    public IJourneyLegPublisher? JourneyLegPublisher { get; }
    public IJourneyLegReader? JourneyLegReader { get; }
}
