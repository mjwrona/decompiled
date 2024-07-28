// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ReleaseWorkItemsCommitsHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ReleaseWorkItemsCommitsHelper
  {
    public const string WorkItemsWebUrlFieldKey = "Release.WorkItemWebUrl";
    public const string WorkItemsAssignedToFieldKey = "System.AssignedTo";
    public const string WorkItemsTitleFieldKey = "System.Title";
    public const string WorkItemsStateFieldKey = "System.State";
    public const string WorkItemsTypeKey = "System.WorkItemType";

    public static bool DoesArtifactTypeSupportsCommitsAndWorkItemsTraceability(
      IVssRequestContext requestContext,
      string artifactTypeId)
    {
      return ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsCommitsTraceability(requestContext, artifactTypeId) && ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsWorkItemsTraceability(requestContext, artifactTypeId);
    }

    public static bool DoesArtifactTypeSupportsCommitsOrWorkItemsTraceability(
      IVssRequestContext requestContext,
      string artifactTypeId)
    {
      return ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsCommitsTraceability(requestContext, artifactTypeId) || ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsWorkItemsTraceability(requestContext, artifactTypeId);
    }

    public static bool DoesArtifactTypeSupportsCommitsTraceability(
      IVssRequestContext requestContext,
      string artifactTypeId)
    {
      if (string.IsNullOrEmpty(artifactTypeId))
        return false;
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.GetService<ArtifactTypeServiceBase>().GetArtifactType(requestContext.Elevate(), artifactTypeId).IsCommitsTraceabilitySupported;
    }

    public static bool DoesArtifactTypeSupportsWorkItemsTraceability(
      IVssRequestContext requestContext,
      string artifactTypeId)
    {
      if (string.IsNullOrEmpty(artifactTypeId))
        return false;
      ArtifactTypeBase artifactTypeBase = requestContext != null ? requestContext.GetService<ArtifactTypeServiceBase>().GetArtifactType(requestContext.Elevate(), artifactTypeId) : throw new ArgumentNullException(nameof (requestContext));
      if (!artifactTypeBase.Equals((object) "Jenkins"))
        return artifactTypeBase.IsWorkitemsTraceabilitySupported;
      return artifactTypeBase.IsWorkitemsTraceabilitySupported && requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.BuildArtifactsTasks");
    }

    public static Release GetRelease(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int releaseId)
    {
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "ReleaseWorkItemsCommitsHelper.GetRelease", 1971005))
      {
        Func<ReleaseSqlComponent, Release> action = (Func<ReleaseSqlComponent, Release>) (component => component.GetRelease(projectInfo.Id, releaseId));
        return requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Release>(action);
      }
    }

    public static IList<Release> GetLastTwoReleases(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Release release)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      return ReleaseWorkItemsCommitsHelper.GetLastTwoReleases(requestContext, release.Id, release.ReleaseDefinitionId, 0, projectInfo);
    }

    public static IList<Release> GetLastTwoReleases(
      IVssRequestContext requestContext,
      int releaseId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      ProjectInfo projectInfo)
    {
      return ReleaseWorkItemsCommitsHelper.GetLastTwoReleases(requestContext, releaseId, releaseDefinitionId, definitionEnvironmentId, projectInfo, string.Empty, string.Empty);
    }

    public static Release GetPreviousRelease(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Release release,
      string artifactTypeId,
      string artifactSourceId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      return ReleaseWorkItemsCommitsHelper.GetLastTwoReleases(requestContext, release.Id, release.ReleaseDefinitionId, 0, projectInfo, artifactTypeId, artifactSourceId).FirstOrDefault<Release>((Func<Release, bool>) (r => r.Id < release.Id));
    }

    public static IList<Release> GetReleaseRange(
      IVssRequestContext requestContext,
      int releaseId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      ProjectInfo projectInfo,
      string artifactTypeId,
      string sourceId,
      int top)
    {
      IEnumerable<Release> source = (IEnumerable<Release>) new List<Release>();
      if (projectInfo != null)
        source = requestContext.GetService<ReleasesService>().ListReleases(requestContext, projectInfo.Id, releaseDefinitionId, definitionEnvironmentId, string.Empty, string.Empty, ReleaseStatus.Undefined, ReleaseEnvironmentStatus.Undefined, new DateTime?(), new DateTime?(), new DateTime?(), ReleaseQueryOrder.IdDescending, top, releaseId, true, false, false, false, false, false, artifactTypeId, sourceId, string.Empty, string.Empty, false, false, (IEnumerable<string>) null);
      return (IList<Release>) source.ToList<Release>();
    }

    public static IList<Release> GetLastTwoReleases(
      IVssRequestContext requestContext,
      int releaseId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      ProjectInfo projectInfo,
      string artifactTypeId,
      string sourceId)
    {
      return ReleaseWorkItemsCommitsHelper.GetReleaseRange(requestContext, releaseId, releaseDefinitionId, definitionEnvironmentId, projectInfo, artifactTypeId, sourceId, 2);
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Need to compute isOnlyRelease")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Need to compute both baseRelease and release")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Need to compute both baseRelease and release")]
    public static void GetReleaseRange(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int baseReleaseId,
      int releaseId,
      out Release baseRelease,
      out Release release,
      out bool isOnlyRelease)
    {
      baseRelease = (Release) null;
      release = (Release) null;
      release = ReleaseWorkItemsCommitsHelper.GetRelease(requestContext, projectInfo, releaseId);
      IList<Release> lastTwoReleases = ReleaseWorkItemsCommitsHelper.GetLastTwoReleases(requestContext, projectInfo, release);
      isOnlyRelease = lastTwoReleases.Count == 1;
      if (baseReleaseId == 0)
        return;
      baseRelease = ReleaseWorkItemsCommitsHelper.GetRelease(requestContext, projectInfo, baseReleaseId);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions while fetching external data should not be propagated")]
    public static IEnumerable<WorkItem> GetWorkItemsForRelease(
      IVssRequestContext requestContext,
      int releaseId,
      int definitionId,
      int definitionEnvironmentId,
      ProjectInfo projectInfo,
      int top,
      bool includeUrl,
      bool isDeploymentPending)
    {
      IEnumerable<WorkItem> workItems = (IEnumerable<WorkItem>) new List<WorkItem>();
      IList<Release> lastTwoReleases = ReleaseWorkItemsCommitsHelper.GetLastTwoReleases(requestContext, releaseId, definitionId, definitionEnvironmentId, projectInfo);
      int lastReleaseId = !isDeploymentPending ? (lastTwoReleases.Count<Release>() <= 1 ? 0 : lastTwoReleases.Last<Release>().Id) : (!lastTwoReleases.Any<Release>() ? 0 : lastTwoReleases.First<Release>().Id);
      IEnumerable<WorkItem> workItemsForRelease;
      try
      {
        workItemsForRelease = ReleaseWorkItemsCommitsHelper.GetWorkItems(requestContext, lastReleaseId, releaseId, projectInfo, top);
        if (includeUrl)
        {
          if (projectInfo != null)
          {
            foreach (WorkItem workItem in workItemsForRelease)
            {
              string itemWebAccessUri = WebAccessUrlBuilder.GetWorkItemWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectInfo.Name, workItem.Id);
              workItem.Fields.Add("Release.WorkItemWebUrl", (object) itemWebAccessUri);
            }
          }
        }
      }
      catch (Exception ex)
      {
        workItemsForRelease = (IEnumerable<WorkItem>) new List<WorkItem>()
        {
          new WorkItem() { Id = new int?(-1) }
        };
      }
      return workItemsForRelease;
    }

    public static IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      int lastReleaseId,
      int releaseId,
      ProjectInfo project,
      int top)
    {
      IEnumerable<WorkItem> workItems = (IEnumerable<WorkItem>) new List<WorkItem>();
      IEnumerable<ReleaseWorkItemRef> releaseWorkItemRefs = requestContext.GetService<ReleaseWorkItemsService>().GetReleaseWorkItemRefs(requestContext, project, lastReleaseId, releaseId, top);
      List<string> fields = new List<string>()
      {
        "System.AssignedTo",
        "System.State",
        "System.Title",
        "System.WorkItemType"
      };
      List<int> witIds = new List<int>();
      foreach (ReleaseWorkItemRef releaseWorkItemRef in releaseWorkItemRefs)
      {
        int result;
        if (int.TryParse(releaseWorkItemRef.Id, out result))
          witIds.Add(result);
      }
      if (witIds.Any<int>())
      {
        WorkItemTrackingHttpClient workItemsClient = requestContext.GetClient<WorkItemTrackingHttpClient>();
        Func<Task<List<WorkItem>>> func = (Func<Task<List<WorkItem>>>) (() => workItemsClient.GetWorkItemsAsync((IEnumerable<int>) witIds, (IEnumerable<string>) fields));
        workItems = (IEnumerable<WorkItem>) requestContext.ExecuteAsyncAndSyncResult<List<WorkItem>>(func);
      }
      return workItems;
    }

    public static PipelineArtifactSource GetReleaseArtifactSource(
      Release release,
      string artifactTypeId,
      string artifactSourceId,
      bool shouldBePrimary)
    {
      return ReleaseWorkItemsCommitsHelper.GetReleaseArtifactSource(release, artifactTypeId, artifactSourceId, shouldBePrimary, (string) null);
    }

    public static PipelineArtifactSource GetReleaseArtifactSource(
      Release release,
      string artifactTypeId,
      string artifactSourceId,
      bool shouldBePrimary,
      string artifactAlias)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (string.IsNullOrEmpty(artifactTypeId))
        throw new ArgumentNullException(nameof (artifactTypeId));
      ArtifactSource releaseArtifactSource = (ArtifactSource) null;
      if (!string.IsNullOrEmpty(artifactAlias))
        return release.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => artifact.Alias.Equals(artifactAlias, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ArtifactSource>() as PipelineArtifactSource;
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) release.LinkedArtifacts)
      {
        if (linkedArtifact.ArtifactTypeId.Equals(artifactTypeId, StringComparison.OrdinalIgnoreCase))
        {
          releaseArtifactSource = linkedArtifact;
          if (!string.IsNullOrEmpty(artifactSourceId) && !releaseArtifactSource.SourceId.Equals(artifactSourceId, StringComparison.OrdinalIgnoreCase))
            releaseArtifactSource = (ArtifactSource) null;
          if (releaseArtifactSource != null & shouldBePrimary && !linkedArtifact.IsPrimary)
            releaseArtifactSource = (ArtifactSource) null;
          if (releaseArtifactSource != null)
            break;
        }
      }
      return releaseArtifactSource as PipelineArtifactSource;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions while fetching external data should not be propagated")]
    public static IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Change> GetCommitsData(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Release release,
      ReleaseEnvironment currentEnvironment,
      int commitsCount)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      List<Microsoft.TeamFoundation.Build.WebApi.Change> commitsData = new List<Microsoft.TeamFoundation.Build.WebApi.Change>();
      try
      {
        Dictionary<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>> environmentChanges = requestContext.GetService<ReleaseChangesService>().GetReleaseEnvironmentChanges(requestContext, projectInfo.Id, release, currentEnvironment, commitsCount);
        ArtifactSource key = environmentChanges.Keys.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (k => k.IsPrimary && k.IsBuildArtifact));
        IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change> changes;
        if (key == null || !environmentChanges.TryGetValue(key, out changes))
          return (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Change>) commitsData;
        PipelineArtifactSource releaseVersionData = ReleaseWorkItemsCommitsHelper.GetReleaseVersionData(release, (string) null);
        string repositoryId = releaseVersionData == null ? string.Empty : Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetRepositoryIdFromArtifactSource(releaseVersionData.SourceData);
        string repositoryType = releaseVersionData == null ? string.Empty : Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetRepositoryTypeFromArtifactSource(releaseVersionData.SourceData);
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change change1 in changes)
        {
          string uriString = (string) null;
          Uri uri = (Uri) null;
          try
          {
            uriString = WebAccessUrlBuilder.GetCommitWebAccessUri(requestContext, projectInfo.Name, repositoryType, repositoryId, change1.Id);
            uri = new Uri(uriString);
          }
          catch (Exception ex)
          {
            requestContext.Trace(1972003, TraceLevel.Info, "ReleaseManagementService", "Events", "Failed to construct Display Uri for commit using CommitWebAccessUri: {0} for repositoryType: {1}, repositoryId: {2} and buildCommitId: {3}. Exception Details: {4}", (object) uriString, (object) repositoryType, (object) repositoryId, (object) change1.Id, (object) ex);
          }
          Microsoft.TeamFoundation.Build.WebApi.Change change2 = new Microsoft.TeamFoundation.Build.WebApi.Change()
          {
            Id = change1.Id,
            Author = change1.Author,
            Location = change1.Location,
            Message = change1.Message,
            Timestamp = change1.Timestamp,
            DisplayUri = uri
          };
          commitsData.Add(change2);
        }
      }
      catch (ReleaseManagementExternalServiceException ex)
      {
        requestContext.Trace(1972003, TraceLevel.Info, "ReleaseManagementService", "Events", "Failed to fetch commits data from build which can happen if build is deleted. Exception Details {0}", (object) ex);
      }
      return (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Change>) commitsData;
    }

    public static SortedList<int, PipelineArtifactSource> GetReleaseArtifactSources(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Release startRelease,
      Release endRelease,
      PipelineArtifactSource startArtifactSource,
      string artifactAlias)
    {
      if (startArtifactSource == null)
        throw new ArgumentNullException(nameof (startArtifactSource));
      SortedList<int, PipelineArtifactSource> releaseArtifactSources = new SortedList<int, PipelineArtifactSource>();
      if (startArtifactSource.ArtifactTypeId.Equals("Build", StringComparison.OrdinalIgnoreCase))
      {
        releaseArtifactSources.Add(startArtifactSource.ReleaseId, startArtifactSource);
      }
      else
      {
        int num = endRelease.Id - startRelease.Id + 1;
        int top = num > 100 ? 100 : num;
        foreach (Release release in ReleaseWorkItemsCommitsHelper.GetReleaseRange(requestContext, endRelease.Id, endRelease.ReleaseDefinitionId, 0, projectInfo, startArtifactSource.ArtifactTypeId, startArtifactSource.SourceId, top).OrderBy<Release, int>((Func<Release, int>) (r => r.Id)).Where<Release>((Func<Release, bool>) (r => r.Id >= startRelease.Id && r.Id < endRelease.Id)))
        {
          PipelineArtifactSource releaseArtifactSource = ReleaseWorkItemsCommitsHelper.GetReleaseArtifactSource(release, startArtifactSource.ArtifactTypeId, startArtifactSource.SourceId, true, artifactAlias);
          if (releaseArtifactSource != null)
            releaseArtifactSources.Add(releaseArtifactSource.ReleaseId, releaseArtifactSource);
        }
      }
      return releaseArtifactSources;
    }

    public static PipelineArtifactSource GetReleaseVersionData(
      Release release,
      string artifactTypeId)
    {
      return ReleaseWorkItemsCommitsHelper.GetReleaseVersionData(release, artifactTypeId, (string) null);
    }

    public static PipelineArtifactSource GetReleaseVersionData(
      Release release,
      string artifactTypeId,
      string artifactAlias)
    {
      IList<ArtifactSource> source = release != null ? release.LinkedArtifacts : throw new ArgumentNullException(nameof (release));
      if (source == null)
        return (PipelineArtifactSource) null;
      if (source.Count == 0)
        return (PipelineArtifactSource) null;
      return (string.IsNullOrEmpty(artifactAlias) ? (string.IsNullOrEmpty(artifactTypeId) ? (PipelineArtifactSource) ReleaseWorkItemsCommitsHelper.GetPrimaryArtifact((IEnumerable<ArtifactSource>) release.LinkedArtifacts) : (PipelineArtifactSource) source.Where<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => artifact.ArtifactTypeId.Equals(artifactTypeId, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ArtifactSource>()) : (PipelineArtifactSource) source.Where<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => artifact.Alias.Equals(artifactAlias, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ArtifactSource>()) ?? (PipelineArtifactSource) null;
    }

    private static ArtifactSource GetPrimaryArtifact(IEnumerable<ArtifactSource> artifacts)
    {
      foreach (ArtifactSource artifact in artifacts)
      {
        if (artifact != null && artifact.IsPrimary)
          return artifact;
      }
      return (ArtifactSource) null;
    }
  }
}
