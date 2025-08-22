using Caravel.Abstractions;

namespace Caravel.Core;

public sealed class ActionMetaDataFactory : IActionMetaDataFactory
{
    private const string DoAsyncDescription = $"{nameof(Journey)}.{nameof(IJourney.DoAsync)}";
    public string DefaultDoAsyncDescription => DoAsyncDescription;

    public IActionMetaData CreateActionMetaData(string description) =>
        new ActionMetaData(description);
}
