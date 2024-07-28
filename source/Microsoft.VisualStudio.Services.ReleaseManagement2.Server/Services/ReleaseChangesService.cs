// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseChangesService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseChangesService : ReleaseCommitsWorkItemsServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change> GetReleaseChanges(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int startReleaseId,
      int endReleaseId,
      int top)
    {
      return this.GetReleaseChanges(requestContext, projectInfo, startReleaseId, endReleaseId, top, (string) null);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change> GetReleaseChanges(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int startReleaseId,
      int endReleaseId,
      int top,
      string artifactAlias)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.GetArtifactItems<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>(requestContext, projectInfo, startReleaseId, endReleaseId, top, (string) null, ReleaseChangesService.\u003C\u003EO.\u003C0\u003E__GetChanges ?? (ReleaseChangesService.\u003C\u003EO.\u003C0\u003E__GetChanges = new Func<PipelineArtifactSource, IVssRequestContext, SortedList<int, PipelineArtifactSource>, ProjectInfo, int, int, GetConfig, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>>(ArtifactSourceExtensions.GetChanges)), artifactAlias, (GetConfig) null);
    }

    public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change> GetChangesFromLastRelease(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Release release,
      ReleaseEnvironment currentEnvironment,
      int top)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (currentEnvironment == null)
        throw new ArgumentNullException(nameof (currentEnvironment));
      Release lastRelease = ReleaseCommitsWorkItemsServiceBase.GetLastRelease(requestContext, projectInfo.Id, release, currentEnvironment);
      return lastRelease != null ? this.GetReleaseChanges(requestContext, projectInfo, lastRelease.Id, release.Id, top) : (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>();
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is the intended design")]
    public Dictionary<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>> GetReleaseEnvironmentChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      Release release,
      ReleaseEnvironment releaseEnvironment,
      int top)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      ArtifactSource artifactSource = release.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.IsPrimary)).FirstOrDefault<ArtifactSource>();
      string sourceBranchFilter = string.Empty;
      if (artifactSource != null && artifactSource.SourceBranch != null)
        sourceBranchFilter = artifactSource.SourceBranch;
      List<Release> lastTwoReleases = this.GetLastTwoReleases(requestContext, projectId, release, releaseEnvironment, sourceBranchFilter: sourceBranchFilter);
      return ReleaseChangesService.GetChangesBetweenReleases(requestContext, projectId, lastTwoReleases, top);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static Dictionary<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>> GetChangesBetweenReleases(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Release> lastTwoReleases,
      int top)
    {
      Dictionary<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>> changesBetweenReleases = new Dictionary<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>>();
      int num = 0;
      if (lastTwoReleases.Any<Release>())
      {
        foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) lastTwoReleases.First<Release>().LinkedArtifacts)
        {
          if ((linkedArtifact.IsBuildArtifact || linkedArtifact.IsGitHubArtifact) && linkedArtifact is PipelineArtifactSource currentPipelineArtifactSource && (!currentPipelineArtifactSource.IsBuildArtifact || currentPipelineArtifactSource.VersionId != 0))
          {
            PipelineArtifactSource releaseArtifactSource = ReleaseCommitsWorkItemsServiceBase.GetLastReleaseArtifactSource((IList<Release>) lastTwoReleases, linkedArtifact);
            IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> betweenArtifactSource = requestContext.GetService<ArtifactTypeServiceBase>().GetArtifactType(requestContext, currentPipelineArtifactSource.ArtifactTypeId).GetChangesBetweenArtifactSource(requestContext, projectId, currentPipelineArtifactSource, releaseArtifactSource, top);
            changesBetweenReleases[linkedArtifact] = ReleaseChangesService.ToReleaseChange(betweenArtifactSource);
            num += betweenArtifactSource.Count;
          }
        }
        if (lastTwoReleases.Count > 1)
          requestContext.Trace(1900037, TraceLevel.Info, "ReleaseManagementService", "Service", "The last two releases for the PublishCommittersForEnvironmentCompletedEvent are Release {0} and Release {1}. The number of commits between them is {2}", (object) lastTwoReleases.FirstOrDefault<Release>().Id, (object) lastTwoReleases.Last<Release>().Id, (object) num);
      }
      return changesBetweenReleases;
    }

    private static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change> ToReleaseChange(
      IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> changes)
    {
      return changes == null ? (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>) null : changes.Select<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>((Func<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>) (x => new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change()
      {
        Id = x.Id,
        Author = x.Author,
        ChangeType = x.ChangeType,
        DisplayUri = x.DisplayUri,
        Location = x.Location,
        Message = x.Message,
        PushedBy = x.PushedBy,
        Timestamp = x.Timestamp
      }));
    }
  }
}
