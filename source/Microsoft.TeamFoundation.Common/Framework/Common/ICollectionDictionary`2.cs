// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.ICollectionDictionary`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface ICollectionDictionary<TKey, TElement>
  {
    void AddElement(TKey key, TElement element);

    bool RemoveElement(TKey key, TElement element);

    bool Remove(TKey key);

    void Clear();

    bool TryGetValue(TKey key, out ICollection<TElement> collection);

    bool TryGetValue<TCollection>(TKey key, out TCollection collection) where TCollection : ICollection<TElement>;

    bool ContainsElement(TKey key, TElement element);

    bool ContainsKey(TKey key);

    int Count { get; }

    ICollection<TKey> Keys { get; }
  }
}
