// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusDeadLetterQueueHelpers
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusDeadLetterQueueHelpers
  {
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusDeadLetterQueue";

    public static (bool success, int deletedMessage, string error) CleanupDeadLetter(
      IVssRequestContext requestContext,
      string ns,
      string topicName = null,
      string subscriptionName = null)
    {
      if (subscriptionName != null && topicName == null)
        return (false, 0, "If you specify a subscriptionName you must also specify a topicName");
      NamespaceManager managerFromNamespace = ServiceBusManagerHelper.GetNamespaceManagerFromNamespace(requestContext, ns);
      List<TopicDescription> topicDescriptionList = new List<TopicDescription>();
      if (string.IsNullOrEmpty(topicName))
      {
        topicDescriptionList.AddRange(managerFromNamespace.GetTopics());
      }
      else
      {
        try
        {
          TopicDescription topic = managerFromNamespace.GetTopic(topicName);
          topicDescriptionList.Add(topic);
        }
        catch (MessagingEntityNotFoundException ex)
        {
        }
      }
      if (topicDescriptionList.Count == 0)
        return (false, 0, "No topic was found in the namespace " + ns);
      List<SubscriptionDescription> subscriptionDescriptionList = new List<SubscriptionDescription>();
      if (string.IsNullOrEmpty(subscriptionName))
      {
        foreach (TopicDescription topicDescription in topicDescriptionList)
          subscriptionDescriptionList.AddRange(managerFromNamespace.GetSubscriptions(topicDescription.Path));
      }
      else
      {
        try
        {
          SubscriptionDescription subscription = managerFromNamespace.GetSubscription(topicName, subscriptionName);
          subscriptionDescriptionList.Add(subscription);
        }
        catch (MessagingEntityNotFoundException ex)
        {
        }
      }
      if (subscriptionDescriptionList.Count == 0)
        return (false, 0, "No subscription was found in the namespace " + ns);
      Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", ns, string.Empty);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string managementSetting = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<string>(service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/..."), ns, "/SharedAccessKeySettingName", string.Empty);
      MessageBusCredentials messageBusCredentials1 = ServiceBusSettingsHelper.GetMessageBusCredentials(requestContext, managementSetting);
      TransportType transportType = service.GetValue<TransportType>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/TransportType", TransportType.NetMessaging);
      MessageBusCredentials messageBusCredentials2 = messageBusCredentials1;
      int num1 = (int) transportType;
      MessagingFactory messagingFactory = ServiceBusHelper.GetMessagingFactory(serviceUri, messageBusCredentials2, 5000, (TransportType) num1);
      int num2 = 0;
      bool flag = true;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (SubscriptionDescription subscriptionDescription in subscriptionDescriptionList)
      {
        try
        {
          num2 += ServiceBusDeadLetterQueueHelpers.CleanupDeadLetterSubscription(requestContext, messagingFactory, ns, subscriptionDescription.TopicPath, subscriptionDescription.Name);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1005554, "ServiceBus", subscriptionDescription.TopicPath, ex);
          if (flag)
            stringBuilder.Append("Errors in the folowing subscriptions: ");
          flag = false;
          stringBuilder.Append(subscriptionDescription.TopicPath + "/" + subscriptionDescription.Name + "; ");
        }
      }
      return (flag, num2, stringBuilder.ToString());
    }

    private static int CleanupDeadLetterSubscription(
      IVssRequestContext requestContext,
      MessagingFactory factory,
      string ns,
      string topicName,
      string subscriptionName)
    {
      string entityPath = SubscriptionClient.FormatDeadLetterPath(topicName, subscriptionName);
      MessageReceiver m_deadLetterReceiver = factory.CreateMessageReceiver(entityPath, ReceiveMode.ReceiveAndDelete);
      requestContext.TraceAlways(1005617, TraceLevel.Info, "ServiceBus", "ServiceBusDeadLetterQueue", "Starting cleanup in the path " + entityPath);
      int messageDeleted = 0;
      ServiceBusRetryHelper.ExecuteWithRetries((Action) (() =>
      {
        while (m_deadLetterReceiver.Peek() != null)
        {
          IEnumerable<BrokeredMessage> batch = m_deadLetterReceiver.ReceiveBatch(1000, TimeSpan.FromSeconds(5.0));
          int num = batch == null ? 0 : batch.Count<BrokeredMessage>();
          messageDeleted += num;
          VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusDeadLetterMessagesDeletedTotal", topicName);
          performanceCounter.IncrementBy((long) num);
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusDeadLetterMessagesDeletedPerSec", topicName);
          performanceCounter.IncrementBy((long) num);
          DateTime utcNow = DateTime.UtcNow;
          foreach (BrokeredMessage brokeredMessage in batch)
            ServiceBusTracer.TraceServiceBusSubscriberActivity(requestContext, ns, topicName, nameof (CleanupDeadLetterSubscription), brokeredMessage.MessageId, brokeredMessage.ContentType, utcNow, brokeredMessage.Properties.GetCastedValueOrDefault<string, Guid>("SourceServiceInstanceId", Guid.Empty), brokeredMessage.Properties.GetCastedValueOrDefault<string, Guid>("SourceServiceInstanceType", Guid.Empty), true, (Exception) null);
        }
      }));
      return messageDeleted;
    }
  }
}
