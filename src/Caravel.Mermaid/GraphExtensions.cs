using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Caravel.Abstractions;
using Caravel.Mermaid.Exceptions;

namespace Caravel.Mermaid;

public static partial class GraphExtensions
{
    private static string NewLine => "<br>";
    private const string SafeCharPattern = @"^[a-zA-Z0-9 _.\-:]+$";

    public static string ToMermaidMarkdown(this IGraph graph, MermaidOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        options ??= MermaidOptions.Default();
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(
            CultureInfo.InvariantCulture,
            $"graph {options.GraphDirection.ToString()}"
        );

        // order need to be respected for comparison
        List<(Type node, ImmutableHashSet<IEdge> edges)> nodeEdges =
        [
            .. graph
                .Nodes.OrderBy(n => n.Key.GetGridPositionRow())
                .ThenBy(n => n.Key.GetGridPositionColumn())
                .Select(n => (node: n.Key, edges: n.Value.GetEdges())),
        ];

        for (var i = 0; i < nodeEdges.Count; i++)
        {
            var (node, edges) = nodeEdges[i];
            var gridPosition = node.GetGridPosition();
            foreach (var edge in edges.OrderBy(x => x.Neighbor.Name))
            {
                var edgeStr = edge.ToMermaidSection(options, gridPosition);
                stringBuilder.AppendLine(edgeStr);
            }
        }

        var result = stringBuilder.ToString();
        return result.EndsWith(Environment.NewLine, StringComparison.OrdinalIgnoreCase)
            ? result[..^Environment.NewLine.Length]
            : result;
        ;
    }

    public static string ToMermaidHtml(this IGraph graph, MermaidOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        var mermaid = graph.ToMermaidMarkdown(options);
        return mermaid.WithOneGraph().FormatHtml();
    }

    public static string ToMermaidHtml(this IRoute route, MermaidOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(route, nameof(route));
        var mermaid = route.ToMermaidMarkdown(options);
        return mermaid.WithOneGraph().FormatHtml();
    }

    public static async Task<string> ToMermaidHtmlAsync(
        this IJourney journey,
        MermaidOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var mermaid = await journey
            .ToMermaidSequenceDiagramMarkdownAsync(options, cancellationToken)
            .ConfigureAwait(false);

        return mermaid.WithOneGraph().FormatHtml();
    }

    public static async Task<string> ToManyMermaidHtmlAsync(
        this IJourney journey,
        MermaidOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        options ??= MermaidOptions.Default();
        var mermaidLegsByIndexes = await journey
            .ToManyMermaidSequenceDiagramMarkdown(options, cancellationToken)
            .ConfigureAwait(false);

        var mermaidLegs = mermaidLegsByIndexes.Values;
        return mermaidLegs.WithManyGraphs().FormatHtml();
    }

    public static async Task<string> ToMermaidSequenceDiagramMarkdownAsync(
        this IJourney journey,
        MermaidOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var journeyLegs = await GetCompletedJourneyLegsAsync(journey, cancellationToken)
            .ConfigureAwait(false);

        options ??= MermaidOptions.Default();
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(CultureInfo.InvariantCulture, $"sequenceDiagram");

        for (var i = 0; i < journeyLegs.Length; i++)
        {
            var edges = journeyLegs[i].Edges.ToArray();

            foreach (var item in edges.ToSequenceDiagramItem(options))
            {
                stringBuilder.AppendLine(item);
            }
        }

        var result = stringBuilder.ToString();
        return result.EndsWith(Environment.NewLine, StringComparison.OrdinalIgnoreCase)
            ? result[..^Environment.NewLine.Length]
            : result;
    }

    public static async Task<Dictionary<int, string>> ToManyMermaidSequenceDiagramMarkdown(
        this IJourney journey,
        MermaidOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));

        options ??= MermaidOptions.Default();
        var journeyLegs = await GetCompletedJourneyLegsAsync(journey, cancellationToken)
            .ConfigureAwait(false);

        var groups = new Dictionary<int, string>();
        for (var i = 0; i < journeyLegs.Length; i++)
        {
            var edges = journeyLegs[i].Edges.ToArray();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(CultureInfo.InvariantCulture, $"sequenceDiagram");
            stringBuilder.AppendLine("box");
            stringBuilder.FlatParticipants(edges);

            foreach (var item in edges.ToSequenceDiagramItem(options))
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
        var history = await journey
            .GetCompletedJourneyLegsAsync(cancellationToken)
            .ConfigureAwait(false);
        return [.. history];
    }

    private static void ThrowIfNotMermaidSafe(this string description)
    {
        var isSafe = MermaidSafeChars().IsMatch(description);
        if (!isSafe)
        {
            throw new InvalidDescriptionException(description, SafeCharPattern);
        }
    }

    private static string GetEdgeNoteAsMermaid(
        this IEdge edge,
        MermaidOptions options,
        GridPositionAttribute? gridPosition = null
    )
    {
        var weight = edge.Weight.ToString(CultureInfo.InvariantCulture);
        var builder = new StringBuilder(weight);

        if (options.DisplayDescription && edge.NeighborNavigator.MetaData is string description)
        {
            description.ThrowIfNotMermaidSafe();
            builder.Append(CultureInfo.InvariantCulture, $"{NewLine}{description}");
        }

        if (gridPosition is not null && options.DisplayGridPosition)
        {
            var gridPositionStr = $"{NewLine}{gridPosition.Row},{gridPosition.Column}";
            builder.Append(CultureInfo.InvariantCulture, $"{gridPositionStr}");
        }

        return builder.ToString();
    }

    private static string ToMermaidSection(
        this IEdge edge,
        MermaidOptions options,
        GridPositionAttribute? gridPosition = null
    )
    {
        var text = GetEdgeNoteAsMermaid(edge, options, gridPosition);
        return $"{edge.Origin.Name} -->|{text}| {edge.Neighbor.Name}";
    }

    private static IEnumerable<string> ToSequenceDiagramItem(
        this IEdge[] edges,
        MermaidOptions options,
        GridPositionAttribute? gridPosition = null
    )
    {
        for (var i = 0; i < edges.Length; i++)
        {
            var edge = edges[i];
            var text = GetEdgeNoteAsMermaid(edge, options, gridPosition);
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

    private static GridPositionAttribute GetGridPosition(this Type nodeType)
    {
        return nodeType.GetCustomAttribute<GridPositionAttribute>()
            ?? new GridPositionAttribute(0, 0);
    }

    private static int GetGridPositionRow(this Type nodeType)
    {
        return nodeType.GetCustomAttribute<GridPositionAttribute>()?.Row ?? int.MaxValue;
    }

    private static int GetGridPositionColumn(this Type nodeType)
    {
        return nodeType.GetCustomAttribute<GridPositionAttribute>()?.Column ?? int.MaxValue;
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
