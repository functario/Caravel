using System.Reflection;
using System.Runtime.CompilerServices;
using Castle.Core.Internal;

namespace Caravel.Core.UnitTests.Extensions;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "<Pending>"
)]
public static class VerifyExtensions
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.AddScrubber(x => x.Replace("\r\n", "\n"));
        VerifierSettings.UseStrictJson();
        VerifierSettings.DisableRequireUniquePrefix();

        DerivePathInfo(
            (_, projectDirectory, type, method) =>
            {
                // Resolve path based on namespace because default sourcefile is based on
                // VerifySettings constructor called emplacement.
                var root = Path.GetFileName(projectDirectory.TrimEnd(Path.DirectorySeparatorChar));
                var typePath = type
                    .FullName?.Replace(root, "", StringComparison.OrdinalIgnoreCase)
                    .Replace(".", "/", StringComparison.OrdinalIgnoreCase)
                    .TrimStart('/')!;

                var directory = Path.Combine(projectDirectory, $"{typePath}_Snapshots");

                var filePrefix = method.Name;
                var fileName = GetMethodDisplayName(method);

                return new PathInfo(directory, filePrefix, fileName);
            }
        );
    }

    public static SettingsTask VerifyMermaidGraphAsync(
        this string toVerify,
        params object?[] args
    ) => VerifyMermaidGraphAsync(toVerify, "mmd", args);

    public static SettingsTask VerifyMermaidGraphHtmlAsync(
        this string toVerify,
        params object?[] args
    ) => VerifyMermaidGraphAsync(toVerify, "html", args);

    private static SettingsTask VerifyMermaidGraphAsync(
        this string toVerify,
        string fileExtension,
        params object?[] args
    )
    {
        if (args.IsNullOrEmpty())
        {
            return Verify(toVerify, fileExtension);
        }

        return Verify(toVerify, fileExtension).UseParameters(args);
    }

    private static string GetMethodDisplayName(MethodInfo method)
    {
        string displayName;

        var testMethodAttribute = method
            .GetCustomAttributes()
            .FirstOrDefault(x =>
                x.GetType().Name == nameof(FactAttribute)
                || x.GetType().Name == nameof(TheoryAttribute)
            );

        if (testMethodAttribute is null)
        {
            displayName = method.Name;
        }
        else
        {
            var value =
                testMethodAttribute
                    .GetType()
                    .GetProperty("DisplayName", BindingFlags.Public | BindingFlags.Instance)
                    ?.GetValue(testMethodAttribute)
                    ?.ToString() ?? method.Name;

            displayName = value
                .Replace("'", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(",", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" ", "_", StringComparison.OrdinalIgnoreCase);
        }

        return displayName;
    }
}
