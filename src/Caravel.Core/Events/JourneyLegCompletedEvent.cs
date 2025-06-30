using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core.Events;

public record JourneyLegCompletedEvent(DateTimeOffset EventDateTimeOffset, IJourneyLeg JourneyLeg)
    : IJourneyLegCompletedEvent
{ }
