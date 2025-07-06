using Microsoft.Playwright;

namespace WebSite.Facade;

public sealed class App : IAsyncDisposable
{
    public App(IPage page, Uri uri)
    {
        Page = page;
        Uri = uri;
    }

    public IPage Page { get; }
    public Uri Uri { get; }

    public async ValueTask DisposeAsync()
    {
        if (Page.Context is not null)
        {
            await Page.Context.DisposeAsync();
        }
    }
}
