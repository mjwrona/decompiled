// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Messaging.VssMessageBus
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.SignalR.Messaging
{
  public class VssMessageBus : VssMessageBusBase, IVssMessageBus, IMessageBus
  {
    private int m_initialized;
    private readonly string[] m_topicNames;
    private readonly VssMessageBusConfiguration m_configuration;
    private readonly VssMessageBus.SubscriberConnection[] m_subscriberConnections;
    private static readonly Lazy<UTF8Encoding> s_binaryEncoding = new Lazy<UTF8Encoding>((Func<UTF8Encoding>) (() => new UTF8Encoding(false, true)));
    private const int c_maxBatchSize = 225280;

    public VssMessageBus(
      IVssDeploymentServiceHost serviceHost,
      IDependencyResolver dependencyResolver,
      VssMessageBusConfiguration configuration)
      : base(serviceHost, dependencyResolver, (ScaleoutConfiguration) configuration)
    {
      this.m_configuration = configuration;
      this.m_topicNames = configuration.GetTopicNames().ToArray<string>();
      this.m_subscriberConnections = new VssMessageBus.SubscriberConnection[this.m_configuration.TopicCount];
    }

    internal int OpenStreamCount => ((IEnumerable<VssMessageBus.SubscriberConnection>) this.m_subscriberConnections).Count<VssMessageBus.SubscriberConnection>((Func<VssMessageBus.SubscriberConnection, bool>) (x => x != null));

    protected override int StreamCount => this.m_configuration.TopicCount;

    protected override async Task Send(
      IVssRequestContext requestContext,
      int streamIndex,
      IList<Message> messages)
    {
      VssMessageBus vssMessageBus = this;
      if ((vssMessageBus.m_initialized & 1 << streamIndex) == 0)
        TeamFoundationTracingService.TraceRaw(10017006, TraceLevel.Error, vssMessageBus.Area, vssMessageBus.Layer, "Publishing {0} messages to stream {1} in an non-initialized state", (object) messages.Count, (object) streamIndex);
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.SignalR.AsyncPublisher"))
        await vssMessageBus.PublishAsync(requestContext, streamIndex, messages, new Func<IVssRequestContext, IMessageBusPublisherService, string, int, List<Message>, int, Task>(vssMessageBus.PublishBatchAsync));
      else
        await vssMessageBus.PublishAsync(requestContext, streamIndex, messages, new Func<IVssRequestContext, IMessageBusPublisherService, string, int, List<Message>, int, Task>(vssMessageBus.PublishBatch));
    }

    private async Task PublishAsync(
      IVssRequestContext requestContext,
      int streamIndex,
      IList<Message> messages,
      Func<IVssRequestContext, IMessageBusPublisherService, string, int, List<Message>, int, Task> publishBatchAsync)
    {
      VssMessageBus vssMessageBus = this;
      try
      {
        requestContext.TraceEnter(10017000, vssMessageBus.Area, vssMessageBus.Layer, nameof (PublishAsync));
        int batchNumber = 1;
        int num1 = 0;
        List<Message> currentBatch = new List<Message>();
        IMessageBusPublisherService publisherService = requestContext.GetService<IMessageBusPublisherService>();
        foreach (Message message in (IEnumerable<Message>) messages)
        {
          ArraySegment<byte> arraySegment;
          if (currentBatch.Count != 0)
          {
            int num2 = num1;
            arraySegment = message.Value;
            int count = arraySegment.Count;
            if (num2 + count >= 225280)
            {
              await publishBatchAsync(requestContext, publisherService, vssMessageBus.m_topicNames[streamIndex], batchNumber, currentBatch, num1);
              ++batchNumber;
              currentBatch.Clear();
              currentBatch.Add(message);
              arraySegment = message.Value;
              num1 = arraySegment.Count;
              goto label_7;
            }
          }
          currentBatch.Add(message);
          int num3 = num1;
          arraySegment = message.Value;
          int count1 = arraySegment.Count;
          num1 = num3 + count1;
label_7:;
        }
        if (currentBatch.Count > 0)
          await publishBatchAsync(requestContext, publisherService, vssMessageBus.m_topicNames[streamIndex], batchNumber, currentBatch, num1);
        currentBatch = (List<Message>) null;
        publisherService = (IMessageBusPublisherService) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10017008, vssMessageBus.Area, vssMessageBus.Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10017009, vssMessageBus.Area, vssMessageBus.Layer, nameof (PublishAsync));
      }
    }

    private Task PublishBatch(
      IVssRequestContext requestContext,
      IMessageBusPublisherService publisherService,
      string topicName,
      int batchNumber,
      IList<Message> messages,
      int batchContentLength)
    {
      requestContext.Trace(10017002, TraceLevel.Info, this.Area, this.Layer, "Sending message batch {0} of size {1} bytes containing {2} messages", (object) batchNumber, (object) batchContentLength, (object) messages.Count);
      try
      {
        using (PooledMemoryStream pooledMemoryStream = new PooledMemoryStream(batchContentLength))
        {
          VssMessageBus.WriteScaleoutMessageToStream(new ScaleoutMessage(messages), (Stream) pooledMemoryStream);
          pooledMemoryStream.Position = 0L;
          publisherService.Publish(requestContext, topicName, (object[]) new PooledMemoryStream[1]
          {
            pooledMemoryStream
          });
        }
      }
      catch (MessageBusMessageSizeExceededException ex)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (Message message in (IEnumerable<Message>) messages)
        {
          if (stringBuilder.Length <= 2048)
            stringBuilder.AppendLine(message.GetString());
          else
            break;
        }
        requestContext.Trace(10017003, TraceLevel.Error, this.Area, this.Layer, "Message size exceeded for batch {0} of size {1} bytes containing {2} messages. {3}", (object) batchNumber, (object) batchContentLength, (object) messages.Count, (object) stringBuilder.ToString());
      }
      return Task.CompletedTask;
    }

    private async Task PublishBatchAsync(
      IVssRequestContext requestContext,
      IMessageBusPublisherService publisherService,
      string topicName,
      int batchNumber,
      IList<Message> messages,
      int batchContentLength)
    {
      VssMessageBus vssMessageBus = this;
      requestContext.Trace(10017002, TraceLevel.Info, vssMessageBus.Area, vssMessageBus.Layer, "Sending message batch {0} of size {1} bytes containing {2} messages", (object) batchNumber, (object) batchContentLength, (object) messages.Count);
      try
      {
        using (PooledMemoryStream memoryStream = new PooledMemoryStream(batchContentLength))
        {
          VssMessageBus.WriteScaleoutMessageToStream(new ScaleoutMessage(messages), (Stream) memoryStream);
          memoryStream.Position = 0L;
          await publisherService.PublishAsync(requestContext, topicName, (object[]) new PooledMemoryStream[1]
          {
            memoryStream
          });
        }
      }
      catch (MessageBusMessageSizeExceededException ex)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (Message message in (IEnumerable<Message>) messages)
        {
          if (stringBuilder.Length <= 2048)
            stringBuilder.AppendLine(message.GetString());
          else
            break;
        }
        requestContext.Trace(10017003, TraceLevel.Error, vssMessageBus.Area, vssMessageBus.Layer, "Message size exceeded for batch {0} of size {1} bytes containing {2} messages. {3}", (object) batchNumber, (object) batchContentLength, (object) messages.Count, (object) stringBuilder.ToString());
      }
    }

    protected override void CloseStreams(IVssRequestContext systemContext)
    {
      systemContext.TraceEnter(10017050, this.Area, this.Layer, nameof (CloseStreams));
      try
      {
        for (int index = 0; index < this.m_subscriberConnections.Length; ++index)
        {
          if (this.m_subscriberConnections[index] != null)
          {
            this.m_subscriberConnections[index].Unsubscribe(systemContext);
            this.m_subscriberConnections[index] = (VssMessageBus.SubscriberConnection) null;
          }
        }
      }
      catch (Exception ex)
      {
        systemContext.TraceException(10017058, this.Area, this.Layer, ex);
      }
      finally
      {
        this.m_initialized = 0;
        systemContext.TraceLeave(10017059, this.Area, this.Layer, nameof (CloseStreams));
      }
    }

    protected override void OpenStreams(IVssRequestContext systemContext)
    {
      systemContext.TraceEnter(10017010, this.Area, this.Layer, nameof (OpenStreams));
      try
      {
        IMessageBusManagementService service = systemContext.GetService<IMessageBusManagementService>();
        for (int streamIndex = 0; streamIndex < this.m_topicNames.Length; ++streamIndex)
        {
          int num = 10;
          while (num > 0)
          {
            try
            {
              this.OpenStream(systemContext, service, streamIndex);
              this.m_initialized |= 1 << streamIndex;
              break;
            }
            catch (Exception ex)
            {
              systemContext.TraceException(10017013, TraceLevel.Error, this.Area, this.Layer, ex);
              if (--num > 0)
                Thread.Sleep(this.m_configuration.OpenRetryDelay);
            }
          }
        }
      }
      catch (Exception ex)
      {
        systemContext.TraceException(10017018, this.Area, this.Layer, ex);
        throw;
      }
      finally
      {
        systemContext.TraceLeave(10017019, this.Area, this.Layer, nameof (OpenStreams));
      }
    }

    private void OpenStream(
      IVssRequestContext systemContext,
      IMessageBusManagementService managementService,
      int streamIndex)
    {
      MessageBusPublisherCreateOptions options = new MessageBusPublisherCreateOptions()
      {
        DeleteIfExists = false,
        EnableExpress = true,
        EnablePartitioning = new bool?(false),
        Namespace = this.m_configuration.Namespace,
        SubscriptionIdleTimeout = VssMessageBusConfiguration.SubscriptionIdleTimeout,
        SubscriptionMessageTimeToLive = VssMessageBusConfiguration.SubscriptionMessageTimeToLive,
        SubscriptionPrefetchCount = 250
      };
      try
      {
        managementService.CreatePublisher(systemContext, this.m_topicNames[streamIndex], options);
      }
      catch (MessageBusPublisherAlreadyExistsException ex)
      {
        systemContext.TraceException(10017012, TraceLevel.Warning, this.Area, this.Layer, (Exception) ex);
      }
      MessageBusSubscriptionInfo transientSubscriber = managementService.CreateTransientSubscriber(systemContext, this.m_topicNames[streamIndex], Process.GetCurrentProcess().ProcessName);
      VssMessageBus.SubscriberConnection subscriberConnection = new VssMessageBus.SubscriberConnection(this, streamIndex, transientSubscriber);
      subscriberConnection.Subscribe(systemContext);
      this.m_subscriberConnections[streamIndex] = subscriberConnection;
      this.Open(streamIndex);
    }

    private static ScaleoutMessage GetScaleoutMessageFromStream(Stream stream)
    {
      using (BinaryReader binaryReader = new BinaryReader(stream))
      {
        ScaleoutMessage messageFromStream = new ScaleoutMessage();
        messageFromStream.Messages = (IList<Message>) new List<Message>();
        int num = binaryReader.ReadInt32();
        for (int index = 0; index < num; ++index)
          messageFromStream.Messages.Add(Message.ReadFrom(stream));
        messageFromStream.ServerCreationTime = new DateTime(binaryReader.ReadInt64());
        return messageFromStream;
      }
    }

    private static void WriteScaleoutMessageToStream(ScaleoutMessage message, Stream stream)
    {
      using (BinaryWriter binaryWriter = new BinaryWriter(stream, (Encoding) VssMessageBus.s_binaryEncoding.Value, true))
      {
        binaryWriter.Write(message.Messages.Count);
        for (int index = 0; index < message.Messages.Count; ++index)
          message.Messages[index].WriteTo(stream);
        binaryWriter.Write(message.ServerCreationTime.Ticks);
      }
    }

    private class SubscriberConnection
    {
      private readonly int m_topicIndex;
      private readonly VssMessageBus m_owner;
      private MessageBusSubscriptionInfo m_subscription;

      public SubscriberConnection(
        VssMessageBus owner,
        int topicIndex,
        MessageBusSubscriptionInfo subscription)
      {
        this.m_owner = owner;
        this.m_topicIndex = topicIndex;
        this.m_subscription = subscription;
      }

      public int TopicIndex => this.m_topicIndex;

      public MessageBusSubscriptionInfo Subscription => this.m_subscription;

      public void Subscribe(IVssRequestContext requestContext)
      {
        requestContext.TraceEnter(10017020, this.m_owner.Area, this.m_owner.Layer, nameof (Subscribe));
        try
        {
          requestContext.GetService<IMessageBusSubscriberService>().Subscribe(requestContext, this.m_subscription, new Action<IVssRequestContext, IMessage>(this.OnMessageReceived), TeamFoundationHostType.Deployment, new Action<Exception, string, IMessage>(this.OnException), true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10017028, this.m_owner.Area, this.m_owner.Layer, ex);
          throw;
        }
        finally
        {
          requestContext.TraceLeave(10017029, this.m_owner.Area, this.m_owner.Layer, nameof (Subscribe));
        }
      }

      public void Unsubscribe(IVssRequestContext requestContext)
      {
        requestContext.TraceEnter(10017030, this.m_owner.Area, this.m_owner.Layer, nameof (Unsubscribe));
        try
        {
          requestContext.GetService<IMessageBusSubscriberService>().Unsubscribe(requestContext, this.m_subscription);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10017038, this.m_owner.Area, this.m_owner.Layer, ex);
          throw;
        }
        finally
        {
          requestContext.TraceLeave(10017039, this.m_owner.Area, this.m_owner.Layer, nameof (Unsubscribe));
        }
      }

      private void OnException(Exception exception, string topicName, IMessage message)
      {
        using (IVssRequestContext systemContext = this.m_owner.Host.CreateSystemContext())
        {
          try
          {
            systemContext.TraceEnter(10017080, this.m_owner.Area, this.m_owner.Layer, nameof (OnException));
            if (!(exception is MessageBusSubscriberNotFoundException))
            {
              systemContext.TraceException(10017081, this.m_owner.Area, this.m_owner.Layer, exception);
            }
            else
            {
              this.m_subscription = systemContext.GetService<IMessageBusManagementService>().CreateTransientSubscriber(systemContext, this.m_subscription.MessageBusName, Process.GetCurrentProcess().ProcessName);
              systemContext.Trace(10017082, TraceLevel.Info, this.m_owner.Area, this.m_owner.Layer, "Successfully recreated subscriber {0} for topic {1}", (object) this.m_subscription.SubscriptionName, (object) this.m_subscription.MessageBusName);
            }
          }
          catch (Exception ex)
          {
            systemContext.TraceException(10017088, this.m_owner.Area, this.m_owner.Layer, ex);
          }
          finally
          {
            systemContext.TraceLeave(10017089, this.m_owner.Area, this.m_owner.Layer, nameof (OnException));
          }
        }
      }

      private void OnMessageReceived(IVssRequestContext requestContext, IMessage message)
      {
        requestContext.TraceEnter(10017040, this.m_owner.Area, this.m_owner.Layer, nameof (OnMessageReceived));
        try
        {
          if (message == null || !this.m_owner.m_configuration.SupportsClientConnections)
          {
            this.m_owner.Open(this.TopicIndex);
          }
          else
          {
            ScaleoutMessage message1;
            if (message.ContentType.Equals(typeof (ScaleoutMessage).FullName))
            {
              message1 = message.GetBody<ScaleoutMessage>();
            }
            else
            {
              using (Stream body = message.GetBody<Stream>())
                message1 = VssMessageBus.GetScaleoutMessageFromStream(body);
            }
            this.m_owner.OnReceived(this.TopicIndex, (ulong) message.SequenceNumber, message1);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10017048, this.m_owner.Area, this.m_owner.Layer, ex);
        }
        finally
        {
          requestContext.TraceLeave(10017049, this.m_owner.Area, this.m_owner.Layer, nameof (OnMessageReceived));
        }
      }
    }
  }
}
