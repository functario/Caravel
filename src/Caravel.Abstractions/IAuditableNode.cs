namespace Caravel.Abstractions;

public interface IAuditableNode : INode
{
    public Task<bool> Audit(IJourney journey, CancellationToken cancellationToken);
}
