namespace Caravel.Abstractions;

public interface IAuditableNode : INode
{
    public Task<bool> AuditAsync(IJourney journey, CancellationToken cancellationToken);
}
