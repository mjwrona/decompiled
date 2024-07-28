// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.TaskHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal static class TaskHelper
  {
    public static Task InlineIfPossibleAsync(
      Func<Task> function,
      IRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return SynchronizationContext.Current == null ? (retryPolicy == null ? function() : (Task) BackoffRetryUtility<int>.ExecuteAsync((Func<Task<int>>) (async () =>
      {
        await function();
        return 0;
      }), retryPolicy, cancellationToken)) : (retryPolicy == null ? Task.Run(function) : (Task) Task.Run<int>((Func<Task<int>>) (() => BackoffRetryUtility<int>.ExecuteAsync((Func<Task<int>>) (async () =>
      {
        await function();
        return 0;
      }), retryPolicy, cancellationToken))));
    }

    public static Task<TResult> InlineIfPossible<TResult>(
      Func<Task<TResult>> function,
      IRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return SynchronizationContext.Current == null ? (retryPolicy == null ? function() : BackoffRetryUtility<TResult>.ExecuteAsync((Func<Task<TResult>>) (() => function()), retryPolicy, cancellationToken)) : (retryPolicy == null ? Task.Run<TResult>(function) : Task.Run<TResult>((Func<Task<TResult>>) (() => BackoffRetryUtility<TResult>.ExecuteAsync((Func<Task<TResult>>) (() => function()), retryPolicy, cancellationToken))));
    }

    public static Task<TResult> RunInlineIfNeededAsync<TResult>(Func<Task<TResult>> task) => SynchronizationContext.Current == null ? task() : Task.Run<TResult>(task);
  }
}
