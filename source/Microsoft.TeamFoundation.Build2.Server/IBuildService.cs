// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server.ObjectModel;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildService))]
  public interface IBuildService : IVssFrameworkService
  {
    Task<IList<BuildData>> GetAllBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null);

    Task<IList<BuildData>> GetBuildsByDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null);

    Task<IList<BuildData>> GetQueuedBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null);

    Task<IList<BuildData>> GetQueuedBuildsByDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null);

    Task<IList<BuildData>> GetRunningBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null);

    Task<IList<BuildData>> GetRunningBuildsByDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null);

    Task<IList<BuildData>> GetCompletedBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null);

    Task<IList<BuildData>> GetCompletedBuildsByDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null);

    [Obsolete("Use GetBuildsLegacy")]
    IEnumerable<BuildData> GetBuilds(
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
      int? maxBuildsPerDefinition = null);

    IEnumerable<BuildData> GetBuildsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> buildIds,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false);

    Task<IEnumerable<BuildData>> GetBuildsByIdsAsync(
      IVssRequestContext requestContext,
      IEnumerable<int> buildIds,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false);

    Task<IList<BuildData>> GetDeletedBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxCount,
      int? definitionId,
      string folderPath,
      DateTime? maxQueueTime);

    Task<IList<BuildData>> FilterBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxCount,
      int? definitionId = null,
      string folderPath = null,
      HashSet<int> repositoryIds = null,
      HashSet<int> branchIds = null,
      HashSet<string> keywordFilter = null,
      HashSet<Guid> requestedForFilter = null,
      BuildResult? resultFilter = null,
      BuildStatus? statusFilter = null,
      HashSet<string> tagFilter = null,
      DateTime? minQueueTime = null,
      DateTime? maxQueueTime = null);

    Task<IList<BuildData>> FilterBuildsByBranchAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int? definitionId,
      string folderPath,
      IEnumerable<int> repositoryIds,
      IEnumerable<int> branchIds,
      DateTime? maxQueueTime,
      int maxCount);

    Task<IList<BuildData>> FilterBuildsByTagsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int? definitionId,
      string folderPath,
      HashSet<string> tagFilter,
      DateTime? maxQueueTime,
      int maxCount);

    BuildData QueueBuild(
      IVssRequestContext requestContext,
      BuildData build,
      IEnumerable<IBuildRequestValidator> validators,
      BuildRequestValidationFlags validationFlags = BuildRequestValidationFlags.None,
      string checkInTicket = null,
      int? sourceBuildId = null,
      [CallerMemberName] string callingMethod = null,
      [CallerFilePath] string callingFile = null);

    BuildData QueueBuild(
      IVssRequestContext requestContext,
      BuildData build,
      IEnumerable<IBuildRequestValidator> validators,
      out string finalYaml,
      BuildRequestValidationFlags validationFlags = BuildRequestValidationFlags.None,
      string checkInTicket = null,
      int? sourceBuildId = null,
      [CallerMemberName] string callingMethod = null,
      [CallerFilePath] string callingFile = null);

    Task<List<BuildData>> UpdateBuildsAsync(
      IVssRequestContext requestContext,
      List<BuildData> builds,
      bool publishSignalR = true,
      bool ignoreDeleteSpec = false);

    Task<int> DeleteBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<DeleteBuildSpec> deleteBuildSpecs,
      Microsoft.VisualStudio.Services.Identity.Identity deletedBy = null,
      bool ignoreLowPriorityLeases = false);

    int DestroyBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      DateTime maxDeletedTime,
      int maxBuilds);

    IEnumerable<string> AddTags(
      IVssRequestContext requestContext,
      BuildData build,
      IEnumerable<string> tags);

    IEnumerable<string> DeleteTags(
      IVssRequestContext requestContext,
      BuildData build,
      IEnumerable<string> tags);

    List<BuildForRetention> GetBuildsForRetention(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      DateTime minFinishTime,
      DateTime maxFinishTime,
      int count);

    BuildArtifact AddArtifact(
      IVssRequestContext requestContext,
      BuildData build,
      BuildArtifact artifact);

    IList<BuildArtifact> GetArtifacts(
      IVssRequestContext requestContext,
      BuildData build,
      string artifactName = null);

    GetChangesResult GetChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      bool includeSourceChange = false,
      int startId = 0,
      int maxChanges = 50);

    IEnumerable<Change> GetChangesBetweenBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      int fromBuildId,
      int toBuildId,
      int maxChanges);

    BuildWorkItemRefsResult GetIndirectBuildWorkItemRefs(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      IEnumerable<string> commitIds,
      int maxItems,
      IEnumerable<int> excludedIds = null);

    IList<ResourceRef> GetBuildWorkItemRefs(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      IEnumerable<string> commitIds,
      int maxItems);

    IList<ResourceRef> GetWorkItemsBetweenBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      int fromBuildId,
      int toBuildId,
      IEnumerable<string> commitIds,
      int maxItems);

    Task<BuildData> RetryBuildAsync(IVssRequestContext requestContext, Guid projectId, int buildId);

    PropertiesCollection UpdateProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      PropertiesCollection properties);

    BuildResult? GetBranchStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName,
      string stageName = null,
      string jobName = null,
      string configuration = null);

    BuildData GetLatestBuildForBranch(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName);

    Task<BuildData> GetLatestCompletedBuildAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryIdentifier,
      string repositoryType,
      string branchName);

    BuildData GetLatestSuccessfulBuildForBranch(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName,
      DateTime maxFinishTime);

    Task<IList<BuildData>> GetLatestBuildsUnderFolderAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string folderPath,
      DateTime? maxQueueTime,
      int count);

    void CancelStage(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string stageRefName);

    Task RetryStageAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string stageRefName,
      bool forceRetryAllJobs = false,
      bool retryDependencies = true);

    Task<RetentionLease> AddRetentionLease(
      IVssRequestContext requestContext,
      Guid projectId,
      string ownerId,
      int buildId,
      int daysValid,
      bool protectPipeline,
      int maxLeases);

    Task<IReadOnlyList<RetentionLease>> AddRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<RetentionLease> leases);

    Task<RetentionLease> GetRetentionLease(
      IVssRequestContext requestContext,
      Guid projectId,
      int leaseId);

    Task<IReadOnlyList<RetentionLease>> GetRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      string ownerId,
      int? definitionId,
      int? runId);

    Task<IReadOnlyList<RetentionLease>> GetRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<MinimalRetentionLease> minimalRetentionLeases);

    Task<IReadOnlyList<RetentionLease>> GetRetentionLeasesForRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> runIds);

    Task RemoveRetentionLease(
      IVssRequestContext requestContext,
      Guid projectId,
      string ownerId,
      int runId,
      int definitionId);

    Task RemoveRetentionLease(IVssRequestContext requestContext, Guid projectId, int leaseId);

    Task RemoveRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyList<MinimalRetentionLease> leases);

    Task RemoveRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyList<int> leaseIds);

    Task<RetentionLease> UpdateRetentionLease(
      IVssRequestContext requestContext,
      Guid projectId,
      int leaseId,
      RetentionLeaseUpdate lease);

    void SampleRetentionData(IVssRequestContext requestContext, int retentionDays);

    IEnumerable<BuildRetentionSample> GetRetentionHistory(
      IVssRequestContext requestContext,
      int lookbackDays);

    List<PoisonedBuild> GetPoisonedBuilds(IVssRequestContext requestContext);

    PlanConcurrency GetPlanConcurrency(IVssRequestContext requestContext, BuildData buildData);
  }
}
