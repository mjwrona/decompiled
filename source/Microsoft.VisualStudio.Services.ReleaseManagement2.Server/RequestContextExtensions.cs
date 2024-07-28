// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.RequestContextExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class RequestContextExtensions
  {
    public static void SendReleaseEnvironmentUpdatedEvent(
      this IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId,
      int releaseEnvironmentId)
    {
      context.SafeExecute((Action) (() => context.GetService<IReleaseEventDispatcherService>().SendReleaseEnvironmentUpdatedEvent(context, projectId, definitionId, releaseId, releaseEnvironmentId)), 1999999, "Events");
    }

    public static void SendDeployJobStartedEvent(
      this IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentId)
    {
      context.SafeExecute((Action) (() => context.GetService<IReleaseEventDispatcherService>().SendDeployJobStartedEvent(context, projectId, definitionId, releaseId, definitionEnvironmentId, releaseEnvironmentId)), 1999999, "Events");
    }

    public static void SendReleaseUpdatedEvent(
      this IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId)
    {
      context.SafeExecute((Action) (() => context.GetService<IReleaseEventDispatcherService>().SendReleaseUpdatedEvent(context, projectId, definitionId, releaseId)), 1999999, "Events");
    }

    public static void SendReleaseCreatedEvent(
      this IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId)
    {
      context.SafeExecute((Action) (() => context.GetService<IReleaseEventDispatcherService>().SendReleaseCreatedEvent(context, projectId, definitionId, releaseId)), 1999999, "Events");
    }

    public static void SendReleaseEnvironmentStatusUpdatedNotification(
      this IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId,
      int releaseEnvironmentId,
      EnvironmentStatus releaseEnvironmentStatus,
      DeploymentStatus latestDeploymentStatus,
      DeploymentOperationStatus latestDeploymentOperationStatus)
    {
      context.SafeExecute((Action) (() =>
      {
        ReleaseEnvironmentStatusUpdatedEvent notificationEvent = new ReleaseEnvironmentStatusUpdatedEvent(projectId, definitionId, releaseId, releaseEnvironmentId, releaseEnvironmentStatus, latestDeploymentStatus, latestDeploymentOperationStatus);
        context.GetService<IReleaseEventDispatcherService>().SendRealTimeEvent(context, notificationEvent);
      }), 1999998, "Events");
    }

    public static void SendReleaseApprovalPendingEvent(
      this IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId,
      int releaseEnvironmentId,
      int approvalId,
      Guid approverId)
    {
      context.SafeExecute((Action) (() => context.GetService<IReleaseEventDispatcherService>().SendReleaseApprovalPendingEvent(context, projectId, definitionId, releaseId, releaseEnvironmentId, approvalId, approverId)), 1999999, "Events");
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safe execution.")]
    internal static void SafeExecute(
      this IVssRequestContext context,
      Action action,
      int tracePoint,
      string layer)
    {
      try
      {
        action();
      }
      catch (Exception ex)
      {
        context.TraceException(tracePoint, "ReleaseManagementService", layer, ex);
      }
    }
  }
}
