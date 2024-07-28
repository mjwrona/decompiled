// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RefUpdateNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class RefUpdateNotification : GitNotification
  {
    internal RefUpdateNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      IdentityDescriptor pusher,
      string authenticatedUserName,
      TfsGitRefUpdateResult refUpdateResult)
      : base(teamProjectUri, repositoryId, repositoryName)
    {
      this.Pusher = pusher;
      this.AutenticatedUserName = authenticatedUserName;
      this.RefUpdateResult = refUpdateResult;
    }

    public IdentityDescriptor Pusher { get; private set; }

    public string AutenticatedUserName { get; private set; }

    public TfsGitRefUpdateResult RefUpdateResult { get; private set; }
  }
}
