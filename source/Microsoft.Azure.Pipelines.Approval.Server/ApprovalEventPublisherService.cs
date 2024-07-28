// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.ApprovalEventPublisherService
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Pipelines.Approval.Server
{
  public sealed class ApprovalEventPublisherService : 
    IApprovalEventPublisherService,
    IVssFrameworkService
  {
    private const string c_layer = "ApprovalEventPublisher";
    private const string c_useExtensionModelToUpdateCheckSuiteFF = "Pipelines.Checks.UseExtensionForApprovalCompletion";

    public void NotifyApprovalCompletedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval)
    {
      using (new MethodScope(requestContext, "ApprovalEventPublisher", nameof (NotifyApprovalCompletedEvent)))
      {
        requestContext.TraceVerbose(34001701, "ApprovalEventPublisher", "Raised event with approval data: {0}", (object) approval);
        ApprovalCompletedNotificationEvent notificationEvent = new ApprovalCompletedNotificationEvent(projectId, approval);
        this.PublishEvent(requestContext, (object) notificationEvent);
        this.RaiseApprovalListenerEvent(requestContext, (object) notificationEvent, typeof (ApprovalCompletedNotificationEvent), approval.Owner);
      }
    }

    public void NotifyApprovalSkippedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval,
      IdentityRef skippedBy)
    {
      using (new MethodScope(requestContext, "ApprovalEventPublisher", nameof (NotifyApprovalSkippedEvent)))
      {
        requestContext.TraceVerbose(34001701, "ApprovalEventPublisher", "Raised event with approval data: {0}", (object) approval);
        ApprovalCompletedNotificationEvent notificationEvent = new ApprovalCompletedNotificationEvent(projectId, approval, skippedBy);
        this.PublishEvent(requestContext, (object) notificationEvent);
        this.RaiseApprovalListenerEvent(requestContext, (object) notificationEvent, typeof (ApprovalCompletedNotificationEvent), approval.Owner);
      }
    }

    public void NotifyApprovalPendingEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval)
    {
      using (new MethodScope(requestContext, "ApprovalEventPublisher", nameof (NotifyApprovalPendingEvent)))
      {
        requestContext.TraceVerbose(34001703, "ApprovalEventPublisher", "Raised event with approval data: {0}", (object) approval);
        ApprovalPendingNotificationEvent notificationEventPayload = new ApprovalPendingNotificationEvent(projectId, approval);
        this.RaiseApprovalListenerEvent(requestContext, (object) notificationEventPayload, typeof (ApprovalPendingNotificationEvent), approval.Owner);
      }
    }

    internal void PublishEvent(IVssRequestContext requestContext, object eventData)
    {
      try
      {
        if (requestContext.IsFeatureEnabled("Pipelines.Checks.UseExtensionForApprovalCompletion"))
        {
          using (IDisposableReadOnlyList<IApprovalCompletedEventSubscriber> extensions = requestContext.GetExtensions<IApprovalCompletedEventSubscriber>(ExtensionLifetime.Service))
            extensions.ForEach<IApprovalCompletedEventSubscriber>((Action<IApprovalCompletedEventSubscriber>) (approvalCompletedEventSubscriber =>
            {
              ApprovalCompletedNotificationEvent approvalCompletedEvent = (ApprovalCompletedNotificationEvent) eventData;
              if (approvalCompletedEventSubscriber.GetApprovalOwner() != approvalCompletedEvent.Approval.Owner)
                return;
              approvalCompletedEventSubscriber.ProcessApprovalCompletedEvent(requestContext, approvalCompletedEvent);
            }));
        }
        else
          requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, eventData);
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34001702, "ApprovalEventPublisher", "Failed to fire event with data {0}. Exception received: {1}", eventData, (object) ex);
      }
    }

    internal void RaiseApprovalListenerEvent(
      IVssRequestContext requestContext,
      object notificationEventPayload,
      Type eventType,
      ApprovalOwner owner)
    {
      using (IDisposableReadOnlyList<IApprovalEventListener> extensions = requestContext.GetExtensions<IApprovalEventListener>())
      {
        try
        {
          extensions.ForEach<IApprovalEventListener>((Action<IApprovalEventListener>) (eventListener =>
          {
            if (!eventListener.IsASubsriber(owner, eventType))
              return;
            eventListener.PublishNotification(requestContext, notificationEventPayload);
          }));
        }
        catch (Exception ex)
        {
          requestContext.TraceError(34001705, "ApprovalEventPublisher", "Failed to raise approval event for event data {0}. Exception received: {1}", notificationEventPayload, (object) ex);
        }
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
