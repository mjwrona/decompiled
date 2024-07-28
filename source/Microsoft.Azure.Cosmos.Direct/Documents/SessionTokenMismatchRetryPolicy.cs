// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SessionTokenMismatchRetryPolicy
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
  internal sealed class SessionTokenMismatchRetryPolicy : IRetryPolicy
  {
    private const string sessionRetryInitialBackoff = "AZURE_COSMOS_SESSION_RETRY_INITIAL_BACKOFF";
    private const string sessionRetryMaximumBackoff = "AZURE_COSMOS_SESSION_RETRY_MAXIMUM_BACKOFF";
    private const int defaultWaitTimeInMilliSeconds = 5000;
    private const int defaultInitialBackoffTimeInMilliseconds = 5;
    private const int defaultMaximumBackoffTimeInMilliseconds = 500;
    private const int backoffMultiplier = 2;
    private static readonly Lazy<int> sessionRetryInitialBackoffConfig = new Lazy<int>((Func<int>) (() =>
    {
      string environmentVariable = Environment.GetEnvironmentVariable("AZURE_COSMOS_SESSION_RETRY_INITIAL_BACKOFF");
      if (!string.IsNullOrWhiteSpace(environmentVariable))
      {
        int result;
        if (int.TryParse(environmentVariable, out result) && result >= 0)
          return result;
        DefaultTrace.TraceCritical("The value of AZURE_COSMOS_SESSION_RETRY_INITIAL_BACKOFF is invalid. Value: {0}", (object) result);
      }
      return 5;
    }));
    private static readonly Lazy<int> sessionRetryMaximumBackoffConfig = new Lazy<int>((Func<int>) (() =>
    {
      string environmentVariable = Environment.GetEnvironmentVariable("AZURE_COSMOS_SESSION_RETRY_MAXIMUM_BACKOFF");
      if (!string.IsNullOrWhiteSpace(environmentVariable))
      {
        int result;
        if (int.TryParse(environmentVariable, out result) && result >= 0)
          return result;
        DefaultTrace.TraceCritical("The value of AZURE_COSMOS_SESSION_RETRY_MAXIMUM_BACKOFF is invalid. Value: {0}", (object) result);
      }
      return 500;
    }));
    private int retryCount;
    private Stopwatch durationTimer = new Stopwatch();
    private int waitTimeInMilliSeconds;
    private int? currentBackoffInMilliSeconds;

    public SessionTokenMismatchRetryPolicy(int waitTimeInMilliSeconds = 5000)
    {
      this.durationTimer.Start();
      this.retryCount = 0;
      this.waitTimeInMilliSeconds = waitTimeInMilliSeconds;
      this.currentBackoffInMilliSeconds = new int?();
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult result = ShouldRetryResult.NoRetry();
      if (exception is DocumentClientException documentClientException)
        result = this.ShouldRetryInternalAsync((HttpStatusCode?) documentClientException?.StatusCode, documentClientException?.GetSubStatus());
      return Task.FromResult<ShouldRetryResult>(result);
    }

    private ShouldRetryResult ShouldRetryInternalAsync(
      HttpStatusCode? statusCode,
      SubStatusCodes? subStatusCode)
    {
      if (statusCode.HasValue && statusCode.Value == HttpStatusCode.NotFound && subStatusCode.HasValue && subStatusCode.Value == SubStatusCodes.PartitionKeyRangeGone)
      {
        int val2 = this.waitTimeInMilliSeconds - Convert.ToInt32(this.durationTimer.Elapsed.TotalMilliseconds);
        if (val2 <= 0)
        {
          this.durationTimer.Stop();
          DefaultTrace.TraceInformation("SessionTokenMismatchRetryPolicy not retrying because it has exceeded the time limit. Retry count = {0}", (object) this.retryCount);
          return ShouldRetryResult.NoRetry();
        }
        TimeSpan backoffTime = TimeSpan.Zero;
        if (this.retryCount > 0)
        {
          if (!this.currentBackoffInMilliSeconds.HasValue)
            this.currentBackoffInMilliSeconds = new int?(SessionTokenMismatchRetryPolicy.sessionRetryInitialBackoffConfig.Value);
          backoffTime = TimeSpan.FromMilliseconds((double) Math.Min(this.currentBackoffInMilliSeconds.Value, val2));
          this.currentBackoffInMilliSeconds = new int?(Math.Min(this.currentBackoffInMilliSeconds.Value * 2, SessionTokenMismatchRetryPolicy.sessionRetryMaximumBackoffConfig.Value));
        }
        ++this.retryCount;
        DefaultTrace.TraceInformation("SessionTokenMismatchRetryPolicy will retry. Retry count = {0}. Backoff time = {1} ms", (object) this.retryCount, (object) backoffTime.Milliseconds);
        return ShouldRetryResult.RetryAfter(backoffTime);
      }
      this.durationTimer.Stop();
      return ShouldRetryResult.NoRetry();
    }
  }
}
