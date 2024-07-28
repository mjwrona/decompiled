// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.TaskSafety
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class TaskSafety
  {
    public static void SyncResultOnThreadPool(
      Func<Task> taskFunc,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Task.Run(taskFunc, cancellationToken).GetAwaiter().GetResult();
    }

    public static T SyncResultOnThreadPool<T>(
      Func<Task<T>> taskFunc,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return Task.Run<T>(taskFunc, cancellationToken).GetAwaiter().GetResult();
    }
  }
}
