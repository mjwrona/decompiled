// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.AsyncExtensions
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class AsyncExtensions
  {
    internal static IAsyncResult AsApm<T>(this Task<T> task, AsyncCallback callback, object state)
    {
      if (task == null)
        throw new ArgumentNullException(nameof (task));
      TaskCompletionSource<T> tcs = new TaskCompletionSource<T>(state);
      task.ContinueWith((Action<Task<T>>) (t =>
      {
        if (t.IsFaulted)
          tcs.TrySetException((IEnumerable<Exception>) t.Exception.InnerExceptions);
        else if (t.IsCanceled)
          tcs.TrySetCanceled();
        else
          tcs.TrySetResult(t.Result);
        if (callback == null)
          return;
        callback((IAsyncResult) tcs.Task);
      }), TaskScheduler.Default);
      return (IAsyncResult) tcs.Task;
    }

    internal static IAsyncResult AsApm(this Task task, AsyncCallback callback, object state)
    {
      if (task == null)
        throw new ArgumentNullException(nameof (task));
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(state);
      task.ContinueWith((Action<Task>) (t =>
      {
        if (t.IsFaulted)
          tcs.TrySetException((IEnumerable<Exception>) t.Exception.InnerExceptions);
        else if (t.IsCanceled)
          tcs.TrySetCanceled();
        else
          tcs.TrySetResult((object) null);
        if (callback == null)
          return;
        callback((IAsyncResult) tcs.Task);
      }), TaskScheduler.Default);
      return (IAsyncResult) tcs.Task;
    }
  }
}
