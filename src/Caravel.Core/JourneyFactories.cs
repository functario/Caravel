using Caravel.Abstractions;

namespace Caravel.Core;

public sealed class JourneyFactories : IJourneyFactories
{
    public JourneyFactories()
    {
        JourneyLegFactory = new JourneyLegFactory();
        ActionMetaDataFactory = new ActionMetaDataFactory();
        TimeProvider = TimeProvider.System;
    }

    public IJourneyLegFactory JourneyLegFactory { get; init; }

    public IActionMetaDataFactory ActionMetaDataFactory { get; init; }

    public TimeProvider TimeProvider { get; init; }
}
