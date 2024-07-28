// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Notifications.PullRequest.PullRequestBrokenStateNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core;
using System;

namespace Microsoft.TeamFoundation.Git.Server.Notifications.PullRequest
{
  public class PullRequestBrokenStateNotification : PullRequestNotification
  {
    internal PullRequestBrokenStateNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      TeamFoundationIdentity updater,
      string reason)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.Updater = updater;
      this.Reason = reason;
    }

    public TeamFoundationIdentity Updater { get; }

    public string Reason { get; private set; }
  }
}
