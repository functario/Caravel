using Caravel.Abstractions;
using Caravel.Abstractions.Events;
using Caravel.Core.Events;

namespace Caravel.Core;

public sealed class JourneyConfiguration : IJourneyConfiguration
{
    private readonly TimeProvider _timeProvider;

    public JourneyConfiguration(
        TimeProvider timeProvider,
        IJourneyLegPublisher journeyLegPublisher,
        IJourneyLegReader journeyLegReader
    )
    {
        _timeProvider = timeProvider;
        JourneyLegPublisher = journeyLegPublisher;
        JourneyLegReader = journeyLegReader;
    }

    /// <summary>
    /// Create a <see cref="JourneyConfiguration"/> with <see cref="InMemoryJourneyLegStore"/>
    /// as <see cref="IJourneyLegPublisher"/> and <see cref="IJourneyLegReader"/>.
    /// </summary>
    /// <param name="timeProvider">The <see cref="TimeProvider"/>.</param>
    public JourneyConfiguration(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;

        // Publish and Reader are InMemoryJourneyLegStore.
        var inMemoryJourneyLegStore = new InMemoryJourneyLegStore();
        JourneyLegPublisher = inMemoryJourneyLegStore;
        JourneyLegReader = inMemoryJourneyLegStore;
    }

    public IJourneyFactories JourneyFactories => new JourneyFactories();

    public IJourneyLegEventFactory JourneyLegEventFactory =>
        new JourneyLegEventFactory(_timeProvider);

    public IJourneyLegPublisher? JourneyLegPublisher { get; init; }

    public IJourneyLegReader? JourneyLegReader { get; init; }
}
