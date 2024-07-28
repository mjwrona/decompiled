// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ResetMultipleVotesNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class ResetMultipleVotesNotification : PullRequestNotification
  {
    internal ResetMultipleVotesNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      IEnumerable<TeamFoundationIdentity> reviewers,
      bool AllVotesReset,
      TeamFoundationIdentity initiator = null,
      string reason = null)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.Reviewers = reviewers;
      this.Initiator = initiator;
      this.Reason = reason;
    }

    public IEnumerable<TeamFoundationIdentity> Reviewers { get; }

    public bool AllVotesReset { get; }

    public TeamFoundationIdentity Initiator { get; }

    public string Reason { get; }
  }
}
