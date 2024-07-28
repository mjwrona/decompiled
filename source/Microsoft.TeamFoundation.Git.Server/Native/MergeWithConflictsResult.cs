// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Native.MergeWithConflictsResult
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server.Native
{
  internal class MergeWithConflictsResult
  {
    public PullRequestAsyncStatus Status { get; set; }

    public PullRequestMergeFailureType FailureType { get; set; }

    public Sha1Id MergeBaseCommitId { get; set; }

    public Sha1Id MergeCommitId { get; set; }

    public Index Index { get; set; }

    public Exception ObservedException { get; set; }

    public string FailureMessage => this.ObservedException?.Message;
  }
}
