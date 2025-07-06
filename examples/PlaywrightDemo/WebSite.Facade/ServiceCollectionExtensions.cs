using Caravel.Abstractions;
using Caravel.Graph.Dijkstra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebSite.Facade.POMs.Abstractions;
using WebSite.Facade.POMs.Components;
using WebSite.Facade.POMs.Pages;

namespace WebSite.Facade;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebSiteFacade(
        this IServiceCollection services,
        HostBuilderContext _
    )
    {
        services.AddScoped<NavigationButtons>();
        services.AddScoped<PageTitle>();
        services.AddScoped<IStartingPOM, PageA>();

        // Registered like INode to inject in Map
        services.AddScoped<IPOM, PageA>();
        services.AddScoped<IPOM, PageB>();
        services.AddScoped<IPOM, PageC>();
        services.AddScoped<IPOM, PageD>();
        services.AddScoped<IPOM, PageE>();

        // Registered like INode to inject in DijkstraGraph
        services.AddScoped<INode, PageA>();
        services.AddScoped<INode, PageB>();
        services.AddScoped<INode, PageC>();
        services.AddScoped<INode, PageD>();
        services.AddScoped<INode, PageE>();

        services.AddScoped<App>();
        services.AddScoped<Map>();

        services.AddScoped<IGraph, DijkstraGraph>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<WebSiteJourneyBuilder>();
        return services;
    }
}
