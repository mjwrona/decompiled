// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.CollectionDictionary`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CollectionDictionary<TKey, TElement> : ICollectionDictionary<TKey, TElement>
  {
    private CollectionDictionary<TKey, TElement>.CreateCollectionDelegate m_createCollectionHandler;
    private bool m_removeKeyOnEmpty;
    private Dictionary<TKey, ICollection<TElement>> m_innerDictionary;

    public CollectionDictionary(
      CollectionDictionary<TKey, TElement>.CreateCollectionDelegate createCollectionHandler,
      IEqualityComparer<TKey> comparer = null,
      bool removeKeyOnEmpty = true)
    {
      this.m_innerDictionary = new Dictionary<TKey, ICollection<TElement>>(comparer);
      this.m_createCollectionHandler = createCollectionHandler;
      this.m_removeKeyOnEmpty = removeKeyOnEmpty;
    }

    public void AddElement(TKey key, TElement element)
    {
      ICollection<TElement> elements;
      if (!this.m_innerDictionary.TryGetValue(key, out elements))
      {
        elements = this.m_createCollectionHandler();
        this.m_innerDictionary.Add(key, elements);
      }
      elements.Add(element);
    }

    public bool RemoveElement(TKey key, TElement element)
    {
      ICollection<TElement> elements;
      if (!this.m_innerDictionary.TryGetValue(key, out elements))
        return false;
      bool flag = elements.Remove(element);
      if (this.m_removeKeyOnEmpty & flag && elements.Count == 0)
        this.m_innerDictionary.Remove(key);
      return flag;
    }

    public bool Remove(TKey key) => this.m_innerDictionary.Remove(key);

    public void Clear() => this.m_innerDictionary.Clear();

    public bool TryGetValue(TKey key, out ICollection<TElement> collection) => this.m_innerDictionary.TryGetValue(key, out collection);

    public bool TryGetValue<TCollection>(TKey key, out TCollection collection) where TCollection : ICollection<TElement>
    {
      ICollection<TElement> elements;
      int num = this.m_innerDictionary.TryGetValue(key, out elements) ? 1 : 0;
      collection = (TCollection) elements;
      return num != 0;
    }

    public bool ContainsElement(TKey key, TElement element)
    {
      ICollection<TElement> elements;
      return this.m_innerDictionary.TryGetValue(key, out elements) && elements.Contains(element);
    }

    public bool ContainsKey(TKey key) => this.m_innerDictionary.ContainsKey(key);

    public int Count => this.m_innerDictionary.Count;

    public ICollection<TKey> Keys => (ICollection<TKey>) this.m_innerDictionary.Keys;

    public delegate ICollection<TElement> CreateCollectionDelegate();
  }
}
