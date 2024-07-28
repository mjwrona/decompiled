// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.TaskEx
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  internal static class TaskEx
  {
    public static async Task<T> WithCancellation<T>(
      this Task<T> task,
      CancellationToken cancellationToken)
    {
      task.RequiresArgumentNotNull<Task<T>>(nameof (task));
      TaskCompletionSource<bool> state = new TaskCompletionSource<bool>();
      CancellationTokenRegistration tokenRegistration = cancellationToken.Register((Action<object>) (s => ((TaskCompletionSource<bool>) s).TrySetResult(true)), (object) state);
      try
      {
        object obj = (object) task;
        Task[] taskArray = new Task[2]
        {
          (Task) task,
          (Task) state.Task
        };
        if (obj != await Task.WhenAny(taskArray).ConfigureAwait(false))
        {
          obj = (object) null;
          cancellationToken.ThrowIfCancellationRequested();
        }
      }
      finally
      {
        tokenRegistration.Dispose();
      }
      tokenRegistration = new CancellationTokenRegistration();
      return await task.ConfigureAwait(false);
    }
  }
}
