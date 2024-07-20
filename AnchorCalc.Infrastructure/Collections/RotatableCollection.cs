using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AnchorCalc.Domain.Collections;

namespace AnchorCalc.Infrastructure.Collections;

internal class RotatableCollection<TItem>(int capacity) : IRotatableCollection<TItem>
{
    private readonly ObservableCollection<TItem> _items = [];

    public IEnumerator<TItem> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => _items.CollectionChanged += value;
        remove => _items.CollectionChanged -= value;
    }

    public void Add(TItem item)
    {
        if (_items.Count == capacity) _items.RemoveAt(0);
        _items.Add(item);
    }

    public void Clear()
    {
        _items.Clear();
    }
}