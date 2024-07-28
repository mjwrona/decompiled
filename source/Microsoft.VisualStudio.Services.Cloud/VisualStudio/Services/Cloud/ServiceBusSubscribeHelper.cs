// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusSubscribeHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusSubscribeHelper
  {
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusManagementService";
    private const int c_delayCoefficient = 500;
    private const int c_maxAttempts = 4;
    private const int c_maxSubscriberCount = 1981;
    private static readonly Regex m_invalidServiceBusSubscriptionChars = new Regex("[^\\d\\w.-]");
    private static readonly TimeSpan s_minDelay = TimeSpan.FromSeconds(10.0);

    internal static SubscriptionDescription CreateSubscriptionDescription(
      IVssRequestContext requestContext,
      string namespaceName,
      string messageBusName,
      string topicName,
      string internalSubscriptionName)
    {
      return new SubscriptionDescription(topicName, internalSubscriptionName)
      {
        AutoDeleteOnIdle = ServiceBusSettingsHelper.GetAutoDeleteOnIdleForSubscription(requestContext, messageBusName, namespaceName),
        DefaultMessageTimeToLive = ServiceBusSettingsHelper.GetMessageTimeToLive(requestContext, messageBusName, namespaceName),
        EnableDeadLetteringOnMessageExpiration = ServiceBusSettingsHelper.GetEnableDeadLetteringOnMessageExpiration(requestContext, messageBusName),
        EnableDeadLetteringOnFilterEvaluationExceptions = ServiceBusSettingsHelper.GetEnableDeadLetteringOnFilterEvaluationExceptions(requestContext, messageBusName)
      };
    }

    internal static string GetFilterExpressionForTest(
      string messageBusName,
      string filterExpression)
    {
      string str = "(NOT EXISTS(TopicName) OR TopicName = '" + messageBusName + "')";
      filterExpression = !string.IsNullOrEmpty(filterExpression) ? str + " AND (" + filterExpression + ")" : str;
      return filterExpression;
    }

    internal static string GetInternalSubscriptionName(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscriptionInfo,
      string subscriptionName,
      bool prefixMachineName)
    {
      requestContext.GetService<IVssRegistryService>();
      string subscriptionName1 = subscriptionName;
      if (prefixMachineName)
        subscriptionName1 = ServiceBusSubscribeHelper.GetInternalSubscriptionNameForTest(requestContext, subscriptionInfo);
      return subscriptionName1;
    }

    internal static string GetInternalSubscriptionNameForTest(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscriptionInfo)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string internalSubscriptionName = subscriptionInfo.SubscriptionName + "-" + subscriptionInfo.MessageBusName;
      if (internalSubscriptionName.Length > 50)
      {
        byte[] hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(internalSubscriptionName));
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte num in hash)
          stringBuilder.Append(num.ToString("X2"));
        internalSubscriptionName = stringBuilder.ToString().Substring(0, 50);
        string subscriberRoot = ServiceBusSettingsHelper.GetSubscriberRegistryRoot(subscriptionInfo);
        RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Subscriber/...").Where<RegistryEntry>((Func<RegistryEntry, bool>) (_ => _.Name.Equals("SubscriptionName", StringComparison.OrdinalIgnoreCase) && _.Value.Equals(internalSubscriptionName, StringComparison.OrdinalIgnoreCase) && !_.Path.Equals(subscriberRoot + "/SubscriptionName", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<RegistryEntry>();
        if (registryEntry != null)
        {
          string message = "Could not create subscriber with name " + internalSubscriptionName + " for " + subscriptionInfo.ToString() + " since there already exists a conflicting one with " + registryEntry.Path;
          requestContext.Trace(97186104, TraceLevel.Error, "ServiceBus", "ServiceBusManagementService", message);
          throw new InvalidOperationException(message);
        }
      }
      return internalSubscriptionName;
    }

    internal static SqlFilter GetSqlFilter(
      string filterExpression,
      string messageBusName,
      bool prefixMachineName)
    {
      if (prefixMachineName)
        filterExpression = ServiceBusSubscribeHelper.GetFilterExpressionForTest(messageBusName, filterExpression);
      SqlFilter sqlFilter = (SqlFilter) null;
      if (!string.IsNullOrEmpty(filterExpression))
        sqlFilter = new SqlFilter(filterExpression);
      return sqlFilter;
    }

    internal static string GetSubscriberNameForScaleUnit(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      requestContext.CheckDeploymentRequestContext();
      return ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName).PrefixMachineName ? new Uri(requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.AzureInstanceMappingMoniker).AccessPoint).Host : requestContext.ServiceHost.InstanceId.ToString("D");
    }

    internal static string GetNextForwardedTopicNameForSubscriberWithVacancy(
      IVssRequestContext requestContext,
      NamespaceManager namespaceManager,
      string messageBusName,
      string namespaceName,
      out bool shouldRetry)
    {
      string path = messageBusName;
      int num = 1;
      bool flag = false;
      shouldRetry = false;
      for (; namespaceManager.TopicExists(messageBusName + num.ToString()); ++num)
      {
        if (namespaceManager.GetTopic(messageBusName + num.ToString()).SubscriptionCount < 1981)
        {
          flag = true;
          path = messageBusName + num.ToString();
          break;
        }
      }
      if (!flag)
      {
        if (!requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        {
          requestContext.Trace(246978297, TraceLevel.Error, "ServiceBus", "ServiceBusManagementService", "The forwarded topic {0} on namespace {1} does not have space for more subscriptions; this should be expanded manually", (object) messageBusName, (object) namespaceName);
          throw new MessageBusConfigurationException(HostingResources.ServiceBusForwardedTopicFullException((object) messageBusName));
        }
        try
        {
          path = messageBusName + num.ToString();
          namespaceManager.CreateTopic(path);
          namespaceManager.CreateSubscription(new SubscriptionDescription(messageBusName, string.Format("topic_forwarder_{0}", (object) num))
          {
            ForwardTo = path
          });
          requestContext.TraceAlways(246978295, TraceLevel.Warning, "ServiceBus", "ServiceBusManagementService", string.Format("The forwarded topic {0} on namespace {1} does not have space for more subscriptions; successfully added topic name with suffix {2}", (object) messageBusName, (object) namespaceName, (object) num));
        }
        catch (MessagingEntityAlreadyExistsException ex)
        {
          requestContext.TraceAlways(246978296, TraceLevel.Warning, "ServiceBus", "ServiceBusManagementService", "Failed to create forwarded topic " + path + " on namespace " + namespaceName + " because it already exists; it may have been created by another process");
          shouldRetry = true;
        }
      }
      return path;
    }

    internal static string GetForwardedTopicNameForSubscriber(
      IVssRequestContext requestContext,
      NamespaceManager manager,
      string namespaceName,
      string messageBusName,
      MessageBusSubscriptionInfo subscriptionInfo)
    {
      string topicName;
      if (!ServiceBusSettingsHelper.TryGetSubscriberTopicName(requestContext, subscriptionInfo, out topicName))
      {
        bool shouldRetry = true;
        while (shouldRetry)
          topicName = ServiceBusSubscribeHelper.GetNextForwardedTopicNameForSubscriberWithVacancy(requestContext, manager, messageBusName, namespaceName, out shouldRetry);
      }
      return topicName;
    }

    internal static string GetOrCreateSubscriberTopicName(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusSubscriptionInfo subscriptionInfo,
      bool prefixMachineName,
      string hostnamePrefix)
    {
      string topicName;
      if (!ServiceBusSettingsHelper.TryGetSubscriberTopicName(requestContext, subscriptionInfo, out topicName))
        topicName = ServiceBusSettingsHelper.CreateTopicName(messageBusName, prefixMachineName, hostnamePrefix);
      return topicName;
    }

    internal static bool HandleCreateSubscriberQuotaExceededException(
      IVssRequestContext requestContext,
      Microsoft.ServiceBus.Messaging.QuotaExceededException ex,
      bool isForwarded,
      ref int failedAttempts)
    {
      bool exceededException = false;
      requestContext.Trace(1444551, TraceLevel.Error, "ServiceBus", "ServiceBusManagementService", "CreateSubcriber attempt {0} failed due to max subscriptions per topic exceeded {1}", (object) failedAttempts, (object) ex);
      if (!isForwarded)
        exceededException = true;
      if (++failedAttempts > 4)
      {
        requestContext.Trace(1444552, TraceLevel.Error, "ServiceBus", "ServiceBusManagementService", "CreateSubscriber has exceeded maximum allowed attempts {0}", (object) ex);
        exceededException = true;
      }
      return exceededException;
    }

    internal static bool HandleCreateSubscriberThrottlingException(
      IVssRequestContext requestContext,
      ServerBusyException ex,
      ref int failedAttempts)
    {
      bool throttlingException = false;
      requestContext.Trace(1444549, TraceLevel.Error, "ServiceBus", "ServiceBusManagementService", "CreateSubscriber attempt {0} failed due to ServiceBus throttling {1}", (object) failedAttempts, (object) ex);
      TimeSpan delay = ServiceBusSubscribeHelper.s_minDelay + TimeSpan.FromMilliseconds(Math.Pow(2.0, (double) failedAttempts) * 500.0);
      if (++failedAttempts > 4)
      {
        requestContext.Trace(1444550, TraceLevel.Error, "ServiceBus", "ServiceBusManagementService", "CreateSubscriber has exceeded maximum allowed attempts {0}", (object) ex);
        throttlingException = true;
      }
      else
        Task.Delay(delay, requestContext.CancellationToken).ConfigureAwait(false);
      return throttlingException;
    }

    internal static void ValidateSubscriptionName(string subscriptionName)
    {
      if (subscriptionName == null || string.IsNullOrWhiteSpace(subscriptionName))
        throw new ArgumentException(HostingResources.InvalidSubscriptionNameEmptyName(), nameof (subscriptionName));
      if (ServiceBusSubscribeHelper.m_invalidServiceBusSubscriptionChars.IsMatch(subscriptionName))
        throw new ArgumentException(HostingResources.InvalidSubscriptionNameInvalidCharacters(), nameof (subscriptionName));
    }
  }
}
