// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusMonitoring
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusMonitoring
  {
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusMonitoring";

    internal static void MonitorSubscriptions(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1005400, "ServiceBus", nameof (ServiceBusMonitoring), nameof (MonitorSubscriptions));
      try
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        IVssRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = requestContext;
        RegistryQuery registryQuery = (RegistryQuery) "/Service/MessageBus/ServiceBus/Subscriber/Alert";
        ref RegistryQuery local1 = ref registryQuery;
        int defaultAlertValue = registryService1.GetValue<int>(requestContext1, in local1, 3600);
        IVssRegistryService registryService2 = service;
        IVssRequestContext requestContext2 = requestContext;
        registryQuery = (RegistryQuery) "/Service/MessageBus/ServiceBus/Subscriber/UnprocessedMessageCount";
        ref RegistryQuery local2 = ref registryQuery;
        int unprocessedMessagesDefault = registryService2.GetValue<int>(requestContext2, in local2, 10000);
        requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, ServiceBusManagementService.MonitorSubscribersEventClass, (string) null);
        Func<IGrouping<string, RegistryEntry>, Tuple<int, int>> GetSubscriberAlert = (Func<IGrouping<string, RegistryEntry>, Tuple<int, int>>) (group =>
        {
          int num1 = defaultAlertValue;
          RegistryEntry registryEntry1 = group.FirstOrDefault<RegistryEntry>((Func<RegistryEntry, bool>) (regEntry => VssStringComparer.MessageBusSubscriptionName.Equals(regEntry.Name, "SubscriberIdleAlertInSeconds")));
          if (registryEntry1 != null)
            num1 = registryEntry1.GetValue<int>();
          int num2 = unprocessedMessagesDefault;
          RegistryEntry registryEntry2 = group.FirstOrDefault<RegistryEntry>((Func<RegistryEntry, bool>) (regEntry => VssStringComparer.MessageBusSubscriptionName.Equals(regEntry.Name, "UnprocessedMessageCount")));
          if (registryEntry2 != null)
            num2 = registryEntry2.GetValue<int>();
          return new Tuple<int, int>(num1, num2);
        });
        Dictionary<string, SubscriberSettings> dictionary = service.ReadEntries(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Subscriber/**").GroupBy<RegistryEntry, string>((Func<RegistryEntry, string>) (regEntry => ServiceBusSettingsHelper.GetTopicNameFromPath(regEntry.Path))).Select(group =>
        {
          Tuple<int, int> tuple = GetSubscriberAlert(group);
          return new
          {
            TopicPathName = group.Key,
            SubscriberSettings = new SubscriberSettings(tuple.Item1, tuple.Item2)
          };
        }).ToDictionary(x => x.TopicPathName, x => x.SubscriberSettings, (IEqualityComparer<string>) VssStringComparer.MessageBusName);
        List<ServiceBusMonitoring.SubscriberInfo> list = service.ReadEntries(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Subscriber/**").GroupBy<RegistryEntry, string>((Func<RegistryEntry, string>) (regEntry => regEntry.Path.Substring(0, regEntry.Path.LastIndexOf('/')))).Where<IGrouping<string, RegistryEntry>>((Func<IGrouping<string, RegistryEntry>, bool>) (group => group.Any<RegistryEntry>((Func<RegistryEntry, bool>) (regEntry => VssStringComparer.MessageBusSubscriptionName.Equals(regEntry.Name, "TopicName"))))).Select<IGrouping<string, RegistryEntry>, ServiceBusMonitoring.SubscriberInfo>((Func<IGrouping<string, RegistryEntry>, ServiceBusMonitoring.SubscriberInfo>) (group =>
        {
          string topicNameFromPath = ServiceBusSettingsHelper.GetTopicNameFromPath(group.Key);
          bool flag = false;
          RegistryEntry registryEntry = group.FirstOrDefault<RegistryEntry>((Func<RegistryEntry, bool>) (regEntry => VssStringComparer.MessageBusName.Equals(regEntry.Name, "IsTransient")));
          if (registryEntry != null)
            flag = registryEntry.GetValue<bool>(false);
          return new ServiceBusMonitoring.SubscriberInfo()
          {
            PathTopicName = topicNameFromPath,
            IsTransient = flag,
            Namespace = group.First<RegistryEntry>((Func<RegistryEntry, bool>) (regEntry => VssStringComparer.MessageBusName.Equals(regEntry.Name, "Namespace"))).Value,
            TopicName = group.First<RegistryEntry>((Func<RegistryEntry, bool>) (regEntry => VssStringComparer.MessageBusName.Equals(regEntry.Name, "TopicName"))).Value,
            SubscriptionName = group.First<RegistryEntry>((Func<RegistryEntry, bool>) (regEntry => VssStringComparer.MessageBusSubscriptionName.Equals(regEntry.Name, "SubscriptionName"))).Value
          };
        })).ToList<ServiceBusMonitoring.SubscriberInfo>();
        if (list.Count == 0)
        {
          requestContext.Trace(105402, TraceLevel.Info, "ServiceBus", nameof (ServiceBusMonitoring), "Couldn't find any subscribers bailing");
        }
        else
        {
          RegistryEntryCollection managementSettings = service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/...");
          ServiceBusMonitoring.MonitorTopicsAndSubscription(requestContext, list, dictionary, managementSettings);
        }
      }
      finally
      {
        requestContext.TraceLeave(1005499, "ServiceBus", nameof (ServiceBusMonitoring), nameof (MonitorSubscriptions));
      }
    }

    private static void MonitorTopicsAndSubscription(
      IVssRequestContext requestContext,
      List<ServiceBusMonitoring.SubscriberInfo> subscribers,
      Dictionary<string, SubscriberSettings> subscriberAlertSettings,
      RegistryEntryCollection managementSettings)
    {
      Dictionary<string, NamespaceManager> dictionary = new Dictionary<string, NamespaceManager>();
      foreach (IGrouping<(string, string), ServiceBusMonitoring.SubscriberInfo> grouping in subscribers.GroupBy<ServiceBusMonitoring.SubscriberInfo, (string, string)>((Func<ServiceBusMonitoring.SubscriberInfo, (string, string)>) (x => (x.Namespace, x.TopicName))))
      {
        (string str1, string str2) = grouping.Key;
        int unhealthySubscriberCount1 = 0;
        StringBuilder stringBuilder1 = new StringBuilder(HostingResources.MessageBusSubscriberAlertHeader((object) grouping.Key)).AppendLine();
        int unhealthySubscriberCount2 = 0;
        StringBuilder stringBuilder2 = new StringBuilder(HostingResources.MessageBusSubscriberAlertHeader((object) grouping.Key)).AppendLine();
        NamespaceManager namespaceManager;
        if (!dictionary.TryGetValue(str1, out namespaceManager))
        {
          NamespaceManagerSettings namespaceManagerSettings;
          try
          {
            namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, managementSettings, str1);
          }
          catch (MessageBusConfigurationException ex)
          {
            requestContext.TraceException(105403, "ServiceBus", nameof (ServiceBusMonitoring), (Exception) ex);
            continue;
          }
          namespaceManager = namespaceManagerSettings.GetNamespaceManager();
          dictionary.Add(str1, namespaceManager);
        }
        TopicDescription topic = namespaceManager.GetTopic(str2);
        double num = (1.0 - (double) topic.SizeInBytes / 1024.0 / 1024.0 / (double) topic.MaxSizeInMegabytes) * 100.0;
        IKpiService service = requestContext.GetService<IKpiService>();
        service.EnsureKpiIsRegistered(requestContext, "Microsoft.VisualStudio.ServiceBusMetrics", "PercentageFreeSpaceServiceBusTopic", str2, "Percentage of free space in the ServiceBus Topic");
        service.Publish(requestContext, "Microsoft.VisualStudio.ServiceBusMetrics", str2, "PercentageFreeSpaceServiceBusTopic", num);
        foreach (ServiceBusMonitoring.SubscriberInfo subscriberInfo in (IEnumerable<ServiceBusMonitoring.SubscriberInfo>) grouping)
        {
          requestContext.Trace(105404, TraceLevel.Info, "ServiceBus", nameof (ServiceBusMonitoring), "Checking Namespace: {0}, Topic: {1}, Subscriber: {2}", (object) subscriberInfo.Namespace, (object) subscriberInfo.TopicName, (object) subscriberInfo.SubscriptionName);
          SubscriptionDescription subscriptionDescription;
          using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
          {
            try
            {
              using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
                subscriptionDescription = namespaceManager.GetSubscription(subscriberInfo.TopicName, subscriberInfo.SubscriptionName);
            }
            catch (MessagingException ex)
            {
              if (ex is MessagingEntityNotFoundException && subscriberInfo.IsTransient)
              {
                subscriptionDescription = (SubscriptionDescription) null;
              }
              else
              {
                requestContext.TraceException(13874087, "ServiceBus", nameof (ServiceBusMonitoring), (Exception) ex);
                continue;
              }
            }
          }
          SubscriberSettings subscriberAlertSetting = subscriberAlertSettings[subscriberInfo.PathTopicName];
          if (subscriberInfo.IsTransient && (subscriptionDescription == null || subscriptionDescription.AccessedAt < DateTime.UtcNow.Subtract(TimeSpan.FromSeconds((double) Math.Abs(subscriberAlertSetting.SubscriberIdleAlertInSeconds)))))
          {
            MessageBusSubscriptionInfo subscription = new MessageBusSubscriptionInfo()
            {
              MessageBusName = subscriberInfo.PathTopicName,
              SubscriptionName = subscriberInfo.SubscriptionName
            };
            try
            {
              requestContext.GetService<IMessageBusManagementService>().DeleteSubscriber(requestContext, subscription);
              requestContext.Trace(105407, TraceLevel.Info, "ServiceBus", nameof (ServiceBusMonitoring), "Deleted transient subscription {0}/{1} on namespace: {2} because the idle period lapsed", (object) subscriberInfo.PathTopicName, (object) subscriberInfo.SubscriptionName, (object) subscriberInfo.Namespace);
            }
            catch (Exception ex)
            {
              requestContext.Trace(105408, TraceLevel.Error, "ServiceBus", nameof (ServiceBusMonitoring), "Failed to delete expired transient subscription {0}/{1} on namespace {2}: {3}", (object) subscriberInfo.PathTopicName, (object) subscriberInfo.SubscriptionName, (object) subscriberInfo.Namespace, (object) ex.ToReadableStackTrace());
            }
          }
          else
          {
            stringBuilder1.AppendLine().AppendLine(ServiceBusMonitoring.CheckSubscriberThresholds(subscriptionDescription.Name, subscriptionDescription.AccessedAt, subscriptionDescription.MessageCountDetails.ActiveMessageCount, subscriberAlertSetting, ref unhealthySubscriberCount1));
            if (subscriptionDescription.MessageCountDetails.DeadLetterMessageCount > 0L)
            {
              ++unhealthySubscriberCount2;
              stringBuilder2.AppendLine(string.Format("Name: {0}, DeadLetterCount: {1}", (object) subscriptionDescription.Name, (object) subscriptionDescription.MessageCountDetails.DeadLetterMessageCount));
            }
          }
        }
        if (stringBuilder1.Length > 0)
        {
          ServiceBusMonitoring.LogSubscriberStateAlert(requestContext, stringBuilder1.ToString(), unhealthySubscriberCount1, str2);
          ServiceBusMonitoring.LogSubscriberDeadLetterStateAlert(requestContext, stringBuilder1.ToString(), unhealthySubscriberCount2, str2);
        }
      }
    }

    internal static string CheckSubscriberThresholds(
      string subscriptionName,
      DateTime accessedAt,
      long activeMessageCount,
      SubscriberSettings subscriberSettings,
      ref int unhealthySubscriberCount)
    {
      if (activeMessageCount <= 0L || !(accessedAt < DateTime.UtcNow.Subtract(TimeSpan.FromSeconds((double) Math.Abs(subscriberSettings.SubscriberIdleAlertInSeconds)))) && activeMessageCount <= (long) Math.Abs(subscriberSettings.UnprocessedMessageCount))
        return HostingResources.MessageBusSubscriberAlert((object) subscriptionName, (object) accessedAt, (object) activeMessageCount, (object) "Good");
      ++unhealthySubscriberCount;
      return HostingResources.MessageBusSubscriberAlert((object) subscriptionName, (object) accessedAt, (object) activeMessageCount, (object) "Error");
    }

    private static void LogSubscriberStateAlert(
      IVssRequestContext requestContext,
      string message,
      int unhealthySubscriberCount,
      string topicName)
    {
      IKpiService service = requestContext.GetService<IKpiService>();
      service.EnsureKpiIsRegistered(requestContext, "Microsoft.VisualStudio.ServiceBusMetrics", "UnhealthyServiceBusSubscribers", topicName, "Idle service bus subscribers not processing outstanding messages for a given topic.");
      service.Publish(requestContext, "Microsoft.VisualStudio.ServiceBusMetrics", topicName, "UnhealthyServiceBusSubscribers", (double) unhealthySubscriberCount);
      requestContext.Trace(1005320, unhealthySubscriberCount > 0 ? TraceLevel.Error : TraceLevel.Info, "ServiceBus", nameof (ServiceBusMonitoring), message);
    }

    private static void LogSubscriberDeadLetterStateAlert(
      IVssRequestContext requestContext,
      string message,
      int unhealthySubscriberCount,
      string topicName)
    {
      IKpiService service = requestContext.GetService<IKpiService>();
      service.EnsureKpiIsRegistered(requestContext, "Microsoft.VisualStudio.ServiceBusMetrics", "UnhealthyDeadLetterServiceBusSubscribers", topicName, "Service bus subscribers with message in a dead letter queue for a given topic.");
      service.Publish(requestContext, "Microsoft.VisualStudio.ServiceBusMetrics", topicName, "UnhealthyDeadLetterServiceBusSubscribers", (double) unhealthySubscriberCount);
      requestContext.Trace(1005321, unhealthySubscriberCount > 0 ? TraceLevel.Error : TraceLevel.Info, "ServiceBus", nameof (ServiceBusMonitoring), message);
    }

    internal class SubscriberInfo
    {
      public bool IsTransient;
      public string PathTopicName;
      public string Namespace;
      public string TopicName;
      public string SubscriptionName;
    }
  }
}
