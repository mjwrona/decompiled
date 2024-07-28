// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.CircularBuffer`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class CircularBuffer<T> : 
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private readonly List<T> innerList;
    private int head;
    private readonly int capacity;

    public CircularBuffer(int capacity)
    {
      ArgumentUtility.CheckForNonPositiveInt(capacity, nameof (capacity));
      this.innerList = new List<T>(capacity);
      this.capacity = capacity;
      this.head = 0;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
      for (int i = 0; i < this.Count; ++i)
        yield return this[i];
    }

    public void Add(T item)
    {
      if (this.Count < this.capacity)
      {
        this.innerList.Add(item);
      }
      else
      {
        this.head = this.GetInnerIndex(1);
        this[this.capacity - 1] = item;
      }
    }

    public int Count => this.innerList.Count;

    private int GetInnerIndex(int outerIndex)
    {
      if (outerIndex < 0 || outerIndex >= this.Count)
        throw new IndexOutOfRangeException();
      return (outerIndex + this.head) % this.capacity;
    }

    public T this[int index]
    {
      get => this.innerList[this.GetInnerIndex(index)];
      set => this.innerList[this.GetInnerIndex(index)] = value;
    }
  }
}
