using Caravel.Abstractions.Configurations;

namespace Caravel.Core.Configurations;

public static class JourneyConfigurationFactory
{
    public static IJourneyConfiguration Create(
        JourneyLegHandlingOptions journeyLegHandlingOptions,
        TimeProvider timeProvider
    )
    {
        return journeyLegHandlingOptions switch
        {
            JourneyLegHandlingOptions.InMemory => CreateInMemory(timeProvider),
            JourneyLegHandlingOptions.None => CreateWithoutJourneyLegRecord(timeProvider),
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
