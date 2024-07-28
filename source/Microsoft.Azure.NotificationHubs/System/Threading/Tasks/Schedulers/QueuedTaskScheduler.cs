// Decompiled with JetBrains decompiler
// Type: System.Threading.Tasks.Schedulers.QueuedTaskScheduler
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Threading.Tasks.Schedulers
{
  [DebuggerTypeProxy(typeof (QueuedTaskScheduler.QueuedTaskSchedulerDebugView))]
  [DebuggerDisplay("Id={Id}, Queues={DebugQueueCount}, ScheduledTasks = {DebugTaskCount}")]
  internal sealed class QueuedTaskScheduler : TaskScheduler, IDisposable
  {
    private readonly SortedList<int, QueuedTaskScheduler.QueueGroup> _queueGroups = new SortedList<int, QueuedTaskScheduler.QueueGroup>();
    private readonly CancellationTokenSource _disposeCancellation = new CancellationTokenSource();
    private readonly int _concurrencyLevel;
    private static ThreadLocal<bool> _taskProcessingThread = new ThreadLocal<bool>();
    private readonly TaskScheduler _targetScheduler;
    private readonly Queue<Task> _nonthreadsafeTaskQueue;
    private int _delegatesQueuedOrRunning;
    private readonly Thread[] _threads;
    private readonly BlockingCollection<Task> _blockingTaskQueue;

    public QueuedTaskScheduler()
      : this(TaskScheduler.Default, 0)
    {
    }

    public QueuedTaskScheduler(TaskScheduler targetScheduler)
      : this(targetScheduler, 0)
    {
    }

    public QueuedTaskScheduler(TaskScheduler targetScheduler, int maxConcurrencyLevel)
    {
      if (targetScheduler == null)
        throw new ArgumentNullException("underlyingScheduler");
      if (maxConcurrencyLevel < 0)
        throw new ArgumentOutOfRangeException("concurrencyLevel");
      this._targetScheduler = targetScheduler;
      this._nonthreadsafeTaskQueue = new Queue<Task>();
      this._concurrencyLevel = maxConcurrencyLevel != 0 ? maxConcurrencyLevel : Environment.ProcessorCount;
      if (targetScheduler.MaximumConcurrencyLevel <= 0 || targetScheduler.MaximumConcurrencyLevel >= this._concurrencyLevel)
        return;
      this._concurrencyLevel = targetScheduler.MaximumConcurrencyLevel;
    }

    public QueuedTaskScheduler(int threadCount)
      : this(threadCount, string.Empty)
    {
    }

    public QueuedTaskScheduler(
      int threadCount,
      string threadName = "",
      bool useForegroundThreads = false,
      ThreadPriority threadPriority = ThreadPriority.Normal,
      ApartmentState threadApartmentState = ApartmentState.MTA,
      int threadMaxStackSize = 0,
      Action threadInit = null,
      Action threadFinally = null)
    {
      QueuedTaskScheduler queuedTaskScheduler = this;
      if (threadCount < 0)
        throw new ArgumentOutOfRangeException("concurrencyLevel");
      this._concurrencyLevel = threadCount != 0 ? threadCount : Environment.ProcessorCount;
      this._blockingTaskQueue = new BlockingCollection<Task>();
      this._threads = new Thread[threadCount];
      for (int index = 0; index < threadCount; ++index)
      {
        this._threads[index] = new Thread((ThreadStart) (() => queuedTaskScheduler.ThreadBasedDispatchLoop(threadInit, threadFinally)), threadMaxStackSize)
        {
          Priority = threadPriority,
          IsBackground = !useForegroundThreads
        };
        if (threadName != null)
          this._threads[index].Name = threadName + " (" + (object) index + ")";
        this._threads[index].SetApartmentState(threadApartmentState);
      }
      foreach (Thread thread in this._threads)
        thread.Start();
    }

    private void ThreadBasedDispatchLoop(Action threadInit, Action threadFinally)
    {
      QueuedTaskScheduler._taskProcessingThread.Value = true;
      if (threadInit != null)
        threadInit();
      try
      {
        while (true)
        {
          try
          {
            foreach (Task consuming in this._blockingTaskQueue.GetConsumingEnumerable(this._disposeCancellation.Token))
            {
              if (consuming != null)
              {
                this.TryExecuteTask(consuming);
              }
              else
              {
                Task targetTask;
                QueuedTaskScheduler.QueuedTaskSchedulerQueue queueForTargetTask;
                lock (this._queueGroups)
                  this.FindNextTask_NeedsLock(out targetTask, out queueForTargetTask);
                if (targetTask != null)
                  queueForTargetTask.ExecuteTask(targetTask);
              }
            }
          }
          catch (ThreadAbortException ex)
          {
            if (!Environment.HasShutdownStarted)
            {
              if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
                Thread.ResetAbort();
            }
          }
        }
      }
      catch (OperationCanceledException ex)
      {
      }
      finally
      {
        if (threadFinally != null)
          threadFinally();
        QueuedTaskScheduler._taskProcessingThread.Value = false;
      }
    }

    private void FindNextTask_NeedsLock(
      out Task targetTask,
      out QueuedTaskScheduler.QueuedTaskSchedulerQueue queueForTargetTask)
    {
      targetTask = (Task) null;
      queueForTargetTask = (QueuedTaskScheduler.QueuedTaskSchedulerQueue) null;
      foreach (KeyValuePair<int, QueuedTaskScheduler.QueueGroup> queueGroup1 in this._queueGroups)
      {
        QueuedTaskScheduler.QueueGroup queueGroup2 = queueGroup1.Value;
        foreach (int index in queueGroup2.CreateSearchOrder())
        {
          queueForTargetTask = queueGroup2[index];
          Queue<Task> workItems = queueForTargetTask._workItems;
          if (workItems.Count > 0)
          {
            targetTask = workItems.Dequeue();
            if (queueForTargetTask._disposed && workItems.Count == 0)
              this.RemoveQueue_NeedsLock(queueForTargetTask);
            QueuedTaskScheduler.QueueGroup queueGroup3 = queueGroup2;
            queueGroup3.NextQueueIndex = (queueGroup3.NextQueueIndex + 1) % queueGroup1.Value.Count;
            return;
          }
        }
      }
    }

    protected override void QueueTask(Task task)
    {
      if (this._disposeCancellation.IsCancellationRequested)
        throw new ObjectDisposedException(this.GetType().Name);
      if (this._targetScheduler == null)
      {
        this._blockingTaskQueue.Add(task);
      }
      else
      {
        bool flag = false;
        lock (this._nonthreadsafeTaskQueue)
        {
          this._nonthreadsafeTaskQueue.Enqueue(task);
          if (this._delegatesQueuedOrRunning < this._concurrencyLevel)
          {
            ++this._delegatesQueuedOrRunning;
            flag = true;
          }
        }
        if (!flag)
          return;
        Task.Factory.StartNew(new Action(this.ProcessPrioritizedAndBatchedTasks), CancellationToken.None, TaskCreationOptions.None, this._targetScheduler);
      }
    }

    private void ProcessPrioritizedAndBatchedTasks()
    {
      bool flag = true;
      while (!this._disposeCancellation.IsCancellationRequested & flag)
      {
        try
        {
          QueuedTaskScheduler._taskProcessingThread.Value = true;
          while (!this._disposeCancellation.IsCancellationRequested)
          {
            Task targetTask;
            lock (this._nonthreadsafeTaskQueue)
            {
              if (this._nonthreadsafeTaskQueue.Count != 0)
                targetTask = this._nonthreadsafeTaskQueue.Dequeue();
              else
                break;
            }
            QueuedTaskScheduler.QueuedTaskSchedulerQueue queueForTargetTask = (QueuedTaskScheduler.QueuedTaskSchedulerQueue) null;
            if (targetTask == null)
            {
              lock (this._queueGroups)
                this.FindNextTask_NeedsLock(out targetTask, out queueForTargetTask);
            }
            if (targetTask != null)
            {
              if (queueForTargetTask != null)
                queueForTargetTask.ExecuteTask(targetTask);
              else
                this.TryExecuteTask(targetTask);
            }
          }
        }
        finally
        {
          lock (this._nonthreadsafeTaskQueue)
          {
            if (this._nonthreadsafeTaskQueue.Count == 0)
            {
              --this._delegatesQueuedOrRunning;
              flag = false;
              QueuedTaskScheduler._taskProcessingThread.Value = false;
            }
          }
        }
      }
    }

    private void NotifyNewWorkItem() => this.QueueTask((Task) null);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => QueuedTaskScheduler._taskProcessingThread.Value && this.TryExecuteTask(task);

    protected override IEnumerable<Task> GetScheduledTasks() => this._targetScheduler == null ? (IEnumerable<Task>) this._blockingTaskQueue.Where<Task>((Func<Task, bool>) (t => t != null)).ToList<Task>() : (IEnumerable<Task>) this._nonthreadsafeTaskQueue.Where<Task>((Func<Task, bool>) (t => t != null)).ToList<Task>();

    public override int MaximumConcurrencyLevel => this._concurrencyLevel;

    public void Dispose() => this._disposeCancellation.Cancel();

    public TaskScheduler ActivateNewQueue() => this.ActivateNewQueue(0);

    public TaskScheduler ActivateNewQueue(int priority)
    {
      QueuedTaskScheduler.QueuedTaskSchedulerQueue taskSchedulerQueue = new QueuedTaskScheduler.QueuedTaskSchedulerQueue(priority, this);
      lock (this._queueGroups)
      {
        QueuedTaskScheduler.QueueGroup queueGroup;
        if (!this._queueGroups.TryGetValue(priority, out queueGroup))
        {
          queueGroup = new QueuedTaskScheduler.QueueGroup();
          this._queueGroups.Add(priority, queueGroup);
        }
        queueGroup.Add(taskSchedulerQueue);
      }
      return (TaskScheduler) taskSchedulerQueue;
    }

    private void RemoveQueue_NeedsLock(QueuedTaskScheduler.QueuedTaskSchedulerQueue queue)
    {
      QueuedTaskScheduler.QueueGroup queueGroup = this._queueGroups[queue._priority];
      int index = queueGroup.IndexOf(queue);
      if (queueGroup.NextQueueIndex >= index)
        --queueGroup.NextQueueIndex;
      queueGroup.RemoveAt(index);
    }

    private class QueuedTaskSchedulerDebugView
    {
      private QueuedTaskScheduler _scheduler;

      public QueuedTaskSchedulerDebugView(QueuedTaskScheduler scheduler) => this._scheduler = scheduler != null ? scheduler : throw new ArgumentNullException(nameof (scheduler));

      public IEnumerable<Task> ScheduledTasks => (IEnumerable<Task>) (this._scheduler._targetScheduler != null ? (IEnumerable<Task>) this._scheduler._nonthreadsafeTaskQueue : (IEnumerable<Task>) this._scheduler._blockingTaskQueue).Where<Task>((Func<Task, bool>) (t => t != null)).ToList<Task>();

      public IEnumerable<TaskScheduler> Queues
      {
        get
        {
          List<TaskScheduler> queues = new List<TaskScheduler>();
          foreach (KeyValuePair<int, QueuedTaskScheduler.QueueGroup> queueGroup in this._scheduler._queueGroups)
            queues.AddRange((IEnumerable<TaskScheduler>) queueGroup.Value);
          return (IEnumerable<TaskScheduler>) queues;
        }
      }
    }

    private class QueueGroup : List<QueuedTaskScheduler.QueuedTaskSchedulerQueue>
    {
      public int NextQueueIndex;

      public IEnumerable<int> CreateSearchOrder()
      {
        for (int i = this.NextQueueIndex; i < this.Count; ++i)
          yield return i;
        for (int i = 0; i < this.NextQueueIndex; ++i)
          yield return i;
      }
    }

    [DebuggerDisplay("QueuePriority = {_priority}, WaitingTasks = {WaitingTasks}")]
    [DebuggerTypeProxy(typeof (QueuedTaskScheduler.QueuedTaskSchedulerQueue.QueuedTaskSchedulerQueueDebugView))]
    private sealed class QueuedTaskSchedulerQueue : TaskScheduler, IDisposable
    {
      private readonly QueuedTaskScheduler _pool;
      internal readonly Queue<Task> _workItems;
      internal bool _disposed;
      internal int _priority;

      internal QueuedTaskSchedulerQueue(int priority, QueuedTaskScheduler pool)
      {
        this._priority = priority;
        this._pool = pool;
        this._workItems = new Queue<Task>();
      }

      protected override IEnumerable<Task> GetScheduledTasks() => (IEnumerable<Task>) this._workItems.ToList<Task>();

      protected override void QueueTask(Task task)
      {
        if (this._disposed)
          throw new ObjectDisposedException(this.GetType().Name);
        lock (this._pool._queueGroups)
          this._workItems.Enqueue(task);
        this._pool.NotifyNewWorkItem();
      }

      protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => QueuedTaskScheduler._taskProcessingThread.Value && this.TryExecuteTask(task);

      internal void ExecuteTask(Task task) => this.TryExecuteTask(task);

      public override int MaximumConcurrencyLevel => this._pool.MaximumConcurrencyLevel;

      public void Dispose()
      {
        if (this._disposed)
          return;
        lock (this._pool._queueGroups)
        {
          if (this._workItems.Count == 0)
            this._pool.RemoveQueue_NeedsLock(this);
        }
        this._disposed = true;
      }

      private sealed class QueuedTaskSchedulerQueueDebugView
      {
        private readonly QueuedTaskScheduler.QueuedTaskSchedulerQueue _queue;

        public QueuedTaskSchedulerQueueDebugView(QueuedTaskScheduler.QueuedTaskSchedulerQueue queue) => this._queue = queue != null ? queue : throw new ArgumentNullException(nameof (queue));

        public int Priority => this._queue._priority;

        public int Id => this._queue.Id;

        public IEnumerable<Task> ScheduledTasks => this._queue.GetScheduledTasks();

        public QueuedTaskScheduler AssociatedScheduler => this._queue._pool;
      }
    }
  }
}
