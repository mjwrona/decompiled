// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.SingletonEdgeMap`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Dfa
{
  internal sealed class SingletonEdgeMap<T> : AbstractEdgeMap<T> where T : class
  {
    private readonly int key;
    private readonly T value;

    public SingletonEdgeMap(int minIndex, int maxIndex, int key, T value)
      : base(minIndex, maxIndex)
    {
      if (key >= minIndex && key <= maxIndex)
      {
        this.key = key;
        this.value = value;
      }
      else
      {
        this.key = 0;
        this.value = default (T);
      }
    }

    public int Key => this.key;

    public T Value => this.value;

    public override int Count => (object) this.value == null ? 0 : 1;

    public override bool IsEmpty => (object) this.value == null;

    public override bool ContainsKey(int key) => key == this.key && (object) this.value != null;

    public override T this[int key] => key == this.key ? this.value : default (T);

    public override AbstractEdgeMap<T> Put(int key, T value)
    {
      if (key < this.minIndex || key > this.maxIndex)
        return (AbstractEdgeMap<T>) this;
      if (key == this.key || (object) this.value == null)
        return (AbstractEdgeMap<T>) new SingletonEdgeMap<T>(this.minIndex, this.maxIndex, key, value);
      return (object) value != null ? new SparseEdgeMap<T>(this.minIndex, this.maxIndex).Put(this.key, this.value).Put(key, value) : (AbstractEdgeMap<T>) this;
    }

    public override AbstractEdgeMap<T> Remove(int key) => key == this.key && (object) this.value != null ? (AbstractEdgeMap<T>) new EmptyEdgeMap<T>(this.minIndex, this.maxIndex) : (AbstractEdgeMap<T>) this;

    public override AbstractEdgeMap<T> Clear() => (object) this.value != null ? (AbstractEdgeMap<T>) new EmptyEdgeMap<T>(this.minIndex, this.maxIndex) : (AbstractEdgeMap<T>) this;

    public override IDictionary<int, T> ToMap() => this.IsEmpty ? Antlr4.Runtime.Sharpen.Collections.EmptyMap<int, T>() : Antlr4.Runtime.Sharpen.Collections.SingletonMap<int, T>(this.key, this.value);
  }
}
