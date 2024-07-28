// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.Topic
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public class Topic
  {
    private readonly TimeSpan _lifespan;
    private DateTime _lastUsed = DateTime.UtcNow;
    internal int State;

    public IList<ISubscription> Subscriptions { get; private set; }

    public MessageStore<Message> Store { get; private set; }

    public ReaderWriterLockSlim SubscriptionLock { get; private set; }

    public virtual bool IsExpired
    {
      get
      {
        try
        {
          this.SubscriptionLock.EnterReadLock();
          TimeSpan timeSpan = DateTime.UtcNow - this._lastUsed;
          return this.Subscriptions.Count == 0 && timeSpan > this._lifespan;
        }
        finally
        {
          this.SubscriptionLock.ExitReadLock();
        }
      }
    }

    public DateTime LastUsed => this._lastUsed;

    public Topic(uint storeSize, TimeSpan lifespan)
    {
      this._lifespan = lifespan;
      this.Subscriptions = (IList<ISubscription>) new List<ISubscription>();
      this.Store = new MessageStore<Message>(storeSize);
      this.SubscriptionLock = new ReaderWriterLockSlim();
    }

    public void MarkUsed() => this._lastUsed = DateTime.UtcNow;

    public void AddSubscription(ISubscription subscription)
    {
      if (subscription == null)
        throw new ArgumentNullException(nameof (subscription));
      try
      {
        this.SubscriptionLock.EnterWriteLock();
        this.MarkUsed();
        this.Subscriptions.Add(subscription);
        Interlocked.CompareExchange(ref this.State, 1, 0);
      }
      finally
      {
        this.SubscriptionLock.ExitWriteLock();
      }
    }

    public void RemoveSubscription(ISubscription subscription)
    {
      if (subscription == null)
        throw new ArgumentNullException(nameof (subscription));
      try
      {
        this.SubscriptionLock.EnterWriteLock();
        this.MarkUsed();
        this.Subscriptions.Remove(subscription);
        if (this.Subscriptions.Count != 0)
          return;
        Interlocked.CompareExchange(ref this.State, 0, 1);
      }
      finally
      {
        this.SubscriptionLock.ExitWriteLock();
      }
    }
  }
}
