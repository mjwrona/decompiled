// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.GreenlightingRunner
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public class GreenlightingRunner
  {
    private readonly IVssRequestContext requestContext;
    private readonly Guid projectId;
    private readonly IDataAccessLayer dataAccessLayer;
    private readonly IEnvironmentOrchestrator environmentOrchestrator;
    private readonly IDistributedTaskOrchestrator orchestrator;

    public GreenlightingRunner(IVssRequestContext requestContext, Guid projectId)
    {
      this.requestContext = requestContext;
      this.projectId = projectId;
      this.dataAccessLayer = (IDataAccessLayer) new DataAccessLayer(requestContext, projectId);
      this.environmentOrchestrator = (IEnvironmentOrchestrator) new OrchestratorServiceProcessorV2(requestContext, projectId);
      this.orchestrator = (IDistributedTaskOrchestrator) new GreenlightingOrchestrator(this.requestContext, this.projectId);
    }

    internal GreenlightingRunner(
      IVssRequestContext requestContext,
      Guid projectId,
      IDataAccessLayer dataAccessLayer,
      IEnvironmentOrchestrator orchestratorServiceProcessor,
      IDistributedTaskOrchestrator greenlightingOrchestrator)
    {
      this.requestContext = requestContext;
      this.projectId = projectId;
      this.dataAccessLayer = dataAccessLayer;
      this.environmentOrchestrator = orchestratorServiceProcessor;
      this.orchestrator = greenlightingOrchestrator;
    }

    public void Run(Release release, ReleaseEnvironmentStep step)
    {
      this.TraceInformation("Executing {0} greenlighting step. ReleaseId: {1}, StepId: {2}", (object) step.StepType, (object) release.Id, (object) step.Id);
      ReleaseEnvironment environment = release.GetEnvironment(step.ReleaseEnvironmentId);
      Deployment deploymentByAttempt = environment.GetDeploymentByAttempt(step.TrialNumber);
      ReleaseDefinitionGatesOptions gatesOptions = release.GetEnvironment(step.ReleaseEnvironmentId)?.GetGateStepData(step.StepType)?.GatesOptions;
      if (gatesOptions == null || !gatesOptions.IsEnabled)
        return;
      DeploymentGate deploymentGate1 = this.dataAccessLayer.AddDeploymentGate(release.Id, environment.Id, deploymentByAttempt.Id, step.Id, step.StepType);
      AutomationEngineInput automationEngineInput = this.GetAutomationEngineInput(release, environment, step);
      DeploymentGate deploymentGate2;
      try
      {
        Guid guid = this.orchestrator.StartDeployment(automationEngineInput);
        deploymentGate2 = this.dataAccessLayer.UpdateDeploymentGate(deploymentGate1.ReleaseId, step.ReleaseEnvironmentId, deploymentGate1.ReleaseEnvironmentStepId, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus.InProgress, new Guid?(guid), DeploymentOperationStatus.EvaluatingGates, this.requestContext.GetUserId());
        this.requestContext.SendReleaseEnvironmentStatusUpdatedNotification(release.ProjectId, release.ReleaseDefinitionId, release.Id, environment.Id, environment.Status.ToWebApi(), deploymentByAttempt.Status.ToWebApi(), DeploymentOperationStatus.EvaluatingGates.ToWebApi());
      }
      catch (Exception ex)
      {
        this.TraceError("Problem in starting greenlighting execution. ReleaseId: {0}, StepId: {1}", (object) release.Id, (object) step.Id);
        step.Logs += TeamFoundationExceptionFormatter.FormatException(ex, false);
        this.dataAccessLayer.UpdateReleaseStep(step);
        this.dataAccessLayer.UpdateDeploymentOperationStatus(release.Id, environment.Id, deploymentByAttempt.Attempt, DeploymentOperationStatus.Canceled);
        this.requestContext.SendReleaseEnvironmentStatusUpdatedNotification(release.ProjectId, release.ReleaseDefinitionId, release.Id, environment.Id, environment.Status.ToWebApi(), deploymentByAttempt.Status.ToWebApi(), DeploymentOperationStatus.Canceled.ToWebApi());
        Guid userId = this.requestContext.GetUserId(true);
        this.environmentOrchestrator.RejectStep(step, userId, string.Empty, release);
        throw;
      }
      deploymentByAttempt.DeploymentGates.Add(deploymentGate2);
      this.dataAccessLayer.SendReleaseEnvironmentUpdatedEvent(release.ReleaseDefinitionId, release.Id, step.ReleaseEnvironmentId);
    }

    public void Cancel(int releaseId, int stepId, Guid planId)
    {
      this.TraceInformation("Cancelling greenlighting plan. ReleaseId: {0}, StepId: {1}", (object) releaseId, (object) stepId);
      this.orchestrator.CancelDeployment(planId, TimeSpan.FromMinutes(1.0));
    }

    public void CompleteGreenlighting(
      IVssRequestContext context,
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      TaskResult result)
    {
      this.TraceInformation("Completed greenlighting plan. ReleaseId: {0}, StepId: {1}, Result: {2}", (object) releaseId, (object) stepId, (object) result);
      Release release = context.GetService<ReleasesService>().GetRelease(context, this.projectId, releaseId);
      switch (result)
      {
        case TaskResult.Succeeded:
        case TaskResult.SucceededWithIssues:
          this.UpdateDeploymentGateAndMoveWorkflowForward(releaseId, releaseEnvironmentId, stepId, release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus.Succeeded, DeploymentOperationStatus.Undefined, true);
          break;
        case TaskResult.Failed:
        case TaskResult.Abandoned:
          this.UpdateDeploymentGateAndMoveWorkflowForward(releaseId, releaseEnvironmentId, stepId, release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus.Failed, DeploymentOperationStatus.GateFailed, false);
          break;
        case TaskResult.Canceled:
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus gateStatus = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus.Failed;
          DeploymentOperationStatus operationStatus = DeploymentOperationStatus.GateFailed;
          if (GreenlightingRunner.GetDeploymentOperationStatus(release, releaseEnvironmentId, stepId) == DeploymentOperationStatus.Canceled)
          {
            gateStatus = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus.Canceled;
            operationStatus = DeploymentOperationStatus.Canceled;
          }
          this.UpdateDeploymentGateAndMoveWorkflowForward(releaseId, releaseEnvironmentId, stepId, release, gateStatus, operationStatus, false);
          this.dataAccessLayer.SendReleaseEnvironmentUpdatedEvent(release.ReleaseDefinitionId, release.Id, releaseEnvironmentId);
          break;
      }
    }

    public void HandleStabilizationCompletion(
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      Guid runPlanId)
    {
      this.requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, DeploymentGate>((Func<DeploymentSqlComponent, DeploymentGate>) (component => component.HandleGreenlightingStabilizationCompletion(this.projectId, releaseId, releaseEnvironmentId, stepId, runPlanId)));
      this.dataAccessLayer.SendReleaseEnvironmentUpdatedEvent(0, releaseId, releaseEnvironmentId);
    }

    public void UpdateSucceedingSince(
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      Guid runPlanId,
      DateTime? succeedingSince)
    {
      this.requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, DeploymentGate>((Func<DeploymentSqlComponent, DeploymentGate>) (component => component.UpdateGreenlightingSucceedingSince(this.projectId, releaseId, releaseEnvironmentId, stepId, runPlanId, succeedingSince)));
      this.dataAccessLayer.SendReleaseEnvironmentUpdatedEvent(0, releaseId, releaseEnvironmentId);
    }

    private static string GetStabilizationTime(ReleaseDefinitionGatesStep gates) => gates != null && gates.GatesOptions != null ? (gates.GatesOptions.StabilizationTime < 0 ? 0 : gates.GatesOptions.StabilizationTime).ToString((IFormatProvider) CultureInfo.InvariantCulture) : 0.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    private static string GetTimeout(ReleaseDefinitionGatesStep gates)
    {
      int num;
      string timeout;
      if (gates == null)
      {
        timeout = (string) null;
      }
      else
      {
        ReleaseDefinitionGatesOptions gatesOptions = gates.GatesOptions;
        if (gatesOptions == null)
        {
          timeout = (string) null;
        }
        else
        {
          num = gatesOptions.Timeout;
          timeout = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        }
      }
      if (timeout != null)
        return timeout;
      num = 2880;
      return num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    private static string GetMinimumSuccessfulMinutes(ReleaseDefinitionGatesStep gates) => gates != null && gates.GatesOptions != null ? (gates.GatesOptions.MinimumSuccessDuration < 0 ? 0 : gates.GatesOptions.MinimumSuccessDuration).ToString((IFormatProvider) CultureInfo.InvariantCulture) : 0.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    private static void UpdateGateAndDeploymentStatus(
      Release release,
      int releaseEnvironmentId,
      int stepId,
      DeploymentOperationStatus operationStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus newGateStatus)
    {
      ReleaseEnvironment environment = release.GetEnvironment(releaseEnvironmentId);
      Deployment deploymentByAttempt = environment.GetDeploymentByAttempt(environment.GetStepsForTests.Single<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Id == stepId)).TrialNumber);
      deploymentByAttempt.DeploymentGates.Single<DeploymentGate>((Func<DeploymentGate, bool>) (g => g.ReleaseEnvironmentStepId == stepId)).Status = newGateStatus;
      if (operationStatus == DeploymentOperationStatus.Undefined)
        return;
      deploymentByAttempt.OperationStatus = operationStatus;
    }

    private static DeploymentOperationStatus GetDeploymentOperationStatus(
      Release release,
      int releaseEnvironmentId,
      int stepId)
    {
      ReleaseEnvironment releaseEnvironment = release != null ? release.GetEnvironment(releaseEnvironmentId) : throw new ArgumentNullException(nameof (release));
      return releaseEnvironment.GetDeploymentByAttempt(releaseEnvironment.GetStepsForTests.Single<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Id == stepId)).TrialNumber).OperationStatus;
    }

    private string GetSamplingInterval(ReleaseDefinitionGatesStep gates)
    {
      int intervalInMinutes = DeploymentGatesHelper.GetMinimalSamplingIntervalInMinutes(this.requestContext, gates?.GatesOptions?.Timeout ?? new int?());
      return gates != null && gates.GatesOptions != null ? (gates.GatesOptions.SamplingInterval < intervalInMinutes ? intervalInMinutes : gates.GatesOptions.SamplingInterval).ToString((IFormatProvider) CultureInfo.InvariantCulture) : intervalInMinutes.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    private void UpdateDeploymentGateAndMoveWorkflowForward(
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus gateStatus,
      DeploymentOperationStatus operationStatus,
      bool acceptGateStep)
    {
      this.dataAccessLayer.UpdateDeploymentGate(releaseId, releaseEnvironmentId, stepId, gateStatus, new Guid?(), operationStatus, this.requestContext.GetUserId());
      GreenlightingRunner.UpdateGateAndDeploymentStatus(release, releaseEnvironmentId, stepId, operationStatus, gateStatus);
      if (acceptGateStep)
        this.environmentOrchestrator.AcceptDeployStep(stepId, ReleaseEnvironmentStepStatus.Done, release);
      else
        this.environmentOrchestrator.RejectGreenlightingStep(release, releaseEnvironmentId, stepId);
    }

    private AutomationEngineInput GetAutomationEngineInput(
      Release release,
      ReleaseEnvironment environment,
      ReleaseEnvironmentStep step)
    {
      DeployPhaseRunner deployPhaseRunner = new DeployPhaseRunner(this.requestContext, this.projectId);
      ReleaseDefinitionGatesStep gateStepData = environment.GetGateStepData(step.StepType);
      ServerDeploymentInput serverDeploymentInput = new ServerDeploymentInput();
      serverDeploymentInput.JobCancelTimeoutInMinutes = 1;
      serverDeploymentInput.TimeoutInMinutes = 60;
      ServerDeploymentInput o = serverDeploymentInput;
      List<WorkflowTask> workflowTaskList = step.StepType == EnvironmentStepType.PreGate ? environment.PreDeploymentGates.Gates.SelectMany<ReleaseDefinitionGate, WorkflowTask>((Func<ReleaseDefinitionGate, IEnumerable<WorkflowTask>>) (g => (IEnumerable<WorkflowTask>) g.Tasks)).ToList<WorkflowTask>() : environment.PostDeploymentGates.Gates.SelectMany<ReleaseDefinitionGate, WorkflowTask>((Func<ReleaseDefinitionGate, IEnumerable<WorkflowTask>>) (g => (IEnumerable<WorkflowTask>) g.Tasks)).ToList<WorkflowTask>();
      DeployPhaseSnapshot deployPhaseSnapshot = new DeployPhaseSnapshot()
      {
        DeploymentInput = JObject.FromObject((object) o),
        Rank = 1,
        Workflow = (IList<WorkflowTask>) workflowTaskList,
        PhaseType = DeployPhaseTypes.RunOnServer
      };
      Release release1 = release;
      ReleaseEnvironment releaseEnvironment = environment;
      ReleaseEnvironmentStep step1 = step;
      int trialNumber = step.TrialNumber;
      DeployPhaseSnapshot snapshotToProcess = deployPhaseSnapshot;
      AutomationEngineInput automationEngineInput = deployPhaseRunner.GetAutomationEngineInput(release1, releaseEnvironment, step1, trialNumber, snapshotToProcess, (ReleaseEnvironmentSnapshotDelta) null);
      automationEngineInput.Data["GreenlightingSamplingIntervalInMinutes"] = this.GetSamplingInterval(gateStepData);
      automationEngineInput.Data["GreenlightingStabilizationTimeInMinutes"] = GreenlightingRunner.GetStabilizationTime(gateStepData);
      automationEngineInput.Data["GreenlightingJobTimeoutInMinutes"] = GreenlightingRunner.GetTimeout(gateStepData);
      automationEngineInput.Data["GreenlightingMinimumSuccessfulMinutes"] = GreenlightingRunner.GetMinimumSuccessfulMinutes(gateStepData);
      if (step.StepType == EnvironmentStepType.PostGate)
      {
        ReleaseEnvironmentStep completedDeployStep = environment.GetLatestCompletedDeployStep();
        if (completedDeployStep != null)
          automationEngineInput.Data["DeploymentEndTime"] = completedDeployStep.ModifiedOn.ToString("u", (IFormatProvider) CultureInfo.InvariantCulture);
      }
      return automationEngineInput;
    }

    private void TraceInformation(string format, params object[] args) => VssRequestContextExtensions.Trace(this.requestContext, 1976420, TraceLevel.Info, "ReleaseManagementService", "Pipeline", format, args);

    private void TraceError(string format, params object[] args) => VssRequestContextExtensions.Trace(this.requestContext, 1976420, TraceLevel.Error, "ReleaseManagementService", "Pipeline", format, args);
  }
}
