using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using WebSite.Facade;

var builder = Host.CreateDefaultBuilder(args);
DotEnv.Fluent().WithTrimValues().WithOverwriteExistingVars().Load();

var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 1000 });
var context = await browser.NewContextAsync();
var page = await context.NewPageAsync();

builder.ConfigureServices(
    (context, services) =>
    {
        services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly()
            .WithPromptsFromAssembly();

        services.AddSingleton<IPlaywright>(playwright);
        services.AddSingleton<IBrowser>(browser);
        services.AddSingleton<IPage>(page);
        services.AddWebSiteFacade(context);
        services.AddSingleton<WebSiteJourney>(x =>
        {
            var journeyBuilder = x.GetRequiredService<WebSiteJourneyBuilder>();
            return journeyBuilder.Create(CancellationToken.None);
        });
    }
);

var host = builder.Build();

// Add cleanup for Playwright resources
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(async () =>
{
    try
    {
        await page.CloseAsync();
        await context.CloseAsync();
        await browser.CloseAsync();
        playwright.Dispose();
    }
    finally
    {
        // Ignore cleanup errors
    }
});

var journey = host.Services.GetRequiredService<WebSiteJourney>();
await journey.App.OpenWebSiteAsync("", CancellationToken.None);

await host.RunAsync();
