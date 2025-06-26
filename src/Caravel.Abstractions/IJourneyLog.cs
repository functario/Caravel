namespace Caravel.Abstractions;

public interface IJourneyLog
{
    Queue<IJourneyHistory> History { get; }
}
