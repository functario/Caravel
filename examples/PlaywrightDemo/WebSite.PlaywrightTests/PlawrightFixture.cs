using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using WebSite.Facade;

namespace WebSite.PlaywrightTests;

//[CollectionDefinition(nameof(PlaywrightCollection))]
//public class PlaywrightCollection : ICollectionFixture<PlaywrightFixture>
//{
//    // This class has no code, and is never created. Its purpose is simply
//    // to be the place to apply [CollectionDefinition] and all the
//    // ICollectionFixture<> interfaces.
//}

public sealed class PlaywrightFixture : IAsyncLifetime, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    public PlaywrightFixture()
    {
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(10));
    }

    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;
    public IPage Page { get; private set; } = null!;
    public IBrowserContext Context { get; private set; } = null!;
    public WebSiteJourneyBuilder WebSiteJourneyBuilder { get; private set; } = null!;
    public IHost TestHost { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        Browser = await Playwright.Chromium.LaunchAsync(BrowserTypeLaunchOptions);

        Context = await Browser.NewContextAsync();

        Page = await Context.NewPageAsync();

        TestHost = CreateHost(Page);

        WebSiteJourneyBuilder = TestHost.Services.GetRequiredService<WebSiteJourneyBuilder>();
    }

    private static BrowserTypeLaunchOptions BrowserTypeLaunchOptions =>
        new() { Headless = false, SlowMo = 500 };

    private static IHost CreateHost(IPage page)
    {
        var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(
                (context, services) =>
                {
                    services
                        .AddOptions<AppOptions>()
                        .Bind(context.Configuration.GetSection(AppOptions.Name))
                        .ValidateOnStart();

                    services.ConfigureOptions<AppOptions>();
                    services.AddScoped<IPage>(x => page);
                    services.AddWebSiteFacade(context);
                }
            );

        return hostBuilder.Build();
    }

    public async Task DisposeAsync()
    {
        Dispose();
        if (Context != null)
        {
            await Context.CloseAsync();
        }

        if (Browser != null)
        {
            await Browser.CloseAsync();
        }

        TestHost?.Dispose();
        Playwright?.Dispose();
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
    }
}
