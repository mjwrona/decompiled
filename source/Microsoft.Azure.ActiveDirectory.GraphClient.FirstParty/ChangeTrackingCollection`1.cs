// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ChangeTrackingCollection`1
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class ChangeTrackingCollection<T> : Collection<T>
  {
    internal event EventHandler CollectionChanged;

    public bool Any() => this.Items.Any<T>();

    public bool Any(T value) => this.Items.Contains(value);

    protected override void InsertItem(int index, T item)
    {
      Utils.ThrowIfNull((object) item, nameof (item));
      base.InsertItem(index, item);
      this.OnItemInserted(item);
    }

    protected override void RemoveItem(int index)
    {
      T removedItem = this[index];
      base.RemoveItem(index);
      this.OnItemRemoved(removedItem);
    }

    protected override void SetItem(int index, T item)
    {
      Utils.ThrowIfNull((object) item, nameof (item));
      T replacedItem = this[index];
      base.SetItem(index, item);
      this.OnItemSet(replacedItem, item);
    }

    protected override void ClearItems()
    {
      List<T> clearedItems = new List<T>((IEnumerable<T>) this);
      base.ClearItems();
      this.OnItemsCleared((IEnumerable<T>) clearedItems);
    }

    protected virtual void OnItemInserted(T insertedItem) => this.OnCollectionChanged();

    protected virtual void OnItemRemoved(T removedItem) => this.OnCollectionChanged();

    protected virtual void OnItemSet(T replacedItem, T newItem) => this.OnCollectionChanged();

    protected virtual void OnItemsCleared(IEnumerable<T> clearedItems) => this.OnCollectionChanged();

    protected virtual void OnCollectionChanged()
    {
      if (this.CollectionChanged == null)
        return;
      this.CollectionChanged((object) this, EventArgs.Empty);
    }
  }
}
