using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using WebSite.Facade;

namespace WebSite.PlaywrightTests;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "Required for testing"
)]
public class TestBase : IClassFixture<PlaywrightFixture>, IAsyncLifetime
{
    private readonly PlaywrightFixture _playwrightFixture;
    public CancellationTokenSource JourneyCTSource { get; set; } = null!;
    public WebSiteJourneyBuilder WebSiteJourneyBuilder { get; private set; } = null!;
    public WebSiteJourney WebSiteJourney { get; private set; } = null!;
    public IBrowserContext Context { get; set; } = null!;
    public IPage Page { get; set; } = null!;
    public IHost TestHost { get; set; } = null!;

    public TestBase(PlaywrightFixture playwrightFixture)
    {
        _playwrightFixture = playwrightFixture;
        JourneyCTSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    }

    public async Task InitializeAsync()
    {
        // new Context, page, journey, etc, between tests.
        // But the Browser and Playwright instance are created once by class.
        Context = await _playwrightFixture.Browser.NewContextAsync();
        Page = await Context.NewPageAsync();
        WebSiteJourneyBuilder = _playwrightFixture.CreateWebSiteJourneyBuilder(
            Page,
            out var testHost
        );

        TestHost = testHost;
        WebSiteJourney = WebSiteJourneyBuilder.Create();
    }

    public Task DisposeAsync()
    {
        // Dispose after all tests of the class have run.
        JourneyCTSource?.Dispose();
        TestHost?.Dispose();
        return Task.CompletedTask;
    }
}
