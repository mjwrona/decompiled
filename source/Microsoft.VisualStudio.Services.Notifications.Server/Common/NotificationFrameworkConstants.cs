// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  public static class NotificationFrameworkConstants
  {
    internal const string NotificationBacklogStatusScope = "NotificationBacklogStatusScope";
    internal const string NotificationBacklogStatusUserScope = "NotificationBacklogStatusUserChannelScope";
    internal const string NotificationBacklogStatusServiceHooksScope = "NotificationBacklogStatusServiceHooksScope";
    internal const string NotificationBacklogStatusEventTotal = "NotificationBacklogStatusEventsTotal";
    internal const string UnprocessedEvents = "UnprocessedEvents";
    internal const string UnprocessedEventsDesc = "Total Unprocessed Events";
    internal const string MaxUnprocessedEventAgeMs = "MaxUnprocessedEventAgeMs";
    internal const string MaxUnprocessedEventAgeMsDesc = "Max Unprocessed Event Age";
    internal const string TimeSinceLastProcessedEventMs = "TimeSinceLastProcessedEventMs";
    internal const string TimeSinceLastProcessedEventMsDesc = "Elapsed Ms since last processed event";
    internal const string UnprocessedNotifications = "UnprocessedNotifications";
    internal const string UnprocessedNotificationsDesc = "Total Unprocessed Notifications";
    internal const string MaxUnprocessedNotificationAgeMs = "MaxUnprocessedNotificationAgeMs";
    internal const string MaxUnprocessedNotificationAgeMsDesc = "Max Unprocessed Notification Age";
    internal const string TimeSinceLastProcessedNotificationMs = "TimeSinceLastProcessedNotificationMs";
    internal const string TimeSinceLastProcessedNotificationMsDesc = "Elapsed Ms since last processed Notification";
    internal const int DefaultEventBatchSize = 1000;
    internal const int DefaultNotificationBatchSize = 100;
    internal const int DefaultCleanupEventAgeMins = 5760;
    internal const int DefaultCleanupNotificationAgeMins = 10080;
    internal const int EventQueueAlertThrehold = 10000;
    internal const int NotificationQueueAlertThreshold = 10000;
    internal const int JailSubscriptionUserThreshold = 2000;
    internal const int JailSubscriptionServiceHooksThreshold = 5000;
    internal const int JailSubscriptionMaxAgeSec = 7200;
    public static readonly string NotificationRootPath = FrameworkServerConstants.NotificationRootPath;
    internal static readonly string NotificationBacklogStatusJobDelay = NotificationFrameworkConstants.NotificationRootPath + "/NotificationBacklogStatusJobDelay";
    internal static readonly string SuspendNotificationBatchSize = NotificationFrameworkConstants.NotificationRootPath + "/SuspendNotificationBatchSize";
    internal static readonly string MaxActorMatcherExpandedMembers = NotificationFrameworkConstants.NotificationRootPath + "/MaxActorMatcherExpandedMembers";
    internal static readonly string CustomSmtpHeadersRoot = NotificationFrameworkConstants.NotificationRootPath + "/CustomSmtpHeaders/";
    internal static readonly string SubscriberServiceRoot = NotificationFrameworkConstants.NotificationRootPath + "/SubscriberService";
    internal static readonly string AdminSettingsServiceRoot = NotificationFrameworkConstants.NotificationRootPath + "/AdminSettingsService";
    internal static readonly string ContributedSubscriptionDiagnosticsRoot = NotificationFrameworkConstants.NotificationRootPath + "/ContributedSubscriptionDiagnostics";
    internal static readonly string EventExpirationSeconds = NotificationFrameworkConstants.NotificationRootPath + "/EventExpirationSeconds";
    internal static readonly string EventCreatedDeltaTrigger = NotificationFrameworkConstants.NotificationRootPath + "/EventCreatedDeltaTrigger";
    internal static readonly string PublishEventsRate = NotificationFrameworkConstants.NotificationRootPath + "/PublishEventsRate";
    internal static readonly string NotificationEventSizeTraceLimit = NotificationFrameworkConstants.NotificationRootPath + "/NotificationEventSizeTraceLimit";
    internal static readonly string AdminSettingsGeneration = "SettingsGeneration";
    internal static readonly string JobStatusRoot = NotificationFrameworkConstants.NotificationRootPath + "/JobStatus";
    internal static readonly string EventPublisherStatusRoot = NotificationFrameworkConstants.NotificationRootPath + "/EventPubStatus";
    internal static readonly string NotificationPublisherStatusRoot = NotificationFrameworkConstants.NotificationRootPath + "/NotifPubStatus";
    internal static readonly string BatchStarted = nameof (BatchStarted);
    internal static readonly string BatchCompleted = nameof (BatchCompleted);
    internal static readonly string MaxJobStuckTimeMs = nameof (MaxJobStuckTimeMs);
    internal static readonly string MaxJobWaitForScheduleTimeMs = nameof (MaxJobWaitForScheduleTimeMs);
    internal static readonly string AsciiOnlyEmailAddresses = nameof (AsciiOnlyEmailAddresses);
    public static readonly string AsciiOnlyEmailAddressesFullPath = NotificationFrameworkConstants.NotificationRootPath + "/" + NotificationFrameworkConstants.AsciiOnlyEmailAddresses;
    public const string EvaluationJobTimeOutKey = "EvaluationJobTimeOut";
    public const int DefaultEvaluationJobTimeOut = 10;
    public const string SubscriptionEvaluationTimeOutKey = "SubscriptionEvaluationTimeOut";
    public const int DefaultSubscriptionEvaluationTimeOut = 30;
    public const string SubscriptionEvaluationIntervalKey = "SubscriptionEvaluationInterval";
    public const int DefaultSubscriptionEvaluationInterval = 3;
    public const string SubscriptionNotificationThresholdKey = "SubscriptionNotificationThreshold";
    public const int DefaultSubscriptionNotificationThreshold = 2000;
    public const string SubscriptionEvaluationResultKey = "SubscriptionEvaluationResult";
    public static readonly Guid EvaluationResultpropertyKind = Guid.Parse("d0cb3d73-e4ff-41f4-9f65-879570d4bc12");
    public const string BridgeKey = "@NotifBridge";
    public const string StringFieldModeKey = "@NotifStringFieldMode";
    public const int RetryCountDefaultValue = 5;
    public const string WildCardAll = "*";
    public const string SelfLink = "self";
    public const string EditLink = "edit";
    public const string TeamLink = "team";
    public const DeliveryType InvalidDeliveryType = (DeliveryType) 2147483647;
    public const string NotificationsArea = "Notifications";
    public const string SubscriptionTraceEventProcessingSourceFormat = "08675309-EEEE-{0:X4}-0000-{1:D12}";
    public const string SubscriptionTraceNotificationDeliverySourceFormat = "08675309-DDDD-{0:X4}-0000-{1:D12}";
    public const string NotificationMessageIdSourceFormat = "{0:X8}-{1:X4}-{2:X4}-{3:X4}-{4:D12}";
    public const string SubscriberDeliveryPreferenceProperty = "NotificationSubscriberDeliveryPreference";
    public const string SubscriberPreferrredEmailAddressProperty = "NotificationSubscriberPreferrredEmailAddress";
    public const string DefaultGroupDeliveryPreferenceProperty = "NotificationDefaultGroupDeliveryPreference";
    public static readonly List<string> ValidFilters = new List<string>()
    {
      "Expression",
      "Artifact",
      "Block",
      "Actor",
      "ActorExpressionMatcher",
      "PathExpressionMatcher"
    };
    public static readonly List<string> UniqueTargetDeliveryChannels = new List<string>()
    {
      "User",
      "EmailHtml",
      "EmailPlaintext",
      "UserSystem",
      "Group"
    };
    public static readonly List<string> UserDeliveryChannels = new List<string>()
    {
      "User",
      "EmailHtml",
      "EmailPlaintext",
      "Group"
    };
    public static readonly List<string> EmailTargetDeliveryChannels = NotificationFrameworkConstants.UniqueTargetDeliveryChannels;
    public static readonly List<string> HtmlEmailTargetDeliveryChannels = new List<string>()
    {
      "User",
      "EmailHtml",
      "UserSystem",
      "Group"
    };
    public static readonly List<string> LocalServiceHooksDeliveryChannels = new List<string>()
    {
      "ServiceBus"
    };
    public static readonly HashSet<string> AllActiveDeliveryChannels = new HashSet<string>()
    {
      "User",
      "EmailHtml",
      "EmailPlaintext",
      "UserSystem",
      "Group",
      "ServiceBus",
      "ServiceHooks",
      "Soap"
    };
    public static readonly List<string> OptOutMatcherCandidates = new List<string>()
    {
      "ActorMatcher",
      "PathMatcher",
      "XPathMatcher",
      "JsonPathMatcher",
      "ActorExpressionMatcher",
      "PathExpressionMatcher"
    };

    public static class CustomSqlError
    {
      public const int SubscriptionNotFound = 800401;
      public const int IndexedSubscriptionAlreadyExists = 800402;
    }
  }
}
