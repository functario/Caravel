using System.Diagnostics.CodeAnalysis;

namespace WebSite.Facade.Extensions;

public static class EnvironmentVariableExtensions
{
    [SuppressMessage(
        "Usage",
        "CA2201:Do not raise reserved exception types",
        Justification = "Not production grade."
    )]
    [return: NotNull]
    public static string GetEnvironmentVariable(this string name)
    {
        return Environment.GetEnvironmentVariable(name)
            ?? throw new Exception($"Environment variable '{name}' not set, empty or null.");
    }
}
