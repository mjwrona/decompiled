// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIterator`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ConcurrentIterator<TProducerSource, TValue> : IConcurrentIterator<TValue>, IDisposable
  {
    private readonly CancellationTokenSource disposeCancellationSource;
    private readonly CancellationTokenSource producerCancellationSource;
    private readonly BufferBlock<TValue> bufferBlock;
    private readonly Task checkForErrorsTask;
    private readonly ActionBlock<TProducerSource> producers;
    private bool checkForErrorsTaskObserved;
    private bool disposed;
    private TValue current;

    public ConcurrentIterator(
      IEnumerable<TProducerSource> producerSources,
      int? boundedCapacity,
      CancellationToken cancellationToken,
      Func<TProducerSource, TryAddValueAsyncFunc<TValue>, CancellationToken, Task> producerTask)
      : this(producerSources, new int?(), boundedCapacity, cancellationToken, producerTask)
    {
    }

    public ConcurrentIterator(
      IEnumerable<TProducerSource> producerSources,
      int? maxConcurrentProducers,
      int? boundedCapacity,
      CancellationToken cancellationToken,
      Func<TProducerSource, TryAddValueAsyncFunc<TValue>, CancellationToken, Task> producerTask)
    {
      ConcurrentIterator<TProducerSource, TValue> concurrentIterator = this;
      this.disposeCancellationSource = new CancellationTokenSource();
      this.producerCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.disposeCancellationSource.Token);
      BufferBlock<TValue> bufferBlock;
      if (!boundedCapacity.HasValue)
        bufferBlock = new BufferBlock<TValue>();
      else
        bufferBlock = new BufferBlock<TValue>(new DataflowBlockOptions()
        {
          BoundedCapacity = boundedCapacity.Value
        });
      this.bufferBlock = bufferBlock;
      Func<TProducerSource, Task> action = (Func<TProducerSource, Task>) (source => producerTask(source, new TryAddValueAsyncFunc<TValue>(concurrentIterator.TryAddValue), concurrentIterator.producerCancellationSource.Token));
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.CancellationToken = this.producerCancellationSource.Token;
      dataflowBlockOptions.MaxDegreeOfParallelism = maxConcurrentProducers ?? Environment.ProcessorCount;
      this.producers = NonSwallowingActionBlock.Create<TProducerSource>(action, dataflowBlockOptions);
      this.producers.PostAllToUnboundedAndCompleteAsync<TProducerSource>(producerSources, this.producerCancellationSource.Token);
      this.checkForErrorsTask = this.WaitForProducersAsync(this.producers.Completion);
    }

    public bool EnumerationStarted { get; private set; }

    public TValue Current
    {
      get
      {
        if (this.disposed)
          throw new ObjectDisposedException("Cannot get current value: ConcurrentIterator is disposed.");
        return this.current;
      }
    }

    public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
      if (this.disposed)
        throw new ObjectDisposedException("Cannot get current value: ConcurrentIterator is disposed.");
      this.EnumerationStarted = true;
      bool more = await this.bufferBlock.OutputAvailableAsync<TValue>(cancellationToken).ConfigureAwait(false);
      if (more)
      {
        this.current = await this.bufferBlock.ReceiveAsync<TValue>(cancellationToken).ConfigureAwait(false);
      }
      else
      {
        try
        {
          await this.checkForErrorsTask.ConfigureAwait(false);
        }
        finally
        {
          this.checkForErrorsTaskObserved = true;
        }
      }
      return more;
    }

    public int CurrentBufferCount => this.bufferBlock.Count;

    protected virtual void BeginWaitForCheckForErrorsTask()
    {
    }

    private async Task WaitForProducersAsync(Task producersTask)
    {
      try
      {
        await producersTask.ConfigureAwait(false);
      }
      finally
      {
        this.bufferBlock.Complete();
      }
    }

    private async Task<bool> TryAddValue(TValue valueToAdd)
    {
      if (this.producerCancellationSource.IsCancellationRequested)
        return false;
      await this.bufferBlock.SendOrThrowAsync<TValue, TValue>((ITargetBlock<TValue>) this.bufferBlock, valueToAdd, this.producerCancellationSource.Token).ConfigureAwait(false);
      return true;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.disposed)
        return;
      if (!this.checkForErrorsTaskObserved)
      {
        bool flag;
        if (this.producerCancellationSource.IsCancellationRequested)
        {
          flag = false;
        }
        else
        {
          this.disposeCancellationSource.Cancel(false);
          flag = true;
        }
        this.BeginWaitForCheckForErrorsTask();
        try
        {
          this.checkForErrorsTask.GetAwaiter().GetResult();
        }
        catch (OperationCanceledException ex) when (flag)
        {
        }
        catch (TimeoutException ex) when (flag && ex.InnerException is OperationCanceledException)
        {
        }
        finally
        {
          this.checkForErrorsTaskObserved = true;
          this.disposeCancellationSource.Dispose();
          this.producerCancellationSource.Dispose();
        }
      }
      else
      {
        this.disposeCancellationSource.Dispose();
        this.producerCancellationSource.Dispose();
      }
      this.disposed = true;
    }

    public void Dispose() => this.Dispose(true);
  }
}
