// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.AsyncManualResetEvent
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  public class AsyncManualResetEvent
  {
    private volatile TaskCompletionSource<bool> m_tcs = new TaskCompletionSource<bool>();

    public AsyncManualResetEvent(bool initialStateSignaled)
    {
      if (!initialStateSignaled)
        return;
      this.m_tcs.SetResult(true);
    }

    public Task WaitAsync() => (Task) this.m_tcs.Task;

    public async Task Set()
    {
      TaskCompletionSource<bool> tcs = this.m_tcs;
      int num1 = await Task.Factory.StartNew<bool>((Func<object, bool>) (s => ((TaskCompletionSource<bool>) s).TrySetResult(true)), (object) tcs, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default).ConfigureAwait(false) ? 1 : 0;
      int num2 = await tcs.Task.ConfigureAwait(false) ? 1 : 0;
      tcs = (TaskCompletionSource<bool>) null;
    }

    public void Reset()
    {
      TaskCompletionSource<bool> tcs;
      do
      {
        tcs = this.m_tcs;
      }
      while (tcs.Task.IsCompleted && Interlocked.CompareExchange<TaskCompletionSource<bool>>(ref this.m_tcs, new TaskCompletionSource<bool>(), tcs) != tcs);
    }
  }
}
