// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Heap`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class Heap<TValue, TPriority>
  {
    private readonly IComparer<TPriority> m_priorityComparer;
    private long m_numInsertions;
    private bool m_heapified;
    private readonly List<Heap<TValue, TPriority>.Entry> m_heap;

    public Heap(IComparer<TPriority> priorityComparer)
    {
      this.m_priorityComparer = priorityComparer;
      this.m_heap = new List<Heap<TValue, TPriority>.Entry>();
    }

    public void Enqueue(TValue value, TPriority priority)
    {
      this.m_heap.Add(new Heap<TValue, TPriority>.Entry(value, priority, this.m_numInsertions));
      ++this.m_numInsertions;
      if (!this.m_heapified)
        return;
      this.BackwardFixHeap(this.m_heap.Count - 1);
    }

    public bool TryDequeueBeforeThreshold(TPriority threshold, out TValue value)
    {
      if (this.m_heap.Count == 0)
      {
        value = default (TValue);
        return false;
      }
      this.Heapify();
      Heap<TValue, TPriority>.Entry entry = this.m_heap[0];
      if (this.m_priorityComparer.Compare(entry.Priority, threshold) >= 0)
      {
        value = default (TValue);
        return false;
      }
      if (this.m_heap.Count == 1)
      {
        this.m_heap.Clear();
        this.m_heapified = false;
      }
      else
      {
        this.m_heap[0] = this.m_heap[this.m_heap.Count - 1];
        this.m_heap.RemoveAt(this.m_heap.Count - 1);
        this.ForwardFixHeap(0);
      }
      value = entry.Value;
      return true;
    }

    public void Clear()
    {
      this.m_numInsertions = 0L;
      this.m_heap.Clear();
    }

    public int Compare(Heap<TValue, TPriority>.Entry x, Heap<TValue, TPriority>.Entry y)
    {
      int num = this.m_priorityComparer.Compare(x.Priority, y.Priority);
      if (num != 0)
        return num;
      if (x.InsertOrder > y.InsertOrder)
        return 1;
      return x.InsertOrder < y.InsertOrder ? -1 : 0;
    }

    private void Heapify()
    {
      if (this.m_heapified)
        return;
      for (int index = this.m_heap.Count / 2; index >= 0; --index)
        this.ForwardFixHeap(index);
      this.m_heapified = true;
    }

    private void ForwardFixHeap(int index)
    {
      Heap<TValue, TPriority>.Entry x1 = this.m_heap[index];
      int index1;
      for (; index < this.m_heap.Count; index = index1)
      {
        int index2 = 2 * index + 1;
        int index3 = 2 * index + 2;
        if (index2 >= this.m_heap.Count)
          break;
        Heap<TValue, TPriority>.Entry entry;
        if (index3 < this.m_heap.Count)
        {
          Heap<TValue, TPriority>.Entry x2 = this.m_heap[index2];
          Heap<TValue, TPriority>.Entry y = this.m_heap[index3];
          if (this.Compare(x2, y) <= 0)
          {
            index1 = index2;
            entry = x2;
          }
          else
          {
            index1 = index3;
            entry = y;
          }
        }
        else
        {
          index1 = index2;
          entry = this.m_heap[index2];
        }
        if (this.Compare(x1, this.m_heap[index1]) <= 0)
          break;
        this.m_heap[index] = entry;
        this.m_heap[index1] = x1;
      }
    }

    private void BackwardFixHeap(int index)
    {
      Heap<TValue, TPriority>.Entry x = this.m_heap[index];
      int index1;
      for (; index > 0; index = index1)
      {
        index1 = (index - 1) / 2;
        Heap<TValue, TPriority>.Entry entry = this.m_heap[index1];
        if (this.Compare(x, this.m_heap[index1]) >= 0)
          break;
        this.m_heap[index] = entry;
        this.m_heap[index1] = x;
      }
    }

    [DebuggerDisplay("Value: {Value}, Priority: {Priority}, InsertOrder: {InsertOrder}")]
    public struct Entry
    {
      public Entry(TValue value, TPriority priority, long insertOrder)
      {
        this.Value = value;
        this.Priority = priority;
        this.InsertOrder = insertOrder;
      }

      public TValue Value { get; }

      public TPriority Priority { get; }

      public long InsertOrder { get; }
    }
  }
}
