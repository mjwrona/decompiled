// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.WellKnownReleaseVariables
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [GenerateAllConstants(null)]
  public static class WellKnownReleaseVariables
  {
    public const string AgentReleaseDirectory = "agent.releaseDirectory";
    public const string DeploymentGroupId = "agent.deploymentGroupId";
    public const string EnableAccessTokenVariableName = "system.enableAccessToken";
    public const string HostType = "system.hosttype";
    public const string ArtifactsDirectory = "system.artifactsDirectory";
    public const string CollectionId = "system.collectionId";
    public const string TeamProjectId = "system.teamProjectId";
    public const string TeamProject = "system.teamProject";
    public const string TotalJobsInPhase = "system.totalJobsInPhase";
    public const string JobPositionInPhase = "system.jobPositionInPhase";
    public const string ParallelExecutionType = "system.parallelexecutiontype";
    public const string ReleaseId = "release.releaseId";
    public const string SkipArtifactsDownload = "release.skipartifactsDownload";
    public const string ReleaseName = "release.releaseName";
    public const string ReleaseDescription = "release.releaseDescription";
    public const string ReleaseDisableRobocopy = "release.disableRobocopy";
    public const string ReleaseDefinitionName = "release.definitionName";
    public const string ReleaseDefinitionId = "release.definitionId";
    public const string ReleaseDefinitionEnvironmentId = "release.definitionEnvironmentId";
    public const string DeploymentId = "release.deploymentId";
    public const string ReleaseUri = "release.releaseUri";
    public const string ReleaseWebUrl = "release.releaseWebUrl";
    public const string ReleaseEnvironmentUri = "release.environmentUri";
    public const string ReleaseEnvironmentId = "release.environmentId";
    public const string ReleaseEnvironmentName = "release.environmentName";
    public const string ReleaseDeployPhaseId = "release.deployPhaseId";
    public const string RequestorId = "release.requestedFor";
    public const string ReleaseRequestedForId = "release.requestedForId";
    public const string ReleaseRequestedForEmail = "release.requestedForEmail";
    public const string AttemptNumber = "release.attemptNumber";
    public const string ReleaseReason = "release.reason";
    public const string ReleasePrimaryArtifactSourceAlias = "release.primaryArtifactSourceAlias";
    public const string ReleaseTriggeringArtifactAlias = "release.triggeringartifact.alias";
    public const string ReleaseDeploymentRequestedForId = "release.deployment.requestedForId";
    public const string ReleaseDeploymentRequestedForEmail = "release.deployment.requestedForEmail";
    public const string ReleaseDeploymentRequestedFor = "release.deployment.requestedFor";
    public const string ReleaseDeploymentStartTime = "release.deployment.startTime";
    public const string ReleaseEnvironmentRedeploymentWithDeploymentGroupTargetFilter = "release.redeployment.deploymentGroupTargetFilter";
    public static readonly List<string> ReadOnlySystemVariables = new List<string>()
    {
      "agent.releaseDirectory",
      "agent.deploymentGroupId",
      "system.enableAccessToken",
      "system.hosttype",
      "system.artifactsDirectory",
      "system.collectionId",
      "system.teamProjectId",
      "system.teamProject",
      "system.totalJobsInPhase",
      "system.jobPositionInPhase",
      "system.parallelexecutiontype",
      "release.releaseId",
      "release.skipartifactsDownload",
      "release.releaseName",
      "release.releaseDescription",
      "release.disableRobocopy",
      "release.definitionName",
      "release.definitionId",
      "release.definitionEnvironmentId",
      "release.deploymentId",
      "release.releaseUri",
      "release.releaseWebUrl",
      "release.environmentUri",
      "release.environmentId",
      "release.environmentName",
      "release.deployPhaseId",
      "release.requestedFor",
      "release.requestedForId",
      "release.requestedForEmail",
      "release.attemptNumber",
      "release.reason",
      "release.primaryArtifactSourceAlias",
      "release.triggeringartifact.alias",
      "release.deployment.requestedForId",
      "release.deployment.requestedForEmail",
      "release.deployment.requestedFor",
      "release.deployment.startTime",
      "release.redeployment.deploymentGroupTargetFilter"
    };

    public static string GetReleasePrimaryArtifactVariableKey(string wellKnownVariable) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) "build", (object) wellKnownVariable);

    public static string GetReleaseArtifactVariableKey(
      string artifactAlias,
      string wellKnownVariable)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}", (object) "release.artifacts", (object) artifactAlias, (object) wellKnownVariable);
    }

    public static string GetReleaseEnvironmentStatusKey(string environmentName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}", (object) "release.environments", (object) environmentName, (object) "status");
  }
}
