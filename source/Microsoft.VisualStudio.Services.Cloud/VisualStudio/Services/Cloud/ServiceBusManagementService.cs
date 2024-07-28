// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusManagementService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal sealed class ServiceBusManagementService : 
    VssBaseService,
    IMessageBusManagementService,
    IVssFrameworkService,
    IServiceBusManager
  {
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusManagementService";
    private ServiceBusConnectionManager m_ConnectionManager;
    private ServiceBusManagerHelper m_ManagerHelper;
    private ServiceBusLogger m_Logger;
    private ServiceBusLegacyManagementService m_serviceBusLegacyManagementService;
    private ServiceBusHighAvailabilityManagementService m_serviceBusHighAvailabilityManagementService;
    private INotificationRegistration m_monitorRegistration;
    private TimeSpan m_subscriberProcessingAlertTimeThreshold;
    private VssTaskDispatcher m_publishDispatcher;
    public static readonly string s_noPrefixToken = "[NOPREFIX]";
    private static readonly Random s_random = new Random();
    internal static readonly Guid MonitorSubscribersEventClass = new Guid("{765AA663-14B2-4F86-A032-E25CC552ABFF}");

    internal ServiceBusManagementService()
    {
    }

    internal IVssTaskDispatcher PublishDispatcher => (IVssTaskDispatcher) this.m_publishDispatcher;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1005100, "ServiceBus", nameof (ServiceBusManagementService), nameof (ServiceStart));
      try
      {
        systemRequestContext.CheckDeploymentRequestContext();
        this.m_ConnectionManager = new ServiceBusConnectionManager();
        this.m_ManagerHelper = new ServiceBusManagerHelper(nameof (ServiceBusManagementService));
        this.m_Logger = new ServiceBusLogger();
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/MessageBus/ServiceBus/...");
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnServiceBusKeyChanged), FrameworkServerConstants.ConfigurationSecretsDrawerName, (IEnumerable<string>) new string[1]
        {
          "*"
        });
        this.m_monitorRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", ServiceBusManagementService.MonitorSubscribersEventClass, new SqlNotificationCallback(this.MonitorSubscriberConnections), false, false);
        this.Initialize(systemRequestContext);
        this.m_serviceBusLegacyManagementService = new ServiceBusLegacyManagementService(systemRequestContext, this, this.m_ConnectionManager, this.m_Logger, this.m_publishDispatcher);
        this.m_serviceBusHighAvailabilityManagementService = new ServiceBusHighAvailabilityManagementService(systemRequestContext, this, this.m_ConnectionManager, this.m_Logger, this.m_publishDispatcher);
      }
      finally
      {
        systemRequestContext.TraceLeave(1005120, "ServiceBus", nameof (ServiceBusManagementService), nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1005130, "ServiceBus", nameof (ServiceBusManagementService), nameof (ServiceEnd));
      try
      {
        systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnServiceBusKeyChanged));
        if (this.m_monitorRegistration != null)
          this.m_monitorRegistration.Unregister(systemRequestContext);
        this.m_ConnectionManager.ClearPublishers(systemRequestContext, true);
        this.m_ConnectionManager.ClearSubscribers(systemRequestContext);
        this.m_publishDispatcher.Stop(TimeSpan.FromSeconds(10.0));
      }
      finally
      {
        systemRequestContext.TraceLeave(1005140, "ServiceBus", nameof (ServiceBusManagementService), nameof (ServiceEnd));
      }
    }

    public void FixMessageQueueMappings(
      IVssRequestContext deploymentContext,
      string ns,
      string hostNamePrefix,
      string sharedAccessKeySettingName,
      ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentContext, nameof (deploymentContext));
      ArgumentUtility.CheckStringForNullOrEmpty(ns, "namespace");
      ArgumentUtility.CheckStringForNullOrEmpty(hostNamePrefix, nameof (hostNamePrefix));
      deploymentContext.TraceEnter(1005110, "ServiceBus", nameof (ServiceBusManagementService), nameof (FixMessageQueueMappings));
      try
      {
        if (logger == null)
          logger = (ITFLogger) new NullLogger();
        CachedRegistryService service = deploymentContext.GetService<CachedRegistryService>();
        string defaultNamespace = ServiceBusSettingsHelper.GetDefaultNamespace(deploymentContext);
        string accessKeySettingName = ServiceBusSettingsHelper.GetServiceBusSharedAccessKeySettingName(deploymentContext, defaultNamespace);
        if (!string.Equals(ns, defaultNamespace, StringComparison.OrdinalIgnoreCase))
        {
          RegistryEntryCollection registryEntryCollection = service.ReadEntries(deploymentContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/.../Namespace");
          foreach (RegistryEntry registryEntry in registryEntryCollection)
          {
            if (registryEntry.Value.Equals(defaultNamespace))
              registryEntry.Value = ns;
          }
          service.WriteEntries(deploymentContext, (IEnumerable<RegistryEntry>) registryEntryCollection);
        }
        ServiceBusSettingsHelper.SetServiceBusSharedAccessKeySettingName(deploymentContext, service, sharedAccessKeySettingName);
        string secondaryNamespace;
        if (!defaultNamespace.IsNullOrEmpty<char>() && !accessKeySettingName.IsNullOrEmpty<char>() && this.IsHighAvailabilityPublisherAvailable(deploymentContext, ns, nameof (FixMessageQueueMappings), out string _, out secondaryNamespace) && secondaryNamespace == defaultNamespace)
        {
          logger.Info("Registering secondary namespace: {0}, {1}", (object) secondaryNamespace, (object) accessKeySettingName);
          ((IServiceBusManager) this).RegisterNamespace(deploymentContext, secondaryNamespace, accessKeySettingName, ServiceBusDefaultSettings.GetDefaultTopicMaxSizeInGB(deploymentContext, secondaryNamespace), hostNamePrefix, ServiceBusDefaultSettings.GetDefaultPrefixComputerName(deploymentContext, secondaryNamespace), false, logger);
        }
        RegistryEntry registryEntry1 = new RegistryEntry("/Service/MessageBus/ServiceBus/Management/HostNamePrefix", hostNamePrefix);
        service.WriteEntries(deploymentContext, (IEnumerable<RegistryEntry>) new RegistryEntry[1]
        {
          registryEntry1
        });
        foreach (RegistryEntry readEntry in service.ReadEntries(deploymentContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/*", (object) "/Service/MessageBus/ServiceBus/Publisher"), true))
        {
          if (!readEntry.Path.Equals("/Service/MessageBus/ServiceBus/Publisher", StringComparison.InvariantCultureIgnoreCase))
          {
            logger.Info("Updating Publisher: {0}", (object) readEntry.Path);
            string query = RegistryHelpers.CombinePath(readEntry.Path, "/SubscriptionIdlePeriod");
            double subscriberDeleteOnIdleMinutes = service.GetValue<double>(deploymentContext, (RegistryQuery) query, 0.0);
            logger.Info("Deleting old registry entries for the publisher: {0}.", (object) readEntry.Name);
            service.DeleteEntries(deploymentContext, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/**", (object) readEntry.Path));
            try
            {
              logger.Info("Creating new publisher: {0}", (object) readEntry.Name);
              this.CreatePublisher(deploymentContext, readEntry.Name, false, subscriberDeleteOnIdleMinutes);
            }
            catch (Exception ex)
            {
              logger.Error(ex);
              deploymentContext.TraceException(1034520, "ServiceBus", nameof (ServiceBusManagementService), ex);
              throw;
            }
          }
        }
        foreach (RegistryEntry readEntry in service.ReadEntries(deploymentContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/**{1}", (object) "/Service/MessageBus/ServiceBus/Subscriber", (object) "/SubscriptionName")))
        {
          logger.Info("Updating Subscriber: {0}", (object) readEntry.Path);
          string str1 = readEntry.Value;
          string str2 = readEntry.Path.Substring("/Service/MessageBus/ServiceBus/Subscriber".Length + 1).Split('/')[0];
          string registryPathPattern = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/**", (object) "/Service/MessageBus/ServiceBus/Subscriber", (object) str2);
          logger.Info("Deleting old registry entries for the subscriber: {0}", (object) str2);
          service.DeleteEntries(deploymentContext, registryPathPattern);
        }
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(1005111, "ServiceBus", nameof (ServiceBusManagementService), ex);
        throw;
      }
      finally
      {
        deploymentContext.TraceLeave(1005112, "ServiceBus", nameof (ServiceBusManagementService), nameof (FixMessageQueueMappings));
      }
    }

    public void ClearPublishers(IVssRequestContext requestContext, bool dispose = false) => this.m_ConnectionManager.ClearPublishers(requestContext, dispose);

    private void HighAvailabilityPublishFeatureFlagChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.ClearPublishers(requestContext, true);
    }

    private void OnServiceBusKeyChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> changedItems)
    {
      requestContext.TraceEnter(1005351, "ServiceBus", nameof (ServiceBusManagementService), nameof (OnServiceBusKeyChanged));
      try
      {
        foreach (RegistryEntry registryEntry in requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/...").Where<RegistryEntry>((Func<RegistryEntry, bool>) (re => re.Path.EndsWith("/SharedAccessKeySettingName", StringComparison.OrdinalIgnoreCase))))
        {
          RegistryEntry re = registryEntry;
          if (changedItems.Any<StrongBoxItemName>((Func<StrongBoxItemName, bool>) (ci => StringComparer.OrdinalIgnoreCase.Equals(ci.LookupKey, re.Value))))
          {
            requestContext.Trace(1005352, TraceLevel.Verbose, "ServiceBus", nameof (ServiceBusManagementService), "OnServiceBusKeyChanged was triggered by strongbox item[DrawerId/LookupKey] {0}/{1}", (object) changedItems.First<StrongBoxItemName>().DrawerId, (object) changedItems.First<StrongBoxItemName>().LookupKey);
            this.m_ConnectionManager.ClearPublishers(requestContext);
            requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.RefreshSubscribers)));
            break;
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1005354, "ServiceBus", nameof (ServiceBusManagementService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1005355, "ServiceBus", nameof (ServiceBusManagementService), nameof (OnServiceBusKeyChanged));
      }
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(1005141, "ServiceBus", nameof (ServiceBusManagementService), nameof (OnRegistryChanged));
      try
      {
        if (!changedEntries.Any<RegistryEntry>())
          return;
        string valueFromPath1 = changedEntries.GetValueFromPath<string>("/Service/MessageBus/ServiceBus/Management/ConnectivityMode", "Tcp");
        ConnectivityMode result;
        if (!string.IsNullOrEmpty(valueFromPath1) && Enum.TryParse<ConnectivityMode>(valueFromPath1, true, out result))
          ServiceBusEnvironment.SystemConnectivity.Mode = result;
        foreach (RegistryEntry changedEntry in changedEntries)
        {
          if (changedEntry.Path.StartsWith("/Service/MessageBus/ServiceBus/Publisher", StringComparison.OrdinalIgnoreCase) || changedEntry.Path.StartsWith("/Service/MessageBus/ServiceBus/Management", StringComparison.OrdinalIgnoreCase))
          {
            this.m_ConnectionManager.ClearPublishers(requestContext);
            break;
          }
        }
        foreach (RegistryEntry changedEntry in changedEntries)
        {
          if (changedEntry.Path.StartsWith("/Service/MessageBus/ServiceBus/Subscriber", StringComparison.OrdinalIgnoreCase) || changedEntry.Path.StartsWith("/Service/MessageBus/ServiceBus/Management", StringComparison.OrdinalIgnoreCase))
          {
            requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.RefreshSubscribers)));
            break;
          }
        }
        VssTaskDispatcher publishDispatcher = this.m_publishDispatcher;
        if (publishDispatcher == null)
          return;
        int valueFromPath2 = changedEntries.GetValueFromPath<int>("/Service/MessageBus/ServiceBus/Management/PublishThreadCount", publishDispatcher.MaxThreadCount);
        int valueFromPath3 = changedEntries.GetValueFromPath<int>("/Service/MessageBus/ServiceBus/Management/PublishThreadConcurrency", publishDispatcher.MaxConcurrencyPerThread);
        if (valueFromPath2 == publishDispatcher.MaxThreadCount && valueFromPath3 == publishDispatcher.MaxConcurrencyPerThread)
          return;
        VssTaskDispatcher vssTaskDispatcher = new VssTaskDispatcher(requestContext.ServiceHost.DeploymentServiceHost, "ServiceBusPublish", valueFromPath2, valueFromPath3);
        vssTaskDispatcher.Start();
        this.m_publishDispatcher = vssTaskDispatcher;
        requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.ShutdownDispatcher), (object) publishDispatcher, 0));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1005148, "ServiceBus", nameof (ServiceBusManagementService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1005149, "ServiceBus", nameof (ServiceBusManagementService), nameof (OnRegistryChanged));
      }
    }

    private void ShutdownDispatcher(IVssRequestContext requestContext, object taskArgs) => ((IVssTaskDispatcher) taskArgs).Stop(TimeSpan.FromSeconds(10.0));

    internal void RefreshSubscribers(IVssRequestContext requestContext, object taskArgs)
    {
      foreach (KeyValuePair<MessageBusSubscriptionInfo, ServiceBusSubscribeConnection> subscriberConnection in (IEnumerable<KeyValuePair<MessageBusSubscriptionInfo, ServiceBusSubscribeConnection>>) this.m_ConnectionManager.GetReadOnlySubscriberConnections())
      {
        string ns = (string) null;
        string subscriberNamespace = ServiceBusSettingsHelper.GetSubscriberNamespace(requestContext, subscriberConnection.Key);
        if (subscriberConnection.Key.NamespacePoolName == subscriberNamespace)
          ns = subscriberConnection.Key.Namespace;
        MessageReceiverSettings receiverSettings;
        if (ServiceBusSettingsHelper.TryGetSubscriptionRegistryEntries(requestContext, subscriberConnection.Key, ns, out receiverSettings))
        {
          using (requestContext.Lock(ServiceBusLockHelper.GetSubscriberLockName(requestContext, subscriberConnection.Key)))
            subscriberConnection.Value.Refresh(receiverSettings);
        }
      }
    }

    private void Initialize(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1005101, "ServiceBus", nameof (ServiceBusManagementService), nameof (Initialize));
      try
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        IVssRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = requestContext;
        RegistryQuery registryQuery = (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/ConnectivityMode";
        ref RegistryQuery local1 = ref registryQuery;
        string str = registryService1.GetValue<string>(requestContext1, in local1, true, "Tcp");
        ConnectivityMode result;
        if (!string.IsNullOrEmpty(str) && Enum.TryParse<ConnectivityMode>(str, true, out result))
          ServiceBusEnvironment.SystemConnectivity.Mode = result;
        IVssRegistryService registryService2 = service;
        IVssRequestContext requestContext2 = requestContext;
        registryQuery = (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/PublishThreadCount";
        ref RegistryQuery local2 = ref registryQuery;
        int maximumThreadCount = registryService2.GetValue<int>(requestContext2, in local2, 8);
        IVssRegistryService registryService3 = service;
        IVssRequestContext requestContext3 = requestContext;
        registryQuery = (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/PublishThreadConcurrency";
        ref RegistryQuery local3 = ref registryQuery;
        int maximumConcurrencyPerThread = registryService3.GetValue<int>(requestContext3, in local3, 4);
        this.m_publishDispatcher = new VssTaskDispatcher(requestContext.ServiceHost.DeploymentServiceHost, "ServiceBusPublish", maximumThreadCount, maximumConcurrencyPerThread);
        this.m_publishDispatcher.Start();
        IVssRegistryService registryService4 = service;
        IVssRequestContext requestContext4 = requestContext;
        registryQuery = (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/SubscriberThresholdInSeconds";
        ref RegistryQuery local4 = ref registryQuery;
        this.m_subscriberProcessingAlertTimeThreshold = TimeSpan.FromSeconds((double) registryService4.GetValue<int>(requestContext4, in local4, 900));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1005102, "ServiceBus", nameof (ServiceBusManagementService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1005103, "ServiceBus", nameof (ServiceBusManagementService), nameof (Initialize));
      }
    }

    internal void Publish(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusMessage[] messages,
      bool throwOnMissingPublisher = true)
    {
      if (messages == null || messages.Length == 0)
        return;
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, messageBusName);
      requestContext.TraceConditionally(1005620, TraceLevel.Info, "ServiceBus", nameof (ServiceBusManagementService), (Func<string>) (() => string.Format("Sending {0} messages for bus: {1}.", (object) messages.Length, (object) messageBusName)));
      string primaryNamespace;
      string secondaryNamespace;
      if (this.IsHighAvailabilityPublisherAvailable(requestContext, namespaceName, nameof (Publish), out primaryNamespace, out secondaryNamespace))
        this.m_serviceBusHighAvailabilityManagementService.Publish(requestContext, messageBusName, messages, namespaceName, primaryNamespace, secondaryNamespace, throwOnMissingPublisher);
      else
        this.m_serviceBusLegacyManagementService.Publish(requestContext, messageBusName, messages, throwOnMissingPublisher);
    }

    private IServiceBusPublishConnection GetConnectionInfo(
      IVssRequestContext requestContext,
      string messageBusName,
      bool throwOnMissingPublisher,
      out string layer,
      out string ns)
    {
      string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
      layer = messageBusName;
      IServiceBusPublishConnection connInfo;
      if (this.m_ConnectionManager.TryGetPublishConnection(publisherRegistryRoot, out connInfo))
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
          requestContext.TraceException(1005154, "ServiceBus", nameof (ServiceBusManagementService), (Exception) notFoundException);
          throw notFoundException;
        }
        return (IServiceBusPublishConnection) null;
      }
      IServiceBusPublishConnection publishConnection = ((IServiceBusManager) this).CreateServiceBusPublishConnection(requestContext, ns, valueFromPath);
      return this.m_ConnectionManager.AddPublishConnection(requestContext, publisherRegistryRoot, publishConnection, layer);
    }

    internal async Task PublishAsync(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusMessage[] messages,
      bool throwOnMissingPublisher = true)
    {
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, messageBusName);
      string primaryNamespace;
      string secondaryNamespace;
      if (this.IsHighAvailabilityPublisherAvailable(requestContext, namespaceName, nameof (PublishAsync), out primaryNamespace, out secondaryNamespace))
        await this.m_serviceBusHighAvailabilityManagementService.PublishAsync(requestContext, messageBusName, messages, namespaceName, primaryNamespace, secondaryNamespace, throwOnMissingPublisher);
      else
        await this.m_serviceBusLegacyManagementService.PublishAsync(requestContext, messageBusName, messages, throwOnMissingPublisher);
    }

    IServiceBusPublishConnection IServiceBusManager.CreateServiceBusPublishConnection(
      IVssRequestContext requestContext,
      string ns,
      string topicName)
    {
      return this.m_ConnectionManager.CreateServiceBusPublishConnection(requestContext, ns, topicName);
    }

    internal void Subscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      Action<IVssRequestContext, IMessage> action,
      TeamFoundationHostType acceptedHostTypes,
      Action<Exception, string, IMessage> exceptionNotification,
      bool invokeActionWithNoMessage)
    {
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, subscription.MessageBusName);
      string[] namespacePool;
      if (this.IsHighAvailabilityNamespacePoolAvailable(requestContext, namespaceName, nameof (Subscribe), out namespacePool))
        this.m_serviceBusHighAvailabilityManagementService.Subscribe(requestContext, subscription, action, acceptedHostTypes, exceptionNotification, invokeActionWithNoMessage, namespaceName, namespacePool);
      else
        this.m_serviceBusLegacyManagementService.Subscribe(requestContext, subscription, action, acceptedHostTypes, exceptionNotification, invokeActionWithNoMessage);
    }

    public void CreatePublisher(
      IVssRequestContext requestContext,
      string messageBusName,
      bool deleteIfExists,
      double subscriberDeleteOnIdleMinutes)
    {
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, messageBusName);
      if (this.IsHighAvailabilityNamespacePoolAvailable(requestContext, namespaceName, nameof (CreatePublisher), out string[] _))
        this.m_serviceBusHighAvailabilityManagementService.CreatePublisher(requestContext, messageBusName, deleteIfExists, subscriberDeleteOnIdleMinutes);
      else
        this.m_serviceBusLegacyManagementService.CreatePublisher(requestContext, messageBusName, deleteIfExists, subscriberDeleteOnIdleMinutes);
    }

    public void CreatePublisher(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusPublisherCreateOptions createOptions)
    {
      string[] namespacePool;
      if (this.IsHighAvailabilityNamespacePoolAvailable(requestContext, createOptions?.Namespace, "CreatePublisher CreateOptions", out namespacePool))
        this.m_serviceBusHighAvailabilityManagementService.CreatePublisher(requestContext, messageBusName, createOptions, namespacePool);
      else
        this.m_serviceBusLegacyManagementService.CreatePublisher(requestContext, messageBusName, createOptions);
    }

    TopicDescription IServiceBusManager.EnsureTopicCreated(
      IVssRequestContext requestContext,
      string messageBusName,
      TopicDescription topicDescription,
      string ns,
      bool deleteIfExists)
    {
      return this.m_ManagerHelper.EnsureTopicCreated(requestContext, messageBusName, topicDescription, ns, deleteIfExists);
    }

    TopicDescription IServiceBusManager.UpdateTopic(
      IVssRequestContext requestContext,
      TopicDescription topicDescription,
      string ns)
    {
      requestContext = requestContext.Elevate();
      TopicDescription returnDescription = (TopicDescription) null;
      NamespaceManager manager = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, string.Empty, ns).GetNamespaceManager();
      manager.Settings.OperationTimeout = TimeSpan.FromMinutes(3.0);
      using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
      {
        Action Run = (Action) (() => returnDescription = ServiceBusManagerHelper.UpdateTopic(requestContext, manager, topicDescription));
        CommandSetter commandsetter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusNamespace.UpdateTopic." + ns)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(manager.Settings.OperationTimeout));
        ServiceBusRetryHelper.ExecuteWithRetries((Action) (() => new CommandService(requestContext, commandsetter, Run).Execute()));
      }
      return returnDescription;
    }

    IEnumerable<TopicDescription> IServiceBusManager.GetTopics(
      IVssRequestContext requestContext,
      string ns)
    {
      requestContext = requestContext.Elevate();
      IEnumerable<TopicDescription> returnDescriptions = (IEnumerable<TopicDescription>) null;
      NamespaceManager manager = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, string.Empty, ns).GetNamespaceManager();
      manager.Settings.OperationTimeout = TimeSpan.FromMinutes(3.0);
      using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
      {
        Action Run = (Action) (() => returnDescriptions = ServiceBusManagerHelper.GetTopics(requestContext, manager));
        CommandSetter commandsetter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusNamespace.GetTopics." + ns)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(manager.Settings.OperationTimeout));
        ServiceBusRetryHelper.ExecuteWithRetries((Action) (() => new CommandService(requestContext, commandsetter, Run).Execute()));
      }
      return returnDescriptions;
    }

    IEnumerable<SubscriptionDescription> IServiceBusManager.GetSubscriptionsForTopic(
      IVssRequestContext requestContext,
      string ns,
      string topicName)
    {
      NamespaceManager namespaceManager = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, string.Empty, ns).GetNamespaceManager();
      return ServiceBusManagerHelper.GetSubscriptions(requestContext, namespaceManager, topicName);
    }

    string IServiceBusManager.GetNamespaceAddress(IVssRequestContext requestContext, string ns) => ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, string.Empty, ns).GetNamespaceManager()?.Address.ToString();

    IEnumerable<RuleDescription> IServiceBusManager.GetSubscriptionRules(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName)
    {
      NamespaceManagerSettings namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName);
      MessageBusSubscriptionInfo subscription = new MessageBusSubscriptionInfo()
      {
        MessageBusName = messageBusName,
        SubscriptionName = subscriptionName,
        Namespace = namespaceManagerSettings?.Namespace
      };
      string topicName;
      if (!ServiceBusSettingsHelper.TryGetSubscriberTopicName(requestContext, subscription, out topicName))
        return (IEnumerable<RuleDescription>) null;
      NamespaceManager namespaceManager = namespaceManagerSettings.GetNamespaceManager();
      return !namespaceManager.SubscriptionExists(topicName, subscriptionName) ? (IEnumerable<RuleDescription>) null : namespaceManager.GetRules(topicName, subscriptionName);
    }

    public (bool success, int created, int updated, int deleted, string error) CopyToMultipleNamespaces(
      IVssRequestContext requestContext,
      string originNamespace,
      List<string> destinationNamespaces,
      bool doDeletion,
      bool dryrun)
    {
      bool flag = true;
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      Dictionary<string, TopicDescription> dictionary = ServiceBusManagerHelper.GetNamespaceManagerFromNamespace(requestContext, originNamespace).GetTopics().ToDictionary<TopicDescription, string>((Func<TopicDescription, string>) (x => x.Path));
      foreach (string destinationNamespace in destinationNamespaces)
      {
        (bool success, int created, int updated, int deleted, string error) = this.CopyToNamespace(requestContext, dictionary, destinationNamespace, doDeletion, dryrun);
        if (!success)
        {
          flag = false;
          stringBuilder.AppendLine(error);
        }
        num1 += created;
        num2 += updated;
        num3 += deleted;
      }
      return (flag, num1, num2, num3, stringBuilder.ToString());
    }

    private (bool success, int created, int updated, int deleted, string error) CopyToNamespace(
      IVssRequestContext requestContext,
      Dictionary<string, TopicDescription> originTopics,
      string destinationNamespace,
      bool doDeletion,
      bool dryrun)
    {
      bool flag = true;
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      NamespaceManager managerFromNamespace = ServiceBusManagerHelper.GetNamespaceManagerFromNamespace(requestContext, destinationNamespace);
      Dictionary<string, TopicDescription> dictionary = managerFromNamespace.GetTopics().ToDictionary<TopicDescription, string>((Func<TopicDescription, string>) (x => x.Path));
      (List<string> stringList1, List<string> stringList2, List<string> stringList3) = this.CompareTopics(originTopics, dictionary);
      int num4 = num1 + stringList1.Count;
      int num5 = num2 + stringList2.Count;
      if (doDeletion)
        num3 += stringList3.Count;
      requestContext.TraceAlways(1005555, TraceLevel.Info, "ServiceBus", nameof (CopyToNamespace), string.Format("Creating: {0}", (object) 0), (object) string.Join(", ", (IEnumerable<string>) stringList1));
      requestContext.TraceAlways(1005555, TraceLevel.Info, "ServiceBus", nameof (CopyToNamespace), string.Format("Updating: {0}", (object) 0), (object) string.Join(", ", (IEnumerable<string>) stringList2));
      requestContext.TraceAlways(1005555, TraceLevel.Info, "ServiceBus", nameof (CopyToNamespace), string.Format("Deleting: {0}", (object) 0), (object) string.Join(", ", (IEnumerable<string>) stringList3));
      if (!dryrun)
      {
        foreach (string key in stringList1)
        {
          try
          {
            managerFromNamespace.CreateTopic(originTopics[key]);
          }
          catch (Exception ex)
          {
            flag = false;
            stringBuilder.AppendLine("Error while creating " + key);
            requestContext.TraceException(1005556, "ServiceBus", nameof (CopyToNamespace), ex);
          }
        }
        foreach (string key in stringList2)
        {
          try
          {
            managerFromNamespace.UpdateTopic(originTopics[key]);
          }
          catch (Exception ex)
          {
            flag = false;
            stringBuilder.AppendLine("Error while updating " + key);
            requestContext.TraceException(1005556, "ServiceBus", nameof (CopyToNamespace), ex);
          }
        }
        if (doDeletion)
        {
          foreach (string str in stringList3)
          {
            if (dictionary[str].SubscriptionCount != 0)
            {
              flag = false;
              stringBuilder.AppendLine("The topic " + str + " has some subscriber so we shouldn't delete it");
            }
            else
            {
              try
              {
                managerFromNamespace.DeleteTopic(str);
              }
              catch (Exception ex)
              {
                flag = false;
                stringBuilder.AppendLine("Error while updating " + str);
                requestContext.TraceException(1005556, "ServiceBus", nameof (CopyToNamespace), ex);
              }
            }
          }
        }
      }
      return (flag, num4, num5, num3, stringBuilder.ToString());
    }

    private (List<string> toCreate, List<string> toUpdate, List<string> toDelete) CompareTopics(
      Dictionary<string, TopicDescription> originTopics,
      Dictionary<string, TopicDescription> destinationTopics)
    {
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      List<string> stringList3 = new List<string>();
      foreach (string key in originTopics.Keys)
      {
        if (!destinationTopics.ContainsKey(key))
          stringList1.Add(key);
        else if (!ServiceBusManagementService.AreSameTopic(originTopics[key], destinationTopics[key]))
          stringList2.Add(key);
      }
      foreach (string key in destinationTopics.Keys)
      {
        if (!originTopics.ContainsKey(key))
          stringList3.Add(key);
      }
      return (stringList1, stringList2, stringList3);
    }

    private static bool AreSameTopic(TopicDescription topic1, TopicDescription topic2) => string.Equals(topic1.Path, topic2.Path, StringComparison.OrdinalIgnoreCase) && topic1.IsAnonymousAccessible == topic2.IsAnonymousAccessible && topic1.SupportOrdering == topic2.SupportOrdering && topic1.EnableBatchedOperations == topic2.EnableBatchedOperations && topic1.DuplicateDetectionHistoryTimeWindow == topic2.DuplicateDetectionHistoryTimeWindow && topic1.RequiresDuplicateDetection == topic2.RequiresDuplicateDetection && topic1.MaxSizeInMegabytes == topic2.MaxSizeInMegabytes && topic1.AutoDeleteOnIdle == topic2.AutoDeleteOnIdle && topic1.DefaultMessageTimeToLive == topic2.DefaultMessageTimeToLive && topic1.EnablePartitioning == topic2.EnablePartitioning && topic1.EnableExpress == topic2.EnableExpress;

    internal void MonitorSubscriberConnections(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      IReadOnlyDictionary<MessageBusSubscriptionInfo, ServiceBusSubscribeConnection> subscriberConnections = this.m_ConnectionManager.GetReadOnlySubscriberConnections();
      if (subscriberConnections == null)
        return;
      foreach (MessageBusSubscriptionInfo key in subscriberConnections.Keys)
      {
        using (requestContext.Lock(ServiceBusLockHelper.GetSubscriberLockName(requestContext, key)))
        {
          ServiceBusSubscribeConnection subscribeConnection;
          if (subscriberConnections.TryGetValue(key, out subscribeConnection))
          {
            DateTime processExecution = subscribeConnection.StartTimeOfInProcessExecution;
            TimeSpan timeSpan = DateTime.UtcNow - processExecution;
            if (processExecution != DateTime.MinValue)
            {
              if (timeSpan > this.m_subscriberProcessingAlertTimeThreshold)
                requestContext.Trace(105409, TraceLevel.Error, "ServiceBus", subscribeConnection.TopicName, string.Format("Subscriber on Topic {0}, on namespace {1} using extension {2} started at {3} and is still processing. It has been processing for {4} and may be stuck.", (object) subscribeConnection.TopicName, (object) key?.Namespace, (object) subscribeConnection.ExtensionType, (object) processExecution, (object) timeSpan));
            }
          }
        }
      }
    }

    internal void WriteNamespaceScopedManagementSetting(
      IVssRequestContext requestContext,
      string ns,
      string settingRelativePath,
      string value)
    {
      string defaultNamespace = ServiceBusSettingsHelper.GetDefaultNamespace(requestContext);
      string path = string.Equals(ns, defaultNamespace, StringComparison.OrdinalIgnoreCase) ? "/Service/MessageBus/ServiceBus/Management" + settingRelativePath : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/MessageBus/ServiceBus/Management/Namespaces/{0}", (object) ns) + settingRelativePath;
      requestContext.GetService<IVssRegistryService>().Write(requestContext, (IEnumerable<RegistryItem>) new RegistryItem[1]
      {
        new RegistryItem(path, value)
      });
    }

    void IServiceBusManager.RegisterNamespace(
      IVssRequestContext requestContext,
      string name,
      string sharedAccessKeySettingName,
      int topicMaxSizeInGB,
      string hostNamePrefix,
      bool prefixComputerName,
      bool isGlobal,
      ITFLogger logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckBoundsInclusive(topicMaxSizeInGB, 1, 80, nameof (topicMaxSizeInGB));
      requestContext = requestContext.Elevate();
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      requestContext.TraceEnter(1005310, "ServiceBus", nameof (ServiceBusManagementService), "RegisterNamespace");
      try
      {
        string str;
        if (isGlobal)
        {
          str = "/Service/MessageBus/ServiceBus/Management";
        }
        else
        {
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/MessageBus/ServiceBus/Management/Namespaces/{0}", (object) name);
          string defaultNamespace = ServiceBusSettingsHelper.GetDefaultNamespace(requestContext);
          if (!string.IsNullOrEmpty(defaultNamespace) && string.Equals(name, defaultNamespace, StringComparison.OrdinalIgnoreCase))
          {
            logger.Info("Ignoring registration of non-global namespace {0} because the global namespace is already registered with the same name", (object) name);
            requestContext.Trace(1005311, TraceLevel.Info, "ServiceBus", nameof (ServiceBusManagementService), "Ignoring registration of non-global namespace {0} because the global namespace is already registered with the same name", (object) name);
            return;
          }
        }
        ArgumentUtility.CheckStringForNullOrEmpty(sharedAccessKeySettingName, nameof (sharedAccessKeySettingName));
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        registryEntryList.Add(new RegistryEntry(str + "/Namespace", name));
        registryEntryList.Add(new RegistryEntry(str + "/SharedAccessKeySettingName", sharedAccessKeySettingName));
        registryEntryList.Add(new RegistryEntry(str + "/TopicMaxSizeInGB", topicMaxSizeInGB.ToString("D")));
        registryEntryList.Add(new RegistryEntry(str + "/PrefixComputerName", prefixComputerName.ToString()));
        if (!string.IsNullOrEmpty(hostNamePrefix))
        {
          if (hostNamePrefix.Equals(ServiceBusManagementService.s_noPrefixToken, StringComparison.Ordinal))
            hostNamePrefix = string.Empty;
          registryEntryList.Add(new RegistryEntry(str + "/HostNamePrefix", hostNamePrefix));
        }
        requestContext.GetService<IVssRegistryService>().WriteEntries(requestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1005312, "ServiceBus", nameof (ServiceBusManagementService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1005319, "ServiceBus", nameof (ServiceBusManagementService), "RegisterNamespace");
      }
    }

    void IServiceBusManager.RegisterHighAvailabilityNamespacePool(
      IVssRequestContext requestContext,
      string namepacePoolName,
      string namespacePoolList,
      bool isGlobal,
      ITFLogger logger)
    {
      this.m_serviceBusHighAvailabilityManagementService.RegisterHighAvailabilityNamespacePool(requestContext, namepacePoolName, namespacePoolList, isGlobal, logger);
    }

    void IServiceBusManager.RegisterHighAvailabilityPublisher(
      IVssRequestContext requestContext,
      string namepacePoolName,
      string primaryNamespace,
      string secondaryNamespace,
      bool isGlobal,
      ITFLogger logger)
    {
      this.m_serviceBusHighAvailabilityManagementService.RegisterHighAvailabilityPublisher(requestContext, namepacePoolName, primaryNamespace, secondaryNamespace, isGlobal, logger);
    }

    public void DeletePublisher(IVssRequestContext requestContext, string messageBusName)
    {
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, messageBusName);
      if (this.IsHighAvailabilityNamespacePoolAvailable(requestContext, namespaceName, nameof (DeletePublisher), out string[] _))
        this.m_serviceBusHighAvailabilityManagementService.DeletePublisher(requestContext, messageBusName);
      else
        this.m_serviceBusLegacyManagementService.DeletePublisher(requestContext, messageBusName);
    }

    public MessageBusSubscriptionInfo CreateTransientSubscriber(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriberPrefix)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(messageBusName, nameof (messageBusName));
      requestContext.TraceEnter(1005250, "ServiceBus", nameof (ServiceBusManagementService), "CreateSubscriber");
      try
      {
        string subscriptionName;
        try
        {
          subscriptionName = !AzureRoleUtil.IsAvailable ? ServiceBusManagementService.CreateTemporarySubscriptionName(Environment.MachineName, subscriberPrefix) : ServiceBusManagementService.CreateTemporarySubscriptionName(AzureRoleUtil.Environment, subscriberPrefix);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1005255, "ServiceBus", nameof (ServiceBusManagementService), ex);
          throw new MessageBusConfigurationException(HostingResources.SubscriptionNameCannotBeGenerated(), ex);
        }
        string subscriberFilter = ServiceBusSettingsHelper.GetSubscriberFilter(requestContext, messageBusName);
        return this.CreateSubscriber(requestContext, messageBusName, subscriptionName, subscriberFilter, (ITFLogger) new TraceLogger(requestContext, "ServiceBus", nameof (ServiceBusManagementService)), true);
      }
      finally
      {
        requestContext.TraceLeave(1005251, "ServiceBus", nameof (ServiceBusManagementService), "CreateSubscriber");
      }
    }

    internal static string CreateTemporarySubscriptionName(
      IRoleEnvironment roleEnvironment,
      string subscriberPrefix)
    {
      string subscriptionName = (subscriberPrefix + "-" + roleEnvironment.CurrentRoleInstanceId + "-" + roleEnvironment.DeploymentId).Trim('-');
      if (subscriptionName.Length > 50)
        subscriptionName = subscriptionName.Substring(0, 50);
      return subscriptionName;
    }

    internal static string CreateTemporarySubscriptionName(
      string machineName,
      string subscriberPrefix)
    {
      string subscriptionName = (subscriberPrefix + "-" + machineName + "-" + Guid.NewGuid().ToString("N")).Trim('-');
      if (subscriptionName.Length > 50)
        subscriptionName = subscriptionName.Substring(0, 50);
      return subscriptionName;
    }

    public MessageBusSubscriptionInfo CreateSubscriber(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      ITFLogger logger = null)
    {
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      string subscriberFilter = ServiceBusSettingsHelper.GetSubscriberFilter(requestContext, messageBusName);
      logger.Info("CreateSubscriber: SQL filter expression value from regisrty: " + subscriberFilter);
      return this.CreateSubscriber(requestContext, messageBusName, subscriptionName, subscriberFilter, logger);
    }

    public string GetSubscriberNameForScaleUnit(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      return ServiceBusSubscribeHelper.GetSubscriberNameForScaleUnit(requestContext, messageBusName);
    }

    internal MessageBusSubscriptionInfo CreateSubscriber(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      string filterExpression,
      ITFLogger logger,
      bool isTransient = false)
    {
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, messageBusName);
      if (this.IsHighAvailabilityNamespacePoolAvailable(requestContext, namespaceName, nameof (CreateSubscriber), out string[] _))
      {
        logger.Info("ServiceBusManagementService.CreateSubscriber: HighAvailability Namespace Pool is Available");
        return this.m_serviceBusHighAvailabilityManagementService.CreateSubscriber(requestContext, messageBusName, subscriptionName, filterExpression, logger, isTransient);
      }
      logger.Info("ServiceBusManagementService.CreateSubscriber: HighAvailability Namespace Pool is NOT Available");
      return this.m_serviceBusLegacyManagementService.CreateSubscriber(requestContext, messageBusName, subscriptionName, filterExpression, logger, isTransient);
    }

    SubscriptionDescription IServiceBusManager.EnsureSubscriptionCreated(
      IVssRequestContext requestContext,
      string messageBusName,
      string ns,
      SubscriptionDescription subscriptionDescription,
      Microsoft.ServiceBus.Messaging.Filter filter,
      int retryCount,
      TimeSpan? timeout)
    {
      return this.m_ManagerHelper.EnsureSubscriptionCreated(requestContext, messageBusName, ns, subscriptionDescription, filter, retryCount, timeout);
    }

    public void DeleteSubscriber(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, subscription.MessageBusName);
      if (this.IsHighAvailabilityNamespacePoolAvailable(requestContext, namespaceName, nameof (DeleteSubscriber), out string[] _))
        this.m_serviceBusHighAvailabilityManagementService.DeleteSubscriber(requestContext, subscription);
      else
        this.m_serviceBusLegacyManagementService.DeleteSubscriber(requestContext, subscription);
    }

    internal void Unsubscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      string[] namespacePool;
      if (this.IsHighAvailabilitySubscriberAndNamespacePoolAvailable(requestContext, subscription, out namespacePool))
        this.m_serviceBusHighAvailabilityManagementService.Unsubscribe(requestContext, subscription, namespacePool);
      else
        this.m_serviceBusLegacyManagementService.Unsubscribe(requestContext, subscription);
    }

    public void UpdateSubscribers(
      IVssRequestContext requestContext,
      MessageBusSubscriberSettings subscriberSettings,
      string messageBusName,
      string subscriptionName,
      string namespaceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(messageBusName, nameof (messageBusName));
      ArgumentUtility.CheckForNull<MessageBusSubscriberSettings>(subscriberSettings, nameof (subscriberSettings));
      requestContext.TraceEnter(1005500, "ServiceBus", nameof (ServiceBusManagementService), nameof (UpdateSubscribers));
      try
      {
        requestContext.GetService<IVssRegistryService>();
        ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
        if (namespaceName == null)
        {
          int? nullable1 = subscriberSettings.AutoDeleteOnIdleInMinutes;
          if (nullable1.HasValue)
          {
            IVssRequestContext requestContext1 = requestContext;
            string messageBusName1 = messageBusName;
            nullable1 = subscriberSettings.AutoDeleteOnIdleInMinutes;
            int autoDeleteOnIdleInMinutes = nullable1.Value;
            ServiceBusSettingsHelper.SetAutoDeleteOnIdleForSubscription(requestContext1, messageBusName1, autoDeleteOnIdleInMinutes);
          }
          nullable1 = subscriberSettings.DefaultMessageTimeToLiveInMinutes;
          if (nullable1.HasValue)
          {
            IVssRequestContext requestContext2 = requestContext;
            string messageBusName2 = messageBusName;
            nullable1 = subscriberSettings.DefaultMessageTimeToLiveInMinutes;
            int messageTimeToLiveInMinutes = nullable1.Value;
            ServiceBusSettingsHelper.SetMessageTimeToLiveForSubscription(requestContext2, messageBusName2, messageTimeToLiveInMinutes);
          }
          bool? nullable2 = subscriberSettings.EnableDeadLetteringOnMessageExpiration;
          if (nullable2.HasValue)
          {
            IVssRequestContext requestContext3 = requestContext;
            string messageBusName3 = messageBusName;
            nullable2 = subscriberSettings.EnableDeadLetteringOnMessageExpiration;
            int num = nullable2.Value ? 1 : 0;
            ServiceBusSettingsHelper.SetEnableDeadLetteringOnMessageExpirationForSubscription(requestContext3, messageBusName3, num != 0);
          }
          nullable2 = subscriberSettings.EnableDeadLetteringOnFilterEvaluationExceptions;
          if (nullable2.HasValue)
          {
            IVssRequestContext requestContext4 = requestContext;
            string messageBusName4 = messageBusName;
            nullable2 = subscriberSettings.EnableDeadLetteringOnFilterEvaluationExceptions;
            int num = nullable2.Value ? 1 : 0;
            ServiceBusSettingsHelper.SetEnableDeadLetteringOnFilterEvaluationExceptionsForSubscription(requestContext4, messageBusName4, num != 0);
          }
        }
        NamespaceManagerSettings namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName, namespaceName);
        NamespaceManager namespaceManager = namespaceManagerSettings.GetNamespaceManager();
        MessageBusSubscriptionInfo subscription1 = new MessageBusSubscriptionInfo()
        {
          MessageBusName = messageBusName,
          SubscriptionName = subscriptionName,
          Namespace = namespaceManagerSettings?.Namespace
        };
        string topicName = (string) null;
        if (!ServiceBusSettingsHelper.TryGetSubscriberTopicName(requestContext, subscription1, out topicName))
          topicName = ServiceBusSettingsHelper.CreateTopicName(messageBusName, namespaceManagerSettings);
        using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
        {
          List<SubscriptionDescription> subscriptionDescriptionList = new List<SubscriptionDescription>();
          if (string.IsNullOrEmpty(subscriptionName))
          {
            List<string> subscriptionNames = ServiceBusSettingsHelper.GetSubscriptionNames(requestContext, messageBusName);
            using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
            {
              foreach (string name in subscriptionNames)
              {
                try
                {
                  subscriptionDescriptionList.Add(namespaceManager.GetSubscription(topicName, name));
                }
                catch (MessagingEntityNotFoundException ex)
                {
                  string message = string.Format("No subscription found under topic {0}, namespace {1} with name {2}. Settings for this subscription will not be updated.", (object) topicName, (object) namespaceManagerSettings?.Namespace, (object) subscriptionName);
                  requestContext.Trace(1005510, TraceLevel.Warning, "ServiceBus", nameof (ServiceBusManagementService), message);
                }
              }
            }
            if (subscriptionDescriptionList.Count == 0)
            {
              requestContext.Trace(1005506, TraceLevel.Warning, "ServiceBus", nameof (ServiceBusManagementService), "No subscription found under topic {0}, namespace {1}, operation ignored.", (object) topicName, (object) subscription1.Namespace);
              return;
            }
          }
          else
          {
            try
            {
              using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
                subscriptionDescriptionList.Add(namespaceManager.GetSubscription(topicName, subscriptionName));
            }
            catch (MessagingEntityNotFoundException ex)
            {
              string message = string.Format("No subscription found under topic {0}, namespace {1} with name {2}", (object) topicName, (object) namespaceManagerSettings?.Namespace, (object) subscriptionName);
              requestContext.Trace(1005507, TraceLevel.Error, "ServiceBus", nameof (ServiceBusManagementService), message);
              throw new ArgumentException(message, nameof (subscriptionName));
            }
          }
          List<Exception> exceptions = new List<Exception>();
          subscriptionDescriptionList.ForEach((Action<SubscriptionDescription>) (subscription =>
          {
            if (subscriberSettings.AutoDeleteOnIdleInMinutes.HasValue)
              subscription.AutoDeleteOnIdle = TimeSpan.FromMinutes((double) subscriberSettings.AutoDeleteOnIdleInMinutes.Value);
            if (subscriberSettings.DefaultMessageTimeToLiveInMinutes.HasValue)
              subscription.DefaultMessageTimeToLive = TimeSpan.FromMinutes((double) subscriberSettings.DefaultMessageTimeToLiveInMinutes.Value);
            if (subscriberSettings.EnableDeadLetteringOnFilterEvaluationExceptions.HasValue)
              subscription.EnableDeadLetteringOnFilterEvaluationExceptions = subscriberSettings.EnableDeadLetteringOnFilterEvaluationExceptions.Value;
            if (subscriberSettings.EnableDeadLetteringOnMessageExpiration.HasValue)
              subscription.EnableDeadLetteringOnMessageExpiration = subscriberSettings.EnableDeadLetteringOnMessageExpiration.Value;
            try
            {
              ServiceBusManagerHelper.UpdateSubscription(requestContext, namespaceManager, subscription);
              TeamFoundationTracingService.TraceRaw(1005508, TraceLevel.Info, "ServiceBus", nameof (ServiceBusManagementService), "Updated settings for subscription '{0}'.", (object) subscription.Name, (object) subscription.AutoDeleteOnIdle);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1005509, TraceLevel.Error, "ServiceBus", nameof (ServiceBusManagementService), ex);
              exceptions.Add(ex);
            }
          }));
          if (exceptions.Count > 0)
            throw new AggregateException("Exception thrown during update subscription, check innerException for details", (IEnumerable<Exception>) exceptions);
        }
      }
      finally
      {
        requestContext.TraceLeave(1005501, "ServiceBus", nameof (ServiceBusManagementService), nameof (UpdateSubscribers));
      }
    }

    public IEnumerable<MessageBusSubscriptionInfo> GetSubscribers(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      requestContext.TraceEnter(1005600, "ServiceBus", nameof (ServiceBusManagementService), nameof (GetSubscribers));
      try
      {
        NamespaceManagerSettings namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName);
        NamespaceManager namespaceManager = namespaceManagerSettings.GetNamespaceManager();
        MessageBusSubscriptionInfo subscription = new MessageBusSubscriptionInfo()
        {
          MessageBusName = messageBusName
        };
        string topicName = (string) null;
        if (!ServiceBusSettingsHelper.TryGetSubscriberTopicName(requestContext, subscription, out topicName))
          topicName = ServiceBusSettingsHelper.CreateTopicName(messageBusName, namespaceManagerSettings);
        IEnumerable<SubscriptionDescription> subscriptions;
        using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
        {
          using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
            subscriptions = namespaceManager.GetSubscriptions(topicName);
        }
        requestContext.Trace(1005602, TraceLevel.Verbose, "ServiceBus", nameof (ServiceBusManagementService), "Found {0} subscriptions under topic {1}, namespace {2}", (object) subscriptions.Count<SubscriptionDescription>(), (object) topicName, (object) namespaceManagerSettings?.Namespace);
        return (IEnumerable<MessageBusSubscriptionInfo>) subscriptions.Select<SubscriptionDescription, MessageBusSubscriptionInfo>((Func<SubscriptionDescription, MessageBusSubscriptionInfo>) (s => new MessageBusSubscriptionInfo()
        {
          SubscriptionName = s.Name,
          MessageBusName = messageBusName,
          LastAccessedAt = new DateTime?(s.AccessedAt),
          CreatedAt = new DateTime?(s.CreatedAt),
          AutoDeleteOnIdle = new TimeSpan?(s.AutoDeleteOnIdle)
        })).ToList<MessageBusSubscriptionInfo>();
      }
      finally
      {
        requestContext.TraceLeave(1005601, "ServiceBus", nameof (ServiceBusManagementService), nameof (GetSubscribers));
      }
    }

    private bool IsHighAvailabilitySubscriberAndNamespacePoolAvailable(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      out string[] namespacePool)
    {
      string namespacePoolName;
      if (this.m_ConnectionManager.TryGetHighAvailabilitySubscribeEntry(requestContext, subscription, out namespacePoolName) && !string.IsNullOrEmpty(namespacePoolName))
      {
        if (ServiceBusSettingsHelper.TryGetNamespacePool(requestContext, namespacePoolName, out namespacePool))
          return true;
        requestContext.TraceAlways(1005114, TraceLevel.Warning, "ServiceBus", nameof (ServiceBusManagementService), subscription.MessageBusName + " / " + subscription.SubscriptionName + " : Could not find Namespace Pool with name " + namespacePoolName);
      }
      namespacePool = (string[]) null;
      return false;
    }

    private bool IsHighAvailabilityNamespacePoolAvailable(
      IVssRequestContext requestContext,
      string namespacePoolName,
      string caller,
      out string[] namespacePool)
    {
      if (namespacePoolName != null && !ServiceBusWellKnownNamespaces.IsNotHighAvailabilityNamespace(requestContext, namespacePoolName))
      {
        if (ServiceBusSettingsHelper.TryGetNamespacePool(requestContext, namespacePoolName, out namespacePool))
          return true;
        requestContext.TraceAlways(1005104, TraceLevel.Warning, "ServiceBus", nameof (ServiceBusManagementService), "No namespace pool was available for namespacePool " + namespacePoolName + ".  Caller: " + caller);
      }
      namespacePool = (string[]) null;
      return false;
    }

    private bool IsHighAvailabilityPublisherAvailable(
      IVssRequestContext requestContext,
      string namespacePoolName,
      string caller,
      out string primaryNamespace,
      out string secondaryNamespace)
    {
      if (!ServiceBusWellKnownNamespaces.IsNotHighAvailabilityNamespace(requestContext, namespacePoolName))
      {
        if (ServiceBusSettingsHelper.TryGetHighAvailabilityPublishNamespaces(requestContext, namespacePoolName, out primaryNamespace, out secondaryNamespace))
          return true;
        requestContext.TraceAlways(1005144, TraceLevel.Warning, "ServiceBus", nameof (ServiceBusManagementService), "No publisher was available for namespacePool " + namespacePoolName + ".  Caller: " + caller);
      }
      primaryNamespace = (string) null;
      secondaryNamespace = (string) null;
      return false;
    }

    public void UpdateSubscriptionFilter(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      string newFilterValue,
      bool isTransient = false,
      ITFLogger logger = null)
    {
      MessageBusSubscriptionInfo subscription = new MessageBusSubscriptionInfo()
      {
        MessageBusName = messageBusName,
        SubscriptionName = subscriptionName
      };
      if (this.IsHighAvailabilitySubscriberAndNamespacePoolAvailable(requestContext, subscription, out string[] _))
      {
        logger.Info("UpdateFilter: High availability service bus namespace is available.");
        this.m_serviceBusHighAvailabilityManagementService.UpdateSubscriptionFilter(requestContext, messageBusName, subscriptionName, newFilterValue, false, logger);
      }
      else
      {
        logger.Info("UpdateFilter: High availability service bus namespace is NOT available.");
        this.m_serviceBusLegacyManagementService.UpdateSubscriptionFilter(requestContext, messageBusName, subscriptionName, newFilterValue, false, logger);
      }
    }
  }
}
