// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TransientErrorActionRetryer
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Practices.TransientFaultHandling;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class TransientErrorActionRetryer
  {
    public static readonly RetryStrategy FixedIntervalRetryStrategy = (RetryStrategy) new FixedInterval(nameof (FixedIntervalRetryStrategy), 5, TimeSpan.FromMilliseconds(100.0), false);
    public static readonly RetryStrategy ShorterExponentialBackoffRetryStrategy = (RetryStrategy) new ExponentialBackoff(nameof (ShorterExponentialBackoffRetryStrategy), 5, TimeSpan.FromMilliseconds(100.0), TimeSpan.FromSeconds(4.0), TimeSpan.FromMilliseconds(200.0), false);
    public static readonly RetryStrategy LongerExponentialBackoffRetryStrategy = (RetryStrategy) new ExponentialBackoff(nameof (LongerExponentialBackoffRetryStrategy), 5, TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(3.0), false);

    public static void TryAction<T>(Action action) where T : ITransientErrorDetectionStrategy, new() => TransientErrorActionRetryer.TryAction<T>(action, TransientErrorActionRetryer.LongerExponentialBackoffRetryStrategy);

    public static TResult TryAction<TStrat, TResult>(Func<TResult> func) where TStrat : ITransientErrorDetectionStrategy, new() => TransientErrorActionRetryer.TryAction<TStrat, TResult>(func, TransientErrorActionRetryer.LongerExponentialBackoffRetryStrategy);

    public static Task TryActionAsync<T>(Func<Task> taskAction) where T : ITransientErrorDetectionStrategy, new() => TransientErrorActionRetryer.TryActionAsync<T>(taskAction, TransientErrorActionRetryer.LongerExponentialBackoffRetryStrategy);

    public static Task<TResult> TryActionAsync<TStrat, TResult>(
      Func<CancellationToken, Task<TResult>> func,
      CancellationToken ctx)
      where TStrat : ITransientErrorDetectionStrategy, new()
    {
      return TransientErrorActionRetryer.TryActionAsync<TStrat, TResult>(func, TransientErrorActionRetryer.LongerExponentialBackoffRetryStrategy, ctx);
    }

    public static Task<TResult> TryActionAsync<TStrat, TResult>(Func<Task<TResult>> taskFunc) where TStrat : ITransientErrorDetectionStrategy, new() => TransientErrorActionRetryer.TryActionAsync<TStrat, TResult>(taskFunc, TransientErrorActionRetryer.LongerExponentialBackoffRetryStrategy);

    public static void TryAction<T>(
      Action action,
      RetryStrategy retryStrategy,
      EventHandler<RetryingEventArgs> retryingEventHandler = null)
      where T : ITransientErrorDetectionStrategy, new()
    {
      TransientErrorActionRetryer.SetupRetryPolicy<T>(retryStrategy, retryingEventHandler).ExecuteAction(action);
    }

    public static TResult TryAction<TStrat, TResult>(
      Func<TResult> func,
      RetryStrategy retryStrategy,
      EventHandler<RetryingEventArgs> retryingEventHandler = null)
      where TStrat : ITransientErrorDetectionStrategy, new()
    {
      return TransientErrorActionRetryer.SetupRetryPolicy<TStrat>(retryStrategy, retryingEventHandler).ExecuteAction<TResult>(func);
    }

    public static Task TryActionAsync<T>(
      Func<Task> taskAction,
      RetryStrategy retryStrategy,
      EventHandler<RetryingEventArgs> retryingEventHandler = null)
      where T : ITransientErrorDetectionStrategy, new()
    {
      return TransientErrorActionRetryer.SetupRetryPolicy<T>(retryStrategy, retryingEventHandler).ExecuteAsync(taskAction);
    }

    public static Task<TResult> TryActionAsync<TStrat, TResult>(
      Func<Task<TResult>> taskFunc,
      RetryStrategy retryStrategy,
      EventHandler<RetryingEventArgs> retryingEventHandler = null)
      where TStrat : ITransientErrorDetectionStrategy, new()
    {
      return TransientErrorActionRetryer.SetupRetryPolicy<TStrat>(retryStrategy, retryingEventHandler).ExecuteAsync<TResult>(taskFunc);
    }

    public static Task<TResult> TryActionAsync<TStrat, TResult>(
      Func<CancellationToken, Task<TResult>> taskFunc,
      RetryStrategy retryStrategy,
      CancellationToken ctx,
      EventHandler<RetryingEventArgs> retryingEventHandler = null)
      where TStrat : ITransientErrorDetectionStrategy, new()
    {
      return TransientErrorActionRetryer.SetupRetryPolicy<TStrat>(retryStrategy, retryingEventHandler).ExecuteAsync<TResult>((Func<Task<TResult>>) (() => taskFunc(ctx)), ctx);
    }

    private static RetryPolicy SetupRetryPolicy<TStrat>(
      RetryStrategy retryStrategy,
      EventHandler<RetryingEventArgs> retryingEventHandler)
      where TStrat : ITransientErrorDetectionStrategy, new()
    {
      RetryPolicy<TStrat> retryPolicy = new RetryPolicy<TStrat>(retryStrategy);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      retryPolicy.Retrying += TransientErrorActionRetryer.\u003C\u003EO.\u003C0\u003E__LogRetryAttempt ?? (TransientErrorActionRetryer.\u003C\u003EO.\u003C0\u003E__LogRetryAttempt = new EventHandler<RetryingEventArgs>(TransientErrorActionRetryer.LogRetryAttempt));
      if (retryingEventHandler != null)
        retryPolicy.Retrying += retryingEventHandler;
      return (RetryPolicy) retryPolicy;
    }

    private static void LogRetryAttempt(object o, RetryingEventArgs args)
    {
    }
  }
}
