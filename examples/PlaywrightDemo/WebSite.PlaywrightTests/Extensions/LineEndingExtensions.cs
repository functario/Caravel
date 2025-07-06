namespace WebSite.PlaywrightTests.Extensions;

internal static class LineEndingExtensions
{
    public static string ReplaceLineEndingsToLinux(this string text)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));
        return text.Replace("\r\n", "\n", StringComparison.OrdinalIgnoreCase);
    }
}
