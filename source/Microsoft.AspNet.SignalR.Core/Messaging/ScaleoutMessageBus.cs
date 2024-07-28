// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.ScaleoutMessageBus
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public abstract class ScaleoutMessageBus : MessageBus
  {
    private readonly SipHashBasedStringEqualityComparer _sipHashBasedComparer = new SipHashBasedStringEqualityComparer(0UL, 0UL);
    private readonly ITraceManager _traceManager;
    private readonly TraceSource _trace;
    private readonly Lazy<ScaleoutStreamManager> _streamManager;
    private readonly IPerformanceCounterManager _perfCounters;

    protected ScaleoutMessageBus(IDependencyResolver resolver, ScaleoutConfiguration configuration)
      : base(resolver)
    {
      ScaleoutMessageBus scaleoutMessageBus = this;
      if (configuration == null)
        throw new ArgumentNullException(nameof (configuration));
      this._traceManager = resolver.Resolve<ITraceManager>();
      this._trace = this._traceManager["SignalR." + typeof (ScaleoutMessageBus).Name];
      this._perfCounters = resolver.Resolve<IPerformanceCounterManager>();
      int maxScaloutMappings = resolver.Resolve<IConfigurationManager>().MaxScaleoutMappingsPerStream;
      this._streamManager = new Lazy<ScaleoutStreamManager>((Func<ScaleoutStreamManager>) (() => new ScaleoutStreamManager(new Func<int, IList<Message>, Task>(closure_0.Send), new Action<int, ulong, ScaleoutMessage>(closure_0.OnReceivedCore), closure_0.StreamCount, closure_0._trace, closure_0._perfCounters, configuration, maxScaloutMappings)));
    }

    protected virtual int StreamCount => 1;

    private ScaleoutStreamManager StreamManager => this._streamManager.Value;

    protected void Open(int streamIndex) => this.StreamManager.Open(streamIndex);

    protected void Close(int streamIndex) => this.StreamManager.Close(streamIndex);

    protected void OnError(int streamIndex, Exception exception) => this.StreamManager.OnError(streamIndex, exception);

    protected virtual Task Send(IList<Message> messages)
    {
      if (this.StreamCount == 1)
        return this.StreamManager.Send(0, messages);
      DispatchingTaskCompletionSource<object> taskCompletionSource = new DispatchingTaskCompletionSource<object>();
      this.SendImpl(messages.GroupBy<Message, string>((Func<Message, string>) (m => m.Source)).GetEnumerator(), taskCompletionSource);
      return (Task) taskCompletionSource.Task;
    }

    protected virtual Task Send(int streamIndex, IList<Message> messages) => throw new NotImplementedException();

    private void SendImpl(
      IEnumerator<IGrouping<string, Message>> enumerator,
      DispatchingTaskCompletionSource<object> taskCompletionSource)
    {
      while (enumerator.MoveNext())
      {
        IGrouping<string, Message> current = enumerator.Current;
        Task task = this.StreamManager.Send((int) ((long) (uint) this._sipHashBasedComparer.GetHashCode(current.Key) % (long) this.StreamCount), (IList<Message>) current.ToArray<Message>()).Catch<Task>(this._trace);
        if (task.IsCompleted)
        {
          try
          {
            task.Wait();
          }
          catch (Exception ex)
          {
            taskCompletionSource.SetUnwrappedException(ex);
            return;
          }
        }
        else
        {
          task.Then<IEnumerator<IGrouping<string, Message>>, DispatchingTaskCompletionSource<object>>((Action<IEnumerator<IGrouping<string, Message>>, DispatchingTaskCompletionSource<object>>) ((enumer, tcs) => this.SendImpl(enumer, tcs)), enumerator, taskCompletionSource).ContinueWithNotComplete(taskCompletionSource);
          return;
        }
      }
      taskCompletionSource.TrySetResult((object) null);
    }

    protected virtual void OnReceived(int streamIndex, ulong id, ScaleoutMessage message) => this.StreamManager.OnReceived(streamIndex, id, message);

    private void OnReceivedCore(int streamIndex, ulong id, ScaleoutMessage scaleoutMessage)
    {
      this.Counters.ScaleoutMessageBusMessagesReceivedPerSec.IncrementBy((long) scaleoutMessage.Messages.Count);
      this._trace.TraceInformation("OnReceived({0}, {1}, {2})", (object) streamIndex, (object) id, (object) scaleoutMessage.Messages.Count);
      this.TraceScaleoutMessages(id, scaleoutMessage);
      LocalEventKeyInfo[] localKeyInfo = new LocalEventKeyInfo[scaleoutMessage.Messages.Count];
      HashSet<string> values = new HashSet<string>();
      for (int index = 0; index < scaleoutMessage.Messages.Count; ++index)
      {
        Message message = scaleoutMessage.Messages[index];
        message.MappingId = id;
        message.StreamIndex = streamIndex;
        values.Add(message.Key);
        ulong id1 = this.Save(message);
        this._trace.TraceVerbose("Message id: {0}, stream : {1}, eventKey: '{2}' saved with local id: {3}", (object) id, (object) streamIndex, (object) message.Key, (object) id1);
        MessageStore<Message> store = this.Topics[message.Key].Store;
        localKeyInfo[index] = new LocalEventKeyInfo(message.Key, id1, store);
      }
      this.StreamManager.Streams[streamIndex].Add(id, scaleoutMessage, (IList<LocalEventKeyInfo>) localKeyInfo);
      if (this._trace.Switch.ShouldTrace(TraceEventType.Verbose))
        this._trace.TraceVerbose("Scheduling eventkeys: {0}", (object) string.Join(",", (IEnumerable<string>) values));
      foreach (string eventKey in values)
        this.ScheduleEvent(eventKey);
    }

    private void TraceScaleoutMessages(ulong id, ScaleoutMessage scaleoutMessage)
    {
      if (!this._trace.Switch.ShouldTrace(TraceEventType.Verbose))
        return;
      foreach (Message message in (IEnumerable<Message>) scaleoutMessage.Messages)
        this._trace.TraceVerbose("Received message {0}: '{1}' over ScaleoutMessageBus", (object) id, (object) message.GetString());
    }

    public override Task Publish(Message message)
    {
      this.Counters.MessageBusMessagesPublishedTotal.Increment();
      this.Counters.MessageBusMessagesPublishedPerSec.Increment();
      return this.Send((IList<Message>) new Message[1]
      {
        message
      });
    }

    protected override void Dispose(bool disposing)
    {
      for (int streamIndex = 0; streamIndex < this.StreamCount; ++streamIndex)
        this.Close(streamIndex);
      base.Dispose(disposing);
    }

    protected override Subscription CreateSubscription(
      ISubscriber subscriber,
      string cursor,
      Func<MessageResult, object, Task<bool>> callback,
      int messageBufferSize,
      object state)
    {
      return (Subscription) new ScaleoutSubscription(subscriber.Identity, subscriber.EventKeys, cursor, this.StreamManager.Streams, callback, messageBufferSize, this._traceManager, this.Counters, state);
    }
  }
}
