// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestRetargetNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class PullRequestRetargetNotification : PullRequestNotification
  {
    internal PullRequestRetargetNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      TeamFoundationIdentity updater,
      string newTargetRef)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.Updater = updater;
      this.NewTargetRef = newTargetRef;
    }

    public TeamFoundationIdentity Updater { get; private set; }

    public string NewTargetRef { get; private set; }
  }
}
