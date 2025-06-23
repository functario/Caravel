using Caravel.Abstractions;

namespace Caravel.Core;

public record DriverLog(Queue<Type> History) : IJourneyLog { }
