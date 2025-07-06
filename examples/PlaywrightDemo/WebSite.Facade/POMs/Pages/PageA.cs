using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core.Extensions;
using Microsoft.Playwright;
using WebSite.Facade.POMs.Abstractions;
using WebSite.Facade.POMs.Components;

namespace WebSite.Facade.POMs.Pages;

public sealed class PageA : BasePage, IPOM
{
    private readonly IPage _page;

    public PageA(IPage page, NavigationButtons openButton, PageTitle pageTitle)
        : base(openButton, pageTitle)
    {
        _page = page;
    }

    public ILocator BtnOpen => _page.GetByTestId("btn-PageA");

    public ImmutableHashSet<IEdge> GetEdges()
    {
        return
        [
            this.CreateEdge<PageB>(OpenNextPage<PageB>),
            this.CreateEdge<PageC>(OpenNextPage<PageC>),
            this.CreateEdge<PageD>(OpenNextPage<PageD>),
            this.CreateEdge<PageE>(OpenNextPage<PageE>),
        ];
    }

    public async Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        await Assertions.Expect(PageTitle.TxtTitle).ToHaveTextAsync(nameof(PageA));
    }
}
