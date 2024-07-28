// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.DeploymentConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class DeploymentConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment ConvertModelToContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment,
      IVssRequestContext context,
      Guid projectId)
    {
      if (deployment == null)
        throw new ArgumentNullException(nameof (deployment));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment contract = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment()
      {
        Id = deployment.Id,
        Attempt = deployment.Attempt,
        Reason = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentReason) deployment.Reason,
        DeploymentStatus = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus) deployment.Status,
        OperationStatus = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus) deployment.OperationStatus,
        RequestedBy = new IdentityRef()
        {
          Id = deployment.RequestedBy.ToString()
        },
        RequestedFor = new IdentityRef()
        {
          Id = deployment.RequestedFor.ToString()
        },
        QueuedOn = deployment.QueuedOn,
        StartedOn = deployment.StartedOn,
        LastModifiedOn = deployment.LastModifiedOn,
        LastModifiedBy = new IdentityRef()
        {
          Id = deployment.LastModifiedBy.ToString()
        },
        DefinitionEnvironmentId = deployment.DefinitionEnvironmentId,
        ScheduledDeploymentTime = deployment.ScheduledDeploymentTime == DateTime.MinValue ? new DateTime?() : new DateTime?(deployment.ScheduledDeploymentTime)
      };
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) deployment.LinkedArtifacts)
        contract.Release.Artifacts.Add(linkedArtifact.ToWebApi(context.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion")));
      DeploymentConverter.PopulateDeploymentCompletedOn(contract, deployment);
      DeploymentConverter.PopulateDeploymentConditions(contract, deployment);
      DeploymentConverter.PopulateApprovals(contract, deployment, context, projectId);
      ReleaseShallowReference shallowReference = ShallowReferencesHelper.CreateReleaseShallowReference(context, projectId, deployment.ReleaseId, deployment.ReleaseName);
      string releaseWebAccessUri = WebAccessUrlBuilder.GetReleaseWebAccessUri(context, projectId.ToString(), deployment.ReleaseId);
      contract.Release.Id = shallowReference.Id;
      contract.Release.Name = shallowReference.Name;
      string href1 = shallowReference.Url ?? WebAccessUrlBuilder.GetReleaseRestUrl(context, projectId, deployment.ReleaseId);
      contract.Release.Url = href1;
      contract.Release.WebAccessUri = releaseWebAccessUri;
      contract.Release.Links.AddLink("self", href1);
      contract.Release.Links.AddLink("web", releaseWebAccessUri);
      contract.ReleaseDefinitionReference = ShallowReferencesHelper.CreateDefinitionShallowReference(context, projectId, deployment.ReleaseDefinitionId, deployment.ReleaseDefinitionName, deployment.ReleaseDefinitionPath);
      contract.ReleaseDefinitionReference.ProjectReference = new ProjectReference()
      {
        Id = projectId
      };
      string definitionWebAccessUri = WebAccessUrlBuilder.GetReleaseDefinitionWebAccessUri(context, projectId.ToString(), deployment.ReleaseDefinitionId);
      string href2 = contract.ReleaseDefinitionReference.Url ?? WebAccessUrlBuilder.GetReleaseDefinitionRestUrl(context, projectId, deployment.ReleaseDefinitionId);
      contract.ReleaseDefinitionReference.Links.AddLink("self", href2);
      contract.ReleaseDefinitionReference.Links.AddLink("web", definitionWebAccessUri);
      contract.ReleaseEnvironmentReference = ShallowReferencesHelper.CreateReleaseEnvironmentShallowReference(context, projectId, deployment.ReleaseId, deployment.ReleaseEnvironmentId, deployment.ReleaseEnvironmentName);
      contract.ReleaseEnvironmentReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseEnvironmentRestUri(context, projectId, deployment.ReleaseId, deployment.ReleaseEnvironmentId));
      contract.ReleaseEnvironmentReference.Links.AddLink("web", WebAccessUrlBuilder.GetReleaseLogsWebAccessUriWithEnvironmentId(context, projectId.ToString(), deployment.ReleaseId, deployment.ReleaseEnvironmentId));
      return contract;
    }

    private static void PopulateApprovals(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment deploymentContract,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment,
      IVssRequestContext context,
      Guid projectId)
    {
      IEnumerable<ReleaseApproval> approvalContract1 = ReleaseEnvironmentConverter.ConvertStepsToApprovalContract(deployment.Steps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.PreDeploy)), (ReleaseDefinitionEnvironmentsSnapshot) null);
      IEnumerable<ReleaseApproval> approvalContract2 = ReleaseEnvironmentConverter.ConvertStepsToApprovalContract(deployment.Steps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.PostDeploy)), (ReleaseDefinitionEnvironmentsSnapshot) null);
      foreach (ReleaseApproval approval in approvalContract1)
      {
        DeploymentConverter.PopulateShallowReferences(context, approval, deployment.Steps.FirstOrDefault<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.PreDeploy)), projectId);
        deploymentContract.PreDeployApprovals.Add(approval);
      }
      foreach (ReleaseApproval approval in approvalContract2)
      {
        DeploymentConverter.PopulateShallowReferences(context, approval, deployment.Steps.FirstOrDefault<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.PostDeploy)), projectId);
        deploymentContract.PostDeployApprovals.Add(approval);
      }
    }

    private static void PopulateDeploymentCompletedOn(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment webApiDeployment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment)
    {
      if (deployment.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Failed && deployment.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Succeeded && deployment.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.PartiallySucceeded)
        return;
      webApiDeployment.CompletedOn = deployment.LastModifiedOn;
    }

    private static void PopulateDeploymentConditions(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment webApiDeployment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment)
    {
      IList<ReleaseCondition> conditions = deployment.Conditions;
      if (conditions == null)
        return;
      foreach (ReleaseCondition releaseCondition in (IEnumerable<ReleaseCondition>) conditions)
      {
        ConditionsUtility.ConvertToWebApiCondition((Condition) releaseCondition);
        webApiDeployment.Conditions.Add((Condition) releaseCondition);
      }
    }

    private static void PopulateShallowReferences(
      IVssRequestContext context,
      ReleaseApproval approval,
      ReleaseEnvironmentStep step,
      Guid projectId)
    {
      approval.ReleaseReference = ShallowReferencesHelper.CreateReleaseShallowReference(context, projectId, step.ReleaseId, step.ReleaseName);
      approval.ReleaseDefinitionReference = ShallowReferencesHelper.CreateDefinitionShallowReference(context, projectId, step.ReleaseDefinitionId, step.ReleaseDefinitionName, step.ReleaseDefinitionPath);
      approval.ReleaseEnvironmentReference = ShallowReferencesHelper.CreateReleaseEnvironmentShallowReference(context, projectId, step.ReleaseId, step.ReleaseEnvironmentId, step.ReleaseEnvironmentName);
      approval.Url = WebAccessUrlBuilder.GetReleaseApprovalRestUri(context, projectId, approval.Id);
      approval.ReleaseReference.Links.AddLink("web", WebAccessUrlBuilder.GetReleaseWebAccessUri(context, projectId.ToString(), step.ReleaseId));
      approval.ReleaseReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseRestUrl(context, projectId, step.ReleaseId));
      approval.ReleaseDefinitionReference.Links.AddLink("web", WebAccessUrlBuilder.GetReleaseDefinitionWebAccessUri(context, projectId.ToString(), step.ReleaseDefinitionId));
      approval.ReleaseDefinitionReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseDefinitionRestUrl(context, projectId, step.ReleaseDefinitionId));
      approval.ReleaseEnvironmentReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseEnvironmentRestUri(context, projectId, step.ReleaseId, step.ReleaseDefinitionId));
    }
  }
}
