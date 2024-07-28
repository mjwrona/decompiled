// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ServiceHooksCustomerIntelligenceInfo
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal static class ServiceHooksCustomerIntelligenceInfo
  {
    public static readonly string KeyMessage = "Message";
    public static readonly string KeyNotificationId = "NotificationId";
    public static readonly string KeyEventId = nameof (KeyEventId);
    public static readonly string KeyEventType = "EventType";
    public static readonly string KeyConsumerId = "ConsumerId";
    public static readonly string KeyPublisherId = "PublisherId";
    public static readonly string KeySendDuration = "SendDuration";
    public static readonly string KeyQueueWaitTime = "QueueWaitTime";
    public static readonly string KeyConsumerCount = "ConsumerCount";
    public static readonly string KeySubscriptionId = "SubscriptionId";
    public static readonly string KeyConsumerActionId = "ConsumerActionId";
    public static readonly string KeyProbationRetries = "ProbationRetries";
    public static readonly string KeyConsumerActionCount = "ConsumerActionCount";
    public static readonly string KeyEnabledSubscriptionCount = "EnabledSubscriptionCount";
    public static readonly string KeyDisabledSubscriptionCount = "DisabledSubscriptionCount";
    public static readonly string KeySubscriptionsBeforeFilter = "SubscriptionsBeforeFilter";
    public static readonly string KeySubscriptionsAfterFilter = "SubscriptionsAfterFilter";
    public static readonly string KeyExceptionType = "ExceptionType";
    public static readonly string KeyInnerExceptionType = "InnerExceptionType";
    public static readonly string KeyPayloadSize = "PayloadSize";
    public static readonly string KeySubscriptionMetadata = "SubscriptionMetadata";
    public static readonly string KeyCatchupJobBatchDeliveryTrace = "CatchupJobDeliveryTrace";
    public static readonly string FeatureEventPublished = "EventPublished";
    public static readonly string FeatureCreateSubscription = "CreateSubscription";
    public static readonly string FeatureUpdateSubscription = "UpdateSubscription";
    public static readonly string FeatureDeleteSubscription = "DeleteSubscription";
    public static readonly string FeatureNotificationQueued = "NotificationQueued";
    public static readonly string FeatureSynchronizeDashboard = "SynchronizeDashboard";
    public static readonly string FeatureNotificationDequeued = "NotificationDequeued";
    public static readonly string FeatureNotificationSendFailure = "NotificationSendFailure";
    public static readonly string FeatureNotificationSendSuccess = "NotificationSendSuccess";
    public static readonly string FeatureNotificationEventHandlerFailure = "NotificationEventHandlerFailure";
    public static readonly string FeatureSubscriptionDisableBySystem = "SubscriptionDisableBySystem";
    public static readonly string FeatureSubscriptionReenableBySystem = "SubscriptionReenableBySystem";
    public static readonly string FeatureSubscriptionPlaceOnProbation = "SubscriptionPlaceOnProbation";
    public static readonly string FeatureSubscriptionSkippedForPublishedEvent = "SubscriptionSkippedForPublishedEvent";
    public static readonly string FeatureNoSubscriptionsForPublishedEvent = "NoSubscriptionsForPublishedEvent";
    public static readonly string FeatureCatchupJobBatchDeliveryStarted = "CatchupJobBatchDeliveryStarted";
    public static readonly string Area = "Microsoft.VisualStudio.Services.ServiceHooks.Server";
  }
}
