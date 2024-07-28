// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ArtifactSourceExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to use the types")]
  public static class ArtifactSourceExtensions
  {
    public static Func<IVssRequestContext, Guid, Guid, string, TaskAttachment> GetCommitsAttachmentFromPlanFunc { get; set; }

    public static Func<IVssRequestContext, Guid, Guid, string, TaskAttachment> GetWorkItemsAttachmentFromPlanFunc { get; set; }

    public static Func<IVssRequestContext, Guid, TaskHub, Guid, TaskAttachment, Stream> GetAttachmentAsStreamFunc { get; set; }

    static ArtifactSourceExtensions()
    {
      ArtifactSourceExtensions.GetCommitsAttachmentFromPlanFunc = new Func<IVssRequestContext, Guid, Guid, string, TaskAttachment>(ArtifactSourceExtensions.GetCommitsAttachmentFromPlan);
      ArtifactSourceExtensions.GetWorkItemsAttachmentFromPlanFunc = new Func<IVssRequestContext, Guid, Guid, string, TaskAttachment>(ArtifactSourceExtensions.GetWorkItemsAttachmentFromPlan);
      ArtifactSourceExtensions.GetAttachmentAsStreamFunc = new Func<IVssRequestContext, Guid, TaskHub, Guid, TaskAttachment, Stream>(ArtifactSourceExtensions.GetAttachmentAsStream);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static void UpdateReleaseArtifactCommitsWorkItemsReference(
      this PipelineArtifactSource artifactSource,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.ReleaseArtifact releaseArtifact,
      Guid planId)
    {
      if (artifactSource == null)
        throw new ArgumentNullException(nameof (artifactSource));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (releaseArtifact == null)
        throw new ArgumentNullException(nameof (releaseArtifact));
      if (!artifactSource.IsPrimary || !ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsCommitsTraceability(requestContext, artifactSource.ArtifactTypeId) || artifactSource.SourceData.ContainsKey("ArtifactDetailsReference") || !artifactSource.IsCommitsUploadedByAgent(requestContext, releaseArtifact.ProjectId, planId))
        return;
      if (ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsWorkItemsTraceability(requestContext, artifactSource.ArtifactTypeId) && !artifactSource.IsWorkItemsUploadedByAgent(requestContext, releaseArtifact.ProjectId, planId))
      {
        requestContext.Trace(1976409, TraceLevel.Info, "ReleaseManagementService", "Service", "Cannot find workitems attachment for the plan {0}", (object) planId);
      }
      else
      {
        artifactSource.SourceData["ArtifactDetailsReference"] = new InputValue()
        {
          Value = planId.ToString(),
          DisplayValue = planId.ToString()
        };
        List<PipelineArtifactSource> sources = new List<PipelineArtifactSource>()
        {
          artifactSource
        };
        Action<ReleaseSqlComponent> action = (Action<ReleaseSqlComponent>) (component => component.UpdateReleaseArtifactSources(releaseArtifact.ProjectId, releaseArtifact.ReleaseId, (IEnumerable<PipelineArtifactSource>) sources));
        requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent>(action);
      }
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change> GetChanges(
      this PipelineArtifactSource endArtifactSource,
      IVssRequestContext requestContext,
      SortedList<int, PipelineArtifactSource> previousArtifactSources,
      ProjectInfo projectInfo,
      int releaseDefinitionId,
      int top,
      GetConfig getConfig)
    {
      if (endArtifactSource == null)
        throw new ArgumentNullException(nameof (endArtifactSource));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (!string.IsNullOrEmpty(endArtifactSource.ArtifactTypeId) && (endArtifactSource.ArtifactTypeId.Equals("Build", StringComparison.OrdinalIgnoreCase) || endArtifactSource.ArtifactTypeId.Equals("GitHub", StringComparison.OrdinalIgnoreCase) || endArtifactSource.ArtifactTypeId.Equals("Git", StringComparison.OrdinalIgnoreCase)))
      {
        ArtifactTypeBase artifactType = requestContext.GetService<ArtifactTypeServiceBase>().GetArtifactType(requestContext, endArtifactSource.ArtifactTypeId);
        InputValue startVersion = (InputValue) null;
        if (previousArtifactSources != null && previousArtifactSources.Any<KeyValuePair<int, PipelineArtifactSource>>())
          startVersion = previousArtifactSources.First<KeyValuePair<int, PipelineArtifactSource>>().Value.Version;
        ProjectInfo projectInfo1 = new ProjectInfo();
        InputValue inputValue;
        if (endArtifactSource.SourceData.TryGetValue("project", out inputValue) && inputValue != null)
        {
          projectInfo1.Id = new Guid(inputValue.Value);
          projectInfo1.Name = inputValue.DisplayValue;
        }
        else
          projectInfo1 = projectInfo;
        object artifactContext;
        if (!endArtifactSource.IsInternalArtifact(requestContext, projectInfo1.Id, out artifactContext) && !ArtifactSourceExtensions.HasViewExternalArtifactCommitsAndWorkItemsPermission(requestContext, projectInfo.Id, releaseDefinitionId))
        {
          bool? nullable = artifactType.UsesExternalAndPublicSourceRepo(requestContext, endArtifactSource.SourceData, projectInfo1.Id, artifactContext);
          bool flag = true;
          if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
            goto label_15;
        }
        return ArtifactSourceExtensions.ToReleaseChange(artifactType.GetChanges(requestContext, endArtifactSource.SourceData, startVersion, endArtifactSource.Version, projectInfo1, top, artifactContext));
      }
label_15:
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsCommitsTraceability(requestContext, endArtifactSource.ArtifactTypeId) ? ArtifactSourceExtensions.GetArtifactDetails<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>(requestContext, endArtifactSource, previousArtifactSources, projectInfo, releaseDefinitionId, top, ArtifactSourceExtensions.GetCommitsAttachmentFromPlanFunc, (Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change, string>) (change => change.Id), ArtifactSourceExtensions.\u003C\u003EO.\u003C0\u003E__FilterPublicChanges ?? (ArtifactSourceExtensions.\u003C\u003EO.\u003C0\u003E__FilterPublicChanges = new Func<IVssRequestContext, ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>>(ArtifactSourceExtensions.FilterPublicChanges))) : (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>();
    }

    private static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change> ToReleaseChange(
      IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> changes)
    {
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>) changes.Select<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>((Func<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>) (x => new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change()
      {
        Id = x.Id,
        Author = x.Author,
        ChangeType = x.ChangeType,
        DisplayUri = x.DisplayUri,
        Location = x.Location,
        Message = x.Message,
        PushedBy = x.PushedBy,
        Timestamp = x.Timestamp
      })).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>();
    }

    public static IEnumerable<ReleaseWorkItemRef> GetWorkItems(
      this PipelineArtifactSource endArtifactSource,
      IVssRequestContext requestContext,
      SortedList<int, PipelineArtifactSource> previousArtifactSources,
      ProjectInfo projectInfo,
      int releaseDefinitionId,
      int top,
      GetConfig getConfig)
    {
      if (endArtifactSource == null)
        throw new ArgumentNullException(nameof (endArtifactSource));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (endArtifactSource.IsBuildArtifact)
      {
        ArtifactTypeBase artifactType;
        InputValue startArtifactVersion;
        ProjectInfo artifactSourceProjectInfo;
        ArtifactSourceExtensions.GetArtifactInfo(requestContext, endArtifactSource, previousArtifactSources, projectInfo, out artifactType, out startArtifactVersion, out artifactSourceProjectInfo);
        object artifactContext;
        if (ArtifactSourceExtensions.HasPermissionForWorkItems(endArtifactSource, requestContext, artifactType, artifactSourceProjectInfo, projectInfo, releaseDefinitionId, out artifactContext))
          return ArtifactSourceExtensions.ToReleaseWorkItemRef(artifactType.GetWorkItems(requestContext, endArtifactSource.SourceData, startArtifactVersion, endArtifactSource.Version, artifactSourceProjectInfo, top, artifactContext, getConfig));
      }
      return ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsWorkItemsTraceability(requestContext, endArtifactSource.ArtifactTypeId) ? ArtifactSourceExtensions.GetArtifactDetails<ReleaseWorkItemRef>(requestContext, endArtifactSource, previousArtifactSources, projectInfo, releaseDefinitionId, top, ArtifactSourceExtensions.GetWorkItemsAttachmentFromPlanFunc, (Func<ReleaseWorkItemRef, string>) (workitemRef => workitemRef.Id), (Func<IVssRequestContext, ArtifactSource, IEnumerable<ReleaseWorkItemRef>, IEnumerable<ReleaseWorkItemRef>>) ((context, source, arg3) => (IEnumerable<ReleaseWorkItemRef>) new List<ReleaseWorkItemRef>())) : (IEnumerable<ReleaseWorkItemRef>) new List<ReleaseWorkItemRef>();
    }

    public static void LinkWorkItems(
      this PipelineArtifactSource endArtifactSource,
      IVssRequestContext requestContext,
      SortedList<int, PipelineArtifactSource> previousArtifactSources,
      ProjectInfo projectInfo,
      int releaseDefinitionId,
      int top,
      LinkConfig linkConfig,
      DeploymentData deploymentData)
    {
      if (endArtifactSource == null)
        throw new ArgumentNullException(nameof (endArtifactSource));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (!endArtifactSource.IsBuildArtifact)
        return;
      ArtifactTypeBase artifactType;
      InputValue startArtifactVersion;
      ProjectInfo artifactSourceProjectInfo;
      ArtifactSourceExtensions.GetArtifactInfo(requestContext, endArtifactSource, previousArtifactSources, projectInfo, out artifactType, out startArtifactVersion, out artifactSourceProjectInfo);
      object artifactContext;
      if (!ArtifactSourceExtensions.HasPermissionForWorkItems(endArtifactSource, requestContext, artifactType, artifactSourceProjectInfo, projectInfo, releaseDefinitionId, out artifactContext))
        return;
      artifactType.LinkDeploymentToWorkItems(requestContext, endArtifactSource.SourceData, startArtifactVersion, endArtifactSource.Version, artifactSourceProjectInfo, projectInfo, top, artifactContext, linkConfig, deploymentData);
    }

    private static bool HasPermissionForWorkItems(
      PipelineArtifactSource endArtifactSource,
      IVssRequestContext requestContext,
      ArtifactTypeBase artifactType,
      ProjectInfo artifactSourceProjectInfo,
      ProjectInfo projectInfo,
      int releaseDefinitionId,
      out object artifactContext)
    {
      if (endArtifactSource.IsInternalArtifact(requestContext, artifactSourceProjectInfo.Id, out artifactContext) || ArtifactSourceExtensions.HasViewExternalArtifactCommitsAndWorkItemsPermission(requestContext, projectInfo.Id, releaseDefinitionId))
        return true;
      bool? nullable = artifactType.UsesExternalAndPublicSourceRepo(requestContext, endArtifactSource.SourceData, artifactSourceProjectInfo.Id, artifactContext);
      bool flag = true;
      return nullable.GetValueOrDefault() == flag & nullable.HasValue;
    }

    private static void GetArtifactInfo(
      IVssRequestContext requestContext,
      PipelineArtifactSource endArtifactSource,
      SortedList<int, PipelineArtifactSource> previousArtifactSources,
      ProjectInfo projectInfo,
      out ArtifactTypeBase artifactType,
      out InputValue startArtifactVersion,
      out ProjectInfo artifactSourceProjectInfo)
    {
      ArtifactTypeServiceBase service = requestContext.GetService<ArtifactTypeServiceBase>();
      artifactType = service.GetArtifactType(requestContext, endArtifactSource.ArtifactTypeId);
      startArtifactVersion = (InputValue) null;
      if (previousArtifactSources != null && previousArtifactSources.Any<KeyValuePair<int, PipelineArtifactSource>>())
        startArtifactVersion = previousArtifactSources.First<KeyValuePair<int, PipelineArtifactSource>>().Value.Version;
      artifactSourceProjectInfo = new ProjectInfo();
      InputValue inputValue;
      if (endArtifactSource.SourceData.TryGetValue("project", out inputValue) && inputValue != null)
      {
        artifactSourceProjectInfo.Id = new Guid(inputValue.Value);
        artifactSourceProjectInfo.Name = inputValue.DisplayValue;
      }
      else
        artifactSourceProjectInfo = projectInfo;
    }

    private static IEnumerable<ReleaseWorkItemRef> ToReleaseWorkItemRef(
      IList<WorkItemRef> workItemRef)
    {
      return workItemRef == null ? (IEnumerable<ReleaseWorkItemRef>) null : (IEnumerable<ReleaseWorkItemRef>) workItemRef.Select<WorkItemRef, ReleaseWorkItemRef>((Func<WorkItemRef, ReleaseWorkItemRef>) (x => new ReleaseWorkItemRef()
      {
        Id = x.Id,
        State = x.State,
        Title = x.Title,
        Url = x.Url,
        Provider = x.Provider
      })).ToList<ReleaseWorkItemRef>();
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to continue on error")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Since default is false, we are okay if the value is ignored")]
    public static void GetLatestArtifactVersions(
      this IEnumerable<ArtifactSource> sources,
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Dictionary<string, InputValue> latestArtifactVersions,
      bool failIfDefaultVersionTypeIsSelectDuringReleaseCreation,
      bool retryOnFailure = false)
    {
      if (sources == null)
        throw new ArgumentNullException(nameof (sources));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (latestArtifactVersions == null)
        throw new ArgumentNullException(nameof (latestArtifactVersions));
      ArtifactVersionsService service = requestContext.GetService<ArtifactVersionsService>();
      foreach (ArtifactSource source in sources)
      {
        bool flag = true;
        InputValue inputValue1;
        if (failIfDefaultVersionTypeIsSelectDuringReleaseCreation && source.SourceData != null && source.SourceData.TryGetValue("defaultVersionType", out inputValue1) && inputValue1 != null && string.Equals(inputValue1.Value, "selectDuringReleaseCreationType", StringComparison.OrdinalIgnoreCase))
        {
          string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.CannotGetArtifactVersionForSelectDuringReleaseCreationDefaultVersionType, (object) source.Alias);
          ArtifactSourceExtensions.TraceArtifactUnavailableError(requestContext, source.ArtifactTypeId, str);
          throw new ArtifactVersionUnavailableException(str);
        }
        InputValue inputValue2 = (InputValue) null;
        try
        {
          if (string.Equals(source.ArtifactTypeId, "PackageManagement", StringComparison.OrdinalIgnoreCase))
          {
            Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity = ServiceIdentityProvisioner.GetServiceIdentity(requestContext, ReleaseAuthorizationScope.ProjectCollection, projectInfo.Id, true);
            using (IVssRequestContext requestContext1 = requestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(requestContext, requestContext.ServiceHost.InstanceId, serviceIdentity.Descriptor, true))
            {
              int num = 0;
              while (((num++ == 3 ? 0 : (inputValue2 == null ? 1 : 0)) & (flag ? 1 : 0)) != 0)
              {
                try
                {
                  inputValue2 = service.GetLatestArtifactVersion(requestContext1, projectInfo, source);
                }
                catch (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException ex)
                {
                  ArtifactSourceExtensions.TraceArtifactUnavailableError(requestContext, source.ArtifactTypeId, ex.Message);
                  flag &= retryOnFailure;
                }
              }
            }
          }
          else if (ArtifactTypeUtility.IsCustomArtifact(requestContext, source.ArtifactTypeId))
          {
            int num = 0;
            while (((num++ == 3 ? 0 : (inputValue2 == null ? 1 : 0)) & (flag ? 1 : 0)) != 0)
            {
              try
              {
                inputValue2 = service.GetLatestArtifactVersion(requestContext, projectInfo, source);
              }
              catch (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException ex)
              {
                ArtifactSourceExtensions.TraceArtifactUnavailableError(requestContext, source.ArtifactTypeId, ex.Message);
                flag &= retryOnFailure;
              }
            }
          }
          else
            inputValue2 = service.GetLatestArtifactVersion(requestContext, projectInfo, source);
          latestArtifactVersions[source.Alias] = inputValue2 != null ? inputValue2 : throw new ArtifactVersionUnavailableException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.LatestArtifactVersionUnavailable, (object) source.Alias));
        }
        catch (Exception ex)
        {
          string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.LatestArtifactVersionUnavailable, (object) source.Alias);
          ArtifactSourceExtensions.TraceArtifactUnavailableError(requestContext, source.ArtifactTypeId, ex.Message);
          Exception innerException = ex;
          throw new ArtifactVersionUnavailableException(message, innerException);
        }
      }
    }

    public static string GetPreviousReleaseArtifactVersion(
      this ArtifactSource artifactSource,
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      if (artifactSource == null)
        throw new ArgumentNullException(nameof (artifactSource));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      string empty = string.Empty;
      ProjectInfo projectInfo = new ProjectInfo()
      {
        Id = projectId
      };
      Release release = ReleaseWorkItemsCommitsHelper.GetRelease(requestContext, projectInfo, releaseId);
      if (release != null)
      {
        Release previousRelease = ReleaseWorkItemsCommitsHelper.GetPreviousRelease(requestContext, projectInfo, release, artifactSource.ArtifactTypeId, artifactSource.SourceId);
        if (previousRelease != null)
        {
          PipelineArtifactSource releaseArtifactSource = ReleaseWorkItemsCommitsHelper.GetReleaseArtifactSource(previousRelease, artifactSource.ArtifactTypeId, artifactSource.SourceId, true);
          if (releaseArtifactSource != null)
            empty = releaseArtifactSource.VersionId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          else
            ArtifactSourceExtensions.TraceInfo(requestContext, 1976404, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Artifact source {0} is not primary in release {1}. Commits will be fetched only for current release artifact", (object) artifactSource.SourceId, (object) release.Id));
        }
        else
          ArtifactSourceExtensions.TraceInfo(requestContext, 1976403, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Previous release is not found for sourceId {0}, commits will be downloaded for only target release", (object) artifactSource.SourceId));
      }
      else
        ArtifactSourceExtensions.TraceError(requestContext, 1976401, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Target release {0} not found. Not sending commit version option to download commits", (object) releaseId));
      return empty;
    }

    private static string GetGitHubRepoUrl(Uri changeDisplayUri)
    {
      string gitHubRepoUrl = string.Empty;
      if (changeDisplayUri != (Uri) null)
      {
        string absoluteUri = changeDisplayUri.AbsoluteUri;
        if (absoluteUri != null && absoluteUri.StartsWith("https://github.com", StringComparison.OrdinalIgnoreCase))
        {
          int startIndex = absoluteUri.LastIndexOf("/commit", StringComparison.OrdinalIgnoreCase);
          gitHubRepoUrl = startIndex < 0 ? string.Empty : absoluteUri.Remove(startIndex).Replace("github.com", "api.github.com/repos");
        }
      }
      return gitHubRepoUrl;
    }

    private static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change> FilterPublicChanges(
      IVssRequestContext requestContext,
      ArtifactSource artifactSource,
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change> changes)
    {
      if (artifactSource == null || changes == null)
        return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>();
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change> changeList = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>();
      if (string.Equals(artifactSource.ArtifactTypeId, "Jenkins", StringComparison.OrdinalIgnoreCase))
      {
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change change in changes)
        {
          string gitHubRepoUrl = ArtifactSourceExtensions.GetGitHubRepoUrl(change.DisplayUri);
          if (!string.IsNullOrEmpty(gitHubRepoUrl))
          {
            if (dictionary.ContainsKey(gitHubRepoUrl))
            {
              if (dictionary[gitHubRepoUrl])
                changeList.Add(change);
            }
            else
            {
              dictionary[gitHubRepoUrl] = false;
              GitHubResult<GitHubData.V3.Repository> repo = GitHubHttpClientFactory.Create(requestContext).GetRepo(new GitHubAuthentication(GitHubAuthScheme.None, string.Empty), gitHubRepoUrl);
              if (repo.IsSuccessful)
              {
                GitHubData.V3.Repository result = repo.Result;
                if (result != null && !result.Private)
                {
                  dictionary[gitHubRepoUrl] = true;
                  changeList.Add(change);
                }
              }
            }
          }
        }
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>) changeList;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "requestContext", Justification = "Will be required when the permission bit gets added")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "releaseDefinitionId", Justification = "Will be required when the permission bit gets added")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "projectId", Justification = "Will be required when the permission bit gets added")]
    private static bool HasViewExternalArtifactCommitsAndWorkItemsPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, SecurityConstants.ReleaseManagementUISecurityNamespaceId);
      string uiPermissionToken = SecurityConstants.ReleaseManagementUIPermissionToken;
      IVssRequestContext requestContext1 = requestContext;
      string token = uiPermissionToken;
      return securityNamespace.HasPermission(requestContext1, token, 32, false);
    }

    private static IEnumerable<T> GetArtifactDetails<T>(
      IVssRequestContext requestContext,
      PipelineArtifactSource endArtifactSource,
      SortedList<int, PipelineArtifactSource> previousArtifactSources,
      ProjectInfo projectInfo,
      int releaseDefinitionId,
      int top,
      Func<IVssRequestContext, Guid, Guid, string, TaskAttachment> getArtifactItemsAttachmentFromPlan,
      Func<T, string> getArtifactItemId,
      Func<IVssRequestContext, ArtifactSource, IEnumerable<T>, IEnumerable<T>> getFilteredArtifactItems)
    {
      SortedList<int, PipelineArtifactSource> sortedList = new SortedList<int, PipelineArtifactSource>();
      if (previousArtifactSources != null && previousArtifactSources.Any<KeyValuePair<int, PipelineArtifactSource>>())
        sortedList.AddRange<KeyValuePair<int, PipelineArtifactSource>, SortedList<int, PipelineArtifactSource>>((IEnumerable<KeyValuePair<int, PipelineArtifactSource>>) previousArtifactSources);
      sortedList.Add(endArtifactSource.ReleaseId, endArtifactSource);
      IDictionary<string, int> rollbackItemReferences = (IDictionary<string, int>) new Dictionary<string, int>();
      IDictionary<string, int> forwardItemReferences = (IDictionary<string, int>) new Dictionary<string, int>();
      List<T> forwardItems = new List<T>();
      List<T> rollbackItems = new List<T>();
      List<T> resultItems = new List<T>();
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "ArtifactSource.GetArtifactDetails", 1976400))
      {
        TaskHub releaseTaskHub = requestContext.GetService<IDistributedTaskHubService>().GetReleaseTaskHub(requestContext);
        bool flag = ArtifactSourceExtensions.HasViewExternalArtifactCommitsAndWorkItemsPermission(requestContext, projectInfo.Id, releaseDefinitionId);
        if (sortedList.Count == 1)
        {
          Func<T, bool> onArtifactItemFound = (Func<T, bool>) (item =>
          {
            resultItems.Add(item);
            return resultItems.Count < top;
          });
          ArtifactSourceExtensions.ReadArtifactDetailsAttachment<T>(requestContext, projectInfo, endArtifactSource, releaseTaskHub, getArtifactItemsAttachmentFromPlan, onArtifactItemFound);
          return flag ? (IEnumerable<T>) resultItems : getFilteredArtifactItems(requestContext, (ArtifactSource) endArtifactSource, (IEnumerable<T>) resultItems);
        }
        for (int index = sortedList.Count<KeyValuePair<int, PipelineArtifactSource>>() - 1; index > 0; --index)
        {
          PipelineArtifactSource artifactSource = sortedList.ElementAt<KeyValuePair<int, PipelineArtifactSource>>(index).Value;
          PipelineArtifactSource previousArtifactSource = sortedList.ElementAt<KeyValuePair<int, PipelineArtifactSource>>(index - 1).Value;
          Func<T, bool> onArtifactItemFound = (Func<T, bool>) (item =>
          {
            if (artifactSource.VersionId < previousArtifactSource.VersionId)
              ArtifactSourceExtensions.UpdateArtifactDetailReference<T>(item, rollbackItems, rollbackItemReferences, getArtifactItemId);
            else
              ArtifactSourceExtensions.UpdateArtifactDetailReference<T>(item, forwardItems, forwardItemReferences, getArtifactItemId);
            return Math.Abs(forwardItems.Count - rollbackItems.Count) < top;
          });
          ArtifactSourceExtensions.ReadArtifactDetailsAttachment<T>(requestContext, projectInfo, artifactSource, releaseTaskHub, getArtifactItemsAttachmentFromPlan, onArtifactItemFound);
        }
        releaseManagementTimer.RecordLap("Service", "Read all the items from attachments", 1976400);
        if (((IEnumerable<T>) forwardItems).Any<T>() && !((IEnumerable<T>) rollbackItems).Any<T>())
          resultItems.AddRange((IEnumerable<T>) forwardItems);
        else if (!((IEnumerable<T>) forwardItems).Any<T>() && ((IEnumerable<T>) rollbackItems).Any<T>())
          resultItems.AddRange((IEnumerable<T>) rollbackItems);
        else if (((IEnumerable<T>) forwardItems).Any<T>() && ((IEnumerable<T>) rollbackItems).Any<T>())
        {
          if (sortedList.Last<KeyValuePair<int, PipelineArtifactSource>>().Value.VersionId > sortedList.First<KeyValuePair<int, PipelineArtifactSource>>().Value.VersionId)
            resultItems.AddRange((IEnumerable<T>) ArtifactSourceExtensions.FilterChanges<T>(rollbackItemReferences, forwardItems, getArtifactItemId));
          else
            resultItems.AddRange((IEnumerable<T>) ArtifactSourceExtensions.FilterChanges<T>(forwardItemReferences, rollbackItems, getArtifactItemId));
        }
        return flag ? (IEnumerable<T>) resultItems : getFilteredArtifactItems(requestContext, (ArtifactSource) endArtifactSource, (IEnumerable<T>) resultItems);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "we like this name")]
    private static void ReadArtifactDetailsAttachment<T>(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      PipelineArtifactSource artifactSource,
      TaskHub taskHub,
      Func<IVssRequestContext, Guid, Guid, string, TaskAttachment> getArtifactItemsAttachmentFromPlan,
      Func<T, bool> onArtifactItemFound)
    {
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "ArtifactSource.ReadArtifactDetailsAttachment", 1976410))
      {
        Guid result;
        if (!artifactSource.SourceData.ContainsKey("ArtifactDetailsReference") || !Guid.TryParse(artifactSource.SourceData["ArtifactDetailsReference"].Value, out result))
          return;
        TaskAttachment taskAttachment = getArtifactItemsAttachmentFromPlan(requestContext, projectInfo.Id, result, artifactSource.Alias);
        string lapName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Got attachment for {0}", (object) result);
        releaseManagementTimer.RecordLap("Service", lapName, 1976410);
        if (taskAttachment != null)
        {
          Stream stream = ArtifactSourceExtensions.GetAttachmentAsStreamFunc(requestContext, projectInfo.Id, taskHub, result, taskAttachment);
          releaseManagementTimer.RecordLap("Service", "Got attachment stream", 1976410);
          if (stream == null)
            return;
          using (JsonTextReader reader = new JsonTextReader((TextReader) new StreamReader(stream)))
          {
            while (reader.Read())
            {
              if (reader.TokenType == JsonToken.StartObject)
              {
                T obj = JsonConvert.DeserializeObject<T>(JObject.Load((JsonReader) reader).ToString());
                if (!onArtifactItemFound(obj))
                  break;
              }
            }
          }
        }
        else
          requestContext.Trace(1976405, TraceLevel.Error, "ReleaseManagementService", "Service", "Cannot find artifact details attachment for the plan {0}", (object) result);
      }
    }

    private static List<T> FilterChanges<T>(
      IDictionary<string, int> changeReferencesToRemove,
      List<T> changes,
      Func<T, string> getId)
    {
      changes.RemoveAll((Predicate<T>) (x =>
      {
        string key = getId(x);
        if (!changeReferencesToRemove.ContainsKey(key))
          return false;
        if (changeReferencesToRemove[key] > 1)
          --changeReferencesToRemove[key];
        else
          changeReferencesToRemove.Remove(key);
        return true;
      }));
      return changes;
    }

    private static void UpdateArtifactDetailReference<T>(
      T item,
      List<T> items,
      IDictionary<string, int> changesRecord,
      Func<T, string> getId)
    {
      items.Add(item);
      string key = getId(item);
      if (!changesRecord.ContainsKey(key))
        changesRecord[key] = 1;
      else
        ++changesRecord[key];
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Needed this as extension")]
    private static bool IsCommitsUploadedByAgent(
      this PipelineArtifactSource artifactSource,
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId)
    {
      if (artifactSource == null)
        throw new ArgumentNullException(nameof (artifactSource));
      return ArtifactSourceExtensions.GetCommitsAttachmentFromPlan(requestContext, projectId, planId, artifactSource.Alias) != null;
    }

    private static TaskAttachment GetCommitsAttachmentFromPlan(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      string alias)
    {
      string commitsAttachmentName = ArtifactSourceExtensions.GetCommitsAttachmentName(alias);
      return ArtifactSourceExtensions.GetAttachmentFromPlan(requestContext, projectId, planId, commitsAttachmentName);
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Needed this as extension")]
    private static bool IsWorkItemsUploadedByAgent(
      this PipelineArtifactSource artifactSource,
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId)
    {
      if (artifactSource == null)
        throw new ArgumentNullException(nameof (artifactSource));
      return ArtifactSourceExtensions.GetWorkItemsAttachmentFromPlan(requestContext, projectId, planId, artifactSource.Alias) != null;
    }

    private static TaskAttachment GetWorkItemsAttachmentFromPlan(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      string alias)
    {
      string itemsAttachmentName = ArtifactSourceExtensions.GetWorkItemsAttachmentName(alias);
      return ArtifactSourceExtensions.GetAttachmentFromPlan(requestContext, projectId, planId, itemsAttachmentName);
    }

    private static TaskAttachment GetAttachmentFromPlan(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      string attachmentName)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.GetService<IDistributedTaskHubService>().GetReleaseTaskHub(requestContext).GetAttachments(requestContext.Elevate(), projectId, planId, CoreAttachmentType.FileAttachment).FirstOrDefault<TaskAttachment>((Func<TaskAttachment, bool>) (x => x.Name.Equals(attachmentName, StringComparison.OrdinalIgnoreCase)));
    }

    private static Stream GetAttachmentAsStream(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskHub taskHub,
      Guid planId,
      TaskAttachment attachment)
    {
      Stream attachmentAsStream = (Stream) null;
      if (attachment != null)
        attachmentAsStream = taskHub.GetAttachment(requestContext, projectId, planId, attachment.TimelineId, attachment.RecordId, CoreAttachmentType.FileAttachment, attachment.Name);
      return attachmentAsStream;
    }

    private static string GetCommitsAttachmentName(string alias) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "commits_{0}_v{1}.json", (object) alias, (object) 1);

    private static string GetWorkItemsAttachmentName(string alias) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "workitems_{0}_v{1}.json", (object) alias, (object) 1);

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void TraceInfo(
      IVssRequestContext requestContext,
      int tracePoint,
      string message)
    {
      requestContext.Trace(tracePoint, TraceLevel.Info, "ReleaseManagementService", "Service", message);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void TraceError(
      IVssRequestContext requestContext,
      int tracePoint,
      string message)
    {
      requestContext.Trace(tracePoint, TraceLevel.Error, "ReleaseManagementService", "Service", message);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void TraceArtifactUnavailableError(
      IVssRequestContext requestContext,
      string artifactType,
      string errorMessage)
    {
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ArtifactType: {0}, Exception: {1}", (object) artifactType, (object) errorMessage);
      ArtifactSourceExtensions.TraceError(requestContext, 1972007, message);
    }
  }
}
