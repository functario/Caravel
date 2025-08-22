namespace Caravel.Abstractions;

public interface IJourneyFactories
{
    public IJourneyLegFactory JourneyLegFactory { get; }
    public IActionMetaDataFactory ActionMetaDataFactory { get; }
}
