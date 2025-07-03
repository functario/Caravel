using Caravel.Abstractions;

namespace Caravel.Mermaid;

public static partial class JourneyExtensions
{
    public static string ToMermaidMarkdown(this IJourney journey, MermaidOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return journey.Graph.ToMermaidMarkdown(options);
    }
}
