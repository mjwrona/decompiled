// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Diagnostics.CircularQueue`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Diagnostics
{
  internal sealed class CircularQueue<T> : IEnumerable<T>, IEnumerable
  {
    private readonly T[] buffer;
    private int head;
    private int tail;

    public int Capacity => this.buffer.Length;

    public bool Full => this.GetNextIndex(this.tail) == this.head;

    public bool Empty => this.tail == this.head;

    public CircularQueue(int capacity)
    {
      if (capacity < 1)
        throw new ArgumentOutOfRangeException("circular queue capacity must be positive");
      this.head = 0;
      this.tail = 0;
      this.buffer = new T[capacity + 1];
    }

    public void Add(T element)
    {
      if (this.Full)
        this.TryPop(out T _);
      this.buffer[this.tail] = element;
      this.tail = this.GetNextIndex(this.tail);
    }

    public void AddRange(IEnumerable<T> elements)
    {
      foreach (T element in elements)
        this.Add(element);
    }

    private int GetNextIndex(int index) => (index + 1) % this.Capacity;

    private bool TryPop(out T element)
    {
      element = default (T);
      if (this.Empty)
        return false;
      element = this.buffer[this.head];
      this.head = this.GetNextIndex(this.head);
      return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
      if (!this.Empty)
      {
        for (int i = this.head; i != this.tail; i = this.GetNextIndex(i))
          yield return this.buffer[i];
      }
    }
  }
}
