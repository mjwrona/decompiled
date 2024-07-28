// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public abstract class GitNotification
  {
    protected GitNotification(string teamProjectUri, Guid repositoryId, string repositoryName)
    {
      this.TeamProjectUri = teamProjectUri;
      this.RepositoryId = repositoryId;
      this.RepositoryName = repositoryName;
      this.GitNotificationGuid = Guid.NewGuid().ToString("D");
      this.Timestamp = DateTime.UtcNow;
    }

    public string RepositoryName { get; private set; }

    public Guid RepositoryId { get; private set; }

    public string TeamProjectUri { get; private set; }

    public string GitNotificationGuid { get; private set; }

    public DateTime Timestamp { get; }
  }
}
