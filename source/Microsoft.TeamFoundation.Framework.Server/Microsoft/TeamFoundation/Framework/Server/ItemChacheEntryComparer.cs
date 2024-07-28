// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ItemChacheEntryComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ItemChacheEntryComparer : IComparer<ItemCacheEntry>
  {
    public int Compare(ItemCacheEntry cachedValue, ItemCacheEntry value)
    {
      DateTime? lastUpdateTime = (DateTime?) cachedValue.Item?.LastUpdateTime;
      DateTime t1 = lastUpdateTime ?? DateTime.MinValue;
      lastUpdateTime = (DateTime?) value.Item?.LastUpdateTime;
      DateTime t2 = lastUpdateTime ?? DateTime.MinValue;
      return DateTime.Compare(t1, t2) > 0 ? 1 : -1;
    }
  }
}
