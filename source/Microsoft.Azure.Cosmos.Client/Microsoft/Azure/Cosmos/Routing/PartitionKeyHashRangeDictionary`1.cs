// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.PartitionKeyHashRangeDictionary`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal sealed class PartitionKeyHashRangeDictionary<T>
  {
    private readonly SortedDictionary<PartitionKeyHashRange, (bool, T)> dictionary;

    public PartitionKeyHashRangeDictionary(PartitionKeyHashRanges partitionKeyHashRanges)
    {
      if (partitionKeyHashRanges == null)
        throw new ArgumentNullException(nameof (partitionKeyHashRanges));
      this.dictionary = new SortedDictionary<PartitionKeyHashRange, (bool, T)>();
      foreach (PartitionKeyHashRange partitionKeyHashRange in partitionKeyHashRanges)
        this.dictionary.Add(partitionKeyHashRange, (false, default (T)));
    }

    public bool TryGetValue(PartitionKeyHash partitionKeyHash, out T value)
    {
      PartitionKeyHashRange range;
      if (this.TryGetContainingRange(partitionKeyHash, out range))
        return this.TryGetValue(range, out value);
      value = default (T);
      return false;
    }

    public bool TryGetValue(PartitionKeyHashRange partitionKeyHashRange, out T value)
    {
      (bool, T) tuple;
      if (!this.dictionary.TryGetValue(partitionKeyHashRange, out tuple))
      {
        value = default (T);
        return false;
      }
      if (!tuple.Item1)
      {
        value = default (T);
        return false;
      }
      value = tuple.Item2;
      return true;
    }

    public T this[PartitionKeyHash key]
    {
      get
      {
        T obj;
        if (!this.TryGetValue(key, out obj))
          throw new KeyNotFoundException();
        return obj;
      }
      set
      {
        PartitionKeyHashRange range;
        if (!this.TryGetContainingRange(key, out range))
          throw new NotSupportedException("Dictionary does not support adding new elements.");
        this.dictionary[range] = (true, value);
      }
    }

    public T this[PartitionKeyHashRange key]
    {
      get
      {
        T obj;
        if (!this.TryGetValue(key, out obj))
          throw new KeyNotFoundException();
        return obj;
      }
      set
      {
        if (!this.dictionary.TryGetValue(key, out (bool, T) _))
          throw new NotSupportedException("Dictionary does not support adding new elements.");
        this.dictionary[key] = (true, value);
      }
    }

    private bool TryGetContainingRange(
      PartitionKeyHash partitionKeyHash,
      out PartitionKeyHashRange range)
    {
      foreach (PartitionKeyHashRange key in this.dictionary.Keys)
      {
        if (key.Contains(partitionKeyHash))
        {
          range = key;
          return true;
        }
      }
      range = new PartitionKeyHashRange();
      return false;
    }
  }
}
