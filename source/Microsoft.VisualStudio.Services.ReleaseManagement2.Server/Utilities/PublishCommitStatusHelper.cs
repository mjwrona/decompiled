// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.PublishCommitStatusHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class PublishCommitStatusHelper
  {
    private const string ContinuousDeploymentCommitStatusGenre = "continuous-deployment/release";
    private const string ContextNameSeperator = ")^!!(!";
    private const string CommitShaRegexFormat = "[a-fA-F0-9]{40}";
    private const int MaxReleaseDefinitionNameLength = 90;
    private const string PostFixForTruncatedNames = "#$";

    public static void PublishEnvironmentStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease,
      int releaseEnvironmentId)
    {
      PublishCommitStatusHelper.PublishEnvironmentStatus(requestContext, projectId, serverRelease, releaseEnvironmentId, EnvironmentStatus.Undefined);
    }

    public static void PublishEnvironmentStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease,
      int releaseEnvironmentId,
      EnvironmentStatus environmentStatus)
    {
      if (serverRelease == null)
        return;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = serverRelease.Environments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Id == releaseEnvironmentId));
      if (releaseEnvironment.EnvironmentOptions == null || !releaseEnvironment.EnvironmentOptions.PublishDeploymentStatus)
        return;
      PublishCommitStatusHelper.PublishEnvironmentStatus(requestContext, projectId, serverRelease, releaseEnvironment, environmentStatus);
    }

    private static void PublishEnvironmentStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      EnvironmentStatus environmentStatus)
    {
      if (release == null || releaseEnvironment == null)
        return;
      string releaseDefinitionName;
      if (release.ReleaseDefinitionName.Length > 90)
      {
        int length = 90 - "#$".Length;
        StringBuilder stringBuilder = new StringBuilder(release.ReleaseDefinitionName.Substring(0, length));
        stringBuilder.Append("#$");
        releaseDefinitionName = stringBuilder.ToString();
      }
      else
        releaseDefinitionName = release.ReleaseDefinitionName;
      string displayStatus;
      GitStatusState stateFromEnvironment = PublishCommitStatusHelper.GetGitStatusStateFromEnvironment(releaseEnvironment, environmentStatus, out displayStatus);
      string str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GitCommitStatusDescriptionFormat, (object) releaseEnvironment.Name, (object) displayStatus);
      string withEnvironmentId = WebAccessUrlBuilder.GetReleaseLogsWebAccessUriWithEnvironmentId(requestContext, projectId.ToString("D"), release.Id, releaseEnvironment.Id);
      string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{2}{1}", (object) releaseDefinitionName, (object) releaseEnvironment.Rank, (object) ")^!!(!");
      GitStatus statusToCreate = new GitStatus()
      {
        State = stateFromEnvironment,
        Description = str1,
        TargetUrl = withEnvironmentId,
        Context = new GitStatusContext()
        {
          Genre = "continuous-deployment/release",
          Name = str2
        }
      };
      Dictionary<string, string> commitToRepositoryMap = new Dictionary<string, string>(release.LinkedArtifacts.Count);
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) release.LinkedArtifacts)
      {
        if (linkedArtifact.ArtifactTypeId == "Git")
          PublishCommitStatusHelper.ResolveCommitAndRepoForGitArtifact(linkedArtifact.SourceData, commitToRepositoryMap);
        else if (linkedArtifact.IsBuildArtifact)
          PublishCommitStatusHelper.ResolveCommitAndRepoForBuildArtifact(linkedArtifact.SourceData, commitToRepositoryMap);
      }
      foreach (KeyValuePair<string, string> keyValuePair in commitToRepositoryMap)
        PublishCommitStatusHelper.PublishGitCommitStatus(requestContext, statusToCreate, keyValuePair.Key, keyValuePair.Value);
    }

    private static GitStatusState GetGitStatusStateFromEnvironment(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      EnvironmentStatus environmentStatus,
      out string displayStatus)
    {
      if (environmentStatus == EnvironmentStatus.Undefined)
      {
        if (releaseEnvironment.Status == ReleaseEnvironmentStatus.InProgress)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment = releaseEnvironment.DeploymentAttempts.OrderBy<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, int>) (da => da.Attempt)).LastOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>();
          environmentStatus = deployment == null || deployment.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.NotDeployed || deployment.OperationStatus != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Queued ? EnvironmentStatus.InProgress : EnvironmentStatus.Queued;
        }
        else
          environmentStatus = releaseEnvironment.Status.ToWebApi();
      }
      if (environmentStatus <= EnvironmentStatus.Canceled)
      {
        if (environmentStatus != EnvironmentStatus.InProgress)
        {
          if (environmentStatus != EnvironmentStatus.Succeeded)
          {
            if (environmentStatus == EnvironmentStatus.Canceled)
            {
              displayStatus = Resources.EnvironmentCanceledStatusText;
              return GitStatusState.Failed;
            }
          }
          else
          {
            displayStatus = Resources.EnvironmentSucceededStatusText;
            return GitStatusState.Succeeded;
          }
        }
        else
        {
          displayStatus = Resources.EnvironmentInProgressStatusText;
          return GitStatusState.Pending;
        }
      }
      else if (environmentStatus != EnvironmentStatus.Rejected)
      {
        if (environmentStatus != EnvironmentStatus.Queued)
        {
          if (environmentStatus == EnvironmentStatus.PartiallySucceeded)
          {
            displayStatus = Resources.EnvironmentPartiallySucceededStatusText;
            return GitStatusState.Failed;
          }
        }
        else
        {
          displayStatus = Resources.EnvironmentQueuedStatusText;
          return GitStatusState.Pending;
        }
      }
      else
      {
        displayStatus = Resources.EnvironmentFailedStatusText;
        return GitStatusState.Failed;
      }
      displayStatus = Resources.EnvironmentUndefinedStatusText;
      return GitStatusState.Pending;
    }

    private static void ResolveCommitAndRepoForGitArtifact(
      Dictionary<string, InputValue> artifactSourceData,
      Dictionary<string, string> commitToRepositoryMap)
    {
      InputValue inputValue;
      if (!artifactSourceData.TryGetValue("version", out inputValue) || !PublishCommitStatusHelper.IsValidCommitValue(inputValue.Value))
        return;
      string key = inputValue.Value;
      if (!artifactSourceData.TryGetValue("definition", out inputValue) || !Guid.TryParseExact(inputValue.Value, "D", out Guid _))
        return;
      string str = inputValue.Value;
      commitToRepositoryMap.TryAdd<string, string>(key, str);
    }

    private static void ResolveCommitAndRepoForBuildArtifact(
      Dictionary<string, InputValue> artifactSourceData,
      Dictionary<string, string> commitToRepositoryMap)
    {
      InputValue inputValue;
      if (!artifactSourceData.TryGetValue("version", out inputValue) || inputValue.Data == null)
        return;
      string str1 = inputValue.Data.GetValueOrDefault<string, object>("repositoryType")?.ToString();
      if (string.IsNullOrEmpty(str1) || str1 != "TfsGit")
        return;
      string str2 = inputValue.Data.GetValueOrDefault<string, object>("sourceVersion")?.ToString();
      string input = inputValue.Data.GetValueOrDefault<string, object>("repositoryId")?.ToString();
      if (!PublishCommitStatusHelper.IsValidCommitValue(str2) || !Guid.TryParseExact(input, "D", out Guid _))
        return;
      commitToRepositoryMap.TryAdd<string, string>(str2, input);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This should not break the flow")]
    private static void PublishGitCommitStatus(
      IVssRequestContext context,
      GitStatus statusToCreate,
      string commitId,
      string repositoryId)
    {
      if (string.IsNullOrWhiteSpace(commitId) || string.IsNullOrWhiteSpace(repositoryId))
        return;
      GitHttpClient gitHttpClient = context.GetClient<GitHttpClient>();
      try
      {
        Func<Task<GitStatus>> func = (Func<Task<GitStatus>>) (() => gitHttpClient.CreateCommitStatusAsync(statusToCreate, commitId, repositoryId));
        context.ExecuteAsyncAndSyncResult<GitStatus>(func);
      }
      catch (Exception ex)
      {
        context.TraceException(1971078, "ReleaseManagementService", "Service", ex);
      }
    }

    private static bool IsValidCommitValue(string commitId) => Regex.IsMatch(commitId, "[a-fA-F0-9]{40}");
  }
}
