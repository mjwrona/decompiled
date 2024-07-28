// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusLegacyManagementService
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
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusLegacyManagementService : 
    VssBaseService,
    IMessageBusManagementService,
    IVssFrameworkService,
    IServiceBusManager
  {
    private ServiceBusConnectionManager m_ConnectionManager;
    private ServiceBusManagerHelper m_ManagerHelper;
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusLegacyManagementService";
    private ServiceBusManagementService m_ServiceBusManagementService;
    private TimeSpan m_subscriberProcessingAlertTimeThreshold;
    private ServiceBusLogger m_Logger;
    private VssTaskDispatcher m_publishDispatcher;

    internal ServiceBusLegacyManagementService(
      IVssRequestContext requestContext,
      ServiceBusManagementService serviceBusManagementService,
      ServiceBusConnectionManager connectionManager,
      ServiceBusLogger logger,
      VssTaskDispatcher publishDispatcher)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.m_ManagerHelper = new ServiceBusManagerHelper(nameof (ServiceBusLegacyManagementService));
      this.m_ConnectionManager = connectionManager;
      this.m_ServiceBusManagementService = serviceBusManagementService;
      this.m_Logger = logger;
      this.m_publishDispatcher = publishDispatcher;
      this.m_subscriberProcessingAlertTimeThreshold = TimeSpan.FromSeconds((double) service.GetValue<int>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/SubscriberThresholdInSeconds", 900));
    }

    public void CreatePublisher(
      IVssRequestContext requestContext,
      string messageBusName,
      bool deleteIfExists,
      double subscriberDeleteOnIdleMinutes)
    {
      MessageBusPublisherCreateOptions createOptions = new MessageBusPublisherCreateOptions()
      {
        SubscriptionIdleTimeout = TimeSpan.FromMinutes(subscriberDeleteOnIdleMinutes),
        DeleteIfExists = deleteIfExists
      };
      this.CreatePublisher(requestContext, messageBusName, createOptions);
    }

    public void CreatePublisher(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusPublisherCreateOptions createOptions)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(messageBusName, nameof (messageBusName));
      createOptions = createOptions ?? new MessageBusPublisherCreateOptions();
      requestContext.TraceEnter(1005201, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (CreatePublisher));
      try
      {
        requestContext.GetService<IVssRegistryService>();
        string defaultNamespace = ServiceBusSettingsHelper.GetDefaultNamespace(requestContext);
        if (!string.IsNullOrEmpty(createOptions.Namespace))
          ServiceBusSettingsHelper.ValidateNamespaceName(requestContext, createOptions.Namespace, defaultNamespace);
        else
          createOptions.Namespace = defaultNamespace;
        NamespaceManagerSettings namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName, createOptions.Namespace);
        string topicName = ServiceBusSettingsHelper.CreateTopicName(messageBusName, namespaceManagerSettings);
        TopicDescription topicDescription = ServiceBusSettingsHelper.CreateTopicDescription(requestContext, messageBusName, createOptions, topicName);
        try
        {
          this.EnsureTopicCreated(requestContext, messageBusName, topicDescription, createOptions.Namespace, createOptions.DeleteIfExists);
        }
        catch (MessageBusPublisherAlreadyExistsException ex)
        {
          requestContext.TraceException(45917789, "ServiceBus", nameof (ServiceBusLegacyManagementService), (Exception) ex);
        }
        ServiceBusSettingsHelper.RegisterPublisherSettings(requestContext, messageBusName, createOptions, namespaceManagerSettings.Namespace, topicName);
      }
      finally
      {
        requestContext.TraceLeave(1005220, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (CreatePublisher));
      }
    }

    public IServiceBusPublishConnection CreateServiceBusPublishConnection(
      IVssRequestContext requestContext,
      string ns,
      string topicName)
    {
      return this.m_ConnectionManager.CreateServiceBusPublishConnection(requestContext, ns, topicName);
    }

    public MessageBusSubscriptionInfo CreateSubscriber(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      ITFLogger logger = null)
    {
      string subscriberFilter = ServiceBusSettingsHelper.GetSubscriberFilter(requestContext, messageBusName);
      logger.Info("ServiceBusLegacyManagementService.CreateSubscriber: Created expression: " + subscriberFilter);
      return this.CreateSubscriber(requestContext, messageBusName, subscriptionName, subscriberFilter, logger);
    }

    public MessageBusSubscriptionInfo CreateTransientSubscriber(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriberPrefix)
    {
      throw new NotImplementedException();
    }

    public void DeletePublisher(IVssRequestContext requestContext, string messageBusName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(messageBusName, nameof (messageBusName));
      requestContext.TraceEnter(1005221, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (DeletePublisher));
      try
      {
        string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
        this.m_ConnectionManager.RemovePublishConnection(requestContext, publisherRegistryRoot);
        NamespaceManagerSettings settings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName);
        NamespaceManager manager = settings.GetNamespaceManager();
        string topicName;
        if (!ServiceBusSettingsHelper.TryGetPublisherTopicName(requestContext, messageBusName, out topicName))
          topicName = ServiceBusSettingsHelper.CreateTopicName(messageBusName, settings);
        using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
        {
          Action Run = (Action) (() => ServiceBusManagerHelper.DeleteTopic(requestContext, manager, settings.PrefixMachineName, topicName));
          CommandSetter commandsetter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusNamespace.DeletePublisher." + settings.Namespace)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(manager.Settings.OperationTimeout));
          ServiceBusRetryHelper.ExecuteWithRetries((Action) (() => new CommandService(requestContext, commandsetter, Run).Execute()));
        }
        ServiceBusSettingsHelper.DeletePublisherSettings(requestContext, publisherRegistryRoot);
      }
      finally
      {
        requestContext.TraceLeave(1005240, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (DeletePublisher));
      }
    }

    public void DeleteSubscriber(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MessageBusSubscriptionInfo>(subscription, nameof (subscription));
      requestContext.TraceEnter(1005261, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (DeleteSubscriber));
      try
      {
        requestContext.GetService<IVssRegistryService>();
        if (string.IsNullOrEmpty(subscription.Namespace))
          subscription.Namespace = ServiceBusSettingsHelper.GetNamespaceName(requestContext, subscription.MessageBusName);
        this.m_ConnectionManager.RemoveSubscribeConnection(requestContext, subscription);
        NamespaceManagerSettings namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, subscription.MessageBusName);
        NamespaceManager namespaceManager = namespaceManagerSettings.GetNamespaceManager();
        string topicName;
        if (!ServiceBusSettingsHelper.TryGetSubscriberTopicName(requestContext, subscription, out topicName))
          topicName = ServiceBusSettingsHelper.CreateTopicName(subscription.MessageBusName, namespaceManagerSettings);
        this.m_ManagerHelper.DeleteSubscriptionWithRetry(requestContext, namespaceManager, subscription, topicName);
        ServiceBusSettingsHelper.DeleteSubscriberSettings(requestContext, subscription);
      }
      finally
      {
        requestContext.TraceLeave(1005280, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (DeleteSubscriber));
      }
    }

    public SubscriptionDescription EnsureSubscriptionCreated(
      IVssRequestContext requestContext,
      string messageBusName,
      string ns,
      SubscriptionDescription subscriptionDescription,
      Microsoft.ServiceBus.Messaging.Filter filter,
      int retryCount = 5,
      TimeSpan? timeout = null)
    {
      return this.m_ManagerHelper.EnsureSubscriptionCreated(requestContext, messageBusName, ns, subscriptionDescription, filter, retryCount, timeout);
    }

    public TopicDescription EnsureTopicCreated(
      IVssRequestContext requestContext,
      string messageBusName,
      TopicDescription topicDescription,
      string ns,
      bool deleteIfExists)
    {
      return this.m_ManagerHelper.EnsureTopicCreated(requestContext, messageBusName, topicDescription, ns, deleteIfExists);
    }

    public void FixMessageQueueMappings(
      IVssRequestContext deploymentContext,
      string ns,
      string hostNamePrefix,
      string sharedAccessKeySettingName,
      ITFLogger logger)
    {
      throw new NotImplementedException();
    }

    public string GetNamespaceAddress(IVssRequestContext requestContext, string ns) => throw new NotImplementedException();

    public string GetSubscriberNameForScaleUnit(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<MessageBusSubscriptionInfo> GetSubscribers(
      IVssRequestContext requestContext,
      string topicName)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<RuleDescription> GetSubscriptionRules(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<SubscriptionDescription> GetSubscriptionsForTopic(
      IVssRequestContext requestContext,
      string ns,
      string topicName)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<TopicDescription> GetTopics(IVssRequestContext requestContext, string ns) => throw new NotImplementedException();

    public void RegisterNamespace(
      IVssRequestContext requestContext,
      string name,
      string sharedAccessKeySettingName,
      int topicMaxSizeInGB = 5,
      string hostNamePrefix = null,
      bool prefixComputerName = false,
      bool isGlobal = true,
      ITFLogger logger = null)
    {
      throw new NotImplementedException();
    }

    void IServiceBusManager.RegisterHighAvailabilityNamespacePool(
      IVssRequestContext requestContext,
      string namepacePoolName,
      string namespacePoolList,
      bool isGlobal,
      ITFLogger logger)
    {
      throw new NotImplementedException();
    }

    void IServiceBusManager.RegisterHighAvailabilityPublisher(
      IVssRequestContext requestContext,
      string namepacePoolName,
      string primaryNamespace,
      string secondaryNamespace,
      bool isGlobal,
      ITFLogger logger)
    {
      throw new NotImplementedException();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void UpdateSubscribers(
      IVssRequestContext requestContext,
      MessageBusSubscriberSettings subscriberSettings,
      string messageBusName,
      string subscriptionName,
      string namespaceName)
    {
      throw new NotImplementedException();
    }

    public TopicDescription UpdateTopic(
      IVssRequestContext requestContext,
      TopicDescription topicDescription,
      string ns)
    {
      throw new NotImplementedException();
    }

    public void ClearPublishers(IVssRequestContext requestContext, bool dispose = false) => this.m_ConnectionManager.ClearPublishers(requestContext, dispose);

    internal MessageBusSubscriptionInfo CreateSubscriber(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      string filterExpression,
      ITFLogger logger,
      bool isTransient = false)
    {
      return this.ExecuteCommandOnTopicWithRetries(requestContext, messageBusName, subscriptionName, nameof (CreateSubscriber), (Action<string, string, MessageBusSubscriptionInfo>) ((namespaceName, topicName, subscriptionInfo) =>
      {
        NamespaceManagerSettings namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName);
        string subscriptionName1 = ServiceBusSubscribeHelper.GetInternalSubscriptionName(requestContext, subscriptionInfo, subscriptionName, namespaceManagerSettings.PrefixMachineName);
        SubscriptionDescription subscriptionDescription = ServiceBusSubscribeHelper.CreateSubscriptionDescription(requestContext, namespaceName, messageBusName, topicName, subscriptionName1);
        SqlFilter sqlFilter = ServiceBusSubscribeHelper.GetSqlFilter(filterExpression, messageBusName, namespaceManagerSettings.PrefixMachineName);
        this.EnsureSubscriptionCreated(requestContext, messageBusName, string.Empty, subscriptionDescription, (Microsoft.ServiceBus.Messaging.Filter) sqlFilter, 5, new TimeSpan?());
        subscriptionInfo.MaxDeliveryCount = subscriptionDescription.MaxDeliveryCount;
        ServiceBusSettingsHelper.RegisterSubscriberSettings(requestContext, subscriptionInfo, namespaceName, topicName, subscriptionName1, isTransient);
      }));
    }

    private MessageBusSubscriptionInfo ExecuteCommandOnTopicWithRetries(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      string commandName,
      Action<string, string, MessageBusSubscriptionInfo> command)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(messageBusName, nameof (messageBusName));
      ArgumentUtility.CheckForNull<string>(subscriptionName, nameof (subscriptionName));
      requestContext.TraceEnter(1005241, "ServiceBus", nameof (ServiceBusLegacyManagementService), commandName);
      int failedAttempts = 0;
      bool isForwarded = ServiceBusSettingsHelper.IsForwardedTopic(requestContext, messageBusName);
label_1:
      try
      {
        requestContext.CheckDeploymentRequestContext();
        ServiceBusSubscribeHelper.ValidateSubscriptionName(subscriptionName);
        requestContext.GetService<IVssRegistryService>();
        string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, messageBusName);
        NamespaceManagerSettings namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName);
        MessageBusSubscriptionInfo subscriptionInfo = new MessageBusSubscriptionInfo()
        {
          MessageBusName = messageBusName,
          SubscriptionName = subscriptionName,
          Namespace = namespaceName
        };
        NamespaceManager namespaceManager = namespaceManagerSettings.GetNamespaceManager();
        string topicName = !isForwarded ? ServiceBusSubscribeHelper.GetOrCreateSubscriberTopicName(requestContext, messageBusName, subscriptionInfo, namespaceManagerSettings.PrefixMachineName, namespaceManagerSettings.HostnamePrefix) : ServiceBusSubscribeHelper.GetForwardedTopicNameForSubscriber(requestContext, namespaceManager, namespaceName, messageBusName, subscriptionInfo);
        this.m_ManagerHelper.VerifyTopicCreated(requestContext, namespaceManager, topicName);
        command(namespaceName, topicName, subscriptionInfo);
        return subscriptionInfo;
      }
      catch (ServerBusyException ex) when (ex.Detail.ErrorCode == 50009)
      {
        if (ServiceBusSubscribeHelper.HandleCreateSubscriberThrottlingException(requestContext, ex, ref failedAttempts))
          throw;
        else
          goto label_1;
      }
      catch (Microsoft.ServiceBus.Messaging.QuotaExceededException ex)
      {
        if (ServiceBusSubscribeHelper.HandleCreateSubscriberQuotaExceededException(requestContext, ex, isForwarded, ref failedAttempts))
          throw;
        else
          goto label_1;
      }
      finally
      {
        requestContext.TraceLeave(1005260, "ServiceBus", nameof (ServiceBusLegacyManagementService), commandName);
      }
    }

    internal void Publish(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusMessage[] messages,
      bool throwOnMissingPublisher = true)
    {
      requestContext.TraceEnter(1005150, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (Publish));
      try
      {
        bool? nullable = new bool?();
        Stopwatch stopwatch = Stopwatch.StartNew();
        string ns = (string) null;
        string layer = messageBusName;
        try
        {
          IServiceBusPublishConnection connectionInfo = this.m_ConnectionManager.GetConnectionInfo(requestContext, messageBusName, throwOnMissingPublisher, out layer, out ns);
          if (connectionInfo == null)
            return;
          connectionInfo.Publish(requestContext, messageBusName, messages);
          nullable = new bool?(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1005155, "ServiceBus", layer, ex);
          nullable = new bool?(false);
          throw;
        }
        finally
        {
          stopwatch.Stop();
          if (nullable.HasValue)
          {
            this.m_Logger.LogMessageProcessing(requestContext, ns, messageBusName, nullable.Value);
            VssPerformanceCounter performanceCounter;
            if (nullable.Value)
            {
              VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPublishedMessagesTotal", messageBusName).IncrementBy((long) messages.Length);
              performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPublishedMessagesPerSec", messageBusName);
              performanceCounter.IncrementBy((long) messages.Length);
            }
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAveragePublishTime", messageBusName);
            performanceCounter.IncrementMilliseconds(stopwatch.ElapsedMilliseconds);
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAveragePublishTimeBase", messageBusName);
            performanceCounter.IncrementBy((long) messages.Length);
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(1005170, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (Publish));
      }
    }

    internal async Task PublishAsync(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusMessage[] messages,
      bool throwOnMissingPublisher = true)
    {
      requestContext.TraceEnter(1005150, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (PublishAsync));
      try
      {
        bool? success = new bool?();
        Stopwatch watch = Stopwatch.StartNew();
        string ns = (string) null;
        string layer = messageBusName;
        try
        {
          IServiceBusPublishConnection connectionInfo = this.m_ConnectionManager.GetConnectionInfo(requestContext, messageBusName, throwOnMissingPublisher, out layer, out ns);
          if (connectionInfo != null)
          {
            await connectionInfo.PublishAsync(requestContext, messageBusName, messages);
            success = new bool?(true);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1005155, "ServiceBus", layer, ex);
          success = new bool?(false);
          throw;
        }
        finally
        {
          watch.Stop();
          if (success.HasValue)
          {
            this.m_Logger.LogMessageProcessing(requestContext, ns, messageBusName, success.Value);
            VssPerformanceCounter performanceCounter;
            if (success.Value)
            {
              performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPublishedMessagesTotal", messageBusName);
              performanceCounter.IncrementBy((long) messages.Length);
              performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPublishedMessagesPerSec", messageBusName);
              performanceCounter.IncrementBy((long) messages.Length);
            }
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAveragePublishTime", messageBusName);
            performanceCounter.IncrementMilliseconds(watch.ElapsedMilliseconds);
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAveragePublishTimeBase", messageBusName);
            performanceCounter.IncrementBy((long) messages.Length);
          }
        }
        success = new bool?();
        watch = (Stopwatch) null;
        ns = (string) null;
        layer = (string) null;
      }
      finally
      {
        requestContext.TraceLeave(1005170, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (PublishAsync));
      }
    }

    internal void Subscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      Action<IVssRequestContext, IMessage> action,
      TeamFoundationHostType acceptedHostTypes,
      Action<Exception, string, IMessage> exceptionNotification,
      bool invokeActionWithNoMessage)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MessageBusSubscriptionInfo>(subscription, nameof (subscription));
      ArgumentUtility.CheckForNull<Action<IVssRequestContext, IMessage>>(action, nameof (action));
      if ((acceptedHostTypes & ~TeamFoundationHostType.All) != TeamFoundationHostType.Unknown)
        throw new UnexpectedHostTypeException(acceptedHostTypes);
      requestContext.TraceEnter(1005180, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (Subscribe));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        MessageReceiverSettings receiverSettings;
        if (!ServiceBusSettingsHelper.TryGetSubscriptionRegistryEntries(requestContext, subscription, "", out receiverSettings))
          throw new MessageBusNotFoundException(subscription.MessageBusName);
        if (string.IsNullOrEmpty(subscription.Namespace))
          subscription.Namespace = ServiceBusSettingsHelper.GetNamespaceName(requestContext, subscription.MessageBusName);
        using (requestContext.Lock(ServiceBusLockHelper.GetSubscriberLockName(requestContext, subscription)))
        {
          if (this.m_ConnectionManager.DoesSubscriberConnectionExist(subscription))
            throw new MessageBusAlreadySubscribingException(subscription.MessageBusName, subscription.SubscriptionName);
          ServiceBusSubscribeConnection connInfo = new ServiceBusSubscribeConnection(requestContext, (IMessageBusManagementService) this, subscription, requestContext.ServiceHost, action, exceptionNotification, acceptedHostTypes, invokeActionWithNoMessage, receiverSettings, this.m_Logger);
          this.m_ConnectionManager.AddSubscribeConnection(requestContext, subscription, connInfo);
          connInfo.Subscribe();
        }
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusTotalSubscribers", subscription.MessageBusName).Increment();
      }
      finally
      {
        requestContext.TraceLeave(1005200, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (Subscribe));
      }
    }

    internal void Unsubscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MessageBusSubscriptionInfo>(subscription, nameof (subscription));
      requestContext.TraceEnter(1005210, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (Unsubscribe));
      if (string.IsNullOrEmpty(subscription.Namespace))
        subscription.Namespace = ServiceBusSettingsHelper.GetNamespaceName(requestContext, subscription.MessageBusName);
      try
      {
        this.m_ConnectionManager.RemoveSubscribeConnection(requestContext, subscription);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusTotalSubscribers", subscription.SubscriptionName).Decrement();
      }
      finally
      {
        requestContext.TraceLeave(1005220, "ServiceBus", nameof (ServiceBusLegacyManagementService), nameof (Unsubscribe));
      }
    }

    public void UpdateSubscriptionFilter(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      string newFilterValue,
      bool isTransient = false,
      ITFLogger logger = null)
    {
      this.ExecuteCommandOnTopicWithRetries(requestContext, messageBusName, subscriptionName, nameof (UpdateSubscriptionFilter), (Action<string, string, MessageBusSubscriptionInfo>) ((namespaceName, topicName, subscriptionInfo) =>
      {
        NamespaceManagerSettings namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName);
        string subscriptionName1 = ServiceBusSubscribeHelper.GetInternalSubscriptionName(requestContext, subscriptionInfo, subscriptionName, namespaceManagerSettings.PrefixMachineName);
        SubscriptionDescription subscriptionDescription = ServiceBusSubscribeHelper.CreateSubscriptionDescription(requestContext, namespaceName, messageBusName, topicName, subscriptionName1);
        if (!this.m_ManagerHelper.SubscriptionExists(requestContext, messageBusName, namespaceName, subscriptionDescription))
        {
          logger.Info("UpdateFilter: Subscription " + topicName + "|" + subscriptionName + " Does not exist, creating it with default filter " + newFilterValue);
          SqlFilter sqlFilter = ServiceBusSubscribeHelper.GetSqlFilter(newFilterValue, messageBusName, namespaceManagerSettings.PrefixMachineName);
          this.EnsureSubscriptionCreated(requestContext, messageBusName, string.Empty, subscriptionDescription, (Microsoft.ServiceBus.Messaging.Filter) sqlFilter, 5, new TimeSpan?());
          subscriptionInfo.MaxDeliveryCount = subscriptionDescription.MaxDeliveryCount;
          ServiceBusSettingsHelper.RegisterSubscriberSettings(requestContext, subscriptionInfo, namespaceName, topicName, subscriptionName1, isTransient);
        }
        else
        {
          logger.Info("UpdateFilter: Subscription " + topicName + "|" + subscriptionName + " exist, updating default filter to " + newFilterValue);
          ServiceBusClientHelper.UpdateFilterAsync(requestContext, messageBusName, namespaceName, topicName, subscriptionName, newFilterValue, logger).GetAwaiter().GetResult();
        }
      }));
    }
  }
}
