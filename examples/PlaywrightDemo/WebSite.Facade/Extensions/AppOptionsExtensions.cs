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

    public static AppOptions SetFrom(this AppOptions original, AppOptions other)
    {
        ArgumentNullException.ThrowIfNull(original, nameof(original));
        ArgumentNullException.ThrowIfNull(other, nameof(other));
        original.WebSitePath = other.WebSitePath;
        return original;
    }
}
