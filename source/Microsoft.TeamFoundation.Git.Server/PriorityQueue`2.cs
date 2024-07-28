// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PriorityQueue`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class PriorityQueue<TValue, TPriority> : IComparer<PriorityQueue<TValue, TPriority>.Entry>
  {
    public readonly IEqualityComparer<TValue> ValueComparer;
    public readonly IComparer<TPriority> PriorityComparer;
    private readonly SortedSet<PriorityQueue<TValue, TPriority>.Entry> m_queue;
    private readonly Dictionary<TValue, PriorityQueue<TValue, TPriority>.Entry> m_values;
    private long m_nextUniqueifier;

    public PriorityQueue(
      IEqualityComparer<TValue> valueComparer,
      IComparer<TPriority> priorityComparer)
    {
      this.ValueComparer = valueComparer;
      this.PriorityComparer = priorityComparer;
      this.m_queue = new SortedSet<PriorityQueue<TValue, TPriority>.Entry>((IComparer<PriorityQueue<TValue, TPriority>.Entry>) this);
      this.m_values = new Dictionary<TValue, PriorityQueue<TValue, TPriority>.Entry>(this.ValueComparer);
    }

    public void EnqueueOrUpdate(TValue value, TPriority priority)
    {
      PriorityQueue<TValue, TPriority>.Entry entry1;
      long uniqueifier;
      if (this.m_values.TryGetValue(value, out entry1))
      {
        this.m_queue.Remove(entry1);
        uniqueifier = entry1.Uniqueifier;
      }
      else
        uniqueifier = checked (this.m_nextUniqueifier++);
      PriorityQueue<TValue, TPriority>.Entry entry2 = new PriorityQueue<TValue, TPriority>.Entry(value, priority, uniqueifier);
      this.m_values[value] = entry2;
      this.m_queue.Add(entry2);
    }

    public bool TryDequeueBeforeThreshold(TPriority threshold, out TValue value)
    {
      PriorityQueue<TValue, TPriority>.Entry min;
      if (this.m_queue.Count == 0 || this.PriorityComparer.Compare((min = this.m_queue.Min).Priority, threshold) >= 0)
      {
        value = default (TValue);
        return false;
      }
      this.m_values.Remove(min.Value);
      this.m_queue.Remove(min);
      value = min.Value;
      return true;
    }

    public bool TryGetPriority(TValue value, out TPriority priority)
    {
      PriorityQueue<TValue, TPriority>.Entry entry;
      if (this.m_values.TryGetValue(value, out entry))
      {
        priority = entry.Priority;
        return true;
      }
      priority = default (TPriority);
      return false;
    }

    public TPriority NextPriority => this.m_queue.Min.Priority;

    public int Count => this.m_queue.Count;

    public void Clear()
    {
      this.m_queue.Clear();
      this.m_values.Clear();
      this.m_nextUniqueifier = 0L;
    }

    int IComparer<PriorityQueue<TValue, TPriority>.Entry>.Compare(
      PriorityQueue<TValue, TPriority>.Entry x,
      PriorityQueue<TValue, TPriority>.Entry y)
    {
      int num = this.PriorityComparer.Compare(x.Priority, y.Priority);
      if (num == 0)
        num = Math.Sign(x.Uniqueifier - y.Uniqueifier);
      return num;
    }

    private struct Entry
    {
      public readonly TValue Value;
      public readonly TPriority Priority;
      public readonly long Uniqueifier;

      public Entry(TValue value, TPriority priority, long uniqueifier)
      {
        this.Value = value;
        this.Priority = priority;
        this.Uniqueifier = uniqueifier;
      }
    }
  }
}
