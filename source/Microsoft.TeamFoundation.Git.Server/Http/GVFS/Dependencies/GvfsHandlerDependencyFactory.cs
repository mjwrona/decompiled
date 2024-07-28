// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Http.GVFS.Dependencies.GvfsHandlerDependencyFactory
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;

namespace Microsoft.TeamFoundation.Git.Server.Http.GVFS.Dependencies
{
  internal class GvfsHandlerDependencyFactory : IGvfsHandlerDependencyFactory
  {
    public static readonly IGvfsHandlerDependencyFactory Instance = (IGvfsHandlerDependencyFactory) new GvfsHandlerDependencyFactory();

    public IGitPackWriter GetPackWriter(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      GitPackWriterThrottler throttler,
      ClientTraceData ctData = null)
    {
      return (IGitPackWriter) new GitPackWriter((ITfsGitContentDB<TfsGitObjectLocation>) GitServerUtils.GetContentDB(repo), throttler, ctData);
    }

    public IGvfsRefLogProvider GetRefLogProvider(ITfsGitRepository repo) => (IGvfsRefLogProvider) new GvfsRefLogProvider(repo);

    public IGvfsPrefetchHavesWantsProvider GetPrefetchHavesWantsProvider(
      IVssRequestContext rc,
      ITfsGitRepository repo)
    {
      return rc.IsFeatureEnabled("Git.GVFS.UseSqlPrefetchHavesWants") ? (IGvfsPrefetchHavesWantsProvider) new GvfsSqlPrefetchHavesWantsProvider(rc, repo) : (IGvfsPrefetchHavesWantsProvider) new GvfsRefLogPrefetchHavesWantsProvider(this.GetRefLogProvider(repo));
    }

    public ITfsGitRepository GetRepository(IVssRequestContext rc, RepoNameKey nameKey)
    {
      ITeamFoundationGitRepositoryService service = rc.GetService<ITeamFoundationGitRepositoryService>();
      ClientTraceData clientTraceData = new ClientTraceData();
      IVssRequestContext requestContext = rc;
      string projectName = nameKey.ProjectName;
      string repositoryName = nameKey.RepositoryName;
      return service.FindRepositoryByName(requestContext, projectName, repositoryName);
    }
  }
}
