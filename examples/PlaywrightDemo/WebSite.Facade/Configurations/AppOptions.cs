using Microsoft.Extensions.Options;

namespace WebSite.Facade.Configurations;

public sealed class AppOptions : IValidateOptions<AppOptions>
{
    public const string Name = "AppOptions";

    public string WebSitePath { get; set; } = null!;

    public ValidateOptionsResult Validate(string? name, AppOptions options)
    {
        if (string.IsNullOrWhiteSpace(options?.WebSitePath))
        {
            return ValidateOptionsResult.Fail("Missing path to 'WebSite.html'");
        }

        return ValidateOptionsResult.Success;
    }
}
