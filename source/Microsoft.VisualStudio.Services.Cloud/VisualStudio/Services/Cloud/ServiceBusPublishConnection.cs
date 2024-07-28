// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusPublishConnection
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusPublishConnection : IServiceBusPublishConnection, IDisposable
  {
    private MessageSender m_sender;
    private int m_maxPublishSize;
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusPublishConnection";
    private const string s_useNamespaceInsteadOfTopicName = "TOPIC";

    public string Namespace { get; internal set; }

    public string TopicName { get; private set; }

    public ServiceBusPublishConnection(
      IVssRequestContext requestContext,
      string serviceBusNamespace,
      string sharedAccessSignatureToken,
      string topicName,
      int maxPublishSize)
    {
      TokenProvider signatureTokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(sharedAccessSignatureToken);
      Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceBusNamespace, string.Empty);
      this.InitializeSender(requestContext, topicName, maxPublishSize, MessagingFactory.Create((IEnumerable<Uri>) new Uri[1]
      {
        serviceUri
      }, signatureTokenProvider));
    }

    public ServiceBusPublishConnection(
      IVssRequestContext requestContext,
      string connectionString,
      string topicName,
      int maxPublishSize)
    {
      this.InitializeSender(requestContext, topicName, maxPublishSize, MessagingFactory.CreateFromConnectionString(connectionString));
    }

    public ServiceBusPublishConnection(
      IVssRequestContext requestContext,
      Uri uri,
      MessageBusCredentials messageBusCredentials,
      string serviceBusNamespace,
      string topicName)
    {
      this.Namespace = serviceBusNamespace;
      this.TopicName = topicName;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int maxPublishSize = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/MaxPublishSize", 163840);
      if (maxPublishSize < 0 || maxPublishSize > 163840)
        maxPublishSize = 163840;
      TransportType transportType = service.GetValue<TransportType>(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/TransportType", TransportType.Amqp);
      MessagingFactory messagingFactory = ServiceBusHelper.GetMessagingFactory(uri, messageBusCredentials, -1, transportType);
      this.InitializeSender(requestContext, topicName, maxPublishSize, messagingFactory);
    }

    internal ServiceBusPublishConnection()
    {
    }

    private void InitializeSender(
      IVssRequestContext requestContext,
      string topicName,
      int maxPublishSize,
      MessagingFactory factory)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusTopic.PublishConnection." + this.GetCircuitBreakerSafeName(topicName))).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(TimeSpan.FromMilliseconds(500.0)));
      this.m_sender = new CommandService<MessageSender>(requestContext, setter, (Func<MessageSender>) (() => factory.CreateMessageSender(topicName))).Execute();
      this.m_maxPublishSize = maxPublishSize;
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
            List<BrokeredMessage> messages = new List<BrokeredMessage>(messagesToPublish.Length);
            long messageSize = 160;
            try
            {
              if (requestContext.IsTracing(1005610, TraceLevel.Verbose, "ServiceBus", nameof (ServiceBusPublishConnection)))
              {
                List<string> list = ((IEnumerable<MessageBusMessage>) messagesToPublish).Select<MessageBusMessage, string>((Func<MessageBusMessage, string>) (x =>
                {
                  if (x.PartitionKey == null)
                    return "null";
                  return !(x.PartitionKey == "") ? x.PartitionKey : "empty";
                })).Distinct<string>().ToList<string>();
                requestContext.Trace(1005610, list.Count > 1 ? TraceLevel.Error : TraceLevel.Verbose, "ServiceBus", nameof (ServiceBusPublishConnection), "Messages for {0} have the following distinct partitionIds: {1}", (object) messageBusIdentifier, (object) string.Join(",", (IEnumerable<string>) list));
              }
              foreach (MessageBusMessage input in messagesToPublish)
              {
                BrokeredMessage brokeredMessage = ServiceBusPublishHelper.GetBrokeredMessage(input, messageBusIdentifier);
                long num = brokeredMessage.Size + 5000L + ServiceBusPublishHelper.GetHeaderSize(brokeredMessage);
                if (messages.Count > 0 && messageSize + num > (long) this.m_maxPublishSize)
                {
                  this.PublishInternalIfSafe(requestContext, messageBusIdentifier, messages, messageSize);
                  ServiceBusPublishHelper.DisposeMessages(messages);
                  messageSize = 160L;
                }
                messageSize += num;
                messages.Add(brokeredMessage);
              }
              if (messages.Count <= 0)
                return;
              this.PublishInternalIfSafe(requestContext, messageBusIdentifier, messages, messageSize);
              ServiceBusPublishHelper.DisposeMessages(messages);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1005608, "ServiceBus", messageBusIdentifier, ex);
              string str = string.Empty;
              if (messagesToPublish != null && messagesToPublish.Length != 0)
                str = string.Join(",", ((IEnumerable<MessageBusMessage>) messagesToPublish).Select<MessageBusMessage, string>((Func<MessageBusMessage, string>) (x => x.ContentType)));
              requestContext.Trace(1005607, TraceLevel.Error, "ServiceBus", nameof (ServiceBusPublishConnection), "Failed to send {0} messages of size:{1} for message bus:{2} on Namespace {3}. ContentTypes:{4}", (object) messagesToPublish.Length, (object) messageSize, (object) messageBusIdentifier, (object) this.Namespace, (object) str);
              ServiceBusPublishHelper.TraceSocketExceptionData(requestContext, ex);
              if (ex is MessageSizeExceededException exception2)
                throw ServiceBusHelper.HandleSizeExceededException(requestContext, exception2, messageBusIdentifier, messages);
              throw;
            }
            finally
            {
              if (messages.Count > 0)
                ServiceBusPublishHelper.DisposeMessages(messages);
            }
          });
          CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusTopic.Publish." + this.GetCircuitBreakerSafeName(messageBusIdentifier))).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(TimeSpan.FromMilliseconds((double) (5000 + 500 * messagesToPublish.Length))));
          new CommandService(requestContext, setter, run).Execute();
        }
        finally
        {
          performanceTimer.End();
          if (!requestContext.ServiceHost.IsProduction)
            requestContext.TraceAlways(44403900, TraceLevel.Info, "ServiceBus", this.Namespace + "::" + messageBusIdentifier, string.Format("{0}-{1}", (object) messagesToPublish.Length, (object) performanceTimer.Duration));
        }
      }
    }

    private void PublishInternalIfSafe(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      List<BrokeredMessage> messages,
      long messageSize)
    {
      if (messageSize < 1000000L || !requestContext.IsFeatureEnabled("VisualStudio.Services.ServiceBus.IgnoreMessagesTooLargeForPublish"))
        this.PublishInternal(requestContext, messageBusIdentifier, messages);
      else
        requestContext.TraceAlways(1005115, TraceLevel.Error, "ServiceBus", nameof (ServiceBusPublishConnection), string.Format("Service bus message size over limit of 1 MB. MessageBusIdentifier: {0}, MessageCount: {1}", (object) messageBusIdentifier, (object) messages.Count));
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
            List<BrokeredMessage> messages = new List<BrokeredMessage>(messagesToPublish.Length);
            long size = 160;
            try
            {
              if (requestContext.IsTracing(1005610, TraceLevel.Verbose, "ServiceBus", nameof (ServiceBusPublishConnection)))
              {
                List<string> list = ((IEnumerable<MessageBusMessage>) messagesToPublish).Select<MessageBusMessage, string>((Func<MessageBusMessage, string>) (x =>
                {
                  if (x.PartitionKey == null)
                    return "null";
                  return !(x.PartitionKey == "") ? x.PartitionKey : "empty";
                })).Distinct<string>().ToList<string>();
                requestContext.Trace(1005610, list.Count > 1 ? TraceLevel.Error : TraceLevel.Verbose, "ServiceBus", nameof (ServiceBusPublishConnection), "Messages for {0} have the following distinct partitionIds: {1}", (object) messageBusIdentifier, (object) string.Join(",", (IEnumerable<string>) list));
              }
              MessageBusMessage[] messageBusMessageArray = messagesToPublish;
              for (int index = 0; index < messageBusMessageArray.Length; ++index)
              {
                BrokeredMessage message = ServiceBusPublishHelper.GetBrokeredMessage(messageBusMessageArray[index], messageBusIdentifier);
                long currentSize = message.Size + 5000L + ServiceBusPublishHelper.GetHeaderSize(message);
                if (messages.Count > 0 && size + currentSize > (long) this.m_maxPublishSize)
                {
                  await this.PublishInternalAsync(requestContext, messageBusIdentifier, messages);
                  ServiceBusPublishHelper.DisposeMessages(messages);
                  size = 160L;
                }
                size += currentSize;
                messages.Add(message);
                message = (BrokeredMessage) null;
              }
              messageBusMessageArray = (MessageBusMessage[]) null;
              if (messages.Count <= 0)
              {
                messages = (List<BrokeredMessage>) null;
              }
              else
              {
                await this.PublishInternalAsync(requestContext, messageBusIdentifier, messages);
                ServiceBusPublishHelper.DisposeMessages(messages);
                messages = (List<BrokeredMessage>) null;
              }
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1005608, "ServiceBus", messageBusIdentifier, ex);
              string str = string.Empty;
              if (messagesToPublish != null && messagesToPublish.Length != 0)
                str = string.Join(",", ((IEnumerable<MessageBusMessage>) messagesToPublish).Select<MessageBusMessage, string>((Func<MessageBusMessage, string>) (x => x.ContentType)));
              requestContext.Trace(1005607, TraceLevel.Error, "ServiceBus", nameof (ServiceBusPublishConnection), "Failed to send {0} messages of size:{1} for message bus:{2} on Namespace {3}. ContentTypes:{4}", (object) messagesToPublish.Length, (object) size, (object) messageBusIdentifier, (object) this.Namespace, (object) str);
              ServiceBusPublishHelper.TraceSocketExceptionData(requestContext, ex);
              if (ex is MessageSizeExceededException exception2)
                throw ServiceBusHelper.HandleSizeExceededException(requestContext, exception2, messageBusIdentifier, messages);
              throw;
            }
            finally
            {
              if (messages.Count > 0)
                ServiceBusPublishHelper.DisposeMessages(messages);
            }
          });
          CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("ServiceBusTopic.Publish." + this.GetCircuitBreakerSafeName(messageBusIdentifier))).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(TimeSpan.FromMilliseconds((double) (5000 + 500 * messagesToPublish.Length))));
          return new CommandServiceAsync(requestContext, setter, (ICommandProperties) new CommandPropertiesDefault(setter.CommandPropertiesDefaults), run, continueOnCapturedContext: true).Execute();
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
      List<BrokeredMessage> messages)
    {
      bool success = false;
      DateTime utcNow = DateTime.UtcNow;
      Stopwatch stopwatch = Stopwatch.StartNew();
      VssPerformanceEventSource.Log.MessageBusSendBatchStart(requestContext.UniqueIdentifier, messageBusIdentifier, messages.Count);
      try
      {
        using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
        {
          this.m_sender.SendBatch((IEnumerable<BrokeredMessage>) messages);
          success = true;
        }
      }
      finally
      {
        stopwatch.Stop();
        VssPerformanceEventSource.Log.MessageBusSendBatchStop(requestContext.UniqueIdentifier, messageBusIdentifier, messages.Count, stopwatch.ElapsedMilliseconds);
        ServiceBusTracer.TraceServiceBusPublishLog(requestContext, this.Namespace, this.TopicName, messages, success, utcNow, stopwatch.ElapsedMilliseconds);
      }
    }

    private async Task PublishInternalAsync(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      List<BrokeredMessage> messages)
    {
      bool success = false;
      DateTime startTime = DateTime.UtcNow;
      Stopwatch watch = Stopwatch.StartNew();
      VssPerformanceEventSource.Log.MessageBusSendBatchStart(requestContext.UniqueIdentifier, messageBusIdentifier, messages.Count);
      try
      {
        using (requestContext.AcquireConnectionLock(ConnectionLockNameType.ServiceBus))
        {
          await this.m_sender.SendBatchAsync((IEnumerable<BrokeredMessage>) messages);
          success = true;
        }
      }
      finally
      {
        watch.Stop();
        VssPerformanceEventSource.Log.MessageBusSendBatchStop(requestContext.UniqueIdentifier, messageBusIdentifier, messages.Count, watch.ElapsedMilliseconds);
        ServiceBusTracer.TraceServiceBusPublishLog(requestContext, this.Namespace, this.TopicName, messages, success, startTime, watch.ElapsedMilliseconds);
      }
      watch = (Stopwatch) null;
    }

    public void Dispose()
    {
      if (this.m_sender == null)
        return;
      this.m_sender.Close();
      this.m_sender = (MessageSender) null;
    }

    internal string GetCircuitBreakerSafeName(string topicName) => topicName.StartsWith("TOPIC", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(this.Namespace) ? this.Namespace : topicName;
  }
}
