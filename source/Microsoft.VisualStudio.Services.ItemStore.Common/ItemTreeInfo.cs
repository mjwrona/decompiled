// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.ItemTreeInfo
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class ItemTreeInfo : StoredItem
  {
    public const string RootKey = "cr:root";
    public const string BlobStoreUriKey = "cr:blobs";
    public const string TicketInfoKey = "cr:ticketInfo";

    public ItemTreeInfo()
    {
    }

    public ItemTreeInfo(IItemData data)
      : base(data)
    {
    }

    public ItemTreeInfo(string id, Locator root)
      : this()
    {
      this.Root = root;
    }

    public Uri BlobStoreUri
    {
      get => this.Data["cr:blobs"] != null ? new Uri(this.Data["cr:blobs"]) : (Uri) null;
      set => this.Data["cr:blobs"] = value?.ToString();
    }

    public Locator Root
    {
      get => Locator.Parse(this.Data["cr:root"]);
      set => this.Data["cr:root"] = value.ToString();
    }

    public string TicketInfo
    {
      get => this.Data["cr:ticketInfo"];
      set => this.Data["cr:ticketInfo"] = value;
    }
  }
}
