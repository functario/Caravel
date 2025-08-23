using Caravel.Abstractions;
using Caravel.Abstractions.Configurations;
using Caravel.Core.Configurations;
using Caravel.Graph.Dijkstra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebSite.Facade.Configurations;
using WebSite.Facade.Extensions;
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
        services.ConfigureAppOptions();

        services.AddScoped<App>();
        services.AddScoped<Map>();
        services.AddUIPagesAndComponents();

        // Graph constructors
        services.AddScoped<IEdgeFactory, EdgeFactory>();
        services.AddScoped<IRouteFactory, RouteFactory>();
        services.AddScoped<IGraph, DijkstraGraph>();

        // Journey configuration and builder
        services.AddJourneyConfiguration();
        services.AddScoped<WebSiteJourneyBuilder>();
        return services;
    }

    private static IServiceCollection AddJourneyConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        services.AddScoped<IJourneyConfiguration>(
            (x) =>
            {
                return JourneyConfigurationFactory.Create(
                    JourneyLegConfigurationOptions.InMemory,
                    x.GetRequiredService<TimeProvider>()
                );
            }
        );

        services.AddScoped<InMemoryJourneyLegStore>();
        return services;
    }

    private static IServiceCollection ConfigureAppOptions(this IServiceCollection services)
    {
        services.Configure<AppOptions>(options =>
        {
            options.SetFromEnvironmentVars();
        });

        return services;
    }

    private static IServiceCollection AddUIPagesAndComponents(this IServiceCollection services)
    {
        services.AddScoped<NavigationButtons>();
        services.AddScoped<PageTitle>();
        services.AddScoped<IStartingPOM, PageA>();

        Type[] pageTypes =
        [
            typeof(PageA),
            typeof(PageB),
            typeof(PageC),
            typeof(PageD),
            typeof(PageE),
        ];

        foreach (var pageType in pageTypes)
        {
            services.AddScoped(pageType);

            // Registered like INode to inject in Map
            services.AddScoped(typeof(IPOM), (x) => x.GetRequiredService(pageType));

            // Registered like INode to inject in DijkstraGraph
            services.AddScoped(typeof(INode), (x) => x.GetRequiredService(pageType));
        }

        return services;
    }
}
