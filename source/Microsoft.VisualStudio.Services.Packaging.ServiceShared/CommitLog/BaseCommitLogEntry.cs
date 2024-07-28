// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseCommitLogEntry
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog
{
  public class BaseCommitLogEntry : ICommitLogEntry, ICommitLogEntryHeader
  {
    public BaseCommitLogEntry(
      ICommitOperationData operationData,
      PackagingCommitId commitId,
      long sequenceNumber,
      DateTime createdDate,
      DateTime modifiedDate,
      Guid userId,
      bool corruptEntry)
    {
      this.CommitOperationData = operationData;
      this.CommitId = commitId;
      this.SequenceNumber = sequenceNumber;
      this.CreatedDate = createdDate;
      this.ModifiedDate = modifiedDate;
      this.UserId = userId;
      this.CorruptEntry = corruptEntry;
    }

    public long SequenceNumber { get; }

    public PackagingCommitId CommitId { get; }

    public ICommitOperationData CommitOperationData { get; }

    public DateTime CreatedDate { get; }

    public DateTime ModifiedDate { get; }

    public Guid UserId { get; }

    public bool CorruptEntry { get; }
  }
}
