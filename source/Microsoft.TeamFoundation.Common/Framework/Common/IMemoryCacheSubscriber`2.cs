// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.IMemoryCacheSubscriber`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IMemoryCacheSubscriber<TKey, TValue>
  {
    void OnEntryLookupSucceeded(TKey key, TValue value);

    void OnEntryLookupFailed(TKey key);

    void OnEntryReplaced(
      TKey key,
      TValue previousValue,
      TValue newValue,
      MemoryCacheOperationStatistics stats);

    void OnEntryEvicted(TKey key, TValue value, MemoryCacheOperationStatistics stats);

    void OnEntryInvalidated(TKey key, TValue value, MemoryCacheOperationStatistics stats);

    void OnEntryRemoved(TKey key, TValue value, MemoryCacheOperationStatistics stats);

    void OnEntryAdded(TKey key, TValue value, MemoryCacheOperationStatistics stats);

    void OnCleared(MemoryCacheOperationStatistics stats);
  }
}
