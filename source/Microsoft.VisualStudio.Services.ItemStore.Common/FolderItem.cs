// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.FolderItem
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class FolderItem : ContainedItem
  {
    public const string FolderItemChildEtagKey = "cr:childEtag";
    public const string ChildrenKey = "cr:children";
    private static readonly string DefaultItemType = "folder";

    public FolderItem()
      : this(FolderItem.DefaultItemType)
    {
    }

    public FolderItem(IItemData data)
      : this(data, FolderItem.DefaultItemType)
    {
    }

    protected FolderItem(string itemType)
      : base(itemType)
    {
    }

    protected FolderItem(IItemData data, string itemType)
      : base(data, itemType)
    {
    }

    public IEnumerable<Item> Children
    {
      get => this.Data.GetItems("cr:children");
      set => this.Data.SetItems("cr:children", value);
    }

    public virtual bool HasChildren => this.Children != null && this.Children.Count<Item>() > 0;
  }
}
