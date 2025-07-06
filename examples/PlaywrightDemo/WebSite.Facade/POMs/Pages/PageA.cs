using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core.Extensions;
using Microsoft.Playwright;
using WebSite.Facade.POMs.Abstractions;
using WebSite.Facade.POMs.Components;

namespace WebSite.Facade.POMs.Pages;

public sealed class PageA : BasePage, IStartingPOM
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
            this.CreateEdge<PageB>(OpenNextPageAsync<PageB>),
            this.CreateEdge<PageC>(OpenNextPageAsync<PageC>),
            this.CreateEdge<PageD>(OpenNextPageAsync<PageD>),
            this.CreateEdge<PageE>(OpenNextPageAsync<PageE>),
        ];
    }
}
