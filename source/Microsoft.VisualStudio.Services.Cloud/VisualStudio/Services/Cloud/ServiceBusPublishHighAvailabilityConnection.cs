// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusPublishHighAvailabilityConnection
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusPublishHighAvailabilityConnection : 
    IServiceBusPublishConnection,
    IDisposable
  {
    private MessageSender m_sender;
    private int m_maxPublishSize;
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusPublishHighAvailabilityConnection";
    private HighAvailabilityMessageSender m_senderManager;
    private const int c_CommandSetterExecutionAdditionalTimeoutPerMessageInMs = 500;
    private const int c_CommandSetterExecutionTimeoutBaseInMs = 5000;
    private const int c_CommandSetterPublishConnectionExecutionTimeoutBaseInMs = 500;
    private TimeSpan c_CommandSetterCircuitBreakerDeltaBackoff = TimeSpan.FromMilliseconds(2000.0);

    public string Namespace { get; internal set; }

    public string SecondaryNamespace { get; internal set; }

    public string TopicName { get; private set; }

    public ServiceBusPublishHighAvailabilityConnection(
      IVssRequestContext requestContext,
      HighAvailabilityPublisherSettings poolSettings,
      string topicName)
    {
      this.Namespace = poolSettings.Primary.NamespaceName;
      this.SecondaryNamespace = poolSettings.Secondary.NamespaceName;
      this.TopicName = topicName;
      this.m_senderManager = new HighAvailabilityMessageSender();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int maxPublishSize = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/MaxPublishSize", 163840);
      if (maxPublishSize < 0 || maxPublishSize > 163840)
        maxPublishSize = 163840;
      TransportType transportType = service.GetValue<TransportType>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/TransportType", TransportType.Amqp);
      PublisherSettings primary = poolSettings.Primary;
      if (primary != null)
      {
        Uri resourceIdentifier = primary.UniversalResourceIdentifier;
        MessagingFactory messagingFactory = ServiceBusHelper.GetMessagingFactory(primary.UniversalResourceIdentifier, primary.MessageBusCreds, -1, transportType);
        this.m_senderManager.Primary = this.InitializeSender(requestContext, topicName, maxPublishSize, messagingFactory, this.Namespace);
      }
      PublisherSettings secondary = poolSettings.Secondary;
      if (secondary == null)
        return;
      Uri resourceIdentifier1 = secondary.UniversalResourceIdentifier;
      MessagingFactory messagingFactory1 = ServiceBusHelper.GetMessagingFactory(secondary.UniversalResourceIdentifier, secondary.MessageBusCreds, -1, transportType);
      this.m_senderManager.Secondary = this.InitializeSender(requestContext, topicName, maxPublishSize, messagingFactory1, this.SecondaryNamespace);
    }

    internal ServiceBusPublishHighAvailabilityConnection()
    {
    }

    private MessageSender InitializeSender(
      IVssRequestContext requestContext,
      string topicName,
      int maxPublishSize,
      MessagingFactory factory,
      string namespaceName)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusNamespace.PublishConnection." + namespaceName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(TimeSpan.FromMilliseconds(500.0)));
      this.m_maxPublishSize = maxPublishSize;
      return new CommandService<MessageSender>(requestContext, setter, (Func<MessageSender>) (() => factory.CreateMessageSender(topicName))).Execute();
    }

    public void Publish(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      MessageBusMessage[] messagesToPublish)
    {
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
      {
        try
        {
          Action run = (Action) (() =>
          {
            requestContext.GetService<IVssRegistryService>();
            this.PublishHelper(requestContext, messageBusIdentifier, messagesToPublish, ServiceBusSettingsHelper.GetPublishSecondaryValue(requestContext, messageBusIdentifier));
          });
          Action fallback = (Action) (() => this.PublishHelper(requestContext, messageBusIdentifier, messagesToPublish, true));
          CommandSetter publishCommandSetter = this.GetPublishCommandSetter(messagesToPublish.Length);
          new CommandService(requestContext, publishCommandSetter, run, fallback).Execute();
        }
        finally
        {
          performanceTimer.End();
          if (!requestContext.ServiceHost.IsProduction)
            requestContext.TraceAlways(44403900, TraceLevel.Info, "ServiceBus", this.Namespace + "::" + messageBusIdentifier, string.Format("{0}-{1}", (object) messagesToPublish.Length, (object) performanceTimer.Duration));
        }
      }
    }

    private void PublishHelper(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      MessageBusMessage[] messagesToPublish,
      bool publishToSecondary)
    {
      List<BrokeredMessage> messages = new List<BrokeredMessage>(messagesToPublish.Length);
      long size = -1;
      try
      {
        ServiceBusPublishHelper.TracePartitionIds(requestContext, messageBusIdentifier, messagesToPublish);
        this.PublishMessagesInternal(requestContext, messageBusIdentifier, messagesToPublish, messages, publishToSecondary);
      }
      catch (Exception ex)
      {
        string namespaceName = publishToSecondary ? this.SecondaryNamespace : this.Namespace;
        ServiceBusPublishHelper.HandlePublishException(requestContext, ex, namespaceName, messageBusIdentifier, messagesToPublish, messages, size);
      }
      finally
      {
        if (messages.Count > 0)
          ServiceBusPublishHelper.DisposeMessages(messages);
      }
    }

    private long PublishMessagesInternal(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      MessageBusMessage[] messagesToPublish,
      List<BrokeredMessage> messages,
      bool publishToSecondary)
    {
      long messageSize = 160;
      foreach (MessageBusMessage input in messagesToPublish)
      {
        BrokeredMessage brokeredMessage = ServiceBusPublishHelper.GetBrokeredMessage(input, messageBusIdentifier);
        long num = brokeredMessage.Size + 5000L + ServiceBusPublishHelper.GetHeaderSize(brokeredMessage);
        if (messages.Count > 0 && messageSize + num > (long) this.m_maxPublishSize)
        {
          this.PublishInternalIfSafe(requestContext, messageBusIdentifier, messages, publishToSecondary, messageSize);
          ServiceBusPublishHelper.DisposeMessages(messages);
          messageSize = 160L;
        }
        messageSize += num;
        messages.Add(brokeredMessage);
      }
      if (messages.Count > 0)
      {
        this.PublishInternalIfSafe(requestContext, messageBusIdentifier, messages, publishToSecondary, messageSize);
        ServiceBusPublishHelper.DisposeMessages(messages);
      }
      return messageSize;
    }

    private void PublishInternalIfSafe(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      List<BrokeredMessage> messages,
      bool publishToSecondary,
      long messageSize)
    {
      if (messageSize < 1000000L || !requestContext.IsFeatureEnabled("VisualStudio.Services.ServiceBus.IgnoreMessagesTooLargeForPublish"))
        this.PublishInternal(requestContext, messageBusIdentifier, messages, publishToSecondary);
      else
        requestContext.TraceAlways(1005115, TraceLevel.Error, "ServiceBus", nameof (ServiceBusPublishHighAvailabilityConnection), string.Format("Service bus message size over limit of 1 MB. MessageBusIdentifier: {0}, MessageCount: {1}", (object) messageBusIdentifier, (object) messages.Count));
    }

    private async Task<long> PublishMessagesInternalAsync(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      MessageBusMessage[] messagesToPublish,
      List<BrokeredMessage> messages,
      bool publishToSecondary)
    {
      long size = 160;
      MessageBusMessage[] messageBusMessageArray = messagesToPublish;
      for (int index = 0; index < messageBusMessageArray.Length; ++index)
      {
        BrokeredMessage message = ServiceBusPublishHelper.GetBrokeredMessage(messageBusMessageArray[index], messageBusIdentifier);
        long currentSize = message.Size + 5000L + ServiceBusPublishHelper.GetHeaderSize(message);
        if (messages.Count > 0 && size + currentSize > (long) this.m_maxPublishSize)
        {
          await this.PublishInternalAsync(requestContext, messageBusIdentifier, messages, publishToSecondary);
          ServiceBusPublishHelper.DisposeMessages(messages);
          size = 160L;
        }
        size += currentSize;
        messages.Add(message);
        message = (BrokeredMessage) null;
      }
      messageBusMessageArray = (MessageBusMessage[]) null;
      if (messages.Count > 0)
      {
        await this.PublishInternalAsync(requestContext, messageBusIdentifier, messages, publishToSecondary);
        ServiceBusPublishHelper.DisposeMessages(messages);
      }
      return size;
    }

    private async Task PublishAsyncHelper(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      MessageBusMessage[] messagesToPublish,
      bool publishToSecondary)
    {
      long size = 160;
      List<BrokeredMessage> messages = new List<BrokeredMessage>(messagesToPublish.Length);
      try
      {
        ServiceBusPublishHelper.TracePartitionIds(requestContext, messageBusIdentifier, messagesToPublish);
        long num = await this.PublishMessagesInternalAsync(requestContext, messageBusIdentifier, messagesToPublish, messages, publishToSecondary);
        messages = (List<BrokeredMessage>) null;
      }
      catch (Exception ex)
      {
        ServiceBusPublishHelper.HandlePublishException(requestContext, ex, publishToSecondary ? this.SecondaryNamespace : this.Namespace, messageBusIdentifier, messagesToPublish, messages, size);
        messages = (List<BrokeredMessage>) null;
      }
      finally
      {
        if (messages.Count > 0)
          ServiceBusPublishHelper.DisposeMessages(messages);
      }
    }

    public Task PublishAsync(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      MessageBusMessage[] messagesToPublish)
    {
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "ServiceBus"))
      {
        try
        {
          Func<Task> run = (Func<Task>) (async () =>
          {
            requestContext.GetService<IVssRegistryService>();
            bool publishSecondaryValue = ServiceBusSettingsHelper.GetPublishSecondaryValue(requestContext, messageBusIdentifier);
            await this.PublishAsyncHelper(requestContext, messageBusIdentifier, messagesToPublish, publishSecondaryValue);
          });
          Func<Task> fallback = (Func<Task>) (async () => await this.PublishAsyncHelper(requestContext, messageBusIdentifier, messagesToPublish, true));
          CommandSetter publishCommandSetter = this.GetPublishCommandSetter(messagesToPublish.Length);
          return new CommandServiceAsync(requestContext, publishCommandSetter, (ICommandProperties) new CommandPropertiesDefault(publishCommandSetter.CommandPropertiesDefaults), run, fallback, true).Execute();
        }
        finally
        {
          performanceTimer.End();
          if (!requestContext.ServiceHost.IsProduction)
            requestContext.TraceAlways(44403900, TraceLevel.Info, "ServiceBus", this.Namespace + "::" + messageBusIdentifier, string.Format("{0}-{1}", (object) messagesToPublish.Length, (object) performanceTimer.Duration));
        }
      }
    }

    private void PublishInternal(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      List<BrokeredMessage> messages,
      bool publishToSecondary)
    {
      bool success = false;
      string secondaryNamespace = this.Namespace;
      DateTime utcNow = DateTime.UtcNow;
      Stopwatch stopwatch = Stopwatch.StartNew();
      VssPerformanceEventSource.Log.MessageBusSendBatchStart(requestContext.UniqueIdentifier, messageBusIdentifier, messages.Count);
      try
      {
        using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
        {
          if (!publishToSecondary)
          {
            this.m_senderManager.Primary.SendBatch((IEnumerable<BrokeredMessage>) messages);
          }
          else
          {
            requestContext.TraceAlways(1005113, TraceLevel.Warning, "ServiceBus", nameof (ServiceBusPublishHighAvailabilityConnection), "Publishing message to secondary namespace " + this.SecondaryNamespace + " for topic " + messageBusIdentifier);
            this.m_senderManager.Secondary.SendBatch((IEnumerable<BrokeredMessage>) messages);
            secondaryNamespace = this.SecondaryNamespace;
          }
          success = true;
        }
      }
      finally
      {
        stopwatch.Stop();
        VssPerformanceEventSource.Log.MessageBusSendBatchStop(requestContext.UniqueIdentifier, messageBusIdentifier, messages.Count, stopwatch.ElapsedMilliseconds);
        ServiceBusTracer.TraceServiceBusPublishLog(requestContext, secondaryNamespace, this.TopicName, messages, success, utcNow, stopwatch.ElapsedMilliseconds);
      }
    }

    private async Task PublishInternalAsync(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      List<BrokeredMessage> messages,
      bool publishToSecondary)
    {
      bool success = false;
      string namespaceName = this.Namespace;
      DateTime startTime = DateTime.UtcNow;
      Stopwatch watch = Stopwatch.StartNew();
      VssPerformanceEventSource.Log.MessageBusSendBatchStart(requestContext.UniqueIdentifier, messageBusIdentifier, messages.Count);
      try
      {
        using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
        {
          if (!publishToSecondary)
          {
            await this.m_senderManager.Primary.SendBatchAsync((IEnumerable<BrokeredMessage>) messages);
          }
          else
          {
            requestContext.TraceAlways(1005113, TraceLevel.Warning, "ServiceBus", nameof (ServiceBusPublishHighAvailabilityConnection), "Publishing async message to secondary namespace " + this.SecondaryNamespace + " for topic " + messageBusIdentifier);
            await this.m_senderManager.Secondary.SendBatchAsync((IEnumerable<BrokeredMessage>) messages);
            namespaceName = this.SecondaryNamespace;
          }
          success = true;
        }
      }
      finally
      {
        watch.Stop();
        VssPerformanceEventSource.Log.MessageBusSendBatchStop(requestContext.UniqueIdentifier, messageBusIdentifier, messages.Count, watch.ElapsedMilliseconds);
        ServiceBusTracer.TraceServiceBusPublishLog(requestContext, namespaceName, this.TopicName, messages, success, startTime, watch.ElapsedMilliseconds);
      }
      namespaceName = (string) null;
      watch = (Stopwatch) null;
    }

    public void Dispose()
    {
      if (this.m_sender == null)
        return;
      this.m_sender.Close();
      this.m_sender = (MessageSender) null;
    }

    internal CommandSetter GetPublishCommandSetter(int messagesToPublishCount) => CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusNamespace.Publish." + this.Namespace)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(TimeSpan.FromMilliseconds((double) (5000 + 500 * messagesToPublishCount))).WithCircuitBreakerDeltaBackoff(this.c_CommandSetterCircuitBreakerDeltaBackoff));
  }
}
