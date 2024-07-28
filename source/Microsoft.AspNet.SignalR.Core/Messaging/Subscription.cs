// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.Subscription
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public abstract class Subscription : ISubscription, IDisposable
  {
    private readonly Func<MessageResult, object, Task<bool>> _callback;
    private readonly object _callbackState;
    private readonly IPerformanceCounterManager _counters;
    private int _state;
    private int _subscriptionState;

    private bool Alive => this._subscriptionState != 2;

    public string Identity { get; private set; }

    public IList<string> EventKeys { get; private set; }

    public int MaxMessages { get; private set; }

    public IDisposable Disposable { get; set; }

    protected Subscription(
      string identity,
      IList<string> eventKeys,
      Func<MessageResult, object, Task<bool>> callback,
      int maxMessages,
      IPerformanceCounterManager counters,
      object state)
    {
      if (string.IsNullOrEmpty(identity))
        throw new ArgumentNullException(nameof (identity));
      if (eventKeys == null)
        throw new ArgumentNullException(nameof (eventKeys));
      if (callback == null)
        throw new ArgumentNullException(nameof (callback));
      if (maxMessages < 0)
        throw new ArgumentOutOfRangeException(nameof (maxMessages));
      if (counters == null)
        throw new ArgumentNullException(nameof (counters));
      this.Identity = identity;
      this._callback = callback;
      this.EventKeys = eventKeys;
      this.MaxMessages = maxMessages;
      this._counters = counters;
      this._callbackState = state;
      this._counters.MessageBusSubscribersTotal.Increment();
      this._counters.MessageBusSubscribersCurrent.Increment();
      this._counters.MessageBusSubscribersPerSec.Increment();
    }

    public virtual Task<bool> Invoke(MessageResult result) => this.Invoke(result, (Action<Subscription, object>) ((s, o) => { }), (object) null);

    private async Task<bool> Invoke(
      MessageResult result,
      Action<Subscription, object> beforeInvoke,
      object state)
    {
      Subscription subscription = this;
      if (Interlocked.CompareExchange(ref subscription._subscriptionState, 1, 0) == 2 && !result.Terminal)
        return false;
      beforeInvoke(subscription, state);
      subscription._counters.MessageBusMessagesReceivedTotal.IncrementBy((long) result.TotalCount);
      subscription._counters.MessageBusMessagesReceivedPerSec.IncrementBy((long) result.TotalCount);
      try
      {
        return await subscription._callback(result, subscription._callbackState);
      }
      finally
      {
        Interlocked.CompareExchange(ref subscription._subscriptionState, 0, 1);
      }
    }

    public async Task Work()
    {
      Interlocked.Exchange(ref this._state, 1);
      List<ArraySegment<Message>> items = new List<ArraySegment<Message>>();
      while (this.Alive)
      {
        items.Clear();
        int totalCount;
        object state;
        this.PerformWork((IList<ArraySegment<Message>>) items, out totalCount, out state);
        if (items.Count <= 0)
          break;
        if (!await this.Invoke(new MessageResult((IList<ArraySegment<Message>>) items, totalCount), (Action<Subscription, object>) ((s, o) => s.BeforeInvoke(o)), state))
        {
          this.Dispose();
          break;
        }
      }
    }

    public bool SetQueued() => Interlocked.Increment(ref this._state) == 1;

    public bool UnsetQueued() => Interlocked.CompareExchange(ref this._state, 0, 1) != 1;

    protected virtual void BeforeInvoke(object state)
    {
    }

    protected abstract void PerformWork(
      IList<ArraySegment<Message>> items,
      out int totalCount,
      out object state);

    public virtual bool AddEvent(string key, Topic topic) => this.AddEventCore(key);

    public virtual void RemoveEvent(string key)
    {
      lock (this.EventKeys)
        this.EventKeys.Remove(key);
    }

    public virtual void SetEventTopic(string key, Topic topic) => this.AddEventCore(key);

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      SpinWait spinWait = new SpinWait();
      while (true)
      {
        switch (Interlocked.CompareExchange(ref this._subscriptionState, 2, 0))
        {
          case 1:
            spinWait.SpinOnce();
            continue;
          case 2:
            goto label_4;
          default:
            goto label_3;
        }
      }
label_3:
      this._counters.MessageBusSubscribersCurrent.Decrement();
      this._counters.MessageBusSubscribersPerSec.Decrement();
label_4:
      if (this.Disposable == null)
        return;
      this.Disposable.Dispose();
    }

    public void Dispose() => this.Dispose(true);

    public abstract void WriteCursor(TextWriter textWriter);

    private bool AddEventCore(string key)
    {
      lock (this.EventKeys)
      {
        if (this.EventKeys.Contains(key))
          return false;
        this.EventKeys.Add(key);
        return true;
      }
    }

    private static class State
    {
      public const int Idle = 0;
      public const int Working = 1;
    }

    private static class SubscriptionState
    {
      public const int Idle = 0;
      public const int InvokingCallback = 1;
      public const int Disposed = 2;
    }
  }
}
