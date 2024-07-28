// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.AbstractEdgeMap`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections;
using System.Collections.Generic;

namespace Antlr4.Runtime.Dfa
{
  internal abstract class AbstractEdgeMap<T> : 
    IEdgeMap<T>,
    IEnumerable<KeyValuePair<int, T>>,
    IEnumerable
    where T : class
  {
    protected internal readonly int minIndex;
    protected internal readonly int maxIndex;

    protected AbstractEdgeMap(int minIndex, int maxIndex)
    {
      this.minIndex = minIndex;
      this.maxIndex = maxIndex;
    }

    public abstract AbstractEdgeMap<T> Put(int key, T value);

    IEdgeMap<T> IEdgeMap<T>.Put(int key, T value) => (IEdgeMap<T>) this.Put(key, value);

    public virtual AbstractEdgeMap<T> PutAll(IEdgeMap<T> m)
    {
      AbstractEdgeMap<T> abstractEdgeMap = this;
      foreach (KeyValuePair<int, T> keyValuePair in (IEnumerable<KeyValuePair<int, T>>) m)
        abstractEdgeMap = abstractEdgeMap.Put(keyValuePair.Key, keyValuePair.Value);
      return abstractEdgeMap;
    }

    IEdgeMap<T> IEdgeMap<T>.PutAll(IEdgeMap<T> m) => (IEdgeMap<T>) this.PutAll(m);

    public abstract AbstractEdgeMap<T> Clear();

    IEdgeMap<T> IEdgeMap<T>.Clear() => (IEdgeMap<T>) this.Clear();

    public abstract AbstractEdgeMap<T> Remove(int key);

    IEdgeMap<T> IEdgeMap<T>.Remove(int key) => (IEdgeMap<T>) this.Remove(key);

    public abstract bool ContainsKey(int arg1);

    public abstract T this[int arg1] { get; }

    public abstract bool IsEmpty { get; }

    public abstract int Count { get; }

    public abstract IDictionary<int, T> ToMap();

    public virtual IEnumerator<KeyValuePair<int, T>> GetEnumerator() => this.ToMap().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
