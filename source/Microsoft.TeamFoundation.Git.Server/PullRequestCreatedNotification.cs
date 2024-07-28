// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestCreatedNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class PullRequestCreatedNotification : PullRequestNotification
  {
    internal PullRequestCreatedNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      TeamFoundationIdentity creator,
      string title,
      string description,
      DateTime creationDate,
      string sourceBranchName,
      string targetBranchName,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewers,
      bool notifyPoliciesAsync)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.Creator = creator;
      this.Title = title;
      this.Description = description;
      this.CreationDate = creationDate;
      this.SourceBranchName = sourceBranchName;
      this.TargetBranchName = targetBranchName;
      this.Reviewers = reviewers;
      this.NotifyPoliciesAsync = notifyPoliciesAsync;
    }

    public TeamFoundationIdentity Creator { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public DateTime CreationDate { get; private set; }

    public string SourceBranchName { get; private set; }

    public string TargetBranchName { get; private set; }

    public IEnumerable<TfsGitPullRequest.ReviewerBase> Reviewers { get; private set; }

    public bool NotifyPoliciesAsync { get; private set; }
  }
}
