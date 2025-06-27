using Caravel.Abstractions;

namespace Caravel.History.Mermaid;

public static partial class JourneyExtensions
{

    public static string ToMermaidGraph(
        this IJourney journey,
        MermaidGraphDirections mermaidGraphDirection = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return journey.Graph.ToMermaidGraph(mermaidGraphDirection);
    }
}
