// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Channels.MruCache`2
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Diagnostics;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Channels
{
  internal class MruCache<TKey, TValue>
    where TKey : class, IEquatable<TKey>
    where TValue : class
  {
    private LinkedList<TKey> mruList;
    private Dictionary<TKey, MruCache<TKey, TValue>.CacheEntry> items;
    private int lowWatermark;
    private int highWatermark;
    private MruCache<TKey, TValue>.CacheEntry mruEntry;

    public MruCache(int watermark)
      : this(watermark * 4 / 5, watermark)
    {
    }

    public MruCache(int lowWatermark, int highWatermark)
      : this(lowWatermark, highWatermark, (IEqualityComparer<TKey>) null)
    {
    }

    public MruCache(int lowWatermark, int highWatermark, IEqualityComparer<TKey> comparer)
    {
      DiagnosticUtility.DebugAssert(lowWatermark <= highWatermark, "");
      DiagnosticUtility.DebugAssert(lowWatermark > 0, "");
      this.lowWatermark = lowWatermark;
      this.highWatermark = highWatermark;
      this.mruList = new LinkedList<TKey>();
      if (comparer == null)
        this.items = new Dictionary<TKey, MruCache<TKey, TValue>.CacheEntry>();
      else
        this.items = new Dictionary<TKey, MruCache<TKey, TValue>.CacheEntry>(comparer);
    }

    public void Add(TKey key, TValue value)
    {
      DiagnosticUtility.DebugAssert((object) key != null, "");
      bool flag = false;
      try
      {
        if (this.items.Count == this.highWatermark)
        {
          int num = this.highWatermark - this.lowWatermark;
          for (int index = 0; index < num; ++index)
          {
            TKey key1 = this.mruList.Last.Value;
            this.mruList.RemoveLast();
            TValue obj = this.items[key1].value;
            this.items.Remove(key1);
            this.OnSingleItemRemoved(obj);
          }
        }
        MruCache<TKey, TValue>.CacheEntry cacheEntry;
        cacheEntry.node = this.mruList.AddFirst(key);
        cacheEntry.value = value;
        this.items.Add(key, cacheEntry);
        this.mruEntry = cacheEntry;
        flag = true;
      }
      finally
      {
        if (!flag)
          this.Clear();
      }
    }

    public void Clear()
    {
      this.mruList.Clear();
      this.items.Clear();
      this.mruEntry.value = default (TValue);
      this.mruEntry.node = (LinkedListNode<TKey>) null;
    }

    public bool Remove(TKey key)
    {
      DiagnosticUtility.DebugAssert((object) key != null, "");
      MruCache<TKey, TValue>.CacheEntry cacheEntry;
      if (!this.items.TryGetValue(key, out cacheEntry))
        return false;
      this.items.Remove(key);
      this.OnSingleItemRemoved(cacheEntry.value);
      this.mruList.Remove(cacheEntry.node);
      if (this.mruEntry.node == cacheEntry.node)
      {
        this.mruEntry.value = default (TValue);
        this.mruEntry.node = (LinkedListNode<TKey>) null;
      }
      return true;
    }

    protected virtual void OnSingleItemRemoved(TValue item)
    {
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      if (this.mruEntry.node != null && (object) key != null && key.Equals(this.mruEntry.node.Value))
      {
        value = this.mruEntry.value;
        return true;
      }
      MruCache<TKey, TValue>.CacheEntry cacheEntry;
      int num = this.items.TryGetValue(key, out cacheEntry) ? 1 : 0;
      value = cacheEntry.value;
      if (num == 0)
        return num != 0;
      if (this.mruList.Count <= 1)
        return num != 0;
      if (this.mruList.First == cacheEntry.node)
        return num != 0;
      this.mruList.Remove(cacheEntry.node);
      this.mruList.AddFirst(cacheEntry.node);
      this.mruEntry = cacheEntry;
      return num != 0;
    }

    internal int HighWatermark => this.highWatermark;

    internal Dictionary<TKey, MruCache<TKey, TValue>.CacheEntry> Items => this.items;

    internal struct CacheEntry
    {
      internal TValue value;
      internal LinkedListNode<TKey> node;
    }
  }
}
