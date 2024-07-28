// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.SnapshottingCollection`2
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal abstract class SnapshottingCollection<TItem, TCollection> : 
    ICollection<TItem>,
    IEnumerable<TItem>,
    IEnumerable
    where TCollection : class, ICollection<TItem>
  {
    protected readonly TCollection Collection;
    protected TCollection snapshot;

    protected SnapshottingCollection(TCollection collection) => this.Collection = collection;

    public int Count => this.GetSnapshot().Count;

    public bool IsReadOnly => false;

    public void Add(TItem item)
    {
      lock ((object) this.Collection)
      {
        this.Collection.Add(item);
        this.snapshot = default (TCollection);
      }
    }

    public void Clear()
    {
      lock ((object) this.Collection)
      {
        this.Collection.Clear();
        this.snapshot = default (TCollection);
      }
    }

    public bool Contains(TItem item) => this.GetSnapshot().Contains(item);

    public void CopyTo(TItem[] array, int arrayIndex) => this.GetSnapshot().CopyTo(array, arrayIndex);

    public bool Remove(TItem item)
    {
      lock ((object) this.Collection)
      {
        int num = this.Collection.Remove(item) ? 1 : 0;
        if (num != 0)
          this.snapshot = default (TCollection);
        return num != 0;
      }
    }

    public IEnumerator<TItem> GetEnumerator() => this.GetSnapshot().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    protected abstract TCollection CreateSnapshot(TCollection collection);

    protected TCollection GetSnapshot()
    {
      TCollection snapshot = this.snapshot;
      if ((object) snapshot == null)
      {
        lock ((object) this.Collection)
        {
          this.snapshot = this.CreateSnapshot(this.Collection);
          snapshot = this.snapshot;
        }
      }
      return snapshot;
    }
  }
}
