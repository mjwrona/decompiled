// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitMetricsProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class GitMetricsProvider
  {
    private readonly IVssRequestContext m_requestContext;

    public GitMetricsProvider(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public GitActivityMetrics GetProjectMetrics(
      Guid projectId,
      int startingTimeBucket,
      int endTimeBucket)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return gitCoreComponent.GetGitProjectMetrics(projectId, startingTimeBucket, endTimeBucket);
    }

    public GitActivityMetrics GetRepositoryMetrics(
      Guid projectId,
      Guid repositoryId,
      int startingTimeBucket,
      int endTimeBucket)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(repositoryId, nameof (repositoryId));
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
      {
        RepoKey repoKey = new RepoKey(projectId, repositoryId);
        return gitCoreComponent.GetGitRepositoryMetrics(repoKey, startingTimeBucket, endTimeBucket);
      }
    }

    public IList<GitActivityMetrics> GetRepositoriesMetrics(
      Guid projectId,
      int startingTimeBucket,
      int endTimeBucket,
      int skip,
      int take)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return (IList<GitActivityMetrics>) gitCoreComponent.GetGitRepositoriesMetrics(projectId, startingTimeBucket, endTimeBucket, skip, take);
    }

    public GitActiveRepoInfo GetMostActiveRepo(
      Guid projectId,
      GitMetrics evaluationMetric,
      int timePeriods)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return gitCoreComponent.GetMostActiveRepo(projectId, evaluationMetric, timePeriods);
    }
  }
}
