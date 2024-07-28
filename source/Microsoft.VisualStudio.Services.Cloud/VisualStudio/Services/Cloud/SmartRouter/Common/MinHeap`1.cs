// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.MinHeap`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  public class MinHeap<T> : IEnumerable<T>, IEnumerable
  {
    public MinHeap()
    {
      this.Data = new List<T>();
      this.Comparer = (IComparer<T>) System.Collections.Generic.Comparer<T>.Default;
    }

    public MinHeap(int capacity)
      : this(capacity, (IComparer<T>) System.Collections.Generic.Comparer<T>.Default)
    {
    }

    public MinHeap(int capacity, IComparer<T> comparer)
    {
      this.Data = new List<T>(capacity);
      this.Comparer = comparer;
    }

    public bool IsEmpty => this.Data.Count == 0;

    public int Count => this.Data.Count;

    public int Capacity => this.Data.Capacity;

    public T Top
    {
      get
      {
        if (!this.IsEmpty)
          return this.Data[0];
        throw new InvalidOperationException("heap is empty");
      }
    }

    public void Add(T item)
    {
      int i = this.Data.Count;
      this.Data.Add(item);
      int j;
      for (; i > 0 && this.Comparer.Compare(item, this.Data[j = (i - 1) / 2]) < 0; i = j)
        this.Swap(i, j);
    }

    public T Extract()
    {
      if (this.IsEmpty)
        throw new InvalidOperationException("heap is empty");
      T obj = this.Data[0];
      T x = this.Data[0] = this.Data[this.Data.Count - 1];
      this.Data.RemoveAt(this.Data.Count - 1);
      int i = 0;
      while (true)
      {
        int index1 = (i << 1) + 1;
        int num = index1 >= this.Data.Count || this.Comparer.Compare(x, this.Data[index1]) <= 0 ? i : index1;
        int index2 = index1 + 1;
        if (index2 < this.Data.Count && this.Comparer.Compare(this.Data[num], this.Data[index2]) > 0)
          num = index2;
        if (num != i)
        {
          this.Swap(i, num);
          i = num;
        }
        else
          break;
      }
      return obj;
    }

    public void Clear() => this.Data.Clear();

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.Data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.Data.GetEnumerator();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(1024);
      foreach (T obj in this.Data)
        stringBuilder.AppendFormat("{0} ", (object) obj);
      if (stringBuilder.Length > 0)
        --stringBuilder.Length;
      return stringBuilder.ToString();
    }

    private void Swap(int i, int j)
    {
      List<T> data1 = this.Data;
      int index1 = j;
      List<T> data2 = this.Data;
      int num = i;
      T obj1 = this.Data[i];
      T obj2 = this.Data[j];
      data1[index1] = obj1;
      int index2 = num;
      T obj3 = obj2;
      data2[index2] = obj3;
    }

    private List<T> Data { get; }

    private IComparer<T> Comparer { get; }
  }
}
