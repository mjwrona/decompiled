// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ComparableTaskScheduler
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class ComparableTaskScheduler : IDisposable
  {
    private const int MinimumBatchSize = 1;
    private readonly AsyncCollection<IComparableTask> taskQueue;
    private readonly ConcurrentDictionary<IComparableTask, Task> delayedTasks;
    private readonly CancellationTokenSource tokenSource;
    private readonly SemaphoreSlim canRunTaskSemaphoreSlim;
    private readonly Task schedulerTask;
    private int maximumConcurrencyLevel;
    private volatile bool isStopped;

    public ComparableTaskScheduler()
      : this(Environment.ProcessorCount)
    {
    }

    public ComparableTaskScheduler(int maximumConcurrencyLevel)
      : this(Enumerable.Empty<IComparableTask>(), maximumConcurrencyLevel)
    {
    }

    public ComparableTaskScheduler(IEnumerable<IComparableTask> tasks, int maximumConcurrencyLevel)
    {
      this.taskQueue = new AsyncCollection<IComparableTask>((IProducerConsumerCollection<IComparableTask>) new PriorityQueue<IComparableTask>(tasks, true));
      this.delayedTasks = new ConcurrentDictionary<IComparableTask, Task>();
      this.maximumConcurrencyLevel = maximumConcurrencyLevel;
      this.tokenSource = new CancellationTokenSource();
      this.canRunTaskSemaphoreSlim = new SemaphoreSlim(maximumConcurrencyLevel);
      this.schedulerTask = this.ScheduleAsync();
    }

    public int MaximumConcurrencyLevel => this.maximumConcurrencyLevel;

    public int CurrentRunningTaskCount => this.maximumConcurrencyLevel - Math.Max(0, this.canRunTaskSemaphoreSlim.CurrentCount);

    public bool IsStopped => this.isStopped;

    private CancellationToken CancellationToken => this.tokenSource.Token;

    public void IncreaseMaximumConcurrencyLevel(int delta)
    {
      if (delta <= 0)
        throw new ArgumentOutOfRangeException("delta must be a positive number.");
      this.canRunTaskSemaphoreSlim.Release(delta);
      this.maximumConcurrencyLevel += delta;
    }

    public void Dispose() => this.Stop();

    public void Stop()
    {
      this.isStopped = true;
      this.tokenSource.Cancel();
      this.delayedTasks.Clear();
    }

    public bool TryQueueTask(IComparableTask comparableTask, TimeSpan delay = default (TimeSpan))
    {
      if (comparableTask == null)
        throw new ArgumentNullException("task");
      if (this.isStopped)
        return false;
      Task task = (Task) new Task<Task>((Func<Task>) (() => this.QueueDelayedTaskAsync(comparableTask, delay)), this.CancellationToken);
      if (!this.delayedTasks.TryAdd(comparableTask, task))
        return false;
      task.Start();
      return true;
    }

    private async Task QueueDelayedTaskAsync(IComparableTask comparableTask, TimeSpan delay)
    {
      Task task;
      if (!this.delayedTasks.TryRemove(comparableTask, out task) || task.IsCanceled)
        return;
      if (delay > new TimeSpan())
        await Task.Delay(delay, this.CancellationToken);
      IComparableTask other;
      if (this.taskQueue.TryPeek(out other) && comparableTask.CompareTo(other) <= 0)
        await this.ExecuteComparableTaskAsync(comparableTask);
      else
        await this.taskQueue.AddAsync(comparableTask, this.CancellationToken);
    }

    private async Task ScheduleAsync()
    {
      while (!this.isStopped)
        await this.ExecuteComparableTaskAsync(await this.taskQueue.TakeAsync(this.CancellationToken));
    }

    private async Task ExecuteComparableTaskAsync(IComparableTask comparableTask)
    {
      await this.canRunTaskSemaphoreSlim.WaitAsync(this.CancellationToken);
      Task.Factory.StartNewOnCurrentTaskSchedulerAsync<Task>((Func<Task>) (() => comparableTask.StartAsync(this.CancellationToken).ContinueWith((Action<Task>) (antecendent => this.canRunTaskSemaphoreSlim.Release()), TaskScheduler.Current)), this.CancellationToken);
    }
  }
}
