// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildRequestScopeHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class BuildRequestScopeHelper
  {
    private const string TraceLayer = "BuildRequestScopeHelper";
    private static readonly Guid PipelineArtifactsTaskId = Guid.Parse("61F2A582-95AE-4948-B34D-A1B3C4F6A737");
    private static readonly Guid DownloadBuildArtifactsTaskId = Guid.Parse("A433F589-FCE1-4460-9EE6-44A624AEB1FB");

    public static string GetOAuthScope(
      IVssRequestContext requestContext,
      BuildData build,
      PipelineEnvironment environment,
      IOrchestrationProcess orchestration)
    {
      return string.Join(" ", (IEnumerable<string>) new List<string>()
      {
        BuildRequestScopeHelper.GetRestrictiveOAuthScope(requestContext, build, environment),
        BuildRequestScopeHelper.GetDownloadArtifactScope(requestContext, orchestration, build.Definition),
        ScopeHelpers.ConstructScope("Project.GenericRead", build.ProjectId.ToString()),
        ScopeHelpers.ConstructScope("Build2.Definition.ViewDefinitionAndBuilds", build.Definition.GetToken()),
        "Packaging.Administrator",
        BuildRequestScopeHelper.GetSourceCheckoutScope(requestContext, build.ProjectId, environment)
      });
    }

    private static string GetGitScope(IVssRequestContext requestContext, Guid projectId) => string.Join(" ", new string[2]
    {
      ScopeHelpers.ConstructScope("Project.GenericRead", projectId.ToString()),
      ScopeHelpers.ConstructScope("AzureRepos.Git.ReadAndTag", GitUtils.CalculateSecurable(projectId, Guid.Empty, (string) null).TrimEnd('/'))
    });

    internal static string GetSourceCheckoutScope(
      IVssRequestContext requestContext,
      Guid currProjectId,
      PipelineEnvironment environment)
    {
      string sourceCheckoutScope = BuildRequestScopeHelper.GetGitScope(requestContext, currProjectId);
      ISet<RepositoryResource> repositories = environment.Resources?.Repositories;
      if (repositories != null && repositories.Any<RepositoryResource>())
      {
        foreach (RepositoryResource repositoryResource in (IEnumerable<RepositoryResource>) repositories)
        {
          if (string.Equals(repositoryResource.Type, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.Git, StringComparison.OrdinalIgnoreCase))
          {
            string input;
            Guid result;
            if (repositoryResource.Properties.TryGetValue<string>(RepositoryPropertyNames.Project, out input) && Guid.TryParse(input, out result) && !object.Equals((object) result, (object) currProjectId))
              sourceCheckoutScope = string.Join(" ", new string[2]
              {
                sourceCheckoutScope,
                BuildRequestScopeHelper.GetGitScope(requestContext, result)
              });
          }
          else
          {
            string str1;
            if (string.Equals(repositoryResource.Type, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.Tfvc, StringComparison.OrdinalIgnoreCase) && repositoryResource.Properties.TryGetValue<string>(RepositoryPropertyNames.Id, out str1))
            {
              string str2 = ScopeHelpers.ConstructScope("AzureRepos.Tfvc.Read", str1);
              sourceCheckoutScope = sourceCheckoutScope + " " + str2.TrimEnd('/');
            }
          }
        }
      }
      return sourceCheckoutScope;
    }

    private static bool TryGetDefinition(
      IVssRequestContext requestContext,
      Guid stepProjectId,
      string stepDefinition,
      MinimalBuildDefinition currentDefinition,
      out MinimalBuildDefinition buildDefinition)
    {
      int result;
      if (int.TryParse(stepDefinition, out result) && result == currentDefinition.Id)
      {
        buildDefinition = currentDefinition;
      }
      else
      {
        try
        {
          BuildDefinition definition = requestContext.GetService<IBuildDefinitionService>().GetDefinition(requestContext, stepProjectId, stepDefinition);
          if (definition == null)
            requestContext.TraceError(nameof (BuildRequestScopeHelper), "No definition was found in project {0} for definition ID {1}", (object) stepProjectId, (object) stepDefinition);
          buildDefinition = (MinimalBuildDefinition) definition;
        }
        catch (Exception ex)
        {
          buildDefinition = (MinimalBuildDefinition) null;
          requestContext.TraceException(nameof (BuildRequestScopeHelper), ex);
        }
      }
      return buildDefinition != null;
    }

    private static bool TryGetProjectGuid(
      IVssRequestContext requestContext,
      string stepProject,
      out Guid projectId)
    {
      if (Guid.TryParse(stepProject, out projectId))
        return true;
      try
      {
        requestContext.GetService<IProjectService>();
        ProjectInfo projectInfo = requestContext.GetProjectInfo(stepProject);
        projectId = projectInfo.Id;
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (BuildRequestScopeHelper), ex);
      }
      return false;
    }

    internal static string GetDownloadArtifactScope(
      IVssRequestContext requestContext,
      IOrchestrationProcess orchestration,
      MinimalBuildDefinition definition)
    {
      string downloadArtifactScope = string.Empty;
      foreach (TaskStep taskStep in TaskStepHelper.GetTaskSteps(requestContext, orchestration, (Func<Guid, bool>) (taskId => taskId == BuildRequestScopeHelper.PipelineArtifactsTaskId || taskId == BuildRequestScopeHelper.DownloadBuildArtifactsTaskId), nameof (GetDownloadArtifactScope)))
      {
        string valueOrDefault = taskStep.Inputs.GetValueOrDefault<string, string>("buildType", "current");
        MinimalBuildDefinition minimalBuildDefinition;
        if (string.Equals(valueOrDefault, "current"))
        {
          minimalBuildDefinition = definition;
        }
        else
        {
          string stepProject;
          Guid projectId;
          string stepDefinition;
          MinimalBuildDefinition buildDefinition;
          if (string.Equals(valueOrDefault, "specific") && taskStep.Inputs.TryGetValue("project", out stepProject) && BuildRequestScopeHelper.TryGetProjectGuid(requestContext, stepProject, out projectId) && taskStep.Inputs.TryGetValue(nameof (definition), out stepDefinition) && BuildRequestScopeHelper.TryGetDefinition(requestContext, projectId, stepDefinition, definition, out buildDefinition))
          {
            minimalBuildDefinition = buildDefinition;
          }
          else
          {
            requestContext.TraceError(nameof (BuildRequestScopeHelper), "GetDownloadArtifactScope unknown buildType found: {0}", (object) valueOrDefault);
            continue;
          }
        }
        if (minimalBuildDefinition.Id != definition.Id)
        {
          string str1 = ScopeHelpers.ConstructScope("Project.GenericRead", minimalBuildDefinition.ProjectId.ToString());
          string str2 = ScopeHelpers.ConstructScope("Build2.Definition.ViewDefinitionAndBuilds", minimalBuildDefinition.GetToken());
          downloadArtifactScope = string.Join(" ", new string[3]
          {
            downloadArtifactScope,
            str2,
            str1
          });
        }
      }
      return downloadArtifactScope;
    }

    public static string GetRestrictiveOAuthScope(
      IVssRequestContext requestContext,
      BuildData build,
      PipelineEnvironment environment)
    {
      string restrictiveOauthScope = string.Join(" ", new string[4]
      {
        ScopeHelpers.ConstructScope("ReadAndUpdateBuildByUri", build.Definition.GetToken(), build.Uri.AbsolutePath.Substring(1)),
        ScopeHelpers.ConstructScope("ReadAndPublishTest", build.ProjectId.ToString("D")),
        "LocationService.Connect",
        ScopeHelpers.ConstructScope("PipelineCache.ReadWriteRootAccess")
      });
      if (!environment.Options.RestrictSecrets)
      {
        foreach (Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference file in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference>) environment.Resources.Files)
        {
          string[] strArray = new string[2];
          Guid guid = build.ProjectId;
          strArray[0] = guid.ToString("D");
          guid = file.Id;
          strArray[1] = guid.ToString("D");
          string str = ScopeHelpers.ConstructScope("ReadSecureFile", strArray);
          restrictiveOauthScope = restrictiveOauthScope + " " + str;
        }
      }
      return restrictiveOauthScope;
    }
  }
}
