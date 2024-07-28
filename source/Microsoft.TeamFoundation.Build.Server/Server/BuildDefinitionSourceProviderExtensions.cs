// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDefinitionSourceProviderExtensions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class BuildDefinitionSourceProviderExtensions
  {
    public static void ConvertToProjectGuid(
      this List<BuildDefinitionSourceProvider> sourceProviders,
      IVssRequestContext requestContext)
    {
      foreach (IProjectGuidConverter projectGuidConverter in BuildDefinitionSourceProviderExtensions.GetProjectGuidConverters(requestContext, sourceProviders))
        projectGuidConverter.ConvertToProjectGuid(requestContext);
    }

    public static void ConvertToProjectName(
      this List<BuildDefinitionSourceProvider> sourceProviders,
      IVssRequestContext requestContext)
    {
      foreach (IProjectNameConverter projectNameConverter in BuildDefinitionSourceProviderExtensions.GetProjectNameConverters(requestContext, sourceProviders))
        projectNameConverter.ConvertToProjectName(requestContext);
    }

    internal static IEnumerable<IProjectGuidConverter> GetProjectGuidConverters(
      IVssRequestContext requestContext,
      List<BuildDefinitionSourceProvider> sourceProviders)
    {
      return (IEnumerable<IProjectGuidConverter>) BuildDefinitionSourceProviderExtensions.GetTfGitConverters(requestContext, sourceProviders.Where<BuildDefinitionSourceProvider>((Func<BuildDefinitionSourceProvider, bool>) (sp => BuildSourceProviders.IsTfGit(sp.Name))));
    }

    internal static IEnumerable<IProjectNameConverter> GetProjectNameConverters(
      IVssRequestContext requestContext,
      List<BuildDefinitionSourceProvider> sourceProviders)
    {
      return (IEnumerable<IProjectNameConverter>) BuildDefinitionSourceProviderExtensions.GetTfGitConverters(requestContext, sourceProviders.Where<BuildDefinitionSourceProvider>((Func<BuildDefinitionSourceProvider, bool>) (sp => BuildSourceProviders.IsTfGit(sp.Name))));
    }

    internal static IEnumerable<TfGitConverter> GetTfGitConverters(
      IVssRequestContext requestContext,
      IEnumerable<BuildDefinitionSourceProvider> sourceProviders)
    {
      List<Tuple<string, Guid?, TfGitConverter>> source = new List<Tuple<string, Guid?, TfGitConverter>>();
      IProjectService service1 = requestContext.GetService<IProjectService>();
      HashSet<Guid> repoIds = new HashSet<Guid>();
      foreach (BuildDefinitionSourceProvider sourceProvider in sourceProviders)
      {
        string teamProject;
        string name;
        ProjectInfo project;
        if (sourceProvider.TryGetRepoInfo(out teamProject, out name) && BuildDefinitionSourceProviderExtensions.TryGetProject(requestContext.Elevate(), service1, teamProject, out project))
        {
          TfGitConverter tfGitConverter = new TfGitConverter()
          {
            ProjectInfo = project,
            SourceProvider = sourceProvider
          };
          Guid result;
          if (Guid.TryParse(name, out result))
          {
            repoIds.Add(result);
            source.Add(new Tuple<string, Guid?, TfGitConverter>(name, new Guid?(result), tfGitConverter));
          }
          else
            source.Add(new Tuple<string, Guid?, TfGitConverter>(name, new Guid?(), tfGitConverter));
        }
      }
      ITeamFoundationGitRepositoryService service2 = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      if (repoIds.Count > 0)
      {
        Dictionary<Guid, TfsGitRepositoryInfo> dictionary = service2.QueryRepositoriesAcrossProjects(requestContext.Elevate(), (IEnumerable<Guid>) repoIds).ToDictionary<TfsGitRepositoryInfo, Guid>((Func<TfsGitRepositoryInfo, Guid>) (repo => repo.Key.RepoId));
        foreach (Tuple<string, Guid?, TfGitConverter> tuple in source.Where<Tuple<string, Guid?, TfGitConverter>>((Func<Tuple<string, Guid?, TfGitConverter>, bool>) (tuple => tuple.Item2.HasValue)))
        {
          TfsGitRepositoryInfo repositoryInfo;
          if (dictionary.TryGetValue(tuple.Item2.Value, out repositoryInfo))
          {
            TfGitConverter tfGitConverter = tuple.Item3;
            if (repositoryInfo.Key.GetProjectUri().Equals(tfGitConverter.ProjectInfo.Uri, StringComparison.OrdinalIgnoreCase))
            {
              tfGitConverter.RepoKey = repositoryInfo.Key;
              tfGitConverter.RepoName = repositoryInfo.Name;
              try
              {
                tfGitConverter.RepoUri = repositoryInfo.GetRepositoryWebUri(requestContext.Elevate());
              }
              catch (ProjectDoesNotExistException ex)
              {
                requestContext.TraceConditionally(800038, TraceLevel.Info, "Build", "Service", (Func<string>) (() => ex.ToString()));
              }
            }
          }
        }
      }
      foreach (Tuple<string, Guid?, TfGitConverter> tuple in source)
      {
        string repositoryName = tuple.Item1;
        TfGitConverter tfGitConverter = tuple.Item3;
        if ((RepoScope) tfGitConverter.RepoKey == (RepoScope) null)
        {
          try
          {
            using (ITfsGitRepository repositoryByNameAndUri = service2.FindRepositoryByNameAndUri(requestContext.Elevate(), tfGitConverter.ProjectInfo.Uri, repositoryName))
            {
              tfGitConverter.RepoKey = repositoryByNameAndUri.Key;
              tfGitConverter.RepoName = repositoryByNameAndUri.Name;
              tfGitConverter.RepoUri = repositoryByNameAndUri.GetRepositoryWebUri();
            }
          }
          catch (ProjectDoesNotExistException ex)
          {
            requestContext.TraceConditionally(800038, TraceLevel.Info, "Build", "Service", (Func<string>) (() => ex.ToString()));
          }
          catch (GitRepositoryNotFoundException ex)
          {
            requestContext.TraceConditionally(800036, TraceLevel.Info, "Build", "Service", (Func<string>) (() => ex.ToString()));
          }
        }
      }
      return source.Where<Tuple<string, Guid?, TfGitConverter>>((Func<Tuple<string, Guid?, TfGitConverter>, bool>) (c => (RepoScope) c.Item3.RepoKey != (RepoScope) null)).Select<Tuple<string, Guid?, TfGitConverter>, TfGitConverter>((Func<Tuple<string, Guid?, TfGitConverter>, TfGitConverter>) (c => c.Item3));
    }

    private static bool TryGetProject(
      IVssRequestContext requestContext,
      IProjectService projectService,
      string projectDescriptor,
      out ProjectInfo project)
    {
      project = (ProjectInfo) null;
      Guid result;
      return Guid.TryParse(projectDescriptor, out result) && projectService.TryGetProject(requestContext, result, out project) || projectService.TryGetProject(requestContext, projectDescriptor, out project);
    }
  }
}
