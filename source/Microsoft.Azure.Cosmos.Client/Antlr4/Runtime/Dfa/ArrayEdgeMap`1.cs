// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.ArrayEdgeMap`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace Antlr4.Runtime.Dfa
{
  internal sealed class ArrayEdgeMap<T> : AbstractEdgeMap<T> where T : class
  {
    private readonly T[] arrayData;
    private int size;

    public ArrayEdgeMap(int minIndex, int maxIndex)
      : base(minIndex, maxIndex)
    {
      this.arrayData = new T[maxIndex - minIndex + 1];
    }

    public override int Count => Thread.VolatileRead(ref this.size);

    public override bool IsEmpty => this.Count == 0;

    public override bool ContainsKey(int key) => (object) this[key] != null;

    public override T this[int key] => key < this.minIndex || key > this.maxIndex ? default (T) : Interlocked.CompareExchange<T>(ref this.arrayData[key - this.minIndex], default (T), default (T));

    public override AbstractEdgeMap<T> Put(int key, T value)
    {
      if (key >= this.minIndex && key <= this.maxIndex)
      {
        T obj = Interlocked.Exchange<T>(ref this.arrayData[key - this.minIndex], value);
        if ((object) obj == null && (object) value != null)
          Interlocked.Increment(ref this.size);
        else if ((object) obj != null && (object) value == null)
          Interlocked.Decrement(ref this.size);
      }
      return (AbstractEdgeMap<T>) this;
    }

    public override AbstractEdgeMap<T> Remove(int key) => this.Put(key, default (T));

    public override AbstractEdgeMap<T> PutAll(IEdgeMap<T> m)
    {
      if (m.IsEmpty)
        return (AbstractEdgeMap<T>) this;
      switch (m)
      {
        case ArrayEdgeMap<T> _:
          ArrayEdgeMap<T> arrayEdgeMap1 = (ArrayEdgeMap<T>) m;
          int num1 = Math.Max(this.minIndex, arrayEdgeMap1.minIndex);
          int num2 = Math.Min(this.maxIndex, arrayEdgeMap1.maxIndex);
          ArrayEdgeMap<T> arrayEdgeMap2 = this;
          for (int key = num1; key <= num2; ++key)
            arrayEdgeMap2 = (ArrayEdgeMap<T>) arrayEdgeMap2.Put(key, m[key]);
          return (AbstractEdgeMap<T>) arrayEdgeMap2;
        case SingletonEdgeMap<T> _:
          SingletonEdgeMap<T> singletonEdgeMap = (SingletonEdgeMap<T>) m;
          return this.Put(singletonEdgeMap.Key, singletonEdgeMap.Value);
        case SparseEdgeMap<T> _:
          SparseEdgeMap<T> sparseEdgeMap = (SparseEdgeMap<T>) m;
          lock (sparseEdgeMap)
          {
            int[] keys = sparseEdgeMap.Keys;
            IList<T> values = sparseEdgeMap.Values;
            ArrayEdgeMap<T> arrayEdgeMap3 = this;
            for (int index = 0; index < values.Count; ++index)
              arrayEdgeMap3 = (ArrayEdgeMap<T>) arrayEdgeMap3.Put(keys[index], values[index]);
            return (AbstractEdgeMap<T>) arrayEdgeMap3;
          }
        default:
          throw new NotSupportedException(string.Format("EdgeMap of type {0} is supported yet.", (object) m.GetType().FullName));
      }
    }

    public override AbstractEdgeMap<T> Clear() => (AbstractEdgeMap<T>) new EmptyEdgeMap<T>(this.minIndex, this.maxIndex);

    public override IDictionary<int, T> ToMap()
    {
      if (this.IsEmpty)
        return Antlr4.Runtime.Sharpen.Collections.EmptyMap<int, T>();
      IDictionary<int, T> map = (IDictionary<int, T>) new SortedDictionary<int, T>();
      for (int index = 0; index < this.arrayData.Length; ++index)
      {
        T obj = this.arrayData[index];
        if ((object) obj != null)
          map[index + this.minIndex] = obj;
      }
      return map;
    }
  }
}
