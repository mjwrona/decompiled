// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.EmptyEdgeMap`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Dfa
{
  internal sealed class EmptyEdgeMap<T> : AbstractEdgeMap<T> where T : class
  {
    public EmptyEdgeMap(int minIndex, int maxIndex)
      : base(minIndex, maxIndex)
    {
    }

    public override AbstractEdgeMap<T> Put(int key, T value) => (object) value == null || key < this.minIndex || key > this.maxIndex ? (AbstractEdgeMap<T>) this : (AbstractEdgeMap<T>) new SingletonEdgeMap<T>(this.minIndex, this.maxIndex, key, value);

    public override AbstractEdgeMap<T> Clear() => (AbstractEdgeMap<T>) this;

    public override AbstractEdgeMap<T> Remove(int key) => (AbstractEdgeMap<T>) this;

    public override int Count => 0;

    public override bool IsEmpty => true;

    public override bool ContainsKey(int key) => false;

    public override T this[int key] => default (T);

    public override IDictionary<int, T> ToMap() => (IDictionary<int, T>) new Dictionary<int, T>();
  }
}
