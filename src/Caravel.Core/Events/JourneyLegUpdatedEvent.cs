using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core.Events;

public record JourneyLegUpdatedEvent(DateTimeOffset Timestamp, IJourneyLeg JourneyLeg)
    : IJourneyLegUpdatedEvent { }
