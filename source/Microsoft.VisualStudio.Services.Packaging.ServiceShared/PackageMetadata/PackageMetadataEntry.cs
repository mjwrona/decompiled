// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.PackageMetadataEntry
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class PackageMetadataEntry
  {
    public PackageMetadataEntry(
      PackagingCommitId commitId,
      DateTime createdDate,
      DateTime modifiedDate,
      Guid createdBy,
      Guid modifiedBy,
      IStorageId packageStorageId,
      long packageSize,
      IEnumerable<Guid> views,
      DateTime? scheduledPermanentDeleteDate,
      DateTime? permanentDeletedDate,
      bool isUpstreamCached = false,
      IEnumerable<UpstreamSourceInfo> sourceChain = null)
    {
      this.CommitId = commitId;
      this.CreatedDate = createdDate;
      this.ModifiedDate = modifiedDate;
      this.PackageStorageId = packageStorageId;
      this.PackageSize = packageSize;
      this.CreatedBy = createdBy;
      this.ModifiedBy = modifiedBy;
      this.Views = views;
      this.ScheduledPermanentDeleteDate = scheduledPermanentDeleteDate;
      this.PermanentDeletedDate = permanentDeletedDate;
      this.IsUpstreamCached = isUpstreamCached;
      this.SourceChain = sourceChain;
    }

    public DateTime ModifiedDate { get; private set; }

    public IStorageId PackageStorageId { get; private set; }

    public long PackageSize { get; private set; }

    public PackagingCommitId CommitId { get; private set; }

    public DateTime CreatedDate { get; private set; }

    public Guid CreatedBy { get; private set; }

    public Guid ModifiedBy { get; private set; }

    public IEnumerable<Guid> Views { get; private set; }

    public DateTime? ScheduledPermanentDeleteDate { get; }

    public DateTime? PermanentDeletedDate { get; }

    public bool IsUpstreamCached { get; private set; }

    public IEnumerable<UpstreamSourceInfo> SourceChain { get; private set; }
  }
}
