// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.AssociationsItem
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class AssociationsItem : DictionaryItem<StoredItem>
  {
    public const string DedupUploadStatisticsKey = "cr:dedupUploadStats";
    public const string ItemTreeInfoKey = "cr:itemTree";
    public const string OverwriteKey = "cr:overwrite";
    public const string AbortIfAlreadyExistsKey = "cr:abortIfAlreadyExists";
    public const string BlobIdsWithBlocksKey = "cr:blobIdsWithBlocks";
    public const string ProofNodesKey = "cr:proofNodes";
    private static readonly string DefaultItemType = "associations";

    public AssociationsItem(bool abortIfAlreadyExists)
      : this()
    {
      this.AbortIfAlreadyExists = abortIfAlreadyExists;
    }

    public AssociationsItem()
      : base(AssociationsItem.DefaultItemType)
    {
    }

    public AssociationsItem(IItemData data)
      : base(data, AssociationsItem.DefaultItemType)
    {
    }

    [CLSCompliant(false)]
    public DedupUploadStatistics DedupUploadStats
    {
      get
      {
        IEnumerable<string> strings = this.Data.GetStrings("cr:dedupUploadStats");
        return strings != null ? DedupUploadStatistics.DeserializeForItemStore(strings) : (DedupUploadStatistics) null;
      }
      set => this.Data.SetStrings("cr:dedupUploadStats", (IEnumerable<string>) value.SerializeForItemStore());
    }

    public int ItemCount => (this.Data.GetItems("cr:items") ?? Enumerable.Empty<Item>()).Count<Item>();

    public ItemTreeInfo ItemTreeInfo
    {
      get => this.Data.GetItem("cr:itemTree")?.Convert<ItemTreeInfo>();
      set => this.Data.SetItem("cr:itemTree", (Item) value);
    }

    public bool AbortIfAlreadyExists
    {
      get => this.Data["cr:abortIfAlreadyExists"] == null ? !bool.Parse(this.Data["cr:overwrite"]) : bool.Parse(this.Data["cr:abortIfAlreadyExists"]);
      private set => this.Data["cr:abortIfAlreadyExists"] = value.ToString();
    }

    public IEnumerable<BlobIdentifierWithBlocks> BlobsUploaded
    {
      get
      {
        IEnumerable<string> strings = this.Data.GetStrings("cr:blobIdsWithBlocks");
        return strings != null ? strings.Select<string, BlobIdentifierWithBlocks>((Func<string, BlobIdentifierWithBlocks>) (b => BlobIdentifierWithBlocks.Deserialize(b))) : Enumerable.Empty<BlobIdentifierWithBlocks>();
      }
      set => this.Data.SetStrings("cr:blobIdsWithBlocks", value.Select<BlobIdentifierWithBlocks, string>((Func<BlobIdentifierWithBlocks, string>) (b => b.Serialize())));
    }

    public int BlobsUploadedCount
    {
      get
      {
        IEnumerable<string> strings = this.Data.GetStrings("cr:blobIdsWithBlocks");
        return strings == null ? 0 : strings.Count<string>();
      }
    }

    protected override bool Verify(Lazy<StringBuilder> errorBuilder)
    {
      bool flag = base.Verify(errorBuilder);
      if (this.Data["cr:abortIfAlreadyExists"] == null && this.Data["cr:overwrite"] == null)
      {
        flag = false;
        errorBuilder.Value.AppendLine("Neither overwrite or AbortIfAlreadyExists is not set.");
      }
      if (this.Data["cr:abortIfAlreadyExists"] != null && this.Data["cr:overwrite"] != null)
      {
        flag = false;
        errorBuilder.Value.AppendLine("Both overwrite or AbortIfAlreadyExists are set.");
      }
      return flag;
    }

    public IEnumerable<byte[]> ProofNodes
    {
      get
      {
        IEnumerable<string> strings = this.Data.GetStrings("cr:proofNodes");
        return strings == null ? (IEnumerable<byte[]>) null : strings.Select<string, byte[]>((Func<string, byte[]>) (entry => System.Convert.FromBase64String(entry)));
      }
      set => this.Data.SetMaybeStrings("cr:proofNodes", value != null ? value.Select<byte[], string>((Func<byte[], string>) (entry => System.Convert.ToBase64String(entry))) : (IEnumerable<string>) null);
    }

    public int ProofNodesCount
    {
      get
      {
        IEnumerable<string> strings = this.Data.GetStrings("cr:proofNodes");
        return strings == null ? 0 : strings.Count<string>();
      }
    }
  }
}
