// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.CompatExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
  public static class CompatExtensions
  {
    public static void ToNoPhasesFormat(this ReleaseDefinitionEnvironment environment)
    {
      environment.QueueId = environment != null ? environment.GetCompatQueueId() : throw new ArgumentNullException(nameof (environment));
      if (environment.GetCompatDemands() != null)
        environment.Demands = (IList<Demand>) new List<Demand>((IEnumerable<Demand>) environment.GetCompatDemands());
      AgentDeploymentInput compatDeploymentInput = environment.GetCompatDeploymentInput();
      if (compatDeploymentInput != null)
      {
        environment.EnvironmentOptions.TimeoutInMinutes = compatDeploymentInput.TimeoutInMinutes;
        environment.EnvironmentOptions.EnableAccessToken = compatDeploymentInput.EnableAccessToken;
        environment.EnvironmentOptions.SkipArtifactsDownload = compatDeploymentInput.SkipArtifactsDownload;
      }
      IList<WorkflowTask> compatWorkflow = environment.GetCompatWorkflow();
      if (environment.DeployStep != null && compatWorkflow != null)
        environment.DeployStep.Tasks = new List<WorkflowTask>((IEnumerable<WorkflowTask>) compatWorkflow);
      environment.DeployPhases = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) null;
    }

    public static void ToDeployPhasesFormat(this ReleaseDefinitionEnvironment environment)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase = environment != null ? environment.CreateCompatDeployPhase() : throw new ArgumentNullException(nameof (environment));
      environment.DeployPhases.Clear();
      environment.DeployPhases.Add(deployPhase);
      environment.QueueId = 0;
      if (environment.Demands != null)
        environment.Demands.Clear();
      if (environment.DeployStep == null)
        return;
      environment.DeployStep.Tasks = (List<WorkflowTask>) null;
    }

    public static void ToNoPhasesFormat(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      environment.QueueId = environment != null ? environment.GetCompatQueueId() : throw new ArgumentNullException(nameof (environment));
      environment.Demands.Clear();
      environment.Demands.AddRange((IEnumerable<Demand>) new List<Demand>((IEnumerable<Demand>) environment.GetCompatDemands()));
      environment.WorkflowTasks = environment.GetCompatWorkflow() == null ? new List<WorkflowTask>() : new List<WorkflowTask>((IEnumerable<WorkflowTask>) environment.GetCompatWorkflow());
      AgentDeploymentInput compatDeploymentInput = environment.GetCompatDeploymentInput();
      if (compatDeploymentInput != null)
      {
        environment.EnvironmentOptions.TimeoutInMinutes = compatDeploymentInput.TimeoutInMinutes;
        environment.EnvironmentOptions.EnableAccessToken = compatDeploymentInput.EnableAccessToken;
        environment.EnvironmentOptions.SkipArtifactsDownload = compatDeploymentInput.SkipArtifactsDownload;
      }
      foreach (DeploymentAttempt deployStep in environment.DeploySteps)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase releaseDeployPhase = EnvironmentCompatExtensions.GetCompatReleaseDeployPhase(deployStep);
        if (releaseDeployPhase != null)
        {
          deployStep.ErrorLog = releaseDeployPhase.ErrorLog;
          deployStep.RunPlanId = releaseDeployPhase.RunPlanId ?? Guid.NewGuid();
          deployStep.Issues.Add(new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Issue()
          {
            IssueType = "Error",
            Message = releaseDeployPhase.ErrorLog
          });
          DeploymentJob deploymentJob = releaseDeployPhase.DeploymentJobs.FirstOrDefault<DeploymentJob>();
          if (deploymentJob != null)
          {
            deployStep.Tasks = deploymentJob.Tasks.ToList<ReleaseTask>();
            deployStep.Job = deploymentJob.Job;
          }
        }
      }
    }

    public static void ToDeployPhasesFormat(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (environment.DeployPhasesSnapshot != null && !environment.DeployPhasesSnapshot.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>())
        return;
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhasesSnapshot = environment.CreateCompatDeployPhasesSnapshot(serverEnvironment);
      environment.DeployPhasesSnapshot.Add(deployPhasesSnapshot);
      environment.QueueId = 0;
      environment.Demands.Clear();
      environment.WorkflowTasks.Clear();
      foreach (DeploymentAttempt deployStep in environment.DeploySteps)
      {
        deployStep.Tasks = (List<ReleaseTask>) null;
        deployStep.Job = (ReleaseTask) null;
      }
    }

    public static void HandleDeploymentGatesCompatibility(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment)
    {
      if (environment == null || serverEnvironment == null)
        return;
      environment.PreDeploymentGatesSnapshot = serverEnvironment.PreDeploymentGates;
      environment.PostDeploymentGatesSnapshot = serverEnvironment.PostDeploymentGates;
      if (serverEnvironment.PostApprovalOptions == null || environment.PostApprovalsSnapshot == null || environment.PostApprovalsSnapshot.ApprovalOptions == null)
        return;
      environment.PostApprovalsSnapshot.ApprovalOptions.ExecutionOrder = serverEnvironment.PostApprovalOptions.ExecutionOrder;
    }

    public static void HandleDeploymentGatesCompatibility(
      this ReleaseDefinitionEnvironment environment,
      DefinitionEnvironment serverEnvironment)
    {
      if (environment == null || serverEnvironment == null)
        return;
      environment.PreDeploymentGates = serverEnvironment.PreDeploymentGates;
      environment.PostDeploymentGates = serverEnvironment.PostDeploymentGates;
      if (serverEnvironment.PostApprovalOptions == null || environment.PostDeployApprovals == null || environment.PostDeployApprovals.ApprovalOptions == null)
        return;
      environment.PostDeployApprovals.ApprovalOptions.ExecutionOrder = serverEnvironment.PostApprovalOptions.ExecutionOrder;
    }

    public static void HandleEnvironmentTriggersCompatibility(
      this ReleaseDefinitionEnvironment environment,
      DefinitionEnvironment serverEnvironment)
    {
      if (environment == null || serverEnvironment == null)
        return;
      environment.EnvironmentTriggers = EnvironmentTriggerConverter.ToWebApiEnvironmentTriggers(serverEnvironment.EnvironmentTriggers);
    }

    public static void ToUndefinedReleaseDefinitionSource(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      if (definition.Source != ReleaseDefinitionSource.PortalExtensionApi)
        return;
      definition.Source = ReleaseDefinitionSource.Undefined;
    }

    public static void ToUndefinedDeployStepTasks(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      if (definition == null || definition.Environments == null)
        return;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) definition.Environments)
      {
        if (environment.DeployStep != null)
          environment.DeployStep.Tasks = (List<WorkflowTask>) null;
      }
    }
  }
}
