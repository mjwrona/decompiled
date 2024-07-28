// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LegacyVssMemoryCacheGrouping`3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LegacyVssMemoryCacheGrouping<TKey, TValue, TGroupingKey> : 
    VssMemoryCacheGrouping<TKey, TValue, TGroupingKey, HashSet<TKey>>
  {
    public LegacyVssMemoryCacheGrouping(
      VssMemoryCacheList<TKey, TValue> cache,
      Func<TKey, TValue, IEnumerable<TGroupingKey>> getGroupingKeys,
      IEqualityComparer<TGroupingKey> groupingComparer = null,
      VssMemoryCacheGroupingBehavior groupingBehavior = VssMemoryCacheGroupingBehavior.Append)
      : base(cache, getGroupingKeys, groupingComparer, groupingBehavior)
    {
    }

    protected override bool IsSynchronized => false;

    protected override void Add(TKey key, TValue value, IEnumerable<TGroupingKey> groupingKeys)
    {
      foreach (TGroupingKey groupingKey in groupingKeys)
      {
        HashSet<TKey> keySet;
        if (!this.m_grouping.TryGetValue(groupingKey, out keySet))
        {
          keySet = new HashSet<TKey>(this.m_keyComparer);
          this.m_grouping.Add(groupingKey, keySet);
        }
        else if (this.m_groupingBehavior == VssMemoryCacheGroupingBehavior.Replace)
          keySet.Clear();
        keySet.Add(key);
      }
    }

    protected override void Remove(TKey key, TValue value, IEnumerable<TGroupingKey> groupingKeys)
    {
      foreach (TGroupingKey groupingKey in groupingKeys)
      {
        HashSet<TKey> keySet;
        if (this.m_grouping.TryGetValue(groupingKey, out keySet))
        {
          keySet.Remove(key);
          if (keySet.Count == 0)
            this.m_grouping.Remove(groupingKey);
        }
      }
    }
  }
}
