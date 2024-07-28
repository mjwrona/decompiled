// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationMessageQueueService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationMessageQueueService : 
    ITeamFoundationMessageQueueService,
    IVssFrameworkService
  {
    private object m_thisLock;
    private bool m_shutdown;
    private Timer m_dispatchTimer;
    private Timer m_heartbeatTimer;
    private Timer m_statusChangeTimer;
    private Timer m_disconnectionTimer;
    private int m_disconnectDelay;
    private IVssServiceHost m_host;
    private TeamFoundationMessageQueueService.Settings m_settings;
    private HashSet<QueueOperation> m_pendingOperations;
    private HashSet<QueueOperation> m_operationsWaitingForData;
    private HashSet<QueueOperation> m_operationsReadyForDispatch;
    private List<TfsmqConnectionNotification> m_pendingNotifications;
    private HashSet<TeamFoundationMessageQueue> m_onlineQueues;
    private Dictionary<string, TeamFoundationMessageQueue> m_queueCache;
    private static readonly string s_layer = "Service";
    private static readonly string s_area = "MessageQueue";
    private static readonly string s_StaggerDisconnectDelay = "VisualStudio.Services.MessageQueue.StaggerDisconnectDelay";
    internal static readonly TeamFoundationMessageQueueService.Settings DefaultSettings = new TeamFoundationMessageQueueService.Settings()
    {
      IdleTimeout = TimeSpan.FromMinutes(5.0),
      MessageDispatchDelay = TimeSpan.FromSeconds(1.0),
      OfflineTimeout = TimeSpan.FromMinutes(1.0),
      StatusChangeNotificationDelay = TimeSpan.FromSeconds(1.0)
    };
    internal static readonly TeamFoundationMessageQueueService.Settings MinSettings = new TeamFoundationMessageQueueService.Settings()
    {
      IdleTimeout = TimeSpan.FromSeconds(30.0),
      MessageDispatchDelay = TimeSpan.Zero,
      OfflineTimeout = TimeSpan.FromMinutes(1.0),
      StatusChangeNotificationDelay = TimeSpan.Zero
    };
    internal static readonly TeamFoundationMessageQueueService.Settings MaxSettings = new TeamFoundationMessageQueueService.Settings()
    {
      IdleTimeout = TimeSpan.FromMinutes(10.0),
      MessageDispatchDelay = TimeSpan.FromSeconds(10.0),
      OfflineTimeout = TimeSpan.FromDays(1.0),
      StatusChangeNotificationDelay = TimeSpan.FromSeconds(30.0)
    };
    private string m_sqlNotificationDataspaceCategory;
    private const int c_minDisconnectDelay = 10;
    private const int c_maxDisconnectDelay = 60;
    private const string MessageQueueSettingsFilter = "/Service/MessageQueue/Settings/...";
    internal const string MessageQueueIdleTimeoutKey = "/Service/MessageQueue/Settings/IdleTimeout";
    internal const string MessageQueueOfflineTimeoutKey = "/Service/MessageQueue/Settings/OfflineTimeout";
    internal const string MessageQueueMessageDispatchDelayKey = "/Service/MessageQueue/Settings/MessageDispatchDelay";
    internal const string MessageQueueStatusChangeNotificationDelayKey = "/Service/MessageQueue/Settings/StatusChangeNotificationDelay";

    public TeamFoundationMessageQueueService()
    {
      this.m_thisLock = new object();
      this.m_pendingOperations = new HashSet<QueueOperation>();
      this.m_onlineQueues = new HashSet<TeamFoundationMessageQueue>((IEqualityComparer<TeamFoundationMessageQueue>) new TeamFoundationMessageQueueService.MessageQueueIdComparer());
      this.m_operationsWaitingForData = new HashSet<QueueOperation>();
      this.m_operationsReadyForDispatch = new HashSet<QueueOperation>();
      this.m_queueCache = new Dictionary<string, TeamFoundationMessageQueue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "ServiceStart");
      bool flag = false;
      try
      {
        this.m_host = systemRequestContext.ServiceHost;
        this.m_settings = TeamFoundationMessageQueueService.LoadSettings(systemRequestContext);
        Dataspace dataspace = systemRequestContext.GetService<IDataspaceService>().QueryDataspace(systemRequestContext, "MessageQueue", Guid.Empty, false);
        this.m_sqlNotificationDataspaceCategory = dataspace != null ? dataspace.DataspaceCategory : "Default";
        ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
        service.RegisterNotification(systemRequestContext, this.m_sqlNotificationDataspaceCategory, SqlNotificationEventClasses.MessageQueueConnection, new SqlNotificationCallback(this.OnMessageQueueNotification), true);
        service.RegisterNotification(systemRequestContext, this.m_sqlNotificationDataspaceCategory, SqlNotificationEventClasses.MessageQueueDataAvailable, new SqlNotificationCallback(this.OnMessageQueueNotification), false);
        service.RegisterNotification(systemRequestContext, this.m_sqlNotificationDataspaceCategory, SqlNotificationEventClasses.MessageQueueRegistrationChanged, new SqlNotificationCallback(this.OnMessageQueueNotification), true);
        service.RegisterNotification(systemRequestContext, this.m_sqlNotificationDataspaceCategory, SqlNotificationEventClasses.MessageQueueRegistrationReload, new SqlNotificationCallback(this.OnMessageQueueNotification), false);
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChangedNotification), true, "/Service/MessageQueue/Settings/...");
        this.EnsureCache(systemRequestContext);
        flag = true;
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1019064, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, ex);
        throw;
      }
      finally
      {
        if (!flag)
          this.Shutdown();
        systemRequestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "ServiceEnd");
      try
      {
        ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
        service.UnregisterNotification(systemRequestContext, this.m_sqlNotificationDataspaceCategory, SqlNotificationEventClasses.MessageQueueRegistrationReload, new SqlNotificationCallback(this.OnMessageQueueNotification), false);
        service.UnregisterNotification(systemRequestContext, this.m_sqlNotificationDataspaceCategory, SqlNotificationEventClasses.MessageQueueRegistrationChanged, new SqlNotificationCallback(this.OnMessageQueueNotification), false);
        service.UnregisterNotification(systemRequestContext, this.m_sqlNotificationDataspaceCategory, SqlNotificationEventClasses.MessageQueueDataAvailable, new SqlNotificationCallback(this.OnMessageQueueNotification), false);
        service.UnregisterNotification(systemRequestContext, this.m_sqlNotificationDataspaceCategory, SqlNotificationEventClasses.MessageQueueConnection, new SqlNotificationCallback(this.OnMessageQueueNotification), false);
        systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChangedNotification));
        this.Shutdown();
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1019063, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, ex);
      }
      finally
      {
        systemRequestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "ServiceEnd");
      }
    }

    internal static TeamFoundationMessageQueueService.Settings LoadSettings(
      IVssRequestContext requestContext)
    {
      TeamFoundationMessageQueueService.Settings settings = new TeamFoundationMessageQueueService.Settings();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      try
      {
        RegistryEntryCollection registryEntryCollection = service.ReadEntriesFallThru(requestContext.Elevate(), (RegistryQuery) "/Service/MessageQueue/Settings/...");
        settings.IdleTimeout = registryEntryCollection["IdleTimeout"].GetValue<TimeSpan>(TeamFoundationMessageQueueService.DefaultSettings.IdleTimeout);
        if (settings.IdleTimeout < TeamFoundationMessageQueueService.MinSettings.IdleTimeout || settings.IdleTimeout > TeamFoundationMessageQueueService.MaxSettings.IdleTimeout)
        {
          requestContext.Trace(0, TraceLevel.Warning, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "The IdleTimeout setting of {0} is outside of the range of allowed values", (object) settings.IdleTimeout);
          settings.IdleTimeout = !(settings.IdleTimeout < TeamFoundationMessageQueueService.MinSettings.IdleTimeout) ? TeamFoundationMessageQueueService.MaxSettings.IdleTimeout : TeamFoundationMessageQueueService.MinSettings.IdleTimeout;
        }
        settings.OfflineTimeout = TimeSpan.FromMinutes(registryEntryCollection["OfflineTimeout"].GetValue<double>(TeamFoundationMessageQueueService.DefaultSettings.OfflineTimeout.TotalMinutes));
        if (settings.OfflineTimeout < TeamFoundationMessageQueueService.MinSettings.OfflineTimeout || settings.OfflineTimeout > TeamFoundationMessageQueueService.MaxSettings.OfflineTimeout)
        {
          requestContext.Trace(0, TraceLevel.Warning, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "The OfflineTimeout setting of {0} minutes is outside of the range of allowed values", (object) settings.OfflineTimeout);
          settings.OfflineTimeout = !(settings.OfflineTimeout < TeamFoundationMessageQueueService.MinSettings.OfflineTimeout) ? TeamFoundationMessageQueueService.MaxSettings.OfflineTimeout : TeamFoundationMessageQueueService.MinSettings.OfflineTimeout;
        }
        settings.MessageDispatchDelay = TimeSpan.FromSeconds(registryEntryCollection["MessageDispatchDelay"].GetValue<double>(TeamFoundationMessageQueueService.DefaultSettings.MessageDispatchDelay.TotalSeconds));
        if (settings.MessageDispatchDelay < TeamFoundationMessageQueueService.MinSettings.MessageDispatchDelay || settings.MessageDispatchDelay > TeamFoundationMessageQueueService.MaxSettings.MessageDispatchDelay)
        {
          requestContext.Trace(0, TraceLevel.Warning, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "The MessageDispatchDelay setting of {0} seconds is outside of the range of allowed values", (object) settings.MessageDispatchDelay);
          settings.MessageDispatchDelay = !(settings.MessageDispatchDelay < TeamFoundationMessageQueueService.MinSettings.MessageDispatchDelay) ? TeamFoundationMessageQueueService.MaxSettings.MessageDispatchDelay : TeamFoundationMessageQueueService.MinSettings.MessageDispatchDelay;
        }
        settings.StatusChangeNotificationDelay = TimeSpan.FromSeconds(registryEntryCollection["StatusChangeNotificationDelay"].GetValue<double>(TeamFoundationMessageQueueService.DefaultSettings.StatusChangeNotificationDelay.TotalSeconds));
        if (!(settings.StatusChangeNotificationDelay < TeamFoundationMessageQueueService.MinSettings.StatusChangeNotificationDelay))
        {
          if (!(settings.StatusChangeNotificationDelay > TeamFoundationMessageQueueService.MaxSettings.StatusChangeNotificationDelay))
            goto label_11;
        }
        requestContext.Trace(0, TraceLevel.Warning, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "The StatusChangeNotificationDelay setting of {0} seconds is outside of the range of allowed values", (object) settings.StatusChangeNotificationDelay);
        settings.StatusChangeNotificationDelay = !(settings.StatusChangeNotificationDelay < TeamFoundationMessageQueueService.MinSettings.StatusChangeNotificationDelay) ? TeamFoundationMessageQueueService.MaxSettings.StatusChangeNotificationDelay : TeamFoundationMessageQueueService.MinSettings.StatusChangeNotificationDelay;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1019065, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, ex);
        settings = TeamFoundationMessageQueueService.DefaultSettings;
      }
label_11:
      return settings;
    }

    private void Shutdown()
    {
      if (this.m_shutdown)
        return;
      TeamFoundationMessageQueue[] array = (TeamFoundationMessageQueue[]) null;
      lock (this.m_thisLock)
      {
        if (this.m_shutdown)
          return;
        this.m_shutdown = true;
        if (this.m_queueCache.Count > 0)
        {
          array = new TeamFoundationMessageQueue[this.m_queueCache.Count];
          this.m_queueCache.Values.CopyTo(array, 0);
          this.m_queueCache.Clear();
        }
        if (this.m_dispatchTimer != null)
        {
          this.m_dispatchTimer.Dispose();
          this.m_dispatchTimer = (Timer) null;
        }
        if (this.m_heartbeatTimer != null)
        {
          this.m_heartbeatTimer.Dispose();
          this.m_heartbeatTimer = (Timer) null;
        }
        if (this.m_statusChangeTimer != null)
        {
          this.m_statusChangeTimer.Dispose();
          this.m_statusChangeTimer = (Timer) null;
        }
        if (this.m_disconnectionTimer != null)
        {
          this.m_disconnectionTimer.Dispose();
          this.m_disconnectionTimer = (Timer) null;
        }
      }
      if (array == null)
        return;
      for (int index = 0; index < array.Length; ++index)
        array[index].Shutdown();
    }

    public TimeSpan IdleTimeout => this.m_settings.IdleTimeout;

    public TimeSpan OfflineTimeout => this.m_settings.OfflineTimeout;

    public bool QueueExists(IVssRequestContext requestContext, string queueName)
    {
      lock (this.m_thisLock)
        return this.m_queueCache.ContainsKey(queueName);
    }

    public IAsyncResult BeginAcknowledge(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      IList<AcknowledgementRange> ranges,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAcknowledge(requestContext, queueName, sessionId, ranges, new MessageHeaders(MessageVersion.Soap12WSAddressing10), timeout, callback, state);
    }

    public IAsyncResult BeginAcknowledge(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      IList<AcknowledgementRange> ranges,
      MessageHeaders headers,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(queueName, nameof (queueName));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) ranges, nameof (ranges));
      requestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (BeginAcknowledge));
      AcknowledgeOperation acknowledgeOperation;
      lock (this.m_thisLock)
      {
        TeamFoundationMessageQueue queue;
        if (!this.m_queueCache.TryGetValue(queueName, out queue))
        {
          requestContext.Trace(0, TraceLevel.Error, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Message queue '{0}' was not found in the cache", (object) queueName);
          throw new MessageQueueNotFoundException(queueName);
        }
        acknowledgeOperation = new AcknowledgeOperation(requestContext, queue, sessionId, ranges, headers, timeout, callback, state);
        this.m_pendingOperations.Add((QueueOperation) acknowledgeOperation);
      }
      acknowledgeOperation.Begin();
      requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (BeginAcknowledge));
      return (IAsyncResult) acknowledgeOperation;
    }

    public void EndAcknowledge(IAsyncResult result)
    {
      TeamFoundationTracingService.TraceEnterRaw(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (EndAcknowledge), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      if (!(result is AcknowledgeOperation operation))
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Invalid asynchronous operation");
        throw new InvalidOperationException(TFCommonResources.InvalidAsynchronousOperationParameter((object) nameof (result)));
      }
      try
      {
        operation.End();
      }
      finally
      {
        this.RemoveOperation((QueueOperation) operation);
      }
      TeamFoundationTracingService.TraceLeaveRaw(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (EndAcknowledge));
    }

    public IAsyncResult BeginDequeue(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      long lastMessageId,
      IList<AcknowledgementRange> ranges,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDequeue(requestContext, queueName, sessionId, lastMessageId, ranges, new MessageHeaders(MessageVersion.Soap12WSAddressing10), timeout, callback, state);
    }

    public IAsyncResult BeginDequeue(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      long lastMessageId,
      IList<AcknowledgementRange> ranges,
      MessageHeaders headers,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      ArgumentUtility.CheckForNull<MessageHeaders>(headers, nameof (headers));
      return this.BeginDequeue(requestContext, queueName, sessionId, lastMessageId, ranges, headers, timeout, TfsMessageQueueVersion.V2, callback, state);
    }

    internal IAsyncResult BeginDequeue(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      long lastMessageId,
      IList<AcknowledgementRange> ranges,
      MessageHeaders headers,
      TimeSpan timeout,
      TfsMessageQueueVersion version,
      AsyncCallback callback,
      object state)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(queueName, nameof (queueName));
      requestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "BeginDequeueMessage");
      DequeueOperation dequeueOperation;
      lock (this.m_thisLock)
      {
        TeamFoundationMessageQueue queue;
        if (!this.m_queueCache.TryGetValue(queueName, out queue))
        {
          Monitor.Wait(this.m_thisLock, TimeSpan.FromSeconds(5.0));
          if (!this.m_queueCache.TryGetValue(queueName, out queue))
          {
            requestContext.Trace(0, TraceLevel.Error, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Message queue '{0}' was not found in the cache", (object) queueName);
            throw new MessageQueueNotFoundException(queueName);
          }
        }
        dequeueOperation = new DequeueOperation(requestContext, queue, sessionId, lastMessageId, ranges, headers, timeout, version, callback, state);
        this.m_pendingOperations.Add((QueueOperation) dequeueOperation);
      }
      dequeueOperation.Begin();
      requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "BeginDequeueMessage");
      return (IAsyncResult) dequeueOperation;
    }

    public Message EndDequeue(IAsyncResult result)
    {
      DequeueOperation operation;
      MessageContainer messageContainer = this.EndDequeue2(result, out operation);
      if (messageContainer == null)
        return (Message) null;
      Message message = Message.CreateMessage(XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(messageContainer.Body), XmlDictionaryReaderQuotas.Max), 8192, MessageVersion.Soap12WSAddressing10);
      message.Headers.Add((MessageHeader) new MessageIdHeader(operation.Version, messageContainer.MessageId));
      return message;
    }

    private MessageContainer EndDequeue2(IAsyncResult result, out DequeueOperation operation)
    {
      operation = result as DequeueOperation;
      if (operation == null)
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Invalid asynchronous dequeue operation");
        throw new InvalidOperationException(TFCommonResources.InvalidAsynchronousOperationParameter((object) nameof (result)));
      }
      try
      {
        operation.RequestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "EndDequeueMessage");
        return operation.End();
      }
      finally
      {
        this.RemoveOperation((QueueOperation) operation);
        operation.RequestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "EndDequeueMessage");
      }
    }

    public void CreateQueue(
      IVssRequestContext requestContext,
      string queueName,
      string description)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ValidationHelper.ValidateQueueName(nameof (queueName), queueName, false);
      requestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (CreateQueue));
      TeamFoundationMessageQueue foundationMessageQueue;
      using (MessageQueueComponent component = requestContext.CreateComponent<MessageQueueComponent>())
        foundationMessageQueue = component.RegisterMessageQueue(queueName, description);
      foundationMessageQueue.QueueService = this;
      lock (this.m_thisLock)
        this.m_queueCache.Add(foundationMessageQueue.Name, foundationMessageQueue);
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      TfsmqRegistrationChangedNotification changedNotification = new TfsmqRegistrationChangedNotification(foundationMessageQueue.Id, foundationMessageQueue.Name, TfsmqRegistrationChangeType.Created);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      TfsmqRegistrationChangedNotification notificationEvent = changedNotification;
      service.PublishNotification(requestContext1, (object) notificationEvent);
      requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (CreateQueue));
    }

    public Task DeleteMessagesAsync(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      long messageId,
      TimeSpan timeout)
    {
      AcknowledgementRange[] ranges = new AcknowledgementRange[1]
      {
        new AcknowledgementRange()
        {
          Lower = messageId,
          Upper = messageId
        }
      };
      return Task.Factory.FromAsync((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => this.BeginAcknowledge(requestContext, queueName, sessionId, (IList<AcknowledgementRange>) ranges, timeout, callback, state)), (Action<IAsyncResult>) (result => this.EndAcknowledge(result)), (object) null);
    }

    public Task DeleteMessagesAsync(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      long[] messageIds,
      TimeSpan timeout)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) messageIds, nameof (messageIds));
      List<AcknowledgementRange> ranges = new List<AcknowledgementRange>();
      Array.Sort<long>(messageIds);
      long num1 = messageIds[0];
      long num2 = messageIds[0];
      for (int index = 1; index < messageIds.Length; ++index)
      {
        long messageId = messageIds[index];
        if (messageId == num1 || messageId == num1 + 1L)
        {
          num1 = messageId;
        }
        else
        {
          ranges.Add(new AcknowledgementRange()
          {
            Lower = num2,
            Upper = num1
          });
          num2 = messageId;
          num1 = messageId;
        }
      }
      if (ranges.Count == 0 || ranges[ranges.Count - 1].Lower != num2)
        ranges.Add(new AcknowledgementRange()
        {
          Lower = num2,
          Upper = num1
        });
      return Task.Factory.FromAsync((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => this.BeginAcknowledge(requestContext, queueName, sessionId, (IList<AcknowledgementRange>) ranges, timeout, callback, state)), (Action<IAsyncResult>) (result => this.EndAcknowledge(result)), (object) null);
    }

    public void DeleteQueue(IVssRequestContext requestContext, string queueName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(queueName, nameof (queueName));
      requestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (DeleteQueue));
      lock (this.m_thisLock)
      {
        if (!this.m_queueCache.TryGetValue(queueName, out TeamFoundationMessageQueue _))
        {
          requestContext.Trace(0, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Exited because message queue '{0}' is not found", (object) queueName);
          return;
        }
      }
      using (MessageQueueComponent component = requestContext.CreateComponent<MessageQueueComponent>())
        component.UnregisterMessageQueue(queueName);
      int queueId = this.RemoveMessageQueue(requestContext, queueName);
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      string queueName1 = queueName;
      TfsmqRegistrationChangedNotification notificationEvent = new TfsmqRegistrationChangedNotification(queueId, queueName1, TfsmqRegistrationChangeType.Deleted);
      service.PublishNotification(requestContext.Elevate(), (object) notificationEvent);
      requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (DeleteQueue));
    }

    public Task<MessageContainer> GetMessageAsync(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      TimeSpan timeout,
      long? lastMessageId = null)
    {
      return Task.Factory.FromAsync<MessageContainer>((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => this.BeginDequeue(requestContext, queueName, sessionId, lastMessageId.GetValueOrDefault(), (IList<AcknowledgementRange>) null, timeout, callback, state)), (Func<IAsyncResult, MessageContainer>) (result => this.EndDequeue2(result, out DequeueOperation _)), (object) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public MessageQueueStatus GetQueueConnectionStatus(
      IVssRequestContext requestContext,
      string queueName,
      out DateTime lastConnectedOn)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(queueName, nameof (queueName));
      requestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (GetQueueConnectionStatus));
      TeamFoundationMessageQueue queue = (TeamFoundationMessageQueue) null;
      lock (this.m_thisLock)
      {
        if (!this.m_queueCache.TryGetValue(queueName, out queue))
        {
          requestContext.Trace(0, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Exited because message queue '{0}' is not found", (object) queueName);
          requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (GetQueueConnectionStatus));
          throw new MessageQueueNotFoundException(queueName);
        }
      }
      using (MessageQueueComponent component = requestContext.CreateComponent<MessageQueueComponent>())
        queue = component.GetMessageQueue(queue);
      if (queue == null)
      {
        requestContext.Trace(0, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Exited because message queue '{0}' is not found", (object) queueName);
        requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (GetQueueConnectionStatus));
        throw new MessageQueueNotFoundException(queueName);
      }
      lastConnectedOn = queue.DateLastConnected;
      requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (GetQueueConnectionStatus));
      return queue.Status;
    }

    public void EmptyQueue(IVssRequestContext requestContext, string queueName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(queueName, nameof (queueName));
      requestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (EmptyQueue));
      using (MessageQueueComponent component = requestContext.CreateComponent<MessageQueueComponent>())
        component.EmptyMessageQueue(queueName);
      requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (EmptyQueue));
    }

    public void EnqueueMessage(
      IVssRequestContext requestContext,
      string queueName,
      Message message)
    {
      ArgumentUtility.CheckForNull<Message>(message, nameof (message));
      this.EnqueueMessage(requestContext, queueName, (string) null, TeamFoundationMessageSerializationUtility.SerializeToString(message));
    }

    public long EnqueueMessage(
      IVssRequestContext requestContext,
      string queueName,
      string messageType,
      string message)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(queueName, nameof (queueName));
      ArgumentUtility.CheckStringForNullOrEmpty(message, nameof (message));
      requestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (EnqueueMessage));
      long num;
      using (MessageQueueComponent component = requestContext.CreateComponent<MessageQueueComponent>())
        num = component.EnqueueMessage(queueName, messageType, message);
      this.ProcessDataAvailable(requestContext, queueName);
      requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (EnqueueMessage));
      return num;
    }

    public async Task<long> EnqueueMessageAsync(
      IVssRequestContext requestContext,
      string queueName,
      string messageType,
      string message)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(queueName, nameof (queueName));
      ArgumentUtility.CheckStringForNullOrEmpty(message, nameof (message));
      requestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "EnqueueMessage");
      requestContext.AssertAsyncExecutionEnabled();
      long num;
      using (MessageQueueComponent mqComponent = requestContext.CreateComponent<MessageQueueComponent>())
        num = await mqComponent.EnqueueMessageAsync(queueName, messageType, message);
      this.ProcessDataAvailable(requestContext, queueName);
      requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "EnqueueMessage");
      return num;
    }

    public void SetQueueOffline(IVssRequestContext requestContext, string queueName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(queueName, nameof (queueName));
      requestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (SetQueueOffline));
      TeamFoundationMessageQueue foundationMessageQueue;
      lock (this.m_thisLock)
        this.m_queueCache.TryGetValue(queueName, out foundationMessageQueue);
      if (foundationMessageQueue == null)
      {
        requestContext.Trace(0, TraceLevel.Error, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Message queue '{0}' is not found", (object) queueName);
        throw new MessageQueueNotFoundException(queueName);
      }
      List<TfsmqConnectionNotification> items;
      using (MessageQueueComponent component = requestContext.CreateComponent<MessageQueueComponent>())
      {
        DateTime dateTime = new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        items = component.SetMessageQueuesOffline((IEnumerable<KeyValuePair<int, DateTime>>) new KeyValuePair<int, DateTime>[1]
        {
          new KeyValuePair<int, DateTime>(foundationMessageQueue.Id, dateTime)
        }, true).GetCurrent<TfsmqConnectionNotification>().Items;
      }
      this.ProcessConnectionNotifications(requestContext, (ICollection<TfsmqConnectionNotification>) items);
      requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (SetQueueOffline));
    }

    private void OnRegistrySettingsChangedNotification(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.Trace(0, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Loading registry settings for message queue");
      this.m_settings = TeamFoundationMessageQueueService.LoadSettings(requestContext);
    }

    private void OnMessageQueueNotification(
      IVssRequestContext requestContext,
      Guid eventId,
      string eventData)
    {
      requestContext.TraceEnter(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (OnMessageQueueNotification));
      if (eventId == SqlNotificationEventClasses.MessageQueueConnection)
      {
        requestContext.Trace(0, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Message queue connection event");
        this.ProcessConnectionNotifications(requestContext, TfsmqConnectionNotification.FromXml(eventData));
      }
      else if (eventId == SqlNotificationEventClasses.MessageQueueDataAvailable)
      {
        requestContext.Trace(0, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Message queue data available event");
        this.ProcessDataAvailable(requestContext, TfsmqDataAvailableNotification.FromXml(eventData).QueueName);
      }
      else if (eventId == SqlNotificationEventClasses.MessageQueueRegistrationChanged)
      {
        requestContext.Trace(0, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Message queue registration changed event");
        TfsmqRegistrationChangedNotification changedNotification = TfsmqRegistrationChangedNotification.FromXml(eventData);
        switch (changedNotification.ChangeType)
        {
          case TfsmqRegistrationChangeType.Created:
            requestContext.Trace(0, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Message queue '{0}' created notification", (object) changedNotification.QueueName);
            lock (this.m_thisLock)
            {
              TeamFoundationMessageQueue foundationMessageQueue = (TeamFoundationMessageQueue) null;
              if (!this.m_queueCache.TryGetValue(changedNotification.QueueName, out foundationMessageQueue))
              {
                foundationMessageQueue = new TeamFoundationMessageQueue(this, changedNotification.QueueId, changedNotification.QueueName, MessageQueueStatus.Offline);
                this.m_queueCache.Add(foundationMessageQueue.Name, foundationMessageQueue);
                Monitor.PulseAll(this.m_thisLock);
                break;
              }
              break;
            }
          case TfsmqRegistrationChangeType.Deleted:
            requestContext.Trace(0, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Message queue '{0}' deleted notification", (object) changedNotification.QueueName);
            this.RemoveMessageQueue(requestContext, changedNotification.QueueName);
            break;
        }
      }
      else if (eventId == SqlNotificationEventClasses.MessageQueueRegistrationReload)
        this.EnsureCache(requestContext);
      requestContext.TraceLeave(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (OnMessageQueueNotification));
    }

    private int RemoveMessageQueue(IVssRequestContext requestContext, string queueName)
    {
      TeamFoundationMessageQueue foundationMessageQueue = (TeamFoundationMessageQueue) null;
      lock (this.m_thisLock)
      {
        if (this.m_queueCache.TryGetValue(queueName, out foundationMessageQueue))
        {
          requestContext.Trace(0, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Removing message queue '{0}' from the cache", (object) queueName);
          this.m_queueCache.Remove(queueName);
          this.m_onlineQueues.Remove(foundationMessageQueue);
        }
      }
      if (foundationMessageQueue != null)
      {
        requestContext.Trace(0, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Shutting down message queue '{0}'", (object) foundationMessageQueue.Name);
        foundationMessageQueue.Shutdown();
      }
      return foundationMessageQueue == null ? 0 : foundationMessageQueue.Id;
    }

    private void RemoveOperation(QueueOperation operation)
    {
      lock (this.m_thisLock)
      {
        this.m_pendingOperations.Remove(operation);
        this.m_operationsWaitingForData.Remove(operation);
        this.m_operationsReadyForDispatch.Remove(operation);
      }
    }

    internal void ReadyForDispatch(IVssRequestContext requestContext, QueueOperation operation)
    {
      requestContext.TraceEnter(1019081, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (ReadyForDispatch));
      if (operation == null)
      {
        requestContext.Trace(1019082, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Exited due to no operation to dispatch");
        requestContext.TraceLeave(1019090, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (ReadyForDispatch));
      }
      else
      {
        lock (this.m_thisLock)
        {
          if (!this.m_pendingOperations.Contains(operation))
          {
            requestContext.Trace(1019083, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Exited because the operation is no longer pending");
            requestContext.TraceLeave(1019090, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (ReadyForDispatch));
            return;
          }
          if (!this.m_operationsReadyForDispatch.Add(operation))
          {
            requestContext.Trace(1019084, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Exited because the operation is already scheduled for dispatch");
            requestContext.TraceLeave(1019090, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (ReadyForDispatch));
            return;
          }
          this.m_operationsWaitingForData.Remove(operation);
          this.ScheduleTask(ref this.m_dispatchTimer, this.m_settings.MessageDispatchDelay, new Func<IVssRequestContext, TimeSpan>(this.DispatchCallback), new Action<TimeSpan>(this.DispatchCleanupCallback));
        }
        requestContext.TraceLeave(1019090, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (ReadyForDispatch));
      }
    }

    private void ScheduleTask(
      ref Timer timer,
      TimeSpan dueTime,
      Func<IVssRequestContext, TimeSpan> callback,
      Action<TimeSpan> cleanup)
    {
      if (timer != null || this.m_shutdown)
        return;
      timer = new Timer(new System.Threading.TimerCallback(this.TimerCallback), (object) new Tuple<Func<IVssRequestContext, TimeSpan>, Action<TimeSpan>>(callback, cleanup), dueTime, TimeSpan.FromMilliseconds(-1.0));
    }

    private void TaskCompleted(ref Timer timer, TimeSpan dueTime, bool hasPendingItems)
    {
      if (timer == null || this.m_shutdown)
        return;
      if (hasPendingItems)
      {
        if (dueTime < TimeSpan.Zero)
          dueTime = TimeSpan.Zero;
        timer.Change(dueTime, TimeSpan.FromMilliseconds(-1.0));
      }
      else
      {
        timer.Dispose();
        timer = (Timer) null;
      }
    }

    private TimeSpan DispatchCallback(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1019071, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (DispatchCallback));
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<QueueOperation> queueOperationList1 = (List<QueueOperation>) null;
      try
      {
        lock (this.m_thisLock)
        {
          if (this.m_operationsReadyForDispatch.Count > 0)
          {
            queueOperationList1 = new List<QueueOperation>(this.m_operationsReadyForDispatch.Count);
            queueOperationList1.AddRange((IEnumerable<QueueOperation>) this.m_operationsReadyForDispatch);
            this.m_operationsReadyForDispatch.Clear();
          }
        }
        if (queueOperationList1 != null)
        {
          List<QueueOperation> queueOperationList2 = new List<QueueOperation>();
          List<DequeueOperation> dequeueOperationList = new List<DequeueOperation>();
          List<AcknowledgementRange> acknowledgements = new List<AcknowledgementRange>();
          foreach (QueueOperation queueOperation in queueOperationList1)
          {
            if (queueOperation.Queue.SequenceId > queueOperation.SequenceId && queueOperation.Queue.Status == MessageQueueStatus.Offline)
            {
              queueOperationList2.Add(queueOperation);
            }
            else
            {
              DequeueOperation itemsForDispatch = queueOperation.GetItemsForDispatch(acknowledgements);
              if (itemsForDispatch != null)
                dequeueOperationList.Add(itemsForDispatch);
              else
                queueOperationList2.Add(queueOperation);
            }
          }
          HashSet<DequeueOperation> idleOperations = new HashSet<DequeueOperation>((IEnumerable<DequeueOperation>) dequeueOperationList);
          List<TfsmqConnectionNotification> notifications = new List<TfsmqConnectionNotification>();
          using (MessageQueueComponent component = requestContext.CreateComponent<MessageQueueComponent>())
          {
            requestContext.Trace(1019072, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Dispatching '{0}' operations", (object) queueOperationList1.Count);
            ResultCollection resultCollection = component.DispatchMessageQueues((IEnumerable<DequeueOperation>) dequeueOperationList, (IEnumerable<AcknowledgementRange>) acknowledgements);
            ObjectBinder<TfsmqDequeueData> current = resultCollection.GetCurrent<TfsmqDequeueData>();
            while (current.MoveNext())
            {
              requestContext.Trace(1019073, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Processing message queue '{0}'", (object) current.Current.QueueName);
              DequeueOperation dequeueOperation = (DequeueOperation) null;
              lock (this.m_thisLock)
              {
                TeamFoundationMessageQueue foundationMessageQueue;
                if (this.m_queueCache.TryGetValue(current.Current.QueueName, out foundationMessageQueue))
                {
                  requestContext.Trace(1019074, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Got message queue '{0}' from the cache", (object) current.Current.QueueName);
                  dequeueOperation = foundationMessageQueue.CurrentOperation;
                }
              }
              if (dequeueOperation == null)
              {
                requestContext.Trace(1019075, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Found no pending dequeue operation from message queue '{0}'", (object) current.Current.QueueName);
              }
              else
              {
                requestContext.Trace(1019076, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Created message with Id '{0}'", (object) current.Current.MessageId);
                try
                {
                  dequeueOperation.Dispatch(current.Current.CreateMessage(), true);
                }
                catch (Exception ex)
                {
                  requestContext.TraceException(0, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, ex);
                }
                idleOperations.Remove(dequeueOperation);
              }
            }
            resultCollection.NextResult();
            notifications.AddRange((IEnumerable<TfsmqConnectionNotification>) resultCollection.GetCurrent<TfsmqConnectionNotification>().Items);
          }
          foreach (QueueOperation queueOperation in queueOperationList2)
            queueOperation.Dispatch((MessageContainer) null, true);
          this.ProcessIdleConnections(requestContext, idleOperations);
          this.ProcessConnectionNotifications(requestContext, (ICollection<TfsmqConnectionNotification>) notifications);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1019078, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, ex);
      }
      finally
      {
        stopwatch.Stop();
        if (queueOperationList1 != null)
        {
          TraceLevel callbackTraceLevel = TeamFoundationMessageQueueService.GetCallbackTraceLevel(stopwatch.Elapsed);
          requestContext.Trace(1019079, callbackTraceLevel, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Dispatched {0} operations in {1}", (object) queueOperationList1.Count, (object) stopwatch.Elapsed);
        }
        requestContext.TraceLeave(1019080, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (DispatchCallback));
      }
      return stopwatch.Elapsed;
    }

    private void DispatchCleanupCallback(TimeSpan elapsedTime)
    {
      lock (this.m_thisLock)
        this.TaskCompleted(ref this.m_dispatchTimer, this.m_settings.MessageDispatchDelay - elapsedTime, this.m_operationsReadyForDispatch.Count > 0);
    }

    private void EnsureCache(IVssRequestContext systemRequestContext)
    {
      List<TeamFoundationMessageQueue> foundationMessageQueueList = new List<TeamFoundationMessageQueue>();
      using (MessageQueueComponent component = systemRequestContext.CreateComponent<MessageQueueComponent>())
      {
        ResultCollection resultCollection = component.LoadMessageQueues();
        foundationMessageQueueList.AddRange((IEnumerable<TeamFoundationMessageQueue>) resultCollection.GetCurrent<TeamFoundationMessageQueue>().Items);
      }
      List<TeamFoundationMessageQueue> source = new List<TeamFoundationMessageQueue>();
      lock (this.m_thisLock)
      {
        bool flag = false;
        foreach (TeamFoundationMessageQueue foundationMessageQueue in foundationMessageQueueList)
        {
          if (!this.m_queueCache.TryGetValue(foundationMessageQueue.Name, out TeamFoundationMessageQueue _))
          {
            if (!foundationMessageQueue.Initialize(this))
            {
              source.Add(foundationMessageQueue);
              systemRequestContext.Trace(0, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Added expired message queue '{0}'", (object) foundationMessageQueue.Name);
            }
            else if (foundationMessageQueue.Status == MessageQueueStatus.Online)
              this.m_onlineQueues.Add(foundationMessageQueue);
            flag = true;
            this.m_queueCache.Add(foundationMessageQueue.Name, foundationMessageQueue);
            systemRequestContext.Trace(0, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Cached message queue '{0}'", (object) foundationMessageQueue.Name);
          }
        }
        if (flag)
          Monitor.PulseAll(this.m_thisLock);
      }
      if (source.Count <= 0)
        return;
      List<TfsmqConnectionNotification> items;
      using (MessageQueueComponent component = systemRequestContext.CreateComponent<MessageQueueComponent>())
      {
        KeyValuePair<int, DateTime>[] array = source.Select<TeamFoundationMessageQueue, KeyValuePair<int, DateTime>>((Func<TeamFoundationMessageQueue, KeyValuePair<int, DateTime>>) (x => new KeyValuePair<int, DateTime>(x.Id, x.DateLastConnected))).ToArray<KeyValuePair<int, DateTime>>();
        items = component.SetMessageQueuesOffline((IEnumerable<KeyValuePair<int, DateTime>>) array).GetCurrent<TfsmqConnectionNotification>().Items;
      }
      this.ProcessConnectionNotifications(systemRequestContext, (ICollection<TfsmqConnectionNotification>) items);
    }

    private void TimerCallback(object state)
    {
      TeamFoundationTracingService.TraceEnterRaw(1019061, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (TimerCallback), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      Tuple<Func<IVssRequestContext, TimeSpan>, Action<TimeSpan>> tuple = (Tuple<Func<IVssRequestContext, TimeSpan>, Action<TimeSpan>>) state;
      Func<IVssRequestContext, TimeSpan> func = tuple.Item1;
      Action<TimeSpan> action = tuple.Item2;
      TimeSpan timeSpan = TimeSpan.Zero;
      try
      {
        using (IVssRequestContext systemContext = this.m_host.DeploymentServiceHost.CreateSystemContext())
        {
          if (systemContext != null)
            vssRequestContext = systemContext.GetService<TeamFoundationHostManagementService>().BeginRequest(systemContext, this.m_host.InstanceId, RequestContextType.SystemContext, true, true);
        }
        timeSpan = func(vssRequestContext);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1019062, TraceLevel.Error, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, ex, "An error occurred while invoking callback {0}: {1}", (object) func.Method.Name, (object) ex.ToReadableStackTrace());
      }
      finally
      {
        vssRequestContext?.Dispose();
        action(timeSpan);
      }
      TeamFoundationTracingService.TraceLeaveRaw(1019070, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (TimerCallback));
    }

    private TimeSpan QueueDisconnectCallback(IVssRequestContext requestContext)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<KeyValuePair<int, DateTime>> pendingDisconnections = (List<KeyValuePair<int, DateTime>>) null;
      try
      {
        requestContext.TraceEnter(1019041, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "QueueTimeoutCallback");
        lock (this.m_thisLock)
          pendingDisconnections = this.m_onlineQueues.Where<TeamFoundationMessageQueue>((Func<TeamFoundationMessageQueue, bool>) (x => x.Expired)).Select<TeamFoundationMessageQueue, KeyValuePair<int, DateTime>>((Func<TeamFoundationMessageQueue, KeyValuePair<int, DateTime>>) (x => new KeyValuePair<int, DateTime>(x.Id, x.DateLastConnected))).ToList<KeyValuePair<int, DateTime>>();
        if (pendingDisconnections != null)
        {
          // ISSUE: explicit non-virtual call
          if (__nonvirtual (pendingDisconnections.Count) > 0)
          {
            List<TfsmqConnectionNotification> items;
            using (MessageQueueComponent component = requestContext.CreateComponent<MessageQueueComponent>())
              items = component.SetMessageQueuesOffline((IEnumerable<KeyValuePair<int, DateTime>>) pendingDisconnections).GetCurrent<TfsmqConnectionNotification>().Items;
            this.ProcessConnectionNotifications(requestContext, (ICollection<TfsmqConnectionNotification>) items);
          }
        }
      }
      finally
      {
        stopwatch.Stop();
        // ISSUE: explicit non-virtual call
        if (pendingDisconnections != null && __nonvirtual (pendingDisconnections.Count) > 0)
        {
          TraceLevel callbackTraceLevel = TeamFoundationMessageQueueService.GetCallbackTraceLevel(stopwatch.Elapsed);
          requestContext.Trace(1019049, callbackTraceLevel, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Processed {0} disconnections in {1}", (object) pendingDisconnections.Count, (object) stopwatch.Elapsed);
        }
        requestContext.TraceLeave(1019050, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "QueueTimeoutCallback");
      }
      return stopwatch.Elapsed;
    }

    private void QueueDisconnectCleanupCallback(TimeSpan elaspedTime)
    {
      lock (this.m_thisLock)
        this.TaskCompleted(ref this.m_disconnectionTimer, TimeSpan.FromSeconds((double) this.m_disconnectDelay) - elaspedTime, this.m_onlineQueues.Count > 0);
    }

    private TimeSpan QueueHeartbeatCallback(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1019031, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (QueueHeartbeatCallback));
      requestContext.TraceLeave(1019040, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (QueueHeartbeatCallback));
      return TimeSpan.Zero;
    }

    private void QueueHeartbeatCleanupCallback(TimeSpan elapsedTime)
    {
      lock (this.m_thisLock)
      {
        this.TaskCompleted(ref this.m_heartbeatTimer, TimeSpan.Zero, false);
        this.m_operationsReadyForDispatch.UnionWith((IEnumerable<QueueOperation>) this.m_operationsWaitingForData);
        this.m_operationsWaitingForData.Clear();
        this.ScheduleTask(ref this.m_dispatchTimer, TimeSpan.Zero, new Func<IVssRequestContext, TimeSpan>(this.DispatchCallback), new Action<TimeSpan>(this.DispatchCleanupCallback));
      }
    }

    private void ProcessDataAvailable(IVssRequestContext requestContext, string queueName)
    {
      TeamFoundationMessageQueue foundationMessageQueue;
      lock (this.m_thisLock)
      {
        if (!this.m_queueCache.TryGetValue(queueName, out foundationMessageQueue))
        {
          requestContext.Trace(0, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Message queue '{0}' is not found, ignore the notification", (object) queueName);
          return;
        }
      }
      this.ReadyForDispatch(requestContext, (QueueOperation) foundationMessageQueue.CurrentOperation);
    }

    private void ProcessIdleConnections(
      IVssRequestContext requestContext,
      HashSet<DequeueOperation> idleOperations)
    {
      requestContext.TraceEnter(1019021, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (ProcessIdleConnections));
      lock (this.m_thisLock)
      {
        if (idleOperations != null)
        {
          foreach (DequeueOperation dequeueOperation in idleOperations.ToArray<DequeueOperation>())
          {
            if (!this.m_pendingOperations.Contains((QueueOperation) dequeueOperation))
              idleOperations.Remove(dequeueOperation);
          }
          this.m_operationsWaitingForData.UnionWith((IEnumerable<QueueOperation>) idleOperations);
        }
        if (this.m_operationsWaitingForData.Count > 0)
          this.ScheduleTask(ref this.m_heartbeatTimer, TimeSpan.FromSeconds(this.OfflineTimeout.TotalSeconds / 2.0), new Func<IVssRequestContext, TimeSpan>(this.QueueHeartbeatCallback), new Action<TimeSpan>(this.QueueHeartbeatCleanupCallback));
      }
      requestContext.TraceLeave(1019030, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (ProcessIdleConnections));
    }

    private void ProcessConnectionNotifications(
      IVssRequestContext requestContext,
      ICollection<TfsmqConnectionNotification> notifications)
    {
      requestContext.TraceEnter(1019011, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (ProcessConnectionNotifications));
      bool flag = false;
      List<TfsmqConnectionNotification> collection = new List<TfsmqConnectionNotification>();
      lock (this.m_thisLock)
      {
        foreach (TfsmqConnectionNotification notification in (IEnumerable<TfsmqConnectionNotification>) notifications)
        {
          TeamFoundationMessageQueue foundationMessageQueue;
          if (!this.m_queueCache.TryGetValue(notification.QueueName, out foundationMessageQueue))
            requestContext.Trace(1019012, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Skipped notification because the message queue '{0}' is not found", (object) notification.QueueName);
          else if (notification.IsConnect || notification.IsHeartbeat)
          {
            requestContext.Trace(1019013, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Setting the message queue '{0}' online", (object) notification.QueueName);
            this.m_onlineQueues.Add(foundationMessageQueue);
            foundationMessageQueue.SetOnline(notification.SequenceId, notification.DateLastConnected);
            if (notification.IsConnect)
              collection.Add(notification);
          }
          else if (notification.IsDisconnect || notification.IsOffline)
          {
            requestContext.Trace(1019014, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Setting the message queue '{0}' offline", (object) notification.QueueName);
            this.m_onlineQueues.Remove(foundationMessageQueue);
            foundationMessageQueue.SetOffline(requestContext, notification.SequenceId);
            if (notification.IsDisconnect)
              collection.Add(notification);
          }
        }
        flag = this.m_onlineQueues.Count > 0;
      }
      if (flag)
      {
        this.m_disconnectDelay = !requestContext.IsFeatureEnabled(TeamFoundationMessageQueueService.s_StaggerDisconnectDelay) ? 10 : new Random().Next(10, 60);
        this.ScheduleTask(ref this.m_disconnectionTimer, TimeSpan.FromSeconds((double) this.m_disconnectDelay), new Func<IVssRequestContext, TimeSpan>(this.QueueDisconnectCallback), new Action<TimeSpan>(this.QueueDisconnectCleanupCallback));
      }
      if (collection.Count > 0)
      {
        requestContext.Trace(1019015, TraceLevel.Verbose, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Notifying {0} message queues of status changes", (object) collection.Count);
        lock (this.m_thisLock)
        {
          if (this.m_pendingNotifications == null)
            this.m_pendingNotifications = new List<TfsmqConnectionNotification>();
          this.m_pendingNotifications.AddRange((IEnumerable<TfsmqConnectionNotification>) collection);
          this.ScheduleTask(ref this.m_statusChangeTimer, TimeSpan.Zero, new Func<IVssRequestContext, TimeSpan>(this.StatusChangeCallback), new Action<TimeSpan>(this.StatusChangedCleanupCallback));
        }
      }
      requestContext.TraceLeave(1019020, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (ProcessConnectionNotifications));
    }

    private TimeSpan StatusChangeCallback(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1019001, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (StatusChangeCallback));
      Stopwatch stopwatch = Stopwatch.StartNew();
      TfsmqConnectionNotification[] connectionNotificationArray = (TfsmqConnectionNotification[]) null;
      try
      {
        lock (this.m_thisLock)
        {
          if (this.m_pendingNotifications.Count > 0)
          {
            connectionNotificationArray = new TfsmqConnectionNotification[this.m_pendingNotifications.Count];
            this.m_pendingNotifications.CopyTo(connectionNotificationArray, 0);
            this.m_pendingNotifications.Clear();
          }
        }
        if (connectionNotificationArray != null)
        {
          requestContext.Trace(1019008, TraceLevel.Info, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Sending {0} connection notifications", (object) connectionNotificationArray.Length);
          requestContext.GetService<TeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) connectionNotificationArray);
        }
      }
      finally
      {
        stopwatch.Stop();
        if (connectionNotificationArray != null)
        {
          TraceLevel callbackTraceLevel = TeamFoundationMessageQueueService.GetCallbackTraceLevel(stopwatch.Elapsed);
          requestContext.Trace(1019009, callbackTraceLevel, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, "Processed {0} connection notifications in {1}", (object) connectionNotificationArray.Length, (object) stopwatch.Elapsed);
        }
        requestContext.TraceLeave(1019010, TeamFoundationMessageQueueService.s_area, TeamFoundationMessageQueueService.s_layer, nameof (StatusChangeCallback));
      }
      return stopwatch.Elapsed;
    }

    private void StatusChangedCleanupCallback(TimeSpan elaspedTime)
    {
      lock (this.m_thisLock)
        this.TaskCompleted(ref this.m_statusChangeTimer, this.m_settings.StatusChangeNotificationDelay - elaspedTime, this.m_pendingNotifications.Count > 0);
    }

    private static TraceLevel GetCallbackTraceLevel(TimeSpan elapsed)
    {
      if (elapsed < TimeSpan.FromSeconds(1.0))
        return TraceLevel.Info;
      return elapsed < TimeSpan.FromSeconds(5.0) ? TraceLevel.Warning : TraceLevel.Error;
    }

    private static Message CreateSoapMessage(
      MessageContainer message,
      TfsMessageQueueVersion version)
    {
      Message message1 = Message.CreateMessage(XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(message.Body), XmlDictionaryReaderQuotas.Max), 8192, MessageVersion.Soap12WSAddressing10);
      message1.Headers.Add((MessageHeader) new MessageIdHeader(version, message.MessageId));
      return message1;
    }

    internal struct Settings
    {
      public TimeSpan IdleTimeout;
      public TimeSpan OfflineTimeout;
      public TimeSpan MessageDispatchDelay;
      public TimeSpan StatusChangeNotificationDelay;
    }

    private class MessageQueueIdComparer : IEqualityComparer<TeamFoundationMessageQueue>
    {
      public bool Equals(TeamFoundationMessageQueue x, TeamFoundationMessageQueue y)
      {
        int? id1 = x?.Id;
        int? id2 = y?.Id;
        return id1.GetValueOrDefault() == id2.GetValueOrDefault() & id1.HasValue == id2.HasValue;
      }

      public int GetHashCode(TeamFoundationMessageQueue obj) => obj.Id.GetHashCode();
    }
  }
}
