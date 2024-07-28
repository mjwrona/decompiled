// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.MessageBroker
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public class MessageBroker : IDisposable
  {
    private readonly IPerformanceCounterManager _counters;
    private volatile bool _disposed;

    public MessageBroker(
      IPerformanceCounterManager performanceCounterManager)
    {
      this._counters = performanceCounterManager;
    }

    public TraceSource Trace { get; set; }

    public void Schedule(ISubscription subscription)
    {
      if (subscription == null)
        throw new ArgumentNullException(nameof (subscription));
      if (this._disposed || !subscription.SetQueued())
        return;
      this.ScheduleWork(subscription);
    }

    private void ScheduleWork(ISubscription subscription)
    {
      ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(Worker), (object) new MessageBroker.WorkContext(subscription, this));

      async void Worker(object state)
      {
        MessageBroker.WorkContext context = (MessageBroker.WorkContext) state;
        context.Broker._counters.MessageBusAllocatedWorkers.Increment();
        await MessageBroker.DoWork(context);
        context.Broker._counters.MessageBusAllocatedWorkers.Decrement();
      }
    }

    internal static async Task DoWork(MessageBroker.WorkContext context)
    {
      do
      {
        try
        {
          context.Broker._counters.MessageBusBusyWorkers.Increment();
          await context.Subscription.Work();
        }
        catch (Exception ex)
        {
          context.Broker.Trace.TraceError("Failed to process work - " + (object) ex.GetBaseException());
          if (!(context.Subscription is IDisposable subscription))
            break;
          subscription.Dispose();
          break;
        }
        finally
        {
          context.Broker._counters.MessageBusBusyWorkers.Decrement();
        }
      }
      while (context.Subscription.UnsetQueued() && !context.Broker._disposed);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this._disposed = true;
    }

    public void Dispose() => this.Dispose(true);

    internal class WorkContext
    {
      public ISubscription Subscription { get; }

      public MessageBroker Broker { get; }

      public WorkContext(ISubscription subscription, MessageBroker broker)
      {
        this.Subscription = subscription;
        this.Broker = broker;
      }
    }
  }
}
