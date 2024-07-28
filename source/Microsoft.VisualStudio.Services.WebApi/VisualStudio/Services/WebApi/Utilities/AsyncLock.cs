// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Utilities.AsyncLock
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi.Utilities
{
  internal sealed class AsyncLock
  {
    private readonly SemaphoreSlim m_semaphore = new SemaphoreSlim(1, 1);
    private readonly Task<IDisposable> m_releaser;

    public AsyncLock() => this.m_releaser = Task.FromResult<IDisposable>((IDisposable) new AsyncLock.Releaser(this));

    public Task<IDisposable> LockAsync(CancellationToken cancellationToken = default (CancellationToken))
    {
      Task task1 = this.m_semaphore.WaitAsync();
      return task1.IsCompleted ? this.m_releaser : task1.ContinueWith<IDisposable>((Func<Task, object, IDisposable>) ((task, state) => (IDisposable) state), (object) this.m_releaser.Result, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    private sealed class Releaser : IDisposable
    {
      private readonly AsyncLock m_toRelease;

      internal Releaser(AsyncLock toRelease) => this.m_toRelease = toRelease;

      public void Dispose() => this.m_toRelease.m_semaphore.Release();
    }
  }
}
