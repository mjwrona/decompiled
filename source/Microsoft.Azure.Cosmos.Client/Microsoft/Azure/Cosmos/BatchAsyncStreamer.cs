// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BatchAsyncStreamer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class BatchAsyncStreamer : IDisposable
  {
    private static readonly TimeSpan congestionControllerDelay = TimeSpan.FromMilliseconds(1000.0);
    private static readonly TimeSpan batchTimeout = TimeSpan.FromMilliseconds(100.0);
    private readonly object dispatchLimiter = new object();
    private readonly int maxBatchOperationCount;
    private readonly int maxBatchByteSize;
    private readonly BatchAsyncBatcherExecuteDelegate executor;
    private readonly BatchAsyncBatcherRetryDelegate retrier;
    private readonly CosmosSerializerCore serializerCore;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly int congestionIncreaseFactor = 1;
    private readonly int congestionDecreaseFactor = 5;
    private readonly int maxDegreeOfConcurrency;
    private readonly TimerWheel timerWheel;
    private readonly SemaphoreSlim limiter;
    private readonly BatchPartitionMetric oldPartitionMetric;
    private readonly BatchPartitionMetric partitionMetric;
    private readonly CosmosClientContext clientContext;
    private volatile BatchAsyncBatcher currentBatcher;
    private TimerWheelTimer currentTimer;
    private Task timerTask;
    private TimerWheelTimer congestionControlTimer;
    private Task congestionControlTask;
    private int congestionDegreeOfConcurrency = 1;
    private long congestionWaitTimeInMilliseconds = 1000;

    public BatchAsyncStreamer(
      int maxBatchOperationCount,
      int maxBatchByteSize,
      TimerWheel timerWheel,
      SemaphoreSlim limiter,
      int maxDegreeOfConcurrency,
      CosmosSerializerCore serializerCore,
      BatchAsyncBatcherExecuteDelegate executor,
      BatchAsyncBatcherRetryDelegate retrier,
      CosmosClientContext clientContext)
    {
      if (maxBatchOperationCount < 1)
        throw new ArgumentOutOfRangeException(nameof (maxBatchOperationCount));
      if (maxBatchByteSize < 1)
        throw new ArgumentOutOfRangeException(nameof (maxBatchByteSize));
      if (executor == null)
        throw new ArgumentNullException(nameof (executor));
      if (retrier == null)
        throw new ArgumentNullException(nameof (retrier));
      if (serializerCore == null)
        throw new ArgumentNullException(nameof (serializerCore));
      if (limiter == null)
        throw new ArgumentNullException(nameof (limiter));
      if (maxDegreeOfConcurrency < 1)
        throw new ArgumentNullException(nameof (maxDegreeOfConcurrency));
      this.maxBatchOperationCount = maxBatchOperationCount;
      this.maxBatchByteSize = maxBatchByteSize;
      this.executor = executor;
      this.retrier = retrier;
      this.timerWheel = timerWheel;
      this.serializerCore = serializerCore;
      this.clientContext = clientContext;
      this.currentBatcher = this.CreateBatchAsyncBatcher();
      this.ResetTimer();
      this.limiter = limiter;
      this.oldPartitionMetric = new BatchPartitionMetric();
      this.partitionMetric = new BatchPartitionMetric();
      this.maxDegreeOfConcurrency = maxDegreeOfConcurrency;
      this.StartCongestionControlTimer();
    }

    public void Add(ItemBatchOperation operation)
    {
      BatchAsyncBatcher batchAsyncBatcher = (BatchAsyncBatcher) null;
      lock (this.dispatchLimiter)
      {
        while (!this.currentBatcher.TryAdd(operation))
          batchAsyncBatcher = this.GetBatchToDispatchAndCreate();
      }
      batchAsyncBatcher?.DispatchAsync(this.partitionMetric, this.cancellationTokenSource.Token);
    }

    public void Dispose()
    {
      this.cancellationTokenSource.Cancel();
      this.cancellationTokenSource.Dispose();
      this.currentTimer.CancelTimer();
      this.currentTimer = (TimerWheelTimer) null;
      this.timerTask = (Task) null;
      if (this.congestionControlTimer == null)
        return;
      this.congestionControlTimer.CancelTimer();
      this.congestionControlTimer = (TimerWheelTimer) null;
      this.congestionControlTask = (Task) null;
    }

    private void ResetTimer()
    {
      this.currentTimer = this.timerWheel.CreateTimer(BatchAsyncStreamer.batchTimeout);
      this.timerTask = this.GetTimerTaskAsync();
    }

    private async Task GetTimerTaskAsync()
    {
      await this.currentTimer.StartTimerAsync();
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      this.DispatchTimer();
    }

    private void StartCongestionControlTimer()
    {
      this.congestionControlTimer = this.timerWheel.CreateTimer(BatchAsyncStreamer.congestionControllerDelay);
      this.congestionControlTask = (Task) this.congestionControlTimer.StartTimerAsync().ContinueWith<Task>((Func<Task, Task>) (async task => await this.RunCongestionControlAsync()), this.cancellationTokenSource.Token);
    }

    private void DispatchTimer()
    {
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      BatchAsyncBatcher dispatchAndCreate;
      lock (this.dispatchLimiter)
        dispatchAndCreate = this.GetBatchToDispatchAndCreate();
      dispatchAndCreate?.DispatchAsync(this.partitionMetric, this.cancellationTokenSource.Token);
      this.ResetTimer();
    }

    private BatchAsyncBatcher GetBatchToDispatchAndCreate()
    {
      if (this.currentBatcher.IsEmpty)
        return (BatchAsyncBatcher) null;
      BatchAsyncBatcher currentBatcher = this.currentBatcher;
      this.currentBatcher = this.CreateBatchAsyncBatcher();
      return currentBatcher;
    }

    private BatchAsyncBatcher CreateBatchAsyncBatcher() => new BatchAsyncBatcher(this.maxBatchOperationCount, this.maxBatchByteSize, this.serializerCore, this.executor, this.retrier, this.clientContext);

    private async Task RunCongestionControlAsync()
    {
      while (!this.cancellationTokenSource.Token.IsCancellationRequested)
      {
        long timeTakenInMilliseconds = this.partitionMetric.TimeTakenInMilliseconds - this.oldPartitionMetric.TimeTakenInMilliseconds;
        if (timeTakenInMilliseconds >= this.congestionWaitTimeInMilliseconds)
        {
          long diffThrottle = this.partitionMetric.NumberOfThrottles - this.oldPartitionMetric.NumberOfThrottles;
          long changeItemsCount = this.partitionMetric.NumberOfItemsOperatedOn - this.oldPartitionMetric.NumberOfItemsOperatedOn;
          this.oldPartitionMetric.Add(changeItemsCount, timeTakenInMilliseconds, diffThrottle);
          if (diffThrottle > 0L)
          {
            int decreaseCount = Math.Min(this.congestionDecreaseFactor, this.congestionDegreeOfConcurrency / 2);
            for (int i = 0; i < decreaseCount; ++i)
              await this.limiter.WaitAsync(this.cancellationTokenSource.Token);
            this.congestionDegreeOfConcurrency -= decreaseCount;
            this.congestionWaitTimeInMilliseconds += 1000L;
          }
          if (changeItemsCount > 0L && diffThrottle == 0L && this.congestionDegreeOfConcurrency + this.congestionIncreaseFactor <= this.maxDegreeOfConcurrency)
          {
            this.limiter.Release(this.congestionIncreaseFactor);
            this.congestionDegreeOfConcurrency += this.congestionIncreaseFactor;
          }
        }
        else
          break;
      }
      this.StartCongestionControlTimer();
    }
  }
}
