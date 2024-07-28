// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusManagerHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusManagerHelper
  {
    private const string s_Area = "ServiceBus";
    private string m_Layer;
    private static readonly Random s_random = new Random();

    internal ServiceBusManagerHelper(string layer) => this.m_Layer = layer;

    internal SubscriptionDescription CreateSubscription(
      IVssRequestContext requestContext,
      NamespaceManager manager,
      string messageBusName,
      string ns,
      SubscriptionDescription subscriptionDescription,
      Microsoft.ServiceBus.Messaging.Filter filter)
    {
      bool flag;
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
        flag = manager.SubscriptionExists(subscriptionDescription.TopicPath, subscriptionDescription.Name);
      SubscriptionDescription subscription = (SubscriptionDescription) null;
      if (!flag)
      {
        try
        {
          using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
            subscription = filter == null ? manager.CreateSubscription(subscriptionDescription) : manager.CreateSubscription(subscriptionDescription, filter);
          requestContext.TraceAlways(1005242, TraceLevel.Info, "ServiceBus", this.m_Layer, "Created message bus subscription. Message bus: {0}, Subscription: {1}, Namespace {2}, AutoDeleteOnIdle: {3}", (object) messageBusName, (object) subscriptionDescription.Name, (object) ns, (object) subscriptionDescription.AutoDeleteOnIdle);
        }
        catch (MessagingEntityAlreadyExistsException ex)
        {
        }
      }
      if (subscription == null)
      {
        using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
          subscription = manager.GetSubscription(subscriptionDescription.TopicPath, subscriptionDescription.Name);
      }
      return subscription;
    }

    internal bool SubscriptionExists(
      IVssRequestContext requestContext,
      string messageBusName,
      string namespaceName,
      SubscriptionDescription subscriptionDescription)
    {
      NamespaceManager namespaceManager = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName, namespaceName).GetNamespaceManager();
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
        return namespaceManager.SubscriptionExists(subscriptionDescription.TopicPath, subscriptionDescription.Name);
    }

    internal TopicDescription CreateTopic(
      IVssRequestContext requestContext,
      NamespaceManager manager,
      TopicDescription topicDescription,
      bool deleteIfExists,
      string messageBusName,
      string ns)
    {
      bool flag;
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
      {
        flag = manager.TopicExists(topicDescription.Path);
        if (flag & deleteIfExists)
        {
          manager.DeleteTopic(topicDescription.Path);
          flag = false;
        }
      }
      if (!flag)
      {
        try
        {
          using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
            return manager.CreateTopic(topicDescription);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1005166, "ServiceBus", this.m_Layer, ex);
          if (!requestContext.ServiceHost.IsProduction && ex is MessagingException messagingException && messagingException.InnerException is WebException innerException1 && ((HttpWebResponse) innerException1.Response).StatusCode == HttpStatusCode.Conflict)
          {
            Thread.Sleep(ServiceBusManagerHelper.s_random.Next(0, 10000));
            throw new ServerBusyException("Hit a conflict creating topic " + messageBusName + " on namespace: " + ns + ". The server was not able to process the message.", ex);
          }
          if (ex is MessagingEntityAlreadyExistsException innerException2)
            throw new MessageBusPublisherAlreadyExistsException(HostingResources.ServiceBusPublisherAlreadyExists((object) topicDescription.Path, (object) ns, (object) messageBusName), (Exception) innerException2);
          throw;
        }
      }
      else
      {
        using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
          return manager.GetTopic(topicDescription.Path);
      }
    }

    internal static void DeleteTopic(
      IVssRequestContext requestContext,
      NamespaceManager manager,
      bool prefixMachineName,
      string topicName)
    {
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
      {
        if (prefixMachineName || !manager.TopicExists(topicName))
          return;
        manager.DeleteTopic(topicName);
      }
    }

    internal static bool DeleteSubscription(
      IVssRequestContext requestContext,
      NamespaceManager manager,
      string subscriptionName,
      string topicName)
    {
      bool flag = false;
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
      {
        if (manager.SubscriptionExists(topicName, subscriptionName))
        {
          manager.DeleteSubscription(topicName, subscriptionName);
          flag = true;
        }
      }
      return flag;
    }

    internal void DeleteSubscriptionWithRetry(
      IVssRequestContext requestContext,
      NamespaceManager manager,
      MessageBusSubscriptionInfo subscription,
      string topicName)
    {
      bool subscriptionDeleted = false;
      using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
      {
        Action Run = (Action) (() => subscriptionDeleted = ServiceBusManagerHelper.DeleteSubscription(requestContext, manager, subscription.SubscriptionName, topicName));
        CommandSetter commandsetter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusTopic.DeleteSubscriber." + subscription.MessageBusName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(manager.Settings.OperationTimeout));
        ServiceBusRetryHelper.ExecuteWithRetries((Action) (() => new CommandService(requestContext, commandsetter, Run).Execute()));
      }
      if (!subscriptionDeleted)
        return;
      requestContext.TraceAlways(1005262, TraceLevel.Info, "ServiceBus", this.m_Layer, "Deleted message bus subscription. Message bus: {0}, Subscription: {1}", (object) subscription.MessageBusName, (object) subscription.SubscriptionName);
    }

    internal SubscriptionDescription EnsureSubscriptionCreated(
      IVssRequestContext requestContext,
      string messageBusName,
      string ns,
      SubscriptionDescription subscriptionDescription,
      Microsoft.ServiceBus.Messaging.Filter filter,
      int retryCount = 5,
      TimeSpan? timeout = null)
    {
      requestContext = requestContext.Elevate();
      NamespaceManager manager = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName, ns).GetNamespaceManager();
      if (timeout.HasValue)
        manager.Settings.OperationTimeout = timeout.Value;
      using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
      {
        Func<SubscriptionDescription> Run = (Func<SubscriptionDescription>) (() => this.CreateSubscription(requestContext, manager, messageBusName, ns, subscriptionDescription, filter));
        CommandSetter commandsetter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusTopic.CreateSubscriber." + messageBusName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(manager.Settings.OperationTimeout));
        return ServiceBusRetryHelper.ExecuteWithRetries<SubscriptionDescription>((Func<SubscriptionDescription>) (() => new CommandService<SubscriptionDescription>(requestContext, commandsetter, Run).Execute()), retryCount, 0);
      }
    }

    internal static NamespaceManager GetNamespaceManagerFromNamespace(
      IVssRequestContext requestContext,
      string ns)
    {
      Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", ns, string.Empty);
      string managementSetting = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<string>(requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/..."), ns, "/SharedAccessKeySettingName", string.Empty);
      TokenProvider tokenProvider = ServiceBusSettingsHelper.GetMessageBusCredentials(requestContext, managementSetting).CreateTokenProvider();
      return new NamespaceManager(serviceUri, tokenProvider);
    }

    internal TopicDescription EnsureTopicCreated(
      IVssRequestContext requestContext,
      string messageBusName,
      TopicDescription topicDescription,
      string ns,
      bool deleteIfExists)
    {
      requestContext = requestContext.Elevate();
      TopicDescription returnDescription = (TopicDescription) null;
      NamespaceManager manager = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName, ns).GetNamespaceManager();
      manager.Settings.OperationTimeout = TimeSpan.FromMinutes(3.0);
      using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
      {
        Action Run = (Action) (() => returnDescription = this.CreateTopic(requestContext, manager, topicDescription, deleteIfExists, messageBusName, ns));
        CommandSetter commandsetter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusNamespace.CreatePublisher." + ns)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(manager.Settings.OperationTimeout));
        ServiceBusRetryHelper.ExecuteWithRetries((Action) (() => new CommandService(requestContext, commandsetter, Run).Execute()));
      }
      return returnDescription;
    }

    internal static IEnumerable<SubscriptionDescription> GetSubscriptions(
      IVssRequestContext requestContext,
      NamespaceManager manager,
      string topicName)
    {
      manager.Settings.OperationTimeout = TimeSpan.FromMinutes(3.0);
      return manager?.GetSubscriptions(topicName);
    }

    internal static IEnumerable<TopicDescription> GetTopics(
      IVssRequestContext requestContext,
      NamespaceManager manager)
    {
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
        return manager.GetTopics();
    }

    internal static void UpdateSubscription(
      IVssRequestContext requestContext,
      NamespaceManager manager,
      SubscriptionDescription subscription)
    {
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
        manager.UpdateSubscription(subscription);
    }

    internal static TopicDescription UpdateTopic(
      IVssRequestContext requestContext,
      NamespaceManager manager,
      TopicDescription topicDescription)
    {
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
        return manager.UpdateTopic(topicDescription);
    }

    internal void VerifyTopicCreated(
      IVssRequestContext requestContext,
      NamespaceManager manager,
      string topicName)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
        ServiceBusRetryHelper.ExecuteWithRetries((Action) (() =>
        {
          bool flag;
          using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
            flag = manager.TopicExists(topicName);
          if (!flag)
          {
            requestContext.Trace(1005245, TraceLevel.Warning, "ServiceBus", this.m_Layer, "Topic {0} does not exist", (object) topicName);
            throw new MessageBusNotFoundException(topicName);
          }
        }));
    }
  }
}
