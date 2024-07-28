// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiV1Helper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class WikiV1Helper
  {
    public static Microsoft.TeamFoundation.Wiki.WebApi.Wiki CreateWiki(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      string repoName,
      WikiJobHandler wikiJobHandler)
    {
      IVssRequestContext requestContext1 = requestContext;
      WikiCreateParametersV2 parameters = new WikiCreateParametersV2();
      parameters.Name = repoName;
      parameters.ProjectId = teamProjectId;
      parameters.Type = WikiType.ProjectWiki;
      WikiJobHandler wikiJobHandler1 = wikiJobHandler;
      WikiV2 wiki1 = WikiV2Helper.CreateWiki(requestContext1, parameters, wikiJobHandler1);
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, teamProjectId);
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, wiki1.RepositoryId))
      {
        TfsGitRepositoryInfo repo = new TfsGitRepositoryInfo(repositoryById.Name, new RepoKey(repositoryById.Key.ProjectId, repositoryById.Key.RepoId));
        Microsoft.TeamFoundation.Wiki.WebApi.Wiki wiki2 = new Microsoft.TeamFoundation.Wiki.WebApi.Wiki();
        wiki2.Id = repo.Key.RepoId;
        wiki2.Name = repo.Name;
        wiki2.ProjectId = teamProjectId;
        wiki2.HeadCommit = (string) null;
        wiki2.Repository = repo.ToWebApiItem(requestContext, project);
        return wiki2;
      }
    }

    public static IEnumerable<Microsoft.TeamFoundation.Wiki.WebApi.Wiki> GetWikis(
      IVssRequestContext requestContext,
      Guid teamProjectId)
    {
      List<WikiV2> list = WikiV2Helper.GetWikis(requestContext, teamProjectId).Where<WikiV2>((Func<WikiV2, bool>) (wiki => wiki.Type == WikiType.ProjectWiki)).ToList<WikiV2>();
      IDictionary<Guid, TfsGitRepositoryInfo> wikiRepos = (IDictionary<Guid, TfsGitRepositoryInfo>) requestContext.GetService<ITeamFoundationGitRepositoryService>().QueryRepositoriesAcrossProjects(requestContext, list.Select<WikiV2, Guid>((Func<WikiV2, Guid>) (wiki => wiki.RepositoryId)).Distinct<Guid>()).ToDictionary<TfsGitRepositoryInfo, Guid, TfsGitRepositoryInfo>((Func<TfsGitRepositoryInfo, Guid>) (r => r.Key.RepoId), (Func<TfsGitRepositoryInfo, TfsGitRepositoryInfo>) (r => r));
      IDictionary<Guid, ProjectInfo> projectMap;
      if (teamProjectId != Guid.Empty)
      {
        ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, teamProjectId);
        projectMap = (IDictionary<Guid, ProjectInfo>) new Dictionary<Guid, ProjectInfo>()
        {
          {
            project.Id,
            project
          }
        };
      }
      else
        projectMap = (IDictionary<Guid, ProjectInfo>) requestContext.GetService<IProjectService>().GetProjects(requestContext).ToDictionary<ProjectInfo, Guid, ProjectInfo>((Func<ProjectInfo, Guid>) (p => p.Id), (Func<ProjectInfo, ProjectInfo>) (p => p));
      foreach (WikiV2 wikiV2 in list)
      {
        TfsGitRepositoryInfo repo;
        ProjectInfo projectInfo;
        if (wikiRepos.TryGetValue(wikiV2.RepositoryId, out repo) && projectMap.TryGetValue(wikiV2.ProjectId, out projectInfo))
        {
          Microsoft.TeamFoundation.Wiki.WebApi.Wiki wiki = new Microsoft.TeamFoundation.Wiki.WebApi.Wiki();
          wiki.Id = wikiV2.Id;
          wiki.Name = wikiV2.Name;
          wiki.ProjectId = wikiV2.ProjectId;
          wiki.HeadCommit = (string) null;
          wiki.Repository = repo.ToWebApiItem(requestContext, projectInfo);
          yield return wiki;
        }
      }
    }
  }
}
