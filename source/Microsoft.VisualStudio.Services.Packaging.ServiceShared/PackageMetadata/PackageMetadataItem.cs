// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.PackageMetadataItem
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public abstract class PackageMetadataItem : StoredItem
  {
    private const string PackageSizeKey = "packageSize";
    private const string PackageContentKey = "packageContent";
    private const string CreatedDateKey = "createdDate";
    private const string ModifiedDateKey = "modifiedDate";
    private const string CreatedByKey = "createdBy";
    private const string ModifiedByKey = "modifiedBy";
    private const string CommitIdKey = "commitId";
    private const string PackageViewsKey = "packageViews";
    public const string ScheduledPermanentDeleteDateKey = "scheduledPermanentDeleteDate";
    public const string PermanentDeletedDateKey = "permanentDeletedDate";
    private const string IsUpstreamCachedKey = "packageIsCachedFromUpstream";
    private const string SourceChainKey = "sourceChainKey";

    public PackageMetadataItem(string commitLogItemType)
      : base(commitLogItemType)
    {
    }

    public PackageMetadataItem(IItemData data, string commitLogItemType)
      : base(data, commitLogItemType)
    {
    }

    public PackagingCommitId PackagingCommitId
    {
      get => PackagingCommitId.Parse(this.Data["commitId"]);
      set => this.Data["commitId"] = value.ToString();
    }

    public DateTime CreatedDate
    {
      get => DateTime.FromBinary(long.Parse(this.Data["createdDate"]));
      set
      {
        CommitLogUtils.CheckDateIsUtc(value, nameof (value));
        this.Data["createdDate"] = value.ToBinary().ToString();
      }
    }

    public DateTime ModifiedDate
    {
      get => DateTime.FromBinary(long.Parse(this.Data["modifiedDate"]));
      set
      {
        CommitLogUtils.CheckDateIsUtc(value, nameof (value));
        this.Data["modifiedDate"] = value.ToBinary().ToString();
      }
    }

    public Guid CreatedBy
    {
      get
      {
        string input = this.Data["createdBy"];
        return input == null ? Guid.Empty : Guid.Parse(input);
      }
      set => this.Data["createdBy"] = value.ToString();
    }

    public Guid ModifiedBy
    {
      get
      {
        string input = this.Data["modifiedBy"];
        return input == null ? Guid.Empty : Guid.Parse(input);
      }
      set => this.Data["modifiedBy"] = value.ToString();
    }

    public IStorageId PackageStorageId
    {
      get => StorageId.Parse(this.Data["packageContent"]);
      set => this.Data["packageContent"] = value.ValueString;
    }

    public long PackageSize
    {
      get => long.Parse(this.Data["packageSize"]);
      set => this.Data["packageSize"] = value.ToString();
    }

    public IEnumerable<Guid> Views
    {
      get
      {
        string str = this.Data["packageViews"];
        IEnumerable<Guid> guids;
        if (str == null)
          guids = (IEnumerable<Guid>) null;
        else
          guids = ((IEnumerable<string>) str.Split(',')).Select<string, Guid>(PackageMetadataItem.\u003C\u003EO.\u003C0\u003E__Parse ?? (PackageMetadataItem.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, Guid>(Guid.Parse)));
        return guids ?? Enumerable.Empty<Guid>();
      }
      set => this.Data["packageViews"] = string.Join<Guid>(",", value);
    }

    public DateTime? ScheduledPermanentDeleteDate
    {
      get
      {
        string s = this.Data["scheduledPermanentDeleteDate"];
        return !string.IsNullOrWhiteSpace(s) ? new DateTime?(DateTime.FromBinary(long.Parse(s))) : new DateTime?();
      }
      set
      {
        if (value.HasValue)
        {
          CommitLogUtils.CheckDateIsUtc(value.Value, nameof (value));
          this.Data["scheduledPermanentDeleteDate"] = value.Value.ToBinary().ToString();
        }
        else
          this.Data["scheduledPermanentDeleteDate"] = (string) null;
      }
    }

    public DateTime? PermanentDeletedDate
    {
      get
      {
        string s = this.Data["permanentDeletedDate"];
        return !string.IsNullOrWhiteSpace(s) ? new DateTime?(DateTime.FromBinary(long.Parse(s))) : new DateTime?();
      }
      set
      {
        if (value.HasValue)
        {
          CommitLogUtils.CheckDateIsUtc(value.Value, nameof (value));
          this.Data["permanentDeletedDate"] = value.Value.ToBinary().ToString();
        }
        else
          this.Data["permanentDeletedDate"] = (string) null;
      }
    }

    public bool IsUpstreamCached
    {
      get => System.Convert.ToBoolean(this.Data["packageIsCachedFromUpstream"]);
      set => this.Data["packageIsCachedFromUpstream"] = value.ToString();
    }

    public virtual VersionDetails GetProtocolVersionDetails() => new VersionDetails()
    {
      Views = this.Views
    };

    public IEnumerable<UpstreamSourceInfo> SourceChain
    {
      get
      {
        string json = this.Data["sourceChainKey"];
        return json == null ? (IEnumerable<UpstreamSourceInfo>) null : JsonUtilities.Deserialize<IEnumerable<UpstreamSourceInfo>>(json);
      }
      set
      {
        if (value == null)
          return;
        this.Data["sourceChainKey"] = value.Serialize<IEnumerable<UpstreamSourceInfo>>();
      }
    }
  }
}
