// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.TaskFactoryExtensions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
