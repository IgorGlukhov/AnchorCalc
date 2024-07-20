using AnchorCalc.Domain.Collections;

namespace AnchorCalc.Infrastructure.Collections;

internal class RotatableCollectionFactory : IRotatableCollectionFactory
{
    public IRotatableCollection<TItem> Create<TItem>(int capacity)
    {
        return new RotatableCollection<TItem>(capacity);
    }
}