// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentContractExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseEnvironmentContractExtensions
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment ToContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverModelEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext context,
      Guid projectId,
      bool includeTasks = true)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApi = serverModelEnvironment.ToWebApi(release, context, projectId);
      if (includeTasks)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        webApi.FillEnvironmentTasks(context, projectId, serverModelEnvironment, ReleaseEnvironmentContractExtensions.\u003C\u003EO.\u003C0\u003E__GetTimelineRecords ?? (ReleaseEnvironmentContractExtensions.\u003C\u003EO.\u003C0\u003E__GetTimelineRecords = new Func<IVssRequestContext, Guid, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes, IEnumerable<TimelineRecord>>(ReleaseEnvironmentContractExtensions.GetTimelineRecords)));
        webApi.PopulateTaskLogUrls(context, release.Id, projectId);
      }
      DefinitionEnvironmentData snapshotEnvironment = release.DefinitionSnapshot.Environments.First<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (e => e.Id == serverModelEnvironment.DefinitionEnvironmentId));
      webApi.PopulateApprovalsSnapshots(serverModelEnvironment, snapshotEnvironment);
      ReleaseEnvironmentContractExtensions.PopulateIdentities(context, webApi, true);
      return webApi;
    }

    private static void PopulateIdentities(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      bool includeIdentityUrls)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "ReleaseEnvironmentContractExtensions.PopulateIdentities", 1961101))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ReleaseEnvironmentContractExtensions.PopulateIdentitiesImplementation(environment, context, ReleaseEnvironmentContractExtensions.\u003C\u003EO.\u003C1\u003E__GetIdentityHelper ?? (ReleaseEnvironmentContractExtensions.\u003C\u003EO.\u003C1\u003E__GetIdentityHelper = new Func<IVssRequestContext, IList<string>, bool, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper>(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper.GetIdentityHelper)), new Action<IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper>(new ReleaseIdentityHandler().PopulateIdentities), includeIdentityUrls);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Testing requirements")]
    private static void PopulateIdentitiesImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      IVssRequestContext context,
      Func<IVssRequestContext, IList<string>, bool, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper> getIdentityMap,
      Action<IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper> updateReferences,
      bool includeIdentityUrls)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (getIdentityMap == null)
        throw new ArgumentNullException(nameof (getIdentityMap));
      if (updateReferences == null)
        throw new ArgumentNullException(nameof (updateReferences));
      IEnumerable<string> teamFoundationIds = ReleaseIdentityHandler.GetTeamFoundationIds((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment[1]
      {
        environment
      });
      Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper identityHelper = getIdentityMap(context, (IList<string>) teamFoundationIds.ToList<string>(), includeIdentityUrls);
      updateReferences(context, environment, identityHelper);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is a delegate definition")]
    public static void FillEnvironmentTasks(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment,
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverModelEnvironment,
      Func<IVssRequestContext, Guid, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes, IEnumerable<TimelineRecord>> getTimelineRecords,
      int numberOfGateRecords = 5)
    {
      if (webApiEnvironment == null)
        throw new ArgumentNullException(nameof (webApiEnvironment));
      if (serverModelEnvironment == null)
        throw new ArgumentNullException(nameof (serverModelEnvironment));
      if (getTimelineRecords == null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        getTimelineRecords = ReleaseEnvironmentContractExtensions.\u003C\u003EO.\u003C0\u003E__GetTimelineRecords ?? (ReleaseEnvironmentContractExtensions.\u003C\u003EO.\u003C0\u003E__GetTimelineRecords = new Func<IVssRequestContext, Guid, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes, IEnumerable<TimelineRecord>>(ReleaseEnvironmentContractExtensions.GetTimelineRecords));
      }
      foreach (ReleaseEnvironmentStep deployOrGateStep in serverModelEnvironment.GetDeployOrGateSteps())
      {
        DeploymentAttempt deploymentAttempt = webApiEnvironment.GetDeploymentAttempt(deployOrGateStep.TrialNumber);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deploymentByAttempt = serverModelEnvironment.GetDeploymentByAttempt(deployOrGateStep.TrialNumber);
        if (deploymentAttempt != null)
        {
          switch (deployOrGateStep.StepType)
          {
            case EnvironmentStepType.Deploy:
              ReleaseEnvironmentContractExtensions.FillDeployStep(requestContext, projectId, getTimelineRecords, serverModelEnvironment, deployOrGateStep, deploymentAttempt, numberOfGateRecords);
              continue;
            case EnvironmentStepType.PreGate:
            case EnvironmentStepType.PostGate:
              ReleaseEnvironmentContractExtensions.FillGateStep(requestContext, projectId, deploymentByAttempt, deploymentAttempt, deployOrGateStep.StepType, numberOfGateRecords);
              continue;
            default:
              continue;
          }
        }
      }
    }

    public static void PopulateTaskLogUrls(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment,
      IVssRequestContext requestContext,
      int releaseId,
      Guid projectId)
    {
      if (webApiEnvironment == null)
        throw new ArgumentNullException(nameof (webApiEnvironment));
      foreach (DeploymentAttempt deployStep in webApiEnvironment.DeploySteps)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase releaseDeployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase>) deployStep.ReleaseDeployPhases)
        {
          int id = releaseDeployPhase.Id;
          foreach (DeploymentJob deploymentJob in (IEnumerable<DeploymentJob>) releaseDeployPhase.DeploymentJobs)
          {
            foreach (ReleaseTask task in (IEnumerable<ReleaseTask>) deploymentJob.Tasks)
              task.LogUrl = WebAccessUrlBuilder.GetTaskLogUrl(requestContext, projectId, releaseId, webApiEnvironment.Id, deployStep.Attempt, id, task);
            if (deploymentJob.Job != null)
              deploymentJob.Job.LogUrl = WebAccessUrlBuilder.GetTaskLogUrl(requestContext, projectId, releaseId, webApiEnvironment.Id, deployStep.Attempt, id, deploymentJob.Job);
          }
        }
        ReleaseEnvironmentContractExtensions.PopulateGateLogUrls(requestContext, projectId, releaseId, webApiEnvironment.Id, deployStep.PreDeploymentGates);
        ReleaseEnvironmentContractExtensions.PopulateGateLogUrls(requestContext, projectId, releaseId, webApiEnvironment.Id, deployStep.PostDeploymentGates);
      }
    }

    private static void PopulateGateLogUrls(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      ReleaseGates gates)
    {
      if (gates?.DeploymentJobs == null)
        return;
      foreach (DeploymentJob deploymentJob in (IEnumerable<DeploymentJob>) gates.DeploymentJobs)
      {
        if (deploymentJob.Tasks != null)
        {
          foreach (ReleaseTask task in (IEnumerable<ReleaseTask>) deploymentJob.Tasks)
            task.LogUrl = WebAccessUrlBuilder.GetDeploymentGateLogUrl(requestContext, projectId, releaseId, environmentId, gates.Id, task.Id);
        }
      }
    }

    private static void FillGateStep(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment serverDeployment,
      DeploymentAttempt webApiEnvironmentDeployStep,
      EnvironmentStepType stepType,
      int numberOfGateRecords)
    {
      DeploymentGate deploymentGate = serverDeployment.DeploymentGates.FirstOrDefault<DeploymentGate>((Func<DeploymentGate, bool>) (g => g.GateType == stepType));
      if (deploymentGate == null)
        return;
      ReleaseGates releaseGates = deploymentGate.ToReleaseGates();
      Guid? runPlanId1 = deploymentGate.RunPlanId;
      if (!runPlanId1.HasValue)
        return;
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      runPlanId1 = deploymentGate.RunPlanId;
      Guid runPlanId2 = runPlanId1.Value;
      List<TimelineRecord> list = ReleaseEnvironmentContractExtensions.GetGreenlightingTimelineRecords(requestContext1, projectId1, runPlanId2).ToList<TimelineRecord>();
      if (list.Count > 0)
      {
        IEnumerable<DeploymentJob> deployStepRecords = list.ToDeployStepRecords(out string _, numberOfGateRecords);
        releaseGates.DeploymentJobs = (IList<DeploymentJob>) deployStepRecords.ToList<DeploymentJob>();
      }
      if (stepType == EnvironmentStepType.PreGate)
        webApiEnvironmentDeployStep.PreDeploymentGates = releaseGates;
      else
        webApiEnvironmentDeployStep.PostDeploymentGates = releaseGates;
    }

    private static void FillDeployStep(
      IVssRequestContext requestContext,
      Guid projectId,
      Func<IVssRequestContext, Guid, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes, IEnumerable<TimelineRecord>> getTimelineRecords,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverModelEnvironment,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      DeploymentAttempt webApiEnvironmentDeployStep,
      int numberOfGatesPhaseRecords)
    {
      if (releaseEnvironmentStep.RunPlanId.HasValue)
      {
        IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase> releaseDeployPhases = new PipelineOrchestrator(requestContext, projectId).GetTimelineRecords(releaseEnvironmentStep.RunPlanId.Value).ToList<TimelineRecord>().ToReleaseDeployPhases(releaseEnvironmentStep.RunPlanId.Value);
        webApiEnvironmentDeployStep.ReleaseDeployPhases.AddRange<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase, IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase>>((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase>) releaseDeployPhases);
      }
      else
      {
        if (!webApiEnvironmentDeployStep.ReleaseDeployPhases.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase>())
          ReleaseEnvironmentExtensions.FillDeployPhases(requestContext, projectId, serverModelEnvironment, webApiEnvironmentDeployStep);
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase releaseDeployPhase1 in serverModelEnvironment.ReleaseDeployPhases.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, bool>) (phase => phase.Attempt == releaseEnvironmentStep.TrialNumber)).Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, bool>) (phase => phase.RunPlanId.HasValue)))
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase serverDeployPhase = releaseDeployPhase1;
          List<TimelineRecord> list = getTimelineRecords(requestContext, projectId, serverDeployPhase.RunPlanId.Value, serverDeployPhase.PhaseType).ToList<TimelineRecord>();
          if (list.Count > 0)
          {
            int top = serverDeployPhase.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates ? numberOfGatesPhaseRecords : 0;
            IEnumerable<DeploymentJob> deployStepRecords = list.ToDeployStepRecords(out string _, top);
            Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase releaseDeployPhase2 = webApiEnvironmentDeployStep.ReleaseDeployPhases.Single<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase, bool>) (webApiDeployPhase => webApiDeployPhase.Id == serverDeployPhase.Id));
            releaseDeployPhase2.RunPlanId = serverDeployPhase.RunPlanId;
            releaseDeployPhase2.DeploymentJobs = (IList<DeploymentJob>) deployStepRecords.ToList<DeploymentJob>();
          }
        }
      }
    }

    public static IList<ReleaseDefinitionApprovalStep> GetSnapshotApprovals(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      EnvironmentStepType stepType)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      List<ReleaseDefinitionApprovalStep> snapshotApprovals = new List<ReleaseDefinitionApprovalStep>();
      switch (stepType)
      {
        case EnvironmentStepType.PreDeploy:
          snapshotApprovals = environment.PreApprovalsSnapshot != null ? environment.PreApprovalsSnapshot.Approvals : snapshotApprovals;
          break;
        case EnvironmentStepType.PostDeploy:
          snapshotApprovals = environment.PostApprovalsSnapshot != null ? environment.PostApprovalsSnapshot.Approvals : snapshotApprovals;
          break;
      }
      return (IList<ReleaseDefinitionApprovalStep>) snapshotApprovals;
    }

    public static bool HasEnvironmentAtLeastOnePrePostApprovalSnapshot(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      IList<ReleaseDefinitionApprovalStep> snapshotApprovals1 = environment.GetSnapshotApprovals(EnvironmentStepType.PreDeploy);
      IList<ReleaseDefinitionApprovalStep> snapshotApprovals2 = environment.GetSnapshotApprovals(EnvironmentStepType.PostDeploy);
      return snapshotApprovals1.Count<ReleaseDefinitionApprovalStep>() > 0 && snapshotApprovals2.Count<ReleaseDefinitionApprovalStep>() > 0;
    }

    public static void CheckSnapshotApprovals(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      if (environment == null)
        return;
      ReleaseDefinitionApprovalStepExtensions.ValidateApprovalSteps((ICollection<ReleaseDefinitionApprovalStep>) environment.GetSnapshotApprovals(EnvironmentStepType.PreDeploy), environment.Name);
      ReleaseDefinitionApprovalStepExtensions.ValidateApprovalSteps((ICollection<ReleaseDefinitionApprovalStep>) environment.GetSnapshotApprovals(EnvironmentStepType.PostDeploy), environment.Name);
    }

    public static void PopulateApprovalsSnapshots(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment releaseEnvironment,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment,
      DefinitionEnvironmentData snapshotEnvironment)
    {
      if (releaseEnvironment == null || serverEnvironment == null)
        return;
      releaseEnvironment.PreApprovalsSnapshot.ApprovalOptions = serverEnvironment.PreApprovalOptions.ToWebApiApprovalOptions();
      releaseEnvironment.PostApprovalsSnapshot.ApprovalOptions = serverEnvironment.PostApprovalOptions.ToWebApiApprovalOptions();
      ReleaseEnvironmentContractExtensions.CopyReleaseDefinitionSnapshotApprovalsToReleaseDefinitionApprovals(releaseEnvironment, snapshotEnvironment);
    }

    public static void CopyReleaseDefinitionSnapshotApprovalsToReleaseDefinitionApprovals(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment releaseEnvironment,
      DefinitionEnvironmentData releaseDefinitionEnvironment)
    {
      if (releaseEnvironment == null || releaseDefinitionEnvironment == null)
        return;
      foreach (DefinitionEnvironmentStepData step in (IEnumerable<DefinitionEnvironmentStepData>) releaseDefinitionEnvironment.Steps)
      {
        ReleaseDefinitionApprovalStep definitionApprovalStep = step.ToReleaseDefinitionApprovalStep();
        switch (step.StepType)
        {
          case EnvironmentStepType.PreDeploy:
            releaseEnvironment.PreApprovalsSnapshot.Approvals.Add(definitionApprovalStep);
            continue;
          case EnvironmentStepType.PostDeploy:
            releaseEnvironment.PostApprovalsSnapshot.Approvals.Add(definitionApprovalStep);
            continue;
          default:
            continue;
        }
      }
      DefinitionEnvironmentStepData environmentStepData = releaseDefinitionEnvironment.Steps.FirstOrDefault<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (step => step.StepType == EnvironmentStepType.Deploy));
      if (environmentStepData == null)
        return;
      int deploymentGatesStepRank1 = releaseEnvironment.PreDeploymentGatesSnapshot.GetDeploymentGatesStepRank(releaseEnvironment.PreApprovalsSnapshot, 0);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions approvalOptions1 = releaseEnvironment.PreApprovalsSnapshot.ApprovalOptions;
      int num1 = (approvalOptions1 != null ? (int) approvalOptions1.ExecutionOrder : 1) == 1 ? 0 : deploymentGatesStepRank1;
      foreach (ReleaseDefinitionApprovalStep approval in releaseEnvironment.PreApprovalsSnapshot.Approvals)
        approval.Rank -= num1;
      int deploymentGatesStepRank2 = releaseEnvironment.PostDeploymentGatesSnapshot.GetDeploymentGatesStepRank(releaseEnvironment.PostApprovalsSnapshot, environmentStepData.Rank);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions approvalOptions2 = releaseEnvironment.PostApprovalsSnapshot.ApprovalOptions;
      int num2 = (approvalOptions2 != null ? (int) approvalOptions2.ExecutionOrder : 1) == 1 ? environmentStepData.Rank : deploymentGatesStepRank2;
      foreach (ReleaseDefinitionApprovalStep approval in releaseEnvironment.PostApprovalsSnapshot.Approvals)
        approval.Rank -= num2;
    }

    public static bool AreApprovalsParallel(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      EnvironmentStepType approvalType)
    {
      foreach (ReleaseDefinitionApprovalStep snapshotApproval in (IEnumerable<ReleaseDefinitionApprovalStep>) environment.GetSnapshotApprovals(approvalType))
      {
        if (snapshotApproval.Rank != 1)
          return false;
      }
      return true;
    }

    public static DeploymentAttempt GetDeploymentAttempt(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      int attempt)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      return environment.DeploySteps.FirstOrDefault<DeploymentAttempt>((Func<DeploymentAttempt, bool>) (step => step.Attempt == attempt));
    }

    internal static IEnumerable<TimelineRecord> GetTimelineRecords(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid runPlanId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes phaseType)
    {
      return DistributedTaskOrchestrator.CreateDistributedTaskOrchestrator(requestContext, projectId, phaseType).GetTimelineRecords(runPlanId);
    }

    internal static IEnumerable<TimelineRecord> GetGreenlightingTimelineRecords(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid runPlanId)
    {
      return new GreenlightingOrchestrator(requestContext, projectId).GetTimelineRecords(runPlanId);
    }
  }
}
