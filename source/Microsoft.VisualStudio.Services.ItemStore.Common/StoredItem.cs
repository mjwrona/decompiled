// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.StoredItem
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class StoredItem : Item
  {
    public const string ItemTypeKey = "cr:itemType";
    protected const string TypeBase = "http://vs.ms.com/item/itemtype/";
    private static readonly string DefaultItemType = "item";

    public StoredItem()
      : this(StoredItem.DefaultItemType)
    {
    }

    public StoredItem(IItemData data)
      : this(data, StoredItem.DefaultItemType)
    {
    }

    protected StoredItem(string itemType)
      : this((IItemData) new JObjectItemData(), itemType)
    {
    }

    protected StoredItem(IItemData data, string itemType)
      : base(data)
    {
      this.ItemType = this.ItemType ?? itemType;
    }

    public string ItemTypeId => "http://vs.ms.com/item/itemtype/" + this.ItemType;

    public string ItemType
    {
      get => this.Data["cr:itemType"];
      set => this.Data["cr:itemType"] = value;
    }

    public string StorageETag { get; set; }

    protected override bool Verify(Lazy<StringBuilder> errorBuilder)
    {
      bool flag = base.Verify(errorBuilder);
      if (this.Data["cr:itemType"] == null)
      {
        flag = false;
        errorBuilder.Value.AppendLine("ItemType is not set.");
      }
      return flag;
    }
  }
}
