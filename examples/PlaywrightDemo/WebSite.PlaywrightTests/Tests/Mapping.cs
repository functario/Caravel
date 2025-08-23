using AwesomeAssertions;
using WebSite.Facade;
using WebSite.Facade.POMs.Pages;

namespace WebSite.PlaywrightTests.Tests;

public sealed class Mapping : TestBase
{
    public Mapping(PlaywrightFixture playwrightFixture)
        : base(playwrightFixture) { }

    [Fact(DisplayName = "Get pages from Map")]
    public async Task Test1()
    {
        await WebSiteJourney.App.OpenWebSiteAsync("", JourneyCTSource.Token);

        var map = WebSiteJourney.Map;

        map.PageA.Should().NotBeNull().And.BeOfType<PageA>();
        map.PageB.Should().NotBeNull().And.BeOfType<PageB>();
        map.PageC.Should().NotBeNull().And.BeOfType<PageC>();
        map.PageD.Should().NotBeNull().And.BeOfType<PageD>();
        map.PageE.Should().NotBeNull().And.BeOfType<PageE>();
    }
}
