// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusConnectionManager
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusConnectionManager
  {
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusManagementService";
    private ConcurrentDictionary<string, IServiceBusPublishConnection> m_publishEntries;
    private ConcurrentDictionary<MessageBusSubscriptionInfo, ServiceBusSubscribeConnection> m_subscribeEntries;
    private ConcurrentDictionary<MessageBusHighAvailabilitySubscribeEntry, string> m_highAvailabilityEntries;

    internal ServiceBusConnectionManager()
    {
      this.m_publishEntries = new ConcurrentDictionary<string, IServiceBusPublishConnection>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_subscribeEntries = new ConcurrentDictionary<MessageBusSubscriptionInfo, ServiceBusSubscribeConnection>();
      this.m_highAvailabilityEntries = new ConcurrentDictionary<MessageBusHighAvailabilitySubscribeEntry, string>();
    }

    internal void ClearPublishers(IVssRequestContext requestContext, bool dispose = false)
    {
      if (this.m_publishEntries == null)
        return;
      requestContext.TraceAlways(1005574, TraceLevel.Info, "ServiceBus", "ServiceBusManagementService", "ClearPublishers called");
      foreach (string key in (IEnumerable<string>) this.m_publishEntries.Keys)
      {
        using (requestContext.Lock(ServiceBusLockHelper.GetPublisherLockName(requestContext, key)))
        {
          IServiceBusPublishConnection publishConnection;
          if (this.m_publishEntries.TryRemove(key, out publishConnection) & dispose)
            publishConnection.Dispose();
        }
      }
    }

    internal void ClearSubscribers(IVssRequestContext requestContext)
    {
      if (this.m_subscribeEntries == null)
        return;
      foreach (MessageBusSubscriptionInfo key in (IEnumerable<MessageBusSubscriptionInfo>) this.m_subscribeEntries.Keys)
      {
        using (requestContext.Lock(ServiceBusLockHelper.GetSubscriberLockName(requestContext, key)))
        {
          ServiceBusSubscribeConnection subscribeConnection;
          if (this.m_subscribeEntries.TryRemove(key, out subscribeConnection))
          {
            subscribeConnection.Dispose();
            requestContext.TraceAlways(1005570, TraceLevel.Info, "ServiceBus", "ServiceBusManagementService", "Removing subscriber {0}/{1}/{2}", (object) (key.MessageBusName ?? string.Empty), (object) (key.SubscriptionName ?? string.Empty), (object) (key.Namespace ?? string.Empty));
          }
        }
      }
    }

    internal IServiceBusPublishConnection AddPublishConnection(
      IVssRequestContext requestContext,
      string keyName,
      IServiceBusPublishConnection newConnInfo,
      string layer)
    {
      IServiceBusPublishConnection publishConnection;
      using (requestContext.Lock(ServiceBusLockHelper.GetPublisherLockName(requestContext, keyName)))
      {
        if (!this.m_publishEntries.TryGetValue(keyName, out publishConnection))
        {
          this.m_publishEntries[keyName] = newConnInfo;
          publishConnection = newConnInfo;
          requestContext.Trace(1005153, TraceLevel.Info, "ServiceBus", layer, "Added entry to m_publishentries");
        }
        else
          newConnInfo.Dispose();
      }
      return publishConnection;
    }

    internal void AddSubscribeConnection(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      ServiceBusSubscribeConnection connInfo)
    {
      this.m_subscribeEntries[subscription] = connInfo;
      requestContext.TraceAlways(1005571, TraceLevel.Info, "ServiceBus", "ServiceBusManagementService", "Adding subscriber {0}/{1}/{2}", (object) (subscription.MessageBusName ?? string.Empty), (object) (subscription.SubscriptionName ?? string.Empty), (object) (subscription.Namespace ?? string.Empty));
    }

    internal void AddHighAvailabilitySubscribeEntry(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      this.m_highAvailabilityEntries[new MessageBusHighAvailabilitySubscribeEntry(subscription)] = subscription.NamespacePoolName;
      requestContext.TraceAlways(1005575, TraceLevel.Info, "ServiceBus", "ServiceBusManagementService", "Adding HA subscriber entry {0}/{1}/{2}", (object) (subscription.MessageBusName ?? string.Empty), (object) (subscription.SubscriptionName ?? string.Empty), (object) (subscription.NamespacePoolName ?? string.Empty));
    }

    internal bool TryGetHighAvailabilitySubscribeEntry(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      out string namespacePoolName)
    {
      return this.m_highAvailabilityEntries.TryGetValue(new MessageBusHighAvailabilitySubscribeEntry(subscription), out namespacePoolName);
    }

    internal bool RemoveHighAvailabilitySubscribeEntry(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      return this.m_highAvailabilityEntries.TryRemove(new MessageBusHighAvailabilitySubscribeEntry(subscription), out string _);
    }

    internal bool DoesSubscriberConnectionExist(MessageBusSubscriptionInfo subscription) => this.m_subscribeEntries.ContainsKey(subscription);

    internal IReadOnlyDictionary<MessageBusSubscriptionInfo, ServiceBusSubscribeConnection> GetReadOnlySubscriberConnections() => (IReadOnlyDictionary<MessageBusSubscriptionInfo, ServiceBusSubscribeConnection>) this.m_subscribeEntries;

    internal void RemovePublishConnection(IVssRequestContext requestContext, string keyName)
    {
      using (requestContext.Lock(ServiceBusLockHelper.GetPublisherLockName(requestContext, keyName)))
      {
        IServiceBusPublishConnection publishConnection;
        if (!this.m_publishEntries.TryRemove(keyName, out publishConnection))
          return;
        publishConnection.Dispose();
      }
    }

    internal void RemoveSubscribeConnection(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      using (requestContext.Lock(ServiceBusLockHelper.GetSubscriberLockName(requestContext, subscription)))
      {
        ServiceBusSubscribeConnection subscribeConnection;
        if (this.m_subscribeEntries.TryRemove(subscription, out subscribeConnection))
        {
          subscribeConnection.Dispose();
          requestContext.TraceAlways(1005572, TraceLevel.Info, "ServiceBus", "ServiceBusManagementService", "Removing subscriber {0}/{1}/{2}", (object) (subscription.MessageBusName ?? string.Empty), (object) (subscription.SubscriptionName ?? string.Empty), (object) (subscription.Namespace ?? string.Empty));
        }
        else
          requestContext.Trace(1005211, TraceLevel.Error, "ServiceBus", "ServiceBusManagementService", "Could not find subscription {0}", (object) subscription);
      }
    }

    internal bool TryGetPublishConnection(string keyName, out IServiceBusPublishConnection connInfo) => this.m_publishEntries.TryGetValue(keyName, out connInfo);

    internal IServiceBusPublishConnection GetConnectionInfo(
      IVssRequestContext requestContext,
      string messageBusName,
      bool throwOnMissingPublisher,
      out string layer,
      out string ns)
    {
      string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
      layer = messageBusName;
      IServiceBusPublishConnection connInfo;
      if (this.TryGetPublishConnection(publisherRegistryRoot, out connInfo))
      {
        ns = connInfo.Namespace;
        return connInfo;
      }
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (publisherRegistryRoot + "/*"));
      ns = registryEntryCollection.GetValueFromPath<string>(publisherRegistryRoot + "/Namespace", string.Empty);
      string valueFromPath = registryEntryCollection.GetValueFromPath<string>(publisherRegistryRoot + "/TopicName", string.Empty);
      if (string.IsNullOrEmpty(ns) || string.IsNullOrEmpty(valueFromPath))
      {
        if (throwOnMissingPublisher)
        {
          MessageBusNotFoundException notFoundException = new MessageBusNotFoundException(messageBusName);
          requestContext.TraceException(1005154, "ServiceBus", "ServiceBusManagementService", (Exception) notFoundException);
          throw notFoundException;
        }
        return (IServiceBusPublishConnection) null;
      }
      IServiceBusPublishConnection publishConnection = this.CreateServiceBusPublishConnection(requestContext, ns, valueFromPath);
      requestContext.TraceAlways(1005573, TraceLevel.Info, "ServiceBus", "ServiceBusManagementService", "Legacy Publish Connection created for topic {0} with namespace {1}", (object) valueFromPath, (object) ns);
      return this.AddPublishConnection(requestContext, publisherRegistryRoot, publishConnection, layer);
    }

    internal IServiceBusPublishConnection CreateServiceBusHighAvailabilityPublishConnection(
      IVssRequestContext requestContext,
      string primaryNamespace,
      string secondaryNamespace,
      string messageBusName)
    {
      requestContext = requestContext.Elevate();
      string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
      string valueFromPath = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (publisherRegistryRoot + "/*")).GetValueFromPath<string>(publisherRegistryRoot + "/TopicName", string.Empty);
      IServiceBusPublishConnection connInfo;
      if (this.TryGetPublishConnection(publisherRegistryRoot, out connInfo))
        return connInfo;
      HighAvailabilityPublisherSettings poolSettings = new HighAvailabilityPublisherSettings()
      {
        Primary = this.GetPublisherSettingsManager(requestContext, primaryNamespace, messageBusName),
        Secondary = this.GetPublisherSettingsManager(requestContext, secondaryNamespace, messageBusName)
      };
      IServiceBusPublishConnection newConnInfo = (IServiceBusPublishConnection) new ServiceBusPublishHighAvailabilityConnection(requestContext, poolSettings, valueFromPath);
      requestContext.TraceAlways(1005572, TraceLevel.Info, "ServiceBus", "ServiceBusManagementService", "High Availability Publish Connection created for topic {0} with Primary {1} and Secondary {2}", (object) valueFromPath, (object) primaryNamespace, (object) secondaryNamespace);
      return this.AddPublishConnection(requestContext, publisherRegistryRoot, newConnInfo, messageBusName);
    }

    private PublisherSettings GetPublisherSettingsManager(
      IVssRequestContext requestContext,
      string namespaceName,
      string topicName)
    {
      NamespaceManagerSettings namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, topicName, namespaceName);
      MessageBusCredentials messageBusCredentials = namespaceManagerSettings.MessageBusCredentials;
      Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", namespaceName, string.Empty);
      return new PublisherSettings(namespaceName, namespaceManagerSettings, messageBusCredentials, serviceUri);
    }

    internal IServiceBusPublishConnection CreateServiceBusPublishConnection(
      IVssRequestContext requestContext,
      string ns,
      string topicName)
    {
      requestContext = requestContext.Elevate();
      MessageBusCredentials messageBusCredentials = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, string.Empty, ns).MessageBusCredentials;
      Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", ns, string.Empty);
      return (IServiceBusPublishConnection) new ServiceBusPublishConnection(requestContext, serviceUri, messageBusCredentials, ns, topicName);
    }
  }
}
