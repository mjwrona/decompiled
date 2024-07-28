// Decompiled with JetBrains decompiler
// Type: Nest.BulkOperationsCollection`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nest
{
  [ComVisible(false)]
  public sealed class BulkOperationsCollection<TOperation> : 
    IList<TOperation>,
    ICollection<TOperation>,
    IEnumerable<TOperation>,
    IEnumerable,
    IList,
    ICollection
    where TOperation : IBulkOperation
  {
    private readonly object _lock = new object();

    public BulkOperationsCollection() => this.Items = new List<TOperation>();

    public BulkOperationsCollection(IEnumerable<TOperation> operations)
    {
      this.Items = new List<TOperation>();
      this.Items.AddRange(operations);
    }

    public int Count
    {
      get
      {
        lock (this._lock)
          return this.Items.Count;
      }
    }

    public TOperation this[int index]
    {
      get
      {
        lock (this._lock)
          return this.Items[index];
      }
      set
      {
        lock (this._lock)
        {
          if (index < 0 || index >= this.Items.Count)
            throw new ArgumentOutOfRangeException(nameof (index), (object) index, string.Format("value {0} must be in range of {1}", (object) index, (object) this.Items.Count));
          this.Items[index] = value;
        }
      }
    }

    bool IList.IsFixedSize => false;

    bool ICollection<TOperation>.IsReadOnly => false;

    bool IList.IsReadOnly => false;

    bool ICollection.IsSynchronized => true;

    object IList.this[int index]
    {
      get => (object) this[index];
      set
      {
        BulkOperationsCollection<TOperation>.VerifyValueType(value);
        this[index] = (TOperation) value;
      }
    }

    private List<TOperation> Items { get; }

    object ICollection.SyncRoot => this._lock;

    void ICollection.CopyTo(Array array, int index)
    {
      lock (this._lock)
        ((ICollection) this.Items).CopyTo(array, index);
    }

    public void Add(TOperation item)
    {
      lock (this._lock)
        this.Items.Add(item);
    }

    public void Clear()
    {
      lock (this._lock)
        this.Items.Clear();
    }

    public bool Contains(TOperation item)
    {
      lock (this._lock)
        return this.Items.Contains(item);
    }

    public void CopyTo(TOperation[] array, int index)
    {
      lock (this._lock)
        this.Items.CopyTo(array, index);
    }

    public bool Remove(TOperation item)
    {
      lock (this._lock)
      {
        int index = this.InternalIndexOf(item);
        if (index < 0)
          return false;
        this.RemoveItem(index);
        return true;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.Items).GetEnumerator();

    public IEnumerator<TOperation> GetEnumerator()
    {
      lock (this._lock)
        return (IEnumerator<TOperation>) this.Items.GetEnumerator();
    }

    int IList.Add(object value)
    {
      BulkOperationsCollection<TOperation>.VerifyValueType(value);
      lock (this._lock)
      {
        this.Add((TOperation) value);
        return this.Count - 1;
      }
    }

    bool IList.Contains(object value)
    {
      BulkOperationsCollection<TOperation>.VerifyValueType(value);
      return this.Contains((TOperation) value);
    }

    int IList.IndexOf(object value)
    {
      BulkOperationsCollection<TOperation>.VerifyValueType(value);
      return this.IndexOf((TOperation) value);
    }

    void IList.Insert(int index, object value)
    {
      BulkOperationsCollection<TOperation>.VerifyValueType(value);
      this.Insert(index, (TOperation) value);
    }

    void IList.Remove(object value)
    {
      BulkOperationsCollection<TOperation>.VerifyValueType(value);
      this.Remove((TOperation) value);
    }

    public static implicit operator BulkOperationsCollection<TOperation>(List<TOperation> items) => new BulkOperationsCollection<TOperation>((IEnumerable<TOperation>) items);

    public int IndexOf(TOperation item)
    {
      lock (this._lock)
        return this.InternalIndexOf(item);
    }

    public void Insert(int index, TOperation item)
    {
      lock (this._lock)
      {
        if (index < 0 || index > this.Items.Count)
          throw new ArgumentOutOfRangeException(nameof (index), (object) index, string.Format("value {0} must be in range of {1}", (object) index, (object) this.Items.Count));
        this.InsertItem(index, item);
      }
    }

    public void RemoveAt(int index)
    {
      lock (this._lock)
      {
        if (index < 0 || index >= this.Items.Count)
          throw new ArgumentOutOfRangeException(nameof (index), (object) index, string.Format("value {0} must be in range of {1}", (object) index, (object) this.Items.Count));
        this.RemoveItem(index);
      }
    }

    public void AddRange(IEnumerable<TOperation> items)
    {
      lock (this._lock)
        this.Items.AddRange(items);
    }

    private int InternalIndexOf(TOperation item)
    {
      int count = this.Items.Count;
      for (int index = 0; index < count; ++index)
      {
        if (object.Equals((object) this.Items[index], (object) item))
          return index;
      }
      return -1;
    }

    private void InsertItem(int index, TOperation item) => this.Items.Insert(index, item);

    private void RemoveItem(int index) => this.Items.RemoveAt(index);

    private static void VerifyValueType(object value)
    {
      if (value == null)
      {
        if (typeof (TOperation).IsValueType)
          throw new ArgumentException("value is null and a value type");
      }
      else if (!(value is TOperation))
        throw new ArgumentException("object is of type " + value.GetType().FullName + " but collection is of " + typeof (TOperation).FullName);
    }
  }
}
