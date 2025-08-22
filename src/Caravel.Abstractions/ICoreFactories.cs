using Caravel.Abstractions.Events;

namespace Caravel.Abstractions;

public interface ICoreFactories
{
    public IJourneyFactories JourneyFactories { get; }
    public IJourneyLegPublisherFactory JourneyLegPublisherFactory { get; }
}
