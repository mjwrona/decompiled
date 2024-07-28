// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.BlobItem
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class BlobItem : ContainedItem
  {
    private const string BlobLocationKey = "cr:blobLocation";
    private const string BlobIdentifierKey = "cr:blobIdentifier";
    private const string DeleteStartedUtcKey = "cr:deleteStartedUtc";
    private const string DomainIdKey = "cr:domainId";
    private const string DefaultItemType = "blob";

    public BlobItem()
      : base("blob")
    {
    }

    public BlobItem(IItemData data)
      : this(data, "blob")
    {
    }

    protected BlobItem(string itemType)
      : base(itemType)
    {
    }

    protected BlobItem(IItemData data, string itemType)
      : base(data, itemType)
    {
    }

    public PreauthenticatedUri? BlobLocation
    {
      get => PreauthenticatedUri.FromPossiblyNullString(this.Data["cr:blobLocation"], EdgeType.Unknown);
      set => this.Data["cr:blobLocation"] = value.HasValue ? value.Value.NotNullUri.AbsoluteUri : (string) null;
    }

    public BlobIdentifier BlobIdentifier
    {
      get => BlobIdentifier.Deserialize(this.Data["cr:blobIdentifier"]);
      set => this.Data["cr:blobIdentifier"] = value.ValueString;
    }

    public IDomainId DomainId
    {
      get => DomainIdFactory.Create(this.Data["cr:domainId"]);
      set => this.Data["cr:domainId"] = value?.Serialize();
    }

    private DateTime? DeleteStartedUtc => this.Data.TryGetWithNullCheck<DateTime?>("cr:deleteStartedUtc", new DateTime?(), (Func<string, DateTime?>) (str => new DateTime?(DateTime.Parse(str).ToUniversalTime())));

    public static bool HasBlobId(Item possibleBlobItem)
    {
      ArgumentUtility.CheckForNull<Item>(possibleBlobItem, nameof (possibleBlobItem));
      return possibleBlobItem.Data["cr:blobIdentifier"] != null;
    }

    public static bool HasDomainId(Item possibleBlobItem)
    {
      ArgumentUtility.CheckForNull<Item>(possibleBlobItem, nameof (possibleBlobItem));
      return possibleBlobItem.Data["cr:domainId"] != null;
    }

    public static bool HasVsoHashBlobId(Item possibleBlobItem) => BlobItem.HasBlobId(possibleBlobItem) && BlobIdentifier.Deserialize(possibleBlobItem.Data["cr:blobIdentifier"]).AlgorithmId == (byte) 0;

    public static bool TryGetBlobId(Item possibleBlobItem, out BlobIdentifier blobid)
    {
      ArgumentUtility.CheckForNull<Item>(possibleBlobItem, nameof (possibleBlobItem));
      if (possibleBlobItem.Data["cr:blobIdentifier"] == null)
      {
        blobid = (BlobIdentifier) null;
        return false;
      }
      blobid = BlobIdentifier.Deserialize(possibleBlobItem.Data["cr:blobIdentifier"]);
      return possibleBlobItem.Data["cr:blobIdentifier"] != null;
    }

    public static bool IsDeleteInProgress(Item possibleBlobItem)
    {
      ArgumentUtility.CheckForNull<Item>(possibleBlobItem, nameof (possibleBlobItem));
      return possibleBlobItem.Data["cr:deleteStartedUtc"] != null;
    }

    public static bool TrySetDeleteInProgress(StoredItem possibleBlobItem)
    {
      ArgumentUtility.CheckForNull<StoredItem>(possibleBlobItem, nameof (possibleBlobItem));
      if (!BlobItem.HasBlobId((Item) possibleBlobItem) || BlobItem.IsDeleteInProgress((Item) possibleBlobItem))
        return false;
      possibleBlobItem.Data["cr:deleteStartedUtc"] = DateTime.UtcNow.ToString("o");
      return true;
    }

    protected override bool Verify(Lazy<StringBuilder> errorBuilder)
    {
      bool flag = base.Verify(errorBuilder);
      if (!BlobItem.HasBlobId((Item) this))
      {
        flag = false;
        errorBuilder.Value.AppendLine("cr:blobIdentifier is not set.");
      }
      return flag;
    }
  }
}
