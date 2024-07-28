// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.OrchestrationContext
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using ImpromptuInterface;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public abstract class OrchestrationContext
  {
    public OrchestrationInstance OrchestrationInstance => this.OrchestrationRuntimeState?.OrchestrationInstance;

    public OrchestrationRuntimeState OrchestrationRuntimeState { get; internal set; }

    public abstract TaskScheduler Scheduler { get; }

    public virtual DateTime CurrentUtcDateTime { get; internal set; }

    public virtual IOrchestrationTracer Tracer { get; internal set; }

    public bool IsReplaying { get; internal set; }

    public IActivityShardLocator ActivityShardLocator { get; internal set; }

    public virtual T CreateClient<T>() where T : class => this.CreateClient<T>(true);

    public virtual T CreateClient<T>(bool useFullyQualifiedMethodNames) where T : class => this.CreateClient<T>(useFullyQualifiedMethodNames, (string) null);

    public virtual T CreateClient<T>(bool useFullyQualifiedMethodNames, string dispatcherType = null) where T : class
    {
      if (!typeof (T).IsInterface)
        throw new InvalidOperationException("Pass in an interface.");
      return new ScheduleProxy(this, typeof (T), useFullyQualifiedMethodNames, dispatcherType).ActLike<T>();
    }

    public virtual T CreateShardedClient<T>(
      bool useFullyQualifiedMethodNames,
      int activityDispatcherShardsCount,
      string dispatcherType = null)
      where T : class
    {
      string shardValue;
      if ((this.ActivityShardLocator ?? (IActivityShardLocator) new ActivityShardLocatorRandom(activityDispatcherShardsCount)).TryGetShardValue((IActivityShardKey) null, out shardValue, dispatcherType))
        dispatcherType = shardValue;
      return this.CreateClient<T>(useFullyQualifiedMethodNames, dispatcherType);
    }

    public virtual T CreateShardedClient<T>(
      bool useFullyQualifiedMethodNames,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey,
      string dispatcherType = null)
      where T : class
    {
      string shardValue;
      if ((this.ActivityShardLocator ?? (IActivityShardLocator) new ActivityShardLocatorRandom(activityDispatcherShardsCount)).TryGetShardValue(shardKey, out shardValue, dispatcherType))
        dispatcherType = shardValue;
      return this.CreateClient<T>(useFullyQualifiedMethodNames, dispatcherType);
    }

    public virtual T CreateRetryableClient<T>(RetryOptions retryOptions) where T : class
    {
      if (!typeof (T).IsInterface)
        throw new InvalidOperationException("Pass in an interface.");
      ScheduleProxy originalDynamic = new ScheduleProxy(this, typeof (T));
      return new RetryProxy<T>(this, retryOptions, originalDynamic.ActLike<T>()).ActLike<T>();
    }

    public virtual Task<T> ScheduleWithRetry<T>(
      Type taskActivityType,
      RetryOptions retryOptions,
      params object[] parameters)
    {
      return this.ScheduleWithRetry<T>(NameVersionHelper.GetDefaultName((object) taskActivityType), NameVersionHelper.GetDefaultVersion((object) taskActivityType), retryOptions, parameters);
    }

    public virtual Task<T> ScheduleWithRetry<T>(
      string name,
      string version,
      RetryOptions retryOptions,
      params object[] parameters)
    {
      Func<Task<T>> retryCall = (Func<Task<T>>) (() => this.ScheduleTask<T>(name, version, parameters));
      return new RetryInterceptor<T>(this, retryOptions, retryCall).Invoke();
    }

    public virtual Task<T> CreateSubOrchestrationInstanceWithRetry<T>(
      Type orchestrationType,
      RetryOptions retryOptions,
      object input)
    {
      return this.CreateSubOrchestrationInstanceWithRetry<T>(NameVersionHelper.GetDefaultName((object) orchestrationType), NameVersionHelper.GetDefaultVersion((object) orchestrationType), retryOptions, input);
    }

    public virtual Task<T> CreateSubOrchestrationInstanceWithRetry<T>(
      Type orchestrationType,
      string instanceId,
      RetryOptions retryOptions,
      object input)
    {
      return this.CreateSubOrchestrationInstanceWithRetry<T>(NameVersionHelper.GetDefaultName((object) orchestrationType), NameVersionHelper.GetDefaultVersion((object) orchestrationType), instanceId, retryOptions, input);
    }

    public virtual Task<T> CreateSubOrchestrationInstanceWithRetry<T>(
      string name,
      string version,
      RetryOptions retryOptions,
      object input)
    {
      Func<Task<T>> retryCall = (Func<Task<T>>) (() => this.CreateSubOrchestrationInstance<T>(name, version, input));
      return new RetryInterceptor<T>(this, retryOptions, retryCall).Invoke();
    }

    public virtual Task<T> CreateSubOrchestrationInstanceWithRetry<T>(
      string name,
      string version,
      string instanceId,
      RetryOptions retryOptions,
      object input)
    {
      Func<Task<T>> retryCall = (Func<Task<T>>) (() => this.CreateSubOrchestrationInstance<T>(name, version, instanceId, input));
      return new RetryInterceptor<T>(this, retryOptions, retryCall).Invoke();
    }

    public virtual Task<TResult> ScheduleTask<TResult>(
      Type activityType,
      params object[] parameters)
    {
      return this.ScheduleTask<TResult>(NameVersionHelper.GetDefaultName((object) activityType), NameVersionHelper.GetDefaultVersion((object) activityType), parameters);
    }

    public abstract Task<TResult> ScheduleTask<TResult>(
      string name,
      string version,
      params object[] parameters);

    public abstract Task<TResult> ScheduleTask<TResult>(
      string name,
      string version,
      string dispatcherType,
      params object[] parameters);

    public abstract Task<T> CreateTimer<T>(DateTime fireAt, T state);

    public abstract Task<T> CreateTimer<T>(DateTime fireAt, T state, CancellationToken cancelToken);

    public virtual Task<T> CreateSubOrchestrationInstance<T>(Type orchestrationType, object input) => this.CreateSubOrchestrationInstance<T>(NameVersionHelper.GetDefaultName((object) orchestrationType), NameVersionHelper.GetDefaultVersion((object) orchestrationType), input);

    public virtual Task<T> CreateSubOrchestrationInstance<T>(
      Type orchestrationType,
      string instanceId,
      object input)
    {
      return this.CreateSubOrchestrationInstance<T>(NameVersionHelper.GetDefaultName((object) orchestrationType), NameVersionHelper.GetDefaultVersion((object) orchestrationType), instanceId, input);
    }

    public abstract Task<T> CreateSubOrchestrationInstance<T>(
      string name,
      string version,
      object input);

    public abstract Task<T> CreateSubOrchestrationInstance<T>(
      string name,
      string version,
      string instanceId,
      object input);

    public abstract void ContinueAsNew(object input);

    public abstract void ContinueAsNew(string newVersion, object input);

    public void Trace(int eventId, TraceLevel level, string format, params object[] arguments)
    {
      if (this.Tracer == null || this.IsReplaying)
        return;
      this.Tracer.Trace(this.OrchestrationInstance.InstanceId, eventId, level, format, arguments);
    }

    public void Trace(
      string area,
      string layer,
      int eventId,
      TraceLevel level,
      string format,
      params object[] arguments)
    {
      if (this.Tracer == null || this.IsReplaying)
        return;
      this.Tracer.Trace(this.OrchestrationInstance.InstanceId, eventId, level, area, layer, format, arguments);
    }
  }
}
