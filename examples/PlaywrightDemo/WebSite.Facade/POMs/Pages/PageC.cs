using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core.Extensions;
using Microsoft.Playwright;
using WebSite.Facade.POMs.Abstractions;
using WebSite.Facade.POMs.Components;

namespace WebSite.Facade.POMs.Pages;

public sealed class PageC : BasePage, IPOM
{
    private readonly IPage _page;

    public PageC(IPage page, NavigationButtons openButton, PageTitle pageTitle)
        : base(openButton, pageTitle)
    {
        _page = page;
    }

    public ImmutableHashSet<IEdge> GetEdges()
    {
        return
        [
            this.CreateEdge<PageA>(OpenNextPageAsync<PageA>),
            this.CreateEdge<PageB>(OpenNextPageAsync<PageB>),
            this.CreateEdge<PageD>(OpenNextPageAsync<PageD>),
            this.CreateEdge<PageE>(OpenNextPageAsync<PageE>),
        ];
    }
}
