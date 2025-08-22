using Caravel.Abstractions.Events;

namespace Caravel.Abstractions.Exceptions;

public class JourneyLegReaderNullException : CaravelException
{
    private static string DefaultMessage(IJourney journey) =>
        $"Optional {nameof(IJourneyLegReader)} in {nameof(IJourney)} with Id '{journey.Id}' is null.";

    public JourneyLegReaderNullException(IJourney journey)
        : base(DefaultMessage(journey)) { }
}
