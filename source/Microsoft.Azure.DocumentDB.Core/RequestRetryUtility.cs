// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RequestRetryUtility
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
  internal static class RequestRetryUtility
  {
    public static Task<IRetriableResponse> ProcessRequestAsync<TInitialArguments, TRequest, IRetriableResponse>(
      Func<TInitialArguments, Task<IRetriableResponse>> executeAsync,
      Func<TRequest> prepareRequest,
      IRequestRetryPolicy<TInitialArguments, TRequest, IRetriableResponse> policy,
      CancellationToken cancellationToken)
    {
      return RequestRetryUtility.ProcessRequestAsync<TRequest, IRetriableResponse>((Func<Task<IRetriableResponse>>) (() => executeAsync(policy.ExecuteContext)), prepareRequest, (IRequestRetryPolicy<TRequest, IRetriableResponse>) policy, cancellationToken);
    }

    public static Task<IRetriableResponse> ProcessRequestAsync<TInitialArguments, TRequest, IRetriableResponse>(
      Func<TInitialArguments, Task<IRetriableResponse>> executeAsync,
      Func<TRequest> prepareRequest,
      IRequestRetryPolicy<TInitialArguments, TRequest, IRetriableResponse> policy,
      Func<TInitialArguments, Task<IRetriableResponse>> inBackoffAlternateCallbackMethod,
      TimeSpan minBackoffForInBackoffCallback,
      CancellationToken cancellationToken)
    {
      return inBackoffAlternateCallbackMethod != null ? RequestRetryUtility.ProcessRequestAsync<TRequest, IRetriableResponse>((Func<Task<IRetriableResponse>>) (() => executeAsync(policy.ExecuteContext)), prepareRequest, (IRequestRetryPolicy<TRequest, IRetriableResponse>) policy, cancellationToken, (Func<Task<IRetriableResponse>>) (() => inBackoffAlternateCallbackMethod(policy.ExecuteContext)), new TimeSpan?(minBackoffForInBackoffCallback)) : RequestRetryUtility.ProcessRequestAsync<TRequest, IRetriableResponse>((Func<Task<IRetriableResponse>>) (() => executeAsync(policy.ExecuteContext)), prepareRequest, (IRequestRetryPolicy<TRequest, IRetriableResponse>) policy, cancellationToken);
    }

    public static async Task<IRetriableResponse> ProcessRequestAsync<TRequest, IRetriableResponse>(
      Func<Task<IRetriableResponse>> executeAsync,
      Func<TRequest> prepareRequest,
      IRequestRetryPolicy<TRequest, IRetriableResponse> policy,
      CancellationToken cancellationToken,
      Func<Task<IRetriableResponse>> inBackoffAlternateCallbackMethod = null,
      TimeSpan? minBackoffForInBackoffCallback = null)
    {
      IRetriableResponse response;
      ExceptionDispatchInfo capturedException;
      ShouldRetryResult shouldRetry;
      while (true)
      {
        response = default (IRetriableResponse);
        Exception exception = (Exception) null;
        capturedException = (ExceptionDispatchInfo) null;
        TRequest request = default (TRequest);
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
          request = prepareRequest();
          policy.OnBeforeSendRequest(request);
          response = await executeAsync();
        }
        catch (Exception ex)
        {
          capturedException = ExceptionDispatchInfo.Capture(ex);
          exception = capturedException.SourceException;
        }
        shouldRetry = (ShouldRetryResult) null;
        if (!policy.TryHandleResponseSynchronously(request, response, exception, out shouldRetry))
          shouldRetry = await policy.ShouldRetryAsync(request, response, exception, cancellationToken);
        if (shouldRetry.ShouldRetry)
        {
          TimeSpan delay = shouldRetry.BackoffTime;
          if (inBackoffAlternateCallbackMethod != null && delay >= minBackoffForInBackoffCallback.Value)
          {
            Stopwatch stopwatch = new Stopwatch();
            try
            {
              stopwatch.Start();
              IRetriableResponse inBackoffResponse = await inBackoffAlternateCallbackMethod();
              stopwatch.Stop();
              ShouldRetryResult shouldRetryResult = (ShouldRetryResult) null;
              if (!policy.TryHandleResponseSynchronously(request, inBackoffResponse, (Exception) null, out shouldRetryResult))
                shouldRetryResult = await policy.ShouldRetryAsync(request, inBackoffResponse, (Exception) null, cancellationToken);
              if (!shouldRetryResult.ShouldRetry)
                return inBackoffResponse;
              DefaultTrace.TraceInformation("Failed inBackoffAlternateCallback with response, proceeding with retry. Time taken: {0}ms", (object) stopwatch.ElapsedMilliseconds);
              inBackoffResponse = default (IRetriableResponse);
            }
            catch (Exception ex)
            {
              stopwatch.Stop();
              DefaultTrace.TraceInformation("Failed inBackoffAlternateCallback with {0}, proceeding with retry. Time taken: {1}ms", (object) ex.ToString(), (object) stopwatch.ElapsedMilliseconds);
            }
            delay = shouldRetry.BackoffTime > stopwatch.Elapsed ? shouldRetry.BackoffTime - stopwatch.Elapsed : TimeSpan.Zero;
            stopwatch = (Stopwatch) null;
          }
          if (delay != TimeSpan.Zero)
            await Task.Delay(delay, cancellationToken);
          response = default (IRetriableResponse);
          exception = (Exception) null;
          capturedException = (ExceptionDispatchInfo) null;
          request = default (TRequest);
          shouldRetry = (ShouldRetryResult) null;
        }
        else
          break;
      }
      if (capturedException != null || shouldRetry.ExceptionToThrow != null)
        shouldRetry.ThrowIfDoneTrying(capturedException);
      return response;
    }
  }
}
