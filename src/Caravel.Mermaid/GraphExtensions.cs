using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Caravel.Abstractions;

namespace Caravel.Mermaid;

public static partial class GraphExtensions
{
    private static string NewLine => "<br>";
    private const string SafeCharPattern = @"^[a-zA-Z0-9 _.\-:]+$";

    public static string ToMermaidGraph(
        this IGraph graph,
        bool isDescriptionDisplayed = false,
        MermaidGraphDirections mermaidGraphDirection = default
    )
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(
            CultureInfo.InvariantCulture,
            $"graph {mermaidGraphDirection.ToString()}"
        );

        // order need to be respected for comparison
        List<(Type node, ImmutableHashSet<IEdge> edges)> nodeEdges =
        [
            .. graph
                .Nodes.OrderBy(x => x.Key.Name)
                .Select(n => (node: n.Key, edges: n.Value.GetEdges())),
        ];

        for (var i = 0; i < nodeEdges.Count; i++)
        {
            foreach (var edge in nodeEdges[i].edges.OrderBy(x => x.Neighbor.Name))
            {
                var edgeStr = edge.ToMermaidSection(isDescriptionDisplayed);
                stringBuilder.AppendLine(edgeStr);
            }
        }

        var result = stringBuilder.ToString();
        return result.EndsWith(Environment.NewLine, StringComparison.OrdinalIgnoreCase)
            ? result[..^Environment.NewLine.Length]
            : result;
        ;
    }

    public static string ToMermaidHtml(
        this IGraph graph,
        bool isDescriptionDisplayed = false,
        MermaidGraphDirections mermaidGraphDirection = default
    )
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));

        var mermaid = graph.ToMermaidGraph(isDescriptionDisplayed, mermaidGraphDirection);
        return mermaid.WithOneGraph().FormatHtml();
    }

    public static string ToMermaidHtml(
        this IRoute route,
        MermaidGraphDirections mermaidGraphDirection = default
    )
    {
        ArgumentNullException.ThrowIfNull(route, nameof(route));

        var mermaid = route.ToMermaidGraph(mermaidGraphDirection);
        return mermaid.WithOneGraph().FormatHtml();
    }

    public static async Task<string> ToMermaidHtml(
        this IJourney journey,
        bool isDescriptionDisplayed = false,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));

        var mermaid = await journey
            .ToMermaidSequenceDiagram(isDescriptionDisplayed, cancellationToken)
            .ConfigureAwait(false);

        return mermaid.WithOneGraph().FormatHtml();
    }

    public static async Task<string> ToManyMermaidHtml(
        this IJourney journey,
        bool isDescriptionDisplayed = false,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));

        var mermaidLegsByIndexes = await journey
            .ToManyMermaidSequenceDiagram(isDescriptionDisplayed, cancellationToken)
            .ConfigureAwait(false);

        var mermaidLegs = mermaidLegsByIndexes.Values;
        return mermaidLegs.WithManyGraphs().FormatHtml();
    }

    public static async Task<string> ToMermaidSequenceDiagram(
        this IJourney journey,
        bool isDescriptionDisplayed = false,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var journeyLegs = await GetCompletedJourneyLegsAsync(journey, cancellationToken).ConfigureAwait(false);

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(CultureInfo.InvariantCulture, $"sequenceDiagram");

        for (var i = 0; i < journeyLegs.Length; i++)
        {
            var edges = journeyLegs[i].Edges.ToArray();

            foreach (var item in edges.ToSequenceDiagramItem(isDescriptionDisplayed))
            {
                stringBuilder.AppendLine(item);
            }
        }

        var result = stringBuilder.ToString();
        return result.EndsWith(Environment.NewLine, StringComparison.OrdinalIgnoreCase)
            ? result[..^Environment.NewLine.Length]
            : result;
    }

    public static async Task<Dictionary<int, string>> ToManyMermaidSequenceDiagram(
        this IJourney journey,
        bool isDescriptionDisplayed = false,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));

        var journeyLegs = await GetCompletedJourneyLegsAsync(journey, cancellationToken).ConfigureAwait(false);

        var groups = new Dictionary<int, string>();
        for (var i = 0; i < journeyLegs.Length; i++)
        {
            var edges = journeyLegs[i].Edges.ToArray();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(CultureInfo.InvariantCulture, $"sequenceDiagram");
            stringBuilder.AppendLine("box");
            stringBuilder.FlatParticipants(edges);

            foreach (var item in edges.ToSequenceDiagramItem(isDescriptionDisplayed))
            {
                stringBuilder.AppendLine(item);
            }

            var result = stringBuilder.ToString();
            var sequenceDiagram = result.EndsWith(
                Environment.NewLine,
                StringComparison.OrdinalIgnoreCase
            )
                ? result[..^Environment.NewLine.Length]
                : result;

            groups.Add(i, sequenceDiagram);
        }

        return groups;
    }

    private static StringBuilder FlatParticipants(this StringBuilder stringBuilder, IEdge[] edges)
    {
        var origins = edges.SelectMany(e => new[] { $"{e.Origin.Name}" }).Distinct().ToList();

        string[] all = [.. origins, edges.Last().Neighbor.Name];

        foreach (var edge in all)
        {
            stringBuilder.AppendLine(
                CultureInfo.InvariantCulture,
                $"""
                participant {edge}
                """
            );
        }

        stringBuilder.AppendLine("end");

        return stringBuilder;
    }

    private static async Task<IJourneyLeg[]> GetCompletedJourneyLegsAsync(
        this IJourney journey,
        CancellationToken cancellationToken
    )
    {
        var history = await journey.GetCompletedJourneyLegsAsync(cancellationToken).ConfigureAwait(false);
        return [.. history];
    }

    private static void ThrowIfNotMermaidSafe(this string description)
    {
        var isSafe = MermaidSafeChars().IsMatch(description);
        if (!isSafe)
        {
            throw new ArgumentException(
                $"The description must respect the regex pattern '{SafeCharPattern}'."
            );
        }
    }

    private static string GetEdgeNoteAsMermaid(this IEdge edge, bool isDescriptionDisplayed)
    {
        var weight = edge.Weight.ToString(CultureInfo.InvariantCulture);
        if (isDescriptionDisplayed && edge.NeighborNavigator.MetaData is string description)
        {
            description.ThrowIfNotMermaidSafe();
            return $"{weight}{NewLine}{description}";
        }

        return weight;
    }

    private static string ToMermaidSection(this IEdge edge, bool isDescriptionDisplayed)
    {
        var text = GetEdgeNoteAsMermaid(edge, isDescriptionDisplayed);
        return $"{edge.Origin.Name} -->|{text}| {edge.Neighbor.Name}";
    }

    private static IEnumerable<string> ToSequenceDiagramItem(
        this IEdge[] edges,
        bool isDescriptionDisplayed
    )
    {
        for (var i = 0; i < edges.Length; i++)
        {
            var edge = edges[i];
            var text = GetEdgeNoteAsMermaid(edge, isDescriptionDisplayed);
            yield return $"{edge.Origin.Name}->>{edge.Neighbor.Name}:{text}";
        }
    }

    private static string WithManyGraphs(this ICollection<string> graphs)
    {
        return string.Join(
            Environment.NewLine,
            graphs.Select(
                (graph, index) =>
                    $"""
                    <h2 class="diagram-title">{index + 1}</h2>
                    <div class="mermaid">{graph}</div>
                    <hr class="separator" />
                    """
            )
        );
    }

    private static string WithOneGraph(this string graph)
    {
        return $"""
            <div class="mermaid">{graph}</div>
            """;
    }

    // Note: Mermaid config is space sensitive.
    private static string FormatHtml(this string graphs)
    {
        return $$"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8" />
                <title>Mermaid Graphs</title>
                <style>
                    html, body {
                        background-color: black;
                        color: white;
                    }
                                        
                    .diagram-title {
                        text-align: center;
                        margin: 40px 0 10px;
                        font-size: 1.5em;
                        color: #ccc;
                    }

                    .mermaid {
                        display: flex;
                        align-items: center;
                        justify-content: center;
                    }

                    .separator {
                        border: none;
                        border-top: 1px solid #444;
                        margin: 40px auto;
                        width: 80%;
                    }
                </style>
            </head>
            <body>
            {{graphs}}

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
    }

    [GeneratedRegex(SafeCharPattern)]
    private static partial Regex MermaidSafeChars();
}
