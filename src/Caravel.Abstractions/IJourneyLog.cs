namespace Caravel.Abstractions;

public interface IJourneyLog
{
    Queue<Type> History { get; }
}
