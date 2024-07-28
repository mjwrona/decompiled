// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.VssRequestSynchronizationContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VssRequestSynchronizationContext : SynchronizationContext
  {
    private CultureInfo m_culture;
    private CultureInfo m_uiCulture;
    private CultureInfo m_originalCulture;
    private CultureInfo m_originalUICulture;
    private int m_activeThreadId;
    private readonly Guid m_activityId;
    private readonly long m_requestId;
    private readonly IVssScheduler m_scheduler;
    private const string c_area = "VssRequestContext";
    private const string c_layer = "VssRequestSynchronizationContext";

    public VssRequestSynchronizationContext()
      : this(LockHelperContext.NewRequestId())
    {
    }

    public VssRequestSynchronizationContext(Guid activityId)
      : this(LockHelperContext.NewRequestId(), activityId, (IVssScheduler) new VssThreadPoolScheduler())
    {
    }

    internal VssRequestSynchronizationContext(long requestId)
      : this(requestId, Guid.Empty)
    {
    }

    internal VssRequestSynchronizationContext(IVssScheduler scheduler)
      : this(scheduler, Guid.Empty)
    {
    }

    internal VssRequestSynchronizationContext(IVssScheduler scheduler, Guid activityId)
      : this(LockHelperContext.NewRequestId(), activityId, scheduler)
    {
    }

    internal VssRequestSynchronizationContext(long requestId, Guid activityId)
      : this(requestId, activityId, (IVssScheduler) new VssThreadPoolScheduler())
    {
    }

    internal VssRequestSynchronizationContext(
      long requestId,
      Guid activityId,
      IVssScheduler scheduler,
      bool detectPotentialDeadlocks = false)
    {
      ArgumentUtility.CheckForNull<IVssScheduler>(scheduler, nameof (scheduler));
      this.m_requestId = requestId;
      this.m_scheduler = scheduler;
      this.m_activityId = activityId == Guid.Empty ? EventProvider.CreateActivityId() : activityId;
      if (!detectPotentialDeadlocks)
        return;
      this.SetWaitNotificationRequired();
    }

    public long RequestId => this.m_requestId;

    public CultureInfo Culture => this.m_culture;

    public CultureInfo UICulture => this.m_uiCulture;

    public IVssScheduler Scheduler => this.m_scheduler;

    public override SynchronizationContext CreateCopy() => (SynchronizationContext) new VssRequestSynchronizationContext(this.m_requestId, this.m_activityId, this.m_scheduler);

    public Task RunAsync(Func<Task> function)
    {
      TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
      SendOrPostCallback postCallback = (SendOrPostCallback) (state =>
      {
        try
        {
          function().ContinueWith((Action<Task>) (x =>
          {
            if (x.Status == TaskStatus.Faulted)
              taskCompletionSource.SetException((Exception) x.Exception);
            else if (x.Status == TaskStatus.Canceled)
              taskCompletionSource.SetCanceled();
            else
              taskCompletionSource.SetResult((object) null);
          }));
        }
        catch (Exception ex)
        {
          taskCompletionSource.SetException(ex);
        }
      });
      this.m_scheduler.RunAsync((SendOrPostCallback) (s => this.WrapCallback(postCallback, s)), (object) null);
      return (Task) taskCompletionSource.Task;
    }

    public Task<TResult> RunAsync<TResult>(Func<Task<TResult>> function)
    {
      TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
      SendOrPostCallback postCallback = (SendOrPostCallback) (state =>
      {
        try
        {
          function().ContinueWith((Action<Task<TResult>>) (x =>
          {
            if (x.Status == TaskStatus.Faulted)
              taskCompletionSource.SetException((Exception) x.Exception);
            else if (x.Status == TaskStatus.Canceled)
              taskCompletionSource.SetCanceled();
            else
              taskCompletionSource.SetResult(x.Result);
          }));
        }
        catch (Exception ex)
        {
          taskCompletionSource.SetException(ex);
        }
      });
      this.m_scheduler.RunAsync((SendOrPostCallback) (s => this.WrapCallback(postCallback, s)), (object) null);
      return taskCompletionSource.Task;
    }

    public override void Post(SendOrPostCallback d, object state)
    {
      if (this.m_scheduler.IsBlockedInWait)
        this.TracePotentialDeadlock(nameof (Post));
      this.m_scheduler.RunAsync((SendOrPostCallback) (s => this.WrapCallback(d, s)), state);
    }

    public override void Send(SendOrPostCallback d, object state)
    {
      if (this.m_scheduler.IsBlockedInWait)
        this.TracePotentialDeadlock(nameof (Send));
      this.m_scheduler.Run((SendOrPostCallback) (s => this.WrapCallback(d, s)), state);
    }

    public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
    {
      bool flag = false;
      try
      {
        flag = this.m_scheduler.EnterWait();
        return base.Wait(waitHandles, waitAll, millisecondsTimeout);
      }
      finally
      {
        if (flag)
          this.m_scheduler.LeaveWait();
      }
    }

    private void WrapCallback(SendOrPostCallback callback, object state)
    {
      SynchronizationContext current = SynchronizationContext.Current;
      try
      {
        this.m_activeThreadId = Environment.CurrentManagedThreadId;
        this.SetThreadCulture();
        SynchronizationContext.SetSynchronizationContext((SynchronizationContext) this);
        using (new VssActivityScope(this.m_activityId))
        {
          using (new VssRequestLockScope(this.m_requestId))
            callback(state);
        }
      }
      finally
      {
        this.m_activeThreadId = 0;
        this.RestoreAndUpdateThreadCulture();
        SynchronizationContext.SetSynchronizationContext(current);
      }
    }

    private void RestoreAndUpdateThreadCulture()
    {
      CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
      CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
      if (this.m_originalCulture != null)
      {
        if (this.m_originalCulture != currentCulture)
        {
          this.m_culture = currentCulture;
          Thread.CurrentThread.CurrentCulture = this.m_originalCulture;
        }
        this.m_originalCulture = (CultureInfo) null;
      }
      if (this.m_originalUICulture == null)
        return;
      if (this.m_originalUICulture != currentUiCulture)
      {
        this.m_uiCulture = currentUiCulture;
        Thread.CurrentThread.CurrentUICulture = this.m_originalUICulture;
      }
      this.m_originalUICulture = (CultureInfo) null;
    }

    private void SetThreadCulture()
    {
      this.m_originalCulture = Thread.CurrentThread.CurrentCulture;
      this.m_originalUICulture = Thread.CurrentThread.CurrentUICulture;
      if (this.m_culture != null && this.m_culture != this.m_originalCulture)
        Thread.CurrentThread.CurrentCulture = this.m_culture;
      if (this.m_uiCulture == null || this.m_uiCulture == this.m_originalUICulture)
        return;
      Thread.CurrentThread.CurrentUICulture = this.m_uiCulture;
    }

    private void TracePotentialDeadlock(string methodName)
    {
      using (new VssActivityScope(this.m_activityId))
      {
        TeamFoundationTracingService.TraceRaw(8675309, TraceLevel.Error, "VssRequestContext", nameof (VssRequestSynchronizationContext), "Potential deadlock detected invoking SynchronizationContext.{0} on request {1}", (object) methodName, (object) this.m_requestId);
        if (!TeamFoundationTracingService.IsRawTracingEnabled(8675310, TraceLevel.Info, "VssRequestContext", nameof (VssRequestSynchronizationContext), (string[]) null))
          return;
        TeamFoundationTracingService.TraceRaw(8675310, TraceLevel.Info, "VssRequestContext", nameof (VssRequestSynchronizationContext), "Deadlock victim callstack: {0}", (object) new StackTrace().ToString());
      }
    }

    internal IDisposable ActivateOnCurrentThread() => (IDisposable) new VssRequestSynchronizationContext.SyncContextActivationScope(this);

    private class SyncContextActivationScope : IDisposable
    {
      private bool m_isDisposed;
      private readonly VssRequestLockScope m_lockScope;
      private readonly SynchronizationContext m_previousContext;

      public SyncContextActivationScope(VssRequestSynchronizationContext newContext)
      {
        this.m_isDisposed = false;
        this.m_previousContext = SynchronizationContext.Current;
        this.m_lockScope = new VssRequestLockScope(newContext.RequestId);
        SynchronizationContext.SetSynchronizationContext((SynchronizationContext) newContext);
      }

      public void Dispose()
      {
        if (this.m_isDisposed)
          return;
        this.m_isDisposed = true;
        this.m_lockScope.Dispose();
        SynchronizationContext.SetSynchronizationContext(this.m_previousContext);
      }
    }
  }
}
