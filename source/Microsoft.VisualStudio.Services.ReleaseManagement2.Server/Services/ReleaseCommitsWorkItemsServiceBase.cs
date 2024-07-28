// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseCommitsWorkItemsServiceBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseCommitsWorkItemsServiceBase : ReleaseManagement2ServiceBase
  {
    protected static PipelineArtifactSource GetLastReleaseArtifactSource(
      IList<Release> lastTwoReleases,
      ArtifactSource artifact)
    {
      if (lastTwoReleases == null)
        throw new ArgumentNullException(nameof (lastTwoReleases));
      PipelineArtifactSource releaseArtifactSource = (PipelineArtifactSource) null;
      if (lastTwoReleases.Count != 1)
      {
        ArtifactSource artifactSource = lastTwoReleases.Last<Release>().LinkedArtifacts.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (x => string.Equals(x.SourceId, artifact.SourceId)));
        if (artifactSource != null)
          releaseArtifactSource = artifactSource as PipelineArtifactSource;
      }
      return releaseArtifactSource;
    }

    protected static Release GetLastRelease(
      IVssRequestContext requestContext,
      Guid projectId,
      Release release,
      ReleaseEnvironment releaseEnvironment)
    {
      return ReleaseCommitsWorkItemsServiceBase.GetLastReleases(requestContext, projectId, release, releaseEnvironment, 1).FirstOrDefault<Release>();
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    protected IEnumerable<T> GetArtifactItems<T>(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int startReleaseId,
      int endReleaseId,
      int top,
      string artifactTypeId,
      Func<PipelineArtifactSource, IVssRequestContext, SortedList<int, PipelineArtifactSource>, ProjectInfo, int, int, GetConfig, IEnumerable<T>> getArtifactDetails,
      string artifactAlias,
      GetConfig getConfig)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (startReleaseId == endReleaseId)
        return (IEnumerable<T>) new List<T>();
      if (getArtifactDetails == null)
        throw new ArgumentNullException(nameof (getArtifactDetails));
      ReleaseCommitsWorkItemsServiceBase.SwapReleaseIdIfNeeded(ref startReleaseId, ref endReleaseId);
      using (ReleaseManagementTimer timer = ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseCommitsWorkItemsService.GetArtifactItems", 1976411))
      {
        Release startRelease;
        Release endRelease;
        PipelineArtifactSource endArtifactSource;
        bool isFirstRelease;
        ReleaseCommitsWorkItemsServiceBase.GetReleaseInfo(requestContext, projectInfo, startReleaseId, endReleaseId, artifactTypeId, artifactAlias, timer, out startRelease, out endRelease, out endArtifactSource, out isFirstRelease);
        if (endArtifactSource == null || !ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsCommitsOrWorkItemsTraceability(requestContext, endArtifactSource.ArtifactTypeId))
          return (IEnumerable<T>) new List<T>();
        if (ReleaseCommitsWorkItemsServiceBase.NoReleaseToCompare(startRelease, isFirstRelease))
          return getArtifactDetails(endArtifactSource, requestContext, (SortedList<int, PipelineArtifactSource>) null, projectInfo, endRelease != null ? endRelease.ReleaseDefinitionId : 0, top, getConfig);
        PipelineArtifactSource releaseVersionData = ReleaseWorkItemsCommitsHelper.GetReleaseVersionData(startRelease, artifactTypeId, artifactAlias);
        if (releaseVersionData == null)
        {
          if (artifactAlias == null)
            throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.CannotFindPrimaryArtifact, (object) startReleaseId));
          return getArtifactDetails(endArtifactSource, requestContext, (SortedList<int, PipelineArtifactSource>) null, projectInfo, endRelease != null ? endRelease.ReleaseDefinitionId : 0, top, getConfig);
        }
        if (!ReleaseCommitsWorkItemsServiceBase.AreArtifactSourcesValid(releaseVersionData, endArtifactSource))
          return (IEnumerable<T>) new List<T>();
        SortedList<int, PipelineArtifactSource> artifactSources;
        ReleaseCommitsWorkItemsServiceBase.GetReleaseArtifactSources(requestContext, projectInfo, startRelease, endRelease, releaseVersionData, artifactAlias, timer, out artifactSources);
        IEnumerable<T> artifactItems = getArtifactDetails(endArtifactSource, requestContext, artifactSources, projectInfo, endRelease != null ? endRelease.ReleaseDefinitionId : 0, top, getConfig);
        timer.RecordLap("Service", "Retrieved ArtifaceItems", 1973110);
        return artifactItems;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    protected void LinkArtifactWorkItems(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int startReleaseId,
      int endReleaseId,
      int top,
      string artifactTypeId,
      string artifactAlias,
      LinkConfig linkConfig,
      DeploymentData deploymentData)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (startReleaseId == endReleaseId)
        return;
      ReleaseCommitsWorkItemsServiceBase.SwapReleaseIdIfNeeded(ref startReleaseId, ref endReleaseId);
      using (ReleaseManagementTimer timer = ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseCommitsWorkItemsService.LinkArtifactWorkItems", 1976493))
      {
        Release startRelease;
        Release endRelease;
        PipelineArtifactSource endArtifactSource;
        bool isFirstRelease;
        ReleaseCommitsWorkItemsServiceBase.GetReleaseInfo(requestContext, projectInfo, startReleaseId, endReleaseId, artifactTypeId, artifactAlias, timer, out startRelease, out endRelease, out endArtifactSource, out isFirstRelease);
        if (endArtifactSource == null || !ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsWorkItemsTraceability(requestContext, endArtifactSource.ArtifactTypeId))
          return;
        if (ReleaseCommitsWorkItemsServiceBase.NoReleaseToCompare(startRelease, isFirstRelease))
        {
          endArtifactSource.LinkWorkItems(requestContext, (SortedList<int, PipelineArtifactSource>) null, projectInfo, endRelease != null ? endRelease.ReleaseDefinitionId : 0, top, linkConfig, deploymentData);
        }
        else
        {
          PipelineArtifactSource releaseVersionData = ReleaseWorkItemsCommitsHelper.GetReleaseVersionData(startRelease, artifactTypeId, artifactAlias);
          if (releaseVersionData == null)
          {
            if (artifactAlias == null)
              throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.CannotFindPrimaryArtifact, (object) startReleaseId));
            endArtifactSource.LinkWorkItems(requestContext, (SortedList<int, PipelineArtifactSource>) null, projectInfo, endRelease != null ? endRelease.ReleaseDefinitionId : 0, top, linkConfig, deploymentData);
          }
          else
          {
            if (!ReleaseCommitsWorkItemsServiceBase.AreArtifactSourcesValid(releaseVersionData, endArtifactSource))
              return;
            SortedList<int, PipelineArtifactSource> artifactSources;
            ReleaseCommitsWorkItemsServiceBase.GetReleaseArtifactSources(requestContext, projectInfo, startRelease, endRelease, releaseVersionData, artifactAlias, timer, out artifactSources);
            endArtifactSource.LinkWorkItems(requestContext, artifactSources, projectInfo, endRelease != null ? endRelease.ReleaseDefinitionId : 0, top, linkConfig, deploymentData);
            timer.RecordLap("Service", "Linked ArtifactWorkItems", 1976494);
          }
        }
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "List is fine here.")]
    public List<Release> GetLastTwoReleases(
      IVssRequestContext requestContext,
      Guid projectId,
      Release release,
      ReleaseEnvironment releaseEnvironment,
      bool includeEnvironments = false,
      string sourceBranchFilter = "")
    {
      return ReleaseCommitsWorkItemsServiceBase.GetLastReleases(requestContext, projectId, release, releaseEnvironment, 2, includeEnvironments, sourceBranchFilter);
    }

    private static List<Release> GetLastReleases(
      IVssRequestContext requestContext,
      Guid projectId,
      Release release,
      ReleaseEnvironment releaseEnvironment,
      int releaseCount,
      bool includeEnvironments = false,
      string sourceBranchFilter = "")
    {
      return requestContext.GetService<ReleasesService>().ListReleases(requestContext, projectId, release.ReleaseDefinitionId, releaseEnvironment.DefinitionEnvironmentId, string.Empty, string.Empty, ReleaseStatus.Active, ReleaseEnvironmentStatus.Undefined, new DateTime?(), new DateTime?(), new DateTime?(), ReleaseQueryOrder.IdDescending, releaseCount, release.Id, true, false, false, false, includeEnvironments, false, string.Empty, string.Empty, string.Empty, sourceBranchFilter, false, false, (IEnumerable<string>) null).ToList<Release>();
    }

    private static void GetReleaseArtifactSources(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Release startRelease,
      Release endRelease,
      PipelineArtifactSource startArtifactSource,
      string artifactAlias,
      ReleaseManagementTimer timer,
      out SortedList<int, PipelineArtifactSource> artifactSources)
    {
      timer.RecordLap("Service", "Got release version data for start artifact", 1976411);
      artifactSources = ReleaseWorkItemsCommitsHelper.GetReleaseArtifactSources(requestContext, projectInfo, startRelease, endRelease, startArtifactSource, artifactAlias);
      timer.RecordLap("Service", "Found list of release artifact sources", 1976411);
    }

    private static bool NoReleaseToCompare(Release startRelease, bool isFirstRelease) => startRelease == null | isFirstRelease;

    private static void SwapReleaseIdIfNeeded(ref int startReleaseId, ref int endReleaseId)
    {
      if (startReleaseId <= endReleaseId)
        return;
      int num = startReleaseId;
      startReleaseId = endReleaseId;
      endReleaseId = num;
    }

    private static void GetReleaseInfo(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int startReleaseId,
      int endReleaseId,
      string artifactTypeId,
      string artifactAlias,
      ReleaseManagementTimer timer,
      out Release startRelease,
      out Release endRelease,
      out PipelineArtifactSource endArtifactSource,
      out bool isFirstRelease)
    {
      ReleaseWorkItemsCommitsHelper.GetReleaseRange(requestContext, projectInfo, startReleaseId, endReleaseId, out startRelease, out endRelease, out isFirstRelease);
      timer.RecordLap("Service", "Got release range", 1976411);
      if (startRelease != null && endRelease != null && startRelease.ReleaseDefinitionId != endRelease.ReleaseDefinitionId)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ReleasesFromDifferentDefinitionCannotBeCompared);
      endArtifactSource = ReleaseWorkItemsCommitsHelper.GetReleaseVersionData(endRelease, artifactTypeId, artifactAlias);
      timer.RecordLap("Service", "Got release version data for end artifact", 1976411);
      if (!string.IsNullOrEmpty(artifactAlias) && endArtifactSource == null)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.CannotFindArtifactWithAlias, (object) artifactAlias));
    }

    private static bool AreArtifactSourcesValid(
      PipelineArtifactSource startArtifactSource,
      PipelineArtifactSource endArtifactSource)
    {
      if (!string.Equals(startArtifactSource.ArtifactTypeId, endArtifactSource.ArtifactTypeId, StringComparison.OrdinalIgnoreCase))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ReleaseComparisionWithIncompatibleArtifactTypeException, (object) startArtifactSource.ArtifactTypeId, (object) endArtifactSource.ArtifactTypeId));
      InputValue inputValue1;
      InputValue inputValue2;
      if (startArtifactSource.SourceData == null || !startArtifactSource.SourceData.TryGetValue("definition", out inputValue1) || endArtifactSource.SourceData == null || !endArtifactSource.SourceData.TryGetValue("definition", out inputValue2) || !string.Equals(inputValue1.Value, inputValue2.Value, StringComparison.OrdinalIgnoreCase))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ReleaseComparisionWithIncompatibleArtifactSourceDefinition));
      return true;
    }
  }
}
