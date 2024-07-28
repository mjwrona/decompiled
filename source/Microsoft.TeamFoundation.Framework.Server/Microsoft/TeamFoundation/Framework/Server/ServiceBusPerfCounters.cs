// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceBusPerfCounters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServiceBusPerfCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBus";
    internal const string PublishedMessagesTotal = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPublishedMessagesTotal";
    internal const string PublishedMessagesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPublishedMessagesPerSec";
    internal const string ReceivedMessagesTotal = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusReceivedMessagesTotal";
    internal const string ReceivedMessagesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusReceivedMessagesPerSec";
    internal const string DeadLetterMessagesDeletedTotal = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusDeadLetterMessagesDeletedTotal";
    internal const string DeadLetterMessagesDeletedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusDeadLetterMessagesDeletedPerSec";
    internal const string TotalSubscribers = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusTotalSubscribers";
    internal const string AvgReceiveTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAvgReceiveTime";
    internal const string AvgReceiveTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAvgReceiveTimeBase";
    public const string PostedClientNotificationsTotal = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPostedClientNotificationsTotal";
    public const string PostedClientNotificationsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPostedClientNotificationsPerSec";
    public const string ClientSubscriptionsTotal = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusClientSubscriptionsTotal";
    public const string ClientSubscriptionsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusClientSubscriptionsPerSec";
    public const string AvgSendClientNotificationTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAvgSendClientNotificationTime";
    public const string AvgSendClientNotificationTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAvgSendClientNotificationTimeBase";
    internal const string AveragePublishTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAveragePublishTime";
    internal const string AveragePublishTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAveragePublishTimeBase";
    public const string ClientActiveSubscribeCalls = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusClientActiveSubscribeCalls";
    public const string ProfileAvatarSynchronizeCalls = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusProfileAvatarSynchronizeCalls";
  }
}
