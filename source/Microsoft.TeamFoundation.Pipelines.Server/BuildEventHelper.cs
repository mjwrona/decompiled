// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.BuildEventHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.ServiceHooks.Sdk.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal static class BuildEventHelper
  {
    private const string c_layer = "EventsService";

    public static void HandleEventsForAllDefinitions(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      JObject authentication,
      List<IExternalGitEvent> externalEvents)
    {
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      foreach (IExternalGitEvent externalEvent in externalEvents)
      {
        try
        {
          switch (externalEvent)
          {
            case ExternalGitPullRequest externalGitPullRequest:
              if (!BuildEventHelper.TryQueueRetryBuild(requestContext, (IExternalGitEvent) externalGitPullRequest, externalGitPullRequest.ProjectId, externalGitPullRequest.BuildToRetry))
              {
                BuildEventHelper.HandleExternalGitPullRequest(requestContext, provider, externalGitPullRequest, authentication, instanceId);
                continue;
              }
              continue;
            case ExternalGitPush externalGitPush:
              if (!BuildEventHelper.TryQueueRetryBuild(requestContext, (IExternalGitEvent) externalGitPush, externalGitPush.ProjectId, externalGitPush.BuildToRetry))
              {
                BuildEventHelper.HandleEventForAllProjects(requestContext, provider, externalGitPush, instanceId);
                continue;
              }
              continue;
            default:
              requestContext.TraceError(TracePoints.Events.HandleEvent, "EventsService", "Unexpected external Git event " + externalEvent.GetType().FullName);
              continue;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(TracePoints.Events.HandleEvent, "EventsService", ex);
          PipelineEventLogger.LogException(requestContext, externalEvent, ex);
          if (externalEvent is ExternalGitPullRequest externalGitPullRequest)
          {
            string[] strArray = externalGitPullRequest.Repo.RepoNameWithOwner().Split('/');
            provider.PullRequestProvider.PostComment(requestContext, authentication, strArray[0], strArray[1], externalGitPullRequest.Number, CommentResponseBuilder.Build("There was an error handling pipeline event " + externalEvent.PipelineEventId + "."));
          }
        }
      }
    }

    private static void HandleEventForAllProjects(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      ExternalGitPush push,
      Guid accountId)
    {
      using (requestContext.TraceScope("EventsService", nameof (HandleEventForAllProjects)))
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IProjectService service = vssRequestContext.GetService<IProjectService>();
        IBuildDefinitionService definitionService = vssRequestContext.GetService<IBuildDefinitionService>();
        if (string.IsNullOrEmpty(push.Repo.Id))
        {
          requestContext.TraceInfo(TracePoints.Provider.HandleEvent, "EventsService", "Failed to handle external '" + provider.ProviderId + "' push event. RepoId is null or empty.");
        }
        else
        {
          bool foundDefinitions = false;
          IEnumerable<ProjectInfo> projects = service.GetProjects(vssRequestContext, ProjectState.WellFormed);
          requestContext.TraceInfo(TracePoints.Provider.HandleEvent, "EventsService", string.Format("Checking '{0}' projects for external '{1}' push event.", (object) 0, (object) provider.ProviderId), (object) projects.Count<ProjectInfo>());
          List<Guid> projectList = new List<Guid>();
          foreach (ProjectInfo projectInfo1 in projects)
          {
            ProjectInfo projectInfo = projectInfo1;
            try
            {
              requestContext.RunAsUser(accountId, projectInfo.Id, provider.ConnectionCreator.IdentityRole, (Action<IVssRequestContext>) (collectionContext =>
              {
                IVssRequestContext requestContext1 = collectionContext.Elevate();
                RepositoryUpdateInfo repositoryUpdateInfo = push.GetRepositoryUpdateInfo(collectionContext, provider.ConnectionCreator.RepositoryType);
                IBuildDefinitionService definitionService1 = definitionService;
                IVssRequestContext requestContext2 = collectionContext;
                List<Guid> projectIds1 = new List<Guid>();
                projectIds1.Add(projectInfo.Id);
                string repositoryId = repositoryUpdateInfo.RepositoryId.ToString();
                string repositoryType1 = repositoryUpdateInfo.RepositoryType;
                List<BuildDefinition> list = definitionService1.GetYamlDefinitionsForRepository(requestContext2, projectIds1, repositoryId, repositoryType1).ToList<BuildDefinition>();
                if (list.Count > 0)
                {
                  collectionContext.TraceInfo(TracePoints.Provider.HandleEvent, "EventsService", string.Format("BuildEventHelper: Checking for YAML Resource repositories updates for {0} definition(s) across {1} refUpdate(s).", (object) list.Count, (object) repositoryUpdateInfo.RefUpdates.Count));
                  FilteredBuildTriggerHelper.UpdateResourceRepositories(collectionContext, list, repositoryUpdateInfo);
                  list.RemoveAll((Predicate<BuildDefinition>) (definition => definition.HasDesignerSchedules()));
                  if (list.Count > 0)
                  {
                    collectionContext.TraceInfo(12030223, "EventsService", string.Format("BuildEventHelper: Checking for YAML Schedule updates for {0} definition(s) across {1} refUpdate(s).", (object) list.Count, (object) repositoryUpdateInfo.RefUpdates.Count));
                    using (requestContext.TraceScope("EventsService", nameof (HandleEventForAllProjects)))
                      CronScheduleHelper.UpdateCronSchedules(collectionContext, list, repositoryUpdateInfo);
                  }
                }
                List<BuildDefinition> buildDefinitionList;
                if (string.IsNullOrEmpty(push.DefinitionToBuild))
                {
                  IBuildDefinitionService definitionService2 = definitionService;
                  IVssRequestContext requestContext3 = requestContext1;
                  List<Guid> projectIds2 = new List<Guid>();
                  projectIds2.Add(projectInfo.Id);
                  string repositoryType2 = provider.ConnectionCreator.RepositoryType;
                  string id = push.Repo.Id;
                  buildDefinitionList = definitionService2.GetDefinitionsWithTriggers(requestContext3, projectIds2, repositoryType2, id, DefinitionTriggerType.ContinuousIntegration).Where<BuildDefinition>((Func<BuildDefinition, bool>) (d =>
                  {
                    if (d.QueueStatus != DefinitionQueueStatus.Disabled)
                    {
                      DefinitionQuality? definitionQuality3 = d.DefinitionQuality;
                      DefinitionQuality definitionQuality4 = DefinitionQuality.Definition;
                      if (definitionQuality3.GetValueOrDefault() == definitionQuality4 & definitionQuality3.HasValue && provider.ConnectionCreator.IsProviderDefinition(collectionContext, d))
                        return !d.IsTooNewForTriggers(requestContext);
                    }
                    return false;
                  })).ToList<BuildDefinition>();
                }
                else
                {
                  buildDefinitionList = new List<BuildDefinition>();
                  int result;
                  if (int.TryParse(push.DefinitionToBuild, out result))
                  {
                    BuildDefinition definition = definitionService.GetDefinition(requestContext1, projectInfo.Id, result);
                    if (definition != null && definition.QueueStatus != DefinitionQueueStatus.Disabled)
                    {
                      DefinitionQuality? definitionQuality5 = definition.DefinitionQuality;
                      DefinitionQuality definitionQuality6 = DefinitionQuality.Definition;
                      if (definitionQuality5.GetValueOrDefault() == definitionQuality6 & definitionQuality5.HasValue && provider.ConnectionCreator.IsProviderDefinition(collectionContext, definition))
                        buildDefinitionList.Add(definition);
                    }
                  }
                  else
                    requestContext.TraceInfo(TracePoints.Provider.HandleEvent, "EventsService", "Failed to parse definition Id for '" + provider.ProviderId + "' push event.");
                }
                if (buildDefinitionList.Any<BuildDefinition>())
                {
                  BuildEventHelper.HandleExternalGitPush(collectionContext, projectInfo.Id, buildDefinitionList, provider, push);
                  foundDefinitions = true;
                }
                else
                  requestContext.TraceInfo(TracePoints.Provider.HandleEvent, "EventsService", "Failed to handle external '" + provider.ProviderId + "' push event. No matching definitions found for event.");
                if (requestContext1.IsFeatureEnabled("Pipelines.HandleAllProjectsInExternalGitPushYamlTriggerMaterializationJob"))
                  projectList.Add(projectInfo.Id);
                else
                  BuildEventHelper.QueueGitPushYamlTriggerMaterializationJob(requestContext1, projectInfo, push, provider.ConnectionCreator.RepositoryType);
              }));
            }
            catch (IdentityNotFoundException ex)
            {
              requestContext.TraceInfo(TracePoints.Provider.ServiceIdentityNotFound, "EventsService", string.Format("No service identity was found when getting CI definitions for team project={0}, repo type={1}, repo id={2}. Processing will continue. Details: {3}", (object) projectInfo.Id, (object) provider.ConnectionCreator.RepositoryType, (object) push.Repo.Id, (object) ex.ToString()));
              PipelineEventLogger.IgnoreEvent(requestContext, (IExternalGitEvent) push, "GitHub App identity not found");
            }
            catch (Exception ex)
            {
              requestContext.TraceError(TracePoints.Provider.HandleEvent, "EventsService", string.Format("Failed to get CI definitions for team project={0}, repo type={1}, repo id={2}. Processing will continue. Error: {3}", (object) projectInfo.Id, (object) provider.ConnectionCreator.RepositoryType, (object) push.Repo.Id, (object) ex.ToString()));
              PipelineEventLogger.LogException(requestContext, (IExternalGitEvent) push, ex);
            }
          }
          if (requestContext.IsFeatureEnabled("Pipelines.HandleAllProjectsInExternalGitPushYamlTriggerMaterializationJob") && projectList.Any<Guid>())
            BuildEventHelper.QueueGitPushYamlTriggerMaterializationJob(vssRequestContext, projectList, push, provider.ConnectionCreator.RepositoryType);
          if (foundDefinitions)
            return;
          PipelineEventLogger.Log(requestContext, PipelineEventType.NoMatchingPipelines, (IExternalGitEvent) push);
        }
      }
    }

    private static void QueueGitPushYamlTriggerMaterializationJob(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      ExternalGitPush push,
      string repoType)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
        return;
      ExternalGitPushYamlHandlerJobData gitPushYamlJobData = new ExternalGitPushYamlHandlerJobData()
      {
        ProjectInfo = projectInfo,
        GitPushNotification = push,
        RepoType = repoType
      };
      BuildEventHelper.QueueGitPushYamlTriggerMaterializationJob(requestContext, gitPushYamlJobData, push, repoType);
    }

    private static void QueueGitPushYamlTriggerMaterializationJob(
      IVssRequestContext requestContext,
      List<Guid> projectList,
      ExternalGitPush push,
      string repoType)
    {
      ExternalGitPushYamlHandlerJobData gitPushYamlJobData = new ExternalGitPushYamlHandlerJobData()
      {
        ProjectList = projectList,
        GitPushNotification = push,
        RepoType = repoType
      };
      BuildEventHelper.QueueGitPushYamlTriggerMaterializationJob(requestContext, gitPushYamlJobData, push, repoType);
    }

    private static void QueueGitPushYamlTriggerMaterializationJob(
      IVssRequestContext requestContext,
      ExternalGitPushYamlHandlerJobData gitPushYamlJobData,
      ExternalGitPush push,
      string repoType)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
        return;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      XmlNode jsonXmlNode = gitPushYamlJobData.SerializeToJsonXmlNode();
      IVssRequestContext requestContext1 = requestContext;
      string jobName = typeof (ExternalGitPushYamlTriggerMaterializationJob).Namespace;
      string fullName = typeof (ExternalGitPushYamlTriggerMaterializationJob).FullName;
      XmlNode jobData = jsonXmlNode;
      string messageFormat = string.Format("Queued Job with Id {0} for external notification with AfterSha {1} in repository {2}, Id {3}", (object) service.QueueOneTimeJob(requestContext1, jobName, fullName, jobData, true), (object) push.AfterSha, (object) push.Repo.Name, (object) push.Repo.Id);
      if (gitPushYamlJobData.ProjectInfo != null)
        messageFormat += string.Format(" and project {0}", (object) gitPushYamlJobData.ProjectInfo.Id);
      requestContext.TraceInfo(12030263, "PipelineTriggerMaterialization", messageFormat);
    }

    private static bool TryQueueRetryBuild(
      IVssRequestContext requestContext,
      IExternalGitEvent gitEvent,
      string projectId,
      string buildToRetry)
    {
      int buildId;
      if (!string.IsNullOrEmpty(projectId) && !string.IsNullOrEmpty(buildToRetry) && int.TryParse(buildToRetry, out buildId))
      {
        Guid projectIdGuid;
        if (Guid.TryParse(projectId, out projectIdGuid))
        {
          try
          {
            IVssRequestContext elevatedContext = requestContext.Elevate();
            IBuildService buildService = elevatedContext.GetService<IBuildService>();
            BuildData buildData = elevatedContext.RunSynchronously<BuildData>((Func<Task<BuildData>>) (() => buildService.RetryBuildAsync(elevatedContext, projectIdGuid, buildId)));
            requestContext.TraceInfo(TracePoints.Events.HandleEvent, "EventsService", string.Format("Queued retry for build id '{0}' in project {1}", (object) buildId, (object) projectId));
            PipelineEventLogger.RetryBuild(requestContext, gitEvent, buildId, buildData?.Status.ToString());
            return true;
          }
          catch (Exception ex)
          {
            requestContext.TraceInfo(TracePoints.Provider.HandleEvent, "EventsService", string.Format("Failed to retry build with id '{0}'. Error: {1}", (object) buildId, (object) ex));
            PipelineEventLogger.LogException(requestContext, gitEvent, ex);
            return false;
          }
        }
      }
      return false;
    }

    private static void HandleExternalGitPush(
      IVssRequestContext requestContext,
      Guid projectId,
      List<BuildDefinition> definitions,
      IPipelineSourceProvider provider,
      ExternalGitPush push)
    {
      if (requestContext.IsFeatureEnabled("Pipelines.DoNotHandleGhAppCiTriggersOnATs"))
        BuildEventHelper.QueueGitHubAppPushHandlerJob(requestContext, projectId, definitions, provider, push);
      else
        BuildEventHelper.ParseCITriggersFromYamlFilesAndQueueBuilds(requestContext, definitions, provider, push);
    }

    private static void QueueGitHubAppPushHandlerJob(
      IVssRequestContext requestContext,
      Guid projectId,
      List<BuildDefinition> definitions,
      IPipelineSourceProvider provider,
      ExternalGitPush push)
    {
      GitHubAppPushHandlerJobData pushHandlerJobData = new GitHubAppPushHandlerJobData()
      {
        ProjectId = projectId,
        PipelineProviderId = provider.ProviderId,
        DefinitionIds = definitions.Select<BuildDefinition, int>((Func<BuildDefinition, int>) (d => d.Id)).ToList<int>(),
        Push = push
      };
      requestContext.GetService<TeamFoundationJobService>().QueueOneTimeJob(requestContext, "GitHubAppPushHandlerJobExtension", typeof (GitHubAppPushHandlerJobExtension).FullName, pushHandlerJobData.Serialize(), true);
    }

    internal static List<BuildData> ParseCITriggersFromYamlFilesAndQueueBuilds(
      IVssRequestContext requestContext,
      List<BuildDefinition> definitions,
      IPipelineSourceProvider provider,
      ExternalGitPush push)
    {
      requestContext.TraceInfo(TracePoints.Events.HandleEvent, "EventsService", "HandleExternalGitPush - provider=" + provider.ProviderId + ", sha=" + push.AfterSha);
      IdentityRef identityRefFromEmail = ExternalBuildHelper.GetIdentityRefFromEmail(requestContext, push.PushedBy?.Email);
      RepositoryUpdateInfo repositoryUpdateInfo = push.GetRepositoryUpdateInfo(requestContext, provider.ConnectionCreator.RepositoryType);
      List<TriggerLoadError> triggerLoadErrors = new List<TriggerLoadError>();
      List<TriggerInstance> ciBranchUpdates = FilteredBuildTriggerHelper.GetCIBranchUpdates(requestContext, definitions, repositoryUpdateInfo, identityRefFromEmail, triggerLoadErrors, (IExternalGitEvent) push);
      requestContext.TraceInfo(0, "EventsService", string.Format("Retrieved {0} trigger instances for the push", (object) ciBranchUpdates?.Count));
      List<BuildData> builds = FilteredBuildTriggerHelper.QueueBuildsForRepositoryUpdate(requestContext, ciBranchUpdates, triggerLoadErrors, (Action<BuildData>) (b =>
      {
        b.Properties[BuildProperties.PipelinesProvider] = (object) provider.ProviderId;
        b.Properties[BuildProperties.PipelineEventId] = (object) push.PipelineEventId;
      }));
      PipelineEventLogger.CIBuildsQueued(requestContext, push, (IEnumerable<BuildData>) builds);
      return builds;
    }

    private static void HandleExternalGitPullRequest(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      ExternalGitPullRequest pullRequest,
      JObject authentication,
      Guid hostId)
    {
      requestContext.TraceInfo(TracePoints.Events.HandleEvent, "EventsService", "HandleExternalGitPullRequest - provider=" + provider.ProviderId);
      PullRequestHandlerJobData jobData = new PullRequestHandlerJobData()
      {
        HostId = hostId,
        Authentication = authentication,
        ProviderId = provider.ProviderId,
        PullRequest = pullRequest,
        ExecutionCount = 0
      };
      int result;
      if (int.TryParse(pullRequest.DefinitionToBuild, out result))
        jobData.DefinitionId = new int?(result);
      XmlNode jsonXmlNode = jobData.SerializeToJsonXmlNode();
      Guid guid = PullRequestHandlerJobUtilities.QueuePullRequestHandlerJob(requestContext, jsonXmlNode);
      requestContext.TraceInfo(TracePoints.Events.HandleEvent, "EventsService", string.Format("Queued GitHubPullRequestHandlerJob {0} for pull_request={1} with provider {2}", (object) guid, (object) pullRequest.Url, (object) provider.ProviderId));
    }
  }
}
