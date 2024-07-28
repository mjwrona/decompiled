// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.MessageBus
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public class MessageBus : IMessageBus, IDisposable
  {
    private readonly MessageBroker _broker;
    private readonly uint _messageStoreSize;
    private readonly int _maxTopicsWithNoSubscriptions;
    private readonly IStringMinifier _stringMinifier;
    private readonly ITraceManager _traceManager;
    private readonly TraceSource _trace;
    private Timer _gcTimer;
    private int _gcRunning;
    private static readonly TimeSpan _gcInterval = TimeSpan.FromSeconds(5.0);
    private readonly TimeSpan _topicTtl;
    internal Action<string, Topic> BeforeTopicGarbageCollected;
    internal Action<string, Topic> AfterTopicGarbageCollected;
    internal Action<string, Topic> BeforeTopicMarked;
    internal Action<string> BeforeTopicCreated;
    internal Action<string, Topic> AfterTopicMarkedSuccessfully;
    internal Action<string, Topic, int> AfterTopicMarked;
    private const int DefaultMaxTopicsWithNoSubscriptions = 1000;
    private readonly Func<string, Topic> _createTopic;
    private readonly Action<ISubscriber, string> _addEvent;
    private readonly Action<ISubscriber, string> _removeEvent;
    private readonly Action<object> _disposeSubscription;

    public MessageBus(IDependencyResolver resolver)
      : this(resolver.Resolve<IStringMinifier>(), resolver.Resolve<ITraceManager>(), resolver.Resolve<IPerformanceCounterManager>(), resolver.Resolve<IConfigurationManager>(), 1000)
    {
    }

    public MessageBus(
      IStringMinifier stringMinifier,
      ITraceManager traceManager,
      IPerformanceCounterManager performanceCounterManager,
      IConfigurationManager configurationManager,
      int maxTopicsWithNoSubscriptions)
    {
      if (stringMinifier == null)
        throw new ArgumentNullException(nameof (stringMinifier));
      if (traceManager == null)
        throw new ArgumentNullException(nameof (traceManager));
      if (performanceCounterManager == null)
        throw new ArgumentNullException(nameof (performanceCounterManager));
      if (configurationManager == null)
        throw new ArgumentNullException(nameof (configurationManager));
      if (configurationManager.DefaultMessageBufferSize < 0)
        throw new ArgumentOutOfRangeException(Resources.Error_BufferSizeOutOfRange);
      this._stringMinifier = stringMinifier;
      this._traceManager = traceManager;
      this.Counters = performanceCounterManager;
      this._trace = this._traceManager["SignalR." + typeof (MessageBus).Name];
      this._maxTopicsWithNoSubscriptions = maxTopicsWithNoSubscriptions;
      this._gcTimer = new Timer((TimerCallback) (_ => this.GarbageCollectTopics()), (object) null, MessageBus._gcInterval, MessageBus._gcInterval);
      this._broker = new MessageBroker(this.Counters)
      {
        Trace = this._trace
      };
      this._messageStoreSize = (uint) configurationManager.DefaultMessageBufferSize;
      this._topicTtl = configurationManager.TopicTtl();
      this._createTopic = new Func<string, Topic>(this.CreateTopic);
      this._addEvent = new Action<ISubscriber, string>(this.AddEvent);
      this._removeEvent = new Action<ISubscriber, string>(this.RemoveEvent);
      this._disposeSubscription = (Action<object>) (o => this.DisposeSubscription(o));
      this.Topics = new TopicLookup();
    }

    protected internal TopicLookup Topics { get; private set; }

    protected IPerformanceCounterManager Counters { get; private set; }

    public virtual Task Publish(Message message)
    {
      if (message == null)
        throw new ArgumentNullException(nameof (message));
      Topic topic;
      if (this.Topics.TryGetValue(message.Key, out topic))
      {
        long num = (long) topic.Store.Add(message);
        this.ScheduleTopic(topic);
      }
      this.Counters.MessageBusMessagesPublishedTotal.Increment();
      this.Counters.MessageBusMessagesPublishedPerSec.Increment();
      return TaskAsyncHelper.Empty;
    }

    protected ulong Save(Message message)
    {
      Topic topic = message != null ? this.GetTopic(message.Key) : throw new ArgumentNullException(nameof (message));
      topic.MarkUsed();
      return topic.Store.Add(message);
    }

    public virtual IDisposable Subscribe(
      ISubscriber subscriber,
      string cursor,
      Func<MessageResult, object, Task<bool>> callback,
      int maxMessages,
      object state)
    {
      if (subscriber == null)
        throw new ArgumentNullException(nameof (subscriber));
      if (callback == null)
        throw new ArgumentNullException(nameof (callback));
      Subscription subscription = this.CreateSubscription(subscriber, cursor, callback, maxMessages, state);
      subscriber.Subscription = subscription;
      HashSet<Topic> topicSet = new HashSet<Topic>();
      foreach (string eventKey in (IEnumerable<string>) subscriber.EventKeys)
      {
        Topic topic = this.SubscribeTopic(eventKey);
        subscription.SetEventTopic(eventKey, topic);
        topicSet.Add(topic);
      }
      subscriber.EventKeyAdded += this._addEvent;
      subscriber.EventKeyRemoved += this._removeEvent;
      subscriber.WriteCursor = new Action<TextWriter>(subscription.WriteCursor);
      MessageBus.SubscriptionState state1 = new MessageBus.SubscriptionState(subscriber);
      DisposableAction disposableAction = new DisposableAction(this._disposeSubscription, (object) state1);
      subscription.Disposable = (IDisposable) disposableAction;
      foreach (Topic topic in topicSet)
        topic.AddSubscription((ISubscription) subscription);
      state1.Initialized.Set();
      if (!string.IsNullOrEmpty(cursor))
        this._broker.Schedule((ISubscription) subscription);
      return (IDisposable) disposableAction;
    }

    protected virtual Subscription CreateSubscription(
      ISubscriber subscriber,
      string cursor,
      Func<MessageResult, object, Task<bool>> callback,
      int messageBufferSize,
      object state)
    {
      return (Subscription) new DefaultSubscription(subscriber.Identity, subscriber.EventKeys, this.Topics, cursor, callback, messageBufferSize, this._stringMinifier, this.Counters, state);
    }

    protected void ScheduleEvent(string eventKey)
    {
      Topic topic;
      if (!this.Topics.TryGetValue(eventKey, out topic))
        return;
      this.ScheduleTopic(topic);
    }

    private void ScheduleTopic(Topic topic)
    {
      try
      {
        topic.SubscriptionLock.EnterReadLock();
        for (int index = 0; index < topic.Subscriptions.Count; ++index)
          this._broker.Schedule(topic.Subscriptions[index]);
      }
      finally
      {
        topic.SubscriptionLock.ExitReadLock();
      }
    }

    protected virtual Topic CreateTopic(string key)
    {
      this.Counters.MessageBusTopicsCurrent.Increment();
      return new Topic(this._messageStoreSize, this._topicTtl);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this._gcRunning == 2)
        return;
      this._broker.Dispose();
      while (Interlocked.CompareExchange(ref this._gcRunning, 2, 0) == 1)
        Thread.Sleep(250);
      this.Topics.Clear();
      if (this._gcTimer == null)
        return;
      this._gcTimer.Dispose();
    }

    public void Dispose() => this.Dispose(true);

    internal void GarbageCollectTopics()
    {
      if (Interlocked.CompareExchange(ref this._gcRunning, 1, 0) != 0)
        return;
      int num1 = 0;
      foreach (KeyValuePair<string, Topic> topic in this.Topics)
      {
        if (topic.Value.IsExpired)
        {
          if (this.BeforeTopicGarbageCollected != null)
            this.BeforeTopicGarbageCollected(topic.Key, topic.Value);
          this.DestroyTopic(topic.Key, topic.Value);
        }
        else if (topic.Value.State == 0)
          ++num1;
      }
      int num2 = num1 - this._maxTopicsWithNoSubscriptions;
      if (num2 > 0)
      {
        List<KeyValuePair<string, Topic>> keyValuePairList = new List<KeyValuePair<string, Topic>>();
        foreach (KeyValuePair<string, Topic> topic in this.Topics)
        {
          if (topic.Value.State == 0)
            keyValuePairList.Add(topic);
        }
        keyValuePairList.Sort((Comparison<KeyValuePair<string, Topic>>) ((leftPair, rightPair) => leftPair.Value.LastUsed.CompareTo(rightPair.Value.LastUsed)));
        for (int index = 0; index < num2 && index < keyValuePairList.Count; ++index)
        {
          KeyValuePair<string, Topic> keyValuePair = keyValuePairList[index];
          if (InterlockedHelper.CompareExchangeOr(ref keyValuePair.Value.State, 3, 0, 2))
            this.DestroyTopicCore(keyValuePair.Key, keyValuePair.Value);
        }
      }
      Interlocked.CompareExchange(ref this._gcRunning, 0, 1);
    }

    private void DestroyTopic(string key, Topic topic)
    {
      if (Interlocked.CompareExchange(ref topic.State, 2, 0) != 2 || Interlocked.CompareExchange(ref topic.State, 3, 2) != 2)
        return;
      this.DestroyTopicCore(key, topic);
    }

    private void DestroyTopicCore(string key, Topic topic)
    {
      this.Topics.TryRemove(key);
      this._stringMinifier.RemoveUnminified(key);
      this.Counters.MessageBusTopicsCurrent.Decrement();
      this._trace.TraceInformation("RemoveTopic(" + key + ")");
      if (this.AfterTopicGarbageCollected == null)
        return;
      this.AfterTopicGarbageCollected(key, topic);
    }

    internal Topic GetTopic(string key)
    {
      Topic orAdd;
      int num;
      do
      {
        if (this.BeforeTopicCreated != null)
          this.BeforeTopicCreated(key);
        orAdd = this.Topics.GetOrAdd(key, this._createTopic);
        if (this.BeforeTopicMarked != null)
          this.BeforeTopicMarked(key, orAdd);
        num = Interlocked.CompareExchange(ref orAdd.State, 0, 2);
        if (this.AfterTopicMarked != null)
          this.AfterTopicMarked(key, orAdd, orAdd.State);
      }
      while (num == 3);
      if (this.AfterTopicMarkedSuccessfully != null)
        this.AfterTopicMarkedSuccessfully(key, orAdd);
      return orAdd;
    }

    internal Topic SubscribeTopic(string key)
    {
      Topic orAdd;
      do
      {
        if (this.BeforeTopicCreated != null)
          this.BeforeTopicCreated(key);
        orAdd = this.Topics.GetOrAdd(key, this._createTopic);
        if (this.BeforeTopicMarked != null)
          this.BeforeTopicMarked(key, orAdd);
        InterlockedHelper.CompareExchangeOr(ref orAdd.State, 1, 0, 2);
        if (this.AfterTopicMarked != null)
          this.AfterTopicMarked(key, orAdd, orAdd.State);
      }
      while (orAdd.State != 1);
      if (this.AfterTopicMarkedSuccessfully != null)
        this.AfterTopicMarkedSuccessfully(key, orAdd);
      return orAdd;
    }

    private void AddEvent(ISubscriber subscriber, string eventKey)
    {
      Topic topic = this.SubscribeTopic(eventKey);
      if (!subscriber.Subscription.AddEvent(eventKey, topic))
        return;
      topic.AddSubscription((ISubscription) subscriber.Subscription);
    }

    private void RemoveEvent(ISubscriber subscriber, string eventKey)
    {
      Topic topic;
      if (!this.Topics.TryGetValue(eventKey, out topic))
        return;
      topic.RemoveSubscription((ISubscription) subscriber.Subscription);
      subscriber.Subscription.RemoveEvent(eventKey);
    }

    private void DisposeSubscription(object state)
    {
      MessageBus.SubscriptionState subscriptionState = (MessageBus.SubscriptionState) state;
      ISubscriber subscriber = subscriptionState.Subscriber;
      subscriber.Subscription.Dispose();
      try
      {
        subscriber.Subscription.Invoke(MessageResult.TerminalMessage).Wait();
      }
      catch
      {
      }
      subscriptionState.Initialized.Wait();
      subscriber.EventKeyAdded -= this._addEvent;
      subscriber.EventKeyRemoved -= this._removeEvent;
      subscriber.WriteCursor = (Action<TextWriter>) null;
      for (int index = subscriber.EventKeys.Count - 1; index >= 0; --index)
      {
        string eventKey = subscriber.EventKeys[index];
        this.RemoveEvent(subscriber, eventKey);
      }
    }

    private class SubscriptionState
    {
      public ISubscriber Subscriber { get; private set; }

      public ManualResetEventSlim Initialized { get; private set; }

      public SubscriptionState(ISubscriber subscriber)
      {
        this.Initialized = new ManualResetEventSlim();
        this.Subscriber = subscriber;
      }
    }

    private static class GCState
    {
      public const int Idle = 0;
      public const int Running = 1;
      public const int Disposed = 2;
    }
  }
}
