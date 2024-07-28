// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationMessageQueue
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Framework")]
  internal sealed class TeamFoundationMessageQueue
  {
    private TeamFoundationMessageQueue.State m_state;
    private object m_thisLock;
    private DateTime m_expirationTime;
    private Lazy<string> m_securityToken;
    private DequeueOperation m_pendingOperation;
    private TeamFoundationMessageQueueService m_queueService;
    private static readonly string s_layer = "Service";
    private static readonly string s_area = "MessageQueue";

    public TeamFoundationMessageQueue()
    {
      this.m_thisLock = new object();
      this.m_securityToken = new Lazy<string>(new Func<string>(this.CreateSecurityToken));
    }

    internal TeamFoundationMessageQueue(
      TeamFoundationMessageQueueService queueService,
      int queueId,
      string name,
      MessageQueueStatus status)
      : this()
    {
      this.Id = queueId;
      this.Name = name;
      this.Status = status;
      this.QueueService = queueService;
    }

    public DateTime DateLastConnected { get; set; }

    internal int Id { get; set; }

    public string Name { get; set; }

    internal int SequenceId { get; set; }

    internal TeamFoundationMessageQueueService QueueService
    {
      get => this.m_queueService;
      set => this.m_queueService = value;
    }

    public MessageQueueStatus Status { get; set; }

    internal string SecurityToken => this.m_securityToken.Value;

    internal DequeueOperation CurrentOperation => this.m_pendingOperation;

    internal bool Expired => this.m_expirationTime <= DateTime.UtcNow;

    private string CreateSecurityToken() => FrameworkSecurity.MessageQueueNamespaceRootToken + (object) FrameworkSecurity.MessageQueuePathSeparator + this.Name;

    internal bool Initialize(TeamFoundationMessageQueueService queueService)
    {
      TeamFoundationTracingService.TraceEnterRaw(0, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "InitializeMessageQueue", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      bool flag = false;
      this.QueueService = queueService;
      TimeSpan timeSpan = DateTime.UtcNow - this.DateLastConnected;
      if (timeSpan >= TimeSpan.Zero && timeSpan < queueService.OfflineTimeout)
      {
        this.m_expirationTime = DateTime.UtcNow + (queueService.OfflineTimeout - timeSpan);
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Change expiration time to '{0}' for message queue '{1}'", (object) this.m_expirationTime, (object) this.Name);
      }
      else if (this.DateLastConnected > DateTime.MinValue)
      {
        flag = this.Status == MessageQueueStatus.Online;
        this.m_expirationTime = DateTime.MinValue;
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "The message queue '{0}' has expired", (object) this.Name);
      }
      TeamFoundationTracingService.TraceLeaveRaw(0, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "InitializeMessageQueue");
      return !flag;
    }

    internal void Shutdown()
    {
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Verbose, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Shutting down message queue '{0}'", (object) this.Name);
      DequeueOperation pendingOperation;
      lock (this.m_thisLock)
      {
        if (this.m_state == TeamFoundationMessageQueue.State.Closed)
        {
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Message queue '{0}' is already closed", (object) this.Name);
          return;
        }
        this.m_queueService = (TeamFoundationMessageQueueService) null;
        this.m_expirationTime = DateTime.MinValue;
        this.m_state = TeamFoundationMessageQueue.State.Closed;
        pendingOperation = this.m_pendingOperation;
        this.m_pendingOperation = (DequeueOperation) null;
      }
      if (pendingOperation == null)
        return;
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Verbose, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Canceling message queue '{0}' pending operation", (object) this.Name);
      pendingOperation.Cancel();
    }

    internal void SetOnline(int sequenceId, DateTime dateLastConnected)
    {
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Verbose, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Setting message queue '{0}' online", (object) this.Name);
      lock (this.m_thisLock)
      {
        if (this.m_state == TeamFoundationMessageQueue.State.Closed)
        {
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Message queue '{0}' is already closed", (object) this.Name);
        }
        else
        {
          this.DateLastConnected = dateLastConnected;
          this.SequenceId = sequenceId;
          this.Status = MessageQueueStatus.Online;
          if (this.m_pendingOperation != null)
            this.m_pendingOperation.SequenceId = sequenceId;
          if (!(this.m_queueService.OfflineTimeout > TimeSpan.Zero))
            return;
          this.m_expirationTime = DateTime.UtcNow + this.m_queueService.OfflineTimeout;
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Change offline timeout to '{0}' for message queue '{1}'", (object) this.m_queueService.OfflineTimeout, (object) this.Name);
        }
      }
    }

    internal void SetOffline(IVssRequestContext requestContext, int sequenceId)
    {
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Verbose, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Setting message queue '{0}' offline", (object) this.Name);
      lock (this.m_thisLock)
      {
        this.m_expirationTime = DateTime.MinValue;
        this.SequenceId = sequenceId;
        this.Status = MessageQueueStatus.Offline;
      }
      if (this.m_pendingOperation == null)
        return;
      this.QueueService.ReadyForDispatch(requestContext, (QueueOperation) this.m_pendingOperation);
    }

    internal void ReleaseLock(DequeueOperation operation)
    {
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Verbose, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Releasing dequeue operation of message queue '{0}'", (object) this.Name);
      if (this.m_pendingOperation == null)
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Verbose, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "No dequeue operation to release");
      }
      else
      {
        lock (this.m_thisLock)
        {
          if (this.m_pendingOperation != null && this.m_pendingOperation != operation)
            TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Wrong dequeue operation to release");
          else
            this.m_pendingOperation = (DequeueOperation) null;
        }
      }
    }

    internal bool TryAcquireLock(DequeueOperation operation)
    {
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Verbose, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Trying acquiring message queue '{0}' for a new dequeue operation", (object) this.Name);
      lock (this.m_thisLock)
      {
        if (this.m_state == TeamFoundationMessageQueue.State.Closed)
        {
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Message queue '{0}' is already closed", (object) this.Name);
          return false;
        }
        if (this.m_pendingOperation != null)
        {
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationMessageQueue.s_area, TeamFoundationMessageQueue.s_layer, "Message queue '{0}' has already had a dequeue operation", (object) this.Name);
          return false;
        }
        this.m_pendingOperation = operation;
        return true;
      }
    }

    private enum State
    {
      Open,
      Closed,
    }
  }
}
