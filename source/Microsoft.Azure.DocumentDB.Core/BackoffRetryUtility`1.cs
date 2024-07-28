// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.BackoffRetryUtility`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal static class BackoffRetryUtility<T>
  {
    public const string ExceptionSourceToIgnoreForIgnoreForRetry = "BackoffRetryUtility";

    public static Task<T> ExecuteAsync(
      Func<Task<T>> callbackMethod,
      IRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken),
      Action<Exception> preRetryCallback = null)
    {
      return BackoffRetryUtility<T>.ExecuteRetryAsync((Func<Task<T>>) (() => callbackMethod()), (Func<Exception, CancellationToken, Task<ShouldRetryResult>>) ((exception, token) => retryPolicy.ShouldRetryAsync(exception, cancellationToken)), (Func<Task<T>>) null, TimeSpan.Zero, cancellationToken, preRetryCallback);
    }

    public static Task<T> ExecuteAsync<TPolicyArg1>(
      Func<TPolicyArg1, Task<T>> callbackMethod,
      IRetryPolicy<TPolicyArg1> retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken),
      Action<Exception> preRetryCallback = null)
    {
      TPolicyArg1 policyArg1 = retryPolicy.InitialArgumentValue;
      return BackoffRetryUtility<T>.ExecuteRetryAsync((Func<Task<T>>) (() => callbackMethod(policyArg1)), (Func<Exception, CancellationToken, Task<ShouldRetryResult>>) (async (exception, token) =>
      {
        ShouldRetryResult<TPolicyArg1> shouldRetryResult = await retryPolicy.ShouldRetryAsync(exception, cancellationToken);
        policyArg1 = shouldRetryResult.PolicyArg1;
        return (ShouldRetryResult) shouldRetryResult;
      }), (Func<Task<T>>) null, TimeSpan.Zero, cancellationToken, preRetryCallback);
    }

    public static Task<T> ExecuteAsync(
      Func<Task<T>> callbackMethod,
      IRetryPolicy retryPolicy,
      Func<Task<T>> inBackoffAlternateCallbackMethod,
      TimeSpan minBackoffForInBackoffCallback,
      CancellationToken cancellationToken = default (CancellationToken),
      Action<Exception> preRetryCallback = null)
    {
      Func<Task<T>> inBackoffAlternateCallbackMethod1 = (Func<Task<T>>) null;
      if (inBackoffAlternateCallbackMethod != null)
        inBackoffAlternateCallbackMethod1 = (Func<Task<T>>) (() => inBackoffAlternateCallbackMethod());
      return BackoffRetryUtility<T>.ExecuteRetryAsync((Func<Task<T>>) (() => callbackMethod()), (Func<Exception, CancellationToken, Task<ShouldRetryResult>>) ((exception, token) => retryPolicy.ShouldRetryAsync(exception, cancellationToken)), inBackoffAlternateCallbackMethod1, minBackoffForInBackoffCallback, cancellationToken, preRetryCallback);
    }

    public static Task<T> ExecuteAsync<TPolicyArg1>(
      Func<TPolicyArg1, Task<T>> callbackMethod,
      IRetryPolicy<TPolicyArg1> retryPolicy,
      Func<TPolicyArg1, Task<T>> inBackoffAlternateCallbackMethod,
      TimeSpan minBackoffForInBackoffCallback,
      CancellationToken cancellationToken = default (CancellationToken),
      Action<Exception> preRetryCallback = null)
    {
      TPolicyArg1 policyArg1 = retryPolicy.InitialArgumentValue;
      Func<Task<T>> inBackoffAlternateCallbackMethod1 = (Func<Task<T>>) null;
      if (inBackoffAlternateCallbackMethod != null)
        inBackoffAlternateCallbackMethod1 = (Func<Task<T>>) (() => inBackoffAlternateCallbackMethod(policyArg1));
      return BackoffRetryUtility<T>.ExecuteRetryAsync((Func<Task<T>>) (() => callbackMethod(policyArg1)), (Func<Exception, CancellationToken, Task<ShouldRetryResult>>) (async (exception, token) =>
      {
        ShouldRetryResult<TPolicyArg1> shouldRetryResult = await retryPolicy.ShouldRetryAsync(exception, cancellationToken);
        policyArg1 = shouldRetryResult.PolicyArg1;
        return (ShouldRetryResult) shouldRetryResult;
      }), inBackoffAlternateCallbackMethod1, minBackoffForInBackoffCallback, cancellationToken, preRetryCallback);
    }

    internal static async Task<T> ExecuteRetryAsync(
      Func<Task<T>> callbackMethod,
      Func<Exception, CancellationToken, Task<ShouldRetryResult>> callShouldRetry,
      Func<Task<T>> inBackoffAlternateCallbackMethod,
      TimeSpan minBackoffForInBackoffCallback,
      CancellationToken cancellationToken,
      Action<Exception> preRetryCallback = null)
    {
      while (true)
      {
        ExceptionDispatchInfo exception = (ExceptionDispatchInfo) null;
        try
        {
          cancellationToken.ThrowIfCancellationRequested();
          return await callbackMethod();
        }
        catch (Exception ex)
        {
          exception = ExceptionDispatchInfo.Capture(ex);
        }
        cancellationToken.ThrowIfCancellationRequested();
        ShouldRetryResult result = await callShouldRetry(exception.SourceException, cancellationToken);
        result.ThrowIfDoneTrying(exception);
        TimeSpan delay = result.BackoffTime;
        if (inBackoffAlternateCallbackMethod != null && result.BackoffTime >= minBackoffForInBackoffCallback)
        {
          Stopwatch stopwatch = new Stopwatch();
          try
          {
            stopwatch.Start();
            return await inBackoffAlternateCallbackMethod();
          }
          catch (Exception ex)
          {
            stopwatch.Stop();
            DefaultTrace.TraceInformation("Failed inBackoffAlternateCallback with {0}, proceeding with retry. Time taken: {1}ms", (object) ex.ToString(), (object) stopwatch.ElapsedMilliseconds);
          }
          delay = result.BackoffTime > stopwatch.Elapsed ? result.BackoffTime - stopwatch.Elapsed : TimeSpan.Zero;
          stopwatch = (Stopwatch) null;
        }
        if (preRetryCallback != null)
          preRetryCallback(exception.SourceException);
        if (delay != TimeSpan.Zero)
          await Task.Delay(delay, cancellationToken);
        exception = (ExceptionDispatchInfo) null;
        result = (ShouldRetryResult) null;
      }
    }
  }
}
