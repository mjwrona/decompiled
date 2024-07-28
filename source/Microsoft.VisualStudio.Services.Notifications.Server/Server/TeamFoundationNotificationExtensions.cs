// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationNotificationExtensions
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal static class TeamFoundationNotificationExtensions
  {
    internal static void EnsureDiagnosticNotification(this TeamFoundationNotification notification)
    {
      if (notification.DiagnosticNotification != null)
        return;
      TeamFoundationNotification foundationNotification = notification;
      DiagnosticNotification diagnosticNotification = new DiagnosticNotification();
      diagnosticNotification.Id = notification.Id;
      TeamFoundationEvent teamFoundationEvent = notification.Event;
      diagnosticNotification.EventId = teamFoundationEvent != null ? teamFoundationEvent.Id : -1;
      diagnosticNotification.EventType = notification.Event?.EventType;
      foundationNotification.DiagnosticNotification = diagnosticNotification;
      if (notification.DeliveryDetails == null)
        return;
      notification.DiagnosticNotification.SubscriptionId = notification.DeliveryDetails.NotificationSource;
      foreach (NotificationRecipient recipient in notification.DeliveryDetails.Recipients)
      {
        DiagnosticRecipient diagnosticRecipient = new DiagnosticRecipient()
        {
          Recipient = DiagnosticLogUtil.CreateDiagnosticIdentity(recipient.Id, recipient.Identity)
        };
        notification.DiagnosticNotification.Recipients[recipient.Id] = diagnosticRecipient;
      }
    }

    internal static void AddMessage(
      this TeamFoundationNotification notification,
      TraceLevel level,
      string message)
    {
      notification.EnsureDiagnosticNotification();
      notification.DiagnosticNotification.Messages.Append(level, message);
    }

    internal static bool IsSubscriptionTracingEnabled(this TeamFoundationNotification notification)
    {
      NotificationDeliveryDetails deliveryDetails = notification.DeliveryDetails;
      return deliveryDetails != null && deliveryDetails.DeliveryTracing;
    }

    internal static void StartSubscriptionTracing(this TeamFoundationNotification notification)
    {
      NotificationDeliveryDetails deliveryDetails = notification.DeliveryDetails;
      if ((deliveryDetails != null ? (deliveryDetails.DeliveryTracing ? 1 : 0) : 0) == 0)
        return;
      notification.EnsureDiagnosticNotification();
    }

    internal static void EndSubscriptionTracing(
      this TeamFoundationNotification notification,
      INotificationDeliveryJob deliveryJob)
    {
      NotificationDeliveryDetails deliveryDetails = notification.DeliveryDetails;
      if ((deliveryDetails != null ? (deliveryDetails.DeliveryTracing ? 1 : 0) : 0) == 0 || notification.DiagnosticNotification == null)
        return;
      if (notification.DeliveryStopwatch != null)
        notification.DiagnosticNotification.Stats["NotificationDeliveryTime"] = (int) notification.DeliveryStopwatch.ElapsedMilliseconds;
      string notificationSource = notification.DeliveryDetails.NotificationSource;
      if (!string.IsNullOrEmpty(notificationSource))
      {
        SubscriptionTraceDiagnosticLog traceDiagnosticLog = deliveryJob.GetSubscriptionTraceDiagnosticLog(notificationSource);
        if (traceDiagnosticLog is SubscriptionTraceNotificationDeliveryLog notificationDeliveryLog)
        {
          notificationDeliveryLog.Notifications.Add(notification.DiagnosticNotification);
        }
        else
        {
          string str = traceDiagnosticLog?.GetType().Name ?? "null";
          deliveryJob.LogNotificationWarning(CoreRes.SubscriptionTraceErrorLoggerInvalid((object) notificationSource, (object) str), notification);
        }
      }
      else
        deliveryJob.LogNotificationWarning(CoreRes.SubscriptionTraceErrorBadSource((object) notificationSource), notification);
    }

    public static void UpdateRecipientStatus(
      this TeamFoundationNotification notification,
      Guid recipientId,
      string emailAddress,
      string status)
    {
      DiagnosticRecipient diagnosticRecipient;
      if (notification.DiagnosticNotification == null || recipientId.Equals(Guid.Empty) || !notification.DiagnosticNotification.Recipients.TryGetValue(recipientId, out diagnosticRecipient))
        return;
      if (!string.IsNullOrEmpty(emailAddress) && diagnosticRecipient.Recipient != null)
        diagnosticRecipient.Recipient.EmailAddress = string.IsNullOrEmpty(diagnosticRecipient.Recipient.EmailAddress) ? emailAddress : diagnosticRecipient.Recipient.EmailAddress + ";" + emailAddress;
      if (!string.IsNullOrEmpty(diagnosticRecipient.Status))
        diagnosticRecipient.Status = diagnosticRecipient.Status + Environment.NewLine + status;
      else
        diagnosticRecipient.Status = status;
    }
  }
}
