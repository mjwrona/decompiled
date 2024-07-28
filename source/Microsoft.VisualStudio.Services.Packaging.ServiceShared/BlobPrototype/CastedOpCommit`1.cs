// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CastedOpCommit`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;
using System.Runtime.CompilerServices;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public record CastedOpCommit<TOperationData>(

    #nullable disable
    ICommitLogEntry Commit,
    TOperationData CommitOperationData) : ICommitLogEntry, ICommitLogEntryHeader
    where TOperationData : class, ICommitOperationData
  {
    public long SequenceNumber => this.Commit.SequenceNumber;

    public PackagingCommitId CommitId => this.Commit.CommitId;

    public DateTime CreatedDate => this.Commit.CreatedDate;

    public DateTime ModifiedDate => this.Commit.ModifiedDate;

    public Guid UserId => this.Commit.UserId;

    public bool CorruptEntry => this.Commit.CorruptEntry;

    ICommitOperationData ICommitLogEntry.CommitOperationData => this.Commit.CommitOperationData;

    [CompilerGenerated]
    protected virtual bool PrintMembers(
    #nullable enable
    StringBuilder builder)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      builder.Append("Commit = ");
      builder.Append((object) this.Commit);
      builder.Append(", CommitOperationData = ");
      builder.Append((object) this.CommitOperationData);
      builder.Append(", SequenceNumber = ");
      builder.Append(this.SequenceNumber.ToString());
      builder.Append(", CommitId = ");
      builder.Append(this.CommitId.ToString());
      builder.Append(", CreatedDate = ");
      builder.Append(this.CreatedDate.ToString());
      builder.Append(", ModifiedDate = ");
      builder.Append(this.ModifiedDate.ToString());
      builder.Append(", UserId = ");
      builder.Append(this.UserId.ToString());
      builder.Append(", CorruptEntry = ");
      builder.Append(this.CorruptEntry.ToString());
      return true;
    }
  }
}
