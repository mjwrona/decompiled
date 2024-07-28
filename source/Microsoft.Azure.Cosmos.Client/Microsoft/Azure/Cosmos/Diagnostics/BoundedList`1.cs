// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Diagnostics.BoundedList`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Diagnostics
{
  internal sealed class BoundedList<T> : IEnumerable<T>, IEnumerable
  {
    private readonly int capacity;
    private List<T> elementList;
    private CircularQueue<T> circularQueue;

    public BoundedList(int capacity)
    {
      this.capacity = capacity >= 1 ? capacity : throw new ArgumentOutOfRangeException("BoundedList capacity must be positive");
      this.elementList = new List<T>();
      this.circularQueue = (CircularQueue<T>) null;
    }

    public void Add(T element)
    {
      if (this.circularQueue != null)
        this.circularQueue.Add(element);
      else if (this.elementList.Count < this.capacity)
      {
        this.elementList.Add(element);
      }
      else
      {
        this.circularQueue = new CircularQueue<T>(this.capacity);
        this.circularQueue.AddRange((IEnumerable<T>) this.elementList);
        this.elementList = (List<T>) null;
        this.circularQueue.Add(element);
      }
    }

    public void AddRange(IEnumerable<T> elements)
    {
      foreach (T element in elements)
        this.Add(element);
    }

    public IEnumerator<T> GetListEnumerator()
    {
      List<T> elements = this.elementList;
      for (int index = 0; index < elements.Count; ++index)
        yield return elements[index];
    }

    public IEnumerator<T> GetEnumerator() => this.circularQueue != null ? this.circularQueue.GetEnumerator() : this.GetListEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
