using Caravel.Abstractions;

namespace Caravel.Core.Configurations;

public enum JourneyLegConfigurationOptions
{
    /// <summary>
    /// The <see cref="IJourneyLeg"/> are stored in memory.
    /// </summary>
    InMemory = 0,

    /// <summary>
    /// No <see cref="IJourneyLeg"/> stored.
    /// </summary>
    None,
}
