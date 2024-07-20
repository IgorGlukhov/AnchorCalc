namespace AnchorCalc.Domain.Collections;

public interface IRotatableCollectionFactory
{
    IRotatableCollection<TItem> Create<TItem>(int capacity);
}