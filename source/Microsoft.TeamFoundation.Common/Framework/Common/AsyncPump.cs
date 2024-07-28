// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.AsyncPump
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class AsyncPump
  {
    public static T Run<T>(Func<Task<T>> func)
    {
      if (func == null)
        throw new ArgumentNullException(nameof (func));
      SynchronizationContext current = SynchronizationContext.Current;
      try
      {
        AsyncPump.SingleThreadSynchronizationContext syncCtx = new AsyncPump.SingleThreadSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext((SynchronizationContext) syncCtx);
        Task<T> task = func();
        if (task == null)
          throw new InvalidOperationException("No task provided.");
        task.ContinueWith((Action<Task<T>>) (_param1 => syncCtx.Complete()), TaskScheduler.Default);
        syncCtx.RunOnCurrentThread();
        return task.GetAwaiter().GetResult();
      }
      finally
      {
        SynchronizationContext.SetSynchronizationContext(current);
      }
    }

    public static void Run(Func<Task> func) => AsyncPump.Run<int>((Func<Task<int>>) (async () =>
    {
      await func().ConfigureAwait(false);
      return 0;
    }));

    private sealed class SingleThreadSynchronizationContext : SynchronizationContext
    {
      private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> queue = new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();
      private readonly Thread thread = Thread.CurrentThread;

      public override void Post(SendOrPostCallback d, object state)
      {
        if (d == null)
          throw new ArgumentNullException(nameof (d));
        this.queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
      }

      public override void Send(SendOrPostCallback d, object state) => throw new NotSupportedException("Synchronously sending is not supported.");

      public void RunOnCurrentThread()
      {
        foreach (KeyValuePair<SendOrPostCallback, object> consuming in this.queue.GetConsumingEnumerable())
          consuming.Key(consuming.Value);
      }

      public void Complete() => this.queue.CompleteAdding();
    }
  }
}
