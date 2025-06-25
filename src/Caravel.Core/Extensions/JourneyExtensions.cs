using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static partial class JourneyExtensions
{
    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));

        return await journey
            .GotoAsync<TDestination>(
                Waypoints.Empty(),
                ExcludedNodes.Empty(),
                localCancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        ExcludedNodes excludedNodes,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .GotoAsync<TDestination>(Waypoints.Empty(), excludedNodes, localCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        Waypoints waypoints,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .GotoAsync<TDestination>(waypoints, ExcludedNodes.Empty(), localCancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<IJourney> GotoAsync<TDestination>(
        this IJourney journey,
        Waypoints waypoints,
        ExcludedNodes excludeNodes,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return await journey
            .GotoAsync<TDestination>(waypoints, excludeNodes, localCancellationToken)
            .ConfigureAwait(false);
    }

    public static string ToMermaidGraph(
        this IJourney journey,
        MermaidGraphDirections mermaidGraphDirection = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        return journey.Graph.ToMermaidGraph(mermaidGraphDirection);
    }

    public static string AsMermaidHtml(
        this IJourney journey,
        MermaidGraphDirections mermaidGraphDirection = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));

        var mermaid = journey.Graph.ToMermaidGraph(mermaidGraphDirection);

        var html =
            $$"""
            <body>
                <pre class="mermaid">
                {{mermaid}}
                </pre>
                <script type="module">
                    import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
                    mermaid.initialize({ startOnLoad: true });
                </script>
            </body>
            """;

        return html;
    }
}
