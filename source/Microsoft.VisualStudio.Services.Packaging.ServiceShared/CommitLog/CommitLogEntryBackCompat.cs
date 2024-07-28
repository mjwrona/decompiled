// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.CommitLogEntryBackCompat
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog
{
  public class CommitLogEntryBackCompat
  {
    public CommitLogEntryBackCompat(
      PackagingCommitId commitId,
      CommitOperation commitOperation,
      PackagingCommitId previousCommitId,
      PackagingCommitId nextCommitId,
      DateTime createdDate,
      DateTime modifiedDate,
      IStorageId packageStorageId,
      long packageSize)
    {
      this.CommitId = commitId;
      this.PreviousCommitId = previousCommitId;
      this.NextCommitId = nextCommitId;
      this.CommitOperation = commitOperation;
      this.CreatedDate = createdDate;
      this.ModifiedDate = modifiedDate;
      this.PackageStorageId = packageStorageId;
      this.PackageSize = packageSize;
    }

    public DateTime ModifiedDate { get; private set; }

    public IStorageId PackageStorageId { get; private set; }

    public long PackageSize { get; private set; }

    public PackagingCommitId CommitId { get; private set; }

    public DateTime CreatedDate { get; private set; }

    public PackagingCommitId PreviousCommitId { get; private set; }

    public PackagingCommitId NextCommitId { get; private set; }

    public CommitOperation CommitOperation { get; private set; }
  }
}
