using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using WebSite.Facade;

var builder = Host.CreateDefaultBuilder(args);

var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 1000 });
var context = await browser.NewContextAsync();
var page = await context.NewPageAsync();

builder.ConfigureServices(
    (context, services) =>
    {
        services.AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly();
        services
            .AddOptions<AppOptions>()
            .Bind(context.Configuration.GetSection(AppOptions.Name))
            .ValidateOnStart();

        services.ConfigureOptions<AppOptions>();
        services.AddSingleton<IPage>(page);
        services.AddWebSiteFacade(context);
        services.AddScoped<WebSiteJourney>(x =>
        {
            var journeyBuilder = x.GetRequiredService<WebSiteJourneyBuilder>();
            return journeyBuilder.Create(CancellationToken.None);
        });
    }
);

await builder.Build().RunAsync();
