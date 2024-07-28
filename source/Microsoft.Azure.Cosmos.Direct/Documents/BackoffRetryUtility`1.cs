// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.BackoffRetryUtility`1
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
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
      return BackoffRetryUtility<T>.ExecuteRetryAsync<object, object>(callbackMethod, (Func<object, CancellationToken, Task<T>>) null, (Func<object, Task<T>>) null, (object) null, retryPolicy, (IRetryPolicy<object>) null, (Func<Task<T>>) null, (Func<object, Task<T>>) null, TimeSpan.Zero, cancellationToken, preRetryCallback);
    }

    public static Task<T> ExecuteAsync<TParam>(
      Func<TParam, CancellationToken, Task<T>> callbackMethod,
      IRetryPolicy retryPolicy,
      TParam param,
      CancellationToken cancellationToken,
      Action<Exception> preRetryCallback = null)
    {
      return BackoffRetryUtility<T>.ExecuteRetryAsync<TParam, object>((Func<Task<T>>) null, callbackMethod, (Func<object, Task<T>>) null, param, retryPolicy, (IRetryPolicy<object>) null, (Func<Task<T>>) null, (Func<object, Task<T>>) null, TimeSpan.Zero, cancellationToken, preRetryCallback);
    }

    public static Task<T> ExecuteAsync<TPolicyArg1>(
      Func<TPolicyArg1, Task<T>> callbackMethod,
      IRetryPolicy<TPolicyArg1> retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken),
      Action<Exception> preRetryCallback = null)
    {
      return BackoffRetryUtility<T>.ExecuteRetryAsync<object, TPolicyArg1>((Func<Task<T>>) null, (Func<object, CancellationToken, Task<T>>) null, callbackMethod, (object) null, (IRetryPolicy) null, retryPolicy, (Func<Task<T>>) null, (Func<TPolicyArg1, Task<T>>) null, TimeSpan.Zero, cancellationToken, preRetryCallback);
    }

    public static Task<T> ExecuteAsync(
      Func<Task<T>> callbackMethod,
      IRetryPolicy retryPolicy,
      Func<Task<T>> inBackoffAlternateCallbackMethod,
      TimeSpan minBackoffForInBackoffCallback,
      CancellationToken cancellationToken = default (CancellationToken),
      Action<Exception> preRetryCallback = null)
    {
      return BackoffRetryUtility<T>.ExecuteRetryAsync<object, object>(callbackMethod, (Func<object, CancellationToken, Task<T>>) null, (Func<object, Task<T>>) null, (object) null, retryPolicy, (IRetryPolicy<object>) null, inBackoffAlternateCallbackMethod, (Func<object, Task<T>>) null, minBackoffForInBackoffCallback, cancellationToken, preRetryCallback);
    }

    public static Task<T> ExecuteAsync<TPolicyArg1>(
      Func<TPolicyArg1, Task<T>> callbackMethod,
      IRetryPolicy<TPolicyArg1> retryPolicy,
      Func<TPolicyArg1, Task<T>> inBackoffAlternateCallbackMethod,
      TimeSpan minBackoffForInBackoffCallback,
      CancellationToken cancellationToken = default (CancellationToken),
      Action<Exception> preRetryCallback = null)
    {
      return BackoffRetryUtility<T>.ExecuteRetryAsync<object, TPolicyArg1>((Func<Task<T>>) null, (Func<object, CancellationToken, Task<T>>) null, callbackMethod, (object) null, (IRetryPolicy) null, retryPolicy, (Func<Task<T>>) null, inBackoffAlternateCallbackMethod, minBackoffForInBackoffCallback, cancellationToken, preRetryCallback);
    }

    private static async Task<T> ExecuteRetryAsync<TParam, TPolicy>(
      Func<Task<T>> callbackMethod,
      Func<TParam, CancellationToken, Task<T>> callbackMethodWithParam,
      Func<TPolicy, Task<T>> callbackMethodWithPolicy,
      TParam param,
      IRetryPolicy retryPolicy,
      IRetryPolicy<TPolicy> retryPolicyWithArg,
      Func<Task<T>> inBackoffAlternateCallbackMethod,
      Func<TPolicy, Task<T>> inBackoffAlternateCallbackMethodWithPolicy,
      TimeSpan minBackoffForInBackoffCallback,
      CancellationToken cancellationToken,
      Action<Exception> preRetryCallback)
    {
      TPolicy policyArg1 = retryPolicyWithArg == null ? default (TPolicy) : retryPolicyWithArg.InitialArgumentValue;
      while (true)
      {
        cancellationToken.ThrowIfCancellationRequested();
        int num;
        try
        {
          if (callbackMethod != null)
            return await callbackMethod();
          return callbackMethodWithParam != null ? await callbackMethodWithParam(param, cancellationToken) : await callbackMethodWithPolicy(policyArg1);
        }
        catch (Exception ex)
        {
          num = 1;
        }
        ExceptionDispatchInfo exception;
        if (num == 1)
        {
          Exception ex = ex;
          await Task.Yield();
          exception = ExceptionDispatchInfo.Capture(ex);
          ex = (Exception) null;
        }
        ShouldRetryResult result;
        if (retryPolicyWithArg != null)
        {
          ShouldRetryResult<TPolicy> shouldRetryResult = await retryPolicyWithArg.ShouldRetryAsync(exception.SourceException, cancellationToken);
          policyArg1 = shouldRetryResult.PolicyArg1;
          result = (ShouldRetryResult) shouldRetryResult;
        }
        else
          result = await retryPolicy.ShouldRetryAsync(exception.SourceException, cancellationToken);
        result.ThrowIfDoneTrying(exception);
        TimeSpan delay = result.BackoffTime;
        if ((inBackoffAlternateCallbackMethod != null ? 1 : (inBackoffAlternateCallbackMethodWithPolicy != null ? 1 : 0)) != 0 && result.BackoffTime >= minBackoffForInBackoffCallback)
        {
          ValueStopwatch stopwatch = ValueStopwatch.StartNew();
          TimeSpan elapsed;
          try
          {
            return inBackoffAlternateCallbackMethod != null ? await inBackoffAlternateCallbackMethod() : await inBackoffAlternateCallbackMethodWithPolicy(policyArg1);
          }
          catch (Exception ex)
          {
            elapsed = stopwatch.Elapsed;
            DefaultTrace.TraceInformation("Failed inBackoffAlternateCallback with {0}, proceeding with retry. Time taken: {1}ms", (object) ex.ToString(), (object) elapsed.TotalMilliseconds);
          }
          delay = result.BackoffTime > elapsed ? result.BackoffTime - elapsed : TimeSpan.Zero;
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
