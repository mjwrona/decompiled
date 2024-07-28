// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RepoStatsProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class RepoStatsProvider
  {
    private readonly IVssRequestContext m_requestContext;

    public RepoStatsProvider(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public RepoStats GetStats(ITfsGitRepository repository)
    {
      if (repository == null)
        throw new ArgumentNullException(nameof (repository));
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return gitCoreComponent.GetRepositoryStats(repository.Key);
    }
  }
}
