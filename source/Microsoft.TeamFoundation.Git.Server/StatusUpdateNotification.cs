// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.StatusUpdateNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class StatusUpdateNotification : PullRequestNotification
  {
    internal StatusUpdateNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      TeamFoundationIdentity updater,
      PullRequestStatus status,
      Sha1Id? associatedCommit = null)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.Updater = updater;
      this.Status = status;
      this.AssociatedCommit = associatedCommit;
      this.SourceBranchState = PullRequestCompletionSourceBranchState.NotDeleted;
    }

    public TeamFoundationIdentity Updater { get; private set; }

    public PullRequestStatus Status { get; private set; }

    public Sha1Id? AssociatedCommit { get; private set; }

    public PullRequestCompletionSourceBranchState SourceBranchState { get; set; }
  }
}
