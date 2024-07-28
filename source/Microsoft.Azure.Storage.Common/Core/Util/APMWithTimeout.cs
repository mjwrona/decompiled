// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.APMWithTimeout
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Threading;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal sealed class APMWithTimeout : IDisposable
  {
    private TimerCallback timeoutCallback;
    private RegisteredWaitHandle waitHandle;
    private IAsyncResult asyncResult;
    private bool disposed;

    public static void RunWithTimeout(
      Func<AsyncCallback, object, IAsyncResult> beginMethod,
      AsyncCallback callback,
      TimerCallback timeoutCallback,
      object state,
      TimeSpan timeout)
    {
      CommonUtility.AssertNotNull(nameof (beginMethod), (object) beginMethod);
      CommonUtility.AssertNotNull(nameof (callback), (object) callback);
      CommonUtility.AssertNotNull(nameof (timeoutCallback), (object) timeoutCallback);
      new APMWithTimeout(timeoutCallback).Begin(beginMethod, callback, state, timeout);
    }

    private APMWithTimeout(TimerCallback timeoutCallback) => this.timeoutCallback = timeoutCallback;

    private void Begin(
      Func<AsyncCallback, object, IAsyncResult> beginMethod,
      AsyncCallback callback,
      object state,
      TimeSpan timeout)
    {
      this.asyncResult = beginMethod(callback, state);
      this.waitHandle = ThreadPool.RegisterWaitForSingleObject(this.asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(this.WaitCallback), state, timeout, true);
      if (!this.disposed)
        return;
      this.UnregisterWaitHandle();
    }

    private void WaitCallback(object state, bool timedOut)
    {
      try
      {
        if (!timedOut || this.asyncResult.IsCompleted)
          return;
        TimerCallback timeoutCallback = this.timeoutCallback;
        this.timeoutCallback = (TimerCallback) null;
        if (timeoutCallback == null)
          return;
        timeoutCallback(state);
      }
      finally
      {
        this.Dispose();
      }
    }

    private void UnregisterWaitHandle() => Interlocked.Exchange<RegisteredWaitHandle>(ref this.waitHandle, (RegisteredWaitHandle) null)?.Unregister((WaitHandle) null);

    public void Dispose()
    {
      if (this.disposed)
        return;
      this.disposed = true;
      this.UnregisterWaitHandle();
    }
  }
}
