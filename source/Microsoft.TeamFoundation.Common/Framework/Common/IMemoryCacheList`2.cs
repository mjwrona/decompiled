// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.IMemoryCacheList`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IMemoryCacheList<TKey, TValue>
  {
    int Count { get; }

    bool Add(
      TKey key,
      TValue value,
      bool overwrite,
      IMemoryCacheValidityPolicy<TKey, TValue> validityPolicy);

    bool TryGetValue(TKey key, out TValue value);

    bool Remove(TKey key);

    void Clear();

    int Sweep();
  }
}
