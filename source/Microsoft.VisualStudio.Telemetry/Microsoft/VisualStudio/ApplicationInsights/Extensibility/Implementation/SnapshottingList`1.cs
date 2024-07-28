// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.SnapshottingList`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal class SnapshottingList<T> : 
    SnapshottingCollection<T, IList<T>>,
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    public SnapshottingList()
      : base((IList<T>) new List<T>())
    {
    }

    public T this[int index]
    {
      get => this.GetSnapshot()[index];
      set
      {
        lock (this.Collection)
        {
          this.Collection[index] = value;
          this.snapshot = (IList<T>) null;
        }
      }
    }

    public int IndexOf(T item) => this.GetSnapshot().IndexOf(item);

    public void Insert(int index, T item)
    {
      lock (this.Collection)
      {
        this.Collection.Insert(index, item);
        this.snapshot = (IList<T>) null;
      }
    }

    public void RemoveAt(int index)
    {
      lock (this.Collection)
      {
        this.Collection.RemoveAt(index);
        this.snapshot = (IList<T>) null;
      }
    }

    protected override sealed IList<T> CreateSnapshot(IList<T> collection) => (IList<T>) new List<T>((IEnumerable<T>) collection);
  }
}
