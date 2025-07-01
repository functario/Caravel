using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core.Events;

public record JourneyLegStartedEvent(DateTimeOffset Timestamp, IJourneyLeg JourneyLeg)
    : IJourneyLegStartedEvent
{ }
