using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core.Events;

public record JourneyLegStartedEvent(DateTimeOffset EventDateTimeOffset, IJourneyLeg JourneyLeg)
    : IJourneyLegStartedEvent
{ }
