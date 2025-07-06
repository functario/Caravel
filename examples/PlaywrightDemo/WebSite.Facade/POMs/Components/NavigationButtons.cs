using Microsoft.Playwright;

namespace WebSite.Facade.POMs.Components;

public sealed class NavigationButtons
{
    private readonly IPage _page;

    public NavigationButtons(IPage page)
    {
        _page = page;
    }

    public ILocator BtnOpenPageA => _page.GetByTestId("btn-PageA");
    public ILocator BtnOpenPageB => _page.GetByTestId("btn-PageB");
    public ILocator BtnOpenPageC => _page.GetByTestId("btn-PageC");
    public ILocator BtnOpenPageD => _page.GetByTestId("btn-PageD");
    public ILocator BtnOpenPageE => _page.GetByTestId("btn-PageE");
}
