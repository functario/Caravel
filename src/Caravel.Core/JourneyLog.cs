using Caravel.Abstractions;

namespace Caravel.Core;

public record JourneyLog : IJourneyLog
{
    public JourneyLog()
    {
        History = new Queue<Type>();
    }

    public Queue<Type> History { get; init; }
}
