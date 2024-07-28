// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.MemoryCacheDummySubscriber`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class MemoryCacheDummySubscriber<TKey, TValue> : IMemoryCacheSubscriber<TKey, TValue>
  {
    public void OnEntryLookupSucceeded(TKey key, TValue value)
    {
    }

    public void OnEntryLookupFailed(TKey key)
    {
    }

    public void OnEntryAdded(TKey key, TValue value, MemoryCacheOperationStatistics stats)
    {
    }

    public void OnEntryReplaced(
      TKey key,
      TValue previousValue,
      TValue newValue,
      MemoryCacheOperationStatistics stats)
    {
    }

    public void OnEntryRemoved(TKey key, TValue value, MemoryCacheOperationStatistics stats)
    {
    }

    public void OnEntryEvicted(TKey key, TValue value, MemoryCacheOperationStatistics stats)
    {
    }

    public void OnEntryInvalidated(TKey key, TValue value, MemoryCacheOperationStatistics stats)
    {
    }

    public void OnCleared(MemoryCacheOperationStatistics stats)
    {
    }
  }
}
