// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.AutomationEngineNotification
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class AutomationEngineNotification : IAutomationEngineNotification
  {
    private readonly Action<IVssRequestContext, Guid, int, int, int, DeployPhaseStatus, DeploymentOperationStatus> acceptDeployPhase;
    private readonly Action<IVssRequestContext, Guid, int, int, int, DeployPhaseStatus, DeploymentOperationStatus> rejectDeployPhase;

    public AutomationEngineNotification()
      : this(AutomationEngineNotification.\u003C\u003EO.\u003C0\u003E__AcceptDeployPhase ?? (AutomationEngineNotification.\u003C\u003EO.\u003C0\u003E__AcceptDeployPhase = new Action<IVssRequestContext, Guid, int, int, int, DeployPhaseStatus, DeploymentOperationStatus>(AutomationEngineNotification.AcceptDeployPhase)), AutomationEngineNotification.\u003C\u003EO.\u003C1\u003E__RejectDeployPhase ?? (AutomationEngineNotification.\u003C\u003EO.\u003C1\u003E__RejectDeployPhase = new Action<IVssRequestContext, Guid, int, int, int, DeployPhaseStatus, DeploymentOperationStatus>(AutomationEngineNotification.RejectDeployPhase)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    protected AutomationEngineNotification(
      Action<IVssRequestContext, Guid, int, int, int, DeployPhaseStatus, DeploymentOperationStatus> acceptDeployPhase,
      Action<IVssRequestContext, Guid, int, int, int, DeployPhaseStatus, DeploymentOperationStatus> rejectDeployPhase)
    {
      this.acceptDeployPhase = acceptDeployPhase;
      this.rejectDeployPhase = rejectDeployPhase;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This will be implemented in pipeline")]
    public void DeployPhaseComplete(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseStepId,
      int releaseDeployPhaseId,
      TaskResult taskResult)
    {
      switch (taskResult)
      {
        case TaskResult.Succeeded:
          this.acceptDeployPhase(requestContext, projectId, releaseId, releaseStepId, releaseDeployPhaseId, DeployPhaseStatus.Succeeded, DeploymentOperationStatus.PhaseSucceeded);
          break;
        case TaskResult.SucceededWithIssues:
          this.acceptDeployPhase(requestContext, projectId, releaseId, releaseStepId, releaseDeployPhaseId, DeployPhaseStatus.PartiallySucceeded, DeploymentOperationStatus.PhasePartiallySucceeded);
          break;
        case TaskResult.Failed:
          this.rejectDeployPhase(requestContext, projectId, releaseId, releaseStepId, releaseDeployPhaseId, DeployPhaseStatus.Failed, DeploymentOperationStatus.PhaseFailed);
          break;
        case TaskResult.Canceled:
        case TaskResult.Abandoned:
          this.rejectDeployPhase(requestContext, projectId, releaseId, releaseStepId, releaseDeployPhaseId, DeployPhaseStatus.Canceled, DeploymentOperationStatus.PhaseCanceled);
          break;
      }
    }

    public void JobStarted(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseStepId,
      int releaseDeployPhaseId)
    {
      new OrchestratorServiceProcessorV2(requestContext, projectId).HandleJobStarted(releaseId, releaseStepId, releaseDeployPhaseId);
    }

    public void PipelineAssigned(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseStepId)
    {
      new OrchestratorServiceProcessorV2(requestContext, projectId).HandlePipelineAssigned(releaseId, releaseStepId);
    }

    public void QueuedForPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseStepId)
    {
      new OrchestratorServiceProcessorV2(requestContext, projectId).HandleQueuedForPipeline(releaseId, releaseStepId);
    }

    private static void AcceptDeployPhase(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseStepId,
      int releaseDeployPhaseId,
      DeployPhaseStatus deployPhaseStatus,
      DeploymentOperationStatus operationStatus)
    {
      if (releaseDeployPhaseId != 0)
      {
        new DeployPhaseOrchestrator(requestContext, projectId).AcceptDeployPhase(releaseId, releaseStepId, releaseDeployPhaseId, deployPhaseStatus, operationStatus);
      }
      else
      {
        OrchestratorServiceProcessorV2 serviceProcessorV2 = new OrchestratorServiceProcessorV2(requestContext, projectId);
        ReleaseEnvironmentStepStatus environmentStepStatus = deployPhaseStatus == DeployPhaseStatus.PartiallySucceeded ? ReleaseEnvironmentStepStatus.PartiallySucceeded : ReleaseEnvironmentStepStatus.Done;
        int releaseId1 = releaseId;
        int deployStepId = releaseStepId;
        int status = (int) environmentStepStatus;
        serviceProcessorV2.AcceptDeployStep(releaseId1, deployStepId, (ReleaseEnvironmentStepStatus) status);
      }
    }

    private static void RejectDeployPhase(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseStepId,
      int releaseDeployPhaseId,
      DeployPhaseStatus deployPhaseStatus,
      DeploymentOperationStatus operationStatus)
    {
      if (releaseDeployPhaseId != 0)
        new DeployPhaseOrchestrator(requestContext, projectId).RejectDeployPhase(releaseId, releaseStepId, releaseDeployPhaseId, deployPhaseStatus, operationStatus, true);
      else
        new OrchestratorServiceProcessorV2(requestContext, projectId).RejectDeployStep(releaseId, releaseStepId);
    }
  }
}
