using WebSite.Facade.Configurations;

namespace WebSite.Facade.Extensions;

public static class AppOptionsExtensions
{
    public static AppOptions SetFromEnvironmentVars(this AppOptions appOptions)
    {
        ArgumentNullException.ThrowIfNull(appOptions, nameof(appOptions));
        appOptions.WebSitePath = EnvironmentVariablesNames.WebSiteFilePath.GetEnvironmentVariable();
        return appOptions;
    }
}
