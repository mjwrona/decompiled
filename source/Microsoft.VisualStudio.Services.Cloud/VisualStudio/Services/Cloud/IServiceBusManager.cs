// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IServiceBusManager
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DefaultServiceImplementation(typeof (ServiceBusManagementService))]
  public interface IServiceBusManager : IVssFrameworkService
  {
    void RegisterNamespace(
      IVssRequestContext requestContext,
      string name,
      string sharedAccessKeySettingName,
      int topicMaxSizeInGB = 5,
      string hostNamePrefix = null,
      bool prefixComputerName = false,
      bool isGlobal = true,
      ITFLogger logger = null);

    SubscriptionDescription EnsureSubscriptionCreated(
      IVssRequestContext requestContext,
      string messageBusName,
      string ns,
      SubscriptionDescription subscriptionDescription,
      Microsoft.ServiceBus.Messaging.Filter filter,
      int retryCount = 5,
      TimeSpan? timeout = null);

    TopicDescription EnsureTopicCreated(
      IVssRequestContext requestContext,
      string messageBusName,
      TopicDescription topicDescription,
      string ns,
      bool deleteIfExists);

    IEnumerable<TopicDescription> GetTopics(IVssRequestContext requestContext, string ns);

    TopicDescription UpdateTopic(
      IVssRequestContext requestContext,
      TopicDescription topicDescription,
      string ns);

    IServiceBusPublishConnection CreateServiceBusPublishConnection(
      IVssRequestContext requestContext,
      string ns,
      string topicName);

    IEnumerable<SubscriptionDescription> GetSubscriptionsForTopic(
      IVssRequestContext requestContext,
      string ns,
      string topicName);

    string GetNamespaceAddress(IVssRequestContext requestContext, string ns);

    IEnumerable<RuleDescription> GetSubscriptionRules(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName);

    void RegisterHighAvailabilityNamespacePool(
      IVssRequestContext requestContext,
      string namepacePoolName,
      string namespacePoolList,
      bool isGlobal,
      ITFLogger logger);

    void RegisterHighAvailabilityPublisher(
      IVssRequestContext requestContext,
      string namepacePoolName,
      string PrimaryNamespace,
      string secondaryNamespace,
      bool isGlobal,
      ITFLogger logger);
  }
}
