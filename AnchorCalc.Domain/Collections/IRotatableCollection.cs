namespace AnchorCalc.Domain.Collections;

public interface IRotatableCollection<TItem> : IRotatableReadonlyCollection<TItem>
{
    void Add(TItem item);
    void Clear();
}