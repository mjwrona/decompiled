// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.DependencyCollection`1
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  internal class DependencyCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : CatalogObject
  {
    private ICollection<T> m_items;
    private CatalogObject m_parent;
    private string m_dependencyName;

    public DependencyCollection(
      CatalogObject parent,
      string dependencyName,
      ICollection<T> collection)
    {
      this.m_parent = parent;
      this.m_dependencyName = dependencyName;
      this.m_items = collection;
    }

    public void Add(T item)
    {
      this.m_items.Add(item);
      IDictionary<string, IList<CatalogNode>> sets = this.m_parent.CatalogNode.Dependencies.Sets;
      if (!sets.ContainsKey(this.m_dependencyName))
        sets.Add(this.m_dependencyName, (IList<CatalogNode>) new List<CatalogNode>());
      sets[this.m_dependencyName].Add(item.CatalogNode);
      this.m_parent.Context.ModifyObject(this.m_parent);
    }

    public void Clear()
    {
      foreach (T obj in this.m_items.ToList<T>())
        this.Remove(obj);
    }

    public bool Contains(T item) => this.m_items.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => this.m_items.CopyTo(array, arrayIndex);

    public int Count => this.m_items.Count;

    public bool IsReadOnly => false;

    public bool Remove(T item)
    {
      int num = this.m_items.Remove(item) ? 1 : 0;
      IDictionary<string, IList<CatalogNode>> sets = this.m_parent.CatalogNode.Dependencies.Sets;
      if (sets.ContainsKey(this.m_dependencyName))
        sets[this.m_dependencyName].Remove(item.CatalogNode);
      this.m_parent.Context.ModifyObject(this.m_parent);
      return num != 0;
    }

    public IEnumerator<T> GetEnumerator() => this.m_items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
