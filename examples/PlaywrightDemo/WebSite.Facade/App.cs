using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace WebSite.Facade;

public sealed class App : IAsyncDisposable
{
    public App(IPage page, IOptionsMonitor<AppOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        Page = page;
        Uri = GetUri(options.CurrentValue.WebSitePath);
    }

    public IPage Page { get; }
    public Uri Uri { get; }

    public async Task OpenWebSiteAsync(CancellationToken cancellationToken) =>
        await Page.GotoAsync(Uri.AbsoluteUri).WaitAsync(cancellationToken);

    public async ValueTask DisposeAsync()
    {
        if (Page.Context is not null)
        {
            await Page.Context.DisposeAsync();
        }
    }

    private static Uri GetUri(string webSitePath)
    {
        var filePath = Path.GetFullPath(webSitePath);
        return new Uri(filePath);
    }
}
