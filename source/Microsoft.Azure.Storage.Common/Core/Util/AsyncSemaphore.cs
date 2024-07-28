// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.AsyncSemaphore
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class AsyncSemaphore
  {
    private static readonly Task<bool> CompletedTask = Task.FromResult<bool>(true);
    private readonly Queue<TaskCompletionSource<bool>> pendingWaits = new Queue<TaskCompletionSource<bool>>();
    private int count;

    public Task<bool> WaitAsync()
    {
      lock (this.pendingWaits)
      {
        if (this.count > 0)
        {
          --this.count;
          return AsyncSemaphore.CompletedTask;
        }
        TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
        this.pendingWaits.Enqueue(completionSource);
        return completionSource.Task;
      }
    }

    public void Release()
    {
      TaskCompletionSource<bool> completionSource = (TaskCompletionSource<bool>) null;
      lock (this.pendingWaits)
      {
        if (this.pendingWaits.Count > 0)
          completionSource = this.pendingWaits.Dequeue();
        else
          ++this.count;
      }
      completionSource?.SetResult(false);
    }

    public AsyncSemaphore(int initialCount)
    {
      CommonUtility.AssertInBounds<int>(nameof (initialCount), initialCount, 0, int.MaxValue);
      this.count = initialCount;
    }
  }
}
