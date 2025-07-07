using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Caravel.Core.Extensions;
using WebSite.Facade;
using WebSite.Facade.POMs.Pages;
using WebSite.PlaywrightTests.Extensions;

namespace WebSite.PlaywrightTests.Tests;

public sealed class Navigations : TestBase
{
    public Navigations(PlaywrightFixture playwrightFixture)
        : base(playwrightFixture) { }

    [Fact(DisplayName = "From PageA Goto PageB")]
    public async Task Test1()
    {
        await WebSiteJourney.App.OpenWebSiteAsync(JourneyCTSource.Token);

        await WebSiteJourney.GotoAsync<PageB>();

        // Route validation
        await WebSiteJourney.VerifyRouteAsync();
    }

    [Fact(DisplayName = "From PageA Goto PageB - PageE - PageC")]
    public async Task Test2()
    {
        using var localCancellationTokenSource = new CancellationTokenSource(
            TimeSpan.FromSeconds(60)
        );

        var journeyCancellationToken = JourneyCTSource.Token;
        await WebSiteJourney.App.OpenWebSiteAsync(JourneyCTSource.Token);
        // csharpier-ignore
        await WebSiteJourney
            .GotoAsync<PageB>()
            .GotoAsync<PageE>()
            .GotoAsync<PageC>(
             localCancellationTokenSource.Token // (optional) local token merged with journeyCancellationToken
            );

        // Route validation
        await WebSiteJourney.VerifyRouteAsync();
    }

    [Fact(
        DisplayName = "From PageA Goto PageE - Do something on PageE - Continue from PageE to PageA"
    )]
    public async Task Test3()
    {
        using var localCancellationTokenSource = new CancellationTokenSource(
            TimeSpan.FromSeconds(60)
        );

        await WebSiteJourney.App.OpenWebSiteAsync(JourneyCTSource.Token);
        // csharpier-ignore
        await WebSiteJourney
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
                localCancellationTokenSource.Token // (optional) local token merged with journeyCancellationToken
            )
            .GotoAsync<PageA>();

        // Route validation
        await WebSiteJourney.VerifyRouteAsync();
    }

    [Fact(
        DisplayName = "From PageA Goto PageE - Do something on PageE - But continue from PageD to Page C"
    )]
    public async Task Test4()
    {
        await WebSiteJourney.App.OpenWebSiteAsync(JourneyCTSource.Token);
        // csharpier-ignore
        await WebSiteJourney
            .GotoAsync<PageE>()
            .DoAsync<PageE, PageD>(
                async (journey, pageE, ct) =>
                {
                    // Cast the IJourney to specify WebSiteJourney.
                    var webSiteJourney = journey.OfType<WebSiteJourney>();


                    // validations
                    using var assertionScope = new AssertionScope();
                    webSiteJourney.CurrentNode.Should().BeEquivalentTo(pageE);

                    // Return a different page than origin
                    return await pageE.OpenPageD(journey, ct);
                }
            ).GotoAsync<PageC>();

        // Route validation
        await WebSiteJourney.VerifyRouteAsync();
    }
}
