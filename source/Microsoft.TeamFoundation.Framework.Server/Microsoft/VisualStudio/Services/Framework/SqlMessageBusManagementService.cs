// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Framework.SqlMessageBusManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Framework
{
  internal sealed class SqlMessageBusManagementService : 
    IMessageBusManagementService,
    IVssFrameworkService
  {
    private const string s_Area = "SqlNotificationProvider";
    private const string s_Layer = "Service";
    private const string s_MessageBusPublisherLockPrefix = "SqlNotificationProvider.Service:";
    private IDictionary<string, Guid> m_publishEntries;
    private object m_publishLock = new object();
    private IDictionary<string, SqlNotificationHandler> m_subscribers;
    private object m_subscriberLock = new object();
    private int m_retries;
    private int m_retrySleepDelay;
    private int m_deliveryWarningThresholdMilliseconds;
    private int m_objectCountInfoLevel;
    private int m_objectCountWarningLevel;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1014000, "SqlNotificationProvider", "Service", nameof (ServiceStart));
      CachedRegistryService registryService = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? systemRequestContext.GetService<CachedRegistryService>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      registryService.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), RegistryHelpers.CombinePath("/Service/MessageBus/SqlNotificationMessageBus", "..."));
      this.Initialize(systemRequestContext, (IVssRegistryService) registryService);
      systemRequestContext.TraceLeave(1014009, "SqlNotificationProvider", "Service", nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1014060, "SqlNotificationProvider", "Service", nameof (ServiceEnd));
      systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      this.ClearPublishers(systemRequestContext);
      systemRequestContext.TraceLeave(1014069, "SqlNotificationProvider", "Service", nameof (ServiceEnd));
    }

    private void Initialize(IVssRequestContext requestContext, IVssRegistryService registryService)
    {
      this.m_retries = registryService.GetValue<int>(requestContext, (RegistryQuery) "/Service/MessageBus/SqlNotificationMessageBus/Retries", true, 10);
      this.m_retrySleepDelay = registryService.GetValue<int>(requestContext, (RegistryQuery) "/Service/MessageBus/SqlNotificationMessageBus/RetrySleep", true, 1000);
      this.m_deliveryWarningThresholdMilliseconds = registryService.GetValue<int>(requestContext, (RegistryQuery) "/Service/MessageBus/SqlNotificationMessageBus/DeliveryWarningThresholdMilliseconds", true, 1000);
      this.m_objectCountInfoLevel = registryService.GetValue<int>(requestContext, (RegistryQuery) "/Service/MessageBus/SqlNotificationMessageBus/ObjectCountInfoLevel", true, 1);
      this.m_objectCountWarningLevel = registryService.GetValue<int>(requestContext, (RegistryQuery) "/Service/MessageBus/SqlNotificationMessageBus/ObjectCountWarningLevel", true, 10);
      this.m_publishEntries = (IDictionary<string, Guid>) new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_subscribers = (IDictionary<string, SqlNotificationHandler>) new Dictionary<string, SqlNotificationHandler>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(1014010, "SqlNotificationProvider", "Service", nameof (OnRegistryChanged));
      try
      {
        if (changedEntries == null || !changedEntries.Any<RegistryEntry>((Func<RegistryEntry, bool>) (entry => VssStringComparer.RegistryPath.StartsWith(entry.Path, "/Service/MessageBus/SqlNotificationMessageBus/Publisher"))))
          return;
        this.ClearPublishers(requestContext);
      }
      finally
      {
        requestContext.TraceLeave(1014019, "SqlNotificationProvider", "Service", nameof (OnRegistryChanged));
      }
    }

    private void ClearPublishers(IVssRequestContext requestContext)
    {
      IDictionary<string, Guid> publishEntries;
      lock (this.m_publishLock)
      {
        publishEntries = this.m_publishEntries;
        this.m_publishEntries = (IDictionary<string, Guid>) new Dictionary<string, Guid>();
      }
      if (publishEntries == null)
        return;
      this.Resubscribe(requestContext, publishEntries);
    }

    private void Resubscribe(
      IVssRequestContext requestContext,
      IDictionary<string, Guid> previousPublishers)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, Guid>>(previousPublishers, nameof (previousPublishers));
      requestContext.TraceEnter(1014120, "SqlNotificationProvider", "Service", nameof (Resubscribe));
      try
      {
        lock (this.m_subscriberLock)
        {
          ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
          Dictionary<string, SqlNotificationHandler> dictionary = new Dictionary<string, SqlNotificationHandler>(this.m_subscribers);
          foreach (KeyValuePair<string, SqlNotificationHandler> subscriber in (IEnumerable<KeyValuePair<string, SqlNotificationHandler>>) this.m_subscribers)
          {
            string key = subscriber.Key;
            SqlNotificationHandler handler = subscriber.Value;
            bool flag = false;
            Guid eventClass;
            if (previousPublishers.TryGetValue(key, out eventClass))
            {
              try
              {
                Guid publisherEventClass = this.GetPublisherEventClass(requestContext, key);
                if (eventClass != publisherEventClass)
                {
                  service.RegisterNotification(requestContext, "Default", publisherEventClass, handler, false);
                  flag = true;
                }
              }
              catch (MessageBusNotFoundException ex)
              {
                dictionary.Remove(key);
                flag = true;
              }
              catch (Exception ex)
              {
                requestContext.TraceException(1014120, "SqlNotificationProvider", "Service", ex);
              }
              if (flag)
                service.UnregisterNotification(requestContext, "Default", eventClass, handler, true);
            }
          }
          this.m_subscribers = (IDictionary<string, SqlNotificationHandler>) dictionary;
        }
      }
      finally
      {
        requestContext.TraceLeave(1014129, "SqlNotificationProvider", "Service", nameof (Resubscribe));
      }
    }

    public void CreatePublisher(
      IVssRequestContext requestContext,
      string messageBusName,
      bool deleteIfExists,
      double subscriberDeleteOnIdleMinutes)
    {
      MessageBusPublisherCreateOptions createOptions = new MessageBusPublisherCreateOptions()
      {
        DeleteIfExists = deleteIfExists,
        SubscriptionIdleTimeout = TimeSpan.FromMinutes(subscriberDeleteOnIdleMinutes)
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
      requestContext.TraceEnter(1014080, "SqlNotificationProvider", "Service", nameof (CreatePublisher));
      try
      {
        CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
        string parent = RegistryHelpers.CombinePath("/Service/MessageBus/SqlNotificationMessageBus/Publisher", messageBusName);
        string registryPath = RegistryHelpers.CombinePath(parent, "EventClass");
        string query = RegistryHelpers.CombinePath(parent, "...");
        if (service.ReadEntriesFallThru(requestContext, (RegistryQuery) query).Any<RegistryEntry>())
        {
          if (!createOptions.DeleteIfExists)
            return;
          string message = "The SqlNotificationServiceBusProvider does not support setting deleteIfExists=true. There is a small chance that some messages may not be delivered during the period where a publisher is recreated.";
          requestContext.Trace(1014085, TraceLevel.Warning, "SqlNotificationProvider", "Service", message);
        }
        else
        {
          if (service.ReadEntriesFallThru(requestContext, (RegistryQuery) query).Any<RegistryEntry>())
            return;
          RegistryEntry[] registryEntryArray = new RegistryEntry[1]
          {
            RegistryEntry.Create<Guid>(registryPath, Guid.NewGuid())
          };
          service.WriteEntries(requestContext, (IEnumerable<RegistryEntry>) registryEntryArray);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1014088, "SqlNotificationProvider", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1014089, "SqlNotificationProvider", "Service", nameof (CreatePublisher));
      }
    }

    public void DeletePublisher(IVssRequestContext requestContext, string messageBusName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(messageBusName, nameof (messageBusName));
      requestContext.TraceEnter(1014070, "SqlNotificationProvider", "Service", nameof (DeletePublisher));
      try
      {
        Guid guid;
        if (!this.m_publishEntries.TryGetValue(messageBusName, out guid))
          return;
        lock (this.m_publishLock)
        {
          if (!this.m_publishEntries.TryGetValue(messageBusName, out guid))
            return;
          Dictionary<string, Guid> dictionary = new Dictionary<string, Guid>(this.m_publishEntries);
          dictionary.Remove(messageBusName);
          this.m_publishEntries = (IDictionary<string, Guid>) dictionary;
          CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
          string parent = RegistryHelpers.CombinePath("/Service/MessageBus/SqlNotificationMessageBus/Publisher", messageBusName);
          IVssRequestContext requestContext1 = requestContext;
          string registryPathPattern = RegistryHelpers.CombinePath(parent, "...");
          service.DeleteEntries(requestContext1, registryPathPattern);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1014079, "SqlNotificationProvider", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1014079, "SqlNotificationProvider", "Service", nameof (DeletePublisher));
      }
    }

    public MessageBusSubscriptionInfo CreateTransientSubscriber(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriberPrefix)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(messageBusName, nameof (messageBusName));
      return new MessageBusSubscriptionInfo()
      {
        MessageBusName = messageBusName,
        SubscriptionName = string.Empty
      };
    }

    public MessageBusSubscriptionInfo CreateSubscriber(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      ITFLogger logger = null)
    {
      throw new NotSupportedException("Creation of named subscriptions is not supported for on-premise message bus.");
    }

    public void DeleteSubscriber(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MessageBusSubscriptionInfo>(subscription, nameof (subscription));
    }

    public void Publish(
      IVssRequestContext requestContext,
      string messageBusName,
      MessageBusMessage[] messages,
      bool throwOnMissingPublisher = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(messageBusName, nameof (messageBusName));
      ArgumentUtility.CheckForNull<MessageBusMessage[]>(messages, nameof (messages));
      requestContext.TraceEnter(1014020, "SqlNotificationProvider", "Service", nameof (Publish));
      try
      {
        ITeamFoundationSqlNotificationService notificationService = SqlMessageBusManagementService.GetNotificationService(requestContext);
        Guid publisherEventClass = this.GetPublisherEventClass(requestContext, messageBusName);
        int length = messages.Length;
        TraceLevel level = TraceLevel.Verbose;
        if (length > this.m_objectCountWarningLevel)
          level = TraceLevel.Warning;
        else if (length > this.m_objectCountInfoLevel)
          level = TraceLevel.Info;
        requestContext.Trace(1014022, level, "SqlNotificationProvider", "Service", "Publish. Message count: {0}. If message counts is consistently above 1 we should optimize message publishing to batch messages.", (object) length);
        foreach (MessageBusMessage message in messages)
        {
          object body = message.GetBody<object>();
          string eventData = (string) null;
          if (body != null)
            eventData = SqlMessageBusManagementService.SerializeEventData(body);
          requestContext.Trace(1014024, TraceLevel.Verbose, "SqlNotificationProvider", "Service", "Publish. Sending notification message to Sql Notification Service");
          notificationService.SendNotification(requestContext, publisherEventClass, eventData);
          requestContext.Trace(1014026, TraceLevel.Verbose, "SqlNotificationProvider", "Service", "Publish. Sent notification message to Sql Notification Service");
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1014028, "SqlNotificationProvider", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1014029, "SqlNotificationProvider", "Service", nameof (Publish));
      }
    }

    public void Subscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      Action<IVssRequestContext, IMessage> action,
      Action<Exception, string, IMessage> exceptionNotification,
      bool invokeActionWithNoMessage)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MessageBusSubscriptionInfo>(subscription, nameof (subscription));
      ArgumentUtility.CheckForNull<Action<IVssRequestContext, IMessage>>(action, nameof (action));
      requestContext.TraceEnter(1014040, "SqlNotificationProvider", "Service", nameof (Subscribe));
      try
      {
        string messageBusName = subscription.MessageBusName;
        Guid publisherEventClass = this.GetPublisherEventClass(requestContext, messageBusName);
        lock (this.m_subscriberLock)
        {
          if (this.m_subscribers.ContainsKey(messageBusName))
          {
            requestContext.Trace(1014041, TraceLevel.Info, "SqlNotificationProvider", "Service", "Unsubscribe existing subscriber for message bus. Name: {0}", (object) messageBusName);
            this.Unsubscribe(requestContext, subscription);
          }
          SqlNotificationHandler handler = (SqlNotificationHandler) ((rc, args) => this.CallSubscriber(rc, args, messageBusName, action, exceptionNotification));
          SqlMessageBusManagementService.GetNotificationService(requestContext).RegisterNotification(requestContext, "Default", publisherEventClass, handler, false);
          this.m_subscribers[messageBusName] = handler;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1014048, "SqlNotificationProvider", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1014049, "SqlNotificationProvider", "Service", nameof (Subscribe));
      }
    }

    public void Unsubscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MessageBusSubscriptionInfo>(subscription, nameof (subscription));
      requestContext.TraceEnter(1014090, "SqlNotificationProvider", "Service", nameof (Unsubscribe));
      try
      {
        string messageBusName = subscription.MessageBusName;
        SqlNotificationHandler handler;
        if (!this.m_subscribers.TryGetValue(messageBusName, out handler))
          return;
        lock (this.m_subscriberLock)
        {
          if (!this.m_subscribers.TryGetValue(messageBusName, out handler))
            return;
          Guid publisherEventClass = this.GetPublisherEventClass(requestContext, messageBusName);
          SqlMessageBusManagementService.GetNotificationService(requestContext).UnregisterNotification(requestContext, "Default", publisherEventClass, handler, true);
          this.m_subscribers.Remove(messageBusName);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1014098, "SqlNotificationProvider", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1014090, "SqlNotificationProvider", "Service", nameof (Unsubscribe));
      }
    }

    public void FixMessageQueueMappings(
      IVssRequestContext deploymentContext,
      string ns,
      string hostNamePrefix,
      string sharedAccessKeySettingName,
      ITFLogger logger)
    {
    }

    public void UpdateSubscribers(
      IVssRequestContext requestContext,
      MessageBusSubscriberSettings subscriberSettings,
      string messageBusName,
      string subscriptionName,
      string namespaceName)
    {
      throw new NotSupportedException("Update subscriptions is not supported for on-premise message bus.");
    }

    public IEnumerable<MessageBusSubscriptionInfo> GetSubscribers(
      IVssRequestContext requestContext,
      string topicName)
    {
      throw new NotSupportedException("Get subscription is not supported for on-premise message bus.");
    }

    public void ClearPublishers(IVssRequestContext requestContext, bool dispose = false) => throw new NotSupportedException("Clear Publishers is not supported for on-premise message bus.");

    public void UpdateSubscriptionFilter(
      IVssRequestContext requestContext,
      string messageBusName,
      string subscriptionName,
      string newFilterValue,
      bool isTransient = false,
      ITFLogger logger = null)
    {
      throw new NotSupportedException("Named subscriptions are not supported for on-premises message bus.");
    }

    private void CallSubscriber(
      IVssRequestContext requestContext,
      NotificationEventArgs eventArgs,
      string messageBusName,
      Action<IVssRequestContext, IMessage> action,
      Action<Exception, string, IMessage> exceptionNotification)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<NotificationEventArgs>(eventArgs, nameof (eventArgs));
      requestContext.TraceEnter(1014100, "SqlNotificationProvider", "Service", nameof (CallSubscriber));
      try
      {
        long eventId = eventArgs.Id;
        string eventDataStr = eventArgs.Data;
        TeamFoundationTaskService service = requestContext.GetService<TeamFoundationTaskService>();
        requestContext.Trace(1014105, TraceLevel.Verbose, "SqlNotificationProvider", "Service", "Queuing message delivery through Task Service. Message bus: {0}", (object) messageBusName);
        IVssRequestContext requestContext1 = requestContext;
        TeamFoundationTaskCallback callback = (TeamFoundationTaskCallback) ((rc, args) =>
        {
          Stopwatch stopwatch = Stopwatch.StartNew();
          try
          {
            IMessage message = (IMessage) new SqlMessageBusManagementService.NotificationMessage(eventId, eventDataStr);
            this.DeliverWithRetries(rc, message, this.m_retries, action, messageBusName, exceptionNotification);
          }
          finally
          {
            stopwatch.Stop();
            TraceLevel level = stopwatch.ElapsedMilliseconds >= (long) this.m_deliveryWarningThresholdMilliseconds ? TraceLevel.Warning : TraceLevel.Verbose;
            rc.Trace(1014108, level, "SqlNotificationProvider", "Service", "Delivery of message bus message took {0}ms. Message bus: {1}", (object) stopwatch.ElapsedMilliseconds, (object) messageBusName);
          }
        });
        service.AddTask(requestContext1, callback);
      }
      finally
      {
        requestContext.TraceLeave(1014109, "SqlNotificationProvider", "Service", nameof (CallSubscriber));
      }
    }

    private void DeliverWithRetries(
      IVssRequestContext requestContext,
      IMessage message,
      int attemptsRemaining,
      Action<IVssRequestContext, IMessage> action,
      string messageBusName,
      Action<Exception, string, IMessage> exceptionNotification = null)
    {
      requestContext.Trace(1014110, TraceLevel.Verbose, "SqlNotificationProvider", "Service", "CallSubscriber.CallingAction");
      do
      {
        --attemptsRemaining;
        try
        {
          action(requestContext, message);
          break;
        }
        catch (Exception ex1)
        {
          TeamFoundationTracingService.TraceExceptionRaw(1014116, "SqlNotificationProvider", "Service", ex1);
          if (exceptionNotification != null)
          {
            try
            {
              exceptionNotification(ex1, messageBusName, message);
            }
            catch (Exception ex2)
            {
              TeamFoundationTracingService.TraceExceptionRaw(1014117, "SqlNotificationProvider", "Service", ex2);
            }
          }
          if (attemptsRemaining <= 0)
          {
            throw;
          }
          else
          {
            requestContext.Trace(1014118, TraceLevel.Info, "SqlNotificationProvider", "Service", "SqlNotificationServiceBus: Retrying delivery. Remaining attempts: {0}", (object) attemptsRemaining);
            Thread.Sleep(this.m_retrySleepDelay);
          }
        }
      }
      while (attemptsRemaining > 0);
      requestContext.Trace(1014119, TraceLevel.Verbose, "SqlNotificationProvider", "Service", "CallSubscriber.CalledAction");
    }

    internal static string SerializeEventData(object serializableObject)
    {
      if (serializableObject is Stream)
      {
        if (serializableObject is MemoryStream)
          return Convert.ToBase64String(((MemoryStream) serializableObject).ToArray());
        using (MemoryStream destination = new MemoryStream())
        {
          ((Stream) serializableObject).CopyTo((Stream) destination);
          return Convert.ToBase64String(destination.ToArray());
        }
      }
      else
      {
        StringBuilder output = new StringBuilder();
        XmlWriterSettings settings = new XmlWriterSettings()
        {
          OmitXmlDeclaration = true,
          Indent = false
        };
        using (XmlWriter writer = XmlWriter.Create(output, settings))
          new DataContractSerializer(serializableObject.GetType()).WriteObject(writer, serializableObject);
        return output.ToString();
      }
    }

    internal static T DeserializeEventData<T>(string eventData)
    {
      if (typeof (T) == typeof (Stream))
        return (T) new MemoryStream(Convert.FromBase64String(eventData));
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      using (StringReader input = new StringReader(eventData))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
          return (T) new DataContractSerializer(typeof (T)).ReadObject(reader);
      }
    }

    private Guid GetPublisherEventClass(IVssRequestContext requestContext, string messageBusName)
    {
      Guid publisherEventClass;
      if (!this.m_publishEntries.TryGetValue(messageBusName, out publisherEventClass))
      {
        lock (this.m_publishLock)
        {
          if (!this.m_publishEntries.TryGetValue(messageBusName, out publisherEventClass))
          {
            string query = RegistryHelpers.CombinePath(RegistryHelpers.CombinePath("/Service/MessageBus/SqlNotificationMessageBus/Publisher", messageBusName), "EventClass");
            publisherEventClass = requestContext.GetService<CachedRegistryService>().GetValue<Guid>(requestContext, (RegistryQuery) query, true, Guid.Empty);
            if (!publisherEventClass.Equals(Guid.Empty))
            {
              this.m_publishEntries = (IDictionary<string, Guid>) new Dictionary<string, Guid>(this.m_publishEntries)
              {
                {
                  messageBusName,
                  publisherEventClass
                }
              };
              requestContext.Trace(1014030, TraceLevel.Info, "SqlNotificationProvider", "Service", "Added entry to m_publishentries");
            }
            else
            {
              MessageBusNotFoundException notFoundException = new MessageBusNotFoundException(messageBusName);
              requestContext.TraceException(1014031, "SqlNotificationProvider", "Service", (Exception) notFoundException);
              throw notFoundException;
            }
          }
        }
      }
      return publisherEventClass;
    }

    private static ITeamFoundationSqlNotificationService GetNotificationService(
      IVssRequestContext requestContext)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationSqlNotificationService>();
    }

    public string GetSubscriberNameForScaleUnit(
      IVssRequestContext requestContext,
      string messageBusName)
    {
      throw new NotImplementedException();
    }

    private class NotificationMessage : IMessage
    {
      private long m_id;
      private string m_data;
      private Dictionary<string, object> m_emptyProperties = new Dictionary<string, object>();

      public NotificationMessage(long eventId, string eventData)
      {
        this.m_id = eventId;
        this.m_data = eventData;
      }

      public long SequenceNumber => this.m_id;

      public IDictionary<string, object> Properties => (IDictionary<string, object>) this.m_emptyProperties;

      public T GetBody<T>() => SqlMessageBusManagementService.DeserializeEventData<T>(this.m_data);

      public string ContentType => string.Empty;

      public string PartitionKey { get; set; }

      public DateTime EnqueuedTimeUtc { get; set; }
    }

    private class RegistryFormatStrings
    {
      public const string MessageBusPath = "/Service/MessageBus/SqlNotificationMessageBus";
      public const string PublisherPath = "/Service/MessageBus/SqlNotificationMessageBus/Publisher";
      public const string MessageDeliveryRetries = "/Service/MessageBus/SqlNotificationMessageBus/Retries";
      public const string MessageDeliveryRetrySleep = "/Service/MessageBus/SqlNotificationMessageBus/RetrySleep";
      public const string ObjectCountInfoLevel = "/Service/MessageBus/SqlNotificationMessageBus/ObjectCountInfoLevel";
      public const string ObjectCountWarningLevel = "/Service/MessageBus/SqlNotificationMessageBus/ObjectCountWarningLevel";
      public const string DeliveryWarningThresholdMilliSeconds = "/Service/MessageBus/SqlNotificationMessageBus/DeliveryWarningThresholdMilliseconds";
      public const string EventClassRelativePath = "EventClass";
    }
  }
}
