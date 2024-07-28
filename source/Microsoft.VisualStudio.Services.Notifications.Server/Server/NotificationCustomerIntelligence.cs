// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationCustomerIntelligence
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal static class NotificationCustomerIntelligence
  {
    internal static readonly string UserDeliveryJobFeature = "UserDeliveryJob";
    internal static readonly string UserSystemDeliveryJobFeature = "UserSystemDeliveryJob";
    internal static readonly string ServiceHooksDeliveryJobFeature = "ServiceHooksDeliveryJob";
    internal static readonly string ServiceBusDeliveryJobFeature = "ServiceBusDeliveryJob";
    internal static readonly string ServiceHooksGroupDeliveryJobFeature = "ServiceHooksGroupDeliveryJob";
    internal static readonly string WorkItemSerializationFeature = "WorkItemSerialization";
    internal static readonly string ProcessingJobFeature = "ProcessingJob";
    internal static readonly string AuditJobFeature = "AuditJob";
    internal static readonly string NotificationEventServiceFeature = "NotificationEventService";
    internal static readonly string BacklogStatusFeature = "BacklogStatus";
    internal static readonly string ProcessingDeliveryAction = "ProcessingDelivery";
    internal static readonly string JobBatchStartAction = "JobBatchStart";
    internal static readonly string JobBatchCompleteAction = "JobBatchComplete";
    internal static readonly string PublisherBatchStartAction = "PublisherBatchStart";
    internal static readonly string PublisherBatchCompleteAction = "PublisherBatchComplete";
    internal static readonly string DeliveryCompleteAction = "DeliveryComplete";
    internal static readonly string JobStatistics = nameof (JobStatistics);
    internal static readonly string SerializeWorkItemEventAction = "SerializeWorkItemEvent";
    internal static readonly string EventStatusAction = "EventStatus";
    internal static readonly string NotificationStatusAction = "NotificationStatus";
    internal static readonly string UpdateSubscriberAction = "UpdateSubscriber";
    internal static readonly string PayloadCreatedAction = "PayloadCreated";
    internal static readonly string PublishedEventsAction = "PublishedEvents";
    internal static readonly string BlockedEventsAction = "BlockedEvents";
    internal static readonly string PublishDelayedEventAction = "PublishDelayedEvent";
    internal static readonly string NotificationsRead = "read";
    internal static readonly string NotificationsAttempted = "attempted";
    internal static readonly string NotificationsSucceeded = "succeeded";
    internal static readonly string NotificationsFailed = "failed";
    internal static readonly string NotificationsFailedAlertable = "failedAlertable";
    internal static readonly string NotificationsFailedNonAlertable = "failedNonAlertable";
    internal static readonly string NotificationsFailedRetryable = "failedRetryable";
    internal static readonly string RecipientCount = "recipientCount";
    internal static readonly string RecipientsFiltered = "recipientsFiltered";
    internal static readonly string RecipientsAccountError = "recipientsAccountError";
    internal static readonly string RecipientsBindPending = "recipientsBindPending";
    internal static readonly string NotificationFailureDetailFormat = "failedWith{0}";
    internal static readonly string NotificationSuccessDetailFormat = "succeededWith{0}";
    internal static readonly string EmailProcessed = "emailProcessed";
    internal static readonly string EmailStarted = "emailStarted";
    internal static readonly string EmailFailed = "emailFailed";
    internal static readonly string EmailExceptions = "emailExceptions";
    internal static readonly string EmailDelivered = "emailDelivered";
    internal static readonly string EmailSkipped = "emailSkipped";
    internal static readonly string EmailAttemptedMessages = "emailAttemptedMessages";
    internal static readonly string EmailDeliveredMessages = "emailDeliveredMessages";
    internal static readonly string EmailAttemptedRecipients = "emailAttemptedRecipients";
    internal static readonly string EmailDeliveredRecipients = "emailDeliveredRecipients";
    internal static readonly string EmailRecipientBadAddress = "emailRecipientBadAddress";
    internal static readonly string EmailRecipientCustomExtra = "emailRecipientCustomExtra";
    internal static readonly string EmailRecipientsSkipped = "emailRecipientsSkipped";
    internal static readonly string EmailSkippedCodeReviewEvents = "emailSkippedCodeReviewEvents";
    internal static readonly string JobId = "jobId";
    internal static readonly string ProcessQueue = "processQueue";
    internal static readonly string Statistics = "statisitics";
    private const string s_area = "Notifications";
    private const string s_layer = "NotificationCustomerIntelligence";

    internal static void PublishEvent(
      IVssRequestContext requestContext,
      string feature,
      string action,
      CustomerIntelligenceData ciData)
    {
      try
      {
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        ciData.Add(CustomerIntelligenceProperty.Action, action);
        IVssRequestContext requestContext1 = requestContext;
        string notifications = CustomerIntelligenceArea.Notifications;
        string feature1 = feature;
        CustomerIntelligenceData properties = ciData;
        service.Publish(requestContext1, notifications, feature1, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002030, "Notifications", nameof (NotificationCustomerIntelligence), ex);
      }
    }
  }
}
