using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class MermaidGraphExtensions
{
    // Note: Mermaid config is space sensitive. 
    private static string FormatHtml(string mermaid) =>
        $$"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8" />
                <title>Mermaid Graph</title>
                <style>
                    html, body {
                        height: 100%;
                        margin: 0;
                        background-color: black;
                        color: white;
                    }

                    .mermaid {
                        display: flex;
                        align-items: center;
                        justify-content: center;
                    }
                </style>
            </head>
            <body>
                <div class="mermaid">
                {{mermaid}}
                </div>

                <script type="module">
                    import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.esm.min.mjs';

                    mermaid.initialize({
                        startOnLoad: true,
                        theme: 'dark',
                        themeVariables: {
                            darkMode: true
                        }
                    });
                </script>
            </body>
            </html>
            
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
