// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Forks.GitRepositoryRefBuilder
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Core.CollectionManagement.WebServer;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Forks
{
  internal class GitRepositoryRefBuilder
  {
    private readonly IVssRequestContext m_rc;
    private readonly IVssRequestContext m_orgRc;
    private readonly ITeamProjectCollectionPropertiesService m_collectionSvc;
    private readonly IProjectService m_projectSvc;
    private readonly IReadOnlyDictionary<Guid, TfsGitRepositoryInfo> m_callerVisibleRepoInfo;
    private readonly ILocationService m_locationSvc;
    private readonly string m_baseUrl;

    public GitRepositoryRefBuilder(
      IVssRequestContext requestContext,
      IReadOnlyDictionary<Guid, TfsGitRepositoryInfo> callerVisibleRepoInfo,
      ITeamProjectCollectionPropertiesService collectionSvc,
      IProjectService projSvc,
      ILocationService locationSvc)
    {
      this.m_rc = requestContext;
      this.m_callerVisibleRepoInfo = callerVisibleRepoInfo;
      this.m_orgRc = requestContext.To(TeamFoundationHostType.Application);
      this.m_collectionSvc = collectionSvc;
      this.m_projectSvc = projSvc;
      this.m_locationSvc = locationSvc;
      this.m_baseUrl = GitServerUtils.GetPublicBaseUrl(this.m_rc, true);
    }

    public bool TryGetFromMinimalGlobalKey(
      MinimalGlobalRepoKey repoKey,
      out GitRepositoryRef result)
    {
      TfsGitRepositoryInfo gitRepositoryInfo;
      if (this.m_callerVisibleRepoInfo.TryGetValue(repoKey.RepositoryId, out gitRepositoryInfo))
      {
        Guid collectionId1 = repoKey.CollectionId;
        Guid collectionId2 = !(collectionId1 == Guid.Empty) ? collectionId1 : this.m_rc.ServiceHost.InstanceId;
        ProjectInfo projectInfo;
        if (this.m_projectSvc.TryGetProject(this.m_rc, gitRepositoryInfo.Key.ProjectId, out projectInfo))
        {
          TeamProjectReference projectReference = projectInfo.ToTeamProjectReference(this.m_rc);
          string repositoryCloneUrl = GitServerUtils.GetRepositoryCloneUrl(this.m_rc, this.m_baseUrl, projectReference.Name, gitRepositoryInfo.Name);
          result = new GitRepositoryRef()
          {
            Collection = this.m_collectionSvc.GetCollectionProperties(this.m_orgRc, collectionId2, ServiceHostFilterFlags.None).ToTeamProjectCollectionReference(this.m_orgRc),
            ProjectReference = projectReference,
            Id = repoKey.RepositoryId,
            Name = gitRepositoryInfo.Name,
            IsFork = gitRepositoryInfo.IsFork,
            Url = this.m_locationSvc.GetResourceUri(this.m_rc, "git", GitWebApiConstants.RepositoriesLocationId, (object) new
            {
              repositoryId = repoKey.RepositoryId
            }).AbsoluteUri,
            RemoteUrl = repositoryCloneUrl,
            SshUrl = GitServerUtils.GetSshUrl(this.m_rc, repositoryCloneUrl, out bool _)
          };
          return true;
        }
      }
      result = (GitRepositoryRef) null;
      return false;
    }
  }
}
