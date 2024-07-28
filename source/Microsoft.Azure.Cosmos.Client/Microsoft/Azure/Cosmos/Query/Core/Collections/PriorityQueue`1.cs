// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Collections.PriorityQueue`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Collections
{
  internal sealed class PriorityQueue<T> : 
    IProducerConsumerCollection<T>,
    IEnumerable<T>,
    IEnumerable,
    ICollection
  {
    private const int DefaultInitialCapacity = 17;
    private readonly List<T> queue;

    public PriorityQueue(bool isSynchronized = false)
      : this(17, isSynchronized)
    {
    }

    public PriorityQueue(int initialCapacity, bool isSynchronized = false)
      : this(initialCapacity, (IComparer<T>) System.Collections.Generic.Comparer<T>.Default, isSynchronized)
    {
    }

    public PriorityQueue(IComparer<T> comparer, bool isSynchronized = false)
      : this(17, comparer, isSynchronized)
    {
    }

    public PriorityQueue(IEnumerable<T> enumerable, bool isSynchronized = false)
      : this(enumerable, (IComparer<T>) System.Collections.Generic.Comparer<T>.Default, isSynchronized)
    {
    }

    public PriorityQueue(IEnumerable<T> enumerable, IComparer<T> comparer, bool isSynchronized = false)
      : this(new List<T>(enumerable), comparer, isSynchronized)
    {
      this.Heapify();
    }

    public PriorityQueue(int initialCapacity, IComparer<T> comparer, bool isSynchronized = false)
      : this(new List<T>(initialCapacity), comparer, isSynchronized)
    {
    }

    private PriorityQueue(List<T> queue, IComparer<T> comparer, bool isSynchronized)
    {
      this.IsSynchronized = isSynchronized;
      this.queue = queue ?? throw new ArgumentNullException(nameof (queue));
      this.Comparer = comparer ?? throw new ArgumentNullException(nameof (comparer));
    }

    public int Count => this.queue.Count;

    public IComparer<T> Comparer { get; }

    public bool IsSynchronized { get; }

    public object SyncRoot => (object) this;

    public void CopyTo(T[] array, int index)
    {
      if (this.IsSynchronized)
      {
        lock (this.SyncRoot)
          this.CopyToPrivate(array, index);
      }
      else
        this.CopyToPrivate(array, index);
    }

    public bool TryAdd(T item)
    {
      this.Enqueue(item);
      return true;
    }

    public bool TryTake(out T item)
    {
      if (!this.IsSynchronized)
        return this.TryTakePrivate(out item);
      lock (this.SyncRoot)
        return this.TryTakePrivate(out item);
    }

    public bool TryPeek(out T item)
    {
      if (!this.IsSynchronized)
        return this.TryPeekPrivate(out item);
      lock (this.SyncRoot)
        return this.TryPeekPrivate(out item);
    }

    public void CopyTo(Array array, int index) => throw new NotImplementedException();

    public void Clear()
    {
      if (this.IsSynchronized)
      {
        lock (this.SyncRoot)
          this.ClearPrivate();
      }
      else
        this.ClearPrivate();
    }

    public bool Contains(T item)
    {
      if (!this.IsSynchronized)
        return this.ContainsPrivate(item);
      lock (this.SyncRoot)
        return this.ContainsPrivate(item);
    }

    public T Dequeue()
    {
      if (!this.IsSynchronized)
        return this.DequeuePrivate();
      lock (this.SyncRoot)
        return this.DequeuePrivate();
    }

    public void Enqueue(T item)
    {
      if (this.IsSynchronized)
      {
        lock (this.SyncRoot)
          this.EnqueuePrivate(item);
      }
      else
        this.EnqueuePrivate(item);
    }

    public void EnqueueRange(IEnumerable<T> items)
    {
      if (this.IsSynchronized)
      {
        lock (this.SyncRoot)
          this.EnqueueRangePrivate(items);
      }
      else
        this.EnqueueRangePrivate(items);
    }

    public T Peek()
    {
      if (!this.IsSynchronized)
        return this.PeekPrivate();
      lock (this.SyncRoot)
        return this.PeekPrivate();
    }

    public T[] ToArray()
    {
      if (!this.IsSynchronized)
        return this.ToArrayPrivate();
      lock (this.SyncRoot)
        return this.ToArrayPrivate();
    }

    public IEnumerator<T> GetEnumerator()
    {
      if (!this.IsSynchronized)
        return this.GetEnumeratorPrivate();
      lock (this.SyncRoot)
        return this.GetEnumeratorPrivate();
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    private void CopyToPrivate(T[] array, int index) => this.queue.CopyTo(array, index);

    private bool TryTakePrivate(out T item)
    {
      if (this.queue.Count <= 0)
      {
        item = default (T);
        return false;
      }
      item = this.DequeuePrivate();
      return true;
    }

    private bool TryPeekPrivate(out T item)
    {
      if (this.queue.Count <= 0)
      {
        item = default (T);
        return false;
      }
      item = this.PeekPrivate();
      return true;
    }

    private void ClearPrivate() => this.queue.Clear();

    private bool ContainsPrivate(T item) => this.queue.Contains(item);

    private T DequeuePrivate()
    {
      T obj = this.queue.Count > 0 ? this.queue[0] : throw new InvalidOperationException("No more elements");
      this.queue[0] = this.queue[this.queue.Count - 1];
      this.queue.RemoveAt(this.queue.Count - 1);
      this.DownHeap(0);
      return obj;
    }

    private void EnqueuePrivate(T item)
    {
      this.queue.Add(item);
      this.UpHeap(this.queue.Count - 1);
    }

    private void EnqueueRangePrivate(IEnumerable<T> items)
    {
      this.queue.AddRange(items);
      this.Heapify();
    }

    private T PeekPrivate() => this.queue.Count > 0 ? this.queue[0] : throw new InvalidOperationException("No more elements");

    private T[] ToArrayPrivate() => this.queue.ToArray();

    private IEnumerator<T> GetEnumeratorPrivate() => (IEnumerator<T>) new List<T>((IEnumerable<T>) this.queue).GetEnumerator();

    private void Heapify()
    {
      for (int parentIndex = this.GetParentIndex(this.Count); parentIndex >= 0; --parentIndex)
        this.DownHeap(parentIndex);
    }

    private void DownHeap(int itemIndex)
    {
      while (itemIndex < this.queue.Count)
      {
        int smallestChildIndex = this.GetSmallestChildIndex(itemIndex);
        if (smallestChildIndex == itemIndex)
          break;
        T obj = this.queue[itemIndex];
        this.queue[itemIndex] = this.queue[smallestChildIndex];
        itemIndex = smallestChildIndex;
        this.queue[itemIndex] = obj;
      }
    }

    private void UpHeap(int itemIndex)
    {
      while (itemIndex > 0)
      {
        int parentIndex = this.GetParentIndex(itemIndex);
        T y = this.queue[parentIndex];
        T x = this.queue[itemIndex];
        if (this.Comparer.Compare(x, y) >= 0)
          break;
        this.queue[itemIndex] = y;
        itemIndex = parentIndex;
        this.queue[itemIndex] = x;
      }
    }

    private int GetSmallestChildIndex(int parentIndex)
    {
      int index1 = parentIndex * 2 + 1;
      int index2 = index1 + 1;
      int index3 = parentIndex;
      if (index1 < this.queue.Count && this.Comparer.Compare(this.queue[index3], this.queue[index1]) > 0)
        index3 = index1;
      if (index2 < this.queue.Count && this.Comparer.Compare(this.queue[index3], this.queue[index2]) > 0)
        index3 = index2;
      return index3;
    }

    private int GetParentIndex(int childIndex) => (childIndex - 1) / 2;
  }
}
