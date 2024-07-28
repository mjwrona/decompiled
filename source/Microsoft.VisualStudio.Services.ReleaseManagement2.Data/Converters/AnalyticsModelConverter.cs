// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.AnalyticsModelConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class AnalyticsModelConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.Release ToAnalyticsModel(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.Release) null;
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.Release()
      {
        ProjectGuid = release.ProjectId,
        ReleaseId = release.Id,
        ReleaseDefinitionId = new int?(release.ReleaseDefinitionId),
        DefinitionSnapshotRevision = new int?(release.DefinitionSnapshotRevision),
        Name = release.Name,
        Description = release.Description,
        Status = new int?((int) release.Status),
        Reason = new int?((int) release.Reason),
        CreatedByGuid = new Guid?(release.CreatedBy),
        CreatedOn = new DateTime?(release.CreatedOn),
        ModifiedByGuid = new Guid?(release.ModifiedBy),
        ModifiedOn = new DateTime?(release.ModifiedOn),
        KeepForever = new bool?(release.KeepForever),
        IsDeleted = new bool?(release.IsDeleted)
      };
    }

    public static ReleaseArtifactSource ToAnalyticsModel(
      this PipelineArtifactSource releaseArtifactSource)
    {
      if (releaseArtifactSource == null)
        return (ReleaseArtifactSource) null;
      return new ReleaseArtifactSource()
      {
        ProjectGuid = releaseArtifactSource.ProjectId,
        ReleaseId = releaseArtifactSource.ReleaseId,
        ReleaseArtifactSourceId = releaseArtifactSource.Id,
        ReleaseDefinitionId = releaseArtifactSource.ReleaseDefinitionId,
        SourceId = releaseArtifactSource.SourceId,
        Alias = releaseArtifactSource.Alias,
        ArtifactType = releaseArtifactSource.ArtifactTypeId,
        ArtifactVersionId = releaseArtifactSource.VersionId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        ArtifactVersionName = releaseArtifactSource.VersionName,
        SourceBranch = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter.AddBranchPrefixIfRequired(releaseArtifactSource.SourceBranch),
        IsPrimary = releaseArtifactSource.IsPrimary,
        CreatedOn = releaseArtifactSource.CreatedOn
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseDefinition ToAnalyticsModel(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseDefinition) null;
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseDefinition()
      {
        ProjectGuid = releaseDefinition.ProjectId,
        ReleaseDefinitionId = releaseDefinition.Id,
        Revision = new int?(releaseDefinition.Revision),
        Name = releaseDefinition.Name,
        Description = releaseDefinition.Description,
        Source = new int?((int) releaseDefinition.Source),
        Path = releaseDefinition.Path,
        PipelineProcessType = new int?((int) releaseDefinition.PipelineProcessType),
        CreatedByGuid = new Guid?(releaseDefinition.CreatedBy),
        CreatedOn = new DateTime?(releaseDefinition.CreatedOn.Value),
        ModifiedByGuid = new Guid?(releaseDefinition.ModifiedBy),
        ModifiedOn = new DateTime?(releaseDefinition.ModifiedOn.Value),
        IsDeleted = new bool?(releaseDefinition.IsDeleted),
        IsDisabled = new bool?(releaseDefinition.IsDisabled)
      };
    }

    public static ReleaseDeployment ToAnalyticsModel(this Deployment deployment)
    {
      if (deployment == null)
        return (ReleaseDeployment) null;
      return new ReleaseDeployment()
      {
        ProjectGuid = deployment.ProjectId,
        ReleaseId = deployment.ReleaseId,
        ReleaseEnvironmentId = deployment.ReleaseEnvironmentId,
        ReleaseDeploymentId = deployment.Id,
        ReleaseDefinitionId = deployment.ReleaseDefinitionId,
        EnvironmentDefinitionId = deployment.DefinitionEnvironmentId,
        Attempt = deployment.Attempt,
        Reason = (int) deployment.Reason,
        DeploymentStatus = (int) deployment.Status,
        OperationStatus = (int) deployment.OperationStatus,
        RequestedByGuid = deployment.RequestedBy,
        RequestedForGuid = deployment.RequestedFor,
        LastModifiedByGuid = deployment.LastModifiedBy,
        QueuedOn = deployment.QueuedOn,
        StartedOn = new DateTime?(deployment.StartedOn),
        LastModifiedOn = deployment.LastModifiedOn
      };
    }

    public static ReleaseDeploymentGate ToAnalyticsModel(this DeploymentGate deploymentGate)
    {
      if (deploymentGate == null)
        return (ReleaseDeploymentGate) null;
      return new ReleaseDeploymentGate()
      {
        ProjectGuid = deploymentGate.ProjectId,
        ReleaseId = deploymentGate.ReleaseId,
        ReleaseEnvironmentStepId = deploymentGate.ReleaseEnvironmentStepId,
        ReleaseEnvironmentId = deploymentGate.ReleaseEnvironmentId,
        ReleaseDeploymentId = deploymentGate.DeploymentId,
        GateType = (int) deploymentGate.GateType,
        Status = (int) deploymentGate.Status,
        RunPlanGuid = deploymentGate.RunPlanId,
        StartedOn = deploymentGate.StartedOn,
        LastModifiedOn = deploymentGate.LastModifiedOn,
        StabilizationCompletedOn = deploymentGate.StabilizationCompletedOn,
        DeploymentLastModifiedOn = deploymentGate.DeploymentLastModifiedOn
      };
    }

    public static ReleaseDeploymentRunPlan ToAnalyticsModel(
      this ReleaseDeployPhase releaseDeployPhase)
    {
      if (releaseDeployPhase == null)
        return (ReleaseDeploymentRunPlan) null;
      return new ReleaseDeploymentRunPlan()
      {
        ProjectGuid = releaseDeployPhase.ProjectId,
        ReleaseId = releaseDeployPhase.ReleaseId,
        ReleaseEnvironmentId = releaseDeployPhase.ReleaseEnvironmentId,
        ReleaseDeploymentId = releaseDeployPhase.DeploymentId,
        RunPlanGuid = releaseDeployPhase.RunPlanId,
        DeploymentLastModifiedOn = releaseDeployPhase.DeploymentLastModifiedOn
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironment ToAnalyticsModel(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      if (releaseEnvironment == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironment) null;
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironment()
      {
        ProjectGuid = releaseEnvironment.ProjectId,
        ReleaseId = releaseEnvironment.ReleaseId,
        ReleaseEnvironmentId = releaseEnvironment.Id,
        EnvironmentDefinitionId = releaseEnvironment.DefinitionEnvironmentId,
        Name = releaseEnvironment.Name,
        Status = (int) releaseEnvironment.Status,
        OwnerGuid = releaseEnvironment.OwnerId,
        DeploymentLastModifiedOn = releaseEnvironment.DeploymentLastModifiedOn
      };
    }

    public static ReleaseEnvironmentDefinition ToAnalyticsModel(
      this DefinitionEnvironment definitionEnvironment)
    {
      if (definitionEnvironment == null)
        return (ReleaseEnvironmentDefinition) null;
      return new ReleaseEnvironmentDefinition()
      {
        ProjectGuid = definitionEnvironment.ProjectId,
        ReleaseDefinitionId = definitionEnvironment.ReleaseDefinitionId,
        ReleaseEnvironmentDefinitionId = definitionEnvironment.Id,
        Name = definitionEnvironment.Name,
        OwnerGuid = definitionEnvironment.OwnerId,
        ModifiedOn = definitionEnvironment.ModifiedOn
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironmentStep ToAnalyticsModel(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      if (releaseEnvironmentStep == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironmentStep) null;
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironmentStep()
      {
        ProjectGuid = releaseEnvironmentStep.ProjectId,
        ReleaseId = releaseEnvironmentStep.ReleaseId,
        ReleaseEnvironmentStepId = releaseEnvironmentStep.Id,
        ReleaseEnvironmentId = releaseEnvironmentStep.ReleaseEnvironmentId,
        EnvironmentDefinitionId = releaseEnvironmentStep.DefinitionEnvironmentId,
        TrialNumber = releaseEnvironmentStep.TrialNumber,
        StepType = (int) releaseEnvironmentStep.StepType,
        Status = (int) releaseEnvironmentStep.Status,
        ApproverGuid = new Guid?(releaseEnvironmentStep.ApproverId),
        ActualApproverGuid = new Guid?(releaseEnvironmentStep.ActualApproverId),
        CreatedOn = releaseEnvironmentStep.CreatedOn,
        ModifiedOn = releaseEnvironmentStep.ModifiedOn,
        IsAutomated = releaseEnvironmentStep.IsAutomated,
        DeploymentLastModifiedOn = releaseEnvironmentStep.DeploymentLastModifiedOn
      };
    }
  }
}
