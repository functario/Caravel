namespace Caravel.Abstractions;

public interface IActionMetaDataFactory
{
    public string DefaultDoAsyncDescription { get; }
    public IActionMetaData CreateActionMetaData(string description);
}
