using Caravel.Abstractions.Configurations;
using Caravel.Abstractions.Events;

namespace Caravel.Core.Configurations;

public sealed record JourneyConfiguration(
    TimeProvider TimeProvider,
    IJourneyLegEventFactory JourneyLegEventFactory,
    IJourneyLegPublisher JourneyLegPublisher,
    IJourneyLegReader JourneyLegReader,
    IActionMetaDataFactory ActionMetaDataFactory,
    IJourneyLegFactory JourneyLegFactory
) : IJourneyConfiguration
{ }
