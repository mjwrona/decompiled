// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.CancellableAsyncResultTaskWrapper`1
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class CancellableAsyncResultTaskWrapper<TResult> : CancellableAsyncResultTaskWrapper
  {
    public CancellableAsyncResultTaskWrapper(
      Func<CancellationToken, Task<TResult>> generateTask,
      AsyncCallback callback,
      object state)
    {
      CancellableAsyncResultTaskWrapper<TResult> ar1 = this;
      AsyncCallback callback1 = (AsyncCallback) (ar =>
      {
        ar1.internalAsyncResult = ar;
        if (callback == null)
          return;
        callback((IAsyncResult) ar1);
      });
      Task<TResult> task = generateTask(this.cancellationTokenSource.Token);
      if (task.IsCompleted)
        this.completedSync = true;
      this.internalAsyncResult = task.AsApm<TResult>(callback1, state);
    }

    internal TResult Result => CommonUtility.RunWithoutSynchronizationContext<TResult>((Func<TResult>) (() => ((Task<TResult>) this.internalAsyncResult).Result));

    internal TaskAwaiter<TResult> GetAwaiter() => ((Task<TResult>) this.internalAsyncResult).GetAwaiter();
  }
}
