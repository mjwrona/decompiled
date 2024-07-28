// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.Internal.Set`1
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class Set<T> : ICollection<T>, IEnumerable<T>, IEnumerable
  {
    private Dictionary<T, object> m_dictionary;

    public Set() => this.m_dictionary = new Dictionary<T, object>();

    public Set(int itemCount) => this.m_dictionary = new Dictionary<T, object>(itemCount);

    public Set(IEqualityComparer<T> equalityComparer) => this.m_dictionary = new Dictionary<T, object>(equalityComparer);

    public Set(int itemCount, IEqualityComparer<T> equalityComparer) => this.m_dictionary = new Dictionary<T, object>(itemCount, equalityComparer);

    public Set(IList<T> initialContents)
    {
      this.m_dictionary = new Dictionary<T, object>(initialContents.Count);
      this.AddRange((IEnumerable<T>) initialContents);
    }

    public Set(IList<T> initialContents, IEqualityComparer<T> equalityComparer)
    {
      this.m_dictionary = new Dictionary<T, object>(initialContents.Count, equalityComparer);
      this.AddRange((IEnumerable<T>) initialContents);
    }

    public void Add(T item) => this.m_dictionary[item] = (object) null;

    public void AddRange(IEnumerable<T> list)
    {
      foreach (T obj in list)
        this.Add(obj);
    }

    public void Clear() => this.m_dictionary.Clear();

    public bool Contains(T item) => this.m_dictionary.ContainsKey(item);

    public void CopyTo(T[] array, int index) => this.m_dictionary.Keys.CopyTo(array, index);

    public bool Remove(T item) => this.m_dictionary.Remove(item);

    public int Count => this.m_dictionary.Count;

    public bool IsReadOnly => false;

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.m_dictionary.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public T[] ToArray()
    {
      T[] array = new T[this.Count];
      this.CopyTo(array, 0);
      return array;
    }

    public List<T> ToList() => new List<T>((IEnumerable<T>) this.m_dictionary.Keys);
  }
}
