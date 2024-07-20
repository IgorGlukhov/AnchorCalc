using System.Collections.Specialized;

namespace AnchorCalc.Domain.Collections;

public interface IRotatableReadonlyCollection<out TItem> : IEnumerable<TItem>, INotifyCollectionChanged
{
}