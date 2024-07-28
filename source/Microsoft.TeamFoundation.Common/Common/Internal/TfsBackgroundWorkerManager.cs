// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.TfsBackgroundWorkerManager
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfsBackgroundWorkerManager : IDisposable
  {
    private static TfsBackgroundWorkerManager s_instance;
    private const int cDefaultMaxWorkers = 3;
    private int m_maxWorkers;
    private bool isShuttingDown;
    private Dictionary<object, TfsBackgroundWorkerManager.BackgroundTask> m_allTasks;
    private Dictionary<Guid, int> m_groupMaxWorkers;
    private List<TfsBackgroundWorkerManager.BackgroundTask> m_runningTasks;
    private List<TfsBackgroundWorkerManager.BackgroundTask> m_queuedTasks;
    private List<BackgroundWorker> m_freeWorkers;
    private List<BackgroundWorker> m_busyWorkers;
    private object m_lock = new object();
    private CultureInfo m_culture;
    private CultureInfo m_uiCulture;
    private static int s_taskCounter;

    public static TfsBackgroundWorkerManager Instance
    {
      get
      {
        if (TfsBackgroundWorkerManager.s_instance == null)
        {
          TfsBackgroundWorkerManager.s_instance = new TfsBackgroundWorkerManager(15);
          TfsBackgroundWorkerManager.s_instance.SetWorkerGroupThreadLimit(TfsBackgroundWorkerManager.Groups.WorkItemTracking, 5);
          TfsBackgroundWorkerManager.s_instance.SetWorkerGroupThreadLimit(TfsBackgroundWorkerManager.Groups.TeamExplorer, 5);
          TfsBackgroundWorkerManager.s_instance.SetWorkerGroupThreadLimit(TfsBackgroundWorkerManager.Groups.Build, 3);
          TfsBackgroundWorkerManager.s_instance.SetWorkerGroupThreadLimit(TfsBackgroundWorkerManager.Groups.TeamExplorerAsyncDocumentLauncher, 5);
        }
        return TfsBackgroundWorkerManager.s_instance;
      }
    }

    public TfsBackgroundWorkerManager()
      : this(3)
    {
    }

    public TfsBackgroundWorkerManager(int maxWorkers)
    {
      this.m_maxWorkers = maxWorkers;
      this.Initialize();
    }

    public void Shutdown()
    {
      if (this.isShuttingDown)
        return;
      this.isShuttingDown = true;
      this.ClearQueue();
      this.TryStopAllRunningTasks();
    }

    public bool RegisterWorker(TfsBaseWorker tfsWorker) => this.RegisterWorker(tfsWorker, Guid.Empty);

    public bool RegisterWorker(TfsBaseWorker tfsWorker, Guid groupId)
    {
      lock (this.m_lock)
      {
        if (this.isShuttingDown || this.m_allTasks.ContainsKey(tfsWorker.Identifier))
          return false;
        TfsBackgroundWorkerManager.BackgroundTask backgroundTask = new TfsBackgroundWorkerManager.BackgroundTask();
        backgroundTask.TFSWorker = tfsWorker;
        backgroundTask.State = TfsWorkerState.Initialized;
        backgroundTask.GroupId = groupId;
        backgroundTask.TFSWorker.WaitHandle.Reset();
        backgroundTask.Worker = (BackgroundWorker) null;
        this.m_allTasks[tfsWorker.Identifier] = backgroundTask;
        Interlocked.Increment(ref TfsBackgroundWorkerManager.s_taskCounter);
        return true;
      }
    }

    public void SetWorkerGroupThreadLimit(Guid groupId, int maxWorkers) => this.m_groupMaxWorkers[groupId] = maxWorkers;

    public bool RunWorker(object identifier, object argument, TfsBackgroundWorkerPriority priority)
    {
      bool flag = false;
      TfsBackgroundWorkerManager.BackgroundTask task = (TfsBackgroundWorkerManager.BackgroundTask) null;
      BackgroundWorker worker = (BackgroundWorker) null;
      lock (this.m_lock)
      {
        if (this.isShuttingDown)
          return false;
        if (this.m_allTasks.TryGetValue(identifier, out task))
        {
          if (task.State == TfsWorkerState.Initialized)
          {
            task.Argument = argument;
            if (this.m_freeWorkers.Count > 0 && !this.IsTaskExceedingThreadLimit(task))
            {
              worker = this.m_freeWorkers[0];
              this.PrepareRunWorker(task, worker);
            }
            else
              this.EnqueueTaskInternal(task, priority);
            flag = true;
          }
          else if (task.State == TfsWorkerState.Queued)
          {
            if (priority == TfsBackgroundWorkerPriority.Highest)
            {
              this.m_queuedTasks.Remove(task);
              this.EnqueueTaskInternal(task, priority);
            }
            flag = true;
          }
        }
      }
      if (task != null && worker != null)
        worker.RunWorkerAsync((object) task);
      return flag;
    }

    public TfsWorkerState QueryStatus(object identifier)
    {
      lock (this.m_lock)
      {
        TfsBackgroundWorkerManager.BackgroundTask backgroundTask;
        return this.m_allTasks.TryGetValue(identifier, out backgroundTask) ? backgroundTask.State : TfsWorkerState.Unknown;
      }
    }

    public bool IsQueued(object identifier)
    {
      switch (this.QueryStatus(identifier))
      {
        case TfsWorkerState.Initialized:
        case TfsWorkerState.Queued:
          return true;
        default:
          return false;
      }
    }

    public bool IsQueuedOrRunning(object identifier)
    {
      switch (this.QueryStatus(identifier))
      {
        case TfsWorkerState.Initialized:
        case TfsWorkerState.Queued:
        case TfsWorkerState.Running:
          return true;
        default:
          return false;
      }
    }

    public bool TryGetWaitHandle(object identifier, out WaitHandle waitHandle)
    {
      waitHandle = (WaitHandle) null;
      lock (this.m_lock)
      {
        if (!this.m_allTasks.ContainsKey(identifier))
          return false;
        waitHandle = (WaitHandle) this.m_allTasks[identifier].TFSWorker.WaitHandle;
        return true;
      }
    }

    public bool GetArgument(object identifier, out object argument)
    {
      argument = (object) null;
      lock (this.m_lock)
      {
        TfsBackgroundWorkerManager.BackgroundTask backgroundTask;
        if (this.m_allTasks.TryGetValue(identifier, out backgroundTask))
        {
          argument = backgroundTask.Argument;
          return true;
        }
      }
      return false;
    }

    public bool SetArgument(object identifier, object argument)
    {
      lock (this.m_lock)
      {
        TfsBackgroundWorkerManager.BackgroundTask backgroundTask;
        if (this.m_allTasks.TryGetValue(identifier, out backgroundTask))
        {
          backgroundTask.Argument = argument;
          return true;
        }
      }
      return false;
    }

    public bool CancelWorker(object identifier)
    {
      TfsBackgroundWorkerManager.BackgroundTask backgroundTask = (TfsBackgroundWorkerManager.BackgroundTask) null;
      bool flag = false;
      lock (this.m_lock)
      {
        if (!this.m_allTasks.TryGetValue(identifier, out backgroundTask))
          return false;
        if (backgroundTask.State == TfsWorkerState.Running)
          backgroundTask.TFSWorker.PendingCancellation = true;
        else if (backgroundTask.State == TfsWorkerState.Queued)
        {
          backgroundTask.TFSWorker.PendingCancellation = true;
          backgroundTask.State = TfsWorkerState.Cancelled;
          flag = true;
          this.m_allTasks.Remove(identifier);
          this.m_queuedTasks.Remove(backgroundTask);
          Interlocked.Decrement(ref TfsBackgroundWorkerManager.s_taskCounter);
        }
        else
        {
          this.m_allTasks.Remove(identifier);
          Interlocked.Decrement(ref TfsBackgroundWorkerManager.s_taskCounter);
        }
      }
      if (flag && backgroundTask != null)
      {
        backgroundTask.TFSWorker.WaitHandle.Set();
        backgroundTask.TFSWorker.WorkCompleted(backgroundTask.Argument, (object) null, new AsyncCompletedEventArgs((Exception) null, true, (object) null));
      }
      return true;
    }

    public IEnumerable<object> GetIdentifiersByGroup(Guid groupId) => this.m_allTasks.Where<KeyValuePair<object, TfsBackgroundWorkerManager.BackgroundTask>>((Func<KeyValuePair<object, TfsBackgroundWorkerManager.BackgroundTask>, bool>) (pair => pair.Value.GroupId == groupId)).Select<KeyValuePair<object, TfsBackgroundWorkerManager.BackgroundTask>, object>((Func<KeyValuePair<object, TfsBackgroundWorkerManager.BackgroundTask>, object>) (pair => pair.Key));

    public void CancelAll()
    {
      this.ClearQueue();
      this.TryStopAllRunningTasks();
    }

    public void CancelAllQueued() => this.ClearQueue();

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public int FreeWorkersCount => this.m_freeWorkers.Count;

    public int MaxWorkersCount => this.m_maxWorkers;

    protected void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.Shutdown();
    }

    private void Initialize()
    {
      this.m_allTasks = new Dictionary<object, TfsBackgroundWorkerManager.BackgroundTask>();
      this.m_runningTasks = new List<TfsBackgroundWorkerManager.BackgroundTask>();
      this.m_queuedTasks = new List<TfsBackgroundWorkerManager.BackgroundTask>();
      this.m_busyWorkers = new List<BackgroundWorker>();
      this.m_freeWorkers = new List<BackgroundWorker>(this.m_maxWorkers);
      this.m_groupMaxWorkers = new Dictionary<Guid, int>();
      for (int index = 0; index < this.m_maxWorkers; ++index)
      {
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        backgroundWorker.DoWork += new DoWorkEventHandler(this.worker_DoWork);
        backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
        this.m_freeWorkers.Add(backgroundWorker);
      }
      this.m_culture = Thread.CurrentThread.CurrentCulture;
      this.m_uiCulture = Thread.CurrentThread.CurrentUICulture;
    }

    private void EnqueueTaskInternal(
      TfsBackgroundWorkerManager.BackgroundTask task,
      TfsBackgroundWorkerPriority runFlag)
    {
      switch (runFlag)
      {
        case TfsBackgroundWorkerPriority.Highest:
          this.m_queuedTasks.Insert(0, task);
          break;
        case TfsBackgroundWorkerPriority.Normal:
          this.m_queuedTasks.Add(task);
          break;
      }
      task.State = TfsWorkerState.Queued;
    }

    private bool IsTaskExceedingThreadLimit(TfsBackgroundWorkerManager.BackgroundTask task)
    {
      if (task.GroupId == Guid.Empty)
        return false;
      int num1 = 0;
      foreach (TfsBackgroundWorkerManager.BackgroundTask runningTask in this.m_runningTasks)
      {
        if (runningTask.GroupId == task.GroupId)
          ++num1;
      }
      int num2 = 1;
      if (this.m_groupMaxWorkers.ContainsKey(task.GroupId))
        num2 = this.m_groupMaxWorkers[task.GroupId];
      return num1 >= num2;
    }

    private void TryStopAllRunningTasks()
    {
      lock (this.m_lock)
      {
        foreach (TfsBackgroundWorkerManager.BackgroundTask runningTask in this.m_runningTasks)
          runningTask.TFSWorker.PendingCancellation = true;
      }
    }

    private void ClearQueue()
    {
      List<TfsBackgroundWorkerManager.BackgroundTask> backgroundTaskList = new List<TfsBackgroundWorkerManager.BackgroundTask>();
      lock (this.m_lock)
      {
        foreach (TfsBackgroundWorkerManager.BackgroundTask queuedTask in this.m_queuedTasks)
        {
          queuedTask.TFSWorker.PendingCancellation = true;
          queuedTask.State = TfsWorkerState.Cancelled;
          backgroundTaskList.Add(queuedTask);
          this.m_allTasks.Remove(queuedTask.TFSWorker.Identifier);
          Interlocked.Decrement(ref TfsBackgroundWorkerManager.s_taskCounter);
        }
        foreach (TfsBackgroundWorkerManager.BackgroundTask backgroundTask in this.m_allTasks.Values)
          Interlocked.Decrement(ref TfsBackgroundWorkerManager.s_taskCounter);
        this.m_queuedTasks.Clear();
      }
      foreach (TfsBackgroundWorkerManager.BackgroundTask backgroundTask in backgroundTaskList)
      {
        backgroundTask.TFSWorker.WaitHandle.Set();
        backgroundTask.TFSWorker.WorkCompleted(backgroundTask.Argument, (object) null, new AsyncCompletedEventArgs((Exception) null, true, (object) null));
      }
    }

    private void PrepareRunWorker(
      TfsBackgroundWorkerManager.BackgroundTask task,
      BackgroundWorker worker)
    {
      this.m_freeWorkers.Remove(worker);
      this.m_busyWorkers.Add(worker);
      this.m_runningTasks.Add(task);
      task.State = TfsWorkerState.Running;
      if (this.m_queuedTasks.Contains(task))
        this.m_queuedTasks.Remove(task);
      task.Worker = worker;
    }

    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
      CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
      CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
      try
      {
        Thread.CurrentThread.CurrentCulture = this.m_culture;
        Thread.CurrentThread.CurrentUICulture = this.m_uiCulture;
        TfsBackgroundWorkerManager.BackgroundTask backgroundTask = e.Argument as TfsBackgroundWorkerManager.BackgroundTask;
        TfsBackgroundWorkerManager.WorkerResult workerResult = new TfsBackgroundWorkerManager.WorkerResult();
        workerResult.Task = backgroundTask;
        try
        {
          workerResult.Result = backgroundTask.TFSWorker.DoWork(backgroundTask.Argument, (CancelEventArgs) e);
        }
        catch (Exception ex)
        {
          workerResult.Error = ex;
          workerResult.Result = (object) null;
        }
        finally
        {
          backgroundTask.TFSWorker.WaitHandle.Set();
        }
        if (e.Cancel)
        {
          e.Cancel = false;
          workerResult.Cancelled = true;
        }
        e.Result = (object) workerResult;
      }
      finally
      {
        if (currentCulture != null)
          Thread.CurrentThread.CurrentCulture = currentCulture;
        if (currentUiCulture != null)
          Thread.CurrentThread.CurrentUICulture = currentUiCulture;
      }
    }

    private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      TfsBackgroundWorkerManager.WorkerResult result = e.Result as TfsBackgroundWorkerManager.WorkerResult;
      TfsBackgroundWorkerManager.BackgroundTask task = result.Task;
      lock (this.m_lock)
      {
        this.m_runningTasks.Remove(task);
        this.m_allTasks.Remove(task.TFSWorker.Identifier);
        BackgroundWorker worker = task.Worker;
        this.m_busyWorkers.Remove(worker);
        task.Worker = (BackgroundWorker) null;
        this.m_freeWorkers.Add(worker);
      }
      task.State = TfsWorkerState.Completed;
      try
      {
        if (!this.isShuttingDown)
        {
          AsyncCompletedEventArgs e1 = new AsyncCompletedEventArgs(result.Error, result.Cancelled || task.TFSWorker.PendingCancellation, (object) null);
          task.TFSWorker.WorkCompleted(task.Argument, result.Result, e1);
        }
      }
      finally
      {
        Interlocked.Decrement(ref TfsBackgroundWorkerManager.s_taskCounter);
      }
      this.RunEnqueuedTask();
    }

    private void RunEnqueuedTask()
    {
      TfsBackgroundWorkerManager.BackgroundTask task = (TfsBackgroundWorkerManager.BackgroundTask) null;
      BackgroundWorker worker = (BackgroundWorker) null;
      lock (this.m_lock)
      {
        if (this.m_freeWorkers.Count > 0)
        {
          if (this.m_queuedTasks.Count > 0)
          {
            foreach (TfsBackgroundWorkerManager.BackgroundTask queuedTask in this.m_queuedTasks)
            {
              if (!this.IsTaskExceedingThreadLimit(queuedTask))
              {
                task = queuedTask;
                break;
              }
            }
            if (task != null)
            {
              this.m_queuedTasks.Remove(task);
              worker = this.m_freeWorkers[0];
              this.PrepareRunWorker(task, worker);
            }
          }
        }
      }
      if (task == null || worker == null)
        return;
      worker.RunWorkerAsync((object) task);
    }

    public static int TaskCounter => TfsBackgroundWorkerManager.s_taskCounter;

    public static class Groups
    {
      public static readonly Guid TeamExplorer = new Guid("{A8CFB34E-67A1-4DC1-9149-E471901B76FA}");
      public static readonly Guid WorkItemTracking = new Guid("{BF39DD8F-CBBD-4727-907F-E6E598C921F8}");
      public static readonly Guid Build = new Guid("1BC2A3E2-34EF-42AF-8403-DEEC238D123C");
      public static readonly Guid TeamExplorerAsyncDocumentLauncher = new Guid("DAA355FF-45F1-4BEF-A72C-78EA8C005CB0");
      public static readonly Guid WITAttachmentUploads = new Guid("EBC45692-5B28-BA49-E78A-B29A90213026");
    }

    private class BackgroundTask
    {
      public TfsBaseWorker TFSWorker { get; set; }

      public TfsWorkerState State { get; set; }

      public BackgroundWorker Worker { get; set; }

      public object Argument { get; set; }

      public Guid GroupId { get; set; }
    }

    private class WorkerResult
    {
      public object Result { get; set; }

      public TfsBackgroundWorkerManager.BackgroundTask Task { get; set; }

      public Exception Error { get; set; }

      public bool Cancelled { get; set; }
    }
  }
}
