// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Tasks.TaskExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Tasks
{
  public static class TaskExtensions
  {
    public static IAsyncResult ToApm(this Task task, AsyncCallback cb, object state)
    {
      if (task == null)
        throw new ArgumentNullException(nameof (task));
      IAsyncResult asyncResult;
      if (task.IsCompleted)
      {
        asyncResult = (IAsyncResult) new TaskWrapperAsyncResult(task, state, true);
        if (cb != null)
          cb(asyncResult);
      }
      else
      {
        asyncResult = task.AsyncState == state ? (IAsyncResult) task : (IAsyncResult) new TaskWrapperAsyncResult(task, state, false);
        if (cb != null)
          task.ConfigureAwait(false).GetAwaiter().OnCompleted((Action) (() => cb(asyncResult)));
      }
      return asyncResult;
    }

    public static void WaitForApmConvertedTask(this IAsyncResult result)
    {
      if (result == null)
        throw new ArgumentNullException(nameof (result));
      (result is TaskWrapperAsyncResult wrapperAsyncResult ? wrapperAsyncResult.Task : (Task) result).GetAwaiter().GetResult();
    }
  }
}
