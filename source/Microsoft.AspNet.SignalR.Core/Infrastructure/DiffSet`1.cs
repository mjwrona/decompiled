// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.DiffSet`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class DiffSet<T>
  {
    private readonly HashSet<T> _items;
    private readonly HashSet<T> _addedItems;
    private readonly HashSet<T> _removedItems;

    public DiffSet(IEnumerable<T> items)
    {
      this._addedItems = new HashSet<T>();
      this._removedItems = new HashSet<T>();
      this._items = new HashSet<T>(items);
    }

    public bool Add(T item)
    {
      if (!this._items.Add(item))
        return false;
      if (!this._removedItems.Remove(item))
        this._addedItems.Add(item);
      return true;
    }

    public bool Remove(T item)
    {
      if (!this._items.Remove(item))
        return false;
      if (!this._addedItems.Remove(item))
        this._removedItems.Add(item);
      return true;
    }

    public bool Contains(T item) => this._items.Contains(item);

    public ICollection<T> GetSnapshot() => (ICollection<T>) this._items;

    public bool DetectChanges()
    {
      int num = this._addedItems.Count > 0 ? 1 : (this._removedItems.Count > 0 ? 1 : 0);
      this._addedItems.Clear();
      this._removedItems.Clear();
      return num != 0;
    }
  }
}
