// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.DeployPhaseOrchestrator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public class DeployPhaseOrchestrator
  {
    private readonly IVssRequestContext requestContext;
    private readonly Guid projectId;
    private readonly IDataAccessLayer dataAccessLayer;
    private readonly OrchestratorServiceProcessorV2 environmentOrchestrator;

    public DeployPhaseOrchestrator(IVssRequestContext requestContext, Guid projectId)
      : this(requestContext, projectId, (IDataAccessLayer) new DataAccessLayer(requestContext, projectId))
    {
      this.environmentOrchestrator = new OrchestratorServiceProcessorV2(requestContext, projectId);
    }

    protected DeployPhaseOrchestrator(
      IVssRequestContext requestContext,
      Guid projectId,
      IDataAccessLayer dataAccessLayer)
    {
      this.requestContext = requestContext;
      this.projectId = projectId;
      this.dataAccessLayer = dataAccessLayer;
    }

    public void ProcessDeployPhases(Release release, ReleaseEnvironmentStep deployStep) => this.ProcessDeployPhasesImplementation(release, deployStep, new Func<Release, ReleaseEnvironment, ReleaseEnvironmentStep, DeployPhaseSnapshot, DeployPhaseRunStatus>(this.RunDeployPhase), (Action<int, int, ReleaseEnvironmentStepStatus, Release>) ((relId, stepId, status, r) => this.environmentOrchestrator.AcceptDeployStep(stepId, status, r)), (Action<int, int, Release>) ((r, s, rel) => this.environmentOrchestrator.RejectStep(r, s, rel)), (Action<int, int, int?, Exception>) ((r, s, p, ex) => this.FailOrchestration(r, s, p, ex)));

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Dependency injection for unit test.")]
    public void ProcessDeployPhasesImplementation(
      Release release,
      ReleaseEnvironmentStep deployStep,
      Func<Release, ReleaseEnvironment, ReleaseEnvironmentStep, DeployPhaseSnapshot, DeployPhaseRunStatus> runDeployPhase,
      Action<int, int, ReleaseEnvironmentStepStatus, Release> acceptStep,
      Action<int, int, Release> rejectStep,
      Action<int, int, int?, Exception> handleException)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (deployStep == null)
        throw new ArgumentNullException(nameof (deployStep));
      ReleaseEnvironment environment = release.GetEnvironment(deployStep.ReleaseEnvironmentId);
      if (environment.IsYamlEnvironment())
      {
        PipelineOrchestrator pipelineOrchestrator = new PipelineOrchestrator(this.requestContext, this.projectId);
        AutomationEngineInput automationEngineInput = new DeployPhaseRunner(this.requestContext, this.projectId).GetAutomationEngineInput(release, environment, deployStep, deployStep.TrialNumber, (DeployPhaseSnapshot) null, (ReleaseEnvironmentSnapshotDelta) null);
        YamlDeploymentSnapshot deploymentSnapshot = (YamlDeploymentSnapshot) environment.DeploymentSnapshot;
        automationEngineInput.Process = deploymentSnapshot.Process;
        deployStep.RunPlanId = new Guid?(pipelineOrchestrator.StartDeployment(automationEngineInput));
        this.dataAccessLayer.UpdateReleaseStep(deployStep);
      }
      else if (DeployPhaseOrchestrator.UsePipelineOrchestrator(this.requestContext, environment))
      {
        PipelineOrchestrator pipelineOrchestrator = new PipelineOrchestrator(this.requestContext, this.projectId);
        AutomationEngineInput automationEngineInput = new DeployPhaseRunner(this.requestContext, this.projectId).GetAutomationEngineInput(release, environment, deployStep, deployStep.TrialNumber, (DeployPhaseSnapshot) null, (ReleaseEnvironmentSnapshotDelta) null);
        IList<DeployPhaseSnapshot> deployPhaseSnapshots = environment.GetDesignerDeployPhaseSnapshots();
        automationEngineInput.Process = deployPhaseSnapshots.GetPipelineProcess(this.requestContext, this.projectId, automationEngineInput);
        deployStep.RunPlanId = new Guid?(pipelineOrchestrator.StartDeployment(automationEngineInput));
        this.dataAccessLayer.UpdateReleaseStep(deployStep);
      }
      else
      {
        DeployPhaseSnapshot snapshotToProcess = environment.GetDesignerDeployPhaseSnapshots().Single<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (snapshot => snapshot.Rank == 1));
        this.InvokeRunner(release, deployStep, environment, snapshotToProcess, runDeployPhase, acceptStep, rejectStep, handleException);
      }
    }

    public bool CancelDeployPhase(Release release, ReleaseEnvironmentStep deployStep)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      return deployStep != null ? this.CancelDeployPhaseImplementation(release, deployStep, new Func<Release, ReleaseEnvironmentStep, bool>(this.CancelRunner)) : throw new ArgumentNullException(nameof (deployStep));
    }

    public void AcceptDeployPhase(
      int releaseId,
      int deployStepId,
      int releaseDeployPhaseId,
      DeployPhaseStatus deployPhaseStatus,
      DeploymentOperationStatus operationStatus)
    {
      this.HandleDeployPhaseCompletion(releaseId, deployStepId, releaseDeployPhaseId, deployPhaseStatus, operationStatus, new Func<Release, ReleaseEnvironment, ReleaseEnvironmentStep, DeployPhaseSnapshot, DeployPhaseRunStatus>(this.RunDeployPhase), (Action<int, int, ReleaseEnvironmentStepStatus, Release>) ((relId, stepId, status, release) => this.environmentOrchestrator.AcceptDeployStep(stepId, status, release)), (Action<int, int, Release>) ((relId, stepId, release) => this.environmentOrchestrator.RejectStep(relId, stepId, release)), (Action<int, int, int?, Exception>) ((relId, stepId, phaseId, ex) => this.FailOrchestration(releaseId, deployStepId, new int?(releaseDeployPhaseId), ex)), false);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Dependency injection for testability.")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "By Design")]
    public void HandleDeployPhaseCompletion(
      int releaseId,
      int deployStepId,
      int releaseDeployPhaseId,
      DeployPhaseStatus deployPhaseStatus,
      DeploymentOperationStatus operationStatus,
      Func<Release, ReleaseEnvironment, ReleaseEnvironmentStep, DeployPhaseSnapshot, DeployPhaseRunStatus> runDeployPhase,
      Action<int, int, ReleaseEnvironmentStepStatus, Release> acceptStep,
      Action<int, int, Release> rejectStep,
      Action<int, int, int?, Exception> handleException,
      bool skipIfCanceledAlready)
    {
      bool shouldUpdateIfAnyStatus = !skipIfCanceledAlready || !deployPhaseStatus.Equals((object) DeployPhaseStatus.Canceled);
      Action<Release, ReleaseEnvironment, ReleaseEnvironmentStep, ReleaseDeployPhase> moveWorkflowForward = (Action<Release, ReleaseEnvironment, ReleaseEnvironmentStep, ReleaseDeployPhase>) ((r, e, s, p) => DeployPhaseOrchestrator.MoveWorkflowForward(r, e, s, p, runDeployPhase, acceptStep, rejectStep));
      this.UpdateDeployPhaseAndMoveWorkflowForward(releaseId, deployStepId, releaseDeployPhaseId, deployPhaseStatus, operationStatus, moveWorkflowForward, handleException, shouldUpdateIfAnyStatus);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "By Design")]
    public void RejectDeployPhase(
      int releaseId,
      int deployStepId,
      int releaseDeployPhaseId,
      DeployPhaseStatus deployPhaseStatus,
      DeploymentOperationStatus operationStatus,
      bool skipIfCanceledAlready = false)
    {
      this.HandleDeployPhaseCompletion(releaseId, deployStepId, releaseDeployPhaseId, deployPhaseStatus, operationStatus, new Func<Release, ReleaseEnvironment, ReleaseEnvironmentStep, DeployPhaseSnapshot, DeployPhaseRunStatus>(this.RunDeployPhase), (Action<int, int, ReleaseEnvironmentStepStatus, Release>) ((relId, stepId, status, release) => this.environmentOrchestrator.AcceptDeployStep(stepId, status, release)), (Action<int, int, Release>) ((relId, stepId, release) => this.environmentOrchestrator.RejectStep(relId, stepId, release)), (Action<int, int, int?, Exception>) ((relId, stepId, phaseId, ex) => this.FailOrchestration(releaseId, deployStepId, new int?(releaseDeployPhaseId), ex)), skipIfCanceledAlready);
    }

    public virtual void SendCancelUpdateNotification(
      Release release,
      int environmentId,
      string comment)
    {
      this.environmentOrchestrator.SendCancelUpdateNotification(release, environmentId, comment, 1972120);
    }

    public void FailOrchestration(
      int releaseId,
      int releaseStepId,
      int? releaseDeployPhaseId,
      Exception ex)
    {
      this.FailOrchestrationImplementation(releaseId, releaseStepId, releaseDeployPhaseId, (Action<int, int, Release>) ((relId, stepId, release) => this.environmentOrchestrator.RejectStep(relId, stepId, release)), ex);
    }

    public void FailOrchestrationImplementation(
      int releaseId,
      int releaseStepId,
      int? releaseDeployPhaseId,
      Action<int, int, Release> rejectStep,
      Exception ex)
    {
      if (rejectStep == null)
        throw new ArgumentNullException(nameof (rejectStep));
      if (ex == null)
        throw new ArgumentNullException(nameof (ex));
      if (!(ex.GetType() == typeof (ReleaseManagementHandledException)))
      {
        this.requestContext.TraceException(1972130, "ReleaseManagementService", "Pipeline", ex);
        try
        {
          Release release = this.dataAccessLayer.GetRelease(releaseId);
          ReleaseEnvironmentStep step = release.GetStep(releaseStepId);
          this.CreateWorkflowFailedEvent(releaseId, step, ex);
          if (releaseDeployPhaseId.HasValue)
          {
            ReleaseDeployPhase deployPhase = release.GetEnvironment(step.ReleaseEnvironmentId).GetDeployPhase(releaseDeployPhaseId.Value);
            if (deployPhase.Status != DeployPhaseStatus.Canceled)
            {
              deployPhase.Status = DeployPhaseStatus.Failed;
              deployPhase.Logs += TeamFoundationExceptionFormatter.FormatException(ex, false);
              this.dataAccessLayer.UpdateDeployPhase(deployPhase, DeploymentOperationStatus.PhaseFailed, release.ReleaseDefinitionId);
            }
          }
          else
          {
            step.Logs += TeamFoundationExceptionFormatter.FormatException(ex, false);
            this.dataAccessLayer.UpdateReleaseStep(step);
          }
          rejectStep(releaseId, releaseStepId, release);
        }
        catch (Exception ex1)
        {
          this.requestContext.TraceException(1972130, "ReleaseManagementService", "Pipeline", ex1);
          throw;
        }
        throw new ReleaseManagementHandledException(ex);
      }
    }

    private static bool UsePipelineOrchestrator(
      IVssRequestContext requestContext,
      ReleaseEnvironment releaseEnvironment)
    {
      return requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.UsePipelineOrchestrator.DeployStep") && !releaseEnvironment.HasPhaseType(DeployPhaseTypes.RunOnServer) && !releaseEnvironment.HasPhaseType(DeployPhaseTypes.MachineGroupBasedDeployment);
    }

    private static void MoveWorkflowForward(
      Release release,
      ReleaseEnvironment releaseEnvironment,
      ReleaseEnvironmentStep deployStep,
      ReleaseDeployPhase deployPhase,
      Func<Release, ReleaseEnvironment, ReleaseEnvironmentStep, DeployPhaseSnapshot, DeployPhaseRunStatus> runDeployPhase,
      Action<int, int, ReleaseEnvironmentStepStatus, Release> acceptStep,
      Action<int, int, Release> rejectStep)
    {
      if (releaseEnvironment.Status == ReleaseEnvironmentStatus.Canceled)
      {
        rejectStep(release.Id, deployStep.Id, release);
      }
      else
      {
        IList<DeployPhaseSnapshot> deployPhaseSnapshots = ((DesignerDeploymentSnapshot) releaseEnvironment.DeploymentSnapshot).DeployPhaseSnapshots;
        DeployPhaseSnapshot deployPhaseSnapshot = deployPhaseSnapshots.SingleOrDefault<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (s => s.Rank == deployPhase.Rank + 1));
        int i = 1;
        while (deployPhaseSnapshot != null && !runDeployPhase(release, releaseEnvironment, deployStep, deployPhaseSnapshot).HasStarted)
        {
          deployPhaseSnapshot = deployPhaseSnapshots.SingleOrDefault<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (s => s.Rank == deployPhase.Rank + 1 + i));
          i++;
        }
        if (deployPhaseSnapshot != null)
          return;
        if (releaseEnvironment.ReleaseDeployPhases.Where<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (d => d.Attempt == deployPhase.Attempt)).Any<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (p => p.Status == DeployPhaseStatus.Failed || p.Status == DeployPhaseStatus.Canceled)))
        {
          rejectStep(release.Id, deployStep.Id, release);
        }
        else
        {
          ReleaseEnvironmentStepStatus environmentStepStatus = releaseEnvironment.ReleaseDeployPhases.Where<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (d => d.Attempt == deployPhase.Attempt)).Any<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (p => p.Status == DeployPhaseStatus.PartiallySucceeded)) ? ReleaseEnvironmentStepStatus.PartiallySucceeded : ReleaseEnvironmentStepStatus.Done;
          acceptStep(release.Id, deployStep.Id, environmentStepStatus, release);
        }
      }
    }

    private void CreateWorkflowFailedEvent(
      int releaseId,
      ReleaseEnvironmentStep deployStep,
      Exception ex)
    {
      switch (ex)
      {
        case ReleaseManagementExternalServiceException _:
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException _:
          break;
        case DeploymentGroupNotFoundException _:
          break;
        case PipelineValidationException _:
          break;
        case Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException _:
          break;
        default:
          string message = TeamFoundationExceptionFormatter.FormatException(ex, false);
          RmTelemetryFactory.GetLogger(this.requestContext).PublishWorkflowFailedEvent(this.requestContext, releaseId, this.projectId, deployStep, message);
          KpiHelper.CreateWorkflowFailedEvent(this.requestContext, releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1);
          break;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "catching to handle the exception.")]
    private void UpdateDeployPhaseAndMoveWorkflowForward(
      int releaseId,
      int deployStepId,
      int releaseDeployPhaseId,
      DeployPhaseStatus deployPhaseStatus,
      DeploymentOperationStatus operationStatus,
      Action<Release, ReleaseEnvironment, ReleaseEnvironmentStep, ReleaseDeployPhase> moveWorkflowForward,
      Action<int, int, int?, Exception> handleException,
      bool shouldUpdateIfAnyStatus = true)
    {
      bool flag = false;
      try
      {
        Release release = this.dataAccessLayer.GetRelease(releaseId);
        ReleaseEnvironmentStep step = release.GetStep(deployStepId);
        ReleaseEnvironment environment = release.GetEnvironment(step.ReleaseEnvironmentId);
        ReleaseDeployPhase deployPhase = environment.GetDeployPhase(releaseDeployPhaseId);
        if (environment.Status == ReleaseEnvironmentStatus.Canceled)
        {
          this.TraceInformationMessage(1972120, "Already deployment updated for release: {0}, environment: {1}, in {2} state, deployment with trialNumber: {3} .", (object) release.Id, (object) environment.Id, (object) environment.Status, (object) step.TrialNumber);
        }
        else
        {
          if (shouldUpdateIfAnyStatus || deployPhase.Status != deployPhaseStatus)
          {
            deployPhase.Status = deployPhaseStatus;
            try
            {
              this.dataAccessLayer.UpdateDeployPhase(deployPhase, operationStatus, release.ReleaseDefinitionId);
            }
            catch (DeploymentUpdateNotAllowedException ex)
            {
              DeploymentOperationStatus deploymentOperationStatus1 = (DeploymentOperationStatus) ex.Data[(object) "currentOperationStatus"];
              DeploymentOperationStatus deploymentOperationStatus2 = (DeploymentOperationStatus) ex.Data[(object) "currentDeploymentStatus"];
              if (deploymentOperationStatus1 == DeploymentOperationStatus.Cancelling)
                this.SendCancelUpdateNotification(this.dataAccessLayer.CancelDeploymentOnEnvironment(release.Id, environment.Id, string.Empty, false, false), environment.Id, step.ApproverComment);
              this.TraceInformationMessage(1972120, "Already deployment updated for release: {0}, environment: {1}, deployment with trialnumber {2}, operationStatus {3}, deploymentStatus {4}, desiredOperationStatus {5}.", (object) releaseId, (object) step.ReleaseEnvironmentId, (object) step.TrialNumber, (object) deploymentOperationStatus1, (object) deploymentOperationStatus2, (object) operationStatus);
              return;
            }
          }
          flag = true;
          moveWorkflowForward(release, environment, step, deployPhase);
        }
      }
      catch (Exception ex)
      {
        if (flag)
          handleException(releaseId, deployStepId, new int?(), ex);
        else
          handleException(releaseId, deployStepId, new int?(releaseDeployPhaseId), ex);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "for logging purposes")]
    private void InvokeRunner(
      Release release,
      ReleaseEnvironmentStep deployStep,
      ReleaseEnvironment releaseEnvironment,
      DeployPhaseSnapshot snapshotToProcess,
      Func<Release, ReleaseEnvironment, ReleaseEnvironmentStep, DeployPhaseSnapshot, DeployPhaseRunStatus> runDeployPhase,
      Action<int, int, ReleaseEnvironmentStepStatus, Release> acceptStep,
      Action<int, int, Release> rejectStep,
      Action<int, int, int?, Exception> handleException)
    {
      if (handleException == null)
        throw new ArgumentNullException(nameof (handleException));
      try
      {
        DeployPhaseRunStatus deployPhaseRunStatus = runDeployPhase(release, releaseEnvironment, deployStep, snapshotToProcess);
        if (deployPhaseRunStatus.HasStarted)
          return;
        this.HandleDeployPhaseCompletion(release.Id, deployStep.Id, deployPhaseRunStatus.ReleaseDeployPhaseId, DeployPhaseStatus.Skipped, DeploymentOperationStatus.Undefined, runDeployPhase, acceptStep, rejectStep, handleException, false);
      }
      catch (Exception ex)
      {
        handleException(release.Id, deployStep.Id, new int?(), ex);
      }
    }

    private DeployPhaseRunStatus RunDeployPhase(
      Release release,
      ReleaseEnvironment releaseEnvironment,
      ReleaseEnvironmentStep deployStep,
      DeployPhaseSnapshot snapshotToProcess)
    {
      return new DeployPhaseRunner(this.requestContext, this.projectId).Run(release, releaseEnvironment, deployStep, snapshotToProcess);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Would hinder readability as it would require re-ordering")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "cancelRunner is never null")]
    private bool CancelDeployPhaseImplementation(
      Release release,
      ReleaseEnvironmentStep deployStep,
      Func<Release, ReleaseEnvironmentStep, bool> cancelRunner)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (deployStep == null)
        throw new ArgumentNullException(nameof (deployStep));
      return cancelRunner(release, deployStep);
    }

    private bool CancelRunner(Release release, ReleaseEnvironmentStep deployStep) => new DeployPhaseRunner(this.requestContext, this.projectId).Cancel(release, deployStep);

    private void TraceInformationMessage(int tracePoint, string format, params object[] args) => VssRequestContextExtensions.Trace(this.requestContext, tracePoint, TraceLevel.Info, "ReleaseManagementService", "Pipeline", format, args);
  }
}
