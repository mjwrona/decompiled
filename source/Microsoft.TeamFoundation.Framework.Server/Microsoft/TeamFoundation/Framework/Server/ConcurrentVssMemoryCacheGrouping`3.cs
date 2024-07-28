// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConcurrentVssMemoryCacheGrouping`3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ConcurrentVssMemoryCacheGrouping<TKey, TValue, TGroupingKey> : 
    VssMemoryCacheGrouping<TKey, TValue, TGroupingKey, ConcurrentHashSet<TKey>>
  {
    public ConcurrentVssMemoryCacheGrouping(
      VssMemoryCacheList<TKey, TValue> cache,
      Func<TKey, TValue, IEnumerable<TGroupingKey>> getGroupingKeys,
      IEqualityComparer<TGroupingKey> groupingComparer = null,
      VssMemoryCacheGroupingBehavior groupingBehavior = VssMemoryCacheGroupingBehavior.Append)
      : base(cache, getGroupingKeys, groupingComparer, groupingBehavior)
    {
    }

    protected override bool IsSynchronized => true;

    protected override void Add(TKey key, TValue value, IEnumerable<TGroupingKey> groupingKeys)
    {
      foreach (TGroupingKey groupingKey in groupingKeys)
      {
        ConcurrentHashSet<TKey> concurrentHashSet;
        if (!this.m_grouping.TryGetValue(groupingKey, out concurrentHashSet))
        {
          concurrentHashSet = new ConcurrentHashSet<TKey>(this.m_keyComparer);
          this.m_grouping.Add(groupingKey, concurrentHashSet);
        }
        else if (this.m_groupingBehavior == VssMemoryCacheGroupingBehavior.Replace)
          concurrentHashSet.Clear();
        concurrentHashSet.Add(key);
      }
    }

    protected override void Remove(TKey key, TValue value, IEnumerable<TGroupingKey> groupingKeys)
    {
      foreach (TGroupingKey groupingKey in groupingKeys)
      {
        ConcurrentHashSet<TKey> concurrentHashSet;
        if (this.m_grouping.TryGetValue(groupingKey, out concurrentHashSet) && concurrentHashSet.Remove(key) && concurrentHashSet.IsEmpty)
          this.m_grouping.Remove(groupingKey);
      }
    }
  }
}
