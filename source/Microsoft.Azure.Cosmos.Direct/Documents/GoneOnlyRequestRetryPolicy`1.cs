// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GoneOnlyRequestRetryPolicy`1
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal class GoneOnlyRequestRetryPolicy<TResponse> : 
    IRequestRetryPolicy<GoneOnlyRequestRetryPolicyContext, DocumentServiceRequest, TResponse>,
    IRequestRetryPolicy<DocumentServiceRequest, TResponse>
    where TResponse : IRetriableResponse
  {
    private const int backoffMultiplier = 2;
    private const int initialBackoffTimeInSeconds = 1;
    private Stopwatch durationTimer = new Stopwatch();
    private readonly TimeSpan retryTimeout;
    private int currentBackoffTimeInSeconds;
    private bool isInRetry;

    public GoneOnlyRequestRetryPolicy(TimeSpan retryTimeout)
    {
      this.retryTimeout = retryTimeout;
      this.currentBackoffTimeInSeconds = 1;
      this.isInRetry = false;
      this.ExecuteContext.RemainingTimeInMsOnClientRequest = retryTimeout;
      this.durationTimer.Start();
    }

    public GoneOnlyRequestRetryPolicyContext ExecuteContext { get; } = new GoneOnlyRequestRetryPolicyContext();

    public void OnBeforeSendRequest(DocumentServiceRequest request)
    {
    }

    public bool TryHandleResponseSynchronously(
      DocumentServiceRequest request,
      TResponse response,
      Exception exception,
      out ShouldRetryResult shouldRetryResult)
    {
      if (((object) response != null ? (response.StatusCode != HttpStatusCode.Gone ? 1 : 0) : 1) != 0 && !(exception is GoneException))
      {
        shouldRetryResult = ShouldRetryResult.NoRetry();
        return true;
      }
      SubStatusCodes forGoneRetryPolicy = DocumentClientException.GetExceptionSubStatusForGoneRetryPolicy(exception);
      TimeSpan elapsed = this.durationTimer.Elapsed;
      if (elapsed >= this.retryTimeout)
      {
        DefaultTrace.TraceInformation("GoneOnlyRequestRetryPolicy - timeout {0}, elapsed {1}", (object) this.retryTimeout, (object) elapsed);
        this.durationTimer.Stop();
        shouldRetryResult = ShouldRetryResult.NoRetry((Exception) ServiceUnavailableException.Create(new SubStatusCodes?(forGoneRetryPolicy), exception));
        return true;
      }
      TimeSpan timeSpan = this.retryTimeout - elapsed;
      TimeSpan backoffTime = TimeSpan.Zero;
      if (this.isInRetry)
      {
        backoffTime = TimeSpan.FromSeconds((double) this.currentBackoffTimeInSeconds);
        this.currentBackoffTimeInSeconds *= 2;
        if (backoffTime > timeSpan)
        {
          DefaultTrace.TraceInformation("GoneOnlyRequestRetryPolicy - timeout {0}, elapsed {1}, backoffTime {2}", (object) this.retryTimeout, (object) elapsed, (object) backoffTime);
          this.durationTimer.Stop();
          shouldRetryResult = ShouldRetryResult.NoRetry((Exception) ServiceUnavailableException.Create(new SubStatusCodes?(forGoneRetryPolicy), exception));
          return true;
        }
      }
      else
        this.isInRetry = true;
      DefaultTrace.TraceInformation("GoneOnlyRequestRetryPolicy - timeout {0}, elapsed {1}, backoffTime {2}, remainingTime {3}", (object) this.retryTimeout, (object) elapsed, (object) backoffTime, (object) timeSpan);
      shouldRetryResult = ShouldRetryResult.RetryAfter(backoffTime);
      this.ExecuteContext.IsInRetry = this.isInRetry;
      this.ExecuteContext.ForceRefresh = true;
      this.ExecuteContext.RemainingTimeInMsOnClientRequest = timeSpan;
      return true;
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      DocumentServiceRequest request,
      TResponse response,
      Exception exception,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}
