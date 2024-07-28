// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.CancellableAsyncResultTaskWrapper
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class CancellableAsyncResultTaskWrapper : ICancellableAsyncResult, IAsyncResult
  {
    internal IAsyncResult internalAsyncResult;
    protected CancellationTokenSource cancellationTokenSource;
    protected bool completedSync;

    public static ICancellableAsyncResult Create(
      Func<CancellationToken, Task> generateTask,
      AsyncCallback callback,
      object state)
    {
      return (ICancellableAsyncResult) new CancellableAsyncResultTaskWrapper(generateTask, callback, state);
    }

    public static ICancellableAsyncResult Create<T>(
      Func<CancellationToken, Task<T>> generateTask,
      AsyncCallback callback,
      object state)
    {
      return (ICancellableAsyncResult) new CancellableAsyncResultTaskWrapper<T>(generateTask, callback, state);
    }

    private CancellableAsyncResultTaskWrapper(
      Func<CancellationToken, Task> generateTask,
      AsyncCallback callback,
      object state)
      : this()
    {
      CancellableAsyncResultTaskWrapper ar1 = this;
      AsyncCallback callback1 = (AsyncCallback) (ar =>
      {
        ar1.internalAsyncResult = ar;
        if (callback == null)
          return;
        callback((IAsyncResult) ar1);
      });
      Task task = generateTask(this.cancellationTokenSource.Token);
      if (task.IsCompleted)
        this.completedSync = true;
      this.internalAsyncResult = task.AsApm(callback1, state);
    }

    protected CancellableAsyncResultTaskWrapper() => this.cancellationTokenSource = new CancellationTokenSource();

    public object AsyncState => this.internalAsyncResult.AsyncState;

    public WaitHandle AsyncWaitHandle => this.internalAsyncResult.AsyncWaitHandle;

    public bool CompletedSynchronously => this.completedSync;

    public bool IsCompleted => this.internalAsyncResult.IsCompleted;

    public void Cancel() => this.cancellationTokenSource.Cancel();

    internal void Wait() => CommonUtility.RunWithoutSynchronizationContext((Action) (() => ((Task) this.internalAsyncResult).Wait()));

    internal TaskAwaiter GetAwaiter() => ((Task) this.internalAsyncResult).GetAwaiter();
  }
}
