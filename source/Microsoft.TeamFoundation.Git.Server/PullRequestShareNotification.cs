// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestShareNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class PullRequestShareNotification : PullRequestNotification
  {
    public PullRequestShareNotification()
      : base((string) null, Guid.Empty, (string) null, 0)
    {
    }

    internal PullRequestShareNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      Guid sender,
      IEnumerable<IdentityRef> receivers,
      string message)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.Sender = sender;
      this.Receivers = receivers;
      this.Message = message;
    }

    public Guid Sender { get; private set; }

    public IEnumerable<IdentityRef> Receivers { get; private set; }

    public string Message { get; private set; }
  }
}
