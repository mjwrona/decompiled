// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.VssAsyncAutoResetEvent
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  public class VssAsyncAutoResetEvent : IDisposable
  {
    private bool m_signaled;
    private readonly List<VssAsyncAutoResetEvent.Waiter> m_waiters = new List<VssAsyncAutoResetEvent.Waiter>();

    public VssAsyncAutoResetEvent(bool initialState = false)
    {
      this.m_signaled = initialState;
      this.m_waiters = new List<VssAsyncAutoResetEvent.Waiter>();
    }

    void IDisposable.Dispose()
    {
      VssAsyncAutoResetEvent.Waiter[] array;
      lock (this.m_waiters)
      {
        array = new VssAsyncAutoResetEvent.Waiter[this.m_waiters.Count];
        this.m_waiters.CopyTo(array);
        this.m_waiters.Clear();
      }
      if (array.Length == 0)
        return;
      foreach (VssAsyncAutoResetEvent.Waiter waiter in array)
      {
        waiter.Registration.Dispose();
        waiter.TokenSource.Dispose();
        waiter.CompletionSource.TrySetResult(false);
      }
    }

    public void Set()
    {
      VssAsyncAutoResetEvent.Waiter waiter = (VssAsyncAutoResetEvent.Waiter) null;
      lock (this.m_waiters)
      {
        if (this.m_waiters.Count > 0)
        {
          waiter = this.m_waiters[0];
          this.m_waiters.RemoveAt(0);
        }
        else if (!this.m_signaled)
          this.m_signaled = true;
      }
      if (waiter == null)
        return;
      waiter.Registration.Dispose();
      waiter.TokenSource.Dispose();
      waiter.CompletionSource.TrySetResult(true);
    }

    public Task<bool> WaitAsync() => this.WaitAsync(TimeSpan.MaxValue, CancellationToken.None);

    public Task<bool> WaitAsync(TimeSpan timeout) => this.WaitAsync(timeout, CancellationToken.None);

    public Task<bool> WaitAsync(CancellationToken cancellationToken) => this.WaitAsync(TimeSpan.MaxValue, cancellationToken);

    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
        return Task.FromResult<bool>(false);
      lock (this.m_waiters)
      {
        if (this.m_signaled)
        {
          this.m_signaled = false;
          return Task.FromResult<bool>(true);
        }
        if (timeout == TimeSpan.Zero)
          return Task.FromResult<bool>(false);
        VssAsyncAutoResetEvent.Waiter waiter = new VssAsyncAutoResetEvent.Waiter(this, timeout, cancellationToken);
        this.m_waiters.Add(waiter);
        return waiter.CompletionSource.Task;
      }
    }

    private bool RemoveWaiter(VssAsyncAutoResetEvent.Waiter waiter)
    {
      lock (this.m_waiters)
        return this.m_waiters.Remove(waiter);
    }

    private sealed class Waiter
    {
      public readonly CancellationToken CancellationToken;
      public readonly TaskCompletionSource<bool> CompletionSource;
      public readonly VssAsyncAutoResetEvent Owner;
      public readonly CancellationTokenRegistration Registration;
      public readonly CancellationTokenSource TokenSource;

      public Waiter(
        VssAsyncAutoResetEvent owner,
        TimeSpan timeout,
        CancellationToken cancellationToken)
      {
        this.CancellationToken = cancellationToken;
        this.CompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        this.Owner = owner;
        this.TokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (timeout < TimeSpan.MaxValue && timeout.TotalMilliseconds >= 0.0)
          this.TokenSource.CancelAfter(timeout);
        this.Registration = this.TokenSource.Token.Register(new Action(this.OnCanceled));
      }

      private void OnCanceled()
      {
        if (!this.Owner.RemoveWaiter(this))
          return;
        this.Registration.Dispose();
        this.TokenSource.Dispose();
        this.CompletionSource.TrySetResult(false);
      }
    }
  }
}
