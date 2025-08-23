using Caravel.Abstractions.Configurations;

namespace Caravel.Core.Configurations;

public static class JourneyConfigurationFactory
{
    public static IJourneyConfiguration Create(
        JourneyLegConfigurationOptions journeyLegConfigurationOptions,
        TimeProvider timeProvider
    )
    {
        return journeyLegConfigurationOptions switch
        {
            JourneyLegConfigurationOptions.InMemory => CreateInMemory(timeProvider),
            JourneyLegConfigurationOptions.None => CreateWithoutJourneyLegRecord(timeProvider),
            _ => throw new NotImplementedException(),
        };
    }

    private static JourneyConfiguration CreateInMemory(TimeProvider timeProvider)
    {
        var store = new InMemoryJourneyLegStore();
        return new JourneyConfiguration(
            timeProvider,
            new JourneyLegEventFactory(timeProvider),
            store,
            store,
            new ActionMetaDataFactory(),
            new JourneyLegFactory()
        );
    }

    private static JourneyConfiguration CreateWithoutJourneyLegRecord(TimeProvider timeProvider)
    {
        return new JourneyConfiguration(
            timeProvider,
            new JourneyLegEventFactory(timeProvider),
            new DummyJourneyLegPublisher(),
            new DummyJourneyLegReader(),
            new ActionMetaDataFactory(),
            new JourneyLegFactory()
        );
    }
}
