// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDataExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildDataExtensions
  {
    private const string TraceLayer = "BuildDataExtensions";

    internal static ArtifactSpec CreateArtifactSpec(
      this BuildData value,
      IVssRequestContext requestContext)
    {
      return BuildDataExtensions.CreateBuildArtifactSpec(requestContext, value.ProjectId, value.Id);
    }

    internal static ArtifactSpec CreateBuildArtifactSpec(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      return requestContext.GetService<ITeamFoundationResourceManagementService>().GetServiceVersion(requestContext, "Build2", "Build").Version < 8 ? new ArtifactSpec(ArtifactPropertyKinds.Build, buildId, 0) : new ArtifactSpec(ArtifactPropertyKinds.Build, buildId, 0, projectId);
    }

    internal static TimelineData? GetTimelineData(
      this BuildData build,
      IVssRequestContext requestContext,
      Guid? timelineId = null,
      int? changeId = null,
      Guid? planId = null)
    {
      TimelineData timelineData = new TimelineData();
      (Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline, IOrchestrationProcess process, Guid? planId) dtTimelineData = build.GetDTTimelineData(requestContext, timelineId, changeId, planId);
      if (dtTimelineData.timeline == null)
        return new TimelineData?();
      timelineData.Timeline = dtTimelineData.timeline.ToBuildTimeline(requestContext, build.ProjectId, build.Id, build.ToSecuredObject());
      timelineData.Process = dtTimelineData.process;
      return new TimelineData?(timelineData);
    }

    internal static IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline> GetTimelines(
      this BuildData build,
      IVssRequestContext requestContext,
      Guid planId)
    {
      IBuildOrchestrator service = requestContext.GetService<IBuildOrchestrator>();
      using (PerformanceTimer.StartMeasure(requestContext, "BuildDataExtensions.GetTimelines"))
      {
        if (service.GetPlan(requestContext, build.ProjectId, planId) == null)
          return Enumerable.Empty<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline>();
      }
      return service.GetTimelines(requestContext, build.ProjectId, planId);
    }

    internal static Guid? GetPlanId(this IReadOnlyBuildData build)
    {
      if (!build.Deleted)
        return new Guid?(build.OrchestrationPlan.PlanId);
      return build.Plans.Select<TaskOrchestrationPlanReference, TaskOrchestrationPlanReference>((Func<TaskOrchestrationPlanReference, TaskOrchestrationPlanReference>) (x => x)).FirstOrDefault<TaskOrchestrationPlanReference>((Func<TaskOrchestrationPlanReference, bool>) (x =>
      {
        int? orchestrationType = x.OrchestrationType;
        int num = 2;
        return orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue;
      }))?.PlanId;
    }

    internal static (Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline, IOrchestrationProcess process, Guid? planId) GetDTTimelineData(
      this BuildData build,
      IVssRequestContext requestContext,
      Guid? timelineId = null,
      int? changeId = null,
      Guid? planId = null)
    {
      planId = planId ?? build.GetPlanId();
      if (!planId.HasValue)
        return ((Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline) null, (IOrchestrationProcess) null, new Guid?());
      using (PerformanceTimer.StartMeasure(requestContext, "BuildDataExtensions.GetTimeline"))
      {
        IBuildOrchestrator service = requestContext.GetService<IBuildOrchestrator>();
        TaskOrchestrationPlan plan = service.GetPlan(requestContext, build.ProjectId, planId.Value);
        if (plan == null)
          return ((Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline) null, (IOrchestrationProcess) null, new Guid?());
        if (!timelineId.HasValue)
          timelineId = new Guid?(plan.Timeline.Id);
        return (service.GetTimeline(requestContext, plan.ScopeIdentifier, plan.PlanId, timelineId.Value, changeId.GetValueOrDefault(), true) ?? throw new Microsoft.TeamFoundation.Build.WebApi.TimelineNotFoundException(BuildServerResources.TimelineNotFound((object) timelineId)), plan.Process, new Guid?(plan.PlanId));
      }
    }

    internal static (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline> timelines, IOrchestrationProcess process, Guid? planId) GetDTTimelinesForStage(
      this BuildData build,
      IVssRequestContext requestContext,
      string stageName)
    {
      IBuildOrchestrator service = requestContext.GetService<IBuildOrchestrator>();
      Guid planId;
      if (!build.Deleted)
      {
        planId = build.OrchestrationPlan.PlanId;
      }
      else
      {
        TaskOrchestrationPlanReference orchestrationPlanReference = build.Plans.Select<TaskOrchestrationPlanReference, TaskOrchestrationPlanReference>((Func<TaskOrchestrationPlanReference, TaskOrchestrationPlanReference>) (x => x)).FirstOrDefault<TaskOrchestrationPlanReference>((Func<TaskOrchestrationPlanReference, bool>) (x =>
        {
          int? orchestrationType = x.OrchestrationType;
          int num = 2;
          return orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue;
        }));
        if (orchestrationPlanReference == null)
          return ((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline>) null, (IOrchestrationProcess) null, new Guid?());
        planId = orchestrationPlanReference.PlanId;
      }
      using (PerformanceTimer.StartMeasure(requestContext, "BuildDataExtensions.GetTimeline"))
      {
        TaskOrchestrationPlan plan = service.GetPlan(requestContext, build.ProjectId, planId);
        if (plan == null)
          return ((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline>) null, (IOrchestrationProcess) null, new Guid?());
        IList<StageAttempt> attempts = service.GetAttempts(requestContext, plan.ScopeIdentifier, plan.PlanId, stageName);
        return attempts == null ? ((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline>) null, (IOrchestrationProcess) null, new Guid?()) : (attempts.Select<StageAttempt, Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline>((Func<StageAttempt, Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline>) (x => x.Timeline)), plan.Process, new Guid?(plan.PlanId));
      }
    }

    internal static void PopulateProperties(this BuildData build, IVssRequestContext requestContext) => ((IList<BuildData>) new BuildData[1]
    {
      build
    }).PopulateProperties(requestContext);

    internal static void PopulateProperties(
      this IList<BuildData> builds,
      IVssRequestContext requestContext)
    {
      using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, builds.Select<BuildData, ArtifactSpec>((Func<BuildData, ArtifactSpec>) (b => b.CreateArtifactSpec(requestContext))), (IEnumerable<string>) null))
        ArtifactPropertyKinds.MatchProperties<BuildData>(properties, builds, (Func<BuildData, int>) (x => x.Id), (Action<BuildData, PropertiesCollection>) ((x, y) => x.Properties = y));
    }

    internal static string GetShortBranchName(this BuildData build)
    {
      string shortBranchName = build.SourceBranch;
      if (!string.IsNullOrEmpty(shortBranchName))
      {
        shortBranchName = shortBranchName.TrimEnd('/');
        int num = shortBranchName.LastIndexOf('/');
        if (num >= 0 && num < shortBranchName.Length)
          shortBranchName = shortBranchName.Substring(num + 1);
      }
      if (shortBranchName == null)
        shortBranchName = string.Empty;
      return shortBranchName;
    }

    internal static bool IsTriggeredByResourceRepository(
      this BuildData build,
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      return build.TryGetResourceRepositoryInfo(requestContext, definition, out string _, out string _);
    }

    private static bool TryGetResourceRepositoryInfo(
      this BuildData build,
      IVssRequestContext requestContext,
      BuildDefinition definition,
      out string repositoryId,
      out string sourceSha)
    {
      repositoryId = string.Empty;
      sourceSha = string.Empty;
      if (definition != null)
      {
        BuildProcess process = definition.Process;
        if ((process != null ? (process.Type == 2 ? 1 : 0) : 0) != 0 && build.TriggerInfo.TryGetValue("ci.triggerRepository", out repositoryId) && !string.Equals(definition.Repository?.Id, repositoryId, StringComparison.OrdinalIgnoreCase))
          return build.TriggerInfo.TryGetValue("ci.sourceSha", out sourceSha);
      }
      return false;
    }

    internal static List<Change> GetChangesFromResourceRepository(
      this BuildData build,
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      IBuildSourceProviderService sourceProviderService;
      List<string> commitIds;
      BuildRepository providerAndCommitIds = build.GetResourceRepositoryWithProviderAndCommitIds(requestContext, definition, out sourceProviderService, out commitIds);
      return sourceProviderService.GetSourceProvider(requestContext, providerAndCommitIds.Type).GetChanges(requestContext, build.ProjectId, providerAndCommitIds, (IEnumerable<string>) commitIds);
    }

    internal static BuildRepository GetResourceRepository(
      this BuildData build,
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      return build.GetResourceRepositoryWithProviderAndCommitIds(requestContext, definition, out IBuildSourceProviderService _, out List<string> _);
    }

    private static BuildRepository GetResourceRepositoryWithProviderAndCommitIds(
      this BuildData build,
      IVssRequestContext requestContext,
      BuildDefinition definition,
      out IBuildSourceProviderService sourceProviderService,
      out List<string> commitIds)
    {
      string sourceSha;
      string repositoryId;
      build.TryGetResourceRepositoryInfo(requestContext, definition, out repositoryId, out sourceSha);
      commitIds = new List<string>() { sourceSha };
      YamlPipelineLoadResult pipelineLoadResult = definition.LoadYamlPipeline(requestContext, false);
      RepositoryResource repositoryResource = pipelineLoadResult.Template.Resources.Repositories.Union<RepositoryResource>((IEnumerable<RepositoryResource>) pipelineLoadResult.Environment.Resources.Repositories).FirstOrDefault<RepositoryResource>((Func<RepositoryResource, bool>) (r => string.Equals(r.Id, repositoryId, StringComparison.OrdinalIgnoreCase)));
      BuildRepository repository;
      if (repositoryResource == null)
      {
        bool flag = pipelineLoadResult.Template.Errors.Count > 0;
        if (flag)
          requestContext.TraceAlways(100161006, TraceLevel.Info, "Build2", nameof (BuildDataExtensions), pipelineLoadResult.Template.Errors.First<PipelineValidationError>().Message);
        ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        Guid result;
        Guid.TryParse(repositoryId, out result);
        try
        {
          ITfsGitRepository repositoryById = service.FindRepositoryById(requestContext, result);
          BuildRepository buildRepository = new BuildRepository();
          buildRepository.Id = repositoryId;
          buildRepository.Name = repositoryById.Name;
          buildRepository.Url = new Uri(repositoryById.GetRepositoryWebUri());
          buildRepository.Type = "TfsGit";
          repository = buildRepository;
        }
        catch (GitRepositoryNotFoundException ex)
        {
          string str = flag ? BuildServerResources.YamlIsBroken() : string.Empty;
          throw new ResourceNotFoundException(BuildServerResources.RepositoryNotFound((object) repositoryId, (object) definition?.Id, (object) str));
        }
      }
      else
      {
        BuildRepository buildRepository = new BuildRepository();
        buildRepository.Id = repositoryResource.Id;
        buildRepository.Name = repositoryResource.Name;
        buildRepository.Url = repositoryResource.Url;
        buildRepository.Type = repositoryResource.Type;
        repository = buildRepository;
        repository.FixBuildRepositoryType();
      }
      sourceProviderService = requestContext.GetService<IBuildSourceProviderService>();
      return repository;
    }

    public static void PopulateHostedAgentImageId(
      this BuildData build,
      TaskOrchestrationContainer container)
    {
      string str;
      if (!build.Properties.TryGetValue<string>(TaskAgentRequestConstants.HostedAgentImageIdKey, out str))
        return;
      container.Data[TaskAgentRequestConstants.HostedAgentImageIdKey] = str;
    }

    public static BuildData UpdateReferences(
      this BuildData build,
      IVssRequestContext requestContext)
    {
      if (build != null)
      {
        if (build.Definition != null)
          build.Definition.ProjectId = build.ProjectId;
        requestContext.TraceInfo(0, "Performance", "Update references for buildData srv object {0} completed.", (object) build.Id);
      }
      return build;
    }

    public static string GetBuildSourceVersionMessage(
      this BuildData build,
      IVssRequestContext requestContext)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BuildDataExtensions.GetBuildSourceVersionMessage"))
      {
        try
        {
          string enumerable;
          if (build.TriggerInfo.TryGetValue("ci.message", out enumerable))
          {
            if (!enumerable.IsNullOrEmpty<char>())
              return enumerable;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceError(nameof (BuildDataExtensions), "TriggerInfo failed to deserialize the string {0} with the following error: {1}", (object) build.TriggerInfoString, (object) ex.Message);
        }
        if (build.SourceVersionInfo.Message != null)
          return build.SourceVersionInfo.Message;
        IVssRequestContext requestContext1 = requestContext;
        object[] objArray1 = new object[9]
        {
          (object) build.BuildNumber,
          (object) build.ProjectId,
          (object) build.Id,
          (object) build.SourceVersion,
          (object) build.Status,
          (object) build.Result,
          null,
          null,
          null
        };
        DateTime? nullable = build.QueueTime;
        ref DateTime? local = ref nullable;
        objArray1[6] = (object) (local.HasValue ? local.GetValueOrDefault().ToString("d", (IFormatProvider) CultureInfo.InvariantCulture) : (string) null);
        objArray1[7] = (object) build.Reason;
        objArray1[8] = (object) build.Repository?.Type;
        string format = string.Format("No persisted message for {0} (ProjectId: {1}, BuildId: {2}, SourceVersion: {3}, Status: {4}, Result: {5}, QueueDate: {6}, Reason: {7}, RepoType: {8}). Older build?", objArray1);
        object[] objArray2 = Array.Empty<object>();
        requestContext1.TraceAlways(0, TraceLevel.Warning, "SourceVersionInfo", nameof (BuildDataExtensions), format, objArray2);
        string sourceVersionMessage;
        if (build.Reason == BuildReason.PullRequest && build.TriggerInfo != null && build.TriggerInfo.TryGetValue("pr.title", out sourceVersionMessage))
          return sourceVersionMessage;
        IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, build.Repository?.Type, false);
        if (sourceProvider != null)
        {
          if (sourceProvider.GetAttributes(requestContext).IsExternal)
            return string.Empty;
          if (build.Reason == BuildReason.PullRequest)
          {
            int result = 0;
            string s = (string) null;
            build.TriggerInfo.TryGetValue("pr.number", out s);
            if (!string.IsNullOrWhiteSpace(s))
              int.TryParse(s, out result);
            else
              result = sourceProvider.ExtractPullRequestIdFromSourceBranch(build.SourceBranch);
            if (result > 0)
            {
              PullRequest pullRequest = (PullRequest) null;
              try
              {
                pullRequest = sourceProvider.GetPullRequest(requestContext, (IReadOnlyBuildData) build, result.ToString((IFormatProvider) CultureInfo.InvariantCulture));
              }
              catch (Exception ex)
              {
                requestContext.TraceException(12030167, "Service", ex);
              }
              if (pullRequest != null)
                return pullRequest.Title ?? string.Empty;
            }
          }
          IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
          IBuildDefinitionService definitionService = service;
          IVssRequestContext requestContext2 = requestContext;
          Guid projectId = build.ProjectId;
          int id = build.Definition.Id;
          int? revision = build.Definition.Revision;
          nullable = new DateTime?();
          DateTime? minMetricsTime = nullable;
          BuildDefinition definition = definitionService.GetDefinition(requestContext2, projectId, id, revision, includeDeleted: true, minMetricsTime: minMetricsTime);
          if (definition == null)
          {
            requestContext.TraceWarning(100161007, "Build2", nameof (BuildDataExtensions), (object) "Skipping build {0} as we could not find definition {1} of project {2} with revision {3}", (object) build.Id, (object) build.Definition.Id, (object) build.ProjectId, (object) build.Definition.Revision);
            return string.Empty;
          }
          BuildRepository expandedRepository = service.GetExpandedRepository(requestContext, definition);
          return sourceProvider.GetSourceVersionMessage(requestContext, build.ProjectId, expandedRepository, build.SourceVersion) ?? string.Empty;
        }
      }
      return string.Empty;
    }
  }
}
