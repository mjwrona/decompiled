// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRepositoryStatsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitRepositoryStatsController : GitApiController
  {
    [HttpGet]
    [ClientExample("GET__git_repositories__repositoryId__stats_branches__name__baseVersionType-_baseVersionType__baseVersion-_baseVersion_.json", null, null, null)]
    [ClientLocationId("616A5255-74B3-40F5-AE1D-BBAE2EEC8DB5")]
    [ClientInclude(RestClientLanguages.TypeScript | RestClientLanguages.Python)]
    public GitRepositoryStats GetStats([ClientParameterType(typeof (Guid), true)] string repositoryId)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId))
      {
        RepoStats stats = new RepoStatsProvider(this.TfsRequestContext).GetStats(tfsGitRepository);
        return stats == null ? (GitRepositoryStats) null : new GitRepositoryStats(repositoryId, stats.CommitsCount, stats.BranchesCount, stats.ActivePullRequestsCount);
      }
    }
  }
}
