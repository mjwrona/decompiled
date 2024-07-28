// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IMessageBusManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation("Microsoft.VisualStudio.Services.Cloud.ServiceBusManagementService, Microsoft.VisualStudio.Services.Cloud")]
  public interface IMessageBusManagementService : IVssFrameworkService
  {
    void CreatePublisher(
      IVssRequestContext requestContext,
      string messageBusName,
      bool deleteIfExists,
      double subscriberDeleteOnIdleMinutes);

    void CreatePublisher(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusPublisherCreateOptions options);

    void DeletePublisher(IVssRequestContext requestContext, string messageBusName);

    MessageBusSubscriptionInfo CreateTransientSubscriber(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriberPrefix);

    MessageBusSubscriptionInfo CreateSubscriber(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      ITFLogger logger = null);

    void DeleteSubscriber(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription);

    void UpdateSubscribers(
      IVssRequestContext requestContext,
      MessageBusSubscriberSettings subscriberSettings,
      string messageBusName,
      string subscriptionName = null,
      string namespaceName = null);

    IEnumerable<MessageBusSubscriptionInfo> GetSubscribers(
      IVssRequestContext requestContext,
      string topicName);

    void FixMessageQueueMappings(
      IVssRequestContext deploymentContext,
      string ns,
      string hostNamePrefix,
      string sharedAccessKeySettingName,
      ITFLogger logger);

    string GetSubscriberNameForScaleUnit(IVssRequestContext requestContext, string messageBusName);

    void ClearPublishers(IVssRequestContext requestContext, bool dispose);

    void UpdateSubscriptionFilter(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      string newFilterValue,
      bool isTransient = false,
      ITFLogger logger = null);
  }
}
