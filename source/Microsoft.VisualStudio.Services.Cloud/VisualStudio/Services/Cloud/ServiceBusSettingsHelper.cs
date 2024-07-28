// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusSettingsHelper
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusSettingsHelper
  {
    public const string c_noPrefixToken = "[NOPREFIX]";
    private const string c_DefaultPublisherMapKey = "default";
    private const string c_sharedAccessKeyName = "RootManageSharedAccessKey";
    private const string c_Area = "ServiceBus";
    private const string c_Layer = "ServiceBusSettingsHelper";
    private static readonly Regex s_invalidRegistryPathChars = new Regex("[\"'\\\\:<>|#?%_\\[\\]/]");

    internal static TopicDescription CreateTopicDescription(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusPublisherCreateOptions createOptions,
      string topicName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryEntryCollection registryEntries = service.ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/...");
      int managementSetting = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<int>(registryEntries, createOptions.Namespace, "/TopicMaxSizeInGB", ServiceBusDefaultSettings.GetDefaultTopicMaxSizeInGB(requestContext, createOptions.Namespace));
      TopicDescription topicDescription = new TopicDescription(topicName)
      {
        MaxSizeInMegabytes = (long) (managementSetting * 1024)
      };
      topicDescription.EnableExpress = createOptions.EnableExpress;
      topicDescription.EnablePartitioning = !createOptions.EnablePartitioning.HasValue ? ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<bool>(registryEntries, createOptions.Namespace, "/EnablePartitioning", true) : createOptions.EnablePartitioning.Value;
      if (!service.GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.IsProductionEnvironment, true))
        topicDescription.AutoDeleteOnIdle = ServiceBusSettingsHelper.GetDefaultAutoDeleteOnIdleForTopic(requestContext, messageBusName, createOptions.Namespace);
      return topicDescription;
    }

    internal static string CreateTopicName(
      string messageBusName,
      NamespacePoolScopedSettings settings)
    {
      return ServiceBusSettingsHelper.CreateTopicName(messageBusName, settings.PrefixMachineName, settings.HostnamePrefix);
    }

    internal static string CreateTopicName(string messageBusName, NamespaceManagerSettings settings) => ServiceBusSettingsHelper.CreateTopicName(messageBusName, settings.PrefixMachineName, settings.HostnamePrefix);

    internal static string CreateTopicName(
      string messageBusName,
      bool prefixMachineName,
      string hostnamePrefix)
    {
      if (!prefixMachineName)
        return messageBusName;
      StringBuilder stringBuilder = new StringBuilder();
      if (VssStringComparer.Hostname.Contains(hostnamePrefix, FrameworkServerConstants.DevFabricLegacyDefaultDomain) || VssStringComparer.Hostname.Contains(hostnamePrefix, FrameworkServerConstants.DevFabricNewDefaultDomain))
      {
        string machineName = Environment.MachineName;
        stringBuilder.Append(machineName + "-");
      }
      if (!string.IsNullOrEmpty(hostnamePrefix) && !string.Equals(hostnamePrefix, "[NOPREFIX]"))
        stringBuilder.Append(hostnamePrefix ?? "");
      return stringBuilder.ToString();
    }

    internal static TimeSpan GetDefaultAutoDeleteOnIdleForTopic(
      IVssRequestContext requestContext,
      string messageBusName,
      string messageBusNamespace,
      Func<IVssRequestContext, string, TimeSpan> defaultIdlePeriodGetter = null)
    {
      requestContext.GetService<IVssRegistryService>();
      if (requestContext.ServiceHost.IsProduction)
        return TimeSpan.MaxValue;
      TimeSpan autoDeleteOnIdle;
      if (!ServiceBusSettingsHelper.TryGetAutoDeleteOnIdle(requestContext, "/Service/MessageBus/ServiceBus/Management/IdlePeriod", messageBusName, out autoDeleteOnIdle))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        defaultIdlePeriodGetter = defaultIdlePeriodGetter ?? ServiceBusSettingsHelper.\u003C\u003EO.\u003C0\u003E__GetDefaultIdlePeriod ?? (ServiceBusSettingsHelper.\u003C\u003EO.\u003C0\u003E__GetDefaultIdlePeriod = new Func<IVssRequestContext, string, TimeSpan>(ServiceBusDefaultSettings.GetDefaultIdlePeriod));
        autoDeleteOnIdle = defaultIdlePeriodGetter(requestContext, messageBusNamespace);
        requestContext.Trace(1005254, TraceLevel.Info, "ServiceBus", nameof (ServiceBusSettingsHelper), "Message bus: '{0}'. No registry setting found. Fall back to default: '{1}'.", (object) messageBusName, (object) autoDeleteOnIdle);
      }
      return autoDeleteOnIdle;
    }

    internal static TimeSpan GetAutoDeleteOnIdleForSubscription(
      IVssRequestContext requestContext,
      string messageBusName,
      string messageBusNamespace)
    {
      string registryPath = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName) + "/SubscriptionIdlePeriod";
      TimeSpan autoDeleteOnIdle;
      if (!ServiceBusSettingsHelper.TryGetAutoDeleteOnIdle(requestContext, registryPath, messageBusName, out autoDeleteOnIdle) && !ServiceBusSettingsHelper.TryGetAutoDeleteOnIdle(requestContext, "/Service/MessageBus/ServiceBus/Management/SubscriptionIdlePeriod", messageBusName, out autoDeleteOnIdle))
        autoDeleteOnIdle = ServiceBusSettingsHelper.GetDefaultAutoDeleteOnIdleForSubscription(requestContext, messageBusName, messageBusNamespace);
      return autoDeleteOnIdle;
    }

    private static TimeSpan GetDefaultAutoDeleteOnIdleForSubscription(
      IVssRequestContext requestContext,
      string messageBusName,
      string messageBusNamespace)
    {
      TimeSpan autoDeleteOnIdle;
      if (!ServiceBusSettingsHelper.TryGetAutoDeleteOnIdle(requestContext, "/Service/MessageBus/ServiceBus/Management/IdlePeriod", messageBusName, out autoDeleteOnIdle))
      {
        autoDeleteOnIdle = ServiceBusDefaultSettings.GetDefaultSubscriptionIdlePeriod(requestContext, messageBusNamespace);
        requestContext.Trace(1005254, TraceLevel.Info, "ServiceBus", nameof (ServiceBusSettingsHelper), "Message bus: '{0}'. No registry setting found. Fall back to default: '{1}'.", (object) messageBusName, (object) autoDeleteOnIdle);
      }
      return autoDeleteOnIdle;
    }

    private static bool TryGetAutoDeleteOnIdle(
      IVssRequestContext requestContext,
      string registryPath,
      string messageBusName,
      out TimeSpan autoDeleteOnIdle)
    {
      double? nullable = requestContext.GetService<IVssRegistryService>().GetValue<double?>(requestContext, (RegistryQuery) registryPath, new double?());
      if (nullable.HasValue && 0.0 <= nullable.Value)
      {
        if (nullable.Value <= TimeSpan.MaxValue.TotalMinutes)
        {
          try
          {
            autoDeleteOnIdle = TimeSpan.FromMinutes(nullable.Value);
          }
          catch (OverflowException ex)
          {
            autoDeleteOnIdle = TimeSpan.MaxValue;
          }
          requestContext.Trace(1005256, TraceLevel.Info, "ServiceBus", nameof (ServiceBusSettingsHelper), "Message bus: '{0}'. Path: '{1}'. Value: '{2}'. TimeSpan: '{3}'.", (object) messageBusName, (object) registryPath, (object) nullable.Value, (object) autoDeleteOnIdle);
          return true;
        }
      }
      autoDeleteOnIdle = TimeSpan.Zero;
      return false;
    }

    internal static void SetAutoDeleteOnIdleForSubscription(
      IVssRequestContext requestContext,
      string messageBusName,
      int autoDeleteOnIdleInMinutes)
    {
      string path = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName) + "/SubscriptionIdlePeriod";
      requestContext.GetService<IVssRegistryService>().SetValue<int>(requestContext, path, autoDeleteOnIdleInMinutes);
    }

    internal static TimeSpan GetMessageTimeToLive(
      IVssRequestContext requestContext,
      string messageBusName,
      string messageBusNamespace)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string query = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName) + "/SubscriptionMessageTimeToLiveMinutes";
      double? nullable = service.GetValue<double?>(requestContext, (RegistryQuery) query, new double?());
      if (!nullable.HasValue)
        nullable = new double?(service.GetValue<double>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/SubscriptionMessageTimeToLiveMinutes", ServiceBusDefaultSettings.GetDefaultMessageTimeToLive(requestContext, messageBusNamespace)));
      return TimeSpan.FromMinutes(nullable.Value);
    }

    internal static void SetMessageTimeToLiveForSubscription(
      IVssRequestContext requestContext,
      string messageBusName,
      int messageTimeToLiveInMinutes)
    {
      string path = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName) + "/SubscriptionMessageTimeToLiveMinutes";
      requestContext.GetService<IVssRegistryService>().SetValue<int>(requestContext, path, messageTimeToLiveInMinutes);
    }

    internal static bool GetEnableDeadLetteringOnMessageExpiration(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string query = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName) + "/SubscriptionDeadLetteringOnMessageExpiration";
      bool? nullable = service.GetValue<bool?>(requestContext, (RegistryQuery) query, new bool?());
      if (!nullable.HasValue)
        nullable = new bool?(service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/SubscriptionDeadLetteringOnMessageExpiration", false));
      return nullable.Value;
    }

    internal static void SetEnableDeadLetteringOnMessageExpirationForSubscription(
      IVssRequestContext requestContext,
      string messageBusName,
      bool enableDeadLetteringOnMessageExpiration)
    {
      string path = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName) + "/SubscriptionDeadLetteringOnMessageExpiration";
      requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, path, enableDeadLetteringOnMessageExpiration);
    }

    internal static bool GetEnableDeadLetteringOnFilterEvaluationExceptions(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string query = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName) + "/SubscriptionDeadLetteringOnFilterEvaluationExceptions";
      bool? nullable = service.GetValue<bool?>(requestContext, (RegistryQuery) query, new bool?());
      if (!nullable.HasValue)
        nullable = new bool?(service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/SubscriptionDeadLetteringOnFilterEvaluationExceptions", false));
      return nullable.Value;
    }

    internal static void SetEnableDeadLetteringOnFilterEvaluationExceptionsForSubscription(
      IVssRequestContext requestContext,
      string messageBusName,
      bool enableDeadLetteringOnFilterEvaluationExceptions)
    {
      string path = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName) + "/SubscriptionDeadLetteringOnFilterEvaluationExceptions";
      requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, path, enableDeadLetteringOnFilterEvaluationExceptions);
    }

    internal static string GetMessageBusNamespace(
      RegistryEntryCollection registryEntries,
      string messageBusName)
    {
      string messageBusNamespace = registryEntries.GetValueFromPath<string>("/Service/MessageBus/ServiceBus/Publisher/" + messageBusName + "/Namespace", string.Empty);
      if (string.IsNullOrEmpty(messageBusNamespace))
      {
        messageBusNamespace = registryEntries.GetValueFromPath<string>("/Service/MessageBus/ServiceBus/Management/Topics/" + messageBusName + "/Namespace", string.Empty);
        if (string.IsNullOrEmpty(messageBusNamespace))
          messageBusNamespace = ServiceBusSettingsHelper.GetDefaultNamespace(registryEntries);
      }
      return messageBusNamespace;
    }

    internal static MessageBusCredentials GetMessageBusCredentials(
      IVssRequestContext requestContext,
      string sharedAccessKeySettingName)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, FrameworkServerConstants.ConfigurationSecretsDrawerName, sharedAccessKeySettingName, false);
      if (itemInfo == null)
        throw new MessageBusConfigurationException(HostingResources.ServiceBusManagementCredentialsNotFound());
      return new MessageBusCredentials(!string.IsNullOrEmpty(itemInfo.CredentialName) ? itemInfo.CredentialName : "RootManageSharedAccessKey", service.GetString(requestContext, itemInfo));
    }

    internal static NamespaceManagerSettings GetNamespaceManagerSettings(
      IVssRequestContext requestContext,
      string messageBusName,
      string ns = null)
    {
      RegistryEntryCollection registryEntries = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/...");
      if (string.IsNullOrEmpty(ns))
        ns = ServiceBusSettingsHelper.GetMessageBusNamespace(registryEntries, messageBusName);
      return ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, registryEntries, ns);
    }

    internal static NamespaceManagerSettings GetNamespaceManagerSettings(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries,
      string ns)
    {
      if (string.IsNullOrEmpty(ns))
        throw new MessageBusConfigurationException(HostingResources.ServiceBusNamespaceNotRegistered((object) ns));
      bool managementSetting1 = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<bool>(registryEntries, ns, "/PrefixComputerName", ServiceBusDefaultSettings.GetDefaultPrefixComputerName(requestContext, ns));
      string managementSetting2 = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<string>(registryEntries, ns, "/HostNamePrefix", string.Empty);
      string managementSetting3 = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<string>(registryEntries, ns, "/SharedAccessKeySettingName", string.Empty);
      MessageBusCredentials messageBusCredentials = ServiceBusSettingsHelper.GetMessageBusCredentials(requestContext, managementSetting3);
      return new NamespaceManagerSettings(ns, messageBusCredentials, managementSetting1, managementSetting2);
    }

    internal static NamespacePoolScopedSettings GetNamespacePoolScopedSettings(
      IVssRequestContext requestContext,
      string ns,
      string messageBusName)
    {
      NamespacePoolScopedSettings poolScopedSettings = new NamespacePoolScopedSettings();
      RegistryEntryCollection registryEntries = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/...");
      poolScopedSettings.PrefixMachineName = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<bool>(registryEntries, ns, "/PrefixComputerName", ServiceBusDefaultSettings.GetDefaultPrefixComputerName(requestContext, ns));
      poolScopedSettings.HostnamePrefix = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<string>(registryEntries, ns, "/HostNamePrefix", string.Empty);
      poolScopedSettings.TopicMaxSize = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<int>(registryEntries, ns, "/TopicMaxSizeInGB", ServiceBusDefaultSettings.GetDefaultTopicMaxSizeInGB(requestContext, ns));
      poolScopedSettings.EnablePartitioning = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<bool>(registryEntries, ns, "/EnablePartitioning", true);
      poolScopedSettings.AutoDeleteOnIdle = ServiceBusSettingsHelper.GetDefaultAutoDeleteOnIdleForTopic(requestContext, messageBusName, ns);
      return poolScopedSettings;
    }

    internal static T GetNamespaceScopedManagementSetting<T>(
      RegistryEntryCollection registryEntries,
      string ns,
      string settingRelativePath,
      T defaultValue)
    {
      string path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/MessageBus/ServiceBus/Management/Namespaces/{0}", (object) ns) + settingRelativePath;
      return registryEntries.ContainsPath(path) ? registryEntries.GetValueFromPath<T>(path, defaultValue) : registryEntries.GetValueFromPath<T>("/Service/MessageBus/ServiceBus/Management" + settingRelativePath, defaultValue);
    }

    internal static bool TryGetNamespacePairFromMap(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      string region,
      string publisherMapString,
      out NamespacePair pair)
    {
      pair = (NamespacePair) null;
      Dictionary<string, NamespacePair> dictionary;
      try
      {
        dictionary = JsonUtilities.Deserialize<Dictionary<string, NamespacePair>>(publisherMapString, true);
      }
      catch
      {
        servicingContext?.LogInfo("Skipping step because PublisherMap could not be deserialized.");
        requestContext.TraceAlways(1005205, TraceLevel.Error, "ServiceBus", nameof (ServiceBusSettingsHelper), "Default Publisher Map could not be deserialized " + publisherMapString);
        return false;
      }
      if (dictionary.TryGetValue(region, out pair))
      {
        servicingContext?.LogInfo("PublisherMap contained " + region + " with Namespaces " + pair.Primary + " and " + pair.Secondary + ".");
        requestContext.TraceAlways(1005206, TraceLevel.Info, "ServiceBus", nameof (ServiceBusSettingsHelper), "PublisherMap contained " + region + " with Namespaces " + pair.Primary + " and " + pair.Secondary + ".");
      }
      else if (dictionary.TryGetValue("default", out pair))
      {
        servicingContext?.LogInfo("PublisherMap contained default with Namespaces " + pair.Primary + " and " + pair.Secondary + ".");
        requestContext.TraceAlways(1005203, TraceLevel.Info, "ServiceBus", nameof (ServiceBusSettingsHelper), "Default Publisher Map used for Region " + region);
      }
      else
      {
        servicingContext?.LogInfo("PublisherMap did not contain region or defaults!  Skipping registration.");
        requestContext.TraceAlways(1005204, TraceLevel.Error, "ServiceBus", nameof (ServiceBusSettingsHelper), "No NamespacePair was found found for " + region);
        return false;
      }
      return true;
    }

    internal static bool TryGetSubscriptionRegistryEntries(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      string ns,
      out MessageReceiverSettings receiverSettings)
    {
      try
      {
        requestContext.TraceEnter(73186957, "ServiceBus", nameof (ServiceBusSettingsHelper), nameof (TryGetSubscriptionRegistryEntries));
        receiverSettings = (MessageReceiverSettings) null;
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        string subscriberRegistryRoot = ServiceBusSettingsHelper.GetSubscriberRegistryRoot(subscription);
        IVssRequestContext requestContext1 = requestContext;
        // ISSUE: explicit reference operation
        ref RegistryQuery local = @(RegistryQuery) (subscriberRegistryRoot + "/*");
        RegistryEntryCollection registryEntryCollection = service.ReadEntriesFallThru(requestContext1, in local);
        if (string.IsNullOrEmpty(ns))
          ns = registryEntryCollection.GetValueFromPath<string>(subscriberRegistryRoot + "/Namespace", (string) null);
        MessageBusCredentials messageBusCredentials = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, subscription.MessageBusName, ns).MessageBusCredentials;
        string valueFromPath1 = registryEntryCollection.GetValueFromPath<string>(subscriberRegistryRoot + "/TopicName", string.Empty);
        string valueFromPath2 = registryEntryCollection.GetValueFromPath<string>(subscriberRegistryRoot + "/SubscriptionName", string.Empty);
        if ((string.IsNullOrEmpty(ns) || !messageBusCredentials.HasValue || string.IsNullOrEmpty(valueFromPath1) ? 0 : (!string.IsNullOrEmpty(valueFromPath2) ? 1 : 0)) == 0)
          return false;
        receiverSettings = ServiceBusSettingsHelper.GetMessageReceiverSettings(requestContext, subscription, ns, messageBusCredentials, valueFromPath1, valueFromPath2);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(67048443, "ServiceBus", nameof (ServiceBusSettingsHelper), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(68667243, "ServiceBus", nameof (ServiceBusSettingsHelper), nameof (TryGetSubscriptionRegistryEntries));
      }
    }

    internal static void DeletePrefixComputerNameRegistry(
      IVssRequestContext requestContext,
      string namespaceName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string namespaceRootPath = ServiceBusSettingsHelper.GetServiceBusNamespaceRootPath(requestContext, namespaceName);
      IVssRequestContext requestContext1 = requestContext;
      string registryPathPattern = namespaceRootPath + "/PrefixComputerName";
      service.DeleteEntries(requestContext1, registryPathPattern);
    }

    internal static void DeletePublisherSettings(IVssRequestContext requestContext, string keyName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string[] strArray = new string[3]
      {
        keyName + "/Namespace",
        keyName + "/TopicName",
        keyName + "/SubscriptionIdlePeriod"
      };
      service.DeleteEntries(requestContext, strArray);
    }

    internal static void DeleteSubscriberSettings(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string subscriberRegistryRoot = ServiceBusSettingsHelper.GetSubscriberRegistryRoot(subscription);
      string[] strArray = new string[4]
      {
        subscriberRegistryRoot + "/Namespace",
        subscriberRegistryRoot + "/TopicName",
        subscriberRegistryRoot + "/SubscriptionName",
        subscriberRegistryRoot + "/IsTransient"
      };
      service.DeleteEntries(requestContext, strArray);
    }

    internal static string GetDefaultNamespace(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/Namespace", string.Empty);

    internal static string GetDefaultNamespace(RegistryEntryCollection registryEntries) => registryEntries.GetValueFromPath<string>("/Service/MessageBus/ServiceBus/Management/Namespace", string.Empty);

    internal static string GetNamespaceManagerSettings(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/Namespace", string.Empty);

    internal static string GetNamespaceManagerSettings(RegistryEntryCollection registryEntries) => registryEntries.GetValueFromPath<string>("/Service/MessageBus/ServiceBus/Management/Namespace", string.Empty);

    internal static string GetNamespaceName(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      return ServiceBusSettingsHelper.GetMessageBusNamespace(requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/..."), messageBusName);
    }

    internal static string GetPublisherRegistryRoot(string messageBusName) => "/Service/MessageBus/ServiceBus/Publisher/" + messageBusName;

    internal static bool GetPublishSecondaryValue(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) (publisherRegistryRoot + "/PublishSecondary");
      return service.GetValue<bool>(requestContext1, in local);
    }

    internal static MessageReceiverSettings GetMessageReceiverSettings(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      string ns,
      MessageBusCredentials messageBusCredentials,
      string topic,
      string subscriber)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      MessageReceiverSettings receiverSettings = new MessageReceiverSettings();
      receiverSettings.Namespace = ns;
      receiverSettings.Uri = ServiceBusEnvironment.CreateServiceUri("sb", ns, string.Empty);
      receiverSettings.Credentials = messageBusCredentials;
      string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(subscription.MessageBusName);
      receiverSettings.PrefetchCount = service.GetValue<int>(requestContext, (RegistryQuery) (publisherRegistryRoot + "/SubscriptionPrefetchCount"), 10);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/Settings", (object) "/Service/MessageBus/ServiceBus/Subscriber", (object) subscription.MessageBusName);
      RegistryEntryCollection registryEntryCollection = service.ReadEntriesFallThru(requestContext, (RegistryQuery) (str + "/*"));
      receiverSettings.BatchFlushInterval = registryEntryCollection.GetValueFromPath<int>(str + "/BatchFlushInterval", -1);
      receiverSettings.PrefetchCount = registryEntryCollection.GetValueFromPath<int>(str + "/PrefetchCount", receiverSettings.PrefetchCount);
      receiverSettings.TraceDelay = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Subscriber/TraceDelay", 10);
      receiverSettings.DeadLetterCleanupBatchSize = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Subscriber/ServiceBusDeadLetterCleanupBatchSize", 100);
      if (ServiceBusSettingsHelper.HasNonDefaultSubscriberFilter(requestContext, subscription.MessageBusName))
      {
        receiverSettings.FilterKey = string.Empty;
        receiverSettings.FilterValue = string.Empty;
      }
      else
      {
        receiverSettings.FilterKey = ServiceBusPropertyHelper.GetServiceInstanceProperty(requestContext.ServiceInstanceType());
        receiverSettings.FilterValue = ServiceBusPropertyHelper.GetServiceInstanceValue(requestContext.ServiceHost.InstanceId);
      }
      receiverSettings.TransportType = service.GetValue<TransportType>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/TransportType", TransportType.NetMessaging);
      receiverSettings.TopicName = topic;
      receiverSettings.SubscriberName = subscriber;
      return receiverSettings;
    }

    private static string GetServiceBusNamespaceRootPath(
      IVssRequestContext requestContext,
      string namespaceName)
    {
      return string.IsNullOrEmpty(namespaceName) || ServiceBusSettingsHelper.IsDefaultNamespace(requestContext, namespaceName) ? "/Service/MessageBus/ServiceBus/Management" : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/MessageBus/ServiceBus/Management/Namespaces/{0}", (object) namespaceName);
    }

    internal static string GetServiceBusSharedAccessKeySettingName(
      IVssRequestContext deploymentContext,
      string namespaceName)
    {
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      string namespaceRootPath = ServiceBusSettingsHelper.GetServiceBusNamespaceRootPath(deploymentContext, namespaceName);
      IVssRequestContext requestContext = deploymentContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) (namespaceRootPath + "/SharedAccessKeySettingName");
      return service.GetValue(requestContext, in local, (string) null);
    }

    internal static string GetSubscriberFilter(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      Guid serviceOwner = requestContext.ServiceInstanceType();
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "NOT EXISTS({0}) OR {0} LIKE '%{1}%'", (object) ServiceBusPropertyHelper.GetServiceInstanceProperty(serviceOwner), (object) ServiceBusPropertyHelper.GetServiceInstanceValue(instanceId));
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) ("/Service/MessageBus/ServiceBus/Management/Topics/" + messageBusName + "/SubscriptionFilter");
      string defaultValue = str;
      return service.GetValue<string>(requestContext1, in local, defaultValue);
    }

    internal static string GetSubscriberNamespace(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string subscriberRegistryRoot = ServiceBusSettingsHelper.GetSubscriberRegistryRoot(subscription);
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) (subscriberRegistryRoot + "/Namespace");
      return service.GetValue<string>(requestContext1, in local, (string) null);
    }

    internal static string GetSubscriberRegistryRoot(
      MessageBusSubscriptionInfo subscription,
      bool includeSubscription = true)
    {
      return ServiceBusSettingsHelper.GetSubscriberRegistryRoot(subscription.SubscriptionName, subscription.MessageBusName, includeSubscription);
    }

    internal static string GetSubscriberRegistryRoot(
      string subscriptionName,
      string messageBusName,
      bool includeSubscription = true)
    {
      string str = ServiceBusSettingsHelper.s_invalidRegistryPathChars.Replace(subscriptionName, "-");
      return !includeSubscription ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) "/Service/MessageBus/ServiceBus/Subscriber", (object) messageBusName) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) "/Service/MessageBus/ServiceBus/Subscriber", (object) messageBusName, (object) str);
    }

    internal static string GetTopicNameFromPath(string path) => path.Substring("/Service/MessageBus/ServiceBus/Subscriber".Length + 1).Split('/')[0];

    internal static string GetTopicRegistryRoot(string topicName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/MessageBus/ServiceBus/Management/Topics/{0}", (object) topicName);

    internal static bool HasNonDefaultSubscriberFilter(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      return !string.IsNullOrEmpty(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) ("/Service/MessageBus/ServiceBus/Management/Topics/" + messageBusName + "/SubscriptionFilter"), string.Empty));
    }

    internal static bool IsDefaultNamespace(IVssRequestContext requestContext, string namespaceName)
    {
      if (string.IsNullOrEmpty(namespaceName))
        return true;
      string defaultNamespace = ServiceBusSettingsHelper.GetDefaultNamespace(requestContext);
      return namespaceName.Equals(defaultNamespace, StringComparison.OrdinalIgnoreCase);
    }

    internal static bool IsForwardedTopic(IVssRequestContext requestContext, string messageBusName) => requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) ("/Service/MessageBus/ServiceBus/Management/Topics/" + messageBusName + "/IsTopicForwarding"), false);

    internal static void RegisterPublisherSettings(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusPublisherCreateOptions createOptions,
      string namespaceName,
      string topicName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
      List<RegistryEntry> registryEntryList = new List<RegistryEntry>()
      {
        new RegistryEntry(publisherRegistryRoot + "/Namespace", namespaceName),
        new RegistryEntry(publisherRegistryRoot + "/TopicName", topicName)
      };
      if (createOptions.SubscriptionIdleTimeout.TotalMinutes > 0.0)
        registryEntryList.Add(new RegistryEntry(publisherRegistryRoot + "/SubscriptionIdlePeriod", createOptions.SubscriptionIdleTimeout.TotalMinutes.ToString()));
      if (createOptions.SubscriptionMessageTimeToLive.TotalMinutes > 0.0)
        registryEntryList.Add(new RegistryEntry(publisherRegistryRoot + "/SubscriptionMessageTimeToLiveMinutes", createOptions.SubscriptionMessageTimeToLive.TotalMinutes.ToString()));
      if (createOptions.SubscriptionPrefetchCount > 0)
        registryEntryList.Add(new RegistryEntry(publisherRegistryRoot + "/SubscriptionPrefetchCount", createOptions.SubscriptionPrefetchCount.ToString()));
      IVssRequestContext requestContext1 = requestContext;
      RegistryEntry[] array = registryEntryList.ToArray();
      service.WriteEntries(requestContext1, (IEnumerable<RegistryEntry>) array);
    }

    internal static void RegisterSubscriberSettings(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscriptionInfo,
      string namespaceName,
      string topicName,
      string internalSubscriptionName,
      bool isTransient)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string subscriberRegistryRoot = ServiceBusSettingsHelper.GetSubscriberRegistryRoot(subscriptionInfo);
      List<RegistryEntry> registryEntryList1 = new List<RegistryEntry>()
      {
        new RegistryEntry(subscriberRegistryRoot + "/Namespace", namespaceName),
        new RegistryEntry(subscriberRegistryRoot + "/TopicName", topicName),
        new RegistryEntry(subscriberRegistryRoot + "/SubscriptionName", internalSubscriptionName)
      };
      if (isTransient)
        registryEntryList1.Add(new RegistryEntry(subscriberRegistryRoot + "/IsTransient", isTransient.ToString()));
      IVssRequestContext requestContext1 = requestContext;
      List<RegistryEntry> registryEntryList2 = registryEntryList1;
      service.WriteEntries(requestContext1, (IEnumerable<RegistryEntry>) registryEntryList2);
    }

    internal static void SetPrefixComputerNameRegistry(
      IVssRequestContext requestContext,
      string namespaceName,
      bool prefixComputerName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string namespaceRootPath = ServiceBusSettingsHelper.GetServiceBusNamespaceRootPath(requestContext, namespaceName);
      IVssRequestContext requestContext1 = requestContext;
      string path = namespaceRootPath + "/PrefixComputerName";
      string str = prefixComputerName.ToString();
      service.SetValue<string>(requestContext1, path, str);
    }

    internal static void SetCircuitBreakerForceOpenValue(
      IVssRequestContext requestContext,
      string namespaceName,
      bool value)
    {
      requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, ServiceBusSettingsHelper.GetCircuitBreakerForceOpenRegistryKey(namespaceName), value);
    }

    internal static void UnsetCircuitBreakerForceOpenValue(
      IVssRequestContext requestContext,
      string namespaceName)
    {
      requestContext.GetService<IVssRegistryService>().DeleteEntries(requestContext, ServiceBusSettingsHelper.GetCircuitBreakerForceOpenRegistryKey(namespaceName));
    }

    internal static string GetCircuitBreakerForceOpenRegistryKey(string namespaceName) => string.Format("/Configuration/CircuitBreaker/ServiceBusNamespace.Publish.{0}/CircuitBreakerForceOpen", (object) namespaceName);

    internal static bool TryGetPrefixComputerNameRegistry(
      IVssRequestContext requestContext,
      string namespaceName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string namespaceRootPath = ServiceBusSettingsHelper.GetServiceBusNamespaceRootPath(requestContext, namespaceName);
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) (namespaceRootPath + "/PrefixComputerName");
      return service.GetValue<bool>(requestContext1, in local, false);
    }

    internal static List<string> GetSubscriptionNames(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string subscriptionNameRegEntryName = "/SubscriptionName".Substring(1);
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery query = (RegistryQuery) ("/Service/MessageBus/ServiceBus/Subscriber/" + messageBusName + "/**");
      return service.ReadEntries(requestContext1, query).Where<RegistryEntry>((Func<RegistryEntry, bool>) (re => string.Equals(re.Name, subscriptionNameRegEntryName, StringComparison.OrdinalIgnoreCase))).Select<RegistryEntry, string>((Func<RegistryEntry, string>) (re => re.Value)).ToList<string>();
    }

    internal static bool TryGetSubscriberTopicName(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      out string topicName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (string.IsNullOrEmpty(subscription.SubscriptionName))
      {
        List<string> subscriptionNames = ServiceBusSettingsHelper.GetSubscriptionNames(requestContext, subscription.MessageBusName);
        if (subscriptionNames.Count == 1)
        {
          subscription = new MessageBusSubscriptionInfo(subscription)
          {
            SubscriptionName = subscriptionNames[0]
          };
        }
        else
        {
          topicName = (string) null;
          return false;
        }
      }
      string subscriberRegistryRoot = ServiceBusSettingsHelper.GetSubscriberRegistryRoot(subscription);
      topicName = service.GetValue<string>(requestContext, (RegistryQuery) (subscriberRegistryRoot + "/TopicName"), (string) null);
      return topicName != null;
    }

    internal static bool TryGetSubscriptionNamespace(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      out string namespaceName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string subscriberRegistryRoot = ServiceBusSettingsHelper.GetSubscriberRegistryRoot(subscription);
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) (subscriberRegistryRoot + "/*");
      RegistryEntryCollection registryEntryCollection = service.ReadEntriesFallThru(requestContext1, in local);
      namespaceName = registryEntryCollection.GetValueFromPath<string>(subscriberRegistryRoot + "/Namespace", (string) null);
      return namespaceName != null;
    }

    internal static bool TryGetPublisherTopicName(
      IVssRequestContext requestContext,
      string messageBusName,
      out string topicName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
      topicName = service.GetValue<string>(requestContext, (RegistryQuery) (publisherRegistryRoot + "/TopicName"), (string) null);
      return topicName != null;
    }

    internal static void RegisterHighAvailabilityPublisher(
      IVssRequestContext requestContext,
      string namepacePoolName,
      string primaryNamespace,
      string secondaryNamespace,
      bool isGlobal)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = !isGlobal ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/MessageBus/ServiceBus/Management/Namespaces/{0}", (object) namepacePoolName) : "/Service/MessageBus/ServiceBus/Management";
      List<RegistryEntry> registryEntryList1 = new List<RegistryEntry>();
      registryEntryList1.Add(new RegistryEntry(str + "/PublisherPrimaryNamespace", primaryNamespace));
      registryEntryList1.Add(new RegistryEntry(str + "/PublisherSecondaryNamespace", secondaryNamespace));
      IVssRequestContext requestContext1 = requestContext;
      List<RegistryEntry> registryEntryList2 = registryEntryList1;
      service.WriteEntries(requestContext1, (IEnumerable<RegistryEntry>) registryEntryList2);
    }

    internal static void RegisterHighAvailabilityNamespacePool(
      IVssRequestContext requestContext,
      string namepacePoolName,
      string namespacePoolList,
      bool isGlobal)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str1 = !isGlobal ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/MessageBus/ServiceBus/Management/Namespaces/{0}", (object) namepacePoolName) : "/Service/MessageBus/ServiceBus/Management";
      IVssRequestContext requestContext1 = requestContext;
      string path = str1 + "/NamespacePool";
      string str2 = namespacePoolList;
      service.SetValue<string>(requestContext1, path, str2);
    }

    internal static void SetPublishSecondaryValue(
      IVssRequestContext requestContext,
      string messageBusName,
      bool enable)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
      if (enable)
        service.SetValue<bool>(requestContext, publisherRegistryRoot + "/PublishSecondary", enable);
      else
        service.DeleteEntries(requestContext, publisherRegistryRoot + "/PublishSecondary");
    }

    internal static void SetServiceBusSharedAccessKeySettingName(
      IVssRequestContext deploymentContext,
      CachedRegistryService registryService,
      string serviceBusSharedAccessKeySettingName)
    {
      if (string.IsNullOrEmpty(serviceBusSharedAccessKeySettingName))
        serviceBusSharedAccessKeySettingName = "ServiceBusManagementSharedAccessKeyValue";
      registryService.Write(deploymentContext, (IEnumerable<RegistryItem>) new RegistryItem[1]
      {
        new RegistryItem("/Service/MessageBus/ServiceBus/Management/SharedAccessKeySettingName", serviceBusSharedAccessKeySettingName)
      });
    }

    internal static bool TryGetNamespacePool(
      IVssRequestContext requestContext,
      string namepaceName,
      out string[] namespacePoolList)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string namespaceRootPath = ServiceBusSettingsHelper.GetServiceBusNamespaceRootPath(requestContext, namepaceName);
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) (namespaceRootPath + "/NamespacePool");
      string str = service.GetValue<string>(requestContext1, in local, (string) null);
      if (str != null)
      {
        namespacePoolList = str.Split(';');
        if (namespacePoolList.Length != 0)
          return true;
      }
      namespacePoolList = (string[]) null;
      return false;
    }

    internal static void UnregisterHighAvailabilityNamespacePool(
      IVssRequestContext requestContext,
      string namepacePoolName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string namespaceRootPath = ServiceBusSettingsHelper.GetServiceBusNamespaceRootPath(requestContext, namepacePoolName);
      IVssRequestContext requestContext1 = requestContext;
      string registryPathPattern = namespaceRootPath + "/NamespacePool";
      service.DeleteEntries(requestContext1, registryPathPattern);
    }

    internal static void UnregisterHighAvailabilityPublisher(
      IVssRequestContext requestContext,
      string namepacePoolName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string namespaceRootPath = ServiceBusSettingsHelper.GetServiceBusNamespaceRootPath(requestContext, namepacePoolName);
      string[] strArray = new string[2]
      {
        namespaceRootPath + "/PublisherPrimaryNamespace",
        namespaceRootPath + "/PublisherSecondaryNamespace"
      };
      service.DeleteEntries(requestContext, strArray);
    }

    internal static void UnsetPublishSecondaryValue(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
      IVssRequestContext requestContext1 = requestContext;
      string registryPathPattern = publisherRegistryRoot + "/PublishSecondary";
      service.DeleteEntries(requestContext1, registryPathPattern);
    }

    internal static void ValidateNamespaceName(
      IVssRequestContext requestContext,
      string namespaceName,
      string defaultNamespace)
    {
      if (namespaceName.Equals(defaultNamespace, StringComparison.OrdinalIgnoreCase))
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/MessageBus/ServiceBus/Management/Namespaces/{0}", (object) namespaceName);
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery query = (RegistryQuery) (str + "/...");
      if (service.ReadEntries(requestContext1, query).Count == 0)
        throw new MessageBusConfigurationException(HostingResources.ServiceBusNamespaceNotRegistered((object) namespaceName));
    }

    internal static bool TryGetHighAvailabilityPublishNamespaces(
      IVssRequestContext requestContext,
      string poolName,
      out string primaryNamespace,
      out string secondaryNamespace)
    {
      requestContext.GetService<IVssRegistryService>();
      primaryNamespace = ServiceBusSettingsHelper.GetHighAvailabilityPublisherNamespaceName(requestContext, poolName, "/PublisherPrimaryNamespace");
      if (primaryNamespace == null)
      {
        secondaryNamespace = (string) null;
        return false;
      }
      secondaryNamespace = ServiceBusSettingsHelper.GetHighAvailabilityPublisherNamespaceName(requestContext, poolName, "/PublisherSecondaryNamespace");
      return true;
    }

    internal static string GetHighAvailabilityPublisherNamespaceName(
      IVssRequestContext requestContext,
      string poolName,
      string relativePath)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string namespaceRootPath = ServiceBusSettingsHelper.GetServiceBusNamespaceRootPath(requestContext, poolName);
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) (namespaceRootPath + relativePath);
      return service.GetValue<string>(requestContext1, in local, (string) null);
    }
  }
}
