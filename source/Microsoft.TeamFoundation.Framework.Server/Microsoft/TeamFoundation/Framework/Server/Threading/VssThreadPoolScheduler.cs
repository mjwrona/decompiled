// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.VssThreadPoolScheduler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  internal sealed class VssThreadPoolScheduler : VssScheduler
  {
    private Task m_lastScheduledTask;
    private readonly object m_thisLock;

    public VssThreadPoolScheduler()
    {
      this.m_thisLock = new object();
      this.m_lastScheduledTask = (Task) Task.FromResult<object>((object) null);
    }

    public override void Run(SendOrPostCallback callback, object state)
    {
      if (VssScheduler.Current == this)
      {
        callback(state);
      }
      else
      {
        TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
        Task lastScheduledTask;
        lock (this.m_thisLock)
        {
          lastScheduledTask = this.m_lastScheduledTask;
          this.m_lastScheduledTask = (Task) completionSource.Task;
        }
        if (!lastScheduledTask.IsCompleted)
          lastScheduledTask.ContinueWith((Action<Task>) (t => { }), TaskContinuationOptions.ExecuteSynchronously).Wait();
        try
        {
          this.WrapCallback(callback, state);
        }
        finally
        {
          completionSource.TrySetResult((object) null);
        }
      }
    }

    public override void RunAsync(SendOrPostCallback callback, object state)
    {
      lock (this.m_thisLock)
        this.m_lastScheduledTask = this.m_lastScheduledTask.ContinueWith((Action<Task>) (t => this.WrapCallback(callback, state)), TaskScheduler.Default);
    }

    private void WrapCallback(SendOrPostCallback callback, object state)
    {
      try
      {
        VssScheduler.Current = (VssScheduler) this;
        callback(state);
      }
      finally
      {
        VssScheduler.Current = (VssScheduler) null;
      }
    }
  }
}
