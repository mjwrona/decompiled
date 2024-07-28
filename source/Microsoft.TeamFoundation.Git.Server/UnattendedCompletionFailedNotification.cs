// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.UnattendedCompletionFailedNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class UnattendedCompletionFailedNotification : PullRequestNotification
  {
    internal UnattendedCompletionFailedNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      TeamFoundationIdentity completionAuthority,
      int mergeCount,
      UnattendedCompletionFailedNotification.FailureReason reason,
      Exception exception)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.CompletionAuthority = completionAuthority;
      this.MergeCount = mergeCount;
      this.Reason = reason;
      this.Exception = exception;
    }

    public UnattendedCompletionFailedNotification.FailureReason Reason { get; private set; }

    public TeamFoundationIdentity CompletionAuthority { get; private set; }

    public int MergeCount { get; private set; }

    public Exception Exception { get; private set; }

    public enum FailureReason
    {
      OtherError,
      MergeConflict,
      RejectedByPolicy,
      RetryLimitReached,
      RepositoryNotFound,
    }
  }
}
