// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.VssSynchronousScheduler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  internal sealed class VssSynchronousScheduler : VssScheduler
  {
    private bool m_inCallback;
    private readonly object m_thisLock = new object();
    private readonly BlockingCollection<VssSynchronousScheduler.ScheduledCallback> m_callbackQueue = new BlockingCollection<VssSynchronousScheduler.ScheduledCallback>();

    public override void Run(SendOrPostCallback callback, object state)
    {
      if (VssScheduler.Current == this)
      {
        callback(state);
      }
      else
      {
        try
        {
          TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
          this.m_callbackQueue.Add(new VssSynchronousScheduler.ScheduledCallback(callback, state, completionSource));
          completionSource.Task.Wait();
        }
        catch (InvalidOperationException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(8675311, TraceLevel.Error, "VssRequestContext", nameof (VssSynchronousScheduler), (Exception) ex);
        }
      }
    }

    public override void RunAsync(SendOrPostCallback callback, object state)
    {
      try
      {
        this.m_callbackQueue.Add(new VssSynchronousScheduler.ScheduledCallback(callback, state));
      }
      catch (InvalidOperationException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(8675311, TraceLevel.Error, "VssRequestContext", nameof (VssSynchronousScheduler), (Exception) ex);
      }
    }

    public void RunSynchronously(Func<Task> function)
    {
      Task completionTask = function();
      this.RunCallbackLoop(completionTask);
      completionTask.GetAwaiter().GetResult();
    }

    public T RunSynchronously<T>(Func<Task<T>> function)
    {
      Task<T> completionTask = function();
      this.RunCallbackLoop((Task) completionTask);
      return completionTask.GetAwaiter().GetResult();
    }

    protected override bool IsWaitSafe() => !this.m_inCallback;

    private void RunCallbackLoop(Task completionTask)
    {
      if (completionTask.IsCompleted)
        return;
      completionTask.ContinueWith((Action<Task>) (t => this.m_callbackQueue.CompleteAdding()), TaskContinuationOptions.ExecuteSynchronously);
      foreach (VssSynchronousScheduler.ScheduledCallback consuming in this.m_callbackQueue.GetConsumingEnumerable())
      {
        try
        {
          VssScheduler.Current = (VssScheduler) this;
          this.m_inCallback = true;
          consuming.Callback(consuming.State);
        }
        finally
        {
          VssScheduler.Current = (VssScheduler) null;
          this.m_inCallback = false;
          consuming.CompletionSource?.TrySetResult((object) null);
        }
      }
    }

    private sealed class ScheduledCallback
    {
      public readonly object State;
      public readonly SendOrPostCallback Callback;
      public readonly TaskCompletionSource<object> CompletionSource;

      public ScheduledCallback(
        SendOrPostCallback callback,
        object state,
        TaskCompletionSource<object> completionSource = null)
      {
        this.State = state;
        this.Callback = callback;
        this.CompletionSource = completionSource;
      }
    }
  }
}
