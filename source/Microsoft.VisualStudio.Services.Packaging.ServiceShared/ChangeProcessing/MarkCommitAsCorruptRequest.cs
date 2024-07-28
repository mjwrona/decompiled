// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.MarkCommitAsCorruptRequest
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public class MarkCommitAsCorruptRequest
  {
    public FeedCore Feed { get; }

    public PackagingCommitId CommitId { get; }

    public bool IsForceMode { get; }

    public Exception Exception { get; }

    public MarkCommitAsCorruptRequest(
      FeedCore feed,
      PackagingCommitId commitId,
      bool isForceMode,
      Exception exception)
    {
      this.Feed = feed;
      this.CommitId = commitId;
      this.IsForceMode = isForceMode;
      this.Exception = exception;
    }
  }
}
