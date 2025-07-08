using Caravel.Core;
using Caravel.Core.Extensions;
using WebSite.Facade;
using WebSite.Facade.POMs.Pages;
using WebSite.PlaywrightTests.Extensions;

namespace WebSite.PlaywrightTests.Tests;

public sealed class WaypointsAndExcludedNodes : TestBase
{
    public WaypointsAndExcludedNodes(PlaywrightFixture playwrightFixture)
        : base(playwrightFixture) { }

    [Fact(DisplayName = "From PageA Goto PageE with PageC and PageD as waypoints")]
    public async Task Test1()
    {
        await WebSiteJourney.App.OpenWebSiteAsync(JourneyCTSource.Token);

        // Force to pass by PageC then PageD, in order of declaration in Waypoints.
        Waypoints waypoints = [typeof(PageC), typeof(PageD)];
        await WebSiteJourney.GotoAsync<PageE>(waypoints);

        // Route validation
        await WebSiteJourney.VerifyRouteAsync();
    }

    [Fact(DisplayName = "From PageA Goto PageE with PageB as excluded node")]
    public async Task Test2()
    {
        await WebSiteJourney.App.OpenWebSiteAsync(JourneyCTSource.Token);

        // Given PageA-->PageE has a weight of 99,
        // the shortest route should be A, B, E.
        // but excluding PageB will resolve the next shortest one A, C, E.
        ExcludedNodes excludedNodes = [typeof(PageB)];
        await WebSiteJourney.GotoAsync<PageE>(excludedNodes);

        // Route validation
        await WebSiteJourney.VerifyRouteAsync();
    }

    [Fact(
        DisplayName = "From PageA Goto PageE with PageB as excluded node and PageD and PageC as waypoints"
    )]
    public async Task Test3()
    {
        await WebSiteJourney.App.OpenWebSiteAsync(JourneyCTSource.Token);

        // Given PageA-->PageE has a weight of 99,
        // the shortest route should be A, B, E.
        // but excluding PageB will resolve the next shortest one A, C, E.
        ExcludedNodes excludedNodes = [typeof(PageB)];
        Waypoints waypoints = [typeof(PageC), typeof(PageD)];
        await WebSiteJourney.GotoAsync<PageE>(waypoints, excludedNodes);

        // Route validation
        await WebSiteJourney.VerifyRouteAsync();
    }
}
