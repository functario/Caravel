using Caravel.Abstractions;
using WebSite.Facade.POMs.Abstractions;
using WebSite.Facade.POMs.Components;

namespace WebSite.Facade.POMs.Pages;

public class BasePage
{
    public BasePage(NavigationButtons navigationButtons, PageTitle pageTitle)
    {
        NavigationButtons = navigationButtons;
        PageTitle = pageTitle;
    }

    public NavigationButtons NavigationButtons { get; }
    public PageTitle PageTitle { get; }

    public async Task<TPOM> OpenNextPageAsync<TPOM>(
        IJourney journey,
        CancellationToken cancellationToken
    )
        where TPOM : IPOM
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        cancellationToken.ThrowIfCancellationRequested();
        var btnOpen = typeof(TPOM) switch
        {
            var t when t == typeof(PageA) => NavigationButtons.BtnOpenPageA,
            var t when t == typeof(PageB) => NavigationButtons.BtnOpenPageB,
            var t when t == typeof(PageC) => NavigationButtons.BtnOpenPageC,
            var t when t == typeof(PageD) => NavigationButtons.BtnOpenPageD,
            var t when t == typeof(PageE) => NavigationButtons.BtnOpenPageE,
            _ => throw new NotSupportedException($"Unsupported POM type: {typeof(TPOM).Name}"),
        };

        await btnOpen.ClickAsync().WaitAsync(cancellationToken);
        return journey.OfType<WebSiteJourney>().Map.GetPOM<TPOM>();
    }
}
