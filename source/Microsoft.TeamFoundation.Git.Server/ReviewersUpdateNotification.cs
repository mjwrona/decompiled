// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ReviewersUpdateNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class ReviewersUpdateNotification : PullRequestNotification
  {
    internal ReviewersUpdateNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      TeamFoundationIdentity updater,
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> currentReviewers,
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> changedReviewers,
      bool createDiscussionMessage,
      bool sendEmailNotification,
      bool notifyPolicies = true)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.Updater = updater;
      this.CurrentReviewers = currentReviewers;
      this.ChangedReviewers = changedReviewers;
      this.CreateDiscussionMessage = createDiscussionMessage;
      this.SendEmailNotification = sendEmailNotification;
      this.NotifyPolicies = notifyPolicies;
    }

    public TeamFoundationIdentity Updater { get; private set; }

    public IEnumerable<TfsGitPullRequest.ReviewerWithVote> CurrentReviewers { get; private set; }

    public IEnumerable<TfsGitPullRequest.ReviewerWithVote> ChangedReviewers { get; private set; }

    public bool CreateDiscussionMessage { get; private set; }

    public bool SendEmailNotification { get; private set; }

    public bool NotifyPolicies { get; private set; }
  }
}
