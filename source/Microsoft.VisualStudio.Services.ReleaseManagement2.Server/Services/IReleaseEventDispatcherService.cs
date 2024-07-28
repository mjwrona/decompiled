// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.IReleaseEventDispatcherService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  [DefaultServiceImplementation(typeof (ReleaseEventDispatcherService))]
  public interface IReleaseEventDispatcherService : IVssFrameworkService
  {
    Task Disconnect(IVssRequestContext requestContext, string clientId, bool isClientTimeout);

    Task WatchReleaseDefinitionReleases(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      string clientId);

    Task StopWatchingReleaseDefinitionReleases(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      string clientId);

    Task WatchReleaseJobLogs(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      Guid jobId,
      string clientId);

    Task WatchRelease(IVssRequestContext context, Guid projectId, int releaseId, string clientId);

    Task StopWatchingRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      string clientId);

    Task WatchCollection(IVssRequestContext context, string clientId);

    void SendReleaseCreatedEvent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId);

    void SendReleaseUpdatedEvent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId);

    void SendReleaseEnvironmentUpdatedEvent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId,
      int releaseEnvironmentId);

    void SendDeployJobStartedEvent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentId);

    void SendRealTimeEvent(IVssRequestContext context, ReleaseTasksUpdatedEvent notificationEvent);

    void SendRealTimeEvent(IVssRequestContext context, ReleaseTaskLogUpdatedEvent notificationEvent);

    void SendRealTimeEvent(
      IVssRequestContext context,
      TaskOrchestrationPlanGroupsStartedEvent notificationEvent);

    void SendRealTimeEvent(
      IVssRequestContext context,
      ReleaseEnvironmentStatusUpdatedEvent notificationEvent);

    void SendReleaseApprovalPendingEvent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId,
      int releaseEnvironmentId,
      int approvalId,
      Guid approverId);
  }
}
