// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.TaskFactoryExtensions
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal static class TaskFactoryExtensions
  {
    public static Task StartNewOnCurrentTaskSchedulerAsync(
      this TaskFactory taskFactory,
      Action action)
    {
      return taskFactory.StartNew(action, new CancellationToken(), TaskCreationOptions.None, TaskScheduler.Current);
    }

    public static Task StartNewOnCurrentTaskSchedulerAsync(
      this TaskFactory taskFactory,
      Action action,
      CancellationToken cancellationToken)
    {
      return taskFactory.StartNew(action, cancellationToken, TaskCreationOptions.None, TaskScheduler.Current);
    }

    public static Task StartNewOnCurrentTaskSchedulerAsync(
      this TaskFactory taskFactory,
      Action action,
      TaskCreationOptions creationOptions)
    {
      return taskFactory.StartNew(action, new CancellationToken(), creationOptions, TaskScheduler.Current);
    }

    public static Task<TResult> StartNewOnCurrentTaskSchedulerAsync<TResult>(
      this TaskFactory taskFactory,
      Func<TResult> function)
    {
      return taskFactory.StartNew<TResult>(function, new CancellationToken(), TaskCreationOptions.None, TaskScheduler.Current);
    }

    public static Task<TResult> StartNewOnCurrentTaskSchedulerAsync<TResult>(
      this TaskFactory taskFactory,
      Func<TResult> function,
      CancellationToken cancellationToken)
    {
      return taskFactory.StartNew<TResult>(function, cancellationToken, TaskCreationOptions.None, TaskScheduler.Current);
    }

    public static Task<TResult> StartNewOnCurrentTaskSchedulerAsync<TResult>(
      this TaskFactory taskFactory,
      Func<TResult> function,
      TaskCreationOptions creationOptions)
    {
      return taskFactory.StartNew<TResult>(function, new CancellationToken(), creationOptions, TaskScheduler.Current);
    }
  }
}
