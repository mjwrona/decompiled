// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.UnlimitedCapacityMemoryCachePolicy`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UnlimitedCapacityMemoryCachePolicy<TKey, TValue> : 
    IMemoryCacheCapacityPolicy<TKey, TValue>
  {
    private int m_currentLength;

    public int Length => this.m_currentLength;

    public long Size => 0;

    public bool NeedRoom(TKey key, TValue value) => false;

    public bool NeedRoom(TKey key, TValue previousValue, TValue newValue) => false;

    public long OnEntryAdded(TKey key, TValue value)
    {
      ++this.m_currentLength;
      return 0;
    }

    public SizePair OnEntryReplaced(TKey key, TValue previousValue, TValue newValue) => new SizePair(0L, 0L);

    public long OnEntryRemoved(TKey key, TValue value)
    {
      --this.m_currentLength;
      return 0;
    }

    public void OnCleared() => this.m_currentLength = 0;
  }
}
