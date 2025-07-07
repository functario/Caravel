using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using WebSite.Facade;

namespace WebSite.PlaywrightTests;

public sealed class PlaywrightFixture : IAsyncLifetime
{
    public PlaywrightFixture() { }

    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        Browser = await Playwright.Chromium.LaunchAsync(BrowserTypeLaunchOptions);
    }

    private static BrowserTypeLaunchOptions BrowserTypeLaunchOptions =>
        new()
        {
            Headless = false,
            /*SlowMo = 500*/
        };

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Prefered implementation."
    )]
    public WebSiteJourneyBuilder CreateWebSiteJourneyBuilder(IPage page, out IHost testHost)
    {
        testHost = CreateHost(page);
        return testHost.Services.GetRequiredService<WebSiteJourneyBuilder>();
    }

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

                    services.AddScoped<IServiceProvider>(x => x);
                }
            );

        return hostBuilder.Build();
    }

    public async Task DisposeAsync()
    {
        if (Browser != null)
        {
            await Browser.CloseAsync();
        }

        Playwright?.Dispose();
    }
}
