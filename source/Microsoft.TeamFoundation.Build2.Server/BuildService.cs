// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server.Helpers;
using Microsoft.TeamFoundation.Build2.Server.ObjectModel;
using Microsoft.TeamFoundation.Build2.Server.PipelinesMigration;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Orchestration.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class BuildService : IBuildService, IVssFrameworkService, IBuildServiceInternal
  {
    private const int c_CancelOrphanedBuildsInQueueBatchSize = 500;
    private const int c_MaxCleanupTaskGroupSize = 15000;
    private const string TraceLayer = "BuildService";
    private const string c_teamName = "ciBuild";
    private readonly IBuildSecurityProvider SecurityProvider;
    private readonly IBuildRequestHelperFactory BuildHelperFactory;
    private const string c_planIsNull = "BuildData.OrchestrationPlan is null";
    private const string c_planNotFound = "TaskOrchestrationPlan does not exist";
    private const string c_planIsComplete = "TaskOrchestrationPlan is completed";
    private const string c_BuildCancellingHang = "Build status hung in cancelling state";
    private readonly Guid c_buildEventPublisherJobId = new Guid("677E59D0-85C0-4D6F-8384-F40D6E76BD88");
    private const int c_eventPublisherJobDelay = 5;
    private const int c_poisonedBuildsCleanupBatch = 100;

    public void SampleRetentionData(IVssRequestContext requestContext, int retentionDays)
    {
      using (requestContext.TraceScope("Service", nameof (SampleRetentionData)))
      {
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          component.SampleRetentionData(retentionDays);
      }
    }

    public IEnumerable<BuildRetentionSample> GetRetentionHistory(
      IVssRequestContext requestContext,
      int lookbackDays)
    {
      using (requestContext.TraceScope("Service", nameof (GetRetentionHistory)))
      {
        this.SecurityProvider.CheckCollectionPermission(requestContext, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds);
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          return component.GetRetentionHistory(lookbackDays);
      }
    }

    public async Task<RetentionLease> AddRetentionLease(
      IVssRequestContext requestContext,
      Guid projectId,
      string ownerId,
      int buildId,
      int daysValid,
      bool protectPipeline,
      int maxLeases)
    {
      BuildService buildService = this;
      RetentionLease retentionLease1;
      using (requestContext.TraceScope(nameof (BuildService), nameof (AddRetentionLease)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        ArgumentUtility.CheckStringForNullOrEmpty(ownerId, nameof (ownerId));
        ArgumentUtility.CheckForNonPositiveInt(daysValid, nameof (daysValid));
        if (maxLeases < 0)
          throw new ArgumentOutOfRangeException(nameof (maxLeases));
        BuildData build = (await requestContext.UsingBuild2ComponentCallAsync<IEnumerable<BuildData>>((Func<Build2Component, Task<IEnumerable<BuildData>>>) (bc => bc.GetBuildsByIdsAsync((IEnumerable<int>) new int[1]
        {
          buildId
        }, false)))).SingleOrDefault<BuildData>();
        if (build == null)
          throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId));
        if (!buildService.SecurityProvider.HasBuildPermission(requestContext, projectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation))
          buildService.SecurityProvider.CheckBuildPermission(requestContext, projectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.RetainIndefinitely);
        else if (requestContext.IsFeatureEnabled("Build2.LogSuspiciousAccessToBuildApi") && !buildService.SecurityProvider.HasBuildPermission(requestContext, projectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.RetainIndefinitely))
          BuildSecurityProvider.LogUserWouldLooseAccess(requestContext);
        DateTime validUntil = DateTime.UtcNow + TimeSpan.FromDays((double) daysValid);
        RetentionLease retentionLease2 = await requestContext.UsingBuild2ComponentCallAsync<RetentionLease>((Func<Build2Component, Task<RetentionLease>>) (bc => bc.AddRetentionLease(projectId, ownerId, buildId, build.Definition.Id, validUntil, protectPipeline, maxLeases)));
        if (retentionLease2 == null)
          requestContext.TraceError(0, "Service", "No lease returned for buildId {0} and ownerId {1}", (object) buildId, (object) ownerId);
        buildService.AuditRetentionLeaseChanges(requestContext, projectId, (IEnumerable<RetentionLease>) new RetentionLease[1]
        {
          retentionLease2
        }, (IEnumerable<BuildData>) new BuildData[1]
        {
          build
        }, RetentionLeaseHelper.RetentionLeaseChange.LeaseAdded);
        retentionLease1 = retentionLease2;
      }
      return retentionLease1;
    }

    public async Task<IReadOnlyList<RetentionLease>> AddRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<RetentionLease> leases)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) leases, nameof (leases));
      foreach (RetentionLease lease in (IEnumerable<RetentionLease>) leases)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(lease.OwnerId, "OwnerId");
        if (lease.ValidUntil.Date <= DateTime.UtcNow.Date)
          throw new ArgumentOutOfRangeException("ValidUntil");
      }
      BuildData[] builds = (await requestContext.UsingBuild2ComponentCallAsync<IEnumerable<BuildData>>((Func<Build2Component, Task<IEnumerable<BuildData>>>) (bc => bc.GetBuildsByIdsAsync(leases.Select<RetentionLease, int>((Func<RetentionLease, int>) (lease => lease.RunId)).Distinct<int>(), false)))).ToArray<BuildData>();
      int[] array = leases.Select<RetentionLease, int>((Func<RetentionLease, int>) (lease => lease.RunId)).Except<int>(((IEnumerable<BuildData>) builds).Select<BuildData, int>((Func<BuildData, int>) (b => b.Id))).ToArray<int>();
      if (((IEnumerable<int>) array).Any<int>())
        throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) ((IEnumerable<int>) array).First<int>()));
      foreach (BuildData build in builds)
      {
        if (!this.SecurityProvider.HasBuildPermission(requestContext, projectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation))
          this.SecurityProvider.CheckBuildPermission(requestContext, projectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.RetainIndefinitely);
      }
      IReadOnlyList<RetentionLease> leases1 = await requestContext.UsingBuild2ComponentCallAsync<IReadOnlyList<RetentionLease>>((Func<Build2Component, Task<IReadOnlyList<RetentionLease>>>) (bc => bc.AddRetentionLeases(projectId, (IEnumerable<RetentionLease>) leases)));
      this.AuditRetentionLeaseChanges(requestContext, projectId, (IEnumerable<RetentionLease>) leases1, (IEnumerable<BuildData>) builds, RetentionLeaseHelper.RetentionLeaseChange.LeaseAdded);
      IReadOnlyList<RetentionLease> retentionLeaseList = leases1;
      builds = (BuildData[]) null;
      return retentionLeaseList;
    }

    public async Task<RetentionLease> GetRetentionLease(
      IVssRequestContext requestContext,
      Guid projectId,
      int leaseId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      ArgumentUtility.CheckForNonPositiveInt(leaseId, nameof (leaseId), "Build2");
      return await requestContext.UsingBuild2ComponentCallAsync<RetentionLease>((Func<Build2Component, Task<RetentionLease>>) (bc => bc.GetRetentionLeaseById(projectId, leaseId)));
    }

    public async Task<IReadOnlyList<RetentionLease>> GetRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      string ownerId,
      int? definitionId,
      int? runId)
    {
      BuildService buildService = this;
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      if (runId.HasValue)
      {
        ArgumentUtility.CheckForNull<int>(definitionId, nameof (definitionId), "Build2");
        ArgumentUtility.CheckForNonPositiveInt(runId.Value, nameof (runId), "Build2");
      }
      if (definitionId.HasValue)
        ArgumentUtility.CheckForNonPositiveInt(definitionId.Value, nameof (definitionId), "Build2");
      if (string.IsNullOrWhiteSpace(ownerId) && runId.HasValue)
        return await requestContext.UsingBuild2ComponentCallAsync<IReadOnlyList<RetentionLease>>((Func<Build2Component, Task<IReadOnlyList<RetentionLease>>>) (bc => bc.GetRetentionLeasesForRuns(projectId, ((IEnumerable<int>) new int[1]
        {
          runId.Value
        }).ToHashSet<int>())));
      return await buildService.GetRetentionLeases(requestContext, projectId, (IEnumerable<MinimalRetentionLease>) new MinimalRetentionLease[1]
      {
        new MinimalRetentionLease(ownerId, runId, definitionId)
      });
    }

    public async Task<IReadOnlyList<RetentionLease>> GetRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<MinimalRetentionLease> minimalRetentionLeases)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      bool useGetRetentionLeasesImprovement = requestContext.IsFeatureEnabled("Build2.UseGetRetentionLeasesImprovement");
      return await requestContext.UsingBuild2ComponentCallAsync<IReadOnlyList<RetentionLease>>((Func<Build2Component, Task<IReadOnlyList<RetentionLease>>>) (bc => bc.GetRetentionLeases(projectId, minimalRetentionLeases, useGetRetentionLeasesImprovement)));
    }

    public async Task<IReadOnlyList<RetentionLease>> GetRetentionLeasesForRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> runIds)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      return await requestContext.UsingBuild2ComponentCallAsync<IReadOnlyList<RetentionLease>>((Func<Build2Component, Task<IReadOnlyList<RetentionLease>>>) (bc => bc.GetRetentionLeasesForRuns(projectId, runIds.ToHashSet<int>())));
    }

    public Task RemoveRetentionLease(
      IVssRequestContext requestContext,
      Guid projectId,
      string ownerId,
      int runId,
      int definitionId)
    {
      return this.RemoveRetentionLeases(requestContext, projectId, (IReadOnlyList<MinimalRetentionLease>) new MinimalRetentionLease[1]
      {
        new MinimalRetentionLease(ownerId, new int?(runId), new int?(definitionId))
      });
    }

    public Task RemoveRetentionLease(
      IVssRequestContext requestContext,
      Guid projectId,
      int leaseId)
    {
      return this.RemoveRetentionLeases(requestContext, projectId, (IReadOnlyList<int>) new int[1]
      {
        leaseId
      });
    }

    public async Task RemoveRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyList<int> leaseIds)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) leaseIds, nameof (leaseIds));
      IReadOnlyList<BuildData> array;
      using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
        array = (IReadOnlyList<BuildData>) (await bc.GetBuildsByIdsAsync((await bc.GetRetentionLeasesByIds(projectId, leaseIds.ToHashSet<int>())).Select<RetentionLease, int>((Func<RetentionLease, int>) (lease => lease.RunId)).Distinct<int>(), false)).ToArray<BuildData>();
      this.ValidateRetentionLeasePermissions(requestContext, projectId, (IEnumerable<BuildData>) array);
      await this.RemoveRetentionLeases(requestContext, projectId, leaseIds.ToHashSet<int>(), array);
    }

    public async Task RemoveRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyList<MinimalRetentionLease> leases)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) leases, nameof (leases));
      foreach (MinimalRetentionLease lease in (IEnumerable<MinimalRetentionLease>) leases)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(lease.OwnerId, "OwnerId");
        ArgumentUtility.CheckForNull<int>(lease.RunId, "RunId");
        ArgumentUtility.CheckForNull<string>(lease.OwnerId, "OwnerId");
        ArgumentUtility.CheckForNonPositiveInt(lease.RunId.Value, "RunId");
        ArgumentUtility.CheckForNonPositiveInt(lease.DefinitionId.Value, "DefinitionId");
      }
      BuildData[] builds = (await requestContext.UsingBuild2ComponentCallAsync<IEnumerable<BuildData>>((Func<Build2Component, Task<IEnumerable<BuildData>>>) (bc => bc.GetBuildsByIdsAsync(leases.Select<MinimalRetentionLease, int>((Func<MinimalRetentionLease, int>) (lease => lease.RunId.Value)), false)))).ToArray<BuildData>();
      this.ValidateRetentionLeasePermissions(requestContext, projectId, (IEnumerable<BuildData>) builds);
      IReadOnlyList<RetentionLease> retentionLeases = await this.GetRetentionLeases(requestContext, projectId, (IEnumerable<MinimalRetentionLease>) leases);
      await this.RemoveRetentionLeases(requestContext, projectId, retentionLeases.Select<RetentionLease, int>((Func<RetentionLease, int>) (lease => lease.Id)).ToHashSet<int>(), (IReadOnlyList<BuildData>) builds);
      builds = (BuildData[]) null;
    }

    public async Task<RetentionLease> UpdateRetentionLease(
      IVssRequestContext requestContext,
      Guid projectId,
      int leaseId,
      RetentionLeaseUpdate leaseUpdate)
    {
      int? nullable = leaseUpdate != null ? leaseUpdate.DaysValid : throw new RetentionLeaseUpdateIsNull(BuildServerResources.RetentionLeaseUpdateIsNull());
      if (nullable.HasValue)
      {
        nullable = leaseUpdate.DaysValid;
        if (nullable.Value < 1)
          throw new ArgumentOutOfRangeException("DaysValid");
      }
      RetentionLease lease = (await requestContext.UsingBuild2ComponentCallAsync<IReadOnlyList<RetentionLease>>((Func<Build2Component, Task<IReadOnlyList<RetentionLease>>>) (bc => bc.GetRetentionLeasesByIds(projectId, new HashSet<int>()
      {
        leaseId
      })))).FirstOrDefault<RetentionLease>();
      if (lease == null)
        throw new RetentionLeaseNotFoundException(BuildServerResources.RetentionLeaseNotFound((object) leaseId));
      IEnumerable<BuildData> builds = await requestContext.UsingBuild2ComponentCallAsync<IEnumerable<BuildData>>((Func<Build2Component, Task<IEnumerable<BuildData>>>) (async bc => await bc.GetBuildsByIdsAsync((IEnumerable<int>) new int[1]
      {
        lease.RunId
      }, false)));
      this.ValidateRetentionLeasePermissions(requestContext, projectId, builds);
      return await requestContext.UsingBuild2ComponentCallAsync<RetentionLease>((Func<Build2Component, Task<RetentionLease>>) (bc => bc.UpdateRetentionLease(projectId, leaseId, leaseUpdate.DaysValid.HasValue ? new DateTime?(DateTime.UtcNow + TimeSpan.FromDays((double) Math.Min(leaseUpdate.DaysValid.Value, (int) (DateTime.MaxValue - DateTime.UtcNow).TotalDays))) : new DateTime?(), leaseUpdate.ProtectPipeline))) ?? throw new RetentionLeaseNotFoundException(BuildServerResources.RetentionLeaseNotFound((object) leaseId));
    }

    private async Task RemoveRetentionLeases(
      IVssRequestContext requestContext,
      Guid projectId,
      HashSet<int> leaseIds,
      IReadOnlyList<BuildData> buildUpdates)
    {
      IReadOnlyList<RetentionLease> leases = (IReadOnlyList<RetentionLease>) null;
      using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
        leases = await bc.DeleteRetentionLeases(projectId, leaseIds.ToHashSet<int>());
      this.AuditRetentionLeaseChanges(requestContext, projectId, (IEnumerable<RetentionLease>) leases, (IEnumerable<BuildData>) buildUpdates, RetentionLeaseHelper.RetentionLeaseChange.LeaseRemoved);
    }

    private void ValidateRetentionLeasePermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<BuildData> builds)
    {
      foreach (BuildData build in builds)
      {
        if (!this.SecurityProvider.HasBuildPermission(requestContext, projectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation, true))
          this.SecurityProvider.CheckBuildPermission(requestContext, projectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.RetainIndefinitely, true);
      }
    }

    private void AuditRetentionLeaseChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<RetentionLease> leases,
      IEnumerable<BuildData> builds,
      RetentionLeaseHelper.RetentionLeaseChange changeType)
    {
      leases = leases.Where<RetentionLease>((Func<RetentionLease, bool>) (lease => !lease.OwnerId.StartsWith("Branch"))).Where<RetentionLease>((Func<RetentionLease, bool>) (lease => !lease.OwnerId.StartsWith("Pipeline")));
      if (!leases.Any<RetentionLease>() || requestContext.IsFeatureEnabled("Build2.OptOutAuditRetainedByRelease"))
        return;
      string projectName;
      string str1 = requestContext.GetService<IProjectService>().TryGetProjectName(requestContext, projectId, out projectName) ? projectName : "Unknown";
      Dictionary<int, BuildData> dictionary = builds.ToDictionary<BuildData, int>((Func<BuildData, int>) (build => build.Id));
      foreach (RetentionLease lease in leases)
      {
        string str2 = string.Empty;
        BuildData buildData;
        if (dictionary.TryGetValue(lease.RunId, out buildData))
          str2 = buildData.BuildNumber;
        if (string.IsNullOrWhiteSpace(str2))
          str2 = string.Format("unknown/missing build id: {0}", (object) lease.RunId);
        IVssRequestContext requestContext1 = requestContext;
        string actionId = changeType == RetentionLeaseHelper.RetentionLeaseChange.LeaseAdded ? "Pipelines.RunRetained" : "Pipelines.RunUnretained";
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["RunId"] = (object) lease.RunId;
        data["RunName"] = (object) str2;
        data["RetentionLeaseId"] = (object) lease.Id;
        data["RetentionOwnerId"] = (object) lease.OwnerId;
        data["ProjectName"] = (object) str1;
        Guid guid = projectId;
        Guid targetHostId = new Guid();
        Guid projectId1 = guid;
        requestContext1.LogAuditEvent(actionId, data, targetHostId, projectId1);
      }
    }

    public BuildService()
      : this((IBuildSecurityProvider) new BuildSecurityProvider())
    {
    }

    internal BuildService(IBuildSecurityProvider securityProvider)
      : this(securityProvider, (IBuildRequestHelperFactory) new BuildRequestHelperFactory())
    {
    }

    internal BuildService(
      IBuildSecurityProvider securityProvider,
      IBuildRequestHelperFactory buildHelperFactory)
    {
      this.SecurityProvider = securityProvider;
      this.BuildHelperFactory = buildHelperFactory;
    }

    public IEnumerable<BuildData> GetBuilds(
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
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      repositoryId = SourceProviderHelper.NormalizeRepositoryId(requestContext, repositoryType, repositoryId);
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder apiBuildQueryOrder = queryOrder.ToWebApiBuildQueryOrder(statusFilter.HasValue ? statusFilter.Value : BuildStatus.Completed);
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuilds)))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        HashSet<Guid> guidSet = new HashSet<Guid>();
        if (!string.IsNullOrEmpty(requestedFor))
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = service.GetIdentities(requestContext, requestedFor);
          if (identities != null && identities.Count > 0)
          {
            guidSet.AddRange<Guid, HashSet<Guid>>(identities.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id)));
          }
          else
          {
            Guid result;
            if (!Guid.TryParse(requestedFor, out result))
              return (IEnumerable<BuildData>) new List<BuildData>();
            guidSet.Add(result);
          }
        }
        HashSet<int> definitionIds1 = new HashSet<int>();
        if (definitionIds != null)
          definitionIds1 = definitionIds.ToHashSet<int>();
        HashSet<int> queueIds1 = new HashSet<int>();
        if (queueIds != null)
          queueIds1 = queueIds.ToHashSet<int>();
        IEnumerable<BuildData> buildDatas = (IEnumerable<BuildData>) null;
        BuildService.BuildTimeRange timeRange = new BuildService.BuildTimeRange(minFinishTime, maxFinishTime);
        Lazy<string> parametersString = this.GetParametersString(definitionIds, queueIds, buildNumber, timeRange.MinTime, timeRange.MaxTime, requestedFor, reasonFilter, statusFilter, resultFilter, tagFilters, queryOrder, queryDeletedOption, repositoryId, repositoryType, branchName, maxBuildsPerDefinition);
        int maxMilliSeconds = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/SlowCommandThresholdsInMs/GetBuildsLegacy", 3000);
        using (requestContext.TraceSlowCall("Service", maxMilliSeconds, parametersString, nameof (GetBuilds)))
        {
          using (Build2Component component = requestContext.CreateComponent<Build2Component>())
            buildDatas = component.GetBuilds(projectId, (IEnumerable<int>) definitionIds1, (IEnumerable<int>) queueIds1, buildNumber, timeRange.MinTime, timeRange.MaxTime, (IEnumerable<Guid>) guidSet, reasonFilter, statusFilter, resultFilter, tagFilters, count, queryDeletedOption, queryOrder, (IList<int>) null, repositoryId, repositoryType, branchName, maxBuildsPerDefinition);
        }
        HashSet<int> definitionsWithoutPermission = new HashSet<int>();
        bool timeRangeUpdated;
        List<BuildData> allBuilds = this.FilterBuildsAndUpdateTimeRange(requestContext, buildDatas, timeRange, apiBuildQueryOrder, out timeRangeUpdated, ref definitionsWithoutPermission);
        if (definitionsWithoutPermission.Count > 0)
        {
          int count1 = definitionsWithoutPermission.Count;
          while (buildDatas.Any<BuildData>() && allBuilds.Count < count)
          {
            bool flag = count1 < definitionsWithoutPermission.Count;
            count1 = definitionsWithoutPermission.Count;
            if (timeRangeUpdated | flag)
            {
              requestContext.TraceInfo(12030056, "Service", "Making another call to prc_GetBuilds because definition security filtered some out.");
              using (requestContext.TraceSlowCall("Service", maxMilliSeconds, parametersString, nameof (GetBuilds)))
              {
                using (Build2Component component = requestContext.CreateComponent<Build2Component>())
                  buildDatas = component.GetBuilds(projectId, definitionIds, queueIds, buildNumber, timeRange.MinTime, timeRange.MaxTime, (IEnumerable<Guid>) guidSet, reasonFilter, statusFilter, resultFilter, tagFilters, count, queryDeletedOption, queryOrder, (IList<int>) definitionsWithoutPermission.ToList<int>(), repositoryId, repositoryType, branchName, maxBuildsPerDefinition);
              }
              allBuilds.AddRange(this.FilterBuildsAndUpdateTimeRange(requestContext, buildDatas, timeRange, apiBuildQueryOrder, out timeRangeUpdated, ref definitionsWithoutPermission).Where<BuildData>((Func<BuildData, bool>) (b => !allBuilds.Exists((Predicate<BuildData>) (x => b.Id == x.Id)))));
            }
            else
            {
              requestContext.TraceInfo(12030057, "Service", "Not making another call to prc_GetBuilds, even though count was not reached.");
              break;
            }
          }
        }
        this.UpdateBuildProperties(requestContext, allBuilds, propertyFilters);
        this.SortBuilds(allBuilds, apiBuildQueryOrder);
        return allBuilds.Take<BuildData>(count).Select<BuildData, BuildData>((Func<BuildData, BuildData>) (x => x.UpdateReferences(requestContext)));
      }
    }

    public Task<IList<BuildData>> GetDeletedBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxCount,
      int? definitionId,
      string folderPath,
      DateTime? maxQueueTime)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      return this.GetBuildsInternal(requestContext, maxCount, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((bc, timeRange, excludedDefinitionIds) => bc.GetDeletedBuilds(projectId, definitionId, folderPath, timeRange.MaxTime ?? DateTime.MaxValue, maxCount, excludedDefinitionIds)), BuildStatus.All);
    }

    public Task<IList<BuildData>> FilterBuildsAsync(
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
      DateTime? maxQueueTime = null)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      return this.GetBuildsInternal(requestContext, maxCount, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((bc, timeRange, excludedDefinitionIds) => bc.FilterBuildsAsync(projectId, definitionId, folderPath, repositoryIds, branchIds, keywordFilter, requestedForFilter, resultFilter, statusFilter, tagFilter, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), maxCount, excludedDefinitionIds)), BuildStatus.All);
    }

    public IEnumerable<BuildData> GetBuildsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> buildIds,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildsByIds)))
        return this.GetBuildsByIdsInternal(requestContext, buildIds, propertyFilters, includeDeleted, new Guid?(projectId)).Select<BuildData, BuildData>((Func<BuildData, BuildData>) (x => x.UpdateReferences(requestContext)));
    }

    public IEnumerable<BuildData> GetBuildsByIds(
      IVssRequestContext requestContext,
      IEnumerable<int> buildIds,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildsByIds)))
        return this.GetBuildsByIdsInternal(requestContext, buildIds, propertyFilters, includeDeleted, new Guid?()).Select<BuildData, BuildData>((Func<BuildData, BuildData>) (x => x.UpdateReferences(requestContext)));
    }

    public async Task<IEnumerable<BuildData>> GetBuildsByIdsAsync(
      IVssRequestContext requestContext,
      IEnumerable<int> buildIds,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false)
    {
      requestContext.AssertAsyncExecutionEnabled();
      IEnumerable<BuildData> buildsByIdsAsync;
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildsByIdsAsync)))
        buildsByIdsAsync = (await this.GetBuildsByIdsInternalAsync(requestContext, buildIds, propertyFilters, includeDeleted, new Guid?())).Select<BuildData, BuildData>((Func<BuildData, BuildData>) (x => x.UpdateReferences(requestContext)));
      return buildsByIdsAsync;
    }

    public async Task<IEnumerable<BuildData>> GetBuildsByIdsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> buildIds,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false)
    {
      requestContext.AssertAsyncExecutionEnabled();
      IEnumerable<BuildData> buildsByIdsAsync;
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildsByIdsAsync)))
        buildsByIdsAsync = (await this.GetBuildsByIdsInternalAsync(requestContext, buildIds, propertyFilters, includeDeleted, new Guid?(projectId))).Select<BuildData, BuildData>((Func<BuildData, BuildData>) (x => x.UpdateReferences(requestContext)));
      return buildsByIdsAsync;
    }

    public Task<IList<BuildData>> GetAllBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return this.GetBuildsInternal(requestContext, count, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((component, timeRange, excludedDefinitionIds) => component.GetAllBuildsAsync(projectId, count, queryOrder, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), excludedDefinitionIds)), BuildStatus.All, queryOrder, timeFilter, propertyFilters);
    }

    public Task<IList<BuildData>> GetBuildsByDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      HashSet<int> definitionIdsHash = definitionIds.ToHashSet<int>();
      return this.GetBuildsInternal(requestContext, count, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((component, timeRange, excludedDefinitionIds) => component.GetBuildsByDefinitionsAsync(projectId, count, definitionIdsHash, queryOrder, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), excludedDefinitionIds)), BuildStatus.All, queryOrder, timeFilter, propertyFilters);
    }

    public Task<IList<BuildData>> GetQueuedBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return this.GetBuildsInternal(requestContext, count, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((component, timeRange, excludedDefinitionIds) => component.GetQueuedBuildsAsync(projectId, count, queryOrder, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), excludedDefinitionIds)), BuildStatus.NotStarted, queryOrder, timeFilter, propertyFilters);
    }

    public Task<IList<BuildData>> GetQueuedBuildsByDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      HashSet<int> definitionIdsHash = definitionIds.ToHashSet<int>();
      return this.GetBuildsInternal(requestContext, count, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((component, timeRange, excludedDefinitionIds) => component.GetQueuedBuildsByDefinitionsAsync(projectId, count, definitionIdsHash, queryOrder, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), excludedDefinitionIds)), BuildStatus.NotStarted, queryOrder, timeFilter, propertyFilters);
    }

    public Task<IList<BuildData>> GetRunningBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return this.GetBuildsInternal(requestContext, count, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((component, timeRange, excludedDefinitionIds) => component.GetRunningBuildsAsync(projectId, count, queryOrder, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), excludedDefinitionIds)), BuildStatus.InProgress, queryOrder, timeFilter, propertyFilters);
    }

    public Task<IList<BuildData>> GetRunningBuildsByDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      HashSet<int> definitionIdsHash = definitionIds.ToHashSet<int>();
      return this.GetBuildsInternal(requestContext, count, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((component, timeRange, excludedDefinitionIds) => component.GetRunningBuildsByDefinitionsAsync(projectId, count, definitionIdsHash, queryOrder, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), excludedDefinitionIds)), BuildStatus.InProgress, queryOrder, timeFilter, propertyFilters);
    }

    public Task<IList<BuildData>> GetCompletedBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      return this.GetBuildsInternal(requestContext, count, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((component, timeRange, excludedDefinitionIds) => component.GetCompletedBuildsAsync(projectId, count, queryOrder, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), excludedDefinitionIds)), BuildStatus.Completed, queryOrder, timeFilter, propertyFilters);
    }

    public Task<IList<BuildData>> GetCompletedBuildsByDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int count,
      IEnumerable<int> definitionIds,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      HashSet<int> definitionIdsHash = definitionIds.ToHashSet<int>();
      return this.GetBuildsInternal(requestContext, count, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((component, timeRange, excludedDefinitionIds) => component.GetCompletedBuildsByDefinitionsAsync(projectId, count, definitionIdsHash, queryOrder, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), excludedDefinitionIds)), BuildStatus.Completed, queryOrder, timeFilter, propertyFilters);
    }

    public async Task<BuildData> GetLatestCompletedBuildAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryIdentifier,
      string repositoryType,
      string branchName)
    {
      BuildData completedBuildAsync;
      using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
        completedBuildAsync = await bc.GetLatestCompletedBuildAsync(projectId, repositoryIdentifier, repositoryType, branchName);
      return completedBuildAsync;
    }

    public Task<IList<BuildData>> GetLatestBuildsUnderFolderAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string folderPath,
      DateTime? maxQueueTime,
      int count)
    {
      return this.GetBuildsInternal(requestContext, count, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((component, timeRange, excludedDefinitionIds) =>
      {
        Build2Component build2Component = component;
        Guid projectId1 = projectId;
        string folderPath1 = folderPath;
        int num = count;
        TimeFilter? timeFilter = new TimeFilter?(new TimeFilter()
        {
          MinTime = timeRange.MinTime,
          MaxTime = timeRange.MaxTime
        });
        int count1 = num;
        HashSet<int> excludedDefinitionIds1 = excludedDefinitionIds;
        return build2Component.GetAllBuildsUnderFolderAsync(projectId1, folderPath1, timeFilter, count1, excludedDefinitionIds1);
      }), BuildStatus.All);
    }

    private async Task<IList<BuildData>> GetBuildsInternal(
      IVssRequestContext requestContext,
      int count,
      Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>> componentCall,
      BuildStatus status,
      BuildQueryOrder queryOrder = BuildQueryOrder.Descending,
      TimeFilter? timeFilter = null,
      IEnumerable<string> propertyFilters = null)
    {
      IList<BuildData> list;
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildsInternal)))
      {
        IList<BuildData> buildDataList = (IList<BuildData>) null;
        BuildService.BuildTimeRange timeRange = new BuildService.BuildTimeRange(timeFilter);
        HashSet<int> definitionsToExclude = new HashSet<int>();
        Build2Component bc = requestContext.CreateComponent<Build2Component>();
        try
        {
          buildDataList = await componentCall(bc, timeRange, definitionsToExclude);
        }
        finally
        {
          bc?.Dispose();
        }
        bc = (Build2Component) null;
        Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder webApiQueryOrder = queryOrder.ToWebApiBuildQueryOrder(status);
        bool timeRangeUpdated;
        List<BuildData> allBuilds = this.FilterBuildsAndUpdateTimeRange(requestContext, (IEnumerable<BuildData>) buildDataList, timeRange, webApiQueryOrder, out timeRangeUpdated, ref definitionsToExclude);
        if (definitionsToExclude.Count > 0)
        {
          int excludedCount = definitionsToExclude.Count;
          while (buildDataList.Count > 0 && allBuilds.Count < count)
          {
            bool flag = excludedCount < definitionsToExclude.Count;
            excludedCount = definitionsToExclude.Count;
            if (timeRangeUpdated | flag)
            {
              requestContext.TraceInfo(12030056, "Service", "Making another call to prc_GetBuilds because definition security filtered some out.");
              bc = requestContext.CreateComponent<Build2Component>();
              try
              {
                buildDataList = await componentCall(bc, timeRange, definitionsToExclude);
              }
              finally
              {
                bc?.Dispose();
              }
              bc = (Build2Component) null;
              allBuilds.AddRange(this.FilterBuildsAndUpdateTimeRange(requestContext, (IEnumerable<BuildData>) buildDataList, timeRange, webApiQueryOrder, out timeRangeUpdated, ref definitionsToExclude).Where<BuildData>((Func<BuildData, bool>) (b => !allBuilds.Exists((Predicate<BuildData>) (x => b.Id == x.Id)))));
            }
            else
              break;
          }
        }
        this.UpdateBuildProperties(requestContext, allBuilds, propertyFilters);
        this.SortBuilds(allBuilds, webApiQueryOrder);
        list = (IList<BuildData>) allBuilds.Take<BuildData>(count).Select<BuildData, BuildData>((Func<BuildData, BuildData>) (x => x.UpdateReferences(requestContext))).ToList<BuildData>();
      }
      return list;
    }

    public Task<IList<BuildData>> FilterBuildsByBranchAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int? definitionId,
      string folderPath,
      IEnumerable<int> repositoryIds,
      IEnumerable<int> branchIds,
      DateTime? maxQueueTime,
      int maxCount)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      HashSet<int> uniqueRepositoryIds = repositoryIds.ToHashSet<int>();
      HashSet<int> uniqueBranchIds = branchIds.ToHashSet<int>();
      return this.GetBuildsInternal(requestContext, maxCount, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((bc, timeRange, excludedDefinitionIds) => bc.FilterBuildsByBranchAsync(projectId, definitionId, folderPath, uniqueRepositoryIds, uniqueBranchIds, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), maxCount, excludedDefinitionIds)), BuildStatus.All);
    }

    public Task<IList<BuildData>> FilterBuildsByTagsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int? definitionId,
      string folderPath,
      HashSet<string> tagsFilter,
      DateTime? maxQueueTime,
      int maxCount)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      return this.GetBuildsInternal(requestContext, maxCount, (Func<Build2Component, BuildService.BuildTimeRange, HashSet<int>, Task<IList<BuildData>>>) ((bc, timeRange, excludedDefinitionIds) => bc.FilterBuildsByTagsAsync(projectId, definitionId, folderPath, tagsFilter, new TimeFilter?(new TimeFilter()
      {
        MinTime = timeRange.MinTime,
        MaxTime = timeRange.MaxTime
      }), maxCount, excludedDefinitionIds)), BuildStatus.All);
    }

    public BuildResult? GetBranchStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName,
      string stageName = null,
      string jobName = null,
      string configuration = null)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      if (!string.IsNullOrWhiteSpace(stageName) || !string.IsNullOrWhiteSpace(jobName) || !string.IsNullOrWhiteSpace(configuration))
      {
        BuildData latestBuildForBranch = this.GetLatestBuildForBranch(requestContext, projectId, definitionId, repositoryId, repositoryType, branchName);
        if (latestBuildForBranch == null)
          return new BuildResult?();
        TimelineData? timelineData = latestBuildForBranch.GetTimelineData(requestContext);
        return !timelineData.HasValue ? new BuildResult?() : new BuildResult?(TimelineHelpers.GetTimelineResult((IList<Microsoft.TeamFoundation.Build.WebApi.TimelineRecord>) timelineData.Value.Timeline.Records, stageName, jobName, configuration));
      }
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBranchStatus)))
      {
        repositoryId = SourceProviderHelper.NormalizeRepositoryId(requestContext, repositoryType, repositoryId);
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          return component.GetBranchStatus(projectId, definitionId, repositoryId, repositoryType, branchName);
      }
    }

    public BuildData GetLatestBuildForBranch(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetLatestBuildForBranch)))
      {
        repositoryId = SourceProviderHelper.NormalizeRepositoryId(requestContext, repositoryType, repositoryId);
        BuildData latestBuildForBranch;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          latestBuildForBranch = component.GetLatestBuildForBranch(projectId, definitionId, repositoryId, repositoryType, branchName);
        if (latestBuildForBranch == null)
          return (BuildData) null;
        this.SecurityProvider.CheckDefinitionPermission(requestContext, projectId, latestBuildForBranch.Definition, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds);
        return latestBuildForBranch.UpdateReferences(requestContext);
      }
    }

    public BuildData GetLatestSuccessfulBuildForBranch(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName,
      DateTime maxFinishTime)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetLatestSuccessfulBuildForBranch)))
      {
        repositoryId = SourceProviderHelper.NormalizeRepositoryId(requestContext, repositoryType, repositoryId);
        branchName = GitRefspecHelper.NormalizeSourceBranch(branchName);
        BuildData successfulBuildForBranch;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          successfulBuildForBranch = component.GetLatestSuccessfulBuildForBranch(projectId, definitionId, repositoryId, repositoryType, branchName, new DateTime?(maxFinishTime));
        return successfulBuildForBranch.UpdateReferences(requestContext);
      }
    }

    public BuildData QueueBuild(
      IVssRequestContext requestContext,
      BuildData build,
      IEnumerable<IBuildRequestValidator> validators,
      BuildRequestValidationFlags validationFlags = BuildRequestValidationFlags.None,
      string checkInTicket = null,
      int? sourceBuildId = null,
      [CallerMemberName] string callingMethod = null,
      [CallerFilePath] string callingFile = null)
    {
      return this.QueueBuild(requestContext, build, validators, out string _, validationFlags, checkInTicket, sourceBuildId, callingMethod, callingFile);
    }

    public BuildData QueueBuild(
      IVssRequestContext requestContext,
      BuildData build,
      IEnumerable<IBuildRequestValidator> validators,
      out string finalYaml,
      BuildRequestValidationFlags validationFlags = BuildRequestValidationFlags.None,
      string checkInTicket = null,
      int? sourceBuildId = null,
      [CallerMemberName] string callingMethod = null,
      [CallerFilePath] string callingFile = null)
    {
      finalYaml = (string) null;
      ArgumentValidation.CheckBuild(build);
      BuildReason reason = build.Reason;
      if (validators == null)
        validators = Enumerable.Empty<IBuildRequestValidator>();
      if (build.OrchestrationPlan == null)
        build.OrchestrationPlan = new TaskOrchestrationPlanReference()
        {
          PlanId = Guid.NewGuid()
        };
      string orchestrationId1 = build.OrchestrationPlan.PlanId.ToString("D");
      using (requestContext.TraceScope(nameof (BuildService), nameof (QueueBuild)))
      {
        using (requestContext.CreateOrchestrationIdScope(orchestrationId1))
        {
          if (!requestContext.IsBuildFeatureEnabled())
          {
            requestContext.TraceInfo(0, nameof (BuildService), "Build Feature disabled: ignoring {0} QueueBuild request for definition {1}:{2}", (object) reason, (object) build.Definition?.Id, (object) build.Definition?.Name);
            return (BuildData) null;
          }
          OrchestrationTracer ciao = this.GetCIAO(requestContext);
          ciao.TraceStarted(orchestrationId1, "ciBuild");
          ciao.TracePhaseStarted(orchestrationId1, "ciBuild", nameof (QueueBuild));
          build.QueueTime = new DateTime?(DateTime.UtcNow);
          Microsoft.VisualStudio.Services.Identity.Identity requestedFor = requestContext.GetService<IdentityService>().GetIdentity(requestContext, build.RequestedFor);
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          if (userIdentity == null)
            requestContext.TraceError(0, "Service", "Requested By is null for build {0} in project {1}", (object) build.Id, (object) build.ProjectId);
          if (requestedFor == null)
            requestedFor = userIdentity;
          IBuildDefinitionService service1 = requestContext.GetService<IBuildDefinitionService>();
          BuildDefinition definition1 = service1.GetDefinition(requestContext.Elevate(), build.ProjectId, build.Definition.Id, build.Definition.Revision);
          if (definition1 == null)
          {
            ciao.TraceCompletedWithError(orchestrationId1, "ciBuild", nameof (QueueBuild), "DefinitionNotFoundException", BuildServerResources.DefinitionNotFound((object) build.Definition.Id), true);
            throw new DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) build.Definition.Id));
          }
          if (reason == BuildReason.CheckInShelveset && !string.IsNullOrEmpty(checkInTicket))
          {
            using (IDisposableReadOnlyList<IBuildCheckInTicketValidator> extensions = requestContext.GetExtensions<IBuildCheckInTicketValidator>())
            {
              if (extensions.Count == 0)
              {
                ciao.TraceCompletedWithError(orchestrationId1, "ciBuild", nameof (QueueBuild), "BuildException", BuildServerResources.GatedCheckInValidatorNotFound(), true);
                throw new BuildException(BuildServerResources.GatedCheckInValidatorNotFound());
              }
              foreach (IBuildCheckInTicketValidator inTicketValidator in (IEnumerable<IBuildCheckInTicketValidator>) extensions)
              {
                try
                {
                  inTicketValidator.ValidateCheckInTicket(requestContext, checkInTicket, (IReadOnlyBuildDefinition) definition1, (IReadOnlyBuildData) build);
                }
                catch (GatedCheckInTicketValidationException ex)
                {
                  ciao.TraceCompletedWithError(orchestrationId1, "ciBuild", nameof (QueueBuild), "GatedCheckInTicketValidationException", ex.Message, true);
                  throw;
                }
              }
            }
          }
          else
          {
            if (!string.IsNullOrEmpty(build.JustInTime.YamlOverride) && !this.SecurityProvider.HasDefinitionPermission(requestContext, definition1.ProjectId, (MinimalBuildDefinition) definition1, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.EditBuildDefinition))
            {
              ciao.TraceCompletedWithError(orchestrationId1, "ciBuild", nameof (QueueBuild), "BuildEventPermissionException", BuildServerResources.DryRunYamlOverrideNotPermitted((object) build.Definition.Id, (object) "EditBuild"), true);
              throw new BuildEventPermissionException(BuildServerResources.DryRunYamlOverrideNotPermitted((object) definition1.Id, (object) "EditBuild"));
            }
            if (!this.SecurityProvider.HasDefinitionPermission(requestContext, definition1.ProjectId, (MinimalBuildDefinition) definition1, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.QueueBuilds))
            {
              if (!this.SecurityProvider.HasDefinitionPermission(requestContext, definition1.ProjectId, (MinimalBuildDefinition) definition1, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuildDefinition))
              {
                ciao.TraceCompletedWithError(orchestrationId1, "ciBuild", nameof (QueueBuild), "DefinitionNotFoundException", BuildServerResources.DefinitionNotFound((object) build.Definition.Id), true);
                throw new DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) definition1.Id));
              }
              if (!build.JustInTime.PreviewRun)
              {
                try
                {
                  this.SecurityProvider.CheckDefinitionPermission(requestContext, definition1.ProjectId, (MinimalBuildDefinition) definition1, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.QueueBuilds);
                }
                catch (Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException ex)
                {
                  ciao.TraceCompletedWithError(orchestrationId1, "ciBuild", nameof (QueueBuild), "AccessDeniedException", "Lacking QueueBuild permission", true);
                  throw;
                }
              }
            }
          }
          if (definition1.QueueStatus == DefinitionQueueStatus.Disabled)
          {
            ciao.TraceCompletedWithError(orchestrationId1, "ciBuild", nameof (QueueBuild), "DefinitionDisabledException", BuildServerResources.DefinitionDisabled((object) definition1.Name, (object) definition1.ProjectName), true);
            throw new DefinitionDisabledException(BuildServerResources.DefinitionDisabled((object) definition1.Name, (object) definition1.ProjectName));
          }
          BuildProcess process1 = definition1.Process;
          if ((process1 != null ? (process1.Type == 3 ? 1 : 0) : 0) != 0 && !requestContext.IsFeatureEnabled("Build2.DockerProcess"))
          {
            ciao.TraceCompletedWithError(orchestrationId1, "ciBuild", nameof (QueueBuild), "DefinitionDisabledException", BuildServerResources.DefinitionDisabled((object) definition1.Name, (object) definition1.ProjectName), true);
            throw new DefinitionDisabledException(BuildServerResources.DefinitionDisabled((object) definition1.Name, (object) definition1.ProjectName));
          }
          bool flag1 = false;
          int? nullable1 = build.QueueId;
          if (!nullable1.HasValue)
          {
            if (requestContext.IsFeatureEnabled("Build2.OverrideDeprecatedDefaultQueues"))
            {
              BuildData buildData = build;
              IVssRequestContext requestContext1 = requestContext;
              Guid projectId = build.ProjectId;
              AgentPoolQueue queue = definition1.Queue;
              int? requestedQueueId;
              if (queue == null)
              {
                nullable1 = new int?();
                requestedQueueId = nullable1;
              }
              else
                requestedQueueId = new int?(queue.Id);
              ref bool local = ref flag1;
              int? nullable2 = this.MapDeprecatedQueues(requestContext1, projectId, requestedQueueId, out local);
              buildData.QueueId = nullable2;
            }
            else
            {
              BuildData buildData = build;
              AgentPoolQueue queue = definition1.Queue;
              int? nullable3;
              if (queue == null)
              {
                nullable1 = new int?();
                nullable3 = nullable1;
              }
              else
                nullable3 = new int?(queue.Id);
              buildData.QueueId = nullable3;
            }
          }
          string str;
          if (!build.Properties.TryGetValue<string>(TaskAgentRequestConstants.HostedAgentImageIdKey, out str))
          {
            definition1.PopulateProperties(requestContext, (IEnumerable<string>) new List<string>()
            {
              TaskAgentRequestConstants.HostedAgentImageIdKey
            });
            if (definition1.Properties.TryGetValue<string>(TaskAgentRequestConstants.HostedAgentImageIdKey, out str))
              build.Properties[TaskAgentRequestConstants.HostedAgentImageIdKey] = (object) str;
          }
          if (requestContext.IsFeatureEnabled("Build2.EditPipelineQueueConfigurationPermission"))
            new EditQueuePipelineVarSecurityValidator(this.SecurityProvider, ciao).ValidatePermissions(requestContext, build, definition1);
          bool flag2 = flag1 && requestContext.IsFeatureEnabled("Build2.BypassPermissionCheckWhenOverridingDefaultQueue");
          if (definition1.Queue != null)
          {
            nullable1 = build.QueueId;
            if (nullable1.HasValue)
            {
              nullable1 = build.QueueId;
              int id = definition1.Queue.Id;
              if (!(nullable1.GetValueOrDefault() == id & nullable1.HasValue) && !flag2)
              {
                IDistributedTaskPoolService service2 = requestContext.GetService<IDistributedTaskPoolService>();
                try
                {
                  IDistributedTaskPoolService distributedTaskPoolService = service2;
                  IVssRequestContext requestContext2 = requestContext;
                  Guid projectId = build.ProjectId;
                  nullable1 = build.QueueId;
                  int queueId = nullable1.Value;
                  distributedTaskPoolService.CheckUsePermissionForQueue(requestContext2, projectId, queueId);
                }
                catch (Exception ex)
                {
                  ciao.TraceCompletedWithError(orchestrationId1, "ciBuild", nameof (QueueBuild), ex.GetType().Name, ex.Message, true);
                  throw;
                }
              }
            }
          }
          IVssRequestContext elevatedContext = requestContext.Elevate();
          IBuildSourceProvider sourceProvider = (IBuildSourceProvider) null;
          BuildRepository repository1 = (BuildRepository) null;
          BuildProcess process2 = definition1.Process;
          string repository2;
          if ((process2 != null ? (process2.Type == 2 ? 1 : 0) : 0) != 0 && build.TriggerInfo.TryGetValue("pr.triggerRepository", out repository2))
          {
            OrchestrationTracer orchestrationTracer1 = ciao;
            string orchestrationId2 = orchestrationId1;
            object[] objArray1 = new object[2];
            BuildData buildData1 = build;
            int? nullable4;
            if (buildData1 == null)
            {
              nullable1 = new int?();
              nullable4 = nullable1;
            }
            else
            {
              // ISSUE: explicit non-virtual call
              nullable4 = new int?(__nonvirtual (buildData1.Id));
            }
            objArray1[0] = (object) nullable4;
            MinimalBuildDefinition definition2 = build.Definition;
            int? nullable5;
            if (definition2 == null)
            {
              nullable1 = new int?();
              nullable5 = nullable1;
            }
            else
            {
              // ISSUE: explicit non-virtual call
              nullable5 = new int?(__nonvirtual (definition2.Id));
            }
            objArray1[1] = (object) nullable5;
            orchestrationTracer1.Trace(orchestrationId2, 12030264, TraceLevel.Info, "Processing PR trigger for build with build id {0} and definition id {1}", objArray1);
            definition1.GetProcess<YamlProcess>();
            IBuildSourceProviderService service3 = elevatedContext.GetService<IBuildSourceProviderService>();
            string repositoryType;
            if (build.TriggerInfo.TryGetValue("pr.triggerRepository.Type", out repositoryType))
            {
              sourceProvider = service3.GetSourceProvider(elevatedContext, repositoryType);
              Guid result = Guid.Empty;
              string input;
              if (build.TriggerInfo.TryGetValue("pr.triggerRepository.endpointId", out input))
                Guid.TryParse(input, out result);
              SourceRepository userRepository = sourceProvider.GetUserRepository(requestContext, build.ProjectId, new Guid?(result), repository2);
              if (userRepository != null)
              {
                OrchestrationTracer orchestrationTracer2 = ciao;
                string orchestrationId3 = orchestrationId1;
                object[] objArray2 = new object[3]
                {
                  (object) userRepository.Name,
                  null,
                  null
                };
                BuildData buildData2 = build;
                int? nullable6;
                if (buildData2 == null)
                {
                  nullable1 = new int?();
                  nullable6 = nullable1;
                }
                else
                {
                  // ISSUE: explicit non-virtual call
                  nullable6 = new int?(__nonvirtual (buildData2.Id));
                }
                objArray2[1] = (object) nullable6;
                MinimalBuildDefinition definition3 = build.Definition;
                int? nullable7;
                if (definition3 == null)
                {
                  nullable1 = new int?();
                  nullable7 = nullable1;
                }
                else
                {
                  // ISSUE: explicit non-virtual call
                  nullable7 = new int?(__nonvirtual (definition3.Id));
                }
                objArray2[2] = (object) nullable7;
                orchestrationTracer2.Trace(orchestrationId3, 12030264, TraceLevel.Info, "Setting the build repository as {0} for build with build id {1} and definition id {2}", objArray2);
                BuildRepository buildRepository = new BuildRepository();
                buildRepository.Id = userRepository.Id;
                buildRepository.Name = userRepository.Name;
                buildRepository.Url = userRepository.Url;
                buildRepository.Type = repositoryType;
                buildRepository.Properties = userRepository.Properties;
                repository1 = buildRepository;
                build.Repository = new MinimalBuildRepository()
                {
                  Id = repository1.Id,
                  Type = repository1.Type
                };
              }
            }
            string latestSourceVersion = service3.GetSourceProvider(elevatedContext, definition1.Repository.Type).GetLatestSourceVersion(requestContext, definition1, definition1.Repository.DefaultBranch);
            definition1.Repository?.Properties?.Add("sourceVersion", latestSourceVersion);
          }
          else if (definition1.Repository != null)
          {
            repository1 = service1.GetExpandedRepository(elevatedContext, definition1);
            build.Repository = new MinimalBuildRepository()
            {
              Id = repository1.Id,
              Type = repository1.Type
            };
            sourceProvider = elevatedContext.GetService<IBuildSourceProviderService>().GetSourceProvider(elevatedContext, definition1.Repository.Type);
            if (string.IsNullOrEmpty(build.SourceBranch))
              build.SourceBranch = definition1.Repository.DefaultBranch;
          }
          if (definition1.QueueStatus == DefinitionQueueStatus.Paused)
          {
            OrchestrationTracer orchestrationTracer = ciao;
            string orchestrationId4 = orchestrationId1;
            object[] objArray = new object[2]
            {
              (object) build.Id,
              null
            };
            MinimalBuildDefinition definition4 = build.Definition;
            int? nullable8;
            if (definition4 == null)
            {
              nullable1 = new int?();
              nullable8 = nullable1;
            }
            else
            {
              // ISSUE: explicit non-virtual call
              nullable8 = new int?(__nonvirtual (definition4.Id));
            }
            objArray[1] = (object) nullable8;
            orchestrationTracer.Trace(orchestrationId4, 12030304, TraceLevel.Info, "Queuing build with build id {0} on paused definition id {1}", objArray);
            build.QueueOptions = QueueOptions.DoNotRun;
          }
          bool previewRun = build.JustInTime.PreviewRun;
          IBuildRequestHelper buildRequestHelper = this.BuildHelperFactory.GetBuildRequestHelper(elevatedContext, sourceProvider, definition1, repository1, build, userIdentity, requestedFor, validationFlags);
          IOrchestrationEnvironment environment;
          IOrchestrationProcess orchestration;
          using (IDisposableReadOnlyList<IBuildOption> extensions = requestContext.GetExtensions<IBuildOption>())
          {
            List<IBuildOption> list = extensions.OrderBy<IBuildOption, int>((Func<IBuildOption, int>) (feature => feature.GetOrdinal())).ToList<IBuildOption>();
            build = buildRequestHelper.QueueBuild(elevatedContext, (IList<IBuildOption>) list, validators, validationFlags, out environment, out orchestration, sourceBuildId);
          }
          finalYaml = buildRequestHelper.PipelineExpandedYaml;
          if (previewRun)
            return build;
          build = build.UpdateReferences(elevatedContext);
          if (build == null)
          {
            ciao.TraceCompletedWithError(orchestrationId1, "ciBuild", nameof (QueueBuild), "TriggeredBuilds", string.Format("Attempted to queue a build (reason: {0}) due to a call from method {1} of file {2}, but no build was queued.", (object) reason, (object) callingMethod, (object) callingFile), true);
            requestContext.TraceInfo(12030197, "TriggeredBuilds", "Attempted to queue a build (reason: {0}) due to a call from method {1} of file {2}, but no build was queued.", (object) reason, (object) callingMethod, (object) callingFile);
            return (BuildData) null;
          }
          BuildStatus? status = build.Status;
          BuildStatus buildStatus = BuildStatus.Completed;
          if (!(status.GetValueOrDefault() == buildStatus & status.HasValue))
          {
            BuildOptions buildOptions = new BuildOptions()
            {
              ResolveTaskInputAliases = definition1.Process.Type == 2,
              ValidateResources = true,
              ResolveResourceVersions = reason != BuildReason.CheckInShelveset,
              EnableResourceExpressions = definition1.Process.Type == 2
            };
            PlanTemplateType templateType = definition1.Process.Type == 1 ? PlanTemplateType.Designer : PlanTemplateType.Yaml;
            IDistributedTaskPoolService service4 = elevatedContext.GetService<IDistributedTaskPoolService>();
            AgentPoolQueue queue = buildRequestHelper.GetQueue(requestContext);
            int? nullable9 = queue != null ? new int?(queue.Id) : build.QueueId;
            IVssRequestContext requestContext3 = elevatedContext;
            Guid projectId = build.ProjectId;
            int queueId = nullable9.Value;
            TaskAgentQueue agentQueue = service4.GetAgentQueue(requestContext3, projectId, queueId);
            ciao.Trace(orchestrationId1, 12030264, TraceLevel.Info, "Queued build with build id {0} and definition id {1}", new object[2]
            {
              (object) build?.Id,
              (object) build.Definition?.Id
            });
            elevatedContext.GetService<IBuildOrchestrator>().RunPlan(elevatedContext, build.ProjectId, build, agentQueue.Pool, templateType, environment, orchestration, buildOptions, buildRequestHelper.PipelineInitializationLog, buildRequestHelper.PipelineExpandedYaml);
            if (requestContext.IsFeatureEnabled("Build2.RetainPipelineResources") && environment is PipelineEnvironment pipelineEnvironment)
            {
              int runId;
              int definitionId;
              Guid guid;
              foreach (IGrouping<Guid, (Guid, RetentionLease)> grouping in requestContext.GetService<IPipelineBuilderService>().GetResourceStore(requestContext, build.ProjectId, pipelineEnvironment.Resources).Pipelines.GetAll().Select<PipelineResource, (Guid, RetentionLease)>(closure_1 ?? (closure_1 = (Func<PipelineResource, (Guid, RetentionLease)>) (pipeline => pipeline.Properties.TryGetValue<int>(PipelinePropertyNames.PipelineId, out runId) && pipeline.Properties.TryGetValue<int>(PipelinePropertyNames.DefinitionId, out definitionId) && pipeline.Properties.TryGetValue<Guid>(PipelinePropertyNames.ProjectId, out guid) ? (guid, new RetentionLease(-1, RetentionLeaseHelper.GetOwnerIdForBuild(build), runId, definitionId, new DateTime(), DateTime.UtcNow + BuildServerConstants.InfiniteRetentionLease, false)) : (Guid.Empty, (RetentionLease) null)))).Where<(Guid, RetentionLease)>((Func<(Guid, RetentionLease), bool>) (tuple => tuple.ProjectId != Guid.Empty)).GroupBy<(Guid, RetentionLease), Guid>((Func<(Guid, RetentionLease), Guid>) (tuple => tuple.ProjectId)))
              {
                IGrouping<Guid, (Guid, RetentionLease)> leasesPerProject = grouping;
                elevatedContext.RunSynchronously<IReadOnlyList<RetentionLease>>((Func<Task<IReadOnlyList<RetentionLease>>>) (() => this.AddRetentionLeases(elevatedContext, leasesPerProject.Key, (IList<RetentionLease>) leasesPerProject.Select<(Guid, RetentionLease), RetentionLease>((Func<(Guid, RetentionLease), RetentionLease>) (tuple => tuple.Lease)).ToArray<RetentionLease>())));
              }
            }
            this.PublishBuildChangeEvent(requestContext, (IReadOnlyBuildData) build, BuildEventType.BuildQueuedEvent);
          }
          requestContext.TraceInfo(12030197, "TriggeredBuilds", "Queued Build {0} (reason: {1}) due to a call from method {2} of file {3}.", (object) build.Id, (object) reason, (object) callingMethod, (object) callingFile);
          if (build != null && build.ValidationResults.Any<BuildRequestValidationResult>())
          {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("Build {0} had validation issues when queued:", (object) build.Id));
            foreach (BuildRequestValidationResult validationResult in build.ValidationResults)
              stringBuilder.AppendLine(validationResult.Result.ToString() + ": " + validationResult.Message);
            requestContext.TraceInfo(12030227, "Service", stringBuilder.ToString());
          }
          return build;
        }
      }
    }

    public void CancelStage(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string stageRefName)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      IVssRequestContext requestContext1 = requestContext;
      List<int> buildIdsParam = new List<int>();
      buildIdsParam.Add(buildId);
      Guid? projectId1 = new Guid?(projectId);
      BuildData build = this.GetBuildsByIdsInternal(requestContext1, (IEnumerable<int>) buildIdsParam, (IEnumerable<string>) null, false, projectId1).FirstOrDefault<BuildData>();
      if (build == null)
        throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId));
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (!userIdentity.IsRequestor(build))
        this.SecurityProvider.CheckDefinitionPermission(requestContext, projectId, build.Definition, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.StopBuilds);
      int maxCancelTimeout = this.GetMaxCancelTimeout(requestContext, projectId, build, (TaskOrchestrationPlan) null, stageRefName);
      requestContext.GetService<IBuildOrchestrator>().CancelStage(requestContext, build.ProjectId, build.OrchestrationPlan.PlanId, stageRefName, TimeSpan.FromMinutes((double) maxCancelTimeout), BuildServerResources.BuildCanceledReason((object) userIdentity.DisplayName));
    }

    public async Task RetryStageAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string stageRefName,
      bool forceRetryAllJobs = false,
      bool retryDependencies = true)
    {
      BuildService buildService = this;
      using (requestContext.TraceScope(nameof (BuildService), nameof (RetryStageAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        BuildData buildByIdAsync1 = await buildService.GetBuildByIdAsync(requestContext, buildId);
        if (buildByIdAsync1 == null || buildByIdAsync1.ProjectId != projectId)
          throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId));
        if (!requestContext.GetUserIdentity().IsRequestor(buildByIdAsync1))
          buildService.SecurityProvider.CheckDefinitionPermission(requestContext, projectId, buildByIdAsync1.Definition, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.QueueBuilds);
        IPipelineRuntimeService service = requestContext.GetService<IPipelineRuntimeService>();
        IVssRequestContext requestContext1 = requestContext;
        Guid scopeIdentifier = projectId;
        Guid planId = buildByIdAsync1.OrchestrationPlan.PlanId;
        List<string> stageNames = new List<string>();
        stageNames.Add(stageRefName);
        int num1 = forceRetryAllJobs ? 1 : 0;
        int num2 = retryDependencies ? 1 : 0;
        IList<StageAttempt> stageAttemptList = await service.RetryStagesAsync(requestContext1, "Build", scopeIdentifier, planId, (IList<string>) stageNames, num1 != 0, num2 != 0);
        if (requestContext.IsFeatureEnabled("Build2.SendPipelineNotificationOnRetry"))
        {
          BuildData buildByIdAsync2 = await buildService.GetBuildByIdAsync(requestContext, buildId);
          buildService.PublishRunStateChangedNotification(requestContext, (IReadOnlyBuildData) buildByIdAsync2);
        }
      }
    }

    private int GetMaxCancelTimeout(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildData build,
      TaskOrchestrationPlan plan,
      string stageRefName)
    {
      int maxCancelTimeout = 0;
      if (plan == null)
        plan = this.GetPlan(requestContext, build);
      foreach (Stage stage in plan?.Process is PipelineProcess process ? process.Stages.Where<Stage>((Func<Stage, bool>) (x => string.IsNullOrEmpty(stageRefName) || x.Name == stageRefName)) : Enumerable.Empty<Stage>())
      {
        foreach (PhaseNode phase in (IEnumerable<PhaseNode>) stage.Phases)
        {
          int num = phase.Target == null || !(phase.Target.CancelTimeoutInMinutes != (ExpressionValue<int>) null) ? 0 : phase.Target.CancelTimeoutInMinutes.GetValue().Value;
          if (num > maxCancelTimeout)
            maxCancelTimeout = num;
        }
      }
      if (maxCancelTimeout == 0)
      {
        BuildDefinition definition = requestContext.GetService<IBuildDefinitionService>().GetDefinition(requestContext.Elevate(), projectId, build.Definition.Id);
        maxCancelTimeout = definition != null ? definition.JobCancelTimeoutInMinutes : 0;
      }
      return maxCancelTimeout;
    }

    internal void CancelBuildsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<BuildData> buildsToCancel,
      Guid? authorId)
    {
      int size = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/CancelOrphanedBuildsInQueueBatchSize", 500);
      foreach (IList<BuildData> builds in buildsToCancel.Buffer<BuildData>(size, true))
      {
        IList<BuildData> oldBuilds;
        IDictionary<int, BuildDefinition> definitionsById;
        IList<BuildData> buildDataList;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          buildDataList = (IList<BuildData>) component.UpdateBuilds(projectId, (IEnumerable<BuildData>) builds, authorId.Value, out oldBuilds, out definitionsById);
        List<DeleteBuildSpec> source = new List<DeleteBuildSpec>();
        Dictionary<int, BuildData> dictionary = oldBuilds.ToDictionary<BuildData, int>((Func<BuildData, int>) (b => b.Id));
        foreach (BuildData buildData in (IEnumerable<BuildData>) buildDataList)
        {
          BuildData build = buildData;
          try
          {
            BuildDefinition buildDefinition;
            int timeoutInMinutes = definitionsById.TryGetValue(buildData.Definition.Id, out buildDefinition) ? buildDefinition.JobCancelTimeoutInMinutes : 0;
            int num = timeoutInMinutes > 0 ? timeoutInMinutes : 1;
            requestContext.GetService<IBuildOrchestrator>().CancelPlan(requestContext, build.ProjectId, build.OrchestrationPlan.PlanId, TimeSpan.FromMinutes((double) num), BuildServerResources.BuildCanceledReason((object) authorId));
          }
          catch (Exception ex) when (ex is TaskOrchestrationPlanTerminatedException || ex is TaskOrchestrationPlanNotFoundException)
          {
            requestContext.TraceException(12030070, "Service", ex);
          }
          BuildService.SendRealtimeUpdatedNotification(requestContext, projectId, build);
          requestContext.TraceInfo(12030005, "Service", "Publishing BuildUpdatedEvent for build {0}", (object) build.Id);
          ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
          service.PublishNotification(requestContext, (object) new BuildUpdatedEvent((IReadOnlyBuildData) build, true));
          BuildData oldBuild = dictionary[build.Id];
          DateTime? nullable = oldBuild.StartTime;
          int num1;
          if (!nullable.HasValue)
          {
            nullable = build.StartTime;
            num1 = nullable.HasValue ? 1 : 0;
          }
          else
            num1 = 0;
          nullable = oldBuild.FinishTime;
          int num2;
          if (!nullable.HasValue)
          {
            nullable = build.FinishTime;
            num2 = nullable.HasValue ? 1 : 0;
          }
          else
            num2 = 0;
          bool flag = num2 != 0;
          if (num1 != 0)
          {
            requestContext.TraceInfo(12030001, "Service", "Publishing BuildStartedEvent for build {0}", (object) build.Id);
            Stopwatch stopwatch = Stopwatch.StartNew();
            service.SyncPublishNotification(requestContext, (object) new SyncBuildStartedEvent((IReadOnlyBuildData) build));
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            requestContext.TraceInfo(12030058, "Service", "Publishing SyncBuildStartedEvent for build {0} took {1} ms.", (object) build.Id, (object) elapsedMilliseconds);
            this.PublishBuildChangeEvent(requestContext, (IReadOnlyBuildData) build, BuildEventType.BuildStartedEvent, (IReadOnlyBuildData) oldBuild);
          }
          if (flag)
          {
            requestContext.TraceInfo(12030002, "Service", "Publishing BuildCompletedEvent for build {0}", (object) build.Id);
            Stopwatch stopwatch = Stopwatch.StartNew();
            service.SyncPublishNotification(requestContext, (object) new SyncBuildCompletedEvent((IReadOnlyBuildData) build));
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            requestContext.TraceInfo(12030058, "Service", "Publishing SyncBuildCompletedEvent for build {0} took {1} ms.", (object) build.Id, (object) elapsedMilliseconds);
            this.PublishBuildCompletedEvent(requestContext, (IReadOnlyBuildData) build);
            DeleteBuildSpec deleteBuildSpec = (DeleteBuildSpec) null;
            string str = build.Properties.GetValue<string>(BuildProperties.DeleteSpec, (string) null);
            if (!string.IsNullOrEmpty(str))
            {
              try
              {
                deleteBuildSpec = JsonConvert.DeserializeObject<DeleteBuildSpec>(str);
              }
              catch (Exception ex)
              {
                requestContext.TraceException("Service", ex);
              }
            }
            if (deleteBuildSpec != null)
              source.Add(deleteBuildSpec);
          }
        }
        if (source.Any<DeleteBuildSpec>())
          this.DeleteBuilds(requestContext, projectId, (IEnumerable<DeleteBuildSpec>) source);
      }
    }

    public async Task<BuildData> RetryBuildAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      BuildService buildService = this;
      BuildData buildData;
      using (requestContext.TraceScope(nameof (BuildService), nameof (RetryBuildAsync)))
      {
        BuildData buildByIdAsync1 = await buildService.GetBuildByIdAsync(requestContext, buildId);
        if (buildByIdAsync1 == null || buildByIdAsync1.ProjectId != projectId)
          throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId));
        string str = buildByIdAsync1.GetProjectName(requestContext) ?? buildByIdAsync1.ProjectId.ToString();
        if (buildByIdAsync1.Definition.QueueStatus == DefinitionQueueStatus.Disabled)
          throw new DefinitionDisabledException(BuildServerResources.DefinitionDisabled((object) buildByIdAsync1.Definition.Name, (object) str));
        if (buildByIdAsync1.Definition.QueueStatus == DefinitionQueueStatus.Paused)
          throw new InvalidBuildException(BuildServerResources.RetryNotSupportedForPausedDefinition((object) buildByIdAsync1.Definition.Name, (object) str));
        if (!requestContext.GetUserIdentity().IsRequestor(buildByIdAsync1))
          buildService.SecurityProvider.CheckDefinitionPermission(requestContext, buildByIdAsync1.Definition.ProjectId, buildByIdAsync1.Definition, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.QueueBuilds);
        try
        {
          IList<StageAttempt> stageAttemptList = await requestContext.GetService<IPipelineRuntimeService>().RetryPipelineAsync(requestContext, "Build", projectId, buildByIdAsync1.OrchestrationPlan.PlanId);
          BuildData buildByIdAsync2 = await buildService.GetBuildByIdAsync(requestContext, buildId);
          if (requestContext.IsFeatureEnabled("Build2.SendPipelineNotificationOnRetry"))
            buildService.PublishRunStateChangedNotification(requestContext, (IReadOnlyBuildData) buildByIdAsync2);
          buildData = buildByIdAsync2;
        }
        catch (InvalidPipelineOperationException ex)
        {
          throw new BuildStatusInvalidChangeException(ex.Message, (Exception) ex);
        }
      }
      return buildData;
    }

    public async Task<List<BuildData>> UpdateBuildsAsync(
      IVssRequestContext requestContext,
      List<BuildData> builds,
      bool publishSignalR = true,
      bool ignoreDeleteSpec = false)
    {
      ArgumentUtility.CheckForNull<List<BuildData>>(builds, nameof (builds));
      if (!builds.Any<BuildData>())
        return new List<BuildData>();
      Guid projectId = Guid.Empty;
      HashSet<int> distinctBuildIds = new HashSet<int>();
      foreach (BuildData build in builds)
      {
        ArgumentUtility.CheckForNull<BuildData>(build, "build");
        ArgumentUtility.CheckForEmptyGuid(build.ProjectId, "build.Project");
        if (build.ProjectId != Guid.Empty)
        {
          if (projectId == Guid.Empty)
            projectId = build.ProjectId;
          else if (build.ProjectId != projectId)
            throw new ProjectConflictException(BuildServerResources.UpdateBuildsMultipleProjectsNotSupported());
        }
        if (!distinctBuildIds.Add(build.Id))
          throw new DuplicateBuildSpecException(BuildServerResources.BuildSpecifiedMultipleTimes((object) build.Id));
        if (!string.IsNullOrEmpty(build.BuildNumber))
        {
          requestContext.Trace(12030242, TraceLevel.Info, "Build2", nameof (BuildService), "Build number for build with id {0} is being updated to {1} in project {2}", (object) build.Id, (object) build.BuildNumber, (object) build.ProjectId);
          if (!ArgumentValidation.IsValidBuildNumber(build.BuildNumber))
            throw new BuildNumberFormatException(BuildServerResources.BuildNumberFormatInvalidCharacters((object) build.BuildNumber));
        }
      }
      using (requestContext.TraceScope(nameof (BuildService), nameof (UpdateBuildsAsync)))
      {
        List<BuildData> list1 = (await this.GetBuildsByIdsAsync(requestContext, projectId, (IEnumerable<int>) distinctBuildIds)).ToList<BuildData>();
        if (list1.Count != distinctBuildIds.Count)
          throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) distinctBuildIds.Except<int>(list1.Select<BuildData, int>((Func<BuildData, int>) (b => b.Id))).FirstOrDefault<int>()));
        if (requestContext.IsFeatureEnabled("Build2.DisableBranchUpdatesForUserRequests") && requestContext.IsUserContext)
          this.checkIfSourceBranchChanged(list1, builds);
        HashSet<Guid> source1 = new HashSet<Guid>(list1.Select<BuildData, Guid>((Func<BuildData, Guid>) (b => b.ProjectId)));
        if (source1.Count > 1)
          throw new ProjectConflictException(BuildServerResources.UpdateBuildsMultipleProjectsNotSupported());
        if (projectId == Guid.Empty)
          projectId = source1.Single<Guid>();
        else if (projectId != source1.Single<Guid>())
          throw new ProjectConflictException(BuildServerResources.UpdateBuildsMultipleProjectsNotSupported());
        Microsoft.VisualStudio.Services.Identity.Identity requestedBy = requestContext.GetUserIdentity();
        Dictionary<int, BuildData> incomingBuildUpdatesByIds = builds.ToDictionary<BuildData, int>((Func<BuildData, int>) (b => b.Id));
        List<(int, int)> source2 = new List<(int, int)>();
        List<(int, int)> keepForeverRemovals = new List<(int, int)>();
        List<(int, int, bool)> retainedByReleaseChanges = new List<(int, int, bool)>();
        foreach (BuildData build in list1)
        {
          BuildData buildData1 = incomingBuildUpdatesByIds[build.Id];
          BuildStatus? status;
          bool? nullable;
          if (!requestedBy.IsRequestor(build))
          {
            status = build.Status;
            BuildStatus buildStatus1 = BuildStatus.Cancelling;
            if (!(status.GetValueOrDefault() == buildStatus1 & status.HasValue))
            {
              status = buildData1.Status;
              BuildStatus buildStatus2 = BuildStatus.Cancelling;
              if (status.GetValueOrDefault() == buildStatus2 & status.HasValue)
              {
                this.SecurityProvider.CheckBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.StopBuilds);
                goto label_51;
              }
            }
            bool flag = false;
            nullable = buildData1.LegacyInputKeepForever;
            if (nullable.HasValue)
            {
              if (!this.SecurityProvider.HasBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation))
                this.SecurityProvider.CheckBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.RetainIndefinitely);
              nullable = buildData1.LegacyInputKeepForever;
              if (nullable.Value)
                source2.Add((build.Id, build.Definition.Id));
              else
                keepForeverRemovals.Add((build.Id, build.Definition.Id));
              flag = true;
            }
            nullable = buildData1.LegacyInputRetainedByRelease;
            if (nullable.HasValue)
            {
              if (!this.SecurityProvider.HasBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation))
                this.SecurityProvider.CheckBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.RetainIndefinitely);
              List<(int, int, bool)> valueTupleList = retainedByReleaseChanges;
              int id1 = build.Id;
              int id2 = build.Definition.Id;
              nullable = buildData1.LegacyInputRetainedByRelease;
              int num = nullable.Value ? 1 : 0;
              (int, int, bool) valueTuple = (id1, id2, num != 0);
              valueTupleList.Add(valueTuple);
              flag = true;
            }
            if (!flag)
              this.SecurityProvider.CheckBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation);
          }
          else
          {
            nullable = buildData1.LegacyInputKeepForever;
            bool flag = true;
            if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
              source2.Add((build.Id, build.Definition.Id));
            nullable = buildData1.LegacyInputRetainedByRelease;
            if (nullable.HasValue)
            {
              List<(int, int, bool)> valueTupleList = retainedByReleaseChanges;
              int id3 = build.Id;
              int id4 = build.Definition.Id;
              nullable = buildData1.LegacyInputRetainedByRelease;
              int num = nullable.Value ? 1 : 0;
              (int, int, bool) valueTuple = (id3, id4, num != 0);
              valueTupleList.Add(valueTuple);
            }
          }
label_51:
          if (build.TriggeredByBuild == null && buildData1.TriggeredByBuild != null)
            throw new CannotUpdateTriggeredByBuildException(BuildServerResources.CannotUpdateTriggeredByBuildException());
          if (build.TriggeredByBuild != null && (buildData1 == null || buildData1.TriggeredByBuild != null) && buildData1 != null)
          {
            if (!(build.TriggeredByBuild.ProjectId != buildData1.TriggeredByBuild.ProjectId) && build.TriggeredByBuild.DefinitionId == buildData1.TriggeredByBuild.DefinitionId)
            {
              int? definitionVersion1 = build.TriggeredByBuild.DefinitionVersion;
              int? definitionVersion2 = buildData1.TriggeredByBuild.DefinitionVersion;
              if (definitionVersion1.GetValueOrDefault() == definitionVersion2.GetValueOrDefault() & definitionVersion1.HasValue == definitionVersion2.HasValue && build.TriggeredByBuild.BuildId == buildData1.TriggeredByBuild.BuildId)
                goto label_57;
            }
            throw new CannotUpdateTriggeredByBuildException(BuildServerResources.CannotUpdateTriggeredByBuildException());
          }
label_57:
          if (!string.IsNullOrEmpty(buildData1.SourceVersion) && !buildData1.Properties.ContainsKey(BuildProperties.DeleteSpec))
          {
            BuildData buildData2 = buildData1;
            build.SourceVersion = buildData2.SourceVersion;
            try
            {
              buildData2.SourceVersionInfo = this.GenerateSourceVersionInfo(requestContext, (IReadOnlyBuildData) build);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(12030199, "Service", ex);
            }
          }
          status = build.Status;
          BuildStatus buildStatus3 = BuildStatus.Cancelling;
          if (status.GetValueOrDefault() == buildStatus3 & status.HasValue)
          {
            status = buildData1.Status;
            BuildStatus buildStatus4 = BuildStatus.Cancelling;
            string reason;
            if (status.GetValueOrDefault() == buildStatus4 & status.HasValue && this.ShouldForceCancelBuild(requestContext, projectId, build, out reason))
            {
              buildData1.Result = new BuildResult?(BuildResult.Canceled);
              buildData1.Status = new BuildStatus?(BuildStatus.Completed);
              requestContext.TraceAlways(12030245, TraceLevel.Info, "Build2", nameof (BuildService), "Build {0} in project {1} was force cancelled because {2} by {3}", (object) buildData1.Id, (object) projectId, (object) reason, (object) requestedBy.DisplayName);
            }
          }
        }
        List<RetentionLease> list2 = source2.Select<(int, int), RetentionLease>((Func<(int, int), RetentionLease>) (tuple => new RetentionLease(-1, RetentionLeaseHelper.GetOwnerIdForUser(requestedBy.Id), tuple.buildId, tuple.definitionId, new DateTime(), DateTime.UtcNow + BuildServerConstants.InfiniteRetentionLease, false))).Concat<RetentionLease>(retainedByReleaseChanges.Where<(int, int, bool)>((Func<(int, int, bool), bool>) (tuple => tuple.retainedByRelease)).Select<(int, int, bool), RetentionLease>((Func<(int, int, bool), RetentionLease>) (tuple => new RetentionLease(-1, "RM", tuple.buildId, tuple.definitionId, new DateTime(), DateTime.UtcNow + BuildServerConstants.InfiniteRetentionLease, true)))).ToList<RetentionLease>();
        if (list2.Count > 0)
        {
          IReadOnlyList<RetentionLease> retentionLeaseList = await this.AddRetentionLeases(requestContext, projectId, (IList<RetentionLease>) list2);
        }
        int[] array1 = (await this.GetRetentionLeasesForRuns(requestContext, projectId, keepForeverRemovals.Select<(int, int), int>((Func<(int, int), int>) (t => t.buildId)))).Where<RetentionLease>((Func<RetentionLease, bool>) (lease => lease.OwnerId.StartsWith("User"))).Select<RetentionLease, int>((Func<RetentionLease, int>) (lease => lease.Id)).ToArray<int>();
        if (array1.Length != 0)
          await this.RemoveRetentionLeases(requestContext, projectId, (IReadOnlyList<int>) array1);
        MinimalRetentionLease[] array2 = retainedByReleaseChanges.Where<(int, int, bool)>((Func<(int, int, bool), bool>) (tuple => !tuple.retainedByRelease)).Select<(int, int, bool), MinimalRetentionLease>((Func<(int, int, bool), MinimalRetentionLease>) (tuple => new MinimalRetentionLease("RM", new int?(tuple.buildId), new int?(tuple.definitionId)))).ToArray<MinimalRetentionLease>();
        if (array2.Length != 0)
          await this.RemoveRetentionLeases(requestContext, projectId, (IReadOnlyList<MinimalRetentionLease>) array2);
        Build2Component bc = requestContext.CreateComponent<Build2Component>();
        IList<BuildData> updatedBuildsData;
        IDictionary<int, BuildDefinition> definitionsById;
        IList<BuildData> oldBuilds;
        try
        {
          UpdateBuildsResult updateBuildsResult = await bc.UpdateBuildsAsync(projectId, (IEnumerable<BuildData>) builds, requestedBy.Id);
          oldBuilds = updateBuildsResult.OldBuilds;
          updatedBuildsData = updateBuildsResult.NewBuilds;
          definitionsById = updateBuildsResult.DefinitionsById;
          requestContext.Trace(12030242, TraceLevel.Info, "Build2", nameof (BuildService), "Builds successfully updated!");
        }
        finally
        {
          bc?.Dispose();
        }
        bc = (Build2Component) null;
        if (updatedBuildsData == null)
        {
          requestContext.TraceError(0, "Service", "Updated builds are null.");
          updatedBuildsData = (IList<BuildData>) Array.Empty<BuildData>();
        }
        List<ArtifactPropertyValue> artifactPropertyValueList = new List<ArtifactPropertyValue>();
        foreach (BuildData buildData in (IEnumerable<BuildData>) updatedBuildsData)
        {
          buildData.Properties = incomingBuildUpdatesByIds[buildData.Id].Properties;
          if (buildData.Properties != null && buildData.Properties.Count > 0)
            artifactPropertyValueList.Add(new ArtifactPropertyValue(buildData.CreateArtifactSpec(requestContext), buildData.Properties.Convert()));
        }
        if (artifactPropertyValueList.Count > 0)
          requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueList);
        Dictionary<int, BuildData> oldBuildsByIds = oldBuilds.ToDictionary<BuildData, int>((Func<BuildData, int>) (b => b.Id));
        for (int index1 = 0; index1 < updatedBuildsData.Count; ++index1)
        {
          BuildData build = updatedBuildsData[index1];
          BuildData buildData = oldBuildsByIds[build.Id];
          BuildStatus? status = build.Status;
          BuildStatus buildStatus5 = BuildStatus.Cancelling;
          if (status.GetValueOrDefault() == buildStatus5 & status.HasValue)
          {
            status = buildData.Status;
            BuildStatus buildStatus6 = BuildStatus.Cancelling;
            if (!(status.GetValueOrDefault() == buildStatus6 & status.HasValue))
            {
              bool flag = false;
              try
              {
                BuildDefinition buildDefinition;
                int timeoutInMinutes = definitionsById.TryGetValue(build.Definition.Id, out buildDefinition) ? buildDefinition.JobCancelTimeoutInMinutes : 0;
                int num = timeoutInMinutes > 0 ? timeoutInMinutes : 1;
                requestContext.GetService<IBuildOrchestrator>().CancelPlan(requestContext, build.ProjectId, build.OrchestrationPlan.PlanId, TimeSpan.FromMinutes((double) num), BuildServerResources.BuildCanceledReason((object) requestedBy.DisplayName));
              }
              catch (Exception ex) when (ex is TaskOrchestrationPlanTerminatedException || ex is TaskOrchestrationPlanNotFoundException)
              {
                flag = true;
                requestContext.TraceException(12030070, "Service", ex);
              }
              finally
              {
                if (flag)
                {
                  build.Status = new BuildStatus?(BuildStatus.Completed);
                  build.Result = new BuildResult?(BuildResult.Canceled);
                  bc = requestContext.CreateComponent<Build2Component>();
                  try
                  {
                    IList<BuildData> buildDataList = updatedBuildsData;
                    int index2 = index1;
                    buildDataList[index2] = (await bc.UpdateBuildAsync(build, requestedBy.Id)).NewBuilds.Single<BuildData>();
                    buildDataList = (IList<BuildData>) null;
                  }
                  finally
                  {
                    bc?.Dispose();
                  }
                  bc = (Build2Component) null;
                }
              }
            }
          }
        }
        foreach (BuildData build in (IEnumerable<BuildData>) updatedBuildsData)
        {
          build.UpdateReferences(requestContext);
          build.PopulateProperties(requestContext);
        }
        if (string.IsNullOrWhiteSpace(definitionsById.Values.FirstOrDefault<BuildDefinition>((Func<BuildDefinition, bool>) (d => !string.IsNullOrWhiteSpace(d.ProjectName)))?.ProjectName))
          requestContext.GetService<IProjectService>().TryGetProjectName(requestContext, projectId, out string _);
        List<DeleteBuildSpec> source3 = new List<DeleteBuildSpec>();
        foreach (BuildData build in (IEnumerable<BuildData>) updatedBuildsData)
        {
          if (publishSignalR)
            BuildService.SendRealtimeUpdatedNotification(requestContext, projectId, build);
          ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
          requestContext.TraceInfo(12030005, "Service", "Publishing BuildUpdatedEvent for build {0}", (object) build.Id);
          service.PublishNotification(requestContext, (object) new BuildUpdatedEvent((IReadOnlyBuildData) build, publishSignalR));
          BuildData oldBuild = oldBuildsByIds[build.Id];
          int num = oldBuild.StartTime.HasValue ? 0 : (build.StartTime.HasValue ? 1 : 0);
          bool flag = !oldBuild.FinishTime.HasValue && build.FinishTime.HasValue;
          if (num != 0)
          {
            requestContext.TraceInfo(12030001, "Service", "Publishing BuildStartedEvent for build {0}", (object) build.Id);
            Stopwatch stopwatch = Stopwatch.StartNew();
            service.SyncPublishNotification(requestContext, (object) new SyncBuildStartedEvent((IReadOnlyBuildData) build));
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            requestContext.TraceInfo(12030058, "Service", "Publishing SyncBuildStartedEvent for build {0} took {1} ms.", (object) build.Id, (object) elapsedMilliseconds);
            this.PublishBuildChangeEvent(requestContext, (IReadOnlyBuildData) build, BuildEventType.BuildStartedEvent, (IReadOnlyBuildData) oldBuild);
          }
          if (flag)
          {
            requestContext.TraceInfo(12030002, "Service", "Publishing BuildCompletedEvent for build {0}", (object) build.Id);
            this.PublishBuildCompletedEvent(requestContext, (IReadOnlyBuildData) build);
            Stopwatch stopwatch = Stopwatch.StartNew();
            service.SyncPublishNotification(requestContext, (object) new SyncBuildCompletedEvent((IReadOnlyBuildData) build));
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            requestContext.TraceInfo(12030058, "Service", "Publishing SyncBuildCompletedEvent for build {0} took {1} ms.", (object) build.Id, (object) elapsedMilliseconds);
            if (!ignoreDeleteSpec)
            {
              DeleteBuildSpec deleteBuildSpec = (DeleteBuildSpec) null;
              string str = build.Properties.GetValue<string>(BuildProperties.DeleteSpec, (string) null);
              if (!string.IsNullOrEmpty(str))
              {
                try
                {
                  deleteBuildSpec = JsonConvert.DeserializeObject<DeleteBuildSpec>(str);
                }
                catch (Exception ex)
                {
                  requestContext.TraceException("Service", ex);
                }
              }
              if (deleteBuildSpec != null)
                source3.Add(deleteBuildSpec);
            }
          }
        }
        if (source3.Any<DeleteBuildSpec>())
        {
          int num1 = await this.DeleteBuildsAsync(requestContext, projectId, (IEnumerable<DeleteBuildSpec>) source3, requestedBy, false);
        }
        return updatedBuildsData.ToList<BuildData>();
      }
    }

    private void checkIfSourceBranchChanged(
      List<BuildData> existingBuilds,
      List<BuildData> currentBuilds)
    {
      Dictionary<int, string> dictionary = existingBuilds.ToDictionary<BuildData, int, string>((Func<BuildData, int>) (buildData => buildData.Id), (Func<BuildData, string>) (buildData => buildData.SourceBranch));
      foreach (BuildData currentBuild in currentBuilds)
      {
        if (currentBuild.SourceBranch != null && dictionary.GetValueOrDefault<int, string>(currentBuild.Id, (string) null) != currentBuild.SourceBranch)
          throw new BuildException(BuildServerResources.ImmutableSourceBranch());
      }
    }

    private TaskOrchestrationPlan GetPlan(IVssRequestContext requestContext, BuildData build) => requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").GetPlan(requestContext, build.ProjectId, build.OrchestrationPlan.PlanId);

    public PlanConcurrency GetPlanConcurrency(IVssRequestContext requestContext, BuildData build)
    {
      TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");
      TaskOrchestrationPlan plan = this.GetPlan(requestContext, build);
      return plan != null && plan.State == TaskOrchestrationPlanState.Throttled ? requestContext.GetService<IPlanThrottleService>().GetPlanConcurrency(requestContext, taskHub.Name, plan.ScopeIdentifier, plan.Definition) : (PlanConcurrency) null;
    }

    private bool ShouldForceCancelBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildData build,
      out string reason)
    {
      reason = "";
      if (build.OrchestrationPlan == null)
      {
        reason = "BuildData.OrchestrationPlan is null";
        return true;
      }
      Guid planId = build.OrchestrationPlan.PlanId;
      TaskOrchestrationPlan plan = this.GetPlan(requestContext, build);
      if (plan == null)
      {
        reason = "TaskOrchestrationPlan does not exist";
        return true;
      }
      if (plan.State == TaskOrchestrationPlanState.Completed)
      {
        reason = "TaskOrchestrationPlan is completed";
        return true;
      }
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int maxCancelTimeout = this.GetMaxCancelTimeout(requestContext, projectId, build, plan, (string) null);
      if (maxCancelTimeout == 0)
        maxCancelTimeout = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/BuildJobCancellingTimeout", true, maxCancelTimeout);
      DateTime dateTime = DateTime.Now;
      DateTime universalTime1 = dateTime.ToUniversalTime();
      dateTime = build.LastChangedDate;
      DateTime universalTime2 = dateTime.ToUniversalTime();
      if ((universalTime1 - universalTime2).TotalMinutes <= (double) maxCancelTimeout)
        return false;
      reason = "Build status hung in cancelling state";
      return true;
    }

    public BuildArtifact AddArtifact(
      IVssRequestContext requestContext,
      BuildData build,
      BuildArtifact artifact)
    {
      ArgumentUtility.CheckForNull<BuildData>(build, nameof (build));
      ArgumentValidation.CheckBuildArtifact(artifact);
      using (requestContext.TraceScope(nameof (BuildService), nameof (AddArtifact)))
      {
        this.SecurityProvider.CheckBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation);
        IArtifactProvider artifactProvider;
        if (artifact.Resource != null && !string.IsNullOrEmpty(artifact.Resource.Type) && requestContext.GetService<IBuildArtifactProviderService>().TryGetArtifactProvider(requestContext, artifact.Resource.Type, out artifactProvider))
          artifactProvider.AddArtifactReference(requestContext, build.ProjectId, build.Id, artifact);
        BuildArtifact artifact1;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          artifact1 = component.AddArtifact(build.ProjectId, build.Id, artifact);
        requestContext.GetService<IBuildDispatcher>().SendArtifactAdded(requestContext, build.Id, artifact1.Name);
        if (requestContext.IsFeatureEnabled("DistributedTask.EnableArtifactTraceability"))
          BuildArtifactTraceabilityHelper.TracePublishedArtifact(requestContext, build, artifact1);
        return artifact1;
      }
    }

    public IList<BuildArtifact> GetArtifacts(
      IVssRequestContext requestContext,
      BuildData build,
      string artifactName = null)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetArtifacts)))
      {
        ArgumentUtility.CheckForNull<BuildData>(build, nameof (build));
        this.SecurityProvider.CheckBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds);
        List<BuildArtifact> artifacts = (List<BuildArtifact>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          artifacts = component.GetArtifacts(build.ProjectId, build.Id, artifactName).ToList<BuildArtifact>();
        return (IList<BuildArtifact>) artifacts;
      }
    }

    public async Task<IList<BuildArtifact>> GetArtifactsBySourceAsync(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      string source)
    {
      IList<BuildArtifact> artifactsBySourceAsync;
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetArtifactsBySourceAsync)))
      {
        ArgumentUtility.CheckForNull<IReadOnlyBuildData>(build, nameof (build));
        this.SecurityProvider.CheckBuildPermission(requestContext, build.ProjectId, build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds);
        IList<BuildArtifact> buildArtifactList = (IList<BuildArtifact>) null;
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          buildArtifactList = await bc.GetArtifactsBySourceAsync(build.ProjectId, build.Id, source);
        artifactsBySourceAsync = buildArtifactList;
      }
      return artifactsBySourceAsync;
    }

    public GetChangesResult GetChanges(
      IVssRequestContext requestContext,
      int buildId,
      bool includeSourceChange = false,
      int startId = 0,
      int maxChanges = 50,
      Guid? projectId = null)
    {
      GetChangesResult changes = (GetChangesResult) null;
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetChanges)))
      {
        BuildData build = this.GetBuildsByIdsInternal(requestContext, (IEnumerable<int>) new int[1]
        {
          buildId
        }, (IEnumerable<string>) null, false, projectId).FirstOrDefault<BuildData>();
        if (build == null)
          throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId));
        if (build.Repository == null || maxChanges < 1)
          return new GetChangesResult();
        using (requestContext.CITimer("GetChangesElapsedMilliseconds"))
          changes = !build.ChangesCalculated ? this.CalculateChanges(requestContext, build, includeSourceChange, startId, maxChanges) : this.ReadChanges(requestContext, build, includeSourceChange, startId, maxChanges);
        this.NormalizeBuildChanges(requestContext, build.ProjectId, build, (IEnumerable<Change>) changes.Changes);
        return changes;
      }
    }

    public GetChangesResult GetChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      bool includeSourceChange = false,
      int startId = 0,
      int maxChanges = 50)
    {
      GetChangesResult changes = (GetChangesResult) null;
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetChanges)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        BuildData build = this.GetBuildsByIdsInternal(requestContext, (IEnumerable<int>) new int[1]
        {
          buildId
        }, (IEnumerable<string>) null, false, new Guid?(projectId)).FirstOrDefault<BuildData>();
        if (build == null)
          throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId));
        if (build.Repository == null || maxChanges < 1)
          return new GetChangesResult();
        using (requestContext.CITimer("GetChangesElapsedMilliseconds"))
          changes = !build.ChangesCalculated ? this.CalculateChanges(requestContext, build, includeSourceChange, startId, maxChanges) : this.ReadChanges(requestContext, build, includeSourceChange, startId, maxChanges);
        this.NormalizeBuildChanges(requestContext, build.ProjectId, build, (IEnumerable<Change>) changes.Changes);
        return changes;
      }
    }

    public async Task<int> DeleteBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<DeleteBuildSpec> deleteBuildSpecs,
      Microsoft.VisualStudio.Services.Identity.Identity deletedBy,
      bool ignoreLowPriorityLeases)
    {
      requestContext.AssertAsyncExecutionEnabled();
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      Dictionary<int, DeleteBuildSpec> buildsToDelete = new Dictionary<int, DeleteBuildSpec>();
      foreach (DeleteBuildSpec deleteBuildSpec in deleteBuildSpecs)
      {
        if (deleteBuildSpec.DeletedReason == null)
          deleteBuildSpec.DeletedReason = BuildServerResources.BuildDeletedReasonManual();
        if (!buildsToDelete.TryAdd<int, DeleteBuildSpec>(deleteBuildSpec.BuildId, deleteBuildSpec))
          throw new DuplicateBuildSpecException(BuildServerResources.DuplicateDeleteBuildSpecs((object) deleteBuildSpec.BuildId));
      }
      List<int> list1 = buildsToDelete.Keys.ToList<int>();
      using (requestContext.TraceScope(nameof (BuildService), nameof (DeleteBuildsAsync)))
      {
        Microsoft.VisualStudio.Services.Identity.Identity requestedBy = deletedBy ?? requestContext.GetUserIdentity();
        if (requestedBy == null)
          return 0;
        IEnumerable<BuildData> buildsByIdsAsync = await this.GetBuildsByIdsAsync(requestContext, (IEnumerable<int>) list1);
        List<BuildData> source1 = new List<BuildData>();
        foreach (BuildData build in buildsByIdsAsync)
        {
          this.SecurityProvider.CheckBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.DeleteBuilds);
          if (build.RetentionLeases.Any<RetentionLease>((Func<RetentionLease, bool>) (l => l.HighPriority || !ignoreLowPriorityLeases)))
            source1.Add(build);
        }
        if (source1.Count > 0)
          throw new Microsoft.TeamFoundation.Build.WebApi.CannotDeleteRetainedBuildException(BuildServerResources.CannotDeleteRetainedBuild((object) source1[0].BuildNumber), (IReadOnlyList<int>) source1.Select<BuildData, int>((Func<BuildData, int>) (b => b.Id)).ToArray<int>());
        List<BuildData> list2 = buildsByIdsAsync.Where<BuildData>((Func<BuildData, bool>) (b =>
        {
          BuildStatus? status = b.Status;
          BuildStatus buildStatus = BuildStatus.Completed;
          return !(status.GetValueOrDefault() == buildStatus & status.HasValue);
        })).ToList<BuildData>();
        if (list2.Any<BuildData>())
        {
          foreach (BuildData buildData in list2)
          {
            DeleteBuildSpec deleteBuildSpec = buildsToDelete[buildData.Id];
            buildData.Status = new BuildStatus?(BuildStatus.Cancelling);
            buildData.Properties[BuildProperties.DeleteSpec] = (object) JsonConvert.SerializeObject((object) deleteBuildSpec);
          }
          foreach (int key in new HashSet<int>((await this.UpdateBuildsAsync(requestContext, list2, false, true)).Where<BuildData>((Func<BuildData, bool>) (rb =>
          {
            BuildStatus? status = rb.Status;
            BuildStatus buildStatus = BuildStatus.Completed;
            return !(status.GetValueOrDefault() == buildStatus & status.HasValue);
          })).Select<BuildData, int>((Func<BuildData, int>) (rb => rb.Id))))
            buildsToDelete.Remove(key);
        }
        if (buildsToDelete.Count <= 0)
          return 0;
        List<DeleteBuildsResult> deleteResults = new List<DeleteBuildsResult>();
        foreach (IGrouping<\u003C\u003Ef__AnonymousType12<bool, string>, DeleteBuildSpec> source2 in buildsToDelete.Values.GroupBy(spec => new
        {
          DeleteBuildRecord = spec.DeleteBuildRecord,
          DeletedReason = spec.DeletedReason
        }))
        {
          using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          {
            List<DeleteBuildsResult> deleteBuildsResultList = deleteResults;
            deleteBuildsResultList.Add(await bc.DeleteBuildsAsync(projectId, source2.Select<DeleteBuildSpec, int>((Func<DeleteBuildSpec, int>) (spec => spec.BuildId)), requestedBy.Id, source2.Key.DeleteBuildRecord, source2.Key.DeletedReason));
            deleteBuildsResultList = (List<DeleteBuildsResult>) null;
          }
        }
        BuildData[] deletedBuilds = deleteResults.SelectMany<DeleteBuildsResult, BuildData>((Func<DeleteBuildsResult, IEnumerable<BuildData>>) (dr => (IEnumerable<BuildData>) dr.DeletedBuilds)).ToArray<BuildData>();
        ILookup<int, BuildArtifact> deletedArtifacts = deleteResults.SelectMany<DeleteBuildsResult, BuildArtifact>((Func<DeleteBuildsResult, IEnumerable<BuildArtifact>>) (dr => (IEnumerable<BuildArtifact>) dr.DeletedArtifacts)).ToLookup<BuildArtifact, int>((Func<BuildArtifact, int>) (a => a.BuildId));
        Dictionary<int, BuildDefinition> dictionary = (await requestContext.GetService<IBuildDefinitionService>().GetDefinitionsByIdsAsync(requestContext, projectId, ((IEnumerable<BuildData>) deletedBuilds).Select<BuildData, int>((Func<BuildData, int>) (b => b.Definition.Id)).Distinct<int>().ToList<int>(), true)).ToDictionary<BuildDefinition, int>((Func<BuildDefinition, int>) (x => x.Id));
        this.CleanUpDeletedBuilds(requestContext, projectId, (IReadOnlyCollection<BuildData>) deletedBuilds);
        BuildData[] updatedBuilds = ((IEnumerable<BuildData>) deletedBuilds).Select<BuildData, BuildData>((Func<BuildData, BuildData>) (b => b.UpdateReferences(requestContext))).ToArray<BuildData>();
        if (updatedBuilds.Length != 0)
        {
          await this.ScheduleDeleteBuildArtifactJobs(requestContext, buildsToDelete, requestedBy, (IEnumerable<BuildData>) updatedBuilds, deletedArtifacts, dictionary);
          this.NotifyBuildsDeleted(requestContext, projectId, (IEnumerable<BuildData>) updatedBuilds);
        }
        return deletedBuilds.Length;
      }
    }

    public int DestroyBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      DateTime maxDeletedTime,
      int maxBuilds)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      IEnumerable<BuildData> source;
      using (requestContext.TraceScope(nameof (BuildService), nameof (DestroyBuilds)))
      {
        BuildDefinition definition = (BuildDefinition) null;
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          definition = requestContext.RunSynchronously<BuildDefinition>((Func<Task<BuildDefinition>>) (() => bc.GetDefinitionAsync(projectId, definitionId, new int?(), true)));
        if (definition == null)
        {
          requestContext.TraceInfo(0, "Service", "Definition not found {0}", (object) definitionId);
          return 0;
        }
        this.SecurityProvider.CheckDefinitionPermission(requestContext, projectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.DestroyBuilds);
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          source = component.DestroyBuilds(projectId, definitionId, maxDeletedTime, maxBuilds);
      }
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      foreach (BuildData build in source)
      {
        requestContext.TraceInfo(12030017, "Service", "Publishing BuildDestroyedEvent for build {0}", (object) build.Id);
        service.PublishNotification(requestContext, (object) new BuildDestroyedEvent((IReadOnlyBuildData) build));
      }
      return source.Count<BuildData>();
    }

    public List<BuildForRetention> GetBuildsForRetention(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      DateTime minFinishTime,
      DateTime maxFinishTime,
      int count)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildsForRetention)))
      {
        List<BuildForRetention> buildsForRetention;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          buildsForRetention = component.GetBuildsForRetention(projectId, definitionId, minFinishTime, maxFinishTime, count);
        buildsForRetention.Sort((Comparison<BuildForRetention>) ((build1, build2) => build2.FinishTime.CompareTo(build1.FinishTime)));
        return buildsForRetention;
      }
    }

    public IEnumerable<string> AddTags(
      IVssRequestContext requestContext,
      BuildData build,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForNull<BuildData>(build, nameof (build));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tags, nameof (tags));
      using (requestContext.TraceScope(nameof (BuildService), nameof (AddTags)))
      {
        this.SecurityProvider.CheckBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.EditBuildQuality);
        List<string> stringList = (List<string>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          stringList = component.AddBuildTags(build.ProjectId, build.Id, tags).ToList<string>();
        build.Tags.Clear();
        build.Tags.AddRange((IEnumerable<string>) stringList);
        requestContext.GetService<IBuildDispatcher>().SendTagsAdded(requestContext, build.Id);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        requestContext.TraceInfo(12030010, "Service", "Publishing BuildTagsAddedEvent for build {0}", (object) build.Id);
        service.PublishNotification(requestContext, (object) new BuildTagsAddedEvent((IReadOnlyBuildData) build, (IEnumerable<string>) stringList));
        return (IEnumerable<string>) stringList;
      }
    }

    public IEnumerable<string> DeleteTags(
      IVssRequestContext requestContext,
      BuildData build,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForNull<BuildData>(build, nameof (build));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tags, nameof (tags));
      using (requestContext.TraceScope(nameof (BuildService), nameof (DeleteTags)))
      {
        this.SecurityProvider.CheckBuildPermission(requestContext, build.ProjectId, (IReadOnlyBuildData) build, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.EditBuildQuality);
        List<string> collection = (List<string>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          collection = component.DeleteBuildTags(build.ProjectId, build.Id, tags).ToList<string>();
        build.Tags.Clear();
        build.Tags.AddRange((IEnumerable<string>) collection);
        build.PopulateProperties(requestContext);
        BuildService.SendRealtimeUpdatedNotification(requestContext, build.ProjectId, build);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        requestContext.TraceInfo(12030011, "Service", "Publishing BuildUpdatedEvent (deleted tags) for build {0}", (object) build.Id);
        service.PublishNotification(requestContext, (object) new BuildUpdatedEvent((IReadOnlyBuildData) build, true));
        return (IEnumerable<string>) collection;
      }
    }

    public async Task<int> PurgeArtifactsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int daysOld,
      int batchSize)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      ArgumentUtility.CheckForNonPositiveInt(daysOld, nameof (daysOld), "Build2");
      int count;
      using (requestContext.TraceScope(nameof (BuildService), nameof (PurgeArtifactsAsync)))
      {
        PurgedBuildsResults result;
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          result = await bc.PurgeArtifactsAsync(projectId, daysOld, batchSize);
        await this.ScheduleDeleteBuildArtifactJobsAsync(requestContext, projectId, result);
        count = result.PurgedBuilds.Count;
      }
      return count;
    }

    public async Task<int> PurgeBuildsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int daysOld,
      int batchSize,
      string branchPrefix)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      ArgumentUtility.CheckForNonPositiveInt(daysOld, nameof (daysOld), "Build2");
      int count;
      using (requestContext.TraceScope(nameof (BuildService), nameof (PurgeBuildsAsync)))
      {
        PurgedBuildsResults result;
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          result = await bc.PurgeBuildsAsync(projectId, daysOld, batchSize, branchPrefix);
        this.CleanUpDeletedBuilds(requestContext, projectId, result.PurgedBuilds);
        await this.ScheduleDeleteBuildArtifactJobsAsync(requestContext, projectId, result);
        this.NotifyBuildsDeleted(requestContext, projectId, (IEnumerable<BuildData>) result.PurgedBuilds);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        foreach (BuildData purgedBuild in (IEnumerable<BuildData>) result.PurgedBuilds)
        {
          requestContext.TraceInfo(12030017, "Service", "Publishing BuildDestroyedEvent for build {0}", (object) purgedBuild.Id);
          service.PublishNotification(requestContext, (object) new BuildDestroyedEvent((IReadOnlyBuildData) purgedBuild));
        }
        count = result.PurgedBuilds.Count;
      }
      return count;
    }

    private void CleanUpDeletedBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyCollection<BuildData> deletedBuilds)
    {
      IList<Guid> guidList = (IList<Guid>) new List<Guid>();
      if (requestContext.IsFeatureEnabled("Build2.Service.CheckSuitesRetention"))
        guidList = this.GetCheckpointRecordIds(requestContext, projectId, deletedBuilds);
      IVssRequestContext elevatedRequestContext = requestContext.Elevate();
      IBuildOrchestrator service1 = requestContext.GetService<IBuildOrchestrator>();
      IPipelineBuilderService service2 = requestContext.GetService<IPipelineBuilderService>();
      foreach (BuildData buildData in deletedBuilds.Where<BuildData>((Func<BuildData, bool>) (db => db.OrchestrationPlan != null)))
      {
        BuildData build = buildData;
        TaskOrchestrationPlan plan = service1.GetPlan(elevatedRequestContext, projectId, build.OrchestrationPlan.PlanId);
        if (plan != null)
        {
          if (plan.ProcessEnvironment is PipelineEnvironment processEnvironment)
          {
            IResourceStore resourceStore = service2.GetResourceStore(elevatedRequestContext, build.ProjectId, processEnvironment.Resources);
            if (resourceStore != null)
            {
              int num1;
              int num2;
              Guid guid;
              foreach (IGrouping<Guid, (Guid, MinimalRetentionLease)> grouping in resourceStore.Pipelines.GetAll().Select<PipelineResource, (Guid, MinimalRetentionLease)>((Func<PipelineResource, (Guid, MinimalRetentionLease)>) (pipeline => pipeline.Properties.TryGetValue<int>(PipelinePropertyNames.PipelineId, out num1) && pipeline.Properties.TryGetValue<int>(PipelinePropertyNames.DefinitionId, out num2) && pipeline.Properties.TryGetValue<Guid>(PipelinePropertyNames.ProjectId, out guid) ? (guid, new MinimalRetentionLease(RetentionLeaseHelper.GetOwnerIdForBuild(build), new int?(num1), new int?(num2))) : (Guid.Empty, (MinimalRetentionLease) null))).Where<(Guid, MinimalRetentionLease)>((Func<(Guid, MinimalRetentionLease), bool>) (tuple => tuple.ProjectId != Guid.Empty)).GroupBy<(Guid, MinimalRetentionLease), Guid>((Func<(Guid, MinimalRetentionLease), Guid>) (tuple => tuple.ProjectId)))
              {
                IGrouping<Guid, (Guid, MinimalRetentionLease)> leasesPerProject = grouping;
                elevatedRequestContext.RunSynchronously((Func<Task>) (() => this.RemoveRetentionLeases(elevatedRequestContext, leasesPerProject.Key, (IReadOnlyList<MinimalRetentionLease>) leasesPerProject.Select<(Guid, MinimalRetentionLease), MinimalRetentionLease>((Func<(Guid, MinimalRetentionLease), MinimalRetentionLease>) (tuple => tuple.Lease)).ToArray<MinimalRetentionLease>())));
              }
            }
            else
              requestContext.TraceAlways(0, TraceLevel.Info, "Build2", nameof (CleanUpDeletedBuilds), "Did not find resource store for plan with planId {0} for build {1}", (object) build.OrchestrationPlan.PlanId, (object) build.Id);
          }
        }
        else
          requestContext.TraceAlways(0, TraceLevel.Info, "Build2", nameof (CleanUpDeletedBuilds), "Did not find plan with planId {0} for build {1}", (object) build.OrchestrationPlan.PlanId, (object) build.Id);
      }
      service1.DeletePlans(elevatedRequestContext, projectId, (IEnumerable<Guid>) deletedBuilds.Where<BuildData>((Func<BuildData, bool>) (d => d.OrchestrationPlan != null)).Select<BuildData, Guid>((Func<BuildData, Guid>) (b => b.OrchestrationPlan.PlanId)).ToList<Guid>());
      ITeamFoundationFileContainerService service3 = requestContext.GetService<ITeamFoundationFileContainerService>();
      List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> source = service3.QueryContainers(requestContext, (IList<Uri>) deletedBuilds.Select<BuildData, Uri>((Func<BuildData, Uri>) (db => db.Uri)).ToList<Uri>(), projectId);
      try
      {
        service3.DeleteContainers(requestContext, (IList<long>) source.Select<Microsoft.VisualStudio.Services.FileContainer.FileContainer, long>((Func<Microsoft.VisualStudio.Services.FileContainer.FileContainer, long>) (c => c.Id)).ToList<long>(), projectId);
      }
      catch (FileContainerException ex)
      {
        requestContext.TraceException(0, "Service", (Exception) ex);
      }
      IBuildStatusCallbackService service4 = requestContext.GetService<IBuildStatusCallbackService>();
      try
      {
        service4.Delete(requestContext.Elevate(), (IEnumerable<IReadOnlyBuildData>) deletedBuilds);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Service", ex);
      }
      if (!guidList.Any<Guid>())
        return;
      this.PublishDeleteCheckSuitesEvent(requestContext, projectId, guidList);
    }

    private void NotifyBuildsDeleted(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<BuildData> updatedBuilds)
    {
      IEnumerable<IGrouping<int, BuildData>> groupings = updatedBuilds.GroupBy<BuildData, int>((Func<BuildData, int>) (b => b.Definition.Id));
      using (IDisposableReadOnlyList<IBuildsDeletedHandler> extensions = requestContext.GetExtensions<IBuildsDeletedHandler>())
      {
        foreach (IBuildsDeletedHandler buildsDeletedHandler in (IEnumerable<IBuildsDeletedHandler>) extensions)
        {
          foreach (IGrouping<int, BuildData> source in groupings)
          {
            try
            {
              buildsDeletedHandler.HandleBuildsDeleted(requestContext, projectId, source.Key, (IReadOnlyList<BuildData>) source.ToList<BuildData>());
            }
            catch (Exception ex)
            {
              requestContext.TraceException(nameof (BuildService), ex);
            }
          }
        }
      }
    }

    private static void SendRealtimeUpdatedNotification(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildData build)
    {
      requestContext.GetService<IBuildDataCacheService>().Set(requestContext, build);
      requestContext.GetService<IBuildDispatcher>().SendBuildUpdated(requestContext, projectId, build.Definition.Id, build.Id, build.Definition.Path);
    }

    public IEnumerable<Change> GetChangesBetweenBuilds(
      IVssRequestContext requestContext,
      int fromBuildId,
      int toBuildId,
      int maxChanges,
      Guid? projectId = null)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetChangesBetweenBuilds)))
      {
        BuildData fromBuild;
        BuildData toBuild;
        this.GetBuildsByIds(requestContext, projectId, fromBuildId, toBuildId, out fromBuild, out toBuild);
        if (fromBuild.Repository == null || toBuild.Repository == null || fromBuild.Repository.Id != toBuild.Repository.Id)
          return Enumerable.Empty<Change>();
        if (fromBuild.Id == toBuild.Id)
          return (IEnumerable<Change>) this.GetChanges(requestContext, toBuild.Id, false, 0, maxChanges, projectId).Changes;
        IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, fromBuild.Repository.Type, false);
        if (sourceProvider == null)
          return Enumerable.Empty<Change>();
        IEnumerable<Change> changesBetweenBuilds = sourceProvider.GetChangesBetweenBuilds(requestContext, fromBuild, toBuild, maxChanges);
        this.NormalizeBuildChanges(requestContext, fromBuild.ProjectId, fromBuild, changesBetweenBuilds);
        return changesBetweenBuilds;
      }
    }

    public IEnumerable<Change> GetChangesBetweenBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      int fromBuildId,
      int toBuildId,
      int maxChanges)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetChangesBetweenBuilds)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        return this.GetChangesBetweenBuilds(requestContext, fromBuildId, toBuildId, maxChanges, new Guid?(projectId));
      }
    }

    public IList<ResourceRef> GetBuildWorkItemRefs(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      IEnumerable<string> commitIds,
      int maxItems)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildWorkItemRefs)))
        return this.GetBuildWorkItemRefs(requestContext, buildId, commitIds, maxItems, new Guid?(projectId));
    }

    public BuildWorkItemRefsResult GetIndirectBuildWorkItemRefs(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      IEnumerable<string> commitIds,
      int maxItems,
      IEnumerable<int> excludedIds = null)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetIndirectBuildWorkItemRefs)))
      {
        CommitWorkItemRefsResult workItemRefsResult = (CommitWorkItemRefsResult) null;
        IEnumerable<int> workItemIds = Enumerable.Empty<int>();
        if (build.Repository != null && !this.TryGetWorkItemIds(requestContext, build, maxItems, out workItemIds))
        {
          IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, build.Repository.Type, false);
          if (sourceProvider != null)
          {
            workItemRefsResult = this.GetCommitsWorkItemRefs(requestContext, build, commitIds, maxItems, sourceProvider);
            workItemIds = workItemRefsResult.WorkItemIds;
          }
        }
        if (excludedIds != null && workItemIds != null)
          workItemIds = workItemIds.Except<int>(excludedIds);
        List<ResourceRef> resourceRefList = new List<ResourceRef>(WorkItemHelpers.ToWorkItemResourceRefs(requestContext, (IEnumerable<int>) new HashSet<int>(workItemIds), build.ToSecuredObject()));
        requestContext.AddCIEntry("IndirectWorkItemCount", (object) resourceRefList.Count);
        return new BuildWorkItemRefsResult()
        {
          ChangesTruncated = workItemRefsResult != null && workItemRefsResult.CommitsTruncated,
          WorkItemResourceRefs = (IReadOnlyCollection<ResourceRef>) resourceRefList
        };
      }
    }

    public IList<ResourceRef> GetBuildWorkItemRefs(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      IEnumerable<string> commitIds,
      int maxItems)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildWorkItemRefs)))
      {
        IEnumerable<int> ints = (IEnumerable<int>) null;
        using (requestContext.CITimer("GetDirectWorkItemsElapsedMilliseconds"))
          ints = (IEnumerable<int>) WorkItemHelpers.QueryWorkItemIds(requestContext, build.ProjectId, (IEnumerable<Uri>) new Uri[1]
          {
            build.Uri
          }, maxItems);
        List<ResourceRef> buildWorkItemRefs = new List<ResourceRef>(WorkItemHelpers.ToWorkItemResourceRefs(requestContext, ints, build.ToSecuredObject()));
        if (buildWorkItemRefs.Count < maxItems)
          buildWorkItemRefs.AddRange((IEnumerable<ResourceRef>) this.GetIndirectBuildWorkItemRefs(requestContext, build, commitIds, maxItems - buildWorkItemRefs.Count, ints).WorkItemResourceRefs);
        requestContext.AddCIEntry("WorkItemCount", (object) buildWorkItemRefs.Count);
        return (IList<ResourceRef>) buildWorkItemRefs;
      }
    }

    public IList<ResourceRef> GetWorkItemsBetweenBuilds(
      IVssRequestContext requestContext,
      int fromBuildId,
      int toBuildId,
      IEnumerable<string> commitIds,
      int maxItems,
      Guid? projectId = null)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetWorkItemsBetweenBuilds)))
      {
        if (fromBuildId == toBuildId)
          return this.GetBuildWorkItemRefs(requestContext, toBuildId, commitIds, maxItems, projectId);
        BuildData fromBuild = (BuildData) null;
        BuildData toBuild = (BuildData) null;
        this.GetBuildsByIds(requestContext, projectId, fromBuildId, toBuildId, out fromBuild, out toBuild);
        if (fromBuild.ProjectId != toBuild.ProjectId)
        {
          requestContext.AddCIEntry("WorkItemCount", (object) 0);
          return (IList<ResourceRef>) Array.Empty<ResourceRef>();
        }
        if (fromBuild.Repository == null || toBuild.Repository == null || fromBuild.Repository.Id != toBuild.Repository.Id)
        {
          requestContext.AddCIEntry("WorkItemCount", (object) 0);
          return (IList<ResourceRef>) Array.Empty<ResourceRef>();
        }
        IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, fromBuild.Repository.Type, false);
        if (sourceProvider == null)
        {
          requestContext.AddCIEntry("WorkItemCount", (object) 0);
          return (IList<ResourceRef>) Array.Empty<ResourceRef>();
        }
        if (commitIds == null || !commitIds.Any<string>())
        {
          int buildsForWorkItems = WITHelper.GetMaxItemsBetweenBuildsForWorkItems(requestContext);
          using (requestContext.CITimer("GetChangesElapsedMilliseconds"))
          {
            IEnumerable<Change> changesBetweenBuilds = sourceProvider.GetChangesBetweenBuilds(requestContext, fromBuild, toBuild, buildsForWorkItems + 1);
            commitIds = (IEnumerable<string>) changesBetweenBuilds.Take<Change>(buildsForWorkItems).Select<Change, string>((Func<Change, string>) (change => change.Id)).ToList<string>();
            requestContext.AddCIEntry("ExceededMaxItemsForWorkItemLimit", (object) (changesBetweenBuilds.Count<Change>() > buildsForWorkItems));
            requestContext.AddCIEntry("ItemsBetweenBuildsForWorkItemCount", (object) commitIds.Count<string>());
          }
        }
        IEnumerable<Uri> commitUris = sourceProvider.GetCommitUris(requestContext, (IReadOnlyBuildData) toBuild, commitIds);
        IEnumerable<int> workItemIds = (IEnumerable<int>) null;
        using (requestContext.CITimer("GetIndirectWorkItemsElapsedMilliseconds"))
          workItemIds = (IEnumerable<int>) WorkItemHelpers.QueryWorkItemIds(requestContext, toBuild.ProjectId, commitUris, maxItems);
        List<ResourceRef> list = WorkItemHelpers.ToWorkItemResourceRefs(requestContext, workItemIds, toBuild.ToSecuredObject()).Take<ResourceRef>(maxItems).ToList<ResourceRef>();
        requestContext.AddCIEntry("WorkItemCount", (object) list.Count);
        return (IList<ResourceRef>) list;
      }
    }

    public IList<ResourceRef> GetWorkItemsBetweenBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      int fromBuildId,
      int toBuildId,
      IEnumerable<string> commitIds,
      int maxItems)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetWorkItemsBetweenBuilds)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        return this.GetWorkItemsBetweenBuilds(requestContext, fromBuildId, toBuildId, commitIds, maxItems, new Guid?(projectId));
      }
    }

    private IList<ResourceRef> GetBuildWorkItemRefs(
      IVssRequestContext requestContext,
      int buildId,
      IEnumerable<string> commitIds,
      int maxItems,
      Guid? projectId)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildWorkItemRefs)))
      {
        BuildData buildData;
        if (!projectId.HasValue)
          buildData = this.GetBuildsByIds(requestContext, (IEnumerable<int>) new int[1]
          {
            buildId
          }, (IEnumerable<string>) null, false).FirstOrDefault<BuildData>();
        else
          buildData = this.GetBuildsByIds(requestContext, projectId.Value, (IEnumerable<int>) new int[1]
          {
            buildId
          }, (IEnumerable<string>) null, false).FirstOrDefault<BuildData>();
        return this.GetBuildWorkItemRefs(requestContext, (IReadOnlyBuildData) (buildData ?? throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId))), commitIds, maxItems);
      }
    }

    internal TaskOrchestrationPlanReference AttachOrchestrationToBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      int orchestrationType)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (AttachOrchestrationToBuild)))
      {
        if (orchestrationType == 1)
          throw new OrchestrationTypeNotSupportedException(BuildServerResources.OrchestrationTypeNotSupported((object) 1));
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        {
          BuildOrchestrationData orchestrationData = component.AddOrchestration(projectId, buildId, orchestrationType);
          if (orchestrationData != null)
            return orchestrationData.Plan;
        }
        return (TaskOrchestrationPlanReference) null;
      }
    }

    public PropertiesCollection UpdateProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      PropertiesCollection properties)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (UpdateProperties)))
      {
        BuildData buildById = this.GetBuildById(requestContext, projectId, buildId, includeDeleted: true);
        if (buildById == null)
          throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId));
        this.SecurityProvider.CheckBuildPermission(requestContext, projectId, (IReadOnlyBuildData) buildById, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation);
        ArtifactSpec artifactSpec = buildById.CreateArtifactSpec(requestContext);
        return requestContext.GetService<ITeamFoundationPropertyService>().UpdateProperties(requestContext, artifactSpec, properties);
      }
    }

    internal BuildData AddBuild(
      IVssRequestContext requestContext,
      BuildData build,
      Microsoft.VisualStudio.Services.Identity.Identity requestedBy,
      Microsoft.VisualStudio.Services.Identity.Identity requestedFor,
      IEnumerable<ChangeData> changeData)
    {
      ArgumentValidation.CheckBuild(build);
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(requestedBy, nameof (requestedBy));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(requestedFor, nameof (requestedFor));
      using (requestContext.TraceScope(nameof (BuildService), nameof (AddBuild)))
      {
        try
        {
          build.SourceVersionInfo = this.GenerateSourceVersionInfo(requestContext, (IReadOnlyBuildData) build);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12030199, "Service", ex);
        }
        build.QueueTime = new DateTime?(DateTime.UtcNow);
        BuildData build1;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          build1 = component.AddBuild(build, requestedBy.Id, requestedFor.Id, changeData != null, changeData);
        if (build1 == null)
        {
          requestContext.TraceInfo(12030081, "Service", "Call to AddBuild for definition {0} with reason {1}, source version {2} returned null.", (object) build.Definition.Id, (object) build.Reason, (object) build.SourceVersion);
          return (BuildData) null;
        }
        build1.Properties = build.Properties;
        if (build1.Properties != null && build1.Properties.Count > 0)
          requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, build1.CreateArtifactSpec(requestContext), build1.Properties.Convert());
        BuildStatus? status = build1.Status;
        BuildStatus buildStatus = BuildStatus.Completed;
        if (status.GetValueOrDefault() == buildStatus & status.HasValue)
        {
          this.GetChanges(requestContext, build1.ProjectId, build1.Id, false, 0, int.MaxValue);
          this.PublishBuildCompletedEvent(requestContext, (IReadOnlyBuildData) build1);
        }
        return build1;
      }
    }

    public long CreateBuildContainer(IVssRequestContext requestContext, BuildData build)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (CreateBuildContainer)))
      {
        ITeamFoundationFileContainerService service = requestContext.GetService<ITeamFoundationFileContainerService>();
        Uri buildUri = UriHelper.CreateBuildUri(build.Id);
        if (build.Uri == (Uri) null)
          build.Uri = buildUri;
        string token = build.GetToken();
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BUILD_{0}", (object) build.Id);
        IVssRequestContext requestContext1 = requestContext;
        Uri artifactUri = buildUri;
        string securityToken = token;
        string name = str;
        string empty = string.Empty;
        Guid projectId = build.ProjectId;
        long container = service.CreateContainer(requestContext1, artifactUri, securityToken, name, empty, projectId);
        requestContext.TraceVerbose(0, "Service", "Created container {0} for build {1}", (object) container, (object) buildUri);
        return container;
      }
    }

    private bool TryGetWorkItemIds(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      int maxItems,
      out IEnumerable<int> workItemIds)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (TryGetWorkItemIds)))
      {
        if (build.Repository != null)
          return requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, build.Repository.Type).TryGetWorkItemIds(requestContext, build, maxItems, out workItemIds);
      }
      workItemIds = (IEnumerable<int>) null;
      return false;
    }

    private CommitWorkItemRefsResult GetCommitsWorkItemRefs(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      IEnumerable<string> commitIds,
      int maxItems,
      IBuildSourceProvider sourceProvider)
    {
      bool flag = false;
      if (commitIds == null || !commitIds.Any<string>())
      {
        int buildsForWorkItems = WITHelper.GetMaxItemsBetweenBuildsForWorkItems(requestContext);
        GetChangesResult changes = this.GetChanges(requestContext, build.ProjectId, build.Id, false, 0, buildsForWorkItems);
        commitIds = (IEnumerable<string>) changes.Changes.Select<Change, string>((Func<Change, string>) (change => change.Id)).ToList<string>();
        flag = changes.ChangesTruncated;
        requestContext.AddCIEntry("ExceededMaxItemsForWorkItemLimit", (object) flag);
        requestContext.AddCIEntry("ItemsBetweenBuildsForWorkItemCount", (object) commitIds.Count<string>());
      }
      IEnumerable<Uri> commitUris = sourceProvider.GetCommitUris(requestContext, build, commitIds);
      using (requestContext.CITimer("GetIndirectWorkItemsElapsedMilliseconds"))
        return new CommitWorkItemRefsResult()
        {
          CommitsTruncated = flag,
          WorkItemIds = (IEnumerable<int>) WorkItemHelpers.QueryWorkItemIds(requestContext, build.ProjectId, commitUris, maxItems)
        };
    }

    private void SortBuilds(List<BuildData> builds, Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder)
    {
      switch (buildQueryOrder)
      {
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending:
          builds.Sort((Comparison<BuildData>) ((b1, b2) =>
          {
            DateTime? finishTime = b1.FinishTime;
            DateTime t1 = finishTime ?? DateTime.MaxValue;
            finishTime = b2.FinishTime;
            DateTime t2 = finishTime ?? DateTime.MaxValue;
            return DateTime.Compare(t1, t2);
          }));
          break;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending:
          builds.Sort((Comparison<BuildData>) ((b1, b2) =>
          {
            DateTime? finishTime = b2.FinishTime;
            DateTime t1 = finishTime ?? DateTime.MaxValue;
            finishTime = b1.FinishTime;
            DateTime t2 = finishTime ?? DateTime.MaxValue;
            return DateTime.Compare(t1, t2);
          }));
          break;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeDescending:
          builds.Sort((Comparison<BuildData>) ((b1, b2) =>
          {
            DateTime? queueTime = b2.QueueTime;
            DateTime t1 = queueTime ?? DateTime.MaxValue;
            queueTime = b1.QueueTime;
            DateTime t2 = queueTime ?? DateTime.MaxValue;
            return DateTime.Compare(t1, t2);
          }));
          break;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeAscending:
          builds.Sort((Comparison<BuildData>) ((b1, b2) =>
          {
            DateTime? queueTime = b1.QueueTime;
            DateTime t1 = queueTime ?? DateTime.MaxValue;
            queueTime = b2.QueueTime;
            DateTime t2 = queueTime ?? DateTime.MaxValue;
            return DateTime.Compare(t1, t2);
          }));
          break;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeDescending:
          builds.Sort((Comparison<BuildData>) ((b1, b2) =>
          {
            DateTime? startTime = b2.StartTime;
            DateTime t1 = startTime ?? DateTime.MaxValue;
            startTime = b1.StartTime;
            DateTime t2 = startTime ?? DateTime.MaxValue;
            return DateTime.Compare(t1, t2);
          }));
          break;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeAscending:
          builds.Sort((Comparison<BuildData>) ((b1, b2) =>
          {
            DateTime? startTime = b1.StartTime;
            DateTime t1 = startTime ?? DateTime.MaxValue;
            startTime = b2.StartTime;
            DateTime t2 = startTime ?? DateTime.MaxValue;
            return DateTime.Compare(t1, t2);
          }));
          break;
      }
    }

    private void UpdateBuildProperties(
      IVssRequestContext requestContext,
      List<BuildData> builds,
      IEnumerable<string> propertyFilters = null)
    {
      if (builds.Count <= 0 || propertyFilters == null || !propertyFilters.Any<string>())
        return;
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      using (requestContext.CITimer("GetPropertiesElapsedMilliseconds"))
      {
        using (TeamFoundationDataReader properties = service.GetProperties(requestContext, builds.Select<BuildData, ArtifactSpec>((Func<BuildData, ArtifactSpec>) (x => x.CreateArtifactSpec(requestContext))), propertyFilters))
          ArtifactPropertyKinds.MatchProperties<BuildData>(properties, (IList<BuildData>) builds, (Func<BuildData, int>) (x => x.Id), (Action<BuildData, PropertiesCollection>) ((x, y) => x.Properties = y));
      }
    }

    private void GetBuildsByIds(
      IVssRequestContext requestContext,
      Guid? projectId,
      int fromBuildId,
      int toBuildId,
      out BuildData fromBuild,
      out BuildData toBuild)
    {
      fromBuild = toBuild = (BuildData) null;
      IEnumerable<BuildData> buildsByIds;
      if (!projectId.HasValue)
        buildsByIds = this.GetBuildsByIds(requestContext, (IEnumerable<int>) new int[2]
        {
          fromBuildId,
          toBuildId
        }, (IEnumerable<string>) null, false);
      else
        buildsByIds = this.GetBuildsByIds(requestContext, projectId.Value, (IEnumerable<int>) new int[2]
        {
          fromBuildId,
          toBuildId
        }, (IEnumerable<string>) null, false);
      if (buildsByIds == null)
        throw new BuildNotFoundException(BuildServerResources.SourceOrTargetBuildNotFound((object) fromBuildId, (object) toBuildId));
      foreach (BuildData buildData in buildsByIds)
      {
        if (buildData.Id == fromBuildId)
          fromBuild = buildData;
        if (buildData.Id == toBuildId)
          toBuild = buildData;
      }
      if (fromBuild == null)
        throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) fromBuildId));
      if (toBuild == null)
        throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) toBuildId));
      if (fromBuild.Definition.Id != toBuild.Definition.Id)
        throw new DefinitionNotMatchedException(BuildServerResources.DefinitionNotMatched((object) fromBuild.Definition.Name, (object) toBuild.Definition.Name));
    }

    private MinimalBuildDefinition MinimizeDefinitionReference(BuildData build) => new MinimalBuildDefinition()
    {
      ProjectId = build.Definition.ProjectId,
      Id = build.Definition.Id,
      Path = build.Definition.Path,
      QueueStatus = build.Definition.QueueStatus,
      JobAuthorizationScope = build.Definition.JobAuthorizationScope
    };

    private IEnumerable<BuildData> FilterBuilds(
      IVssRequestContext requestContext,
      IEnumerable<BuildData> buildDataList,
      Guid? projectId = null)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (FilterBuilds)))
      {
        foreach (BuildData buildData in buildDataList)
        {
          if (this.SecurityProvider.HasBuildPermission(requestContext, buildData.ProjectId, (IReadOnlyBuildData) buildData, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds))
          {
            if (!this.SecurityProvider.HasDefinitionPermission(requestContext, buildData.ProjectId, buildData.Definition, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds))
              buildData.Definition = this.MinimizeDefinitionReference(buildData);
            if (projectId.HasValue)
            {
              Guid? nullable = projectId;
              Guid projectId1 = buildData.ProjectId;
              if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == projectId1 ? 1 : 0) : 1) : 0) == 0)
                continue;
            }
            yield return buildData;
          }
        }
      }
    }

    private List<BuildData> FilterBuildsAndUpdateTimeRange(
      IVssRequestContext requestContext,
      IEnumerable<BuildData> buildDataList,
      BuildService.BuildTimeRange timeRange,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder queryOrder,
      out bool timeRangeUpdated,
      ref HashSet<int> definitionsWithoutPermission)
    {
      List<BuildData> buildDataList1 = new List<BuildData>();
      timeRangeUpdated = false;
      using (requestContext.TraceScope(nameof (BuildService), nameof (FilterBuildsAndUpdateTimeRange)))
      {
        foreach (BuildData buildData in buildDataList)
        {
          bool flag = this.SecurityProvider.HasDefinitionPermission(requestContext, buildData.ProjectId, buildData.Definition, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds);
          if (!flag)
            definitionsWithoutPermission.Add(buildData.Definition.Id);
          if (this.SecurityProvider.HasBuildPermission(requestContext, buildData.ProjectId, (IReadOnlyBuildData) buildData, Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds))
          {
            buildDataList1.Add(buildData);
            if (!flag)
              buildData.Definition = this.MinimizeDefinitionReference(buildData);
          }
          switch (queryOrder)
          {
            case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending:
              if (buildData.FinishTime.HasValue && (!timeRange.MinTime.HasValue || buildData.FinishTime.Value.CompareTo(timeRange.MinTime.Value) > 0))
              {
                timeRange.MinTime = buildData.FinishTime;
                timeRangeUpdated = true;
                continue;
              }
              continue;
            case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending:
              if (buildData.FinishTime.HasValue && (!timeRange.MaxTime.HasValue || buildData.FinishTime.Value.CompareTo(timeRange.MaxTime.Value) < 0))
              {
                timeRange.MaxTime = buildData.FinishTime;
                timeRangeUpdated = true;
                continue;
              }
              continue;
            case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeDescending:
              if (buildData.QueueTime.HasValue && (!timeRange.MaxTime.HasValue || buildData.QueueTime.Value.CompareTo(timeRange.MaxTime.Value) < 0))
              {
                timeRange.MaxTime = buildData.QueueTime;
                timeRangeUpdated = true;
                continue;
              }
              continue;
            case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeAscending:
              if (buildData.QueueTime.HasValue && (!timeRange.MinTime.HasValue || buildData.QueueTime.Value.CompareTo(timeRange.MinTime.Value) > 0))
              {
                timeRange.MinTime = buildData.QueueTime;
                timeRangeUpdated = true;
                continue;
              }
              continue;
            case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeDescending:
              if (buildData.StartTime.HasValue && (!timeRange.MaxTime.HasValue || buildData.StartTime.Value.CompareTo(timeRange.MaxTime.Value) < 0))
              {
                timeRange.MaxTime = buildData.StartTime;
                timeRangeUpdated = true;
                continue;
              }
              continue;
            case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeAscending:
              if (buildData.StartTime.HasValue && (!timeRange.MinTime.HasValue || buildData.StartTime.Value.CompareTo(timeRange.MinTime.Value) > 0))
              {
                timeRange.MinTime = buildData.StartTime;
                timeRangeUpdated = true;
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      return buildDataList1;
    }

    private List<BuildData> GetBuildsByIdsInternal(
      IVssRequestContext requestContext,
      IEnumerable<int> buildIdsParam,
      IEnumerable<string> propertyFilters,
      bool includeDeleted,
      Guid? projectId)
    {
      if ((buildIdsParam != null ? (!buildIdsParam.Any<int>() ? 1 : 0) : 1) != 0)
        return new List<BuildData>();
      int[] array = buildIdsParam.Distinct<int>().ToArray<int>();
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildsByIdsInternal)))
      {
        List<BuildData> buildDataList = (List<BuildData>) null;
        if (requestContext.IsUserContext)
        {
          IBuildDataCacheService service = requestContext.GetService<IBuildDataCacheService>();
          buildDataList = new List<BuildData>(array.Length);
          foreach (int buildId in array)
          {
            BuildData buildData;
            if (service.TryGet(requestContext, buildId, out buildData))
            {
              buildDataList.Add(buildData);
            }
            else
            {
              buildDataList = (List<BuildData>) null;
              break;
            }
          }
        }
        if (buildDataList == null)
        {
          Lazy<string> parametersString = this.GetParametersString((IEnumerable<int>) array, propertyFilters, includeDeleted, projectId);
          int maxMilliSeconds = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/SlowCommandThresholdsInMs/GetBuildsByIds", 3000);
          using (requestContext.TraceSlowCall("Service", maxMilliSeconds, parametersString, nameof (GetBuildsByIdsInternal)))
          {
            using (Build2Component component = requestContext.CreateComponent<Build2Component>())
              buildDataList = component.GetBuildsByIds((IEnumerable<int>) array, includeDeleted).ToList<BuildData>();
          }
          if (buildDataList != null)
          {
            IBuildDataCacheService service = requestContext.GetService<IBuildDataCacheService>();
            foreach (BuildData buildData in buildDataList)
            {
              if (buildData != null)
                service.Set(requestContext, buildData);
            }
          }
        }
        List<BuildData> list = this.FilterBuilds(requestContext, (IEnumerable<BuildData>) buildDataList, projectId).ToList<BuildData>();
        if (list.Count > 0 && propertyFilters != null && propertyFilters.Any<string>())
        {
          ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
          using (requestContext.CITimer("GetPropertiesElapsedMilliseconds"))
          {
            using (TeamFoundationDataReader properties = service.GetProperties(requestContext, list.Select<BuildData, ArtifactSpec>((Func<BuildData, ArtifactSpec>) (x => x.CreateArtifactSpec(requestContext))), propertyFilters))
              ArtifactPropertyKinds.MatchProperties<BuildData>(properties, (IList<BuildData>) list, (Func<BuildData, int>) (x => x.Id), (Action<BuildData, PropertiesCollection>) ((x, y) => x.Properties = y));
          }
        }
        return list;
      }
    }

    private async Task<IEnumerable<BuildData>> GetBuildsByIdsInternalAsync(
      IVssRequestContext requestContext,
      IEnumerable<int> buildIdsParam,
      IEnumerable<string> propertyFilters,
      bool includeDeleted,
      Guid? projectId)
    {
      IEnumerable<int> source = buildIdsParam;
      if ((source != null ? (!source.Any<int>() ? 1 : 0) : 1) != 0)
        return (IEnumerable<BuildData>) Array.Empty<BuildData>();
      int[] array = buildIdsParam.Distinct<int>().ToArray<int>();
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetBuildsByIdsInternalAsync)))
      {
        List<BuildData> list1;
        using (requestContext.TraceSlowCall("Service", requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/SlowCommandThresholdsInMs/GetBuildsByIds", 3000), this.GetParametersString((IEnumerable<int>) array, propertyFilters, includeDeleted, projectId), nameof (GetBuildsByIdsInternalAsync)))
        {
          using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
            list1 = (await bc.GetBuildsByIdsAsync((IEnumerable<int>) array, includeDeleted)).ToList<BuildData>();
        }
        List<BuildData> list2 = this.FilterBuilds(requestContext, (IEnumerable<BuildData>) list1, projectId).ToList<BuildData>();
        if (list2.Count > 0 && propertyFilters != null && propertyFilters.Any<string>())
        {
          ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
          using (requestContext.CITimer("GetPropertiesElapsedMilliseconds"))
          {
            using (TeamFoundationDataReader properties = service.GetProperties(requestContext, list2.Select<BuildData, ArtifactSpec>((Func<BuildData, ArtifactSpec>) (x => x.CreateArtifactSpec(requestContext))), propertyFilters))
              ArtifactPropertyKinds.MatchProperties<BuildData>(properties, (IList<BuildData>) list2, (Func<BuildData, int>) (x => x.Id), (Action<BuildData, PropertiesCollection>) ((x, y) => x.Properties = y));
          }
        }
        return (IEnumerable<BuildData>) list2;
      }
    }

    private GetChangesResult ReadChanges(
      IVssRequestContext requestContext,
      BuildData build,
      bool includeSourceChange,
      int startId,
      int maxChanges)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (ReadChanges)))
      {
        bool changesTruncated = false;
        int num = maxChanges;
        if (maxChanges != int.MaxValue)
          ++maxChanges;
        List<ChangeData> source = (List<ChangeData>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          source = component.GetChanges(build.ProjectId, build.Id, startId, includeSourceChange, maxChanges);
        IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
        BuildDefinition definition = service.GetDefinition(requestContext.Elevate(), build.ProjectId, build.Definition.Id, build.Definition.Revision, includeDeleted: true);
        if (definition == null)
        {
          requestContext.TraceError(0, "Service", "Build {0} exists in project {1} but its definition {2}, revision {3} was not found.", (object) build.Id, (object) build.ProjectId, (object) build.Definition.Id, (object) build.Definition.Revision);
          return new GetChangesResult();
        }
        BuildRepository repository;
        if (build.IsTriggeredByResourceRepository(requestContext, definition))
        {
          try
          {
            repository = build.GetResourceRepository(requestContext.Elevate(), definition);
          }
          catch (ResourceNotFoundException ex)
          {
            requestContext.TraceError(0, "Service", "Build {0} exists in project {1} but resource repository was not found.", (object) build.Id, (object) build.ProjectId);
            return new GetChangesResult();
          }
          catch (YamlFileNotFoundException ex)
          {
            requestContext.TraceError(0, "Service", "Build {0} exists in project {1} but yaml file was not found.", (object) build.Id, (object) build.ProjectId);
            return new GetChangesResult();
          }
        }
        else
          repository = service.GetExpandedRepository(requestContext.Elevate(), definition);
        int? nextChangeId = new int?();
        List<Change> changes = this.ConvertChanges(requestContext, build.ProjectId, repository, source.Take<ChangeData>(num));
        if (source.Count > num)
        {
          nextChangeId = new int?(source[num].Position);
          changesTruncated = true;
        }
        if (((startId != 0 ? 0 : (changes.Count == 0 ? 1 : 0)) & (includeSourceChange ? 1 : 0)) != 0 && !string.IsNullOrEmpty(build.SourceVersion))
        {
          Change sourceChange = this.GetSourceChange(requestContext, (IReadOnlyBuildData) build, repository);
          if (sourceChange != null)
            changes.Add(sourceChange);
        }
        return new GetChangesResult(changes, nextChangeId, changesTruncated);
      }
    }

    private GetChangesResult CalculateChanges(
      IVssRequestContext requestContext,
      BuildData build,
      bool includeSourceChange,
      int startId,
      int maxChanges)
    {
      using (requestContext.TraceScope(nameof (BuildService), nameof (CalculateChanges)))
      {
        bool changesTruncated = false;
        if (build.Repository == null)
          return new GetChangesResult();
        IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, build.Repository.Type, false);
        if (sourceProvider == null)
          return new GetChangesResult();
        IBuildDefinitionService definitionService = requestContext.GetService<IBuildDefinitionService>();
        BuildDefinition definition = definitionService.GetDefinition(requestContext.Elevate(), build.ProjectId, build.Definition.Id, build.Definition.Revision, includeDeleted: true);
        int? nextChangeId = new int?();
        Lazy<Change> sourceChange = new Lazy<Change>((Func<Change>) (() =>
        {
          if (string.IsNullOrEmpty(build.SourceVersion))
            return (Change) null;
          if (definition != null)
            return this.GetSourceChange(requestContext, (IReadOnlyBuildData) build, definitionService.GetExpandedRepository(requestContext.Elevate(), definition));
          requestContext.TraceError(0, "Service", "Build {0} exists in project {1} but its definition {2}, revision {3} was not found.", (object) build.Id, (object) build.ProjectId, (object) build.Definition.Id, (object) build.Definition.Revision);
          return (Change) null;
        }));
        List<Change> changes1;
        if (sourceProvider.TryCalculateChangesWithValidation(requestContext, build, definition, 201, out changes1))
        {
          IReadOnlyList<ChangeData> changeData = sourceProvider.GetChangeData(requestContext, (IReadOnlyList<Change>) changes1, sourceChange);
          List<ChangeData> changeDataList;
          using (requestContext.AllowAnonymousOrPublicUserWrites(build.ToSecuredObject()))
          {
            using (Build2Component component = requestContext.CreateComponent<Build2Component>(connectionType: new DatabaseConnectionType?(DatabaseConnectionType.Default)))
              changeDataList = component.StoreChanges(build.ProjectId, build.Id, (IEnumerable<ChangeData>) changeData);
          }
          requestContext.GetService<IBuildDispatcher>().SendChangesCalculated(requestContext, build.Id);
          if (changeDataList.Count > maxChanges)
          {
            nextChangeId = new int?(changeDataList[maxChanges].Position);
            changesTruncated = true;
          }
          if (changeDataList.Count > 200)
            changesTruncated = true;
        }
        else
          changes1 = new List<Change>();
        GetChangesResult changes2 = new GetChangesResult(changes1.Take<Change>(maxChanges).ToList<Change>(), nextChangeId, changesTruncated);
        if (changes2.Changes.Count == 0 & includeSourceChange && sourceChange.Value != null)
          changes2.Changes.Add(sourceChange.Value);
        return changes2;
      }
    }

    private Lazy<string> GetParametersString(
      IEnumerable<int> definitionIds,
      IEnumerable<int> queueIds,
      string buildNumber,
      DateTime? minFinishTime,
      DateTime? maxFinishTime,
      string requestedFor,
      BuildReason? reasonFilter,
      BuildStatus? statusFilter,
      BuildResult? resultFilter,
      IEnumerable<string> tagFilters,
      BuildQueryOrder queryOrder,
      QueryDeletedOption queryDeletedOption,
      string repositoryId,
      string repositoryType,
      string branchName,
      int? maxBuildsPerDefinition)
    {
      return new Lazy<string>((Func<string>) (() =>
      {
        string[] strArray = new string[16];
        strArray[0] = definitionIds != null ? "definitionIds:" + string.Join<int>(",", definitionIds) : "";
        strArray[1] = queueIds != null ? "queueIds:" + string.Join<int>(",", queueIds) : "";
        strArray[2] = !string.IsNullOrEmpty(buildNumber) ? "buildNumber:" + buildNumber : "";
        DateTime dateTime;
        string str1;
        if (!minFinishTime.HasValue)
        {
          str1 = "";
        }
        else
        {
          ref DateTime? local = ref minFinishTime;
          string str2;
          if (!local.HasValue)
          {
            str2 = (string) null;
          }
          else
          {
            dateTime = local.GetValueOrDefault();
            dateTime = dateTime.Date;
            str2 = dateTime.ToString();
          }
          str1 = "minFinishTime:" + str2;
        }
        strArray[3] = str1;
        string str3;
        if (!maxFinishTime.HasValue)
        {
          str3 = "";
        }
        else
        {
          ref DateTime? local = ref maxFinishTime;
          string str4;
          if (!local.HasValue)
          {
            str4 = (string) null;
          }
          else
          {
            dateTime = local.GetValueOrDefault();
            dateTime = dateTime.Date;
            str4 = dateTime.ToString();
          }
          str3 = "maxFinishTime:" + str4;
        }
        strArray[4] = str3;
        strArray[5] = !string.IsNullOrEmpty(requestedFor) ? "requestedFor:" + requestedFor : "";
        strArray[6] = reasonFilter.HasValue ? "reasonFilter:" + reasonFilter.ToString() : "";
        strArray[7] = statusFilter.HasValue ? "statusFilter:" + statusFilter.ToString() : "";
        strArray[8] = resultFilter.HasValue ? "resultFilter:" + resultFilter.ToString() : "";
        strArray[9] = tagFilters != null ? "tagFilters:" + string.Join(",", tagFilters) : "";
        strArray[10] = "queryOrder:" + queryOrder.ToString();
        strArray[11] = "queryDeletedOption:" + queryDeletedOption.ToString();
        strArray[12] = !string.IsNullOrEmpty(repositoryId) ? "repositoryId:" + repositoryId : "";
        strArray[13] = !string.IsNullOrEmpty(repositoryType) ? "repositoryType:" + repositoryType : "";
        strArray[14] = !string.IsNullOrEmpty(branchName) ? "branchName:" + branchName : "";
        strArray[15] = maxBuildsPerDefinition.HasValue ? "maxBuildsPerDefinition:" + maxBuildsPerDefinition.Value.ToString() : "";
        return string.Join("|", strArray);
      }));
    }

    private Lazy<string> GetParametersString(
      IEnumerable<int> buildIds,
      IEnumerable<string> propertyFilters,
      bool includeDeleted,
      Guid? projectId)
    {
      return new Lazy<string>((Func<string>) (() => string.Join("|", new string[4]
      {
        buildIds != null ? "buildIds:" + string.Join<int>(",", buildIds) : "",
        propertyFilters != null ? "propertyFilters:" + string.Join(",", propertyFilters) : "",
        "includeDeleted:" + includeDeleted.ToString(),
        projectId.HasValue ? "projectId:" + projectId.ToString() : ""
      })));
    }

    private Change GetSourceChange(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      BuildRepository repository)
    {
      return this.ConvertChanges(requestContext, build.ProjectId, repository, (IEnumerable<ChangeData>) new ChangeData[1]
      {
        new ChangeData() { Descriptor = build.SourceVersion }
      }).FirstOrDefault<Change>();
    }

    internal SourceVersionInfo GenerateSourceVersionInfo(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build)
    {
      ArgumentUtility.CheckForNull<IReadOnlyBuildData>(build, nameof (build));
      ArgumentUtility.CheckForEmptyGuid(build.ProjectId, "ProjectId");
      ArgumentUtility.CheckForNull<MinimalBuildDefinition>(build.Definition, "Definition");
      BuildDefinition definition = requestContext.GetService<IBuildDefinitionService>().GetDefinition(requestContext, build.ProjectId, build.Definition.Id, build.Definition.Revision, includeDeleted: true);
      if (definition?.Repository == null)
      {
        requestContext.TraceError(0, "Service", "Build {0} exists in project {1}, but doesn't have a definition or a repository.", (object) build.Id, (object) build.ProjectId);
        return (SourceVersionInfo) null;
      }
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definition.Repository.Type, false);
      if (sourceProvider == null)
      {
        requestContext.TraceError(0, "Service", "Source provider {0} not found.", (object) definition.Repository.Type);
        return (SourceVersionInfo) null;
      }
      string pullRequestId;
      if (build.TriggerInfo.TryGetValue("pr.number", out pullRequestId))
      {
        PullRequest pullRequest = sourceProvider.GetPullRequest(requestContext, build, pullRequestId);
        if (pullRequest != null)
          return pullRequest.ToSourceVersionInfo();
        requestContext.TraceError(12030199, nameof (BuildService), "Unable to retrieve pull request {0} from repository {1}", (object) pullRequestId, (object) definition.Repository.Id);
        return (SourceVersionInfo) null;
      }
      string str = build.Reason == BuildReason.CheckInShelveset || build.Reason == BuildReason.ValidateShelveset ? build.SourceBranch : build.SourceVersion;
      Change change = sourceProvider.GetChanges(requestContext, build.ProjectId, definition.Repository, (IEnumerable<string>) new string[1]
      {
        str
      }).SingleOrDefault<Change>();
      if (change != null)
        return change.ToSourceVersionInfo();
      if (build.Reason != BuildReason.CheckInShelveset || build.SourceVersionInfo.Message == null)
        requestContext.TraceError(12030199, nameof (BuildService), "Unable to retrieve source change for {0} from repository {1} for build {2}", (object) str, (object) definition.Repository.Id, (object) build.Id);
      return (SourceVersionInfo) null;
    }

    private void NormalizeBuildChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildData build,
      IEnumerable<Change> changes)
    {
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, build.Repository.Type, false);
      if (sourceProvider == null)
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IBuildDefinitionService service = vssRequestContext.GetService<IBuildDefinitionService>();
      BuildDefinition definition = service.GetDefinition(vssRequestContext, build.ProjectId, build.Definition.Id, build.Definition.Revision, includeDeleted: true);
      if (definition == null)
      {
        requestContext.TraceError(0, "Service", "Build {0} exists in project {1} but its definition {2}, revision {3} was not found.", (object) build.Id, (object) build.ProjectId, (object) build.Definition.Id, (object) build.Definition.Revision);
      }
      else
      {
        BuildRepository expandedRepository = service.GetExpandedRepository(vssRequestContext, definition);
        sourceProvider.NormalizeBuildChanges(requestContext, projectId, expandedRepository, changes);
      }
    }

    private List<Change> ConvertChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      IEnumerable<ChangeData> changes)
    {
      ArgumentUtility.CheckForNull<BuildRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<IEnumerable<ChangeData>>(changes, nameof (changes));
      if (!changes.Any<ChangeData>())
        return new List<Change>();
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repository.Type, false);
      if (sourceProvider == null)
        return changes.Select<ChangeData, Change>((Func<ChangeData, Change>) (c => new Change()
        {
          Id = c.Descriptor
        })).ToList<Change>();
      IEnumerable<Change> changes1;
      return sourceProvider.TryDeserializeChanges(requestContext, changes, out changes1) ? changes1.ToList<Change>() : sourceProvider.GetChanges(requestContext, projectId, repository, changes.Select<ChangeData, string>((Func<ChangeData, string>) (c => c.Descriptor)));
    }

    public async Task<IList<ArtifactCleanupRecord>> CleanUpArtifactsAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity requestedBy,
      IList<ArtifactCleanupRecord> artifactCleanupRecords)
    {
      requestedBy = requestedBy ?? requestContext.GetUserIdentity();
      List<ArtifactCleanupRecord> successfullyCleanedArtifacts = new List<ArtifactCleanupRecord>();
      IBuildArtifactProviderService service = requestContext.GetService<IBuildArtifactProviderService>();
      IBuildDefinitionService definitionService = requestContext.GetService<IBuildDefinitionService>();
      Dictionary<int, BuildDefinition> definitionsById = artifactCleanupRecords.ToLookup<ArtifactCleanupRecord, Guid, BuildData>((Func<ArtifactCleanupRecord, Guid>) (acr => acr.Build.ProjectId), (Func<ArtifactCleanupRecord, BuildData>) (acr => acr.Build)).SelectMany<IGrouping<Guid, BuildData>, BuildDefinition>((Func<IGrouping<Guid, BuildData>, IEnumerable<BuildDefinition>>) (group => (IEnumerable<BuildDefinition>) requestContext.RunSynchronously<List<BuildDefinition>>((Func<Task<List<BuildDefinition>>>) (() =>
      {
        try
        {
          return definitionService.GetDefinitionsByIdsAsync(requestContext, group.Key, group.Select<BuildData, int>((Func<BuildData, int>) (b => b.Definition.Id)).Distinct<int>().ToList<int>(), true);
        }
        catch (Exception ex)
        {
          return Task.FromResult<List<BuildDefinition>>(new List<BuildDefinition>());
        }
      })))).ToDictionary<BuildDefinition, int>((Func<BuildDefinition, int>) (x => x.Id));
      Dictionary<int, (BuildData, List<BuildDefinitionStep>)> source = new Dictionary<int, (BuildData, List<BuildDefinitionStep>)>();
      Dictionary<int, BuildData> dictionary = new Dictionary<int, BuildData>();
      foreach (ArtifactCleanupRecord artifactCleanupRecord in (IEnumerable<ArtifactCleanupRecord>) artifactCleanupRecords)
        dictionary[artifactCleanupRecord.Build.Id] = artifactCleanupRecord.Build;
      Dictionary<int, int> buildIdToDataspaceId = new Dictionary<int, int>();
      Dictionary<int, BuildData> queueIdToBuild = new Dictionary<int, BuildData>();
      using (Build2Component buildComponent = requestContext.CreateComponent<Build2Component>())
        buildIdToDataspaceId.AddRange<KeyValuePair<int, int>, Dictionary<int, int>>(dictionary.Values.Select<BuildData, KeyValuePair<int, int>>((Func<BuildData, KeyValuePair<int, int>>) (b => new KeyValuePair<int, int>(b.Id, buildComponent.GetDataspaceId(b.ProjectId)))));
      foreach (ArtifactCleanupRecord artifactCleanupRecord in (IEnumerable<ArtifactCleanupRecord>) artifactCleanupRecords)
      {
        BuildArtifact artifact = artifactCleanupRecord.Artifact;
        BuildData build = artifactCleanupRecord.Build;
        BuildDefinition buildDefinition;
        definitionsById.TryGetValue(build.Definition.Id, out buildDefinition);
        int key = build.QueueId ?? buildDefinition?.Queue?.Id ?? -1;
        if (key > -1)
        {
          IArtifactProvider artifactProvider;
          if (service.TryGetArtifactProvider(requestContext, artifact?.Resource?.Type, out artifactProvider))
          {
            try
            {
              IList<BuildDefinitionStep> collection = artifactProvider.CleanupArtifact(requestContext, buildDefinition?.Name ?? "Unknown Definition", build, artifact);
              if (collection != null && collection.Count > 0)
              {
                foreach (BuildDefinitionStep buildDefinitionStep in (IEnumerable<BuildDefinitionStep>) collection)
                {
                  buildDefinitionStep.AlwaysRun = true;
                  buildDefinitionStep.ContinueOnError = true;
                }
                if (!source.ContainsKey(key))
                  source.Add(key, (build, new List<BuildDefinitionStep>()));
                source[key].Item2.AddRange((IEnumerable<BuildDefinitionStep>) collection);
                if (!queueIdToBuild.ContainsKey(key))
                  queueIdToBuild[key] = build;
              }
              successfullyCleanedArtifacts.Add(artifactCleanupRecord);
            }
            catch (VssServiceResponseException ex)
            {
              if (ex.HttpStatusCode == HttpStatusCode.NotFound)
                successfullyCleanedArtifacts.Add(artifactCleanupRecord);
              requestContext.TraceException(12030046, "Service", (Exception) ex);
            }
            catch (GitRepositoryNotFoundException ex)
            {
              successfullyCleanedArtifacts.Add(artifactCleanupRecord);
              requestContext.TraceException(12030046, "Service", (Exception) ex);
            }
            catch (LabelNotFoundException ex)
            {
              successfullyCleanedArtifacts.Add(artifactCleanupRecord);
              requestContext.TraceException(12030046, "Service", (Exception) ex);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(12030046, "Service", ex);
            }
          }
          else
          {
            successfullyCleanedArtifacts.Add(artifactCleanupRecord);
            if ((artifact == null || artifact.Resource?.Type == null) && artifact != null)
              requestContext.TraceWarning(12030284, nameof (BuildService), string.Format("An artifact type was empty for artifact with Id: {0}", (object) artifact.Id));
            requestContext.TraceWarning(12030284, nameof (BuildService), string.Format("An artifact was not cleaned up due to a missing artifact provider: {0}", (object) artifactCleanupRecord));
          }
        }
        else
        {
          successfullyCleanedArtifacts.Add(artifactCleanupRecord);
          requestContext.TraceWarning(12030283, nameof (BuildService), string.Format("An artifact was not cleaned up due to a missing QueueId: {0}", (object) artifactCleanupRecord));
        }
      }
      IEnumerable<(BuildData, BuildDefinition, List<BuildDefinitionStep>)> cleanupJobs = source.Select<KeyValuePair<int, (BuildData, List<BuildDefinitionStep>)>, (BuildData, BuildDefinition, List<BuildDefinitionStep>)>((Func<KeyValuePair<int, (BuildData, List<BuildDefinitionStep>)>, (BuildData, BuildDefinition, List<BuildDefinitionStep>)>) (kvp =>
      {
        BuildData buildData = queueIdToBuild[kvp.Key];
        BuildDefinition buildDefinition = definitionsById[buildData.Definition.Id];
        return (buildData, buildDefinition, kvp.Value.Tasks);
      }));
      BuildService.RunCleanupTasks(requestContext, requestedBy, cleanupJobs);
      List<ArtifactCleanupRecordKey> list = successfullyCleanedArtifacts.Select<ArtifactCleanupRecord, ArtifactCleanupRecordKey>((Func<ArtifactCleanupRecord, ArtifactCleanupRecordKey>) (record => new ArtifactCleanupRecordKey()
      {
        DataspaceId = buildIdToDataspaceId[record.Build.Id],
        BuildId = record.Build.Id,
        ArtifactId = record.Artifact.Id
      })).ToList<ArtifactCleanupRecordKey>();
      using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
        await bc.DeleteArtifactCleanupRecords((IList<ArtifactCleanupRecordKey>) list);
      IList<ArtifactCleanupRecord> artifactCleanupRecordList = (IList<ArtifactCleanupRecord>) successfullyCleanedArtifacts;
      successfullyCleanedArtifacts = (List<ArtifactCleanupRecord>) null;
      return artifactCleanupRecordList;
    }

    public List<PoisonedBuild> GetPoisonedBuilds(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      using (requestContext.TraceScope(nameof (BuildService), nameof (GetPoisonedBuilds)))
      {
        int batchSize = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/PoisonedBuildsCleanupBatch", 100);
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          return component.GetPoisonedBuilds(batchSize).ToList<PoisonedBuild>();
      }
    }

    private async Task ScheduleDeleteBuildArtifactJobsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      PurgedBuildsResults purgedBuildResults)
    {
      if (purgedBuildResults.PurgedArtifacts.Count <= 0)
        return;
      List<ArtifactCleanupRecord> artifactCleanupRecords = new List<ArtifactCleanupRecord>();
      Dictionary<int, BuildData> buildsById = purgedBuildResults.PurgedBuilds.ToDictionary<BuildData, int>((Func<BuildData, int>) (b => b.Id));
      artifactCleanupRecords.AddRange(purgedBuildResults.PurgedArtifacts.Select<BuildArtifact, ArtifactCleanupRecord>((Func<BuildArtifact, ArtifactCleanupRecord>) (purgedArtifact => new ArtifactCleanupRecord()
      {
        Build = buildsById[purgedArtifact.BuildId],
        Artifact = purgedArtifact
      })));
      IList<ArtifactCleanupRecord> artifactCleanupRecordList = await this.CleanUpArtifactsAsync(requestContext, requestContext.GetUserIdentity(), (IList<ArtifactCleanupRecord>) artifactCleanupRecords);
    }

    private async Task ScheduleDeleteBuildArtifactJobs(
      IVssRequestContext requestContext,
      Dictionary<int, DeleteBuildSpec> buildsToDelete,
      Microsoft.VisualStudio.Services.Identity.Identity requestedBy,
      IEnumerable<BuildData> deletedBuilds,
      ILookup<int, BuildArtifact> deletedArtifacts,
      Dictionary<int, BuildDefinition> definitionsForDeletedBuilds)
    {
      List<ArtifactCleanupRecord> artifactCleanupRecords = new List<ArtifactCleanupRecord>();
      foreach (BuildData deletedBuild in deletedBuilds)
      {
        foreach (BuildArtifact buildArtifact in deletedArtifacts[deletedBuild.Id])
          artifactCleanupRecords.Add(new ArtifactCleanupRecord()
          {
            Build = deletedBuild,
            Artifact = buildArtifact
          });
      }
      IList<ArtifactCleanupRecord> artifactCleanupRecordList = await this.CleanUpArtifactsAsync(requestContext, requestedBy, (IList<ArtifactCleanupRecord>) artifactCleanupRecords);
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      foreach (BuildData deletedBuild in deletedBuilds)
      {
        requestContext.TraceInfo(12030013, "Service", "Publishing BuildDeletedEvent for build {0}", (object) deletedBuild.Id);
        service.PublishNotification(requestContext, (object) new BuildDeletedEvent((IReadOnlyBuildData) deletedBuild));
      }
    }

    private static void RunCleanupTasks(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<(BuildData Build, BuildDefinition Definition, List<BuildDefinitionStep> CleanupSteps)> cleanupJobs)
    {
      foreach ((BuildData buildData, BuildDefinition buildDefinition, List<BuildDefinitionStep> buildDefinitionStepList) in cleanupJobs)
      {
        try
        {
          foreach (IEnumerable<BuildDefinitionStep> source in buildDefinitionStepList.GroupBy<BuildDefinitionStep, Guid>((Func<BuildDefinitionStep, Guid>) (step => step.TaskDefinition.Id)))
          {
            foreach (IList<BuildDefinitionStep> cleanupTasks in source.Batch<BuildDefinitionStep>(15000))
              new CleanupOrchestrationHelper(requestContext, buildDefinition, identity, identity).RunOrchestration(requestContext, buildData, cleanupTasks);
          }
        }
        catch (Exception ex1)
        {
          try
          {
            requestContext.TraceException(nameof (BuildService), ex1);
          }
          catch (SqlException ex2)
          {
          }
        }
      }
    }

    private void PublishBuildChangeEvent(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      BuildEventType eventType,
      IReadOnlyBuildData oldBuild = null)
    {
      requestContext.GetService<IBuildEventService>().AddBuildEvent(requestContext, build.ProjectId, build.Id, eventType);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/EventPublisherJobDelay", 5);
      requestContext.TraceAlways(12030297, TraceLevel.Info, "Build2", nameof (BuildService), "Queuing BuildEventPublisherJob for build event type " + eventType.ToString());
      IVssRequestContext requestContext1 = requestContext;
      List<Guid> jobIds = new List<Guid>();
      jobIds.Add(this.c_buildEventPublisherJobId);
      int maxDelaySeconds = num;
      service.QueueDelayedJobs(requestContext1, (IEnumerable<Guid>) jobIds, maxDelaySeconds);
      requestContext.TraceInfo(12030058, "Service", "published {0} BuildChangeEvent and queued a buildEvent Job for build {1}", (object) eventType.ToString(), (object) build.Id);
      this.PublishRunStateChangedNotification(requestContext, build, oldBuild);
    }

    private void PublishRunStateChangedNotification(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      IReadOnlyBuildData oldBuild = null)
    {
      try
      {
        if (oldBuild != null && build.GetPipelineRunState() == oldBuild.GetPipelineRunState())
        {
          requestContext.TraceInfo(12030232, "Build2", "Run state did not change {0}, {1}", (object) oldBuild.Status, (object) build.Status);
        }
        else
        {
          using (IDisposableReadOnlyList<IPipelinesNotificationPublisher> extensions = requestContext.GetExtensions<IPipelinesNotificationPublisher>())
          {
            requestContext.TraceInfo(12030232, "Build2", "Number of publishers for Run State Changed Notifications {0}", (object) extensions.Count);
            foreach (IPipelinesNotificationPublisher notificationPublisher in (IEnumerable<IPipelinesNotificationPublisher>) extensions)
            {
              try
              {
                notificationPublisher.PublishRunStateChangedNotification(requestContext, build);
              }
              catch (Exception ex)
              {
                requestContext.TraceException("Service", ex);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException("Service", ex);
      }
    }

    private void PublishBuildCompletedEvent(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build)
    {
      this.PublishBuildChangeEvent(requestContext, build, BuildEventType.BuildCompletedEvent);
      if (build.ProjectId == Guid.Empty || build.Definition == null || build.Definition.Id <= 0 || build.Id <= 0 || !build.Result.HasValue)
        requestContext.TraceError(nameof (BuildService), "Could not update LatestBuildResultCache, one of [projectId: {0}, definition: {1}, definitionId: {2}, buildId: {3}, buildResult: {4}] is invalid.", (object) build.ProjectId, (object) build.Definition, (object) build.Definition?.Id, (object) build.Id, (object) build.Result);
      else
        requestContext.GetService<ILatestBuildResultCacheService>().TryUpdateBuildResult(requestContext, build.ProjectId, build.Definition.Id, build.Id, build.Result.Value);
    }

    private OrchestrationTracer GetCIAO(IVssRequestContext requestContext) => requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").GetCIAO(requestContext);

    private void PublishDeleteCheckSuitesEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Guid> checkSuiteIds)
    {
      if (!checkSuiteIds.Any<Guid>())
        return;
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      DeleteCheckSuitesEvent checkSuitesEvent = new DeleteCheckSuitesEvent("MS.Azure.Pipelines.DeleteCheckSuites", projectId, checkSuiteIds);
      IVssRequestContext requestContext1 = requestContext;
      DeleteCheckSuitesEvent notificationEvent = checkSuitesEvent;
      service.PublishNotification(requestContext1, (object) notificationEvent);
      requestContext.TraceInfo(12030259, "Service", "Fired DeleteCheckSuitesEvent, ProjectId: {0}, Check Suite Ids count: {1}", (object) projectId, (object) checkSuiteIds.Count);
    }

    private IList<Guid> GetCheckpointRecordIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyCollection<BuildData> deletedBuilds)
    {
      List<Guid> checkpointRecordIds = new List<Guid>();
      foreach (BuildData deletedBuild in (IEnumerable<BuildData>) deletedBuilds)
      {
        Guid planId = deletedBuild.OrchestrationPlan.PlanId;
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline in deletedBuild.GetTimelines(requestContext, planId))
        {
          TimelineData? timelineData = deletedBuild.GetTimelineData(requestContext, new Guid?(timeline.Id), planId: new Guid?(planId));
          if (timelineData.HasValue && timelineData.Value.Timeline != null)
          {
            List<Microsoft.TeamFoundation.Build.WebApi.TimelineRecord> records = timelineData.Value.Timeline.Records;
            checkpointRecordIds.AddRange(records.Where<Microsoft.TeamFoundation.Build.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.Build.WebApi.TimelineRecord, bool>) (x => x.Identifier == "Checkpoint")).Select<Microsoft.TeamFoundation.Build.WebApi.TimelineRecord, Guid>((Func<Microsoft.TeamFoundation.Build.WebApi.TimelineRecord, Guid>) (x => x.Id)));
          }
        }
      }
      return (IList<Guid>) checkpointRecordIds;
    }

    private int? MapDeprecatedQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      int? requestedQueueId,
      out bool defaultQueueIsDeprecated)
    {
      defaultQueueIsDeprecated = false;
      if (!requestedQueueId.HasValue)
        return new int?();
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          "Hosted Ubuntu 1604",
          "Azure Pipelines"
        },
        {
          "Hosted VS2017",
          "Azure Pipelines"
        }
      };
      try
      {
        IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
        TaskAgentQueue agentQueue1 = service.GetAgentQueue(requestContext, projectId, requestedQueueId.Value);
        if (agentQueue1 == null || !dictionary.ContainsKey(agentQueue1.Name))
          return requestedQueueId;
        string queueName = dictionary[agentQueue1.Name];
        TaskAgentQueue agentQueue2 = service.GetAgentQueue(requestContext, projectId, queueName);
        if (agentQueue2 != null)
        {
          defaultQueueIsDeprecated = true;
          return new int?(agentQueue2.Id);
        }
        requestContext.TraceAlways(12030351, TraceLevel.Warning, "Build2", nameof (BuildService), string.Format("Deprecated Queue '{0}' cannot be overriden to '{1}' because the replacement queue does not exist in the project {2}! Manual intervention is required - cx must update the QueueId of the Definition!", (object) agentQueue1.Name, (object) queueName, (object) projectId));
        return requestedQueueId;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030352, nameof (BuildService), ex);
      }
      return requestedQueueId;
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    internal class BuildTimeRange
    {
      internal DateTime? MinTime { get; set; }

      internal DateTime? MaxTime { get; set; }

      internal BuildTimeRange(DateTime? minTime, DateTime? maxTime)
      {
        this.MinTime = minTime;
        this.MaxTime = maxTime;
      }

      internal BuildTimeRange(TimeFilter? timeFilter)
      {
        TimeFilter valueOrDefault;
        DateTime? nullable1;
        if (!timeFilter.HasValue)
        {
          nullable1 = new DateTime?();
        }
        else
        {
          valueOrDefault = timeFilter.GetValueOrDefault();
          nullable1 = valueOrDefault.MinTime;
        }
        this.MinTime = nullable1;
        DateTime? nullable2;
        if (!timeFilter.HasValue)
        {
          nullable2 = new DateTime?();
        }
        else
        {
          valueOrDefault = timeFilter.GetValueOrDefault();
          nullable2 = valueOrDefault.MaxTime;
        }
        this.MaxTime = nullable2;
      }
    }
  }
}
