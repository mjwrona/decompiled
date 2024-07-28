// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.ListItem`1
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class ListItem<TValue> : StoredItem where TValue : StoredItem
  {
    public const string ItemsKey = "cr:items";
    private static readonly string DefaultItemType = string.Format("list_{0}", (object) typeof (TValue).Name);

    public ListItem()
      : this(ListItem<TValue>.DefaultItemType)
    {
    }

    public ListItem(IItemData data)
      : this(data, ListItem<TValue>.DefaultItemType)
    {
    }

    protected ListItem(string itemType)
      : base(itemType)
    {
    }

    protected ListItem(IItemData data, string itemType)
      : base(data, itemType)
    {
    }

    public IEnumerable<TValue> Items
    {
      get => this.Data.GetItems("cr:items").Select<Item, TValue>((Func<Item, TValue>) (i => i.Convert<TValue>()));
      set => this.Data.SetItems("cr:items", (IEnumerable<Item>) value);
    }
  }
}
