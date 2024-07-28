// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.IDataAccessLayer
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public interface IDataAccessLayer
  {
    Release AddRelease(Release release, string comment);

    Release UpdateDraftRelease(Release release, string comment);

    Release GetRelease(int id);

    ReleaseEnvironmentStep GetReleaseStep(int id);

    IEnumerable<ReleaseEnvironmentStep> AddReleaseSteps(
      Release release,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps,
      bool handleParallelApprovals);

    ReleaseEnvironmentStep UpdateReleaseStep(ReleaseEnvironmentStep releaseEnvironmentStep);

    IEnumerable<ReleaseEnvironmentStep> UpdateReleaseEnvironmentSteps(
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps);

    ReleaseDefinition GetReleaseDefinition(int id);

    IEnumerable<ReleaseEnvironmentStep> GetReleaseSteps(
      int releaseId,
      int releaseEnvironmentId,
      int stepRank,
      int trialNumber);

    DefinitionEnvironment GetDefinitionEnvironment(
      int releaseDefinitionId,
      int definitionEnvironmentId);

    QueuingPolicyResult GetReleaseEnvironmentsTorunAfterEnforcingQueuingPolicy(
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int releaseId,
      int releaseEnvironmentId,
      int maxConcurrent,
      int maxQueueDepth);

    void RemoveEnvironmentFromQueue(int releaseId, int releaseEnvironmentId);

    IEnumerable<ReleaseEnvironmentQueueData> GetUnhealthyReleaseEnvironments(
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int daysToCheck);

    Release UpdateReleaseStatus(Release release, ReleaseStatus status, string comment);

    Release StartDraftRelease(Release release, string comment);

    Release UpdateApprovalStepsStatus(
      int releaseId,
      int releaseEnvironmentId,
      EnvironmentStepType stepType,
      ReleaseEnvironmentStepStatus statusFrom,
      ReleaseEnvironmentStepStatus statusTo,
      string comment);

    void SetReleaseScheduledPromotion(
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentStepId,
      DateTime? deferredDateTime);

    void DeleteReleaseScheduledPromotion(int releaseId);

    Deployment QueueReleaseOnEnvironment(
      int releaseDefinitionId,
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentId,
      Guid requestedBy,
      Guid requestedFor,
      DeploymentReason reason,
      string comment);

    Release UpdateReleaseEnvironmentStatus(
      Release release,
      int releaseEnvironmentId,
      ReleaseEnvironmentStatus statusFrom,
      ReleaseEnvironmentStatus statusTo,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      string comment,
      int attempt);

    Release UpdateReleaseEnvironmentConditions(
      int releaseId,
      IEnumerable<ReleaseEnvironment> releaseEnvironments);

    Release UpdateEnvironmentAndDeploymentStatus(
      Release release,
      int releaseEnvironmentId,
      int attempt,
      ReleaseEnvironmentStatus completionStatus,
      DeploymentStatus deploymentStatus,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      DeploymentOperationStatus operationStatus);

    ReleaseDeployPhase AddDeployPhase(
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      int rank,
      DeployPhaseTypes phaseType);

    ReleaseDeployPhase UpdateDeployPhase(
      ReleaseDeployPhase releaseDeployPhase,
      DeploymentOperationStatus operationStatus,
      int definitionId);

    Release CancelDeploymentOnEnvironment(
      int releaseId,
      int releaseEnvironmentId,
      string comment,
      bool addCommentAsDeploymentIssue,
      bool evaluateForCanceling);

    IEnumerable<Release> RejectReleaseEnvironments(
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps);

    Release UpdateReleaseEnvironment(
      int releaseId,
      int releaseEnvironmentId,
      DateTime? scheduledDateTime,
      Guid? stageSchedulingJobId = null);

    ReleaseEnvironmentStep HandleEnvironmentDeployJobStarted(
      int releaseId,
      int releaseStepId,
      int releaseDeployPhaseId);

    void HandlePipelineAssigned(int releaseId, int releaseStepId);

    void HandleQueuedForPipeline(int releaseId, int releaseStepId);

    Deployment UpdateDeploymentOperationStatus(
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      DeploymentOperationStatus operationStatus);

    ManualIntervention AddManualIntervention(
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      TaskActivityData taskActivityData,
      string instructions);

    ManualIntervention GetManualIntervention(int releaseId, int manualInterventionId);

    IList<ManualIntervention> GetManualInterventionsForRelease(int releaseId);

    ManualIntervention UpdateManualIntervention(
      int releaseId,
      int manualInterventionId,
      Guid approvedBy,
      ManualInterventionStatus status,
      string comment);

    IEnumerable<PipelineArtifactSource> GetReleaseArtifactSources(
      IEnumerable<int> releases,
      string artifactTypeId);

    void SendReleaseUpdatedNotification(int definitionId, int releaseId);

    void SendReleaseEnvironmentUpdatedEvent(
      int definitionId,
      int releaseId,
      int releaseEnvironmentId);

    void SendReleaseApprovalPendingEvent(
      int definitionId,
      int releaseId,
      int releaseEnvironmentId,
      int approvalId,
      Guid approverId);

    void PublishCommitStatusForRelease(Release release, int releaseEnvironmentId);

    void SaveProperties(ArtifactSpec artifactSpec, IEnumerable<PropertyValue> propertyValue);

    void SaveProperties(
      IEnumerable<ArtifactPropertyValue> artifactPropertyValue);

    ReleaseEnvironmentData GetReleaseEnvironmentData(
      int releaseId,
      int releaseEnvironmentId,
      bool includeDeployments,
      bool includeApprovals,
      bool includeArtifacts);

    DeploymentGate AddDeploymentGate(
      int releaseId,
      int releaseEnvironmentId,
      int deploymentId,
      int stepId,
      EnvironmentStepType stepType);

    DeploymentGate GetDeploymentGate(int releaseId, int releaseEnvironmentId, int stepId);

    DeploymentGate UpdateDeploymentGate(
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      GateStatus status,
      Guid? runPlanId,
      DeploymentOperationStatus operationStatus,
      Guid changedBy);

    ReleaseEnvironmentSnapshotDelta AddDeploymentSnapshotDelta(
      int releaseId,
      int releaseEnvironmentId,
      int deploymentId,
      IEnumerable<DeploymentGroupPhaseDelta> deploymentGroupPhaseDelta,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables);

    ReleaseEnvironmentSnapshotDelta GetDeploymentSnapshotDelta(int releaseId, int deploymentId);

    IEnumerable<string> GetBuildsRetainedByReleases(IEnumerable<int> releaseIds);

    DeploymentGate IgnoreGates(
      int releaseId,
      int stepId,
      IEnumerable<string> gatesToIgnore,
      string beforeGatesIgnored,
      string afterGatesIgnored,
      string comment,
      bool markProcessed);

    ReleaseManagementJobInfo GetReleaseManagementJobInfo(string jobName, bool createIfNotExists);

    QueuingPolicyResult GetReleaseEnvironmentsTorunAfterCancelingScheduledReleases(
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int releaseId,
      int releaseEnvironmentId,
      int maxConcurrent,
      int maxQueueDepth);
  }
}
