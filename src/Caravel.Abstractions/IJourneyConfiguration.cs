using Caravel.Abstractions.Events;

namespace Caravel.Abstractions;

public interface IJourneyConfiguration
{
    public IJourneyLegEventFactory JourneyLegEventFactory { get; }
    public IJourneyLegPublisher? JourneyLegPublisher { get; }
    public IJourneyLegReader? JourneyLegReader { get; }
    public IJourneyLegFactory JourneyLegFactory { get; init; }
    public IActionMetaDataFactory ActionMetaDataFactory { get; init; }
}
