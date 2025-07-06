using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core.Extensions;
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
        return
        [
            this.CreateEdge<PageA>(OpenNextPageAsync<PageA>),
            this.CreateEdge<PageB>(OpenNextPageAsync<PageB>),
            this.CreateEdge<PageC>(OpenNextPageAsync<PageC>),
            this.CreateEdge<PageD>(OpenNextPageAsync<PageD>),
        ];
    }

    public async Task<PageD> OpenPageD(IJourney journey, CancellationToken cancellationToken) =>
        await OpenNextPageAsync<PageD>(journey, cancellationToken);

    public async Task DoSomething(IJourney _, CancellationToken __)
    {
        await Task.FromResult(_page.Url);
    }
}
