// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.DictionaryItem`1
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class DictionaryItem<TValue> : StoredItem where TValue : StoredItem
  {
    public const string ItemsKey = "cr:items";
    private static readonly string DefaultItemType = string.Format("dictionary_{0}", (object) typeof (TValue).Name);

    public DictionaryItem()
      : this(DictionaryItem<TValue>.DefaultItemType)
    {
    }

    public DictionaryItem(IItemData data)
      : this(data, DictionaryItem<TValue>.DefaultItemType)
    {
    }

    protected DictionaryItem(string itemType)
      : base(itemType)
    {
    }

    protected DictionaryItem(IItemData data, string itemType)
      : base(data, itemType)
    {
    }

    public IEnumerable<KeyValuePair<Locator, TValue>> Items
    {
      get => (this.Data.GetItems("cr:items") ?? Enumerable.Empty<Item>()).Select<Item, KeyValuePair<Locator, TValue>>((Func<Item, KeyValuePair<Locator, TValue>>) (i => i.Convert<DictionaryItem<TValue>.BatchEntryItem>().ToKeyValue()));
      set => this.SetItems("cr:items", (IEnumerable<Item>) value.Select<KeyValuePair<Locator, TValue>, DictionaryItem<TValue>.BatchEntryItem>((Func<KeyValuePair<Locator, TValue>, DictionaryItem<TValue>.BatchEntryItem>) (itemByPath => new DictionaryItem<TValue>.BatchEntryItem(itemByPath.Key, itemByPath.Value))));
    }

    private class BatchEntryItem : StoredItem
    {
      private const string ItemKeyKey = "cr:itemKey";
      private const string ItemValueKey = "cr:itemValue";
      private static readonly string DefaultItemType = string.Format("dictionary_entry_{0}", (object) typeof (TValue).Name);

      public BatchEntryItem()
        : base(DictionaryItem<TValue>.BatchEntryItem.DefaultItemType)
      {
      }

      public BatchEntryItem(IItemData data)
        : base(data, DictionaryItem<TValue>.BatchEntryItem.DefaultItemType)
      {
      }

      public BatchEntryItem(Locator path, TValue item)
        : this()
      {
        this.Path = path;
        this.Value = item;
      }

      public Locator Path
      {
        get => Locator.Parse(this.Data["cr:itemKey"]);
        set => this.Data["cr:itemKey"] = value.Value;
      }

      public TValue Value
      {
        get => this.Data["cr:itemValue"] != null ? Item.FromJson<TValue>(this.Data["cr:itemValue"]) : default (TValue);
        set => this.Data["cr:itemValue"] = (object) value == null ? (string) null : value.ToJson().ToString();
      }

      public KeyValuePair<Locator, TValue> ToKeyValue() => new KeyValuePair<Locator, TValue>(this.Path, this.Value);
    }
  }
}
