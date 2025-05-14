using System;

public class TransactionHistory<T>
{
    private readonly T[] history;
    private int nextIndex = 0;
    private int count = 0;
    private readonly int capacity;
    private readonly Func<T> emptyFactory;

    public Action<T> OnAddedTransAction;
    public Action<T> OnRemovedTransAction;

    public int Count => count;
    public int Capacity => capacity;

    public TransactionHistory(int capacity, Func<T> emptyFactory)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.");

        this.capacity = capacity;
        this.emptyFactory = emptyFactory ?? throw new ArgumentNullException(nameof(emptyFactory));
        history = new T[this.capacity];

        for (int i = 0; i < this.capacity; i++)
            history[i] = this.emptyFactory();
    }

    public void Add(T item)
    {
        history[nextIndex] = item;
        nextIndex = (nextIndex + 1) % capacity;
        count = Math.Min(count + 1, capacity);

        if (OnAddedTransAction != null) OnAddedTransAction.Invoke(item);
    }

    public void UndoLast()
    {
        if (count == 0)
            throw new InvalidOperationException("No transactions to undo.");

        nextIndex = (nextIndex - 1 + capacity) % capacity;
        T item = history[nextIndex];
        history[nextIndex] = emptyFactory();
        count--;


        if (OnRemovedTransAction != null) OnRemovedTransAction.Invoke(item);
    }

    public void Clear()
    {
        for (int i = 0; i < capacity; i++)
            history[i] = emptyFactory();

        nextIndex = 0;
        count = 0;
    }

    public T GetLastEntry()
    {
        if (count == 0)
            throw new InvalidOperationException("No transactions available.");

        int lastIndex = (nextIndex - 1 + capacity) % capacity;
        return history[lastIndex];
    }
}
