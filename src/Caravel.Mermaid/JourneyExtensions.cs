using Caravel.Abstractions;

namespace Caravel.Mermaid;

public static partial class JourneyExtensions
{
    public static string ToMermaidGraph(
        this IJourney journey,
        bool isDescriptionDisplayed = false,
        MermaidGraphDirections mermaidGraphDirection = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return journey.Graph.ToMermaidGraph(isDescriptionDisplayed, mermaidGraphDirection);
    }
}
