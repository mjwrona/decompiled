// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.AssociationsStatus
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class AssociationsStatus : StoredItem
  {
    public const string ItemTreeInfoKey = "cr:itemTree";
    public const string MissingKey = "cr:missing";
    public const string AccessTokenKey = "cr:accessToken";
    private static readonly string DefaultItemType = "associationsStatus";

    public AssociationsStatus()
      : base(AssociationsStatus.DefaultItemType)
    {
    }

    public AssociationsStatus(IItemData data)
      : base(data)
    {
    }

    public AssociationsStatus(ItemTreeInfo itemTreeInfo, IEnumerable<BlobIdentifier> missing)
      : this()
    {
      ArgumentUtility.CheckForNull<ItemTreeInfo>(itemTreeInfo, nameof (itemTreeInfo));
      ArgumentUtility.CheckForNull<IEnumerable<BlobIdentifier>>(missing, nameof (missing));
      this.ItemTreeInfo = itemTreeInfo;
      this.ItemTreeInfo.TicketInfo = (string) null;
      this.Missing = missing;
    }

    public ItemTreeInfo ItemTreeInfo
    {
      get => this.Data.GetItem("cr:itemTree")?.Convert<ItemTreeInfo>();
      set => this.Data.SetItem("cr:itemTree", (Item) value);
    }

    public IEnumerable<BlobIdentifier> Missing
    {
      get => this.Data.GetStrings("cr:missing").Select<string, BlobIdentifier>((Func<string, BlobIdentifier>) (entry => BlobIdentifier.Deserialize(entry)));
      set => this.Data.SetStrings("cr:missing", value.Select<BlobIdentifier, string>((Func<BlobIdentifier, string>) (entry => entry.ValueString)));
    }

    [Obsolete("Never set by server-side code")]
    public string AccessToken
    {
      get => this.Data["cr:accessToken"];
      set => this.Data["cr:accessToken"] = value;
    }
  }
}
