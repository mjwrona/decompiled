// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Notification.AsyncManualResetEvent
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry.Notification
{
  internal class AsyncManualResetEvent
  {
    private volatile TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

    public Task WaitAsync() => (Task) this.tcs.Task;

    public void Set() => Task.Run<bool>((Func<bool>) (() => this.tcs.TrySetResult(true)));

    public void Reset()
    {
      TaskCompletionSource<bool> tcs;
      do
      {
        tcs = this.tcs;
      }
      while (tcs.Task.IsCompleted && Interlocked.CompareExchange<TaskCompletionSource<bool>>(ref this.tcs, new TaskCompletionSource<bool>(), tcs) != tcs);
    }
  }
}
