// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssMemoryCacheGroupingFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class VssMemoryCacheGroupingFactory
  {
    public static IVssMemoryCacheGrouping<TKey, TValue, TGroupingKey> Create<TKey, TValue, TGroupingKey>(
      IVssRequestContext requestContext,
      VssMemoryCacheList<TKey, TValue> cache,
      Func<TKey, TValue, IEnumerable<TGroupingKey>> getGroupingKeys,
      IEqualityComparer<TGroupingKey> groupingComparer = null,
      VssMemoryCacheGroupingBehavior groupingBehavior = VssMemoryCacheGroupingBehavior.Append)
    {
      return requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.UseConcurrentCacheGrouper") ? (IVssMemoryCacheGrouping<TKey, TValue, TGroupingKey>) new ConcurrentVssMemoryCacheGrouping<TKey, TValue, TGroupingKey>(cache, getGroupingKeys, groupingComparer, groupingBehavior) : (IVssMemoryCacheGrouping<TKey, TValue, TGroupingKey>) new ImmutableVssMemoryCacheGrouping<TKey, TValue, TGroupingKey>(cache, getGroupingKeys, groupingComparer, groupingBehavior);
    }
  }
}
