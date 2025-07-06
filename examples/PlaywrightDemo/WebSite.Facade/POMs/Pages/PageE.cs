using System.Collections.Immutable;
using Caravel.Abstractions;
using Microsoft.Playwright;
using WebSite.Facade.POMs.Abstractions;
using WebSite.Facade.POMs.Components;

namespace WebSite.Facade.POMs.Pages;

public sealed class PageE : BasePage, IPOM
{
    private readonly IPage _page;

    public PageE(IPage page, NavigationButtons openButton, PageTitle pageTitle)
        : base(openButton, pageTitle)
    {
        _page = page;
    }

    public ImmutableHashSet<IEdge> GetEdges()
    {
        throw new NotImplementedException();
    }

    public async Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        await Assertions.Expect(PageTitle.TxtTitle).ToHaveTextAsync(nameof(PageE));
    }
}
