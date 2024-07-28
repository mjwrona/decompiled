// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.VssRequestContextFork
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public static class VssRequestContextFork
  {
    public static Task ForkChildrenAsync<TValue, TTaskService>(
      this IVssRequestContext requestContext,
      int maxParallelism,
      IEnumerable<TValue> inputs,
      Func<IVssRequestContext, TValue, Task> action,
      Func<bool> verifyBeforeWorkOnItem = null,
      Func<bool> verifyEarlyTermination = null)
      where TTaskService : class, IVssTaskService
    {
      return requestContext.ForkChildrenAsync<TValue, TTaskService>(maxParallelism, (IConcurrentIterator<TValue>) new ConcurrentIterator<TValue>(inputs), action, verifyBeforeWorkOnItem, verifyEarlyTermination);
    }

    public static async Task ForkChildrenAsync<TValue, TTaskService>(
      this IVssRequestContext requestContext,
      int maxParallelism,
      IConcurrentIterator<TValue> inputs,
      Func<IVssRequestContext, TValue, Task> action,
      Func<bool> verifyBeforeWorkOnItem = null,
      Func<bool> verifyEarlyTermination = null)
      where TTaskService : class, IVssTaskService
    {
      maxParallelism = maxParallelism <= 0 ? 1 : maxParallelism;
      int maxWorkersToStart = maxParallelism - 1;
      int itemsQueued = 0;
      int itemsDequeued = 0;
      using (AsyncQueue<TValue> itemQueue = new AsyncQueue<TValue>())
      {
        List<Task> forkedRequestContextWork = new List<Task>();
        Func<IVssRequestContext, Task> dequeueAndRunUntilEmpty = (Func<IVssRequestContext, Task>) (async childRequestContext =>
        {
          IConcurrentIterator<TValue> reader = itemQueue.GetAsyncReader();
          try
          {
            while (true)
            {
              if (await reader.MoveNextAsync(childRequestContext.CancellationToken).ConfigureAwait(true))
              {
                Interlocked.Increment(ref itemsDequeued);
                if (verifyBeforeWorkOnItem == null || verifyBeforeWorkOnItem())
                  await action(childRequestContext, reader.Current).ConfigureAwait(true);
                else
                  break;
              }
              else
                goto label_10;
            }
            return;
          }
          finally
          {
            reader?.Dispose();
          }
label_10:
          reader = (IConcurrentIterator<TValue>) null;
        });
        IConcurrentIterator<TValue> concurrentIterator = inputs;
        try
        {
          int inputsQueued = 0;
          while (true)
          {
            do
            {
              if (await inputs.MoveNextAsync(requestContext.CancellationToken).ConfigureAwait(true))
              {
                ++itemsQueued;
                itemQueue.Enqueue(inputs.Current);
                ++inputsQueued;
              }
              else
                goto label_10;
            }
            while (!(inputsQueued > forkedRequestContextWork.Count + 1 & forkedRequestContextWork.Count < maxWorkersToStart));
            forkedRequestContextWork.Add(requestContext.Fork<TTaskService>(dequeueAndRunUntilEmpty, nameof (ForkChildrenAsync)));
          }
        }
        finally
        {
          concurrentIterator?.Dispose();
        }
label_10:
        concurrentIterator = (IConcurrentIterator<TValue>) null;
        itemQueue.CompleteAdding();
        Task selfTask = dequeueAndRunUntilEmpty(requestContext);
        try
        {
          await selfTask.ConfigureAwait(true);
        }
        finally
        {
          await Task.WhenAll(selfTask, Task.WhenAll((IEnumerable<Task>) forkedRequestContextWork)).ConfigureAwait(true);
        }
        if ((!itemQueue.IsCompleted || !itemQueue.IsEmpty || itemsQueued != itemsDequeued) && (verifyEarlyTermination == null || !verifyEarlyTermination()))
          throw new InvalidOperationException("Queue is not fully processed.");
        forkedRequestContextWork = (List<Task>) null;
        dequeueAndRunUntilEmpty = (Func<IVssRequestContext, Task>) null;
        selfTask = (Task) null;
      }
    }
  }
}
