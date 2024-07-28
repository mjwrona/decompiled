// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.CommitLogItemBackCompat
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog
{
  public abstract class CommitLogItemBackCompat : StoredItem
  {
    private const string CommitTypeKey = "commitType";
    private const string PackageSizeKey = "packageSize";
    private const string PackageContentKey = "packageContent";
    private const string CreatedDateKey = "createdDate";
    private const string ModifiedDateKey = "modifiedDate";
    private const string PreviousCommitIdKey = "previousCommitId";
    private const string NextCommitIdKey = "nextCommitId";
    private const string CommitIdKey = "commitId";

    public CommitLogItemBackCompat(string commitLogItemType)
      : base(commitLogItemType)
    {
    }

    public CommitLogItemBackCompat(IItemData data, string commitLogItemType)
      : base(data, commitLogItemType)
    {
    }

    public PackagingCommitId PreviousPackagingCommitId
    {
      get
      {
        string str = this.Data["previousCommitId"];
        return str != null ? PackagingCommitId.Parse(str) : PackagingCommitId.Empty;
      }
      set => this.Data["previousCommitId"] = value.ToString();
    }

    public PackagingCommitId PackagingCommitId
    {
      get => PackagingCommitId.Parse(this.Data["commitId"]);
      set => this.Data["commitId"] = value.ToString();
    }

    public PackagingCommitId NextPackagingCommitId
    {
      get
      {
        string str = this.Data["nextCommitId"];
        return str != null ? PackagingCommitId.Parse(str) : PackagingCommitId.Empty;
      }
      set => this.Data["nextCommitId"] = value.ToString();
    }

    public CommitOperation CommitOperation
    {
      get
      {
        CommitOperation result;
        return !Enum.TryParse<CommitOperation>(this.Data["commitType"], out result) ? CommitOperation.Unknown : result;
      }
      set
      {
        ArgumentUtility.CheckForDefinedEnum<CommitOperation>(value, nameof (value));
        this.Data["commitType"] = value != CommitOperation.Unknown ? value.ToString("G") : throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotCreateUnknownTypeCommitLogEntry(), nameof (value));
      }
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
  }
}
