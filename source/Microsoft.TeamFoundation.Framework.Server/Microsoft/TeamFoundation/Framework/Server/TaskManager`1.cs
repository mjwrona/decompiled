// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TaskManager`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class TaskManager<T> : IDisposable
  {
    private bool m_isShuttingDown;
    private DeploymentServiceHost m_serviceHost;
    private TaskRunner<T>[] m_taskRunners;
    private const int DefaultMaxTaskRunners = 32;
    private ReaderWriterLockSlim m_taskServiceLock = new ReaderWriterLockSlim();
    private const string s_Area = "TeamFoundationTaskService";
    private const string s_Layer = "Task";
    private T m_highPrio;
    private string m_name;
    private TimeSpan m_queueRunningErrorThreshold;
    private TimeSpan m_queueRunningWarningThreshold;
    private TimeSpan m_slowNotificationDueThreshold;
    private TimeSpan m_slowNotificationPeriodThreshold;
    private Timer m_slowNotificationTimer;
    private const string c_registrySettingsRootPath = "/Configuration/TaskManager/Settings";
    private const string c_registrySettingsPathErrorRunningThreshold = "/Configuration/TaskManager/Settings/QueueRunningErrorThreshold";
    private const string c_registrySettingsPathWarningRunningThreshold = "/Configuration/TaskManager/Settings/QueueRunningWarningThreshold";
    private const string c_registrySettingsPathSlowNotificationDueThreshold = "/Configuration/TaskManager/Settings/SlowNotificationDueThreshold";
    private const string c_registrySettingsPathSlowNotificationPeriodThreshold = "/Configuration/TaskManager/Settings/SlowNotificationPeriodThreshold";
    private const int c_defaultQueueRunningErrorThreshold = 10;
    private const int c_defaultQueueRunningWarningThreshold = 1;
    private const int c_defaultSlowNotification = 1;

    public TaskManager(
      IVssRequestContext systemRequestContext,
      string poolName,
      string registryKey,
      T highPrio)
    {
      TaskManager<T> taskManager = this;
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_serviceHost = (DeploymentServiceHost) systemRequestContext.ServiceHost.DeploymentServiceHost;
      this.m_highPrio = highPrio;
      this.m_name = poolName;
      RegistryEntryCollection registryEntryCollection = new RegistryEntryCollection("/Configuration/TaskManager/Settings", RegistryHelpers.GetDeploymentValuesRaw(systemRequestContext.FrameworkConnectionInfo, "/Configuration/TaskManager/Settings"));
      this.m_queueRunningErrorThreshold = TimeSpan.FromMinutes((double) registryEntryCollection.GetValueFromPath<int>("/Configuration/TaskManager/Settings/QueueRunningErrorThreshold", 10));
      this.m_queueRunningWarningThreshold = TimeSpan.FromMinutes((double) registryEntryCollection.GetValueFromPath<int>("/Configuration/TaskManager/Settings/QueueRunningWarningThreshold", 1));
      this.m_slowNotificationDueThreshold = TimeSpan.FromMinutes((double) registryEntryCollection.GetValueFromPath<int>("/Configuration/TaskManager/Settings/SlowNotificationDueThreshold", 1));
      this.m_slowNotificationPeriodThreshold = TimeSpan.FromMinutes((double) registryEntryCollection.GetValueFromPath<int>("/Configuration/TaskManager/Settings/SlowNotificationPeriodThreshold", 1));
      int maxTaskRunners;
      try
      {
        maxTaskRunners = RegistryHelpers.GetDeploymentValueRaw<int>(systemRequestContext.FrameworkConnectionInfo, registryKey, 32);
      }
      catch (DatabaseConnectionException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(27140, "TeamFoundationTaskService", "Task", (Exception) ex);
        TeamFoundationEventLog.Default.LogException(systemRequestContext, FrameworkResources.DatabaseConnectionException(), (Exception) ex, TeamFoundationEventId.DatabaseConnectionException, EventLogEntryType.Warning);
        maxTaskRunners = 32;
      }
      if (maxTaskRunners < 1)
        maxTaskRunners = 1;
      this.m_taskRunners = new TaskRunner<T>[maxTaskRunners];
      TaskRunnerSettings settings = new TaskRunnerSettings(systemRequestContext);
      for (int taskRunnerId = 0; taskRunnerId < maxTaskRunners; ++taskRunnerId)
        this.m_taskRunners[taskRunnerId] = new TaskRunner<T>(systemRequestContext, (IVssDeploymentServiceHost) this.m_serviceHost, taskRunnerId, this.m_name, taskRunnerId == 0, settings);
      this.m_slowNotificationTimer = new Timer((TimerCallback) (state =>
      {
        for (int index = 0; index < maxTaskRunners; ++index)
        {
          CurrentTaskDetails<T> currentTaskDetails;
          if (taskManager.m_taskRunners != null && taskManager.m_taskRunners[index] != null && taskManager.m_taskRunners[index].TryGetCurrentTaskDetails(out currentTaskDetails))
          {
            TimeSpan timeSpan = DateTime.UtcNow.Subtract(currentTaskDetails.StartTime);
            if (timeSpan > taskManager.m_queueRunningErrorThreshold)
              TeamFoundationTracingService.TraceRawAlwaysOn(27111, TraceLevel.Error, "TeamFoundationTaskService", "Task", string.Format("{0} with the QueueId:{1} has been running in the host:{2} and been processingFor:{3} ", (object) currentTaskDetails, (object) index, (object) taskManager.m_serviceHost, (object) timeSpan.TotalMinutes));
            else if (timeSpan > taskManager.m_queueRunningWarningThreshold)
              TeamFoundationTracingService.TraceRaw(27112, TraceLevel.Warning, "TeamFoundationTaskService", "Task", string.Format("{0} with the QueueId:{1} has been running in the host:{2} and been processingFor:{3}", (object) 0, (object) 1, (object) 2, (object) 3), (object) currentTaskDetails, (object) index, (object) taskManager.m_serviceHost, (object) timeSpan.TotalMinutes);
          }
        }
      }), (object) Thread.CurrentThread, this.m_slowNotificationDueThreshold, this.m_slowNotificationPeriodThreshold);
    }

    public void Dispose()
    {
      try
      {
        this.m_taskServiceLock.EnterWriteLock();
        try
        {
          this.m_isShuttingDown = true;
          if (this.m_slowNotificationTimer != null)
          {
            this.m_slowNotificationTimer.Dispose();
            this.m_slowNotificationTimer = (Timer) null;
          }
        }
        finally
        {
          this.m_taskServiceLock.ExitWriteLock();
        }
        this.m_taskServiceLock.EnterWriteLock();
        try
        {
          if (this.m_taskRunners == null)
            return;
          for (int index = 0; index < this.m_taskRunners.Length; ++index)
          {
            if (this.m_taskRunners[index] != null)
            {
              this.m_taskRunners[index].Dispose();
              this.m_taskRunners[index] = (TaskRunner<T>) null;
            }
          }
          this.m_taskRunners = (TaskRunner<T>[]) null;
        }
        finally
        {
          this.m_taskServiceLock.ExitWriteLock();
        }
      }
      finally
      {
        this.m_taskServiceLock.Dispose();
      }
    }

    internal void AddTask(T instanceId, TeamFoundationTask<T> task)
    {
      if (instanceId is string connectionString)
        TeamFoundationTracingService.TraceEnterRaw(27100, "TeamFoundationTaskService", "Task", "TeamFoundationTaskService.AddTask {0} {1}", (object) ConnectionStringUtility.MaskPassword(connectionString), (object) task);
      else
        TeamFoundationTracingService.TraceEnterRaw(27100, "TeamFoundationTaskService", "Task", "TeamFoundationTaskService.AddTask {0} {1}", (object) instanceId, (object) task);
      try
      {
        if (this.m_isShuttingDown)
          return;
        bool flag = false;
        try
        {
          while (!(this.m_isShuttingDown | flag))
            flag = this.m_taskServiceLock.TryEnterReadLock(1);
          if (!flag)
            TeamFoundationTracingService.TraceRaw(27106, TraceLevel.Warning, "TeamFoundationTaskService", "Task", "AddTask bailing out because we're shutting down");
          else if (this.m_isShuttingDown || this.m_taskRunners == null)
          {
            TeamFoundationTracingService.TraceRaw(27107, TraceLevel.Warning, "TeamFoundationTaskService", "Task", "AddTask bailing out because we're shutting down");
          }
          else
          {
            if (object.Equals((object) task.Identifier, (object) default (T)))
              task.Identifier = instanceId;
            this.m_taskRunners[this.GetBucket(instanceId)].AddTask(task);
          }
        }
        finally
        {
          if (flag)
            this.m_taskServiceLock.ExitReadLock();
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(27109, "TeamFoundationTaskService", "Task", ex);
        throw;
      }
      finally
      {
        string fullName = task.Callback.Method.DeclaringType.FullName;
        VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskService_TotalTaskQueued", fullName);
        performanceCounter.Increment();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskService_TaskQueuedPerSec", fullName);
        performanceCounter.Increment();
        TeamFoundationTracingService.TraceLeaveRaw(27110, "TeamFoundationTaskService", "Task", "TeamFoundationTaskService.AddTask");
      }
    }

    public bool IsHighPriority(T identifier) => this.m_highPrio.Equals((object) identifier);

    public void RemoveTask(T instanceId, TeamFoundationTask<T> task) => this.RemoveTask(instanceId, task, false);

    internal void RemoveTask(T instanceId, TeamFoundationTask<T> task, bool isSpecial)
    {
      if (object.Equals((object) task.Identifier, (object) default (T)))
        task.Identifier = instanceId;
      if (instanceId is string connectionString)
        TeamFoundationTracingService.TraceEnterRaw(27120, "TeamFoundationTaskService", "Task", "TeamFoundationTaskService.RemoveTask {0} {1}", (object) ConnectionStringUtility.MaskPassword(connectionString), (object) task);
      else
        TeamFoundationTracingService.TraceEnterRaw(27120, "TeamFoundationTaskService", "Task", "TeamFoundationTaskService.RemoveTask {0} {1}", (object) instanceId, (object) task);
      try
      {
        if (this.m_isShuttingDown)
          return;
        this.m_taskServiceLock.EnterReadLock();
        try
        {
          if (this.m_isShuttingDown || this.m_taskRunners == null)
            return;
          this.m_taskRunners[this.GetBucket(instanceId)].RemoveTask(task);
        }
        finally
        {
          this.m_taskServiceLock.ExitReadLock();
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(27128, "TeamFoundationTaskService", "Task", ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(27129, "TeamFoundationTaskService", "Task", "TeamFoundationTaskService.RemoveTask");
      }
    }

    internal void RemoveAllTasksForHost(T instanceId)
    {
      TeamFoundationTracingService.TraceEnterRaw(27130, "TeamFoundationTaskService", "Task", "TeamFoundationTaskService.RemoveAllTasksForHost {0}", (object) instanceId);
      try
      {
        if (this.m_isShuttingDown)
          return;
        this.m_taskServiceLock.EnterReadLock();
        try
        {
          if (this.m_isShuttingDown || this.m_taskRunners == null)
            return;
          this.m_taskRunners[this.GetBucket(instanceId)].RemoveTaskForHost(instanceId);
        }
        finally
        {
          this.m_taskServiceLock.ExitReadLock();
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(27138, "TeamFoundationTaskService", "Task", ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(27139, "TeamFoundationTaskService", "Task", "TeamFoundationTaskService.RemoveAllTasksForHost");
      }
    }

    private int GetBucket(T instanceId) => this.IsHighPriority(instanceId) ? 0 : Math.Abs(instanceId.GetHashCode() % (this.m_taskRunners.Length - 1)) + 1;
  }
}
