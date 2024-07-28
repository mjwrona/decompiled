// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ForkedConcurrentIterator`1
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class ForkedConcurrentIterator<T> : IDisposable
  {
    private readonly IConcurrentIterator<T> source;
    private readonly BufferBlock<T>[] buffers;
    private readonly Task sourceDrainerTask;
    public readonly IConcurrentIterator<T>[] ForksForParallelConsupmtion;

    public ForkedConcurrentIterator(
      IConcurrentIterator<T> source,
      int forkCount,
      int boundedCapacity,
      CancellationToken token)
    {
      ForkedConcurrentIterator<T> concurrentIterator = this;
      this.source = source;
      this.buffers = Enumerable.Range(0, forkCount).Select<int, BufferBlock<T>>((Func<int, BufferBlock<T>>) (_ => new BufferBlock<T>(new DataflowBlockOptions()
      {
        BoundedCapacity = boundedCapacity,
        CancellationToken = token
      }))).ToArray<BufferBlock<T>>();
      Func<T, Task> func;
      this.sourceDrainerTask = Task.Run((Func<Task>) (async () =>
      {
        try
        {
          await source.ForEachAsyncNoContext<T>(token, func ?? (func = (Func<T, Task>) (async t =>
          {
            BufferBlock<T>[] bufferBlockArray = concurrentIterator.buffers;
            for (int index = 0; index < bufferBlockArray.Length; ++index)
            {
              BufferBlock<T> bufferBlock = bufferBlockArray[index];
              await bufferBlock.SendOrThrowAsync<T, T>((ITargetBlock<T>) bufferBlock, t, token).ConfigureAwait(false);
            }
            bufferBlockArray = (BufferBlock<T>[]) null;
          }))).ConfigureAwait(false);
        }
        finally
        {
          foreach (BufferBlock<T> buffer in concurrentIterator.buffers)
            buffer.Complete();
        }
      }), token);
      this.ForksForParallelConsupmtion = ((IEnumerable<BufferBlock<T>>) this.buffers).Select<BufferBlock<T>, IConcurrentIterator<T>>((Func<BufferBlock<T>, IConcurrentIterator<T>>) (buffer => ForkedConcurrentIterator<T>.CreateFromBuffer(boundedCapacity, buffer, concurrentIterator.sourceDrainerTask, token))).ToArray<IConcurrentIterator<T>>();
    }

    private static IConcurrentIterator<T> CreateFromBuffer(
      int boundedCapacity,
      BufferBlock<T> bufferBlock,
      Task checkForErrorTask,
      CancellationToken cancelToken)
    {
      return (IConcurrentIterator<T>) new ConcurrentIterator<T>(new int?(boundedCapacity), cancelToken, (Func<TryAddValueAsyncFunc<T>, CancellationToken, Task>) (async (valueAdderAsync, cancellationToken) =>
      {
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable;
        do
        {
          configuredTaskAwaitable = bufferBlock.OutputAvailableAsync<T>(cancellationToken).ConfigureAwait(false);
          if (await configuredTaskAwaitable)
            configuredTaskAwaitable = valueAdderAsync(await bufferBlock.ReceiveAsync<T>(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
          else
            break;
        }
        while (await configuredTaskAwaitable);
        await checkForErrorTask.ConfigureAwait(false);
      }));
    }

    public void Dispose()
    {
      foreach (IDisposable disposable in this.ForksForParallelConsupmtion)
        disposable.Dispose();
      this.source.Dispose();
      this.sourceDrainerTask.Wait();
    }
  }
}
