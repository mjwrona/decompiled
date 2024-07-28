// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.ContainedItem
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class ContainedItem : StoredItem
  {
    public const string InternalPathKey = "cr:itempath";
    private static readonly string DefaultItemType = "contained";

    public ContainedItem()
      : base(ContainedItem.DefaultItemType)
    {
    }

    public ContainedItem(IItemData data)
      : this(data, ContainedItem.DefaultItemType)
    {
    }

    protected ContainedItem(string itemType)
      : base(itemType)
    {
    }

    protected ContainedItem(IItemData data, string itemType)
      : base(data, itemType)
    {
    }

    public Locator InternalPath
    {
      get => Locator.Parse(this.Data["cr:itempath"]);
      set => this.Data["cr:itempath"] = value.ToString();
    }
  }
}
