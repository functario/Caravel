using Caravel.Abstractions.Exceptions;

namespace Caravel.Mermaid.Exceptions;

public class InvalidDescriptionException : CaravelException
{
    private static string DefaultMessage(string description, string pattern) =>
        $"The description '{description}' does not respect the regex pattern '{pattern}'.";

    public InvalidDescriptionException(string description, string pattern)
        : base(DefaultMessage(description, pattern)) { }
}
