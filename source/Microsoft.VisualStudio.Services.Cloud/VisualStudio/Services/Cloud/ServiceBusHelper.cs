// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal static class ServiceBusHelper
  {
    private static readonly RegistryQuery s_registryQueryForManagementSettings = new RegistryQuery("/Service/MessageBus/ServiceBus/Management/...");
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusHelper";
    internal static readonly string m_messageSizeExceededExceptionTraceMessage = "Post SendBatch the message was calculated to have {0} elements and be of size {1} when publishing to {2}.";
    private static Dictionary<ServiceBusHelper.MessagingFactoryKey, WeakReference<MessagingFactory>> s_factories = new Dictionary<ServiceBusHelper.MessagingFactoryKey, WeakReference<MessagingFactory>>();
    private static object s_factoryLock = new object();

    internal static IVssRequestContext GetAppropriateRequestContext(
      IVssRequestContext deploymentContext,
      BrokeredMessage message,
      MessageBusSubscriptionInfo subscription,
      TeamFoundationHostType acceptedHostTypes)
    {
      Guid guid = deploymentContext.ServiceHost.InstanceId;
      ITeamFoundationHostManagementService service = deploymentContext.GetService<ITeamFoundationHostManagementService>();
      int num;
      if (message.Properties.TryGetValue<int>("SourceHostType", out num))
      {
        TeamFoundationHostType hostType = (TeamFoundationHostType) num;
        deploymentContext.Trace(1004300, TraceLevel.Info, "ServiceBus", nameof (ServiceBusHelper), "Found host type {0}, Subscriber {1}", (object) hostType, (object) subscription);
        if (!hostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        {
          guid = (Guid) message.Properties["SourceHostId"];
          deploymentContext.Trace(1004301, TraceLevel.Info, "ServiceBus", nameof (ServiceBusHelper), "Found source host id {0}, Subscriber {1}", (object) guid, (object) subscription);
        }
        if ((hostType & acceptedHostTypes) == TeamFoundationHostType.Unknown)
        {
          deploymentContext.Trace(1004302, TraceLevel.Info, "ServiceBus", nameof (ServiceBusHelper), "Source Host Type {0}, Accepted Host Type {1}, Subscriber {2}", (object) hostType, (object) acceptedHostTypes, (object) subscription);
          switch (hostType)
          {
            case TeamFoundationHostType.Deployment:
              throw new UnexpectedHostTypeException(hostType);
            case TeamFoundationHostType.Application:
              if (!acceptedHostTypes.HasFlag((Enum) TeamFoundationHostType.Deployment))
                throw new UnexpectedHostTypeException(hostType);
              guid = deploymentContext.ServiceHost.InstanceId;
              break;
            case TeamFoundationHostType.ProjectCollection:
              if (acceptedHostTypes.HasFlag((Enum) TeamFoundationHostType.Application))
              {
                guid = (Guid) message.Properties["ParentSourceHostId"];
                break;
              }
              if (!acceptedHostTypes.HasFlag((Enum) TeamFoundationHostType.Deployment))
                throw new UnexpectedHostTypeException(hostType);
              guid = deploymentContext.ServiceHost.InstanceId;
              break;
          }
        }
      }
      HostProperties hostProperties = service.QueryServiceHostPropertiesCached(deploymentContext, guid);
      if (hostProperties == null)
        throw new HostDoesNotExistException(guid);
      if (hostProperties.Status != TeamFoundationServiceHostStatus.Started)
        throw new HostShutdownException(string.Format("Could not deliver message {0} to Subscriber: {1} as host {2} is not started {3}.", (object) message.MessageId, (object) subscription, (object) guid, (object) hostProperties.Status));
      IVssRequestContext requestContext = service.BeginRequest(deploymentContext, guid, RequestContextType.SystemContext);
      requestContext.RequestContextInternal().ResetActivityId();
      return requestContext;
    }

    internal static void ResetServiceBusSettingsToDefault(
      IVssRequestContext requestContext,
      ITFLogger logger)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryEntryCollection registryEntryCollection = service.ReadEntries(requestContext, ServiceBusHelper.s_registryQueryForManagementSettings);
      RegistryEntry entry;
      if (!registryEntryCollection.TryGetValue("/Service/MessageBus/ServiceBus/Management/Namespace", out entry))
        logger.Info("Service Bus is not configured.");
      List<RegistryItem> writeToRegistry = new List<RegistryItem>();
      foreach (RegistryEntry registryEntry in registryEntryCollection.Where<RegistryEntry>((Func<RegistryEntry, bool>) (r => r.Path.EndsWith("/Namespace", StringComparison.OrdinalIgnoreCase))))
      {
        bool prefixComputerName = ServiceBusDefaultSettings.GetDefaultPrefixComputerName(requestContext, registryEntry.Value);
        logger.Info(ServiceBusHelper.SetIfDifferent<bool>(registryEntryCollection, ServiceBusHelper.GetSiblingSettingPath(registryEntry.Path, "/PrefixComputerName"), prefixComputerName, prefixComputerName, ref writeToRegistry));
        int topicMaxSizeInGb = ServiceBusDefaultSettings.GetDefaultTopicMaxSizeInGB(requestContext, registryEntry.Value);
        logger.Info(ServiceBusHelper.SetIfDifferent<int>(registryEntryCollection, ServiceBusHelper.GetSiblingSettingPath(registryEntry.Path, "/TopicMaxSizeInGB"), topicMaxSizeInGb, topicMaxSizeInGb, ref writeToRegistry));
      }
      double totalMinutes1 = ServiceBusDefaultSettings.GetDefaultIdlePeriod(requestContext, entry.Value).TotalMinutes;
      logger.Info(ServiceBusHelper.SetIfDifferent<double>(registryEntryCollection, "/Service/MessageBus/ServiceBus/Management/IdlePeriod", totalMinutes1, totalMinutes1, ref writeToRegistry));
      double totalMinutes2 = ServiceBusDefaultSettings.GetDefaultSubscriptionIdlePeriod(requestContext, entry.Value).TotalMinutes;
      logger.Info(ServiceBusHelper.SetIfDifferent<double>(registryEntryCollection, "/Service/MessageBus/ServiceBus/Management/SubscriptionIdlePeriod", totalMinutes1, totalMinutes2, ref writeToRegistry));
      if (!writeToRegistry.Any<RegistryItem>())
        return;
      service.Write(requestContext, (IEnumerable<RegistryItem>) writeToRegistry);
    }

    private static string GetSiblingSettingPath(string registryPath, string siblingRelativePath) => !string.IsNullOrEmpty(registryPath) && registryPath.IndexOf('/') >= 0 && registryPath[registryPath.Length - 1] != '/' ? RegistryHelpers.CombinePath(registryPath.Substring(0, registryPath.LastIndexOf('/')), siblingRelativePath) : throw new ArgumentException("Unexpected registry path: '" + registryPath + "'.");

    internal static string SetIfDifferent<T>(
      RegistryEntryCollection registryCollection,
      string registryPath,
      T defaultValue,
      T newValue,
      ref List<RegistryItem> writeToRegistry)
      where T : IEquatable<T>
    {
      RegistryEntry entry;
      T currentValue;
      string str;
      if (registryCollection.TryGetValue(registryPath, out entry))
      {
        currentValue = entry.GetValue<T>();
        str = currentValue.ToString();
      }
      else
      {
        currentValue = defaultValue;
        str = "unset";
      }
      if (ServiceBusHelper.SetIfDifferent<T>(registryPath, currentValue, newValue, ref writeToRegistry))
        return string.Format("Setting '{0}' from '{1}' to '{2}'.", (object) registryPath, (object) str, (object) newValue);
      return "Skipping '" + registryPath + "'. Current value: '" + str + "'.";
    }

    internal static MessageBusMessageSizeExceededException HandleSizeExceededException(
      IVssRequestContext requestContext,
      MessageSizeExceededException exception,
      string messageBusName,
      List<BrokeredMessage> messages)
    {
      requestContext.Trace(1005609, TraceLevel.Error, "ServiceBus", nameof (ServiceBusHelper), ServiceBusHelper.m_messageSizeExceededExceptionTraceMessage, (object) messages.Count, (object) messages.Sum<BrokeredMessage>((Func<BrokeredMessage, long>) (m => m.Size)), (object) messageBusName);
      exception.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
      return new MessageBusMessageSizeExceededException(exception.Message, (Exception) exception);
    }

    private static bool SetIfDifferent<T>(
      string registryPath,
      T currentValue,
      T newValue,
      ref List<RegistryItem> writeToRegistry)
      where T : IEquatable<T>
    {
      if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
        return false;
      writeToRegistry.Add(new RegistryItem(registryPath, newValue.ToString()));
      return true;
    }

    public static MessagingFactory GetMessagingFactory(
      Uri uri,
      MessageBusCredentials messageBusCredentials,
      int batchFlushInterval,
      TransportType transportType)
    {
      ServiceBusHelper.MessagingFactoryKey key = new ServiceBusHelper.MessagingFactoryKey(uri, messageBusCredentials, batchFlushInterval, transportType);
      MessagingFactory target = (MessagingFactory) null;
      WeakReference<MessagingFactory> weakReference = (WeakReference<MessagingFactory>) null;
      if (!ServiceBusHelper.s_factories.TryGetValue(key, out weakReference) || !weakReference.TryGetTarget(out target))
      {
        lock (ServiceBusHelper.s_factoryLock)
        {
          if (ServiceBusHelper.s_factories.TryGetValue(key, out weakReference))
          {
            if (weakReference.TryGetTarget(out target))
              goto label_10;
          }
          MessagingFactorySettings factorySettings = new MessagingFactorySettings();
          factorySettings.TokenProvider = messageBusCredentials.CreateTokenProvider();
          if (batchFlushInterval > 0)
          {
            factorySettings.NetMessagingTransportSettings.BatchFlushInterval = TimeSpan.FromMilliseconds((double) batchFlushInterval);
            factorySettings.AmqpTransportSettings.BatchFlushInterval = TimeSpan.FromMilliseconds((double) batchFlushInterval);
          }
          factorySettings.TransportType = transportType;
          target = MessagingFactory.Create(uri, factorySettings);
          ServiceBusHelper.s_factories[key] = new WeakReference<MessagingFactory>(target);
        }
      }
label_10:
      return target;
    }

    private struct MessagingFactoryKey
    {
      public MessagingFactoryKey(
        Uri uri,
        MessageBusCredentials credentials,
        int batchFlushInterval,
        TransportType transportType)
      {
        this.Uri = uri;
        this.Credentials = credentials;
        this.BatchFlushInterval = batchFlushInterval;
        this.TransportType = transportType;
      }

      public Uri Uri { get; private set; }

      public MessageBusCredentials Credentials { get; private set; }

      public int BatchFlushInterval { get; private set; }

      public TransportType TransportType { get; private set; }

      public override int GetHashCode() => (int) (((((17 * 23 + this.Uri.GetHashCode()) * 23 + this.Credentials.SharedAccessKeyName.GetHashCode()) * 23 + this.Credentials.SharedAccessKeyValue.GetHashCode()) * 23 + this.BatchFlushInterval) * 23 + this.TransportType);

      public override bool Equals(object obj) => obj is ServiceBusHelper.MessagingFactoryKey messagingFactoryKey && this.BatchFlushInterval == messagingFactoryKey.BatchFlushInterval && this.TransportType == messagingFactoryKey.TransportType && this.Uri.Equals((object) messagingFactoryKey.Uri) && this.Credentials.Equals(messagingFactoryKey.Credentials);
    }
  }
}
