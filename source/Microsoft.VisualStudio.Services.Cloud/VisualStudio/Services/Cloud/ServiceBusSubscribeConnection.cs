// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusSubscribeConnection
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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusSubscribeConnection : IDisposable
  {
    private IVssServiceHost m_host;
    private MessageReceiver m_receiver;
    private MessageSender m_sender;
    public System.Action<IVssRequestContext, IMessage> Action;
    public System.Action<Exception, string, IMessage> m_exceptionNotification;
    private object m_lock = new object();
    private MessageBusSubscriptionInfo m_subscription;
    private bool m_refresh;
    private IMessageBusManagementService m_provider;
    private TeamFoundationHostType m_acceptedHostTypes;
    private bool m_isDisposing;
    private const string s_Layer = "ServiceBusSubscribeConnection";
    private static readonly TimeSpan s_minBackOff = TimeSpan.FromMilliseconds(100.0);
    private static readonly TimeSpan s_maxBackOff = TimeSpan.FromSeconds(30.0);
    private static readonly TimeSpan s_deltaBackOff = TimeSpan.FromSeconds(1.0);
    private int m_attempt;
    private bool m_invokeActionWithNoMessage;
    private Task m_receiveTask;
    private RegistryQuery m_allTopicSettings;
    private RegistryQuery m_maxDeliveryRetryCount;
    private RegistryQuery m_deliveryRetryMinBackoff;
    private RegistryQuery m_deliveryRetryMaxBackoff;
    private RegistryQuery m_deliveryRetryDeltaBackoff;
    private ServiceBusLogger m_logger;
    private const int c_defaultDeliveryMaxRetryCount = 0;
    private const int c_defaultDeliveryRetryMinBackoff = 60;
    private const int c_defaultDeliveryRetryMaxBackoff = 1800;
    private const int c_defaultDeliveryRetryDeltaBackoff = 60;
    private MessageReceiverSettings m_settings;
    private long m_iteration;
    private const string s_Area = "ServiceBus";
    private Guid m_previousActivityId;

    internal string ExtensionType { get; private set; }

    internal string TopicName => this.m_settings.TopicName;

    internal DateTime StartTimeOfInProcessExecution { get; private set; }

    public ServiceBusSubscribeConnection(
      IVssRequestContext requestContext,
      IMessageBusManagementService provider,
      MessageBusSubscriptionInfo subscription,
      IVssServiceHost host,
      System.Action<IVssRequestContext, IMessage> action,
      System.Action<Exception, string, IMessage> exceptionNotification,
      TeamFoundationHostType acceptedHostTypes,
      bool invokeActionWithNoMessage,
      MessageReceiverSettings settings,
      ServiceBusLogger logger)
    {
      this.m_settings = settings;
      this.m_provider = provider;
      this.m_subscription = subscription;
      this.m_logger = logger;
      this.CreateReceiverAndSender();
      this.m_host = host;
      this.Action = action;
      this.m_exceptionNotification = exceptionNotification;
      this.m_invokeActionWithNoMessage = invokeActionWithNoMessage;
      this.m_acceptedHostTypes = acceptedHostTypes;
      this.ExtensionType = this.Action.GetMethodInfo().DeclaringType.FullName;
      string topicRegistryRoot = ServiceBusSettingsHelper.GetTopicRegistryRoot(this.m_subscription.MessageBusName);
      this.m_allTopicSettings = (RegistryQuery) (topicRegistryRoot + "/...");
      this.m_maxDeliveryRetryCount = (RegistryQuery) (topicRegistryRoot + "/MaxDeliveryRetryCount");
      this.m_deliveryRetryMinBackoff = (RegistryQuery) (topicRegistryRoot + "/DeliveryRetryMinBackoff");
      this.m_deliveryRetryMaxBackoff = (RegistryQuery) (topicRegistryRoot + "/DeliveryRetryMaxBackoff");
      this.m_deliveryRetryDeltaBackoff = (RegistryQuery) (topicRegistryRoot + "/DeliveryRetryDeltaBackoff");
    }

    private void CreateReceiverAndSender()
    {
      MessagingFactory messagingFactory = ServiceBusHelper.GetMessagingFactory(this.m_settings.Uri, this.m_settings.Credentials, this.m_settings.BatchFlushInterval, this.m_settings.TransportType);
      this.m_receiver = messagingFactory.CreateMessageReceiver(string.Format("{0}/{1}/{2}", (object) this.m_settings.TopicName, (object) "subscriptions", (object) this.m_settings.SubscriberName));
      if (this.m_settings.PrefetchCount > 0)
        this.m_receiver.PrefetchCount = this.m_settings.PrefetchCount;
      this.m_sender = messagingFactory.CreateMessageSender(this.m_settings.TopicName);
    }

    public void Subscribe() => this.m_receiveTask = this.ReceiveAsync();

    public void ClearSubscriber() => ServiceBusRetryHelper.ExecuteWithRetries((System.Action) (() =>
    {
      int messageCount;
      while ((messageCount = this.m_receiver.PeekBatch(1000).Count<BrokeredMessage>()) > 0)
      {
        if (messageCount > 0)
        {
          IEnumerable<BrokeredMessage> batch = this.m_receiver.ReceiveBatch(messageCount, TimeSpan.FromMilliseconds(5000.0));
          if (batch.Count<BrokeredMessage>() > 0)
            this.m_receiver.CompleteBatch(batch.Select<BrokeredMessage, Guid>((Func<BrokeredMessage, Guid>) (x => x.LockToken)));
        }
      }
    }));

    internal void Refresh(MessageReceiverSettings newSettings)
    {
      lock (this.m_lock)
      {
        if (this.m_settings.Equals(newSettings))
          return;
        TeamFoundationTracingService.TraceRawAlwaysOn(99582163, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "Refreshing subscriber {0} due to settings change.  Old Settings: {1}, New Settings: {2}", (object) this.m_subscription, (object) this.m_settings, (object) newSettings);
        this.m_settings = newSettings;
        this.m_refresh = true;
      }
    }

    private async Task ReceiveAsync()
    {
      while (!this.m_isDisposing)
      {
        try
        {
          this.m_previousActivityId = Trace.CorrelationManager.ActivityId;
          Trace.CorrelationManager.ActivityId = Guid.Empty;
          Stopwatch watch = Stopwatch.StartNew();
          bool? success = new bool?();
          BrokeredMessage message = await this.m_receiver.ReceiveAsync().ConfigureAwait(false);
          long elapsedMilliseconds = watch.ElapsedMilliseconds;
          ++this.m_iteration;
          TeamFoundationTracingService.TraceRaw(85802426, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "Processing iteration {0} for subscriber {1}", (object) this.m_iteration, (object) this.m_subscription);
          using (IVssRequestContext systemContext = this.m_host.DeploymentServiceHost.CreateSystemContext(false))
          {
            try
            {
              if (message != null)
              {
                if (this.m_isDisposing)
                {
                  message.Abandon();
                  systemContext.TraceAlways(134217727, TraceLevel.Info, "ServiceBus", nameof (ServiceBusSubscribeConnection), string.Format("Abandoning message {0} since we are disposed", (object) message.SequenceNumber));
                  break;
                }
                VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusReceivedMessagesTotal", this.m_subscription.MessageBusName).Increment();
                VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusReceivedMessagesPerSec", this.m_subscription.MessageBusName).Increment();
                bool flag1 = systemContext.ExecutionEnvironment.IsDevFabricDeployment || systemContext.IsTracing(1005450, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName);
                bool flag2 = false;
                string str;
                if (message.Properties.TryGetValue<string>(this.m_settings.FilterKey, out str) && !string.IsNullOrEmpty(str) && str.IndexOf(this.m_settings.FilterValue, StringComparison.OrdinalIgnoreCase) < 0)
                {
                  systemContext.TraceAlways(1005452, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "Skipping message because {0} filter value {1} does not match expected {2}, Subscriber {3}", (object) this.m_settings.FilterKey, (object) str, (object) this.m_settings.FilterValue, (object) this.m_subscription);
                  flag2 = true;
                }
                try
                {
                  if (flag1)
                    systemContext.TraceAlways(1005450, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "Received message enqueued at {0}, Sequence Number {1}, Message ID {2}, Skip Message {3}, Subscriber {4}", (object) message.EnqueuedTimeUtc, (object) message.SequenceNumber, (object) message.MessageId, (object) flag2, (object) this.m_subscription);
                  if (!flag2)
                  {
                    ServiceBusMessage serviceBusMessage = new ServiceBusMessage(message);
                    if (this.m_settings.TraceDelay > 0 && message.EnqueuedTimeUtc < DateTime.UtcNow.AddSeconds((double) (-1 * this.m_settings.TraceDelay)))
                      systemContext.Trace(1005451, TraceLevel.Warning, "ServiceBus", this.m_settings.TopicName, "Received *late* message enqueued at {0}, Sequence Number {1}, Subscriber {2}", (object) message.EnqueuedTimeUtc, (object) message.SequenceNumber, (object) this.m_subscription);
                    try
                    {
                      using (IVssRequestContext appropriateRequestContext = ServiceBusHelper.GetAppropriateRequestContext(systemContext, message, this.m_subscription, this.m_acceptedHostTypes))
                      {
                        DateTime utcNow = DateTime.UtcNow;
                        Exception exception = (Exception) null;
                        try
                        {
                          appropriateRequestContext.ServiceHost.BeginRequest(appropriateRequestContext);
                          utcNow = DateTime.UtcNow;
                          this.StartTimeOfInProcessExecution = utcNow;
                          serviceBusMessage.Properties["NamespaceName"] = (object) this.m_settings.Namespace;
                          if (message.DeliveryCount > 3)
                            appropriateRequestContext.TraceAlways(1005301, TraceLevel.Info, "ServiceBus", nameof (ServiceBusSubscribeConnection), string.Format("Message delivery count exceeds 3. MessageId: {0}, DeliveryCount: {1}, Namespace: {2}, TopicName: {3}", (object) message.MessageId, (object) message.DeliveryCount, (object) this.m_settings.Namespace, (object) this.m_settings.TopicName));
                          this.Action(appropriateRequestContext, (IMessage) serviceBusMessage);
                          success = new bool?(true);
                        }
                        catch (Exception ex)
                        {
                          exception = ex;
                          throw;
                        }
                        finally
                        {
                          this.StartTimeOfInProcessExecution = DateTime.MinValue;
                          appropriateRequestContext.ServiceHost.EndRequest(appropriateRequestContext);
                          ServiceBusTracer.TraceServiceBusSubscriberActivity(appropriateRequestContext, this.m_settings.Namespace, this.m_settings.TopicName, this.ExtensionType, message.MessageId, message.ContentType, utcNow, message.Properties.GetCastedValueOrDefault<string, Guid>("SourceServiceInstanceId", Guid.Empty), message.Properties.GetCastedValueOrDefault<string, Guid>("SourceServiceInstanceType", Guid.Empty), success.GetValueOrDefault(), exception);
                        }
                      }
                    }
                    catch (HostDoesNotExistException ex)
                    {
                      systemContext.Trace(1005453, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "Could not find host {0}, subscriber {1}", (object) ex, (object) this.m_subscription);
                    }
                    catch (HostShutdownException ex)
                    {
                      systemContext.Trace(1005454, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "Host Shutdown {0}, subscriber {1}", (object) ex, (object) this.m_subscription);
                    }
                    catch (UnexpectedHostTypeException ex)
                    {
                      success = new bool?(false);
                      this.HandleException((Exception) ex, false, (IMessage) serviceBusMessage);
                    }
                  }
                  this.CompleteMessage(message);
                }
                catch (MessageLockLostException ex)
                {
                  TeamFoundationTracingService.TraceRaw(1005295, TraceLevel.Warning, "ServiceBus", this.m_settings.TopicName, "Topic: {0}, Subscription:{1}, Error:{2}", (object) this.m_settings.TopicName, (object) this.m_subscription, (object) ex);
                }
                catch (Exception ex)
                {
                  this.HandleFailedMessage(systemContext, message);
                  this.HandleException(ex, false, (IMessage) null);
                }
              }
              else
              {
                systemContext.Trace(7163845, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "ReceiveAsync returned null (no message received) for subscription {0} in {1} ms", (object) this.m_subscription, (object) elapsedMilliseconds);
                if (this.m_invokeActionWithNoMessage)
                  this.Action(systemContext, (IMessage) null);
              }
            }
            catch (Exception ex)
            {
              success = new bool?(false);
              throw;
            }
            finally
            {
              watch.Stop();
              if (message != null)
              {
                VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAvgReceiveTime", this.m_subscription.MessageBusName).IncrementMilliseconds(watch.ElapsedMilliseconds);
                VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAvgReceiveTimeBase", this.m_subscription.MessageBusName).Increment();
              }
              if (success.HasValue)
                this.m_logger.LogMessageProcessing(systemContext, this.m_settings.Namespace, this.m_settings.TopicName, success.Value, this.m_subscription.SubscriptionName);
            }
          }
          this.m_attempt = 0;
          watch = (Stopwatch) null;
          success = new bool?();
        }
        catch (Exception ex)
        {
          this.HandleException(ex, true, (IMessage) null);
        }
        finally
        {
          Trace.CorrelationManager.ActivityId = this.m_previousActivityId;
          this.RefreshIfNecessary();
        }
      }
    }

    private void CompleteMessage(BrokeredMessage message) => message.CompleteAsync().ContinueWith((System.Action<Task>) (t =>
    {
      try
      {
        long sequenceNumber = message.SequenceNumber;
        message.Dispose();
        if (t.Exception != null)
          TeamFoundationTracingService.TraceExceptionRaw(1260441917, "ServiceBus", this.m_settings.TopicName, (Exception) t.Exception);
        TeamFoundationTracingService.TraceRaw(43083109, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "Message {0}, Completion Status {1} on topic {2}, subscription {3}", (object) sequenceNumber, (object) t.Status, (object) this.m_settings.TopicName, (object) this.m_subscription);
      }
      catch (Exception ex)
      {
      }
    }));

    private void HandleFailedMessage(IVssRequestContext deploymentContext, BrokeredMessage message)
    {
      bool flag = false;
      RegistryEntryCollection registryEntryCollection = deploymentContext.GetService<IVssRegistryService>().ReadEntries(deploymentContext, this.m_allTopicSettings);
      int valueFromPath1 = registryEntryCollection.GetValueFromPath<int>(this.m_maxDeliveryRetryCount.Path, 0);
      if (valueFromPath1 > 0)
      {
        int castedValueOrDefault = message.Properties.GetCastedValueOrDefault<string, int>("CurrentDeliveryRetryCount");
        if (castedValueOrDefault < valueFromPath1)
        {
          int valueFromPath2 = registryEntryCollection.GetValueFromPath<int>(this.m_deliveryRetryMinBackoff.Path, 60);
          int valueFromPath3 = registryEntryCollection.GetValueFromPath<int>(this.m_deliveryRetryMaxBackoff.Path, 1800);
          int valueFromPath4 = registryEntryCollection.GetValueFromPath<int>(this.m_deliveryRetryDeltaBackoff.Path, 60);
          TimeSpan exponentialBackoff = BackoffTimerHelper.GetExponentialBackoff(castedValueOrDefault, TimeSpan.FromSeconds((double) valueFromPath2), TimeSpan.FromSeconds((double) valueFromPath3), TimeSpan.FromSeconds((double) valueFromPath4));
          BrokeredMessage message1 = message.Clone();
          message1.ScheduledEnqueueTimeUtc = DateTime.UtcNow.Add(exponentialBackoff);
          message1.Properties[this.m_settings.FilterKey] = (object) this.m_settings.FilterValue;
          int num;
          message1.Properties["CurrentDeliveryRetryCount"] = (object) (num = castedValueOrDefault + 1);
          this.m_sender.Send(message1);
          flag = true;
          deploymentContext.TraceAlways(1005296, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "Successfully scheduled the message to be enqueued at {0}. Current retry count: {1}. Max retry count: {2}. Topic: {3}. Subscription: {4}. Message: {5}.", (object) message1.ScheduledEnqueueTimeUtc, (object) num, (object) valueFromPath1, (object) this.m_settings.TopicName, (object) this.m_subscription, (object) message);
        }
        else
          deploymentContext.TraceAlways(1005297, TraceLevel.Error, "ServiceBus", this.m_settings.TopicName, "Did not requeue the message because the retry limit ({0}) was reached. Topic: {1}. Subscription: {2}. Message: {3}.", (object) valueFromPath1, (object) this.m_settings.TopicName, (object) this.m_subscription, (object) message);
      }
      else
        deploymentContext.TraceAlways(1005298, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "The message was not retried. Topic: {0}. Subscription: {1}. Message: {2}.", (object) this.m_settings.TopicName, (object) this.m_subscription, (object) message);
      if (flag)
      {
        this.CompleteMessage(message);
      }
      else
      {
        message.Abandon();
        deploymentContext.Trace(1005299, TraceLevel.Error, "ServiceBus", this.m_settings.TopicName, "Abandoned Message after it hit the Azure Max Delivery Count:{0}. Topic: {1}. Subscription: {2}. MessageId: {3}. EnqueuedSequenceNumber: {4} ", (object) this.m_subscription.MaxDeliveryCount, (object) this.m_settings.TopicName, (object) this.m_subscription, (object) message.MessageId, (object) message.EnqueuedSequenceNumber);
      }
    }

    private void RefreshIfNecessary()
    {
      try
      {
        if (!this.m_refresh)
          return;
        lock (this.m_lock)
        {
          if (this.m_isDisposing || !this.m_refresh)
            return;
          TeamFoundationTracingService.TraceRawAlwaysOn(99582164, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "Refreshing subscriber {0}", (object) this.m_subscription);
          this.m_receiver.Abort();
          this.m_sender.Abort();
          this.CreateReceiverAndSender();
          this.m_refresh = false;
        }
      }
      catch (Exception ex)
      {
        this.HandleException(ex, true, (IMessage) null);
      }
    }

    private void HandleException(Exception ex, bool sleep, IMessage message)
    {
      if (this.m_isDisposing)
        return;
      TeamFoundationTracingService.TraceRaw(1005300, TraceLevel.Error, "ServiceBus", this.m_settings.TopicName, "Hit error while receiving message, subscriber {0}, exception {1}", (object) this.m_subscription, (object) ex);
      if (ex is MessagingEntityNotFoundException)
      {
        ex = (Exception) new MessageBusSubscriberNotFoundException(FrameworkResources.MessageBusSubscriberNotFoundException((object) this.m_subscription.SubscriptionName, (object) this.m_subscription.MessageBusName));
        using (IVssRequestContext systemContext = this.m_host.DeploymentServiceHost.CreateSystemContext(false))
          this.CreateSubscriber(systemContext, this.m_subscription.Namespace, this.m_subscription.MessageBusName, this.m_subscription.SubscriptionName, this.m_subscription.NamespacePoolName);
      }
      if (sleep)
        Thread.Sleep(BackoffTimerHelper.GetExponentialBackoff(this.m_attempt++, ServiceBusSubscribeConnection.s_minBackOff, ServiceBusSubscribeConnection.s_maxBackOff, ServiceBusSubscribeConnection.s_deltaBackOff));
      if (this.m_exceptionNotification == null)
        return;
      this.m_exceptionNotification(ex, this.m_settings.TopicName, message);
    }

    private void AcknowledgeCompletion(IAsyncResult result) => TeamFoundationTracingService.TraceRaw(1005295, TraceLevel.Info, "ServiceBus", this.m_settings.TopicName, "Acknowledged completion for message {0} Subscriber {1}", (object) (long) result.AsyncState, (object) this.m_subscription);

    public void Dispose()
    {
      lock (this.m_lock)
      {
        this.m_isDisposing = true;
        if (this.m_receiver != null)
          this.m_receiver.Abort();
        if (this.m_sender == null)
          return;
        this.m_sender.Abort();
      }
    }

    internal MessageBusSubscriptionInfo CreateSubscriber(
      IVssRequestContext requestContext,
      string namespaceName,
      string messageBusName,
      string subscriptionName,
      string namespacePoolName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(messageBusName, nameof (messageBusName));
      ArgumentUtility.CheckForNull<string>(subscriptionName, nameof (subscriptionName));
      requestContext.TraceEnter(1005241, "ServiceBus", nameof (ServiceBusSubscribeConnection), nameof (CreateSubscriber));
      int failedAttempts = 0;
      bool isForwarded = false;
      ServiceBusManagerHelper busManagerHelper = new ServiceBusManagerHelper(nameof (ServiceBusSubscribeConnection));
      string subscriberFilter = ServiceBusSettingsHelper.GetSubscriberFilter(requestContext, messageBusName);
label_1:
      try
      {
        requestContext.CheckDeploymentRequestContext();
        ServiceBusSubscribeHelper.ValidateSubscriptionName(subscriptionName);
        requestContext.GetService<IVssRegistryService>();
        NamespaceManagerSettings namespaceManagerSettings = ServiceBusSettingsHelper.GetNamespaceManagerSettings(requestContext, messageBusName, namespaceName);
        MessageBusSubscriptionInfo subscriptionInfo = new MessageBusSubscriptionInfo()
        {
          MessageBusName = messageBusName,
          SubscriptionName = subscriptionName,
          Namespace = namespaceName,
          MaxDeliveryCount = 10
        };
        bool prefixMachineName;
        string hostnamePrefix;
        string namespaceName1;
        if (!string.IsNullOrEmpty(namespacePoolName))
        {
          NamespacePoolScopedSettings poolScopedSettings = ServiceBusSettingsHelper.GetNamespacePoolScopedSettings(requestContext, namespacePoolName, messageBusName);
          prefixMachineName = poolScopedSettings.PrefixMachineName;
          hostnamePrefix = poolScopedSettings.HostnamePrefix;
          subscriptionInfo.NamespacePoolName = namespacePoolName;
          namespaceName1 = namespacePoolName;
        }
        else
        {
          prefixMachineName = namespaceManagerSettings.PrefixMachineName;
          hostnamePrefix = namespaceManagerSettings.HostnamePrefix;
          subscriptionInfo.NamespacePoolName = (string) null;
          namespaceName1 = namespaceName;
        }
        string subscriberTopicName = ServiceBusSubscribeHelper.GetOrCreateSubscriberTopicName(requestContext, messageBusName, subscriptionInfo, prefixMachineName, hostnamePrefix);
        string subscriptionName1 = ServiceBusSubscribeHelper.GetInternalSubscriptionName(requestContext, subscriptionInfo, subscriptionName, prefixMachineName);
        SqlFilter sqlFilter = ServiceBusSubscribeHelper.GetSqlFilter(subscriberFilter, messageBusName, prefixMachineName);
        NamespaceManager namespaceManager = namespaceManagerSettings.GetNamespaceManager();
        busManagerHelper.VerifyTopicCreated(requestContext, namespaceManager, subscriberTopicName);
        SubscriptionDescription subscriptionDescription = ServiceBusSubscribeHelper.CreateSubscriptionDescription(requestContext, namespaceName, messageBusName, subscriberTopicName, subscriptionName1);
        busManagerHelper.EnsureSubscriptionCreated(requestContext, messageBusName, namespaceName, subscriptionDescription, (Microsoft.ServiceBus.Messaging.Filter) sqlFilter);
        requestContext.TraceAlways(134217718, TraceLevel.Info, "ServiceBus", nameof (ServiceBusSubscribeConnection), "Creating subscription " + subscriptionName1 + " for topic " + subscriptionInfo.MessageBusName + " in Namespace " + subscriptionInfo.Namespace + " via ServiceBusSubscribeConnection");
        ServiceBusSettingsHelper.RegisterSubscriberSettings(requestContext, subscriptionInfo, namespaceName1, subscriberTopicName, subscriptionName1, false);
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
        requestContext.TraceLeave(1005260, "ServiceBus", nameof (ServiceBusSubscribeConnection), nameof (CreateSubscriber));
        this.Subscribe();
      }
    }
  }
}
