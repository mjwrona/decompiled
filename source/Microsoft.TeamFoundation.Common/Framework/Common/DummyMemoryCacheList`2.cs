// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.DummyMemoryCacheList`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class DummyMemoryCacheList<TKey, TValue> : IMemoryCacheList<TKey, TValue>
  {
    public int Count => 0;

    public bool Add(
      TKey key,
      TValue value,
      bool overwrite,
      IMemoryCacheValidityPolicy<TKey, TValue> validityPolicy = null)
    {
      return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      value = default (TValue);
      return false;
    }

    public bool Remove(TKey key) => false;

    public void Clear()
    {
    }

    public int Sweep() => 0;
  }
}
