// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TaskRunner`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TaskRunner<T> : IDisposable
  {
    private bool m_isDisposed;
    private readonly int m_taskRunnerId;
    private IVssDeploymentServiceHost m_deploymentServiceHost;
    private AutoResetEvent m_event = new AutoResetEvent(true);
    private Thread m_taskProcessingThread;
    private readonly Dictionary<TeamFoundationTask<T>, TaskQueueEntry<T>> m_taskEntryLookup;
    private readonly SortedDictionary<TaskQueueEntry<T>, object> m_taskQueue;
    private const string s_Area = "TeamFoundationTaskService";
    private const string s_Layer = "TaskRunner";
    private long m_taskId;
    private readonly string m_poolName;
    private Guid m_uniqueIdentifier;
    private readonly bool m_isHighPriority;
    private CurrentTaskDetails<T> m_currentTaskDetails;
    private readonly TaskRunnerSettings m_settings;

    public TaskRunner(
      IVssRequestContext systemRequestContext,
      IVssDeploymentServiceHost deploymentServiceHost,
      int taskRunnerId,
      string poolName,
      bool isHighPriority)
      : this(systemRequestContext, deploymentServiceHost, taskRunnerId, poolName, isHighPriority, new TaskRunnerSettings(systemRequestContext))
    {
    }

    public TaskRunner(
      IVssRequestContext systemRequestContext,
      IVssDeploymentServiceHost deploymentServiceHost,
      int taskRunnerId,
      string poolName,
      bool isHighPriority,
      TaskRunnerSettings settings)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_taskRunnerId = taskRunnerId;
      this.m_deploymentServiceHost = deploymentServiceHost;
      this.m_taskEntryLookup = new Dictionary<TeamFoundationTask<T>, TaskQueueEntry<T>>();
      this.m_taskQueue = new SortedDictionary<TaskQueueEntry<T>, object>();
      this.m_poolName = poolName;
      this.m_isHighPriority = isHighPriority;
      this.m_uniqueIdentifier = TaskRunner<T>.CreateUniqueIdentifier(this.m_taskRunnerId, this.m_poolName);
      this.m_settings = settings;
    }

    public void Dispose()
    {
      lock (this)
      {
        if (this.m_isDisposed)
          return;
        this.m_isDisposed = true;
      }
      this.m_event?.Set();
      if (this.m_taskProcessingThread != null)
      {
        if (this.m_taskProcessingThread.ThreadState != System.Threading.ThreadState.Unstarted)
          this.m_taskProcessingThread.Join();
        this.m_taskProcessingThread = (Thread) null;
      }
      this.m_event?.Close();
      this.m_event = (AutoResetEvent) null;
      this.m_deploymentServiceHost = (IVssDeploymentServiceHost) null;
    }

    public void AddTask(TeamFoundationTask<T> task)
    {
      ArgumentUtility.CheckForNull<TeamFoundationTask<T>>(task, nameof (task));
      if (object.Equals((object) task.Identifier, (object) default (T)))
        throw new InvalidOperationException("task.Identifier cannot be null");
      if (TeamFoundationTracingService.IsRawTracingEnabled(27159, TraceLevel.Info, "TeamFoundationTaskService", nameof (TaskRunner<T>), (string[]) null))
        task.CreatorStackTrace = new StackTracer().ToString();
      lock (this)
      {
        if (this.m_isDisposed)
          return;
        if (this.m_taskProcessingThread == null)
        {
          this.m_taskProcessingThread = new Thread(new ThreadStart(this.ProcessTaskQueue))
          {
            IsBackground = true
          };
          try
          {
            this.m_taskProcessingThread.Name = string.Format("Team Foundation Task Thread {0} {1}", (object) this.m_poolName, (object) this.m_taskRunnerId);
          }
          catch (Exception ex)
          {
            TeamFoundationTrace.TraceException(ex);
          }
          this.m_taskProcessingThread.Start();
        }
        DateTime? queueTime = new DateTime?();
        TaskQueueEntry<T> key;
        if (this.m_taskEntryLookup.TryGetValue(task, out key))
        {
          this.m_taskQueue.Remove(key);
          this.m_taskEntryLookup.Remove(task);
          if (task.KeepStartTimeOnRequeue)
            queueTime = new DateTime?(key.QueueTime);
        }
        key = new TaskQueueEntry<T>(task, Interlocked.Increment(ref this.m_taskId), queueTime);
        this.m_taskEntryLookup.Add(task, key);
        this.m_taskQueue.Add(key, (object) null);
        int count = this.m_taskQueue.Count;
        if (count > this.m_settings.QueueLimitThreshold)
          TeamFoundationTracingService.TraceRawAlwaysOn(27168, TraceLevel.Error, "TeamFoundationTaskService", nameof (TaskRunner<T>), string.Format("Queued Task for the bucket:{0} with size:{1} and set threshold:{2}", (object) this.m_taskRunnerId, (object) count, (object) this.m_settings.QueueLimitThreshold));
        this.m_event.Set();
      }
    }

    public void RemoveTask(TeamFoundationTask<T> task)
    {
      ArgumentUtility.CheckForNull<TeamFoundationTask<T>>(task, nameof (task));
      if (object.Equals((object) task.Identifier, (object) default (T)))
        throw new InvalidOperationException("Task identifier is not specified");
      lock (this)
      {
        if (this.m_isDisposed)
          return;
        TaskQueueEntry<T> key;
        if (this.m_taskEntryLookup.TryGetValue(task, out key))
        {
          this.m_taskQueue.Remove(key);
          this.m_taskEntryLookup.Remove(task);
        }
        else
          TeamFoundationTracingService.TraceRaw(27140, TraceLevel.Warning, "TeamFoundationTaskService", nameof (TaskRunner<T>), "Task not found {0}", (object) task);
      }
    }

    public void RemoveTaskForHost(T identifier)
    {
      lock (this)
      {
        if (this.m_isDisposed)
          return;
        List<TeamFoundationTask<T>> teamFoundationTaskList = new List<TeamFoundationTask<T>>();
        foreach (KeyValuePair<TeamFoundationTask<T>, TaskQueueEntry<T>> keyValuePair in this.m_taskEntryLookup)
        {
          TeamFoundationTask<T> key = keyValuePair.Key;
          if (object.Equals((object) key.Identifier, (object) identifier))
          {
            this.m_taskQueue.Remove(keyValuePair.Value);
            teamFoundationTaskList.Add(key);
          }
        }
        foreach (TeamFoundationTask<T> key in teamFoundationTaskList)
          this.m_taskEntryLookup.Remove(key);
      }
    }

    private void CompleteTask(TaskQueueEntry<T> completedQueueEntry)
    {
      lock (this)
      {
        TaskQueueEntry<T> taskQueueEntry;
        if (this.m_isDisposed || !this.m_taskEntryLookup.TryGetValue(completedQueueEntry.Task, out taskQueueEntry) || completedQueueEntry != taskQueueEntry)
          return;
        this.m_taskQueue.Remove(completedQueueEntry);
        if (completedQueueEntry.Task.Interval > 0)
        {
          completedQueueEntry.QueueTime = completedQueueEntry.Task.GetNextRunTime(true);
          this.m_taskQueue.Add(completedQueueEntry, (object) null);
        }
        else
          this.m_taskEntryLookup.Remove(completedQueueEntry.Task);
      }
    }

    private TaskQueueEntry<T> GetNextTaskEntry()
    {
      lock (this)
        return this.m_isDisposed ? (TaskQueueEntry<T>) null : this.m_taskQueue.Keys.FirstOrDefault<TaskQueueEntry<T>>();
    }

    private void ProcessTaskQueue()
    {
      int millisecondsTimeout = -1;
      if (this.m_isHighPriority && string.Equals("NotificationPool", this.m_poolName, StringComparison.OrdinalIgnoreCase))
        this.m_deploymentServiceHost.DeploymentServiceHostInternal().HostManagement.SetRequestIdForNotificationThread();
      TeamFoundationTracingService.TraceRawAlwaysOn(27165, TraceLevel.Info, "TeamFoundationTaskService", nameof (TaskRunner<T>), string.Format("Task Runner {0} {1} running on managed thread {2}", (object) this.m_poolName, (object) this.m_taskRunnerId, (object) Environment.CurrentManagedThreadId));
      while (true)
      {
        if (this.m_event.WaitOne(millisecondsTimeout, false))
          TeamFoundationTracingService.TraceRaw(27150, TraceLevel.Verbose, "TeamFoundationTaskService", nameof (TaskRunner<T>), "TaskRunner {0} waking up because it was signaled", (object) this);
        else
          TeamFoundationTracingService.TraceRaw(27151, TraceLevel.Verbose, "TeamFoundationTaskService", nameof (TaskRunner<T>), "TaskRunner {0} waking up because it's time to process tasks", (object) this);
        if (!this.m_isDisposed)
        {
          try
          {
            millisecondsTimeout = -1;
            for (TaskQueueEntry<T> nextTaskEntry = this.GetNextTaskEntry(); nextTaskEntry != null; nextTaskEntry = this.GetNextTaskEntry())
            {
              DateTime queueTime = nextTaskEntry.QueueTime;
              DateTime utcNow = DateTime.UtcNow;
              if (queueTime > utcNow)
              {
                double totalMilliseconds = queueTime.Subtract(utcNow).TotalMilliseconds;
                millisecondsTimeout = Math.Max((int) totalMilliseconds, 20);
                if (millisecondsTimeout < 0)
                {
                  TeamFoundationTracingService.TraceRaw(27154, TraceLevel.Error, "TeamFoundationTaskService", nameof (TaskRunner<T>), "Task is over 24 days in the future?! milliseconds to wait: {0} (double) / {1} (Int32)", (object) totalMilliseconds, (object) millisecondsTimeout);
                  millisecondsTimeout = int.MaxValue;
                }
                TeamFoundationTracingService.TraceRaw(27155, TraceLevel.Info, "TeamFoundationTaskService", nameof (TaskRunner<T>), "Task Runner {0} waiting for {1} ms", (object) this, (object) millisecondsTimeout);
                break;
              }
              try
              {
                using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(false))
                {
                  DefaultRequestContext defaultRequestContext = (DefaultRequestContext) null;
                  DefaultRequestContext requestContext = (DefaultRequestContext) null;
                  Stopwatch stopwatch = (Stopwatch) null;
                  try
                  {
                    Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                    requestContext = !nextTaskEntry.Task.NeedsTargetRequestContext ? (DefaultRequestContext) systemContext : (defaultRequestContext = (DefaultRequestContext) nextTaskEntry.Task.GetRequestContext(systemContext, nextTaskEntry.Task.Identifier));
                    if (requestContext != null)
                    {
                      requestContext.Items.Add("IsTaskThread", (object) true);
                      requestContext.Items.Add(RequestContextItemsKeys.TaskPoolName, (object) this.m_poolName);
                      requestContext.SetUniqueIdentifier(this.m_uniqueIdentifier);
                      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_TaskExecutedPerSecond").Increment();
                      TimeSpan timeSpan = DateTime.UtcNow - nextTaskEntry.QueueTime;
                      if (timeSpan > this.m_settings.WaitedOnQueueErrorThreshold)
                        TeamFoundationTracingService.TraceRawAlwaysOn(27162, TraceLevel.Error, "TeamFoundationTaskService", nameof (TaskRunner<T>), string.Format("The task {0} with task id {1} for the host {2} stayed in the queue for {3} ms which is over {4} seconds.", (object) nextTaskEntry.Task.Callback.Method.Name, (object) nextTaskEntry.TaskId, (object) requestContext.ServiceHost.InstanceId, (object) timeSpan, (object) this.m_settings.WaitedOnQueueErrorThreshold));
                      else if (timeSpan > this.m_settings.WaitedOnQueueWarningThreshold)
                        TeamFoundationTracingService.TraceRawAlwaysOn(27166, TraceLevel.Warning, "TeamFoundationTaskService", nameof (TaskRunner<T>), string.Format("The task {0} with task id {1} for the host {2} stayed in the queue for {3} ms which is over {4} seconds.", (object) nextTaskEntry.Task.Callback.Method.Name, (object) nextTaskEntry.TaskId, (object) requestContext.ServiceHost.InstanceId, (object) timeSpan, (object) this.m_settings.WaitedOnQueueWarningThreshold));
                      stopwatch = new Stopwatch();
                      TeamFoundationTracingService.TraceRaw(27156, TraceLevel.Info, "TeamFoundationTaskService", nameof (TaskRunner<T>), "Invoking callback {0} with task id {1} for the host {2}", (object) nextTaskEntry.Task.Callback.Method.Name, (object) nextTaskEntry.TaskId, (object) requestContext.ServiceHost.InstanceId);
                      stopwatch.Start();
                      this.m_currentTaskDetails = new CurrentTaskDetails<T>(nextTaskEntry.TaskId, queueTime, nextTaskEntry);
                      VssPerformanceEventSource.Log.TaskCallbackStart(requestContext.ServiceHost.InstanceId, nextTaskEntry.Task.Callback.Method.Name);
                      nextTaskEntry.Task.Callback((IVssRequestContext) requestContext, nextTaskEntry.Task.TaskArgs);
                      VssPerformanceEventSource.Log.TaskCallbackStop(requestContext.ServiceHost.InstanceId, nextTaskEntry.Task.Callback.Method.Name, stopwatch.ElapsedMilliseconds);
                      stopwatch.Stop();
                      TeamFoundationTracingService.TraceRaw(27157, TraceLevel.Info, "TeamFoundationTaskService", nameof (TaskRunner<T>), "{0} with task id {1} for the host {2} returned in {3} ms", (object) nextTaskEntry.Task.Callback.Method.Name, (object) nextTaskEntry.TaskId, (object) requestContext.ServiceHost.InstanceId, (object) stopwatch.ElapsedMilliseconds);
                    }
                  }
                  catch (Exception ex)
                  {
                    TeamFoundationTracingService.TraceRaw(27159, TraceLevel.Error, "TeamFoundationTaskService", nameof (TaskRunner<T>), string.Format("Error during {0}{1}Exception: {2}", (object) nextTaskEntry.Task, (object) Environment.NewLine, (object) ex));
                  }
                  finally
                  {
                    if (requestContext != null && stopwatch != null)
                    {
                      if (stopwatch.IsRunning)
                        stopwatch.Stop();
                      if (stopwatch.ElapsedMilliseconds > 5000L)
                        TeamFoundationTracingService.TraceRawAlwaysOn(27158, stopwatch.ElapsedMilliseconds > 10000L ? TraceLevel.Error : TraceLevel.Warning, "TeamFoundationTaskService", nameof (TaskRunner<T>), "{0} with task id {1} for the host {2} took over {3} ms to complete which is over 5 seconds.", (object) nextTaskEntry.Task.Callback.Method.Name, (object) nextTaskEntry.TaskId, (object) requestContext.ServiceHost.InstanceId, (object) stopwatch.ElapsedMilliseconds);
                    }
                    defaultRequestContext?.Dispose();
                    this.m_currentTaskDetails = (CurrentTaskDetails<T>) null;
                  }
                }
                this.CheckForLeakedLocks(nextTaskEntry);
              }
              catch (Exception ex)
              {
                TeamFoundationTracingService.TraceRaw(27160, TraceLevel.Error, "TeamFoundationTaskService", nameof (TaskRunner<T>), string.Format("Error during {0}{1}Exception: {2}", (object) nextTaskEntry.Task, (object) Environment.NewLine, (object) ex));
              }
              TeamFoundationTracingService.TraceRaw(27161, TraceLevel.Info, "TeamFoundationTaskService", nameof (TaskRunner<T>), "Task Runner {0} completing task for entry {1}", (object) this, (object) nextTaskEntry);
              this.CompleteTask(nextTaskEntry);
            }
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(27163, TraceLevel.Error, "TeamFoundationTaskService", nameof (TaskRunner<T>), ex);
            millisecondsTimeout = 1000;
          }
        }
        else
          break;
      }
      TeamFoundationTracingService.TraceRaw(27152, TraceLevel.Info, "TeamFoundationTaskService", nameof (TaskRunner<T>), "Task Runner {0} exiting", (object) this);
    }

    internal virtual void CheckForLeakedLocks(TaskQueueEntry<T> queueEntryToRun) => this.m_deploymentServiceHost.DeploymentServiceHostInternal().HostManagement.AssertNoLocksHeld(queueEntryToRun.Task.Callback.Method.ToString());

    internal static Guid CreateUniqueIdentifier(int taskRunnerId, string poolName) => new Guid(taskRunnerId.ToString().PadLeft(12, '0') + TaskRunner<T>.ConvertStringToHex(poolName).PadLeft(20, '0').Substring(0, 20));

    private static string ConvertStringToHex(string str)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (char ch in str.ToLower())
      {
        if (ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f')
          stringBuilder.Append(ch);
        else
          stringBuilder.Append(((int) ch % 16).ToString("x"));
      }
      return stringBuilder.ToString();
    }

    internal bool TryGetCurrentTaskDetails(out CurrentTaskDetails<T> currentTaskDetails)
    {
      currentTaskDetails = this.m_currentTaskDetails;
      return currentTaskDetails != null;
    }
  }
}
