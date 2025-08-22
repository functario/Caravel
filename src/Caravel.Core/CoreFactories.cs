using Caravel.Abstractions;
using Caravel.Abstractions.Events;
using Caravel.Core.Events;

namespace Caravel.Core;

public sealed class CoreFactories : ICoreFactories
{
    private readonly TimeProvider _timeProvider;

    public CoreFactories(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public IJourneyFactories JourneyFactories => new JourneyFactories();

    public IJourneyLegEventFactory JourneyLegEventFactory =>
        new JourneyLegEventFactory(_timeProvider);
}
