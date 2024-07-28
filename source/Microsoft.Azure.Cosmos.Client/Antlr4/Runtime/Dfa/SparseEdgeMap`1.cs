// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.SparseEdgeMap`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;

namespace Antlr4.Runtime.Dfa
{
  internal sealed class SparseEdgeMap<T> : AbstractEdgeMap<T> where T : class
  {
    private const int DefaultMaxSize = 5;
    private readonly int[] keys;
    private readonly List<T> values;

    public SparseEdgeMap(int minIndex, int maxIndex)
      : this(minIndex, maxIndex, 5)
    {
    }

    public SparseEdgeMap(int minIndex, int maxIndex, int maxSparseSize)
      : base(minIndex, maxIndex)
    {
      this.keys = new int[maxSparseSize];
      this.values = new List<T>(maxSparseSize);
    }

    private SparseEdgeMap(SparseEdgeMap<T> map, int maxSparseSize)
      : base(map.minIndex, map.maxIndex)
    {
      lock (map)
      {
        if (maxSparseSize < map.values.Count)
          throw new ArgumentException();
        this.keys = Arrays.CopyOf<int>(map.keys, maxSparseSize);
        this.values = new List<T>(maxSparseSize);
        this.values.AddRange((IEnumerable<T>) map.Values);
      }
    }

    public int[] Keys => this.keys;

    public IList<T> Values => (IList<T>) this.values;

    public int MaxSparseSize => this.keys.Length;

    public override int Count => this.values.Count;

    public override bool IsEmpty => this.values.Count == 0;

    public override bool ContainsKey(int key) => (object) this[key] != null;

    public override T this[int key]
    {
      get
      {
        int index = Array.BinarySearch<int>(this.keys, 0, this.Count, key);
        return index < 0 ? default (T) : this.values[index];
      }
    }

    public override AbstractEdgeMap<T> Put(int key, T value)
    {
      if (key < this.minIndex || key > this.maxIndex)
        return (AbstractEdgeMap<T>) this;
      if ((object) value == null)
        return this.Remove(key);
      lock (this)
      {
        int index1 = Array.BinarySearch<int>(this.keys, 0, this.Count, key);
        if (index1 >= 0)
        {
          this.values[index1] = value;
          return (AbstractEdgeMap<T>) this;
        }
        int index2 = -index1 - 1;
        if (this.Count < this.MaxSparseSize && index2 == this.Count)
        {
          this.keys[index2] = key;
          this.values.Add(value);
          return (AbstractEdgeMap<T>) this;
        }
        int maxSparseSize = this.Count >= this.MaxSparseSize ? this.MaxSparseSize * 2 : this.MaxSparseSize;
        int num = this.maxIndex - this.minIndex + 1;
        if (maxSparseSize >= num / 2)
        {
          ArrayEdgeMap<T> arrayEdgeMap = (ArrayEdgeMap<T>) new ArrayEdgeMap<T>(this.minIndex, this.maxIndex).PutAll((IEdgeMap<T>) this);
          arrayEdgeMap.Put(key, value);
          return (AbstractEdgeMap<T>) arrayEdgeMap;
        }
        SparseEdgeMap<T> sparseEdgeMap = new SparseEdgeMap<T>(this, maxSparseSize);
        Array.Copy((Array) sparseEdgeMap.keys, index2, (Array) sparseEdgeMap.keys, index2 + 1, this.Count - index2);
        sparseEdgeMap.keys[index2] = key;
        sparseEdgeMap.values.Insert(index2, value);
        return (AbstractEdgeMap<T>) sparseEdgeMap;
      }
    }

    public override AbstractEdgeMap<T> Remove(int key)
    {
      lock (this)
      {
        int num = Array.BinarySearch<int>(this.keys, 0, this.Count, key);
        if (num < 0)
          return (AbstractEdgeMap<T>) this;
        SparseEdgeMap<T> sparseEdgeMap = new SparseEdgeMap<T>(this, this.MaxSparseSize);
        Array.Copy((Array) sparseEdgeMap.keys, num + 1, (Array) sparseEdgeMap.keys, num, this.Count - num - 1);
        sparseEdgeMap.values.RemoveAt(num);
        return (AbstractEdgeMap<T>) sparseEdgeMap;
      }
    }

    public override AbstractEdgeMap<T> Clear() => this.IsEmpty ? (AbstractEdgeMap<T>) this : (AbstractEdgeMap<T>) new EmptyEdgeMap<T>(this.minIndex, this.maxIndex);

    public override IDictionary<int, T> ToMap()
    {
      if (this.IsEmpty)
        return Antlr4.Runtime.Sharpen.Collections.EmptyMap<int, T>();
      lock (this)
      {
        IDictionary<int, T> map = (IDictionary<int, T>) new SortedDictionary<int, T>();
        for (int index = 0; index < this.Count; ++index)
          map[this.keys[index]] = this.values[index];
        return map;
      }
    }
  }
}
