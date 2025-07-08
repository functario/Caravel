using Caravel.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace WebSite.Facade;

public sealed class App : IAsyncDisposable
{
    private const string DefaultWebSiteGraph = """
        PageA->>PageB
        PageA->>PageC
        PageA->>PageD
        PageA->>PageE

        PageB->>PageA
        PageB->>PageC
        PageB->>PageD
        PageB->>PageE

        PageC->>PageA
        PageC->>PageB
        PageC->>PageD
        PageC->>PageE

        PageD->>PageA
        PageD->>PageB
        PageD->>PageC
        PageD->>PageE

        PageE->>PageA
        PageE->>PageB
        PageE->>PageC
        PageE->>PageD
        """;

    public App(IPage page, IOptionsMonitor<AppOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        Page = page;
        Uri = GetUri(options.CurrentValue.WebSitePath);
    }

    public IPage Page { get; }
    public Uri Uri { get; private set; }

    public async Task OpenWebSiteAsync(CancellationToken cancellationToken)
    {
        SetGraph(DefaultWebSiteGraph);
        await Page.GotoAsync(Uri.AbsoluteUri).WaitAsync(cancellationToken);
    }

    /// <summary>
    /// Overwrite the WebSite Graph which will change the navigation.
    /// </summary>
    /// <param name="webSiteTopology">The topology of the website as
    /// Mermaid formatted graph:<br/>
    /// PageA->>PageB<br/>
    /// PageA->>PageC<br/>
    /// PageA->>PageD<br/>
    /// PageA->>PageE<br/></param>
    /// <param name="cancellationToken"> <see cref="IJourney.JourneyCancellationToken"/>.</param>
    /// <returns>The WebSite is opened with expected graph</returns>
    public async Task OpenWebSiteAsync(string webSiteTopology, CancellationToken cancellationToken)
    {
        SetGraph(webSiteTopology);
        await Page.GotoAsync(Uri.AbsoluteUri).WaitAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (Page.Context is not null)
        {
            await Page.Context.DisposeAsync();
        }
    }

    private void SetGraph(string mermaidGraph)
    {
        ArgumentNullException.ThrowIfNull(mermaidGraph, nameof(mermaidGraph));
        var urlEncodeNewLine = "%0A";
        var segment = mermaidGraph
            .Replace("graph", "", StringComparison.OrdinalIgnoreCase)
            .Replace("TD", "", StringComparison.OrdinalIgnoreCase)
            .Replace("LR", "", StringComparison.OrdinalIgnoreCase)
            .Trim()
            .Replace("\r", "", StringComparison.OrdinalIgnoreCase)
            .Replace("\n", urlEncodeNewLine, StringComparison.OrdinalIgnoreCase);
        var graphAddress = Uri.AbsoluteUri + "?graph=" + segment;
        Uri = new Uri(graphAddress, UriKind.Absolute);
    }

    private static Uri GetUri(string webSitePath)
    {
        var filePath = Path.GetFullPath(webSitePath);
        return new Uri(filePath);
    }
}
