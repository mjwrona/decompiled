// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusHighAvailabilityManagementService
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
  internal class ServiceBusHighAvailabilityManagementService : 
    VssBaseService,
    IMessageBusManagementService,
    IVssFrameworkService,
    IServiceBusManager
  {
    private ServiceBusConnectionManager m_ConnectionManager;
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusHighAvailabilityManagementService";
    private TimeSpan m_subscriberProcessingAlertTimeThreshold;
    private ServiceBusManagementService m_ServiceBusManagementService;
    private ServiceBusManagerHelper m_ManagerHelper;
    private ServiceBusLogger m_Logger;
    private static readonly Random s_random = new Random();
    private VssTaskDispatcher m_publishDispatcher;

    internal ServiceBusHighAvailabilityManagementService(
      IVssRequestContext requestContext,
      ServiceBusManagementService serviceBusManagementService,
      ServiceBusConnectionManager connectionManager,
      ServiceBusLogger logger,
      VssTaskDispatcher publishDispatcher)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.m_ServiceBusManagementService = serviceBusManagementService;
      this.m_ConnectionManager = connectionManager;
      this.m_Logger = logger;
      this.m_publishDispatcher = publishDispatcher;
      this.m_ManagerHelper = new ServiceBusManagerHelper(nameof (ServiceBusHighAvailabilityManagementService));
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
      string[] namespacePoolList;
      if (ServiceBusSettingsHelper.TryGetNamespacePool(requestContext, createOptions.Namespace, out namespacePoolList))
      {
        this.CreatePublisher(requestContext, messageBusName, createOptions, namespacePoolList);
      }
      else
      {
        requestContext.Trace(97186105, TraceLevel.Error, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "CreatePublisher: High Availability Namespace Pool does not exist");
        throw new MessageBusConfigurationException("CreatePublisher: High Availability Namespace Pool does not exist");
      }
    }

    internal void CreatePublisher(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusPublisherCreateOptions createOptions,
      string[] namespacePool)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(messageBusName, nameof (messageBusName));
      ArgumentUtility.CheckForNull<string[]>(namespacePool, nameof (namespacePool));
      requestContext.TraceEnter(1005201, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (CreatePublisher));
      createOptions = createOptions ?? new MessageBusPublisherCreateOptions();
      string defaultNamespace = ServiceBusSettingsHelper.GetDefaultNamespace(requestContext);
      if (!string.IsNullOrEmpty(createOptions.Namespace))
        ServiceBusSettingsHelper.ValidateNamespaceName(requestContext, createOptions.Namespace, defaultNamespace);
      else
        createOptions.Namespace = defaultNamespace;
      string str = createOptions.Namespace;
      try
      {
        requestContext.GetService<IVssRegistryService>();
        NamespacePoolScopedSettings poolScopedSettings = ServiceBusSettingsHelper.GetNamespacePoolScopedSettings(requestContext, str, messageBusName);
        string topicName = ServiceBusSettingsHelper.CreateTopicName(messageBusName, poolScopedSettings);
        requestContext.TraceAlways(1005105, TraceLevel.Info, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "CreatePublisher: Creating Publisher " + topicName + " on Namespaces: " + string.Join(";", namespacePool));
        foreach (string ns in namespacePool)
        {
          createOptions.Namespace = ns;
          TopicDescription topicDescription = ServiceBusHighAvailabilityManagementService.CreateTopicDescription(requestContext, poolScopedSettings, createOptions, topicName);
          try
          {
            this.EnsureTopicCreated(requestContext, messageBusName, topicDescription, ns, createOptions.DeleteIfExists);
          }
          catch (MessageBusPublisherAlreadyExistsException ex)
          {
            requestContext.TraceException(45917789, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), (Exception) ex);
          }
        }
        ServiceBusSettingsHelper.RegisterPublisherSettings(requestContext, messageBusName, createOptions, str, topicName);
      }
      finally
      {
        requestContext.TraceLeave(1005220, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (CreatePublisher));
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
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      string subscriberFilter = ServiceBusSettingsHelper.GetSubscriberFilter(requestContext, messageBusName);
      return this.CreateSubscriber(requestContext, messageBusName, subscriptionName, subscriberFilter, logger);
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
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, messageBusName);
      string[] namespacePoolList;
      if (ServiceBusSettingsHelper.TryGetNamespacePool(requestContext, namespaceName, out namespacePoolList))
        return this.CreateSubscriber(requestContext, namespaceName, namespacePoolList, messageBusName, subscriptionName, filterExpression, logger, isTransient);
      requestContext.Trace(97186105, TraceLevel.Error, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "CreateSubscriber: High Availability Namespace Pool does not exist");
      throw new MessageBusConfigurationException("CreateSubscriber: High Availability Namespace Pool does not exist");
    }

    internal MessageBusSubscriptionInfo CreateSubscriber(
      IVssRequestContext requestContext,
      string namespacePoolName,
      string[] namespacePool,
      string messageBusName,
      string subscriptionName,
      string filterExpression,
      ITFLogger logger,
      bool isTransient = false)
    {
      return this.ExecuteOnTopicWithRetries(requestContext, namespacePoolName, namespacePool, messageBusName, subscriptionName, logger, (Action<string, MessageBusSubscriptionInfo, NamespacePoolScopedSettings>) ((topicName, subscriptionInfo, namespacePoolScopedSettings) =>
      {
        string subscriptionName1 = ServiceBusSubscribeHelper.GetInternalSubscriptionName(requestContext, subscriptionInfo, subscriptionName, namespacePoolScopedSettings.PrefixMachineName);
        requestContext.TraceAlways(1005106, TraceLevel.Info, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "CreateSubscriber: Creating Subscriber " + subscriptionName1 + " on topic " + topicName + " on Namespaces: " + string.Join(";", namespacePool));
        SqlFilter sqlFilter = ServiceBusSubscribeHelper.GetSqlFilter(filterExpression, messageBusName, namespacePoolScopedSettings.PrefixMachineName);
        logger.Info("CreateSubscriber: Creating Subscriber " + subscriptionName1 + " on topic " + topicName + " on Namespaces: " + string.Join(";", namespacePool) + " with filter " + filterExpression);
        foreach (string str in namespacePool)
        {
          SubscriptionDescription subscriptionDescription = ServiceBusSubscribeHelper.CreateSubscriptionDescription(requestContext, str, messageBusName, topicName, subscriptionName1);
          this.EnsureSubscriptionCreated(requestContext, messageBusName, str, subscriptionDescription, (Microsoft.ServiceBus.Messaging.Filter) sqlFilter, 5, new TimeSpan?());
        }
        ServiceBusSettingsHelper.RegisterSubscriberSettings(requestContext, subscriptionInfo, namespacePoolName, topicName, subscriptionName1, isTransient);
      }));
    }

    internal MessageBusSubscriptionInfo ExecuteOnTopicWithRetries(
      IVssRequestContext requestContext,
      string namespacePoolName,
      string[] namespacePool,
      string messageBusName,
      string subscriptionName,
      ITFLogger logger,
      Action<string, MessageBusSubscriptionInfo, NamespacePoolScopedSettings> action)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(messageBusName, nameof (messageBusName));
      ArgumentUtility.CheckForNull<string>(subscriptionName, nameof (subscriptionName));
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      requestContext.TraceEnter(1005241, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "CreateSubscriber");
      int failedAttempts = 0;
      bool isForwarded = ServiceBusSettingsHelper.IsForwardedTopic(requestContext, messageBusName);
      if (isForwarded)
      {
        requestContext.TraceAlways(97186108, TraceLevel.Error, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "CreateSubscriber: High Availability Does not support Forwarded Topics");
        throw new MessageBusConfigurationException("CreateSubscriber: High Availability Does not support Forwarded Topics");
      }
label_4:
      try
      {
        requestContext.CheckDeploymentRequestContext();
        ServiceBusSubscribeHelper.ValidateSubscriptionName(subscriptionName);
        requestContext.GetService<IVssRegistryService>();
        NamespacePoolScopedSettings poolScopedSettings = ServiceBusSettingsHelper.GetNamespacePoolScopedSettings(requestContext, namespacePoolName, messageBusName);
        MessageBusSubscriptionInfo subscriptionInfo = new MessageBusSubscriptionInfo()
        {
          MessageBusName = messageBusName,
          SubscriptionName = subscriptionName,
          Namespace = namespacePoolName,
          NamespacePoolName = namespacePoolName,
          MaxDeliveryCount = 10
        };
        string subscriberTopicName = ServiceBusSubscribeHelper.GetOrCreateSubscriberTopicName(requestContext, messageBusName, subscriptionInfo, poolScopedSettings.PrefixMachineName, poolScopedSettings.HostnamePrefix);
        foreach (string ns in namespacePool)
        {
          NamespaceManager namespaceManager = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName, ns).GetNamespaceManager();
          this.m_ManagerHelper.VerifyTopicCreated(requestContext, namespaceManager, subscriberTopicName);
        }
        action(subscriberTopicName, subscriptionInfo, poolScopedSettings);
        return subscriptionInfo;
      }
      catch (ServerBusyException ex) when (ex.Detail.ErrorCode == 50009)
      {
        if (ServiceBusSubscribeHelper.HandleCreateSubscriberThrottlingException(requestContext, ex, ref failedAttempts))
          throw;
        else
          goto label_4;
      }
      catch (Microsoft.ServiceBus.Messaging.QuotaExceededException ex)
      {
        if (ServiceBusSubscribeHelper.HandleCreateSubscriberQuotaExceededException(requestContext, ex, isForwarded, ref failedAttempts))
          throw;
        else
          goto label_4;
      }
      finally
      {
        requestContext.TraceLeave(1005260, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "CreateSubscriber");
      }
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
      requestContext.TraceEnter(1005221, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (DeletePublisher));
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, messageBusName);
      string[] namespacePoolList;
      if (ServiceBusSettingsHelper.TryGetNamespacePool(requestContext, namespaceName, out namespacePoolList))
      {
        this.DeletePublisher(requestContext, messageBusName, namespaceName, namespacePoolList);
      }
      else
      {
        requestContext.Trace(97186105, TraceLevel.Error, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "DeletePublisher: High Availability Namespace Pool does not exist");
        throw new MessageBusConfigurationException("DeletePublisher: High Availability Namespace Pool does not exist");
      }
    }

    internal void DeletePublisher(
      IVssRequestContext requestContext,
      string messageBusName,
      string namespacePoolName,
      string[] namespacePool)
    {
      try
      {
        string publisherRegistryRoot = ServiceBusSettingsHelper.GetPublisherRegistryRoot(messageBusName);
        this.m_ConnectionManager.RemovePublishConnection(requestContext, publisherRegistryRoot);
        NamespacePoolScopedSettings namespacePoolScopedSettings = ServiceBusSettingsHelper.GetNamespacePoolScopedSettings(requestContext, namespacePoolName, messageBusName);
        string topicName;
        if (!ServiceBusSettingsHelper.TryGetPublisherTopicName(requestContext, messageBusName, out topicName))
          topicName = ServiceBusSettingsHelper.CreateTopicName(messageBusName, namespacePoolScopedSettings);
        requestContext.TraceAlways(1005107, TraceLevel.Info, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "DeletePublisher: Deleting Publisher " + topicName + " on Namespaces: " + string.Join(";", namespacePool));
        foreach (string ns in namespacePool)
        {
          NamespaceManager manager = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName, ns).GetNamespaceManager();
          using (PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
          {
            Action Run = (Action) (() => ServiceBusManagerHelper.DeleteTopic(requestContext, manager, namespacePoolScopedSettings.PrefixMachineName, topicName));
            CommandSetter commandsetter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusNamespace.DeletePublisher." + ns)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(manager.Settings.OperationTimeout));
            ServiceBusRetryHelper.ExecuteWithRetries((Action) (() => new CommandService(requestContext, commandsetter, Run).Execute()));
          }
        }
        ServiceBusSettingsHelper.DeletePublisherSettings(requestContext, publisherRegistryRoot);
      }
      finally
      {
        requestContext.TraceLeave(1005240, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (DeletePublisher));
      }
    }

    public void DeleteSubscriber(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MessageBusSubscriptionInfo>(subscription, nameof (subscription));
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, subscription.MessageBusName);
      string[] namespacePoolList;
      if (ServiceBusSettingsHelper.TryGetNamespacePool(requestContext, namespaceName, out namespacePoolList))
      {
        this.DeleteSubscriber(requestContext, subscription, namespaceName, namespacePoolList);
      }
      else
      {
        requestContext.Trace(97186105, TraceLevel.Error, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "DeleteSubscriber: High Availability Namespace Pool does not exist");
        throw new MessageBusConfigurationException("DeleteSubscriber: High Availability Namespace Pool does not exist");
      }
    }

    internal void DeleteSubscriber(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      string namespacePoolName,
      string[] namespacePoolList)
    {
      requestContext.TraceEnter(1005261, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (DeleteSubscriber));
      try
      {
        requestContext.GetService<IVssRegistryService>();
        NamespacePoolScopedSettings poolScopedSettings = ServiceBusSettingsHelper.GetNamespacePoolScopedSettings(requestContext, namespacePoolName, subscription.MessageBusName);
        this.m_ConnectionManager.RemoveSubscribeConnection(requestContext, subscription);
        requestContext.TraceAlways(1005108, TraceLevel.Info, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "DeleteSubcriber: Deleting Subscriber " + subscription.SubscriptionName + " on Topic " + subscription.MessageBusName + " on Namespaces: " + string.Join(";", namespacePoolList));
        foreach (string namespacePool in namespacePoolList)
        {
          NamespaceManager namespaceManager = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, subscription.MessageBusName, namespacePool).GetNamespaceManager();
          string topicName;
          if (!ServiceBusSettingsHelper.TryGetSubscriberTopicName(requestContext, subscription, out topicName))
            topicName = ServiceBusSettingsHelper.CreateTopicName(subscription.MessageBusName, poolScopedSettings);
          this.m_ManagerHelper.DeleteSubscriptionWithRetry(requestContext, namespaceManager, subscription, topicName);
        }
        ServiceBusSettingsHelper.DeleteSubscriberSettings(requestContext, subscription);
      }
      finally
      {
        requestContext.TraceLeave(1005280, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (DeleteSubscriber));
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

    public void RegisterHighAvailabilityNamespacePool(
      IVssRequestContext requestContext,
      string namepacePoolName,
      string namespacePoolList,
      bool isGlobal,
      ITFLogger logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(namepacePoolName, nameof (namepacePoolName));
      ArgumentUtility.CheckStringForNullOrEmpty(namespacePoolList, nameof (namespacePoolList));
      logger = logger ?? (ITFLogger) new NullLogger();
      ServiceBusSettingsHelper.RegisterHighAvailabilityNamespacePool(requestContext, namepacePoolName, namespacePoolList, isGlobal);
    }

    public void RegisterHighAvailabilityPublisher(
      IVssRequestContext requestContext,
      string namepacePoolName,
      string primaryNamespace,
      string secondaryNamespace,
      bool isGlobal,
      ITFLogger logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(namepacePoolName, nameof (namepacePoolName));
      ArgumentUtility.CheckStringForNullOrEmpty(primaryNamespace, nameof (primaryNamespace));
      ArgumentUtility.CheckStringForNullOrEmpty(secondaryNamespace, nameof (secondaryNamespace));
      logger = logger ?? (ITFLogger) new NullLogger();
      ServiceBusSettingsHelper.RegisterHighAvailabilityPublisher(requestContext, namepacePoolName, primaryNamespace, secondaryNamespace, isGlobal);
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

    internal void Subscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      Action<IVssRequestContext, IMessage> action,
      TeamFoundationHostType acceptedHostTypes,
      Action<Exception, string, IMessage> exceptionNotification,
      bool invokeActionWithNoMessage,
      string namespacePoolName,
      string[] namespacePool)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MessageBusSubscriptionInfo>(subscription, nameof (subscription));
      ArgumentUtility.CheckForNull<Action<IVssRequestContext, IMessage>>(action, nameof (action));
      if ((acceptedHostTypes & ~TeamFoundationHostType.All) != TeamFoundationHostType.Unknown)
        throw new UnexpectedHostTypeException(acceptedHostTypes);
      requestContext.TraceEnter(1005180, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (Subscribe));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        subscription.NamespacePoolName = namespacePoolName;
        requestContext.TraceAlways(1005109, TraceLevel.Info, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "Subscribe: Subscribing " + subscription.SubscriptionName + " on Topic " + subscription.MessageBusName + " on Namespaces: " + string.Join(";", namespacePool));
        foreach (string ns in namespacePool)
        {
          MessageBusSubscriptionInfo subscriptionInfo = new MessageBusSubscriptionInfo(subscription);
          subscriptionInfo.Namespace = ns;
          MessageReceiverSettings receiverSettings;
          if (!ServiceBusSettingsHelper.TryGetSubscriptionRegistryEntries(requestContext, subscriptionInfo, ns, out receiverSettings))
            throw new MessageBusNotFoundException(subscriptionInfo.MessageBusName);
          using (requestContext.Lock(ServiceBusLockHelper.GetSubscriberLockName(requestContext, subscriptionInfo)))
          {
            if (this.m_ConnectionManager.DoesSubscriberConnectionExist(subscriptionInfo))
              throw new MessageBusAlreadySubscribingException(subscriptionInfo.MessageBusName, subscriptionInfo.SubscriptionName);
            ServiceBusSubscribeConnection connInfo = new ServiceBusSubscribeConnection(requestContext, (IMessageBusManagementService) this, subscriptionInfo, requestContext.ServiceHost, action, exceptionNotification, acceptedHostTypes, invokeActionWithNoMessage, receiverSettings, this.m_Logger);
            this.m_ConnectionManager.AddSubscribeConnection(requestContext, subscriptionInfo, connInfo);
            connInfo.Subscribe();
          }
        }
        this.m_ConnectionManager.AddHighAvailabilitySubscribeEntry(requestContext, subscription);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusTotalSubscribers", subscription.MessageBusName).Increment();
      }
      finally
      {
        requestContext.TraceLeave(1005200, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (Subscribe));
      }
    }

    internal void Unsubscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      string[] namespacePool)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MessageBusSubscriptionInfo>(subscription, nameof (subscription));
      requestContext.TraceEnter(1005210, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (Unsubscribe));
      foreach (string str in namespacePool)
      {
        try
        {
          this.m_ConnectionManager.RemoveSubscribeConnection(requestContext, new MessageBusSubscriptionInfo(subscription)
          {
            Namespace = str
          });
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusTotalSubscribers", subscription.SubscriptionName).Decrement();
        }
        catch
        {
          requestContext.TraceAlways(1005119, TraceLevel.Warning, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "Unsubscribe: Failed to remove connection subscription " + subscription.SubscriptionName + " on Topic " + subscription.MessageBusName + " on Namespaces: " + string.Join(";", namespacePool));
        }
      }
      this.m_ConnectionManager.RemoveHighAvailabilitySubscribeEntry(requestContext, subscription);
      requestContext.TraceLeave(1005220, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (Unsubscribe));
    }

    internal void Publish(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusMessage[] messages,
      string namespacePoolName,
      string primaryNamespace,
      string secondaryNamespace,
      bool throwOnMissingPublisher = true)
    {
      requestContext.TraceEnter(1005150, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (Publish));
      try
      {
        bool? nullable = new bool?();
        Stopwatch stopwatch = Stopwatch.StartNew();
        string serviceBusNamespace = (string) null;
        string layer = messageBusName;
        try
        {
          IServiceBusPublishConnection publishConnection = this.m_ConnectionManager.CreateServiceBusHighAvailabilityPublishConnection(requestContext, primaryNamespace, secondaryNamespace, messageBusName);
          if (publishConnection == null)
            return;
          publishConnection.Publish(requestContext, messageBusName, messages);
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
            this.m_Logger.LogMessageProcessing(requestContext, serviceBusNamespace, messageBusName, nullable.Value);
            if (nullable.Value)
            {
              VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPublishedMessagesTotal", messageBusName).IncrementBy((long) messages.Length);
              VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPublishedMessagesPerSec", messageBusName).IncrementBy((long) messages.Length);
            }
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAveragePublishTime", messageBusName).IncrementMilliseconds(stopwatch.ElapsedMilliseconds);
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAveragePublishTimeBase", messageBusName).IncrementBy((long) messages.Length);
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(1005170, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (Publish));
      }
    }

    internal async Task PublishAsync(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusMessage[] messages,
      string namespacePoolName,
      string primaryNamespace,
      string secondaryNamespace,
      bool throwOnMissingPublisher = true)
    {
      requestContext.TraceEnter(1005150, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (PublishAsync));
      try
      {
        bool? success = new bool?();
        Stopwatch watch = Stopwatch.StartNew();
        string ns = (string) null;
        string layer = messageBusName;
        try
        {
          IServiceBusPublishConnection publishConnection = this.m_ConnectionManager.CreateServiceBusHighAvailabilityPublishConnection(requestContext, primaryNamespace, secondaryNamespace, messageBusName);
          if (publishConnection != null)
          {
            await publishConnection.PublishAsync(requestContext, messageBusName, messages);
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
        requestContext.TraceLeave(1005170, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), nameof (PublishAsync));
      }
    }

    private static TopicDescription CreateTopicDescription(
      IVssRequestContext requestContext,
      NamespacePoolScopedSettings namespacePoolScopedSettings,
      MessageBusPublisherCreateOptions createOptions,
      string topicName)
    {
      TopicDescription topicDescription = new TopicDescription(topicName)
      {
        MaxSizeInMegabytes = (long) (namespacePoolScopedSettings.TopicMaxSize * 1024),
        EnableExpress = createOptions.EnableExpress
      };
      topicDescription.EnablePartitioning = !createOptions.EnablePartitioning.HasValue ? namespacePoolScopedSettings.EnablePartitioning : createOptions.EnablePartitioning.Value;
      if (!namespacePoolScopedSettings.IsProductionEnvironment)
        topicDescription.AutoDeleteOnIdle = namespacePoolScopedSettings.AutoDeleteOnIdle;
      return topicDescription;
    }

    public void UpdateSubscriptionFilter(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      string newFilterValue,
      bool isTransient = false,
      ITFLogger logger = null)
    {
      string namespaceName = ServiceBusSettingsHelper.GetNamespaceName(requestContext, messageBusName);
      string[] namespacePoolList;
      if (ServiceBusSettingsHelper.TryGetNamespacePool(requestContext, namespaceName, out namespacePoolList))
      {
        this.UpdateFilter(requestContext, namespaceName, namespacePoolList, messageBusName, subscriptionName, newFilterValue, logger, isTransient);
      }
      else
      {
        requestContext.Trace(97186105, TraceLevel.Error, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "UpdateFilter: High Availability Namespace Pool does not exist");
        throw new MessageBusConfigurationException("UpdateFilter: High Availability Namespace Pool does not exist");
      }
    }

    public void UpdateFilter(
      IVssRequestContext requestContext,
      string namespacePoolName,
      string[] namespacePool,
      string messageBusName,
      string subscriptionName,
      string filterExpression,
      ITFLogger logger,
      bool isTransient = false)
    {
      this.ExecuteOnTopicWithRetries(requestContext, namespacePoolName, namespacePool, messageBusName, subscriptionName, logger, (Action<string, MessageBusSubscriptionInfo, NamespacePoolScopedSettings>) ((topicName, subscriptionInfo, namespacePoolScopedSettings) =>
      {
        string subscriptionName1 = ServiceBusSubscribeHelper.GetInternalSubscriptionName(requestContext, subscriptionInfo, subscriptionName, namespacePoolScopedSettings.PrefixMachineName);
        requestContext.TraceAlways(1005118, TraceLevel.Info, "ServiceBus", nameof (ServiceBusHighAvailabilityManagementService), "UpdateFilter: Creating Subscriber " + subscriptionName1 + " on topic " + topicName + " on Namespaces: " + string.Join(";", namespacePool));
        SqlFilter sqlFilter = ServiceBusSubscribeHelper.GetSqlFilter(filterExpression, messageBusName, namespacePoolScopedSettings.PrefixMachineName);
        logger.Info("CreateSubscriber: Creating Subscriber " + subscriptionName1 + " on topic " + topicName + " on Namespaces: " + string.Join(";", namespacePool) + " with filter " + filterExpression);
        foreach (string namespaceName in namespacePool)
        {
          ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName);
          SubscriptionDescription subscriptionDescription = ServiceBusSubscribeHelper.CreateSubscriptionDescription(requestContext, namespaceName, messageBusName, topicName, subscriptionName1);
          if (!this.m_ManagerHelper.SubscriptionExists(requestContext, messageBusName, namespaceName, subscriptionDescription))
          {
            logger.Info("UpdateFilter: Subscription " + topicName + "|" + subscriptionName + " Does not exist, creating it with default filter " + filterExpression);
            this.EnsureSubscriptionCreated(requestContext, messageBusName, string.Empty, subscriptionDescription, (Microsoft.ServiceBus.Messaging.Filter) sqlFilter, 5, new TimeSpan?());
            subscriptionInfo.MaxDeliveryCount = subscriptionDescription.MaxDeliveryCount;
            ServiceBusSettingsHelper.RegisterSubscriberSettings(requestContext, subscriptionInfo, namespaceName, topicName, subscriptionName1, isTransient);
          }
          else
          {
            logger.Info("UpdateFilter: Subscription " + topicName + "|" + subscriptionName + " exist, updating default filter to " + filterExpression);
            ServiceBusClientHelper.UpdateFilterAsync(requestContext, messageBusName, namespaceName, topicName, subscriptionName, filterExpression, logger).GetAwaiter().GetResult();
          }
        }
      }));
    }
  }
}
