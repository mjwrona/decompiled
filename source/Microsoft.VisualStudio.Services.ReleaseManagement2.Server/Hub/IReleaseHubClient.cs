// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Hub.IReleaseHubClient
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events;
using Microsoft.VisualStudio.Services.SignalR;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Hub
{
  public interface IReleaseHubClient
  {
    void OnReleaseCreated(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId);

    void OnReleaseUpdated2(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int releaseId);

    void OnReleaseEnvironmentUpdated(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int releaseId,
      int releaseEnvironmentId);

    void OnDeployJobStartedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentId);

    void OnReleaseTasksUpdated(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int phaseId,
      int stepId,
      Guid jobTimelineRecordId,
      Guid planId,
      SignalROperation timelineRecords);

    void OnReleaseTaskLogUpdated(
      IVssRequestContext requestContext,
      ReleaseTaskLogUpdatedEvent notificationEvent);

    void OnPlanGroupsStarted(
      IVssRequestContext requestContext,
      TaskOrchestrationPlanGroupsStartedEvent notificationEvent);

    void OnReleaseEnvironmentStatusUpdated(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int releaseId,
      int releaseEnvironmentId,
      EnvironmentStatus releaseEnvironmentStatus,
      DeploymentStatus deploymentStatus,
      DeploymentOperationStatus deploymentOperationStatus);

    void OnDefinitionReleaseUpdated(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int releaseId,
      int environmentId);

    void OnReleaseApprovalPending(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int releaseId,
      int releaseEnvironmentId,
      int approvalId,
      Guid approverId);
  }
}
