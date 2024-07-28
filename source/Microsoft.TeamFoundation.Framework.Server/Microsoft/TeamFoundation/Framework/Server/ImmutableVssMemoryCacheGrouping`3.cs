// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ImmutableVssMemoryCacheGrouping`3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ImmutableVssMemoryCacheGrouping<TKey, TValue, TGroupingKey> : 
    VssMemoryCacheGrouping<TKey, TValue, TGroupingKey, ImmutableHashSet<TKey>>
  {
    public ImmutableVssMemoryCacheGrouping(
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
      ImmutableHashSet<TKey> immutableHashSet1 = (ImmutableHashSet<TKey>) null;
      foreach (TGroupingKey groupingKey in groupingKeys)
      {
        ImmutableHashSet<TKey> immutableHashSet2;
        if (!this.m_grouping.TryGetValue(groupingKey, out immutableHashSet2) || this.m_groupingBehavior == VssMemoryCacheGroupingBehavior.Replace)
        {
          if (immutableHashSet1 == null)
            immutableHashSet1 = ImmutableHashSet.Create<TKey>(this.m_keyComparer, key);
          immutableHashSet2 = immutableHashSet1;
        }
        else
          immutableHashSet2 = immutableHashSet2.Add(key);
        this.m_grouping[groupingKey] = immutableHashSet2;
      }
    }

    protected override void Remove(TKey key, TValue value, IEnumerable<TGroupingKey> groupingKeys)
    {
      foreach (TGroupingKey groupingKey in groupingKeys)
      {
        ImmutableHashSet<TKey> immutableHashSet;
        if (this.m_grouping.TryGetValue(groupingKey, out immutableHashSet))
        {
          if (immutableHashSet.Count == 1)
          {
            if (immutableHashSet.Contains(key))
              this.m_grouping.Remove(groupingKey);
          }
          else
            this.m_grouping[groupingKey] = immutableHashSet.Remove(key);
        }
      }
    }
  }
}
