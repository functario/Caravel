using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Caravel.Core.Extensions;
using Microsoft.Playwright;
using WebSite.Facade;
using WebSite.Facade.POMs.Pages;

namespace WebSite.PlaywrightTests.Tests;

//[Collection(nameof(PlaywrightCollection))]
public sealed class Navigations : IClassFixture<PlaywrightFixture>, IDisposable
{
    private readonly PlaywrightFixture _playwrightFixture;
    private readonly IPage _page;
    private readonly CancellationTokenSource _journeyCTSource;

    public Navigations(PlaywrightFixture playwrightFixture)
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
    public async Task GotoB_Test()
    {
        var webSiteJourney = _playwrightFixture.WebSiteJourneyBuilder.Create();
        await webSiteJourney.App.OpenWebSiteAsync(_journeyCTSource.Token);
        await webSiteJourney.GotoAsync<PageB>();
    }

    [Fact]
    public async Task GotoB_ThenE_ThenC_Test()
    {
        using var localCancellationTokenSource = new CancellationTokenSource(
            TimeSpan.FromSeconds(60)
        );

        var journeyCancellationToken = _journeyCTSource.Token;
        var webSiteJourney = _playwrightFixture.WebSiteJourneyBuilder.Create();
        await webSiteJourney.App.OpenWebSiteAsync(journeyCancellationToken);
        // csharpier-ignore
        await webSiteJourney
            .GotoAsync<PageB>()
            .GotoAsync<PageE>()
            .GotoAsync<PageE>(
             localCancellationTokenSource.Token // local token merged with journeyCancellationToken
            );
    }

    [Fact]
    public async Task GotoThenE_ThenDoSomethingOnE_Test()
    {
        var webSiteJourney = _playwrightFixture.WebSiteJourneyBuilder.Create();
        await webSiteJourney.App.OpenWebSiteAsync(_journeyCTSource.Token);
        // csharpier-ignore
        await webSiteJourney
            .GotoAsync<PageE>()
            .DoAsync<PageE>(
                async (currentNode, ct) =>
                {
                    // busy work
                    await Task.FromResult(true);
                    currentNode.Should().BeAssignableTo<PageE>();

                    // Returns the same page than origin.
                    return currentNode;
                }
            );
    }

    [Fact]
    public async Task GotoThenE_ThenDoSomethingOnE_ThenGotoA_Test()
    {
        using var localCancellationTokenSource = new CancellationTokenSource(
            TimeSpan.FromSeconds(60)
        );

        var webSiteJourney = _playwrightFixture.WebSiteJourneyBuilder.Create();
        await webSiteJourney.App.OpenWebSiteAsync(_journeyCTSource.Token);
        // csharpier-ignore
        await webSiteJourney
            .GotoAsync<PageE>()
            .DoAsync<PageE>(
                async (pageE, ct) =>
                {
                    // busy work
                    await Task.FromResult(true);
                    pageE.Should().BeAssignableTo<PageE>();

                    // Returns the same page than origin.
                    return pageE;
                },
                localCancellationTokenSource.Token // local token merged with journeyCancellationToken
            )
            .GotoAsync<PageA>();
    }

    [Fact]
    public async Task GotoThenE_ThenDoSomethingOpeningD_Test()
    {
        var webSiteJourney = _playwrightFixture.WebSiteJourneyBuilder.Create();
        await webSiteJourney.App.OpenWebSiteAsync(_journeyCTSource.Token);
        // csharpier-ignore
        await webSiteJourney
            .GotoAsync<PageE>()
            .DoAsync<PageE, PageD>(
                async (journey, pageE, ct) =>
                {
                    // Cast the IJourney to specify WebSiteJourney.
                    var webSiteJourney = journey.OfType<WebSiteJourney>();

                    // busy work
                    await pageE.DoSomething(journey, ct);

                    // validations
                    using var assertionScope = new AssertionScope();
                    webSiteJourney.CurrentNode.Should().BeEquivalentTo(pageE);

                    // Return a different page than origin
                    return webSiteJourney.Map.PageD;
                }
            );
    }
}
