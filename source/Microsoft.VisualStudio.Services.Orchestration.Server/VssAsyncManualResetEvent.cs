// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.VssAsyncManualResetEvent
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  public class VssAsyncManualResetEvent : IDisposable
  {
    private readonly object m_thisLock = new object();
    private bool m_signaled;
    private TaskCompletionSource<bool> m_completionSource;

    public VssAsyncManualResetEvent(bool initialState = false)
    {
      this.m_signaled = initialState;
      this.m_completionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
      if (!initialState)
        return;
      this.m_completionSource.SetResult(true);
    }

    void IDisposable.Dispose() => this.m_completionSource.TrySetResult(false);

    public Task<bool> WaitAsync() => this.WaitAsync(CancellationToken.None);

    public Task<bool> WaitAsync(TimeSpan timeout) => this.WaitAsync(timeout, CancellationToken.None);

    public Task<bool> WaitAsync(CancellationToken cancellationToken) => this.WaitAsync(Timeout.InfiniteTimeSpan, cancellationToken);

    public async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
      Task<bool> task;
      lock (this.m_thisLock)
      {
        if (this.m_signaled)
          return true;
        task = this.m_completionSource.Task;
      }
      if (cancellationToken == CancellationToken.None && (timeout < TimeSpan.Zero || timeout >= TimeSpan.MaxValue))
        return await task.ConfigureAwait(false);
      TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
      using (CancellationTokenSource tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
      {
        if (timeout >= TimeSpan.Zero && timeout < TimeSpan.MaxValue)
          tokenSource.CancelAfter(timeout);
        using (tokenSource.Token.Register((Action) (() => completionSource.TrySetResult(false)), false))
          return await Task.WhenAny<bool>(task, completionSource.Task).Unwrap<bool>().ConfigureAwait(false);
      }
    }

    public void Set()
    {
      TaskCompletionSource<bool> completionSource;
      lock (this.m_thisLock)
      {
        if (this.m_signaled)
          return;
        this.m_signaled = true;
        completionSource = this.m_completionSource;
      }
      completionSource.TrySetResult(true);
    }

    public void Reset()
    {
      lock (this.m_thisLock)
      {
        if (!this.m_signaled)
          return;
        this.m_completionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        this.m_signaled = false;
      }
    }
  }
}
