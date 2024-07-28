// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.CounterEventAsync
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal sealed class CounterEventAsync
  {
    private AsyncManualResetEvent internalEvent = new AsyncManualResetEvent(true);
    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
    private int counter;

    public void Increment()
    {
      this.semaphoreSlim.Wait();
      try
      {
        ++this.counter;
        this.internalEvent.Reset();
      }
      finally
      {
        this.semaphoreSlim.Release();
      }
    }

    public async Task DecrementAsync()
    {
      CounterEventAsync counterEventAsync1 = this;
      await counterEventAsync1.semaphoreSlim.WaitAsync().ConfigureAwait(false);
      try
      {
        CounterEventAsync counterEventAsync2 = counterEventAsync1;
        int num1 = counterEventAsync1.counter - 1;
        int num2 = num1;
        counterEventAsync2.counter = num2;
        if (num1 != 0)
          return;
        await counterEventAsync1.internalEvent.Set().ConfigureAwait(false);
      }
      finally
      {
        counterEventAsync1.semaphoreSlim.Release();
      }
    }

    public Task WaitAsync() => this.internalEvent.WaitAsync();
  }
}
