using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core.Extensions;
using Microsoft.Playwright;
using WebSite.Facade.POMs.Abstractions;
using WebSite.Facade.POMs.Components;

namespace WebSite.Facade.POMs.Pages;

public sealed class PageB : BasePage, IPOM
{
    private readonly IPage _page;

    public PageB(IPage page, NavigationButtons openButton, PageTitle pageTitle)
        : base(openButton, pageTitle)
    {
        _page = page;
    }

    public ImmutableHashSet<IEdge> GetEdges()
    {
        return
        [
            this.CreateEdge<PageA>(OpenNextPageAsync<PageA>),
            this.CreateEdge<PageC>(OpenNextPageAsync<PageC>),
            this.CreateEdge<PageD>(OpenNextPageAsync<PageD>),
            this.CreateEdge<PageE>(OpenNextPageAsync<PageE>),
        ];
    }
}
