// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ExternalGitPushYamlTriggerMaterializationJob
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class ExternalGitPushYamlTriggerMaterializationJob : ITeamFoundationJobExtension
  {
    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      resultMessage = string.Empty;
      using (GitPushYamlTriggerTracer tracer = new GitPushYamlTriggerTracer(requestContext, 12030390, "PipelineTriggerMaterialization"))
      {
        using (requestContext.TraceScope(nameof (ExternalGitPushYamlTriggerMaterializationJob), nameof (Run)))
        {
          try
          {
            if (!requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
            {
              resultMessage = "Pipeline trigger is not enabled.";
              return TeamFoundationJobExecutionResult.Succeeded;
            }
            ExternalGitPushYamlHandlerJobData yamlHandlerJobData = ExternalGitPushYamlHandlerJobDataUtilities.DeserializeFromJsonXmlNode(jobDefinition.Data);
            List<Guid> guidList = yamlHandlerJobData.ProjectList;
            if (guidList == null)
              guidList = new List<Guid>()
              {
                yamlHandlerJobData.ProjectInfo.Id
              };
            string repositoryType1 = yamlHandlerJobData.RepoType;
            ExternalGitPush pushNotification = yamlHandlerJobData.GitPushNotification;
            TeamFoundationJobExecutionResult? currentFinalResult = new TeamFoundationJobExecutionResult?();
            foreach (Guid projectGuid in guidList)
            {
              if (string.IsNullOrEmpty(repositoryType1))
              {
                IDictionary<string, string> properties = pushNotification.Properties;
                if ((properties != null ? (!properties.TryGetRepositoryType(requestContext, projectGuid, out repositoryType1, out bool _) ? 1 : 0) : 1) != 0)
                {
                  tracer.TraceAlways(string.Format("Cannot find repo type for repository {0} and project {1}", (object) yamlHandlerJobData.GitPushNotification?.Repo?.Id, (object) projectGuid), new int?(12030256));
                  resultMessage = "Cannot find repo type for repository " + yamlHandlerJobData.GitPushNotification?.Repo?.Id;
                }
                else
                  break;
              }
              else
                break;
            }
            if (string.IsNullOrEmpty(repositoryType1))
              return TeamFoundationJobExecutionResult.PartiallySucceeded;
            RepositoryUpdateInfo repositoryUpdateInfo = pushNotification.GetRepositoryUpdateInfo(requestContext, repositoryType1);
            IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
            foreach (Guid guid in guidList)
            {
              Guid project = guid;
              IBuildDefinitionService definitionService = service;
              IVssRequestContext requestContext1 = requestContext;
              Guid projectId = project;
              string repositoryType2 = repositoryType1;
              string id = pushNotification.Repo.Id;
              int? nullable = new int?(2);
              DateTime? minLastModifiedTime = new DateTime?();
              DateTime? maxLastModifiedTime = new DateTime?();
              DateTime? minMetricsTime = new DateTime?();
              DateTime? builtAfter = new DateTime?();
              DateTime? notBuiltAfter = new DateTime?();
              Guid? taskIdFilter = new Guid?();
              int? processType = nullable;
              List<BuildDefinition> definitions = definitionService.GetDefinitionsForRepository(requestContext1, projectId, repositoryType2, id, minLastModifiedTime: minLastModifiedTime, maxLastModifiedTime: maxLastModifiedTime, minMetricsTime: minMetricsTime, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, taskIdFilter: taskIdFilter, processType: processType).Where<BuildDefinition>((Func<BuildDefinition, bool>) (x =>
              {
                DefinitionQuality? definitionQuality1 = x.DefinitionQuality;
                DefinitionQuality definitionQuality2 = DefinitionQuality.Definition;
                return definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue;
              })).Where<BuildDefinition>((Func<BuildDefinition, bool>) (x => x.QueueStatus != DefinitionQueueStatus.Disabled)).ToList<BuildDefinition>();
              IEnumerable<bool> list = (IEnumerable<bool>) repositoryUpdateInfo.RefUpdates.Select<RefUpdateInfo, bool>((Func<RefUpdateInfo, bool>) (x => this.ProcessRefUpdate(requestContext, tracer, x, (IEnumerable<BuildDefinition>) definitions, repositoryUpdateInfo, project))).ToList<bool>();
              if (list.All<bool>((Func<bool, bool>) (x => !x)))
                currentFinalResult = new TeamFoundationJobExecutionResult?(this.CalculateFinalResult(currentFinalResult, TeamFoundationJobExecutionResult.Failed));
              else if (list.Any<bool>((Func<bool, bool>) (x => !x)))
                currentFinalResult = new TeamFoundationJobExecutionResult?(this.CalculateFinalResult(currentFinalResult, TeamFoundationJobExecutionResult.PartiallySucceeded));
              currentFinalResult = new TeamFoundationJobExecutionResult?(this.CalculateFinalResult(currentFinalResult, TeamFoundationJobExecutionResult.Succeeded));
            }
            return currentFinalResult.GetValueOrDefault();
          }
          catch (Exception ex)
          {
            tracer.TraceException(ex, 12030258);
            resultMessage = ex.Message;
            return TeamFoundationJobExecutionResult.Failed;
          }
        }
      }
    }

    protected TeamFoundationJobExecutionResult CalculateFinalResult(
      TeamFoundationJobExecutionResult? currentFinalResult,
      TeamFoundationJobExecutionResult newResult)
    {
      if (currentFinalResult.HasValue)
      {
        TeamFoundationJobExecutionResult? nullable = currentFinalResult;
        TeamFoundationJobExecutionResult jobExecutionResult1 = newResult;
        if (!(nullable.GetValueOrDefault() == jobExecutionResult1 & nullable.HasValue))
        {
          nullable = currentFinalResult;
          TeamFoundationJobExecutionResult jobExecutionResult2 = TeamFoundationJobExecutionResult.PartiallySucceeded;
          if (!(nullable.GetValueOrDefault() == jobExecutionResult2 & nullable.HasValue))
          {
            switch (newResult)
            {
              case TeamFoundationJobExecutionResult.Succeeded:
                nullable = currentFinalResult;
                TeamFoundationJobExecutionResult jobExecutionResult3 = TeamFoundationJobExecutionResult.Succeeded;
                return !(nullable.GetValueOrDefault() == jobExecutionResult3 & nullable.HasValue) ? TeamFoundationJobExecutionResult.PartiallySucceeded : TeamFoundationJobExecutionResult.Succeeded;
              case TeamFoundationJobExecutionResult.PartiallySucceeded:
                break;
              case TeamFoundationJobExecutionResult.Failed:
                nullable = currentFinalResult;
                TeamFoundationJobExecutionResult jobExecutionResult4 = TeamFoundationJobExecutionResult.Succeeded;
                return !(nullable.GetValueOrDefault() == jobExecutionResult4 & nullable.HasValue) ? TeamFoundationJobExecutionResult.Failed : TeamFoundationJobExecutionResult.PartiallySucceeded;
              default:
                return TeamFoundationJobExecutionResult.PartiallySucceeded;
            }
          }
          return TeamFoundationJobExecutionResult.PartiallySucceeded;
        }
      }
      return newResult;
    }

    private bool ProcessRefUpdate(
      IVssRequestContext requestContext,
      GitPushYamlTriggerTracer tracer,
      RefUpdateInfo refUpdate,
      IEnumerable<BuildDefinition> definitions,
      RepositoryUpdateInfo repositoryUpdateInfo,
      Guid projectId)
    {
      try
      {
        tracer.TraceInfo(string.Format("Project:{0} NewRef{1} OldRef{2} Branch:{3}", (object) projectId, (object) refUpdate.NewObjectId, (object) refUpdate.OldObjectId, (object) refUpdate.RefName));
        if (string.IsNullOrEmpty(refUpdate.RefName))
        {
          tracer.TraceInfo("refUpdate branch is null");
          return false;
        }
        IEnumerable<BuildDefinition> source = definitions.Where<BuildDefinition>((Func<BuildDefinition, bool>) (d => string.Equals(BuildSourceProviders.GitProperties.BranchToRefName(d.Repository.DefaultBranch), BuildSourceProviders.GitProperties.BranchToRefName(refUpdate.RefName), StringComparison.Ordinal)));
        if (!source.Any<BuildDefinition>())
        {
          tracer.TraceInfo("RefUpdate do not match any default branch for definitions");
          return true;
        }
        List<string> filesChanged = new List<string>();
        bool flag = false;
        foreach (BuildDefinition buildDefinition in source)
        {
          if (!filesChanged.Any<string>())
          {
            try
            {
              if (!FilteredBuildTriggerHelper.TryGetFilesChanged(requestContext, buildDefinition, repositoryUpdateInfo, refUpdate, (ExternalGitPullRequest) null, out filesChanged))
              {
                tracer.TraceInfo("Failed to retrieve changes for refUpdate on branch '" + refUpdate.RefName + "' from external Git Push event.");
                flag = true;
                continue;
              }
            }
            catch (Exception ex)
            {
              tracer.TraceException(ex, 12030258);
              flag = true;
              continue;
            }
          }
          IList<string> filePathFromCommit = ExternalGitPushYamlTriggerMaterializationJob.TryGetModifiedYamlFilePathFromCommit(requestContext, tracer, (IList<string>) filesChanged);
          if (!filePathFromCommit.Any<string>())
            tracer.TraceInfo("No Yaml file has changed.No trigger needs to materialized for this external Git Push event");
          else if (ExternalGitPushYamlTriggerMaterializationJob.DefinitionContainsYamlFilePath(buildDefinition, filePathFromCommit))
          {
            tracer.TraceInfo(string.Format("Persisting Pipeline Trigger changes for project {0} on commit {1} for definition {2} for external Git Push event", (object) projectId, (object) refUpdate.NewObjectId, (object) buildDefinition.Name));
            flag = !this.PersistYamlPipelineTriggerChanges(requestContext, tracer, refUpdate, refUpdate.NewObjectId, buildDefinition) | flag;
            tracer.TraceInfo("Persisted trigger info successfully");
          }
          else
            tracer.TraceInfo(string.Format("Definition {0}({1}) yaml file was not changed", (object) buildDefinition.Name, (object) buildDefinition.Id));
        }
        return !flag;
      }
      catch (Exception ex)
      {
        tracer.TraceException(ex, 12030258);
        return false;
      }
    }

    private static IList<string> TryGetModifiedYamlFilePathFromCommit(
      IVssRequestContext requestContext,
      GitPushYamlTriggerTracer tracer,
      IList<string> changedFiles)
    {
      tracer.TraceInfo("Filtering the changed files for yaml files");
      List<string> filePathFromCommit = new List<string>();
      foreach (string changedFile in (IEnumerable<string>) changedFiles)
      {
        string extension = Path.GetExtension(changedFile);
        if (!string.IsNullOrEmpty(extension) && (extension.Equals(".yml", StringComparison.OrdinalIgnoreCase) || extension.Equals(".yaml", StringComparison.OrdinalIgnoreCase)))
          filePathFromCommit.Add(changedFile);
      }
      tracer.TraceInfo("Completed filtering the changed files");
      return (IList<string>) filePathFromCommit;
    }

    private bool PersistYamlPipelineTriggerChanges(
      IVssRequestContext requestContext,
      GitPushYamlTriggerTracer tracer,
      RefUpdateInfo refUpdate,
      string sourceVersion,
      BuildDefinition definition)
    {
      IArtifactYamlTriggerService service = requestContext.GetService<IArtifactYamlTriggerService>();
      YamlProcess process = definition.GetProcess<YamlProcess>();
      try
      {
        PipelineResources authorizedResources = requestContext.GetService<IBuildResourceAuthorizationService>().GetAuthorizedResources(requestContext, definition.ProjectId, definition.Id).ToPipelineResources() ?? new PipelineResources();
        RepositoryResource repositoryResource = definition.Repository.ToRepositoryResource(requestContext, PipelineConstants.SelfAlias, refUpdate.RefName, sourceVersion);
        if (repositoryResource.Endpoint != null && !process.SupportsYamlRepositoryEndpointAuthorization())
          authorizedResources.Endpoints.Add(repositoryResource.Endpoint);
        PipelineBuilder pipelineBuilder = definition.GetPipelineBuilder(requestContext, authorizedResources, true);
        service.UpdateTriggers(requestContext, definition.ProjectId, definition.Id, repositoryResource, process.YamlFilename, pipelineBuilder, sourceVersion, definition.Repository.Url);
      }
      catch (Exception ex)
      {
        tracer.TraceException(ex, 12030258);
        tracer.TraceInfo("Failed to update triggers from Yaml file '" + process.YamlFilename + "', project '" + definition.ProjectName + "', repo '" + definition.Repository.Name + "', ref '" + refUpdate.RefName + "', commit '" + sourceVersion + "', error: " + ex.Message);
        return false;
      }
      return true;
    }

    private static bool DefinitionContainsYamlFilePath(
      BuildDefinition definition,
      IList<string> yamlFiles)
    {
      bool flag = false;
      string normalizedYamlPath;
      return BuildSourceProviders.GitProperties.TryResolvePath((string) null, definition.GetProcess<YamlProcess>().YamlFilename, out normalizedYamlPath) ? yamlFiles.Any<string>((Func<string, bool>) (name => name.Equals(normalizedYamlPath, StringComparison.CurrentCultureIgnoreCase))) : flag;
    }
  }
}
