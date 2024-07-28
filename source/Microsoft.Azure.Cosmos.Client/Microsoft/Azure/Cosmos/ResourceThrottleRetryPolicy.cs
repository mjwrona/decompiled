// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ResourceThrottleRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ResourceThrottleRetryPolicy : IDocumentClientRetryPolicy, IRetryPolicy
  {
    private const int DefaultMaxWaitTimeInSeconds = 60;
    private const int DefaultRetryInSeconds = 5;
    private readonly uint backoffDelayFactor;
    private readonly int maxAttemptCount;
    private readonly TimeSpan maxWaitTimeInMilliseconds;
    private int currentAttemptCount;
    private TimeSpan cumulativeRetryDelay;

    public ResourceThrottleRetryPolicy(
      int maxAttemptCount,
      int maxWaitTimeInSeconds = 60,
      uint backoffDelayFactor = 1)
    {
      if (maxWaitTimeInSeconds > 2147483)
        throw new ArgumentException(nameof (maxWaitTimeInSeconds), "maxWaitTimeInSeconds must be less than " + 2147483.ToString());
      this.maxAttemptCount = maxAttemptCount;
      this.backoffDelayFactor = backoffDelayFactor;
      this.maxWaitTimeInMilliseconds = TimeSpan.FromSeconds((double) maxWaitTimeInSeconds);
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      if (exception is DocumentClientException documentClientException)
      {
        if (this.IsValidThrottleStatusCode(documentClientException.StatusCode))
          return this.ShouldRetryInternalAsync(new TimeSpan?(documentClientException.RetryAfter));
        DefaultTrace.TraceError("Operation will NOT be retried. Current attempt {0}, Status Code: {1} ", (object) this.currentAttemptCount, (object) documentClientException.StatusCode);
        return Task.FromResult<ShouldRetryResult>(ShouldRetryResult.NoRetry());
      }
      DefaultTrace.TraceError("Operation will NOT be retried. Current attempt {0}, Exception: {1} ", (object) this.currentAttemptCount, this.GetExceptionMessage(exception));
      return Task.FromResult<ShouldRetryResult>(ShouldRetryResult.NoRetry());
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      ResponseMessage cosmosResponseMessage,
      CancellationToken cancellationToken)
    {
      if (this.IsValidThrottleStatusCode(cosmosResponseMessage?.StatusCode))
        return this.ShouldRetryInternalAsync((TimeSpan?) cosmosResponseMessage?.Headers.RetryAfter);
      DefaultTrace.TraceError("Operation will NOT be retried. Current attempt {0}, Status Code: {1} ", (object) this.currentAttemptCount, (object) cosmosResponseMessage?.StatusCode);
      return Task.FromResult<ShouldRetryResult>(ShouldRetryResult.NoRetry());
    }

    private Task<ShouldRetryResult> ShouldRetryInternalAsync(TimeSpan? retryAfter)
    {
      TimeSpan retryDelay = TimeSpan.Zero;
      if (this.currentAttemptCount < this.maxAttemptCount && this.CheckIfRetryNeeded(retryAfter, out retryDelay))
      {
        ++this.currentAttemptCount;
        DefaultTrace.TraceWarning("Operation will be retried after {0} milliseconds. Current attempt {1}, Cumulative delay {2}", (object) retryDelay.TotalMilliseconds, (object) this.currentAttemptCount, (object) this.cumulativeRetryDelay);
        return Task.FromResult<ShouldRetryResult>(ShouldRetryResult.RetryAfter(retryDelay));
      }
      DefaultTrace.TraceError("Operation will NOT be retried. Current attempt {0} maxAttempts {1} Cumulative delay {2} requested retryAfter {3} maxWaitTime {4}", (object) this.currentAttemptCount, (object) this.maxAttemptCount, (object) this.cumulativeRetryDelay, (object) retryAfter, (object) this.maxWaitTimeInMilliseconds);
      return Task.FromResult<ShouldRetryResult>(ShouldRetryResult.NoRetry());
    }

    private object GetExceptionMessage(Exception exception) => exception is DocumentClientException documentClientException && documentClientException.StatusCode.HasValue && documentClientException.StatusCode.Value < HttpStatusCode.InternalServerError ? (object) exception.Message : (object) exception;

    public void OnBeforeSendRequest(DocumentServiceRequest request)
    {
    }

    private bool CheckIfRetryNeeded(TimeSpan? retryAfter, out TimeSpan retryDelay)
    {
      retryDelay = TimeSpan.Zero;
      if (retryAfter.HasValue)
        retryDelay = retryAfter.Value;
      if (this.backoffDelayFactor > 1U)
        retryDelay = TimeSpan.FromTicks(retryDelay.Ticks * (long) this.backoffDelayFactor);
      if (!(retryDelay < this.maxWaitTimeInMilliseconds) || !(this.maxWaitTimeInMilliseconds >= (this.cumulativeRetryDelay = retryDelay.Add(this.cumulativeRetryDelay))))
        return false;
      if (retryDelay == TimeSpan.Zero)
      {
        DefaultTrace.TraceInformation("Received retryDelay of 0 with Http 429: {0}", (object) retryAfter);
        retryDelay = TimeSpan.FromSeconds(5.0);
      }
      return true;
    }

    private bool IsValidThrottleStatusCode(HttpStatusCode? statusCode) => statusCode.HasValue && statusCode.Value == (HttpStatusCode) 429;
  }
}
