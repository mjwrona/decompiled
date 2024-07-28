// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RepositoryRenamedNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class RepositoryRenamedNotification : GitNotification
  {
    internal RepositoryRenamedNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      IdentityDescriptor initiator)
      : base(teamProjectUri, repositoryId, repositoryName)
    {
      this.RenamedBy = initiator;
    }

    public string OldName { get; set; }

    public IdentityDescriptor RenamedBy { get; private set; }
  }
}
