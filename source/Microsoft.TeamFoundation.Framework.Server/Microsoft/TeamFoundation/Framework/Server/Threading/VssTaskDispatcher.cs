// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.VssTaskDispatcher
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VssTaskDispatcher : IVssTaskDispatcher
  {
    private bool m_started;
    private readonly string m_name;
    private readonly int m_maxThreadCount;
    private readonly int m_maxTaskConcurrencyPerScheduler;
    private readonly IVssDeploymentServiceHost m_serviceHost;
    private readonly List<Task> m_taskRunners;
    private readonly List<IVssScheduler> m_schedulers;
    private readonly InputQueue<VssTaskDispatcher.TaskQueueEntry> m_runnableTasks;
    private readonly CancellationTokenSource m_cancellationTokenSource;
    private readonly Lazy<VssPerformanceCounter> m_tasksQueuedPerSec;
    private readonly Lazy<VssPerformanceCounter> m_tasksExecutedPerSec;
    private readonly Lazy<VssPerformanceCounter> m_tasksScheduledPerSec;
    private readonly Lazy<VssPerformanceCounter> m_currentTasksQueuedCount;
    private readonly Lazy<VssPerformanceCounter> m_currentTasksExecutingCount;
    private readonly Lazy<VssPerformanceCounter> m_currentTasksScheduledCount;
    private readonly Lazy<VssPerformanceCounter> m_averageTaskQueueTime;
    private readonly Lazy<VssPerformanceCounter> m_averageTaskQueueTimeBase;
    private readonly Lazy<VssPerformanceCounter> m_averageTaskExecutionTime;
    private readonly Lazy<VssPerformanceCounter> m_averageTaskExecutionTimeBase;
    private const string s_Area = "VssTaskService";
    private const string s_Layer = "TaskDispatcher";
    private static readonly TimeSpan s_closeWaitTimeout = TimeSpan.FromSeconds(1.0);

    public VssTaskDispatcher(
      IVssDeploymentServiceHost serviceHost,
      string name,
      int maximumThreadCount,
      int maximumConcurrencyPerThread)
    {
      this.m_name = name;
      this.m_serviceHost = serviceHost;
      this.m_maxThreadCount = maximumThreadCount;
      this.m_maxTaskConcurrencyPerScheduler = maximumConcurrencyPerThread;
      this.m_taskRunners = new List<Task>(this.m_maxThreadCount * this.m_maxTaskConcurrencyPerScheduler);
      this.m_schedulers = ((IEnumerable<IVssScheduler>) Enumerable.Range(0, this.m_maxThreadCount).Select<int, VssThreadPoolScheduler>((Func<int, VssThreadPoolScheduler>) (x => new VssThreadPoolScheduler()))).ToList<IVssScheduler>();
      this.m_runnableTasks = new InputQueue<VssTaskDispatcher.TaskQueueEntry>();
      this.m_cancellationTokenSource = new CancellationTokenSource();
      this.m_currentTasksExecutingCount = new Lazy<VssPerformanceCounter>((Func<VssPerformanceCounter>) (() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_CurrentTasksExecutingCount", this.Name)));
      this.m_currentTasksQueuedCount = new Lazy<VssPerformanceCounter>((Func<VssPerformanceCounter>) (() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_CurrentTasksQueuedCount", this.Name)));
      this.m_currentTasksScheduledCount = new Lazy<VssPerformanceCounter>((Func<VssPerformanceCounter>) (() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_CurrentTasksScheduledCount", this.Name)));
      this.m_tasksExecutedPerSec = new Lazy<VssPerformanceCounter>((Func<VssPerformanceCounter>) (() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_TasksExecutedPerSec", this.Name)));
      this.m_tasksQueuedPerSec = new Lazy<VssPerformanceCounter>((Func<VssPerformanceCounter>) (() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_TasksQueuedPerSec", this.Name)));
      this.m_tasksScheduledPerSec = new Lazy<VssPerformanceCounter>((Func<VssPerformanceCounter>) (() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_TasksScheduledPerSec", this.Name)));
      this.m_averageTaskExecutionTime = new Lazy<VssPerformanceCounter>((Func<VssPerformanceCounter>) (() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_AverageTaskExecutionTime", this.Name)));
      this.m_averageTaskExecutionTimeBase = new Lazy<VssPerformanceCounter>((Func<VssPerformanceCounter>) (() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_AverageTaskExecutionTimeBase", this.Name)));
      this.m_averageTaskQueueTime = new Lazy<VssPerformanceCounter>((Func<VssPerformanceCounter>) (() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_AverageTaskQueueTime", this.Name)));
      this.m_averageTaskQueueTimeBase = new Lazy<VssPerformanceCounter>((Func<VssPerformanceCounter>) (() => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_AverageTaskQueueTimeBase", this.Name)));
    }

    public string Name => this.m_name;

    public int MaxThreadCount => this.m_maxThreadCount;

    public int MaxConcurrencyPerThread => this.m_maxTaskConcurrencyPerScheduler;

    public Task RunAsync(
      IVssRequestContext requestContext,
      string taskName,
      Func<IVssRequestContext, Task> function,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (!this.m_started)
        throw new InvalidOperationException();
      return this.ScheduleAsync(requestContext, taskName, function, TimeSpan.Zero, cancellationToken);
    }

    public async Task ScheduleAsync(
      IVssRequestContext requestContext,
      string taskName,
      Func<IVssRequestContext, Task> function,
      TimeSpan delay,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      VssTaskDispatcher vssTaskDispatcher = this;
      if (!vssTaskDispatcher.m_started)
        throw new InvalidOperationException();
      VssTaskDispatcher.TaskQueueEntry queueEntry = new VssTaskDispatcher.TaskQueueEntry(requestContext.ServiceHost.InstanceId, requestContext.IsUserContext ? RequestContextType.UserContext : RequestContextType.SystemContext, requestContext.RequestContextInternal().Actors, requestContext.AuthenticatedUserName, requestContext.DomainUserName, requestContext.E2EId, requestContext.OrchestrationId, taskName, function, cancellationToken);
      if (delay > TimeSpan.Zero)
      {
        try
        {
          vssTaskDispatcher.m_tasksScheduledPerSec.Value.Increment();
          vssTaskDispatcher.m_currentTasksScheduledCount.Value.Increment();
          await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
          vssTaskDispatcher.m_currentTasksScheduledCount.Value.Decrement();
        }
      }
      vssTaskDispatcher.m_currentTasksQueuedCount.Value.Increment();
      vssTaskDispatcher.m_tasksQueuedPerSec.Value.Increment();
      // ISSUE: reference to a compiler-generated method
      vssTaskDispatcher.m_runnableTasks.Enqueue(queueEntry, new ItemDequeuedCallback(vssTaskDispatcher.\u003CScheduleAsync\u003Eb__8_0));
      await queueEntry.Task.ConfigureAwait(false);
      queueEntry = (VssTaskDispatcher.TaskQueueEntry) null;
    }

    public void Start()
    {
      if (this.m_started)
        return;
      this.m_started = true;
      int num = this.m_maxThreadCount * this.m_maxTaskConcurrencyPerScheduler;
      for (int index = 0; index < num; ++index)
        this.m_taskRunners.Add(this.DispatchAsync(this.m_schedulers[index % this.m_schedulers.Count]));
    }

    public void Stop(TimeSpan drainTimeout)
    {
      if (!this.m_started)
        return;
      this.m_started = false;
      try
      {
        this.m_runnableTasks.Shutdown();
        if (Task.WhenAll(this.m_taskRunners.ToArray()).Wait(Microsoft.TeamFoundation.Framework.Common.TimeoutHelper.Trim(drainTimeout)))
          return;
        this.m_runnableTasks.Close();
        this.m_cancellationTokenSource.Cancel();
        if (Task.WhenAll(this.m_taskRunners.ToArray()).Wait(VssTaskDispatcher.s_closeWaitTimeout))
          return;
        TeamFoundationTracingService.TraceRawAlwaysOn(27164, TraceLevel.Error, "VssTaskService", "TaskDispatcher", "The dispatcher {0} did not shutdown within the allotted drain timeout of {1}", (object) this.Name, (object) drainTimeout);
      }
      finally
      {
        this.m_cancellationTokenSource.Dispose();
      }
    }

    private async Task DispatchAsync(IVssScheduler scheduler)
    {
      VssTaskDispatcher vssTaskDispatcher1 = this;
      while (!vssTaskDispatcher1.m_cancellationTokenSource.IsCancellationRequested)
      {
        VssTaskDispatcher vssTaskDispatcher = vssTaskDispatcher1;
        VssTaskDispatcher.TaskQueueEntry queueEntry = (VssTaskDispatcher.TaskQueueEntry) null;
        try
        {
          queueEntry = await vssTaskDispatcher1.m_runnableTasks.DequeueAsync(vssTaskDispatcher1.m_cancellationTokenSource.Token).ConfigureAwait(false);
          if (queueEntry == null)
            break;
          try
          {
            queueEntry.QueueDuration.Stop();
            if (queueEntry.CancellationToken.IsCancellationRequested)
            {
              vssTaskDispatcher1.m_averageTaskQueueTime.Value.IncrementTicks(queueEntry.QueueDuration);
              TeamFoundationTracingService.TraceRawConditionally(27167, TraceLevel.Info, "VssTaskService", "TaskDispatcher", (Func<string>) (() => string.Format("Task {0} with task id {1} for host {2} wasn't run because cancellation was requested. Queue duration: {3} ms", (object) queueEntry.TaskName, (object) queueEntry.Task?.Id, (object) queueEntry.HostId, (object) queueEntry.QueueDuration?.ElapsedMilliseconds)));
              queueEntry.SetCanceled();
            }
            else
              await new VssRequestSynchronizationContext(scheduler).RunAsync((Func<Task>) (() => vssTaskDispatcher.ExecuteAsync(queueEntry))).ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            queueEntry.SetException(ex);
            TeamFoundationTracingService.TraceExceptionRaw(27163, TraceLevel.Error, "VssTaskService", "TaskDispatcher", ex);
          }
        }
        catch (System.OperationCanceledException ex)
        {
          break;
        }
        finally
        {
          if (queueEntry != null)
            queueEntry.Dispose();
        }
      }
    }

    private async Task ExecuteAsync(VssTaskDispatcher.TaskQueueEntry queueEntry)
    {
      IVssRequestContext deploymentContext = (IVssRequestContext) null;
      IVssRequestContext hostContext = (IVssRequestContext) null;
      try
      {
        deploymentContext = this.m_serviceHost.CreateSystemContext(false);
        hostContext = deploymentContext.GetService<IInternalTeamFoundationHostManagementService>().BeginRequest(deploymentContext, queueEntry.HostId, queueEntry.ContextType, false, false, queueEntry.Actors, HostRequestType.Default);
        if (hostContext != null)
        {
          IRequestContextInternal requestContextInternal = hostContext.RequestContextInternal();
          requestContextInternal.SetAuthenticatedUserName(queueEntry.AuthenticatedUserName);
          requestContextInternal.SetDomainUserName(queueEntry.DomainUserName);
          this.SetupThreadCulture(hostContext);
          hostContext.IsTracked = true;
          requestContextInternal.SetE2EId(queueEntry.E2EId);
          requestContextInternal.SetOrchestrationId(queueEntry.OrchestrationId);
          Stopwatch watch = new Stopwatch();
          hostContext.Trace(27156, TraceLevel.Info, "VssTaskService", "TaskDispatcher", "Invoking callback {0}", (object) queueEntry.TaskName);
          VssPerformanceCounter performanceCounter1 = this.m_currentTasksExecutingCount.Value;
          performanceCounter1.Increment();
          performanceCounter1 = this.m_averageTaskQueueTime.Value;
          performanceCounter1.IncrementTicks(queueEntry.QueueDuration);
          performanceCounter1 = this.m_averageTaskQueueTimeBase.Value;
          performanceCounter1.Increment();
          watch.Start();
          VssPerformanceEventSource.Log.TaskCallbackStart(hostContext.ServiceHost.InstanceId, queueEntry.TaskName);
          using (CancellationTokenSource tokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.m_cancellationTokenSource.Token, queueEntry.CancellationToken))
          {
            CancellationTokenRegistration registration = tokenSource.Token.Register((Action) (() => hostContext.Cancel(FrameworkResources.RequestCanceledError())), false);
            try
            {
              await queueEntry.RunAsync(hostContext);
            }
            finally
            {
              registration.Dispose();
            }
            registration = new CancellationTokenRegistration();
          }
          watch.Stop();
          VssPerformanceEventSource.Log.TaskCallbackStop(hostContext.ServiceHost.InstanceId, queueEntry.TaskName, watch.ElapsedMilliseconds);
          this.m_tasksExecutedPerSec.Value.Increment();
          VssPerformanceCounter performanceCounter2 = this.m_currentTasksExecutingCount.Value;
          performanceCounter2.Decrement();
          performanceCounter2 = this.m_averageTaskExecutionTime.Value;
          performanceCounter2.IncrementTicks(watch);
          performanceCounter2 = this.m_averageTaskExecutionTimeBase.Value;
          performanceCounter2.Increment();
          hostContext.Trace(27157, TraceLevel.Info, "VssTaskService", "TaskDispatcher", "{0} returned in {1} ms", (object) queueEntry.TaskName, (object) watch.ElapsedMilliseconds);
          if (watch.ElapsedMilliseconds > 5000L)
          {
            TraceLevel level = watch.ElapsedMilliseconds > 10000L ? TraceLevel.Error : TraceLevel.Warning;
            hostContext.TraceAlways(27158, level, "VssTaskService", "TaskDispatcher", "{0} took over {1} ms to complete which is over 5 seconds.", (object) queueEntry.TaskName, (object) watch.ElapsedMilliseconds);
          }
          watch = (Stopwatch) null;
          deploymentContext = (IVssRequestContext) null;
        }
        else
        {
          TeamFoundationTracingService.TraceRawConditionally(27169, TraceLevel.Info, "VssTaskService", "TaskDispatcher", (Func<string>) (() => string.Format("Task {0} with task id {1} for host {2} wasn't run because host was dormant. Queue duration: {3} ms", (object) queueEntry.TaskName, (object) queueEntry.Task?.Id, (object) queueEntry.HostId, (object) queueEntry.QueueDuration?.ElapsedMilliseconds)));
          deploymentContext = (IVssRequestContext) null;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(27163, TraceLevel.Error, "VssTaskService", "TaskDispatcher", ex);
        deploymentContext = (IVssRequestContext) null;
      }
      finally
      {
        hostContext?.Dispose();
        deploymentContext?.Dispose();
      }
    }

    private void SetupThreadCulture(IVssRequestContext requestContext)
    {
      CultureInfo culture = requestContext.ServiceHost.GetCulture(requestContext);
      if (culture == null)
        return;
      if (culture != Thread.CurrentThread.CurrentCulture)
        Thread.CurrentThread.CurrentCulture = culture;
      if (culture == Thread.CurrentThread.CurrentUICulture)
        return;
      Thread.CurrentThread.CurrentUICulture = culture;
    }

    private class TaskQueueEntry : IDisposable
    {
      private readonly CancellationToken m_cancellationToken;
      private readonly CancellationTokenRegistration m_cancellationRegistration;
      private readonly Func<IVssRequestContext, Task> m_callback;
      private readonly TaskCompletionSource<object> m_completionSource;

      public TaskQueueEntry(
        Guid hostId,
        RequestContextType contextType,
        IReadOnlyList<IRequestActor> actors,
        string authenticatedUserName,
        string domainUserName,
        Guid e2eId,
        string orchestrationId,
        string taskName,
        Func<IVssRequestContext, Task> callback,
        CancellationToken cancellationToken)
        : this(hostId, contextType, actors, authenticatedUserName, domainUserName, e2eId, orchestrationId, taskName, cancellationToken)
      {
        this.m_callback = callback;
        this.m_completionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        this.m_cancellationRegistration = this.m_cancellationToken.Register(new Action(this.SetCanceled));
      }

      protected TaskQueueEntry(
        Guid hostId,
        RequestContextType contextType,
        IReadOnlyList<IRequestActor> actors,
        string authenticatedUserName,
        string domainUserName,
        Guid e2eId,
        string orchestrationId,
        string taskName,
        CancellationToken cancellationToken)
      {
        this.HostId = hostId;
        this.ContextType = contextType;
        this.Actors = actors;
        this.AuthenticatedUserName = authenticatedUserName;
        this.DomainUserName = domainUserName;
        this.E2EId = e2eId;
        this.OrchestrationId = orchestrationId;
        this.TaskName = taskName;
        this.QueueDuration = Stopwatch.StartNew();
        this.m_cancellationToken = cancellationToken;
      }

      public CancellationToken CancellationToken => this.m_cancellationToken;

      public Guid HostId { get; }

      public RequestContextType ContextType { get; }

      public IReadOnlyList<IRequestActor> Actors { get; }

      public string AuthenticatedUserName { get; }

      public string DomainUserName { get; }

      public Guid E2EId { get; }

      public string OrchestrationId { get; }

      public string TaskName { get; }

      public Stopwatch QueueDuration { get; }

      public Task Task => (Task) this.m_completionSource.Task;

      public void Dispose() => this.Dispose(true);

      protected virtual void Dispose(bool disposing)
      {
        if (!disposing)
          return;
        this.SetCanceled();
        CancellationTokenRegistration cancellationRegistration = this.m_cancellationRegistration;
        this.m_cancellationRegistration.Dispose();
      }

      public async Task RunAsync(IVssRequestContext requestContext)
      {
        try
        {
          requestContext.ServiceHost.BeginRequest(requestContext);
          requestContext.EnterMethod(new MethodInformation(this.TaskName, MethodType.SystemTask, EstimatedMethodCost.Free));
          requestContext.IsTracked = true;
          await this.InvokeAsync(requestContext);
          this.SetResult();
        }
        catch (System.OperationCanceledException ex)
        {
          this.SetCanceled();
        }
        catch (RequestCanceledException ex)
        {
          this.SetCanceled();
        }
        catch (Exception ex)
        {
          requestContext.Trace(27159, TraceLevel.Error, "VssTaskService", "TaskDispatcher", "Error during {0}{1}Exception: {2}", (object) this.TaskName, (object) Environment.NewLine, (object) ex);
          this.SetException(ex);
        }
        finally
        {
          requestContext.LeaveMethod();
          requestContext.ServiceHost.EndRequest(requestContext);
        }
      }

      protected virtual Task InvokeAsync(IVssRequestContext requestContext) => this.m_callback(requestContext);

      public virtual void SetCanceled() => this.m_completionSource.TrySetCanceled();

      public virtual void SetException(Exception exception) => this.m_completionSource.TrySetException(exception);

      public virtual void SetResult() => this.m_completionSource.TrySetResult((object) null);
    }
  }
}
