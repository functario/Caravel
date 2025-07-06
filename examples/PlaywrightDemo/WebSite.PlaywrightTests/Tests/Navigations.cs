using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Caravel.Core.Extensions;
using WebSite.Facade;
using WebSite.Facade.POMs.Pages;

namespace WebSite.PlaywrightTests.Tests;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1001:Types that own disposable fields should be disposable",
    Justification = "Implement IAsyncLifetime"
)]
public sealed class Navigations : TestBase
{
    public Navigations(PlaywrightFixture playwrightFixture)
        : base(playwrightFixture) { }

    [Fact]
    public async Task GotoB_Test()
    {
        await WebSiteJourney.App.OpenWebSiteAsync(JourneyCTSource.Token);
        await WebSiteJourney.GotoAsync<PageB>();
    }

    [Fact]
    public async Task GotoB_ThenE_ThenC_Test()
    {
        using var localCancellationTokenSource = new CancellationTokenSource(
            TimeSpan.FromSeconds(60)
        );

        var journeyCancellationToken = JourneyCTSource.Token;
        await WebSiteJourney.App.OpenWebSiteAsync(journeyCancellationToken);
        // csharpier-ignore
        await WebSiteJourney
            .GotoAsync<PageB>()
            .GotoAsync<PageE>()
            .GotoAsync<PageC>(
             localCancellationTokenSource.Token // local token merged with journeyCancellationToken
            );
    }

    [Fact]
    public async Task GotoThenE_ThenDoSomethingOnE_Test()
    {
        await WebSiteJourney.App.OpenWebSiteAsync(JourneyCTSource.Token);
        // csharpier-ignore
        await WebSiteJourney
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
                localCancellationTokenSource.Token // local token merged with journeyCancellationToken
            )
            .GotoAsync<PageA>();
    }

    [Fact]
    public async Task GotoThenE_ThenDoSomethingOpeningD_Test()
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
            );
    }
}
