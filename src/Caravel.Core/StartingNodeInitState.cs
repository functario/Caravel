using Caravel.Abstractions;

namespace Caravel.Core;

internal sealed class StartingNodeInitState
{
    private readonly Lock _lock = new();
    private INode? _originalStartingNode;
    private bool _isSet;
    private bool _value;

    internal StartingNodeInitState(INode originalStartingNode)
    {
        _originalStartingNode = originalStartingNode;
        OriginalStartingNodeTypeFullName = originalStartingNode?.GetType()?.FullName;
    }

    internal string? OriginalStartingNodeTypeFullName { get; init; }

    internal bool Value
    {
        get
        {
            lock (_lock)
            {
                if (!_isSet)
                {
                    throw new InvalidOperationException(
                        "OriginalStartingNode value has not been set."
                    );
                }
                return _value;
            }
        }
        set
        {
            lock (_lock)
            {
                if (_isSet)
                {
                    throw new InvalidOperationException(
                        "OriginalStartingNode value is already set."
                    );
                }

                _value = value;
                _isSet = true;
                _originalStartingNode = null;
            }
        }
    }

    internal bool IsSet
    {
        get
        {
            lock (_lock)
            {
                return _isSet;
            }
        }
    }

    internal bool IsJourneyAlreadyStarted(INode currentNode) =>
        currentNode == _originalStartingNode;
}
