// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostingRegistryFormatStrings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class HostingRegistryFormatStrings
  {
    public const string ServiceBusPath = "/Service/MessageBus/ServiceBus";
    public const string ServiceBusPublisherPath = "/Service/MessageBus/ServiceBus/Publisher";
    public const string ServiceBusSubscriberPath = "/Service/MessageBus/ServiceBus/Subscriber";
    public const string ServiceBusManagementPath = "/Service/MessageBus/ServiceBus/Management";
    public const string ServiceBusManagementTopicsPath = "/Service/MessageBus/ServiceBus/Management/Topics";
    public const string ServiceBusNamespaceScopePathFormat = "/Service/MessageBus/ServiceBus/Management/Namespaces/{0}";
    public const string ServiceBusTopicScopePathFormat = "/Service/MessageBus/ServiceBus/Management/Topics/{0}";
    public const string ServiceBusNamespaceRelativePath = "/Namespace";
    public const string ServiceBusNamespacePoolRelativePath = "/NamespacePool";
    public const string ServiceBusPrimaryPublisherRelativePath = "/PublisherPrimaryNamespace";
    public const string ServiceBusSecondaryPublisherRelativePath = "/PublisherSecondaryNamespace";
    public const string ServiceBusSubscriptionFilterRelativePath = "/SubscriptionFilter";
    public const string ServiceBusIsTopicForwardingRelativePath = "/IsTopicForwarding";
    public const string ServiceBusIssuerRelativePath = "/Issuer";
    public const string ServiceBusKeyRelativePath = "/Key";
    public const string ServiceBusTopicNameRelativePath = "/TopicName";
    public const string ServiceBusSubscriptionNameRelativePath = "/SubscriptionName";
    public const string ServiceBusMessageTimeToLiveRelativePath = "/SubscriptionMessageTimeToLiveMinutes";
    public const string ServiceBusEnableDeadLetteringOnMessageExpirationRelativePath = "/SubscriptionDeadLetteringOnMessageExpiration";
    public const string ServiceBusEnableDeadLetteringOnFilterEvaluationExceptionsRelativePath = "/SubscriptionDeadLetteringOnFilterEvaluationExceptions";
    public const string ServiceBusSubscriptionIdlePeriodRelativePath = "/SubscriptionIdlePeriod";
    public const string ServiceBusSubscriptionPrefetchCountRelativePath = "/SubscriptionPrefetchCount";
    public const string ServiceBusSubscriptionIsTransientRelativePath = "/IsTransient";
    public const string ServiceBusTopicMaxSizeInGBRelativePath = "/TopicMaxSizeInGB";
    public const string ServiceBusTopicPublishSecondaryRelativePath = "/PublishSecondary";
    public const string ServiceBusEnablePartitioningRelativePath = "/EnablePartitioning";
    public const string ServiceBusPrefixComputerNameRelativePath = "/PrefixComputerName";
    public const string ServiceBusHostNamePrefixRelativePath = "/HostNamePrefix";
    public const string ServiceBusUseLegacyTopicNamesRelativePath = "/UseLegacyTopicNames";
    public const string ServiceBusSharedAccessKeySettingNameRelativePath = "/SharedAccessKeySettingName";
    public const string ServiceBusBatchFlushIntervalRelativePath = "/BatchFlushInterval";
    public const string ServiceBusPrefetchCountRelativePath = "/PrefetchCount";
    public const string ServiceBusTraceDelayRelativePath = "/TraceDelay";
    public const string ServiceBusDeadLetterCleanupBatchSizeRelativePath = "/ServiceBusDeadLetterCleanupBatchSize";
    public const string ServiceBusMaxDeliveryRetryCountRelativePath = "/MaxDeliveryRetryCount";
    public const string ServiceBusDeliveryRetryMinBackoffRelativePath = "/DeliveryRetryMinBackoff";
    public const string ServiceBusDeliveryRetryMaxBackoffRelativePath = "/DeliveryRetryMaxBackoff";
    public const string ServiceBusDeliveryRetryDeltaBackoffRelativePath = "/DeliveryRetryDeltaBackoff";
    public const string ServiceBusDefaultNamespace = "/Service/MessageBus/ServiceBus/Management/Namespace";
    public const string ServiceBusTopicMaxSizeInGB = "/Service/MessageBus/ServiceBus/Management/TopicMaxSizeInGB";
    public const string ServiceBusPrefixComputerName = "/Service/MessageBus/ServiceBus/Management/PrefixComputerName";
    public const string ServiceBusHostNamePrefix = "/Service/MessageBus/ServiceBus/Management/HostNamePrefix";
    public const string ServiceBusConnectivityMode = "/Service/MessageBus/ServiceBus/Management/ConnectivityMode";
    public const string SubscriberThresholdInSeconds = "/Service/MessageBus/ServiceBus/Management/SubscriberThresholdInSeconds";
    public const string ServiceBusTransportType = "/Service/MessageBus/ServiceBus/Management/TransportType";
    public const string ServiceBusMaxPublishSize = "/Service/MessageBus/ServiceBus/Management/MaxPublishSize";
    public const string ServiceBusIdlePeriod = "/Service/MessageBus/ServiceBus/Management/IdlePeriod";
    public const string ServiceBusPublishThreadConcurrency = "/Service/MessageBus/ServiceBus/Management/PublishThreadConcurrency";
    public const string ServiceBusPublishThreadCount = "/Service/MessageBus/ServiceBus/Management/PublishThreadCount";
    public const string ServiceBusSubscriptionIdlePeriod = "/Service/MessageBus/ServiceBus/Management/SubscriptionIdlePeriod";
    public const string ServiceBusSubscriptionMessageTimeToLiveMinutes = "/Service/MessageBus/ServiceBus/Management/SubscriptionMessageTimeToLiveMinutes";
    public const string ServiceBusSubscriptionEnableDeadLetteringOnMessageExpiration = "/Service/MessageBus/ServiceBus/Management/SubscriptionDeadLetteringOnMessageExpiration";
    public const string ServiceBusSubscriptionEnableDeadLetteringOnFilterEvaluationExceptions = "/Service/MessageBus/ServiceBus/Management/SubscriptionDeadLetteringOnFilterEvaluationExceptions";
    public const string ServiceBusSubscriberAlert = "/Service/MessageBus/ServiceBus/Subscriber/Alert";
    public const string ServiceBusSubscriberUnprocessedMessageCount = "/Service/MessageBus/ServiceBus/Subscriber/UnprocessedMessageCount";
  }
}
