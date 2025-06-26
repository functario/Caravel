using Caravel.Abstractions;

namespace Caravel.Core;

public record JourneyLog : IJourneyLog
{
    public JourneyLog()
    {
        History = new Queue<IJourneyHistory>();
    }

    public Queue<IJourneyHistory> History { get; init; }
}
