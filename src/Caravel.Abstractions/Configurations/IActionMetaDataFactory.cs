namespace Caravel.Abstractions.Configurations;

public interface IActionMetaDataFactory
{
    public string DefaultDoAsyncDescription { get; }
    public IActionMetaData CreateActionMetaData(string description);
}
