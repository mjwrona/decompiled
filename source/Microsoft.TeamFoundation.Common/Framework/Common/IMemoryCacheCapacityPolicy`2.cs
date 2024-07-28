// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.IMemoryCacheCapacityPolicy`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IMemoryCacheCapacityPolicy<TKey, TValue>
  {
    int Length { get; }

    long Size { get; }

    bool NeedRoom(TKey key, TValue value);

    bool NeedRoom(TKey key, TValue previousValue, TValue newValue);

    long OnEntryAdded(TKey key, TValue value);

    SizePair OnEntryReplaced(TKey key, TValue previousValue, TValue newValue);

    long OnEntryRemoved(TKey key, TValue value);

    void OnCleared();
  }
}
