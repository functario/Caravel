using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class MermaidGraphExtensions
{
    // Note: Mermaid config is space sensitive. 
    private static string FormatHtml(string mermaid) =>
        $$"""
            <body style="background-color:black;">
                <pre class="mermaid">
            ---
            config:
                theme: 'dark'
                themeVariables:
            darkMode: 'true'
            ---
                {{mermaid}}
                </pre>
                <script type="module">
                    import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
                    mermaid.initialize({ startOnLoad: true });
                </script>
            </body>
            """;

    public static string AsMermaidHtml(
        this IJourney journey,
        MermaidGraphDirections mermaidGraphDirection = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));

        var mermaid = journey.Graph.ToMermaidGraph(mermaidGraphDirection);
        return FormatHtml(mermaid);
    }

    public static string AsMermaidHtml(
        this IRoute route,
        MermaidGraphDirections mermaidGraphDirection = default
    )
    {
        ArgumentNullException.ThrowIfNull(route, nameof(route));

        var mermaid = route.ToMermaidGraph(mermaidGraphDirection);
        return FormatHtml(mermaid);
    }
}
