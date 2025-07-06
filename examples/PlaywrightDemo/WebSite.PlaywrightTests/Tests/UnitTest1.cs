using Caravel.Core.Extensions;
using Microsoft.Playwright;
using WebSite.Facade.POMs.Pages;

namespace WebSite.PlaywrightTests.Tests;

//[Collection(nameof(PlaywrightCollection))]
public sealed class UnitTest1 : IClassFixture<PlaywrightFixture>, IDisposable
{
    private readonly PlaywrightFixture _playwrightFixture;
    private readonly IPage _page;
    private readonly CancellationTokenSource _journeyCTSource;

    public UnitTest1(PlaywrightFixture playwrightFixture)
    {
        _playwrightFixture = playwrightFixture;
        _page = _playwrightFixture.Page;
        _journeyCTSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    }

    public void Dispose()
    {
        _journeyCTSource?.Dispose();
    }

    [Fact]
    public async Task Test1()
    {
        var webSiteJourney = _playwrightFixture.WebSiteJourneyBuilder.Create(
            _journeyCTSource.Token
        );

        await webSiteJourney.App.OpenWebSiteAsync(_journeyCTSource.Token);
        await webSiteJourney.GotoAsync<PageB>();
    }
}
