// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueueOperation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Channels;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal abstract class QueueOperation : AsyncOperation, ICancelable
  {
    private MessageContainer m_message;
    private bool m_acquiredQueue;
    private TimeoutHelper m_timeout;
    private InputQueue<MessageContainer> m_inputQueue;
    private readonly long m_requestId;
    private static readonly string s_layer = nameof (QueueOperation);
    private static readonly string s_area = "MessageQueue";

    protected QueueOperation(
      IVssRequestContext requestContext,
      TeamFoundationMessageQueue queue,
      Guid sessionId,
      IList<AcknowledgementRange> acknowledgements,
      MessageHeaders headers,
      TimeSpan timeout,
      TfsMessageQueueVersion version,
      AsyncCallback callback,
      object state)
      : base(callback, state)
    {
      this.m_timeout = new TimeoutHelper(timeout);
      this.m_inputQueue = new InputQueue<MessageContainer>();
      this.m_requestId = ((VssRequestContext) requestContext).RequestId;
      this.Acknowledgements = acknowledgements;
      this.Headers = headers;
      this.Queue = queue;
      this.RequestContext = requestContext;
      this.SequenceId = this.Queue.SequenceId;
      this.SessionId = sessionId;
      this.Version = version;
    }

    private IList<AcknowledgementRange> Acknowledgements { get; set; }

    private MessageHeaders Headers { get; set; }

    internal int QueueId => this.Queue.Id;

    internal TeamFoundationMessageQueue Queue { get; private set; }

    public IVssRequestContext RequestContext { get; private set; }

    public int SequenceId { get; internal set; }

    public Guid SessionId { get; private set; }

    internal TfsMessageQueueVersion Version { get; private set; }

    public void Begin()
    {
      this.RequestContext.TraceEnter(0, QueueOperation.s_area, QueueOperation.s_layer, "BeginDequeueOperation");
      bool flag = true;
      Exception exception = (Exception) null;
      TfsmqDequeueEvent notificationEvent = new TfsmqDequeueEvent(this.Queue.Name, this.SessionId, this.Headers, this.Version);
      try
      {
        this.RequestContext.GetService<TeamFoundationEventService>().PublishDecisionPoint(this.RequestContext, (object) notificationEvent);
        this.m_acquiredQueue = this.AcquireQueue();
        if (this.m_acquiredQueue)
        {
          TimeSpan timeout = this.m_timeout.RemainingTime();
          IAsyncResult result = this.m_inputQueue.BeginDequeue(timeout, new AsyncCallback(QueueOperation.EndDequeue), (object) this);
          this.RequestContext.Trace(0, TraceLevel.Verbose, QueueOperation.s_area, QueueOperation.s_layer, "Began dequeuing message with timeout of '{0}' second(s)", (object) timeout.TotalSeconds);
          if (result.CompletedSynchronously)
          {
            this.CompleteDequeue(result, true);
            this.RequestContext.Trace(0, TraceLevel.Info, QueueOperation.s_area, QueueOperation.s_layer, "Dequeue completed synchronously");
            return;
          }
          this.RequestContext.EnterCancelableRegion((ICancelable) this);
          this.RequestContext.GetService<TeamFoundationMessageQueueService>().ReadyForDispatch(this.RequestContext, this);
          flag = false;
        }
      }
      catch (ActionDeniedBySubscriberException ex)
      {
        this.RequestContext.Trace(0, TraceLevel.Error, QueueOperation.s_area, QueueOperation.s_layer, "Denied dequeue on message queue {0}", (object) this.Queue.Name);
        exception = notificationEvent.Error ?? (Exception) new MessageQueueNotFoundException(this.Queue.Name);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, QueueOperation.s_area, QueueOperation.s_layer, ex);
        exception = ex;
      }
      if (flag)
      {
        this.RequestContext.Trace(0, TraceLevel.Info, QueueOperation.s_area, QueueOperation.s_layer, "The dequeue operation is aborted");
        this.RequestContext.ExitCancelableRegion((ICancelable) this);
        if (this.m_acquiredQueue)
          this.ReleaseQueue();
        this.Complete(true, exception);
      }
      this.RequestContext.TraceLeave(0, QueueOperation.s_area, QueueOperation.s_layer, "BeginDequeueOperation");
    }

    private static void EndDequeue(IAsyncResult result)
    {
      if (result.CompletedSynchronously)
        return;
      ((QueueOperation) result.AsyncState).CompleteDequeue(result, false);
    }

    private void CompleteDequeue(IAsyncResult result, bool synchronous)
    {
      using (new VssRequestLockScope(this.m_requestId))
      {
        Exception exception = (Exception) null;
        try
        {
          this.m_inputQueue.EndDequeue(result, out this.m_message);
          this.RequestContext.ExitCancelableRegion((ICancelable) this);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, QueueOperation.s_area, QueueOperation.s_layer, ex);
          exception = ex;
        }
        finally
        {
          if (this.m_inputQueue != null)
            this.m_inputQueue.Close();
          if (this.m_acquiredQueue)
            this.ReleaseQueue();
          this.Complete(synchronous, exception);
        }
      }
    }

    protected abstract bool AcquireQueue();

    protected abstract void ReleaseQueue();

    internal DequeueOperation GetItemsForDispatch(List<AcknowledgementRange> acknowledgements)
    {
      if (this.Acknowledgements != null && this.Acknowledgements.Count > 0)
      {
        IList<AcknowledgementRange> acknowledgements1 = this.Acknowledgements;
        this.Acknowledgements = (IList<AcknowledgementRange>) null;
        acknowledgements.AddRange(acknowledgements1.Select<AcknowledgementRange, AcknowledgementRange>((Func<AcknowledgementRange, AcknowledgementRange>) (x =>
        {
          x.QueueId = this.QueueId;
          return x;
        })));
      }
      return this as DequeueOperation;
    }

    public void Cancel()
    {
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, QueueOperation.s_area, QueueOperation.s_layer, "CancelQueueOperation");
      this.m_inputQueue.Shutdown();
    }

    public void Dispatch(MessageContainer message, bool canDispatchOnThisThread)
    {
      IVssRequestContext rootContext = this.RequestContext.RootContext;
      if (rootContext.IsCanceled || rootContext is ITrackClientConnection && !((ITrackClientConnection) rootContext).IsClientConnected)
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Verbose, QueueOperation.s_area, QueueOperation.s_layer, "Message cannot be dispatched as client disconnected");
        message = (MessageContainer) null;
      }
      this.m_inputQueue.Enqueue(message, canDispatchOnThisThread);
    }

    internal MessageContainer End()
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, QueueOperation.s_area, QueueOperation.s_layer, "EndQueueOperation");
      return AsyncOperation.End<QueueOperation>((IAsyncResult) this).m_message;
    }
  }
}
