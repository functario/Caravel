using Microsoft.Playwright;

namespace WebSite.Facade.POMs.Components;

public sealed class PageTitle
{
    private readonly IPage _page;

    public PageTitle(IPage page)
    {
        _page = page;
    }

    public ILocator TxtTitle => _page.GetByTestId("txt-title");
}
