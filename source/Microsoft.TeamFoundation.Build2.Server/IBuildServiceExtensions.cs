// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class IBuildServiceExtensions
  {
    private const string c_getBuildsByIds = "GetBuildsByIds";
    private const string TraceLayer = "IBuildServiceExtensions";

    public static IEnumerable<BuildData> GetBuildsLegacy(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      string project,
      int count,
      IEnumerable<int> definitionIds = null,
      IEnumerable<int> queueIds = null,
      string buildNumber = "*",
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<string> tagFilters = null,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      QueryDeletedOption queryDeletedOption = QueryDeletedOption.ExcludeDeleted,
      string repositoryId = null,
      string repositoryType = null,
      string branchName = null,
      int? maxBuildsPerDefinition = null)
    {
      Guid projectId;
      if (!Guid.TryParse(project, out projectId))
        requestContext.GetService<IProjectService>().TryGetProjectId(requestContext, project, out projectId);
      return buildService.GetBuildsLegacy(requestContext, projectId, count, definitionIds, queueIds, buildNumber, minFinishTime, maxFinishTime, requestedFor, reasonFilter, statusFilter, resultFilter, propertyFilters, tagFilters, queryOrder, queryDeletedOption, repositoryId, repositoryType, branchName, maxBuildsPerDefinition);
    }

    public static IEnumerable<BuildData> GetBuildsLegacy(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds = null,
      IEnumerable<int> queueIds = null,
      string buildNumber = "*",
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<string> tagFilters = null,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      QueryDeletedOption queryDeletedOption = QueryDeletedOption.ExcludeDeleted,
      string repositoryId = null,
      string repositoryType = null,
      string branchName = null,
      int? maxBuildsPerDefinition = null)
    {
      List<BuildData> buildsLegacy = new List<BuildData>();
      string str = requestContext.Method?.Name;
      if (string.IsNullOrEmpty(str) || !str.Contains("|"))
        str = IBuildServiceExtensions.GetBuildsMethodName(requestContext, definitionIds, buildNumber, reasonFilter, statusFilter, resultFilter, tagFilters, queryOrder, queryDeletedOption, repositoryId, repositoryType, branchName, maxBuildsPerDefinition, requestedFor, queueIds);
      bool flag = false;
      if (!str.Contains("|Legacy"))
      {
        if (str.Contains("GetBuilds|GetAllBuilds|"))
          buildsLegacy.AddRange((IEnumerable<BuildData>) buildService.GetAllBuilds(requestContext, projectId, count, queryOrder, propertyFilters: propertyFilters));
        else if (str.Contains("GetBuilds|GetBuildsByDefinitions|"))
          buildsLegacy.AddRange((IEnumerable<BuildData>) buildService.GetBuildsByDefinitions(requestContext, projectId, count, definitionIds, queryOrder, propertyFilters: propertyFilters));
        else if (str.Contains("GetBuilds|GetQueuedBuilds|"))
          buildsLegacy.AddRange((IEnumerable<BuildData>) buildService.GetQueuedBuilds(requestContext, projectId, count, queryOrder, propertyFilters: propertyFilters));
        else if (str.Contains("GetBuilds|GetQueuedBuildsByDefinitions|"))
          buildsLegacy.AddRange((IEnumerable<BuildData>) buildService.GetQueuedBuildsByDefinitions(requestContext, projectId, count, definitionIds, queryOrder, propertyFilters: propertyFilters));
        else if (str.Contains("GetBuilds|GetRunningBuilds|"))
          buildsLegacy.AddRange((IEnumerable<BuildData>) buildService.GetRunningBuilds(requestContext, projectId, count, queryOrder, propertyFilters: propertyFilters));
        else if (str.Contains("GetBuilds|GetRunningBuildsByDefinitions|"))
          buildsLegacy.AddRange((IEnumerable<BuildData>) buildService.GetRunningBuildsByDefinitions(requestContext, projectId, count, definitionIds, queryOrder, propertyFilters: propertyFilters));
        else if (str.Contains("GetBuilds|GetCompletedBuilds|"))
          buildsLegacy.AddRange((IEnumerable<BuildData>) buildService.GetCompletedBuilds(requestContext, projectId, count, queryOrder, propertyFilters: propertyFilters));
        else if (str.Contains("GetBuilds|GetCompletedBuildsByDefinitions|"))
          buildsLegacy.AddRange((IEnumerable<BuildData>) buildService.GetCompletedBuildsByDefinitions(requestContext, projectId, count, definitionIds, queryOrder, propertyFilters: propertyFilters));
        else
          flag = true;
      }
      else
        flag = true;
      if (flag)
        buildsLegacy.AddRange(buildService.GetBuilds(requestContext, projectId, count, definitionIds, queueIds, buildNumber, minFinishTime, maxFinishTime, requestedFor, reasonFilter, statusFilter, resultFilter, propertyFilters, tagFilters, queryOrder, queryDeletedOption, repositoryId, repositoryType, branchName, maxBuildsPerDefinition));
      return (IEnumerable<BuildData>) buildsLegacy;
    }

    public static string GetBuildsMethodName(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      IDictionary<string, object> arguments)
    {
      string buildsMethodName;
      if (arguments.IsNullOrEmpty<KeyValuePair<string, object>>())
      {
        buildsMethodName = IBuildServiceExtensions.GetMethodName(requestContext, "GetBuilds|GetAllBuilds|");
      }
      else
      {
        string castedValueOrDefault1 = arguments.GetCastedValueOrDefault<string, string>("definitions");
        string castedValueOrDefault2 = arguments.GetCastedValueOrDefault<string, string>("buildNumber", "*");
        Microsoft.TeamFoundation.Build.WebApi.BuildReason? castedValueOrDefault3 = arguments.GetCastedValueOrDefault<string, Microsoft.TeamFoundation.Build.WebApi.BuildReason?>("reasonFilter");
        Microsoft.TeamFoundation.Build.WebApi.BuildStatus? castedValueOrDefault4 = arguments.GetCastedValueOrDefault<string, Microsoft.TeamFoundation.Build.WebApi.BuildStatus?>("statusFilter");
        Microsoft.TeamFoundation.Build.WebApi.BuildResult? castedValueOrDefault5 = arguments.GetCastedValueOrDefault<string, Microsoft.TeamFoundation.Build.WebApi.BuildResult?>("resultFilter");
        string castedValueOrDefault6 = arguments.GetCastedValueOrDefault<string, string>("tagFilters");
        Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption castedValueOrDefault7 = arguments.GetCastedValueOrDefault<string, Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption>("deletedFilter");
        Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder castedValueOrDefault8 = arguments.GetCastedValueOrDefault<string, Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder>("queryOrder");
        string castedValueOrDefault9 = arguments.GetCastedValueOrDefault<string, string>("branchName");
        string castedValueOrDefault10 = arguments.GetCastedValueOrDefault<string, string>("buildIds");
        string castedValueOrDefault11 = arguments.GetCastedValueOrDefault<string, string>("repositoryId");
        string castedValueOrDefault12 = arguments.GetCastedValueOrDefault<string, string>("repositoryType");
        int? castedValueOrDefault13 = arguments.GetCastedValueOrDefault<string, int?>("maxBuildsPerDefinition");
        string castedValueOrDefault14 = arguments.GetCastedValueOrDefault<string, string>("requestedFor");
        string castedValueOrDefault15 = arguments.GetCastedValueOrDefault<string, string>("queues");
        if (!string.IsNullOrEmpty(castedValueOrDefault10))
        {
          buildsMethodName = "GetBuildsByIds";
        }
        else
        {
          IVssRequestContext requestContext1 = requestContext;
          IList<int> int32List1 = RestHelpers.ToInt32List(castedValueOrDefault1);
          string buildNumber = castedValueOrDefault2;
          Microsoft.TeamFoundation.Build.WebApi.BuildReason? nullable1 = castedValueOrDefault3;
          BuildReason? reasonFilter = nullable1.HasValue ? new BuildReason?((BuildReason) nullable1.GetValueOrDefault()) : new BuildReason?();
          Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable2 = castedValueOrDefault4;
          BuildStatus? statusFilter = nullable2.HasValue ? new BuildStatus?((BuildStatus) nullable2.GetValueOrDefault()) : new BuildStatus?();
          Microsoft.TeamFoundation.Build.WebApi.BuildResult? nullable3 = castedValueOrDefault5;
          BuildResult? resultFilter = nullable3.HasValue ? new BuildResult?((BuildResult) nullable3.GetValueOrDefault()) : new BuildResult?();
          IList<string> stringList = RestHelpers.ToStringList(castedValueOrDefault6);
          int serverBuildQueryOrder = (int) castedValueOrDefault8.ToServerBuildQueryOrder();
          int num = (int) castedValueOrDefault7;
          string repositoryId = castedValueOrDefault11;
          string repositoryType = castedValueOrDefault12;
          string branchName = castedValueOrDefault9;
          int? maxBuildsPerDefinition = castedValueOrDefault13;
          string requestedFor = castedValueOrDefault14;
          IList<int> int32List2 = RestHelpers.ToInt32List(castedValueOrDefault15);
          buildsMethodName = IBuildServiceExtensions.GetBuildsMethodName(requestContext1, (IEnumerable<int>) int32List1, buildNumber, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) stringList, (BuildQueryOrder) serverBuildQueryOrder, (QueryDeletedOption) num, repositoryId, repositoryType, branchName, maxBuildsPerDefinition, requestedFor, (IEnumerable<int>) int32List2);
        }
      }
      return buildsMethodName;
    }

    public static IList<BuildData> GetAllBuilds(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return requestContext.RunSynchronously<IList<BuildData>>((Func<Task<IList<BuildData>>>) (() => buildService.GetAllBuildsAsync(requestContext, projectId, count, queryOrder, timeFilter, propertyFilters)));
    }

    public static BuildData GetLatestCompletedBuild(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryIdentifier,
      string repositoryType,
      string branchName)
    {
      return requestContext.RunSynchronously<BuildData>((Func<Task<BuildData>>) (() => buildService.GetLatestCompletedBuildAsync(requestContext, projectId, repositoryIdentifier, repositoryType, branchName)));
    }

    public static IList<BuildData> GetLatestBuildsUnderFolder(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      string folderPath,
      DateTime? maxFinishTime,
      int count)
    {
      return requestContext.RunSynchronously<IList<BuildData>>((Func<Task<IList<BuildData>>>) (() => buildService.GetLatestBuildsUnderFolderAsync(requestContext, projectId, folderPath, maxFinishTime, count)));
    }

    public static IList<BuildData> GetBuildsByDefinitions(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return requestContext.RunSynchronously<IList<BuildData>>((Func<Task<IList<BuildData>>>) (() => buildService.GetBuildsByDefinitionsAsync(requestContext, projectId, count, definitionIds, queryOrder, timeFilter, propertyFilters)));
    }

    public static IList<BuildData> GetQueuedBuilds(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return requestContext.RunSynchronously<IList<BuildData>>((Func<Task<IList<BuildData>>>) (() => buildService.GetQueuedBuildsAsync(requestContext, projectId, count, queryOrder, timeFilter, propertyFilters)));
    }

    public static IList<BuildData> GetQueuedBuildsByDefinitions(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return requestContext.RunSynchronously<IList<BuildData>>((Func<Task<IList<BuildData>>>) (() => buildService.GetQueuedBuildsByDefinitionsAsync(requestContext, projectId, count, definitionIds, queryOrder, timeFilter, propertyFilters)));
    }

    public static IList<BuildData> GetRunningBuilds(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return requestContext.RunSynchronously<IList<BuildData>>((Func<Task<IList<BuildData>>>) (() => buildService.GetRunningBuildsAsync(requestContext, projectId, count, queryOrder, timeFilter, propertyFilters)));
    }

    public static IList<BuildData> GetRunningBuildsByDefinitions(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return requestContext.RunSynchronously<IList<BuildData>>((Func<Task<IList<BuildData>>>) (() => buildService.GetRunningBuildsByDefinitionsAsync(requestContext, projectId, count, definitionIds, queryOrder, timeFilter, propertyFilters)));
    }

    public static IList<BuildData> GetCompletedBuilds(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return requestContext.RunSynchronously<IList<BuildData>>((Func<Task<IList<BuildData>>>) (() => buildService.GetCompletedBuildsAsync(requestContext, projectId, count, queryOrder, timeFilter, propertyFilters)));
    }

    public static IList<BuildData> GetCompletedBuildsByDefinitions(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      string project,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      Guid projectId;
      if (!Guid.TryParse(project, out projectId))
        requestContext.GetService<IProjectService>().TryGetProjectId(requestContext, project, out projectId);
      return buildService.GetCompletedBuildsByDefinitions(requestContext, projectId, count, definitionIds, queryOrder, timeFilter, propertyFilters);
    }

    public static IList<BuildData> GetCompletedBuildsByDefinitions(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return requestContext.RunSynchronously<IList<BuildData>>((Func<Task<IList<BuildData>>>) (() => buildService.GetCompletedBuildsByDefinitionsAsync(requestContext, projectId, count, definitionIds, queryOrder, timeFilter, propertyFilters)));
    }

    public static BuildData GetBuildById(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false)
    {
      return buildService.GetBuildsByIds(requestContext, projectId, (IEnumerable<int>) new int[1]
      {
        buildId
      }, propertyFilters, (includeDeleted ? 1 : 0) != 0).FirstOrDefault<BuildData>();
    }

    internal static BuildData GetBuildById(
      this IBuildServiceInternal buildService,
      IVssRequestContext requestContext,
      int buildId,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false)
    {
      return buildService.GetBuildsByIds(requestContext, (IEnumerable<int>) new int[1]
      {
        buildId
      }, propertyFilters, (includeDeleted ? 1 : 0) != 0).FirstOrDefault<BuildData>();
    }

    public static async Task<BuildData> GetBuildByIdAsync(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      int buildId,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false)
    {
      return (await buildService.GetBuildsByIdsAsync(requestContext, (IEnumerable<int>) new int[1]
      {
        buildId
      }, propertyFilters, (includeDeleted ? 1 : 0) != 0)).FirstOrDefault<BuildData>();
    }

    internal static async Task SendRealtimeEventPayloadsToClient(
      this IBuildServiceInternal buildService,
      IVssRequestContext requestContext,
      int buildId,
      string clientId)
    {
      BuildData build = buildService.GetBuildById(requestContext, buildId);
      if (build == null)
      {
        build = (BuildData) null;
      }
      else
      {
        await requestContext.GetService<IBuildDispatcher>().SendBuildUpdatedAsync(requestContext, clientId, build.ProjectId, build.Definition.Id, buildId, build.Definition.Path);
        if (build.OrchestrationPlan == null)
        {
          build = (BuildData) null;
        }
        else
        {
          IBuildOrchestrator service = requestContext.GetService<IBuildOrchestrator>();
          TaskOrchestrationPlan plan = service.GetPlan(requestContext, build.ProjectId, build.OrchestrationPlan.PlanId);
          if (plan == null)
          {
            build = (BuildData) null;
          }
          else
          {
            Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = service.GetTimeline(requestContext, plan.ScopeIdentifier, plan.PlanId, plan.Timeline.Id, includeRecords: true);
            if (timeline == null)
            {
              build = (BuildData) null;
            }
            else
            {
              await TimelineRecordRealtimeEventHelper.SendTimelineRecordsUpdatedEventAsync(requestContext, build.ProjectId, build.Id, plan.PlanId, timeline.Id, (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>) timeline.Records);
              build = (BuildData) null;
            }
          }
        }
      }
    }

    public static BuildData UpdateBuild(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string buildNumber = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      BuildStatus? status = null,
      string sourceBranch = null,
      string sourceVersion = null,
      BuildResult? result = null,
      bool? keepForever = null,
      bool? retainedByRelease = null)
    {
      return requestContext.RunSynchronously<BuildData>((Func<Task<BuildData>>) (() => buildService.UpdateBuildAsync(requestContext, projectId, buildId, buildNumber, startTime, finishTime, status, sourceBranch, sourceVersion, result, keepForever, retainedByRelease)));
    }

    public static Task<BuildData> UpdateBuildAsync(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string buildNumber = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      BuildStatus? status = null,
      string sourceBranch = null,
      string sourceVersion = null,
      BuildResult? result = null,
      bool? keepForever = null,
      bool? retainedByRelease = null)
    {
      return buildService.UpdateBuildAsync(requestContext, new BuildData()
      {
        Id = buildId,
        ProjectId = projectId,
        BuildNumber = buildNumber,
        StartTime = startTime,
        FinishTime = finishTime,
        Status = status,
        SourceBranch = sourceBranch,
        SourceVersion = sourceVersion,
        Result = result,
        LegacyInputKeepForever = keepForever,
        LegacyInputRetainedByRelease = retainedByRelease
      });
    }

    public static BuildData UpdateBuild(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      BuildData build)
    {
      ArgumentUtility.CheckForNull<BuildData>(build, "Build");
      return requestContext.RunSynchronously<List<BuildData>>((Func<Task<List<BuildData>>>) (() => buildService.UpdateBuildsAsync(requestContext, new List<BuildData>()
      {
        build
      }))).Single<BuildData>();
    }

    public static async Task<BuildData> UpdateBuildAsync(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      BuildData build)
    {
      ArgumentUtility.CheckForNull<BuildData>(build, "Build");
      return (await buildService.UpdateBuildsAsync(requestContext, new List<BuildData>()
      {
        build
      })).Single<BuildData>();
    }

    public static List<BuildData> UpdateBuilds(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      List<BuildData> builds,
      bool publishSignalR = true)
    {
      return requestContext.RunSynchronously<List<BuildData>>((Func<Task<List<BuildData>>>) (() => buildService.UpdateBuildsAsync(requestContext, builds, publishSignalR)));
    }

    public static void AbortBuild(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      BuildData build,
      List<BuildRequestValidationResult> issues)
    {
      build.ValidationResults.AddRange((IEnumerable<BuildRequestValidationResult>) issues);
      BuildStatus? status = build.Status;
      BuildStatus buildStatus = BuildStatus.NotStarted;
      if (status.GetValueOrDefault() == buildStatus & status.HasValue)
      {
        build.Status = new BuildStatus?(BuildStatus.Completed);
        build.Result = new BuildResult?(BuildResult.Failed);
      }
      else
        build.Status = new BuildStatus?(BuildStatus.Cancelling);
      buildService.UpdateBuild(requestContext, build);
    }

    public static Task<List<BuildData>> AbortBuildsAsync(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      IEnumerable<BuildData> builds,
      List<BuildRequestValidationResult> issues)
    {
      foreach (BuildData build in builds)
      {
        build.ValidationResults.AddRange((IEnumerable<BuildRequestValidationResult>) issues);
        BuildStatus? status = build.Status;
        BuildStatus buildStatus = BuildStatus.NotStarted;
        if (status.GetValueOrDefault() == buildStatus & status.HasValue)
        {
          build.Status = new BuildStatus?(BuildStatus.Completed);
          build.Result = new BuildResult?(BuildResult.Failed);
        }
        else
          build.Status = new BuildStatus?(BuildStatus.Cancelling);
      }
      return buildService.UpdateBuildsAsync(requestContext, builds.ToList<BuildData>());
    }

    public static void DeleteBuild(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      buildService.DeleteBuilds(requestContext, projectId, (IEnumerable<int>) new int[1]
      {
        buildId
      });
    }

    public static void DeleteBuilds(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> buildIds)
    {
      IEnumerable<DeleteBuildSpec> deleteBuildSpecs = buildIds.Select<int, DeleteBuildSpec>((Func<int, DeleteBuildSpec>) (id =>
      {
        DeleteBuildSpec deleteBuildSpec = new DeleteBuildSpec()
        {
          BuildId = id,
          DeleteBuildRecord = true
        };
        deleteBuildSpec.ArtifactsToDelete.Add("build.SourceLabel");
        if (requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Build2.Retention.FilePathArtifactsAndSymbolsDelete"))
        {
          deleteBuildSpec.ArtifactTypesToDelete.Add("FilePath");
          deleteBuildSpec.ArtifactTypesToDelete.Add("SymbolStore");
          deleteBuildSpec.ArtifactTypesToDelete.Add("SymbolRequest");
          deleteBuildSpec.ArtifactTypesToDelete.Add("PipelineArtifact");
        }
        return deleteBuildSpec;
      }));
      buildService.DeleteBuilds(requestContext, projectId, deleteBuildSpecs);
    }

    public static int DeleteBuilds(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<DeleteBuildSpec> deleteBuildSpecs,
      Microsoft.VisualStudio.Services.Identity.Identity deletedBy = null,
      bool ignoreLowPriorityLeases = false)
    {
      return requestContext.RunSynchronously<int>((Func<Task<int>>) (() => buildService.DeleteBuildsAsync(requestContext, projectId, deleteBuildSpecs, deletedBy, ignoreLowPriorityLeases)));
    }

    public static BuildData QueueBuild(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      BuildData build)
    {
      return buildService.QueueBuild(requestContext, build, Enumerable.Empty<IBuildRequestValidator>(), callingMethod: nameof (QueueBuild), callingFile: "D:\\a\\_work\\1\\s\\Tfs\\Service\\Build2\\Server\\IBuildServiceExtensions.cs");
    }

    private static string GetBuildsMethodName(
      IVssRequestContext requestContext,
      IEnumerable<int> definitionIds = null,
      string buildNumber = "*",
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      QueryDeletedOption queryDeletedOption = QueryDeletedOption.ExcludeDeleted,
      string repositoryId = null,
      string repositoryType = null,
      string branchName = null,
      int? maxBuildsPerDefinition = null,
      string requestedFor = null,
      IEnumerable<int> queueIds = null)
    {
      string existingMethodName = "GetBuilds";
      if (queryDeletedOption == QueryDeletedOption.ExcludeDeleted && !IBuildServiceExtensions.IsBuildNumberSet(buildNumber) && !reasonFilter.HasValue && !resultFilter.HasValue && tagFilters.IsNullOrEmpty<string>() && !maxBuildsPerDefinition.HasValue && requestedFor == null && queueIds.IsNullOrEmpty<int>())
      {
        if (!IBuildServiceExtensions.IsRepositoryQuery(repositoryId, repositoryType, branchName))
        {
          if (!definitionIds.IsNullOrEmpty<int>())
          {
            BuildStatus? nullable1 = statusFilter;
            BuildStatus buildStatus1 = BuildStatus.Completed;
            if (nullable1.GetValueOrDefault() == buildStatus1 & nullable1.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.FinishTimeAscending || queryOrder == BuildQueryOrder.FinishTimeDescending))
            {
              existingMethodName = "GetBuilds|GetCompletedBuildsByDefinitions|";
            }
            else
            {
              BuildStatus? nullable2 = statusFilter;
              BuildStatus buildStatus2 = BuildStatus.NotStarted;
              if (nullable2.GetValueOrDefault() == buildStatus2 & nullable2.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.QueueTimeAscending || queryOrder == BuildQueryOrder.QueueTimeDescending))
              {
                existingMethodName = "GetBuilds|GetQueuedBuildsByDefinitions|";
              }
              else
              {
                BuildStatus? nullable3 = statusFilter;
                BuildStatus buildStatus3 = BuildStatus.InProgress;
                if (nullable3.GetValueOrDefault() == buildStatus3 & nullable3.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.StartTimeAscending || queryOrder == BuildQueryOrder.StartTimeDescending))
                {
                  existingMethodName = "GetBuilds|GetRunningBuildsByDefinitions|";
                }
                else
                {
                  BuildStatus? nullable4 = statusFilter;
                  BuildStatus buildStatus4 = BuildStatus.All;
                  if (nullable4.GetValueOrDefault() == buildStatus4 & nullable4.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.QueueTimeAscending || queryOrder == BuildQueryOrder.QueueTimeDescending))
                    existingMethodName = "GetBuilds|GetBuildsByDefinitions|";
                }
              }
            }
          }
          else if (definitionIds.IsNullOrEmpty<int>())
          {
            BuildStatus? nullable5 = statusFilter;
            BuildStatus buildStatus5 = BuildStatus.Completed;
            if (nullable5.GetValueOrDefault() == buildStatus5 & nullable5.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.FinishTimeAscending || queryOrder == BuildQueryOrder.FinishTimeDescending))
            {
              existingMethodName = "GetBuilds|GetCompletedBuilds|";
            }
            else
            {
              BuildStatus? nullable6 = statusFilter;
              BuildStatus buildStatus6 = BuildStatus.NotStarted;
              if (nullable6.GetValueOrDefault() == buildStatus6 & nullable6.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.QueueTimeAscending || queryOrder == BuildQueryOrder.QueueTimeDescending))
              {
                existingMethodName = "GetBuilds|GetQueuedBuilds|";
              }
              else
              {
                BuildStatus? nullable7 = statusFilter;
                BuildStatus buildStatus7 = BuildStatus.InProgress;
                if (nullable7.GetValueOrDefault() == buildStatus7 & nullable7.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.StartTimeAscending || queryOrder == BuildQueryOrder.StartTimeDescending))
                {
                  existingMethodName = "GetBuilds|GetRunningBuilds|";
                }
                else
                {
                  BuildStatus? nullable8 = statusFilter;
                  BuildStatus buildStatus8 = BuildStatus.All;
                  if (nullable8.GetValueOrDefault() == buildStatus8 & nullable8.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.QueueTimeAscending || queryOrder == BuildQueryOrder.QueueTimeDescending))
                    existingMethodName = "GetBuilds|GetAllBuilds|";
                }
              }
            }
          }
        }
        else if (IBuildServiceExtensions.IsRepositoryQuery(repositoryId, repositoryType, branchName) && !definitionIds.IsNullOrEmpty<int>())
        {
          BuildStatus? nullable9 = statusFilter;
          BuildStatus buildStatus9 = BuildStatus.Completed;
          if (nullable9.GetValueOrDefault() == buildStatus9 & nullable9.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.FinishTimeAscending || queryOrder == BuildQueryOrder.FinishTimeDescending))
          {
            existingMethodName = "GetBuilds|GetCompletedBuildsByRepository|";
          }
          else
          {
            BuildStatus? nullable10 = statusFilter;
            BuildStatus buildStatus10 = BuildStatus.NotStarted;
            if (nullable10.GetValueOrDefault() == buildStatus10 & nullable10.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.QueueTimeAscending || queryOrder == BuildQueryOrder.QueueTimeDescending))
            {
              existingMethodName = "GetBuilds|GetQueuedBuildsByRepository|";
            }
            else
            {
              BuildStatus? nullable11 = statusFilter;
              BuildStatus buildStatus11 = BuildStatus.InProgress;
              if (nullable11.GetValueOrDefault() == buildStatus11 & nullable11.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.StartTimeAscending || queryOrder == BuildQueryOrder.StartTimeDescending))
              {
                existingMethodName = "GetBuilds|GetRunningBuildsByRepository|";
              }
              else
              {
                BuildStatus? nullable12 = statusFilter;
                BuildStatus buildStatus12 = BuildStatus.All;
                if (nullable12.GetValueOrDefault() == buildStatus12 & nullable12.HasValue && (queryOrder == BuildQueryOrder.Ascending || queryOrder == BuildQueryOrder.Descending || queryOrder == BuildQueryOrder.QueueTimeAscending || queryOrder == BuildQueryOrder.QueueTimeDescending))
                  existingMethodName = "GetBuilds|GetBuildsByRepository|";
              }
            }
          }
        }
      }
      return IBuildServiceExtensions.GetMethodName(requestContext, existingMethodName);
    }

    public static BuildArtifact AddArtifact(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      BuildArtifact artifact)
    {
      ArgumentUtility.CheckForNull<IBuildService>(buildService, nameof (buildService));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (IBuildServiceExtensions), nameof (AddArtifact)))
        return buildService.AddArtifact(requestContext, buildService.GetBuildById(requestContext, projectId, buildId) ?? throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId)), artifact);
    }

    public static IList<BuildArtifact> GetArtifacts(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string artifactName = null)
    {
      ArgumentUtility.CheckForNull<IBuildService>(buildService, nameof (buildService));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (IBuildServiceExtensions), nameof (GetArtifacts)))
        return buildService.GetArtifacts(requestContext, buildService.GetBuildById(requestContext, projectId, buildId) ?? throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId)), artifactName);
    }

    public static IEnumerable<string> AddTags(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForNull<IBuildService>(buildService, nameof (buildService), "Build2");
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      ArgumentUtility.CheckForNonPositiveInt(buildId, nameof (buildId), "Build2");
      using (requestContext.TraceScope(nameof (IBuildServiceExtensions), nameof (AddTags)))
        return buildService.AddTags(requestContext, buildService.GetBuildById(requestContext, projectId, buildId) ?? throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId)), tags);
    }

    public static IEnumerable<string> DeleteTags(
      this IBuildService buildService,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForNull<IBuildService>(buildService, nameof (buildService), "Build2");
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      ArgumentUtility.CheckForNonPositiveInt(buildId, nameof (buildId), "Build2");
      using (requestContext.TraceScope(nameof (IBuildServiceExtensions), nameof (DeleteTags)))
        return buildService.DeleteTags(requestContext, buildService.GetBuildById(requestContext, projectId, buildId) ?? throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId)), tags);
    }

    private static string GetMethodName(
      IVssRequestContext requestContext,
      string existingMethodName)
    {
      if (!requestContext.IsFeatureEnabled("Build2.Service.UseNewGetBuilds"))
        existingMethodName += "|Legacy";
      return existingMethodName;
    }

    private static bool IsBuildNumberSet(string buildNumber) => !string.IsNullOrEmpty(buildNumber) && !string.Equals(buildNumber, "*");

    private static bool IsRepositoryQuery(
      string repositoryId = null,
      string repositoryType = null,
      string branchName = null)
    {
      return repositoryId != null && repositoryType != null || branchName != null;
    }
  }
}
