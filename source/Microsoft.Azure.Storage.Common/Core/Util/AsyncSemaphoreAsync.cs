// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.AsyncSemaphoreAsync
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class AsyncSemaphoreAsync
  {
    private int count;
    private readonly Queue<Func<bool, Task>> pendingWaits = new Queue<Func<bool, Task>>();

    public AsyncSemaphoreAsync(int initialCount)
    {
      CommonUtility.AssertInBounds<int>(nameof (initialCount), initialCount, 0, int.MaxValue);
      this.count = initialCount;
    }

    public Task<bool> WaitAsync(
      Func<bool, CancellationToken, Task> callback,
      CancellationToken token)
    {
      CommonUtility.AssertNotNull(nameof (callback), (object) callback);
      bool flag = false;
      lock (this.pendingWaits)
      {
        if (this.count > 0)
        {
          --this.count;
        }
        else
        {
          this.pendingWaits.Enqueue((Func<bool, Task>) (calledInline => callback(calledInline, CancellationToken.None)));
          flag = true;
        }
      }
      return !flag ? callback(true, token).ContinueWith<bool>((Func<Task, bool>) (t => true)) : Task.FromResult<bool>(false);
    }

    public Task ReleaseAsync(CancellationToken token)
    {
      Func<bool, Task> func = (Func<bool, Task>) null;
      lock (this.pendingWaits)
      {
        if (this.pendingWaits.Count > 0)
          func = this.pendingWaits.Dequeue();
        else
          ++this.count;
      }
      return func != null ? func(false) : Task.Delay(0);
    }
  }
}
