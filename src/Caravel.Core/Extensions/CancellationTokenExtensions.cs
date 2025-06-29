namespace Caravel.Core.Extensions;
public static class CancellationTokenExtensions
{
    public static void ThrowExceptionIfCancellationRequested(this CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            throw new TaskCanceledException("Timeout reached.");
        }
    }
}
