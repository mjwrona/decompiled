// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TfsMessageQueue
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class TfsMessageQueue
  {
    private object m_thisLock;
    private Timer m_pollTimer;
    private int m_pollCount;
    private bool m_isShutdown;
    private long m_lastMessageId;
    private int m_pollingSuspended;
    private TimeSpan m_sleepInterval;
    private Timer m_disconnectedTimer;
    private Timer m_acknowledgementTimer;
    private Exception m_pendingException;
    private ITfsRequestChannel m_channel;
    private InputQueue<TfsMessage> m_localQueue;
    private bool m_acknowledgementTimerActive;
    private List<long> m_pendingAcknowledgements;

    internal TfsMessageQueue(
      Uri queueId,
      int maxPendingCount,
      TimeSpan maxAcknowledgementDelay,
      TfsMessageQueueService manager,
      Func<SoapException, Exception> convertException)
    {
      this.m_thisLock = new object();
      this.m_lastMessageId = long.MinValue;
      this.m_channel = manager.CreatePollChannel();
      this.m_localQueue = new InputQueue<TfsMessage>();
      if (convertException == null)
        convertException = new Func<SoapException, Exception>(TeamFoundationServiceException.ConvertException);
      this.Id = queueId;
      this.ConvertException = convertException;
      this.Manager = manager;
      this.MaxPendingCount = maxPendingCount;
      this.AdditionalHeaders = new List<TfsMessageHeader>();
      this.MaxAcknowledgementDelay = maxAcknowledgementDelay;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<TfsMessageHeader> AdditionalHeaders { get; private set; }

    public Uri Id { get; private set; }

    public TimeSpan MaxAcknowledgementDelay { get; private set; }

    public int MaxPendingCount { get; private set; }

    internal Func<SoapException, Exception> ConvertException { get; private set; }

    internal TfsMessageQueueService Manager { get; private set; }

    public event EventHandler Connected;

    public event EventHandler<TfsMessageQueueDisconnectedEventArgs> Disconnected;

    public void Abort()
    {
      if (!this.InitiateShutdown())
        return;
      if (this.m_channel != null)
        this.m_channel.Abort();
      this.Manager.Remove(this);
    }

    internal void Acknowledge(long messageId)
    {
      if (this.m_isShutdown || this.Manager.Version == TfsMessageQueueVersion.V1)
        return;
      lock (this.m_thisLock)
      {
        if (this.m_pendingAcknowledgements == null)
          this.m_pendingAcknowledgements = new List<long>();
        this.m_pendingAcknowledgements.Add(messageId);
        if (this.m_acknowledgementTimer == null)
          this.m_acknowledgementTimer = new Timer(new TimerCallback(this.OnAcknowledgementTimerElapsed), (object) null, -1, -1);
        if (this.m_acknowledgementTimerActive)
          return;
        this.m_acknowledgementTimerActive = true;
        this.m_acknowledgementTimer.Change(this.MaxAcknowledgementDelay, TimeSpan.FromMilliseconds(-1.0));
      }
    }

    public void Close() => this.Close(TimeSpan.FromSeconds(30.0));

    public void Close(TimeSpan timeout)
    {
      if (!this.InitiateShutdown())
        return;
      TfsMessage tfsMessage = (TfsMessage) null;
      try
      {
        AcknowledgementRange[] acknowledgementRanges = this.GetAcknowledgementRanges(true);
        if (acknowledgementRanges.Length == 0)
          return;
        tfsMessage = this.m_channel.Request(this.CreateAcknowledgementMessage(acknowledgementRanges), timeout);
      }
      finally
      {
        tfsMessage?.Close();
        this.Manager.Remove(this);
        this.m_channel.Abort();
      }
    }

    public IAsyncResult BeginClose(AsyncCallback callback, object state) => this.BeginClose(TimeSpan.FromSeconds(30.0), callback, state);

    public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state) => (IAsyncResult) new TfsMessageQueue.CloseOperation(this, timeout, callback, state);

    public void EndClose(IAsyncResult result) => TfsMessageQueue.CloseOperation.End(result);

    public TfsDequeueContext Dequeue(TimeSpan timeout)
    {
      TfsDequeueContext context;
      if (!this.Dequeue(timeout, out context))
        throw new TimeoutException();
      return context;
    }

    public bool Dequeue(TimeSpan timeout, out TfsDequeueContext context)
    {
      context = (TfsDequeueContext) null;
      this.EnsurePolling();
      TfsMessage message;
      if (!this.m_localQueue.Dequeue(timeout, out message))
        return false;
      if (message != null)
        context = new TfsDequeueContext(this, message);
      return true;
    }

    public IAsyncResult BeginDequeue(TimeSpan timeout, AsyncCallback callback, object state)
    {
      this.EnsurePolling();
      return this.m_localQueue.BeginDequeue(timeout, callback, state);
    }

    public TfsDequeueContext EndDequeue(IAsyncResult result)
    {
      TfsDequeueContext context;
      if (!this.EndDequeue(result, out context))
        throw new TimeoutException();
      return context;
    }

    public bool EndDequeue(IAsyncResult result, out TfsDequeueContext context)
    {
      context = (TfsDequeueContext) null;
      TfsMessage message;
      if (!this.m_localQueue.EndDequeue(result, out message))
        return false;
      if (message != null)
        context = new TfsDequeueContext(this, message);
      return true;
    }

    private bool InitiateShutdown()
    {
      lock (this.m_thisLock)
      {
        if (this.m_isShutdown)
          return false;
        this.m_isShutdown = true;
        if (this.m_localQueue != null)
          this.m_localQueue.Close();
        if (this.m_pollTimer != null)
        {
          this.m_pollTimer.Change(-1, -1);
          this.m_pollTimer.Dispose();
          this.m_pollTimer = (Timer) null;
        }
        if (this.m_acknowledgementTimer != null)
        {
          this.m_acknowledgementTimer.Change(-1, -1);
          this.m_acknowledgementTimer.Dispose();
          this.m_acknowledgementTimer = (Timer) null;
        }
        if (this.m_disconnectedTimer != null)
        {
          this.m_disconnectedTimer.Change(-1, -1);
          this.m_disconnectedTimer.Dispose();
          this.m_disconnectedTimer = (Timer) null;
        }
      }
      return true;
    }

    private void EnsurePolling()
    {
      if (!this.m_isShutdown && this.m_localQueue.Count < this.MaxPendingCount)
      {
        if (Interlocked.CompareExchange(ref this.m_pollCount, 1, 0) != 0)
          return;
        AcknowledgementRange[] acknowledgementRanges = this.GetAcknowledgementRanges(true);
        this.m_channel.BeginRequest(this.CreatePollMessage(acknowledgementRanges), new AsyncCallback(this.EndPoll), (object) acknowledgementRanges);
      }
      else
        Interlocked.Exchange(ref this.m_pollingSuspended, 1);
    }

    private void EndPoll(IAsyncResult result)
    {
      bool flag1 = true;
      bool flag2 = true;
      TfsMessage tfsMessage = (TfsMessage) null;
      Exception exception1 = (Exception) null;
      try
      {
        tfsMessage = this.m_channel.EndRequest(result);
        this.OnConnected();
        flag2 = false;
      }
      catch (TimeoutException ex)
      {
      }
      catch (TeamFoundationServerUnauthorizedException ex)
      {
        TeamFoundationTrace.TraceException(TfsmqTraceKeywordSets.MessageQueue, "TfsmqConnection.EndPoll", (Exception) ex);
        exception1 = (Exception) ex;
      }
      catch (RequestDisabledException ex)
      {
        TeamFoundationTrace.TraceException(TfsmqTraceKeywordSets.MessageQueue, "TfsmqConnection.EndPoll", (Exception) ex);
        exception1 = (Exception) ex;
      }
      catch (TeamFoundationServerInvalidResponseException ex)
      {
        TeamFoundationTrace.TraceException(TfsmqTraceKeywordSets.MessageQueue, "TfsmqConnection.EndPoll", (Exception) ex);
        this.OnDisconnected((Exception) ex);
      }
      catch (TeamFoundationServiceUnavailableException ex)
      {
        TeamFoundationTrace.TraceException(TfsmqTraceKeywordSets.MessageQueue, "TfsmqConnection.EndPoll", (Exception) ex);
        this.OnDisconnected((Exception) ex);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(TfsmqTraceKeywordSets.MessageQueue, "TfsmqConnection.EndPoll", ex);
      }
      long num = long.MinValue;
      bool flag3 = tfsMessage != null && tfsMessage != TfsMessage.EmptyMessage;
      if (flag3)
        num = TfsMessageQueueHelpers.ReadMessageIdHeader(this.Manager.Version, (IList<TfsMessageHeader>) tfsMessage.Headers);
      if (num > this.m_lastMessageId)
        this.m_lastMessageId = num;
      else if (num != long.MinValue && tfsMessage != null)
      {
        TeamFoundationTrace.Warning(TfsmqTraceKeywordSets.MessageQueue, "Received duplicate message with ID {0}. Current sequence ID is {1}.", (object) num, (object) this.m_lastMessageId);
        flag1 = false;
        tfsMessage.Close();
        flag3 = false;
      }
      if (flag3)
      {
        if (tfsMessage.IsFault)
        {
          Exception exception2 = tfsMessage.CreateException();
          if (exception2 is SoapException)
            exception2 = this.ConvertException((SoapException) exception2);
          flag1 = true;
          this.m_localQueue.Enqueue(exception2, new ItemDequeuedCallback(this.OnMessageDequeued), false);
        }
        else
        {
          flag1 = false;
          this.m_localQueue.Enqueue(tfsMessage, new ItemDequeuedCallback(this.OnMessageDequeued), false);
        }
      }
      else if (exception1 != null)
        this.m_localQueue.Enqueue(exception1, new ItemDequeuedCallback(this.OnMessageDequeued), false);
      lock (this.m_thisLock)
      {
        if (this.m_isShutdown)
          return;
        if (flag2)
          this.RestoreFailedAcknowledgements((AcknowledgementRange[]) result.AsyncState);
        if (this.m_pollTimer == null)
          this.m_pollTimer = new Timer(new TimerCallback(this.OnPollTimerElapsed), (object) null, -1, -1);
        if (flag1)
        {
          if (this.m_sleepInterval < this.Manager.SleepTimeout)
          {
            this.m_sleepInterval = this.m_sleepInterval.Add(TimeSpan.FromSeconds(1.0));
            if (this.m_sleepInterval > this.Manager.SleepTimeout)
              this.m_sleepInterval = this.Manager.SleepTimeout;
          }
        }
        else
          this.m_sleepInterval = TimeSpan.Zero;
        this.m_pollTimer.Change(this.m_sleepInterval, TimeSpan.FromMilliseconds(-1.0));
      }
    }

    private void RestoreFailedAcknowledgements(AcknowledgementRange[] failedAcks)
    {
      if (failedAcks == null)
        return;
      foreach (AcknowledgementRange failedAck in failedAcks)
      {
        for (long lower = failedAck.Lower; lower <= failedAck.Upper; ++lower)
          this.m_pendingAcknowledgements.Add(lower);
      }
    }

    private void OnMessageDequeued()
    {
      if (Interlocked.CompareExchange(ref this.m_pollingSuspended, 0, 1) != 1)
        return;
      this.EnsurePolling();
    }

    private void OnPollTimerElapsed(object sender)
    {
      Interlocked.Exchange(ref this.m_pollCount, 0);
      this.EnsurePolling();
    }

    private void OnAcknowledgementTimerElapsed(object sender)
    {
      AcknowledgementRange[] acknowledgementRanges = this.GetAcknowledgementRanges(false);
      if (acknowledgementRanges.Length == 0)
        return;
      this.m_channel.BeginRequest(this.CreateAcknowledgementMessage(acknowledgementRanges), new AsyncCallback(this.EndAcknowledge), (object) acknowledgementRanges);
    }

    private void EndAcknowledge(IAsyncResult result)
    {
      bool flag = true;
      TfsMessage tfsMessage = (TfsMessage) null;
      try
      {
        tfsMessage = this.m_channel.EndRequest(result);
        flag = false;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
      finally
      {
        tfsMessage?.Close();
      }
      lock (this.m_thisLock)
      {
        if (this.m_isShutdown)
          return;
        if (flag)
          this.RestoreFailedAcknowledgements((AcknowledgementRange[]) result.AsyncState);
        if (this.m_pendingAcknowledgements.Count > 0)
          this.m_acknowledgementTimer.Change(this.MaxAcknowledgementDelay, TimeSpan.FromMilliseconds(-1.0));
        else
          this.m_acknowledgementTimerActive = false;
      }
    }

    private void OnConnected()
    {
      if (this.Connected == null)
        return;
      bool flag = false;
      lock (this.m_thisLock)
      {
        if (this.m_disconnectedTimer != null)
        {
          flag = true;
          this.m_disconnectedTimer.Dispose();
          this.m_disconnectedTimer = (Timer) null;
        }
      }
      if (!flag || this.Connected == null)
        return;
      this.Connected((object) this, EventArgs.Empty);
    }

    private void OnDisconnected(Exception exception)
    {
      if (this.Disconnected == null)
        return;
      lock (this.m_thisLock)
      {
        if (this.m_disconnectedTimer != null)
          return;
        this.m_pendingException = exception;
        this.m_disconnectedTimer = new Timer(new TimerCallback(TfsMessageQueue.NotifyDisconnectedCallback), (object) this, this.Manager.ReconnectTimeout, TimeSpan.Zero);
      }
    }

    private static void NotifyDisconnectedCallback(object state)
    {
      TfsMessageQueue tfsMessageQueue = (TfsMessageQueue) state;
      try
      {
        tfsMessageQueue.NotifyDisconnected();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void NotifyDisconnected()
    {
      Exception exception = (Exception) null;
      lock (this.m_thisLock)
      {
        if (this.m_disconnectedTimer == null)
          return;
        exception = this.m_pendingException;
      }
      if (this.Disconnected == null)
        return;
      this.Disconnected((object) this, new TfsMessageQueueDisconnectedEventArgs(exception));
    }

    private TfsMessage CreateAcknowledgementMessage(AcknowledgementRange[] ranges)
    {
      TfsMessage message = TfsMessage.CreateMessage("http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2/Acknowledge", TfsMessageQueueHelpers.CreateAcknowledgeWriter((IList<AcknowledgementRange>) ranges));
      message.To = this.Id;
      if (this.AdditionalHeaders.Count > 0)
        this.AdditionalHeaders.ForEach((Action<TfsMessageHeader>) (x => message.Headers.Add(x)));
      return message;
    }

    private TfsMessage CreatePollMessage(AcknowledgementRange[] ranges)
    {
      TfsMessage message = (TfsMessage) null;
      if (this.Manager.Version == TfsMessageQueueVersion.V1)
      {
        message = TfsMessage.CreateMessage("http://docs.oasis-open.org/ws-rx/wsmc/200702/MakeConnection", TfsMessageQueueHelpers.CreateDequeueWriter(this.Manager.Version, this.Id));
        if (this.m_lastMessageId != long.MinValue)
          message.Headers.Add((TfsMessageHeader) new AcknowledgementHeaderV1(this.m_lastMessageId));
      }
      else
      {
        message = TfsMessage.CreateMessage("http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2/Dequeue", TfsMessageQueueHelpers.CreateDequeueWriter(this.Manager.Version, this.Id));
        if (ranges.Length != 0)
          message.Headers.Add((TfsMessageHeader) new AcknowledgementHeaderV2((IList<AcknowledgementRange>) ranges));
        if (this.m_lastMessageId != long.MinValue)
          message.Headers.Add((TfsMessageHeader) new LastMessageHeader(this.m_lastMessageId));
        if (this.AdditionalHeaders.Count > 0)
          this.AdditionalHeaders.ForEach((Action<TfsMessageHeader>) (x => message.Headers.Add(x)));
      }
      message.To = this.Id;
      return message;
    }

    private AcknowledgementRange[] GetAcknowledgementRanges(bool resetAckTimer)
    {
      if (this.Manager.Version == TfsMessageQueueVersion.V1)
        return Array.Empty<AcknowledgementRange>();
      List<AcknowledgementRange> acknowledgementRangeList = new List<AcknowledgementRange>();
      lock (this.m_thisLock)
      {
        if (resetAckTimer && this.m_acknowledgementTimer != null)
        {
          this.m_acknowledgementTimerActive = false;
          this.m_acknowledgementTimer.Change(-1, -1);
        }
        if (this.m_pendingAcknowledgements != null)
        {
          if (this.m_pendingAcknowledgements.Count > 0)
          {
            this.m_pendingAcknowledgements.Sort();
            long upper = this.m_pendingAcknowledgements[0];
            long lower = this.m_pendingAcknowledgements[0];
            for (int index = 1; index < this.m_pendingAcknowledgements.Count; ++index)
            {
              long pendingAcknowledgement = this.m_pendingAcknowledgements[index];
              if (pendingAcknowledgement == upper || pendingAcknowledgement == upper + 1L)
              {
                upper = pendingAcknowledgement;
              }
              else
              {
                acknowledgementRangeList.Add(new AcknowledgementRange(lower, upper));
                lower = pendingAcknowledgement;
                upper = pendingAcknowledgement;
              }
            }
            if (acknowledgementRangeList.Count == 0 || acknowledgementRangeList[acknowledgementRangeList.Count - 1].Lower != lower)
              acknowledgementRangeList.Add(new AcknowledgementRange(lower, upper));
            this.m_pendingAcknowledgements.Clear();
          }
        }
      }
      return acknowledgementRangeList.ToArray();
    }

    private sealed class CloseOperation : Microsoft.TeamFoundation.Framework.Common.AsyncOperation
    {
      private bool m_shutdown;
      private TfsMessageQueue m_queue;

      public CloseOperation(
        TfsMessageQueue queue,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.m_queue = queue;
        if (!this.m_queue.InitiateShutdown())
          this.Complete(true);
        this.m_shutdown = true;
        bool flag = true;
        AcknowledgementRange[] acknowledgementRanges = this.m_queue.GetAcknowledgementRanges(true);
        if (acknowledgementRanges.Length != 0)
        {
          IAsyncResult result = this.m_queue.m_channel.BeginRequest(this.m_queue.CreateAcknowledgementMessage(acknowledgementRanges), timeout, new AsyncCallback(TfsMessageQueue.CloseOperation.EndRequest), (object) this);
          flag = result.CompletedSynchronously && this.CompleteRequest(result);
        }
        if (!flag)
          return;
        this.Complete(true);
      }

      protected override void Dispose()
      {
        if (this.m_shutdown)
        {
          if (this.m_queue.m_channel != null)
            this.m_queue.m_channel.Abort();
          this.m_queue.Manager.Remove(this.m_queue);
        }
        base.Dispose();
      }

      private static void EndRequest(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        TfsMessageQueue.CloseOperation asyncState = (TfsMessageQueue.CloseOperation) result.AsyncState;
        bool flag = true;
        Exception exception = (Exception) null;
        try
        {
          flag = asyncState.CompleteRequest(result);
        }
        catch (Exception ex)
        {
          exception = ex;
        }
        if (!flag)
          return;
        asyncState.Complete(false, exception);
      }

      private bool CompleteRequest(IAsyncResult result)
      {
        TfsMessage tfsMessage = (TfsMessage) null;
        try
        {
          tfsMessage = this.m_queue.m_channel.EndRequest(result);
          return true;
        }
        finally
        {
          tfsMessage?.Close();
        }
      }

      public static void End(IAsyncResult result) => Microsoft.TeamFoundation.Framework.Common.AsyncOperation.End<TfsMessageQueue.CloseOperation>(result);
    }
  }
}
