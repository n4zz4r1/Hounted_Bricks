using System.Collections.Generic;
using System.Threading;

namespace Framework.Base {
public class AtomicInt {
    private int _value;

    public AtomicInt(int initialValue) {
        _value = initialValue;
    }

    public int Value {
        get => Interlocked.CompareExchange(ref _value, 0, 0);
        set => Interlocked.Exchange(ref _value, value);
    }

    public int Increment() {
        return Interlocked.Increment(ref _value);
    }

    public int Decrement() {
        return Interlocked.Decrement(ref _value);
    }

    public int Add(int amount) {
        return Interlocked.Add(ref _value, amount);
    }

    public int Subtract(int amount) {
        return Interlocked.Add(ref _value, -amount);
    }
}

public class AtomicList<T> {
    private readonly List<T> _list = new();
    private readonly object _lock = new();

    public AtomicList() { }

    public AtomicList(IEnumerable<T> initialItems) {
        lock (_lock) {
            _list.AddRange(initialItems);
        }
    }

    // Count of items in the list
    public int Count {
        get {
            lock (_lock) {
                return _list.Count;
            }
        }
    }

    // Add an item to the list
    public void Add(T item) {
        lock (_lock) {
            _list.Add(item);
        }
    }

    // Remove an item from the list
    public bool Remove(T item) {
        lock (_lock) {
            return _list.Remove(item);
        }
    }

    // Get a copy of the list
    public List<T> ToList() {
        lock (_lock) {
            return new List<T>(_list);
        }
    }

    // Get an item at a specific index
    public T Get(int index) {
        lock (_lock) {
            return _list[index];
        }
    }

    // Set an item at a specific index
    public void Set(int index, T item) {
        lock (_lock) {
            _list[index] = item;
        }
    }
}
}