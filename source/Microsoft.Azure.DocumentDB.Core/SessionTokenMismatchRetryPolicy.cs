// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SessionTokenMismatchRetryPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
    private const int defaultMaximumBackoffTimeInMilliseconds = 50;
    private const int backoffMultiplier = 2;
    private int retryCount;
    private Stopwatch durationTimer = new Stopwatch();
    private int waitTimeInMilliSeconds;
    private int initialBackoffTimeInMilliseconds = 5;
    private int maximumBackoffTimeInMilliseconds = 50;
    private int currentBackoffInMilliSeconds;

    public SessionTokenMismatchRetryPolicy(int waitTimeInMilliSeconds = 5000)
    {
      this.durationTimer.Start();
      this.retryCount = 0;
      this.waitTimeInMilliSeconds = waitTimeInMilliSeconds;
      string environmentVariable1 = Environment.GetEnvironmentVariable("AZURE_COSMOS_SESSION_RETRY_INITIAL_BACKOFF");
      if (!string.IsNullOrWhiteSpace(environmentVariable1))
      {
        int result;
        if (int.TryParse(environmentVariable1, out result) && result >= 0)
          this.initialBackoffTimeInMilliseconds = result;
        else
          DefaultTrace.TraceCritical("The value of AZURE_COSMOS_SESSION_RETRY_INITIAL_BACKOFF is invalid.  Value: {0}", (object) result);
      }
      string environmentVariable2 = Environment.GetEnvironmentVariable("AZURE_COSMOS_SESSION_RETRY_MAXIMUM_BACKOFF");
      if (!string.IsNullOrWhiteSpace(environmentVariable2))
      {
        int result;
        if (int.TryParse(environmentVariable2, out result) && result >= 0)
          this.maximumBackoffTimeInMilliseconds = result;
        else
          DefaultTrace.TraceCritical("The value of AZURE_COSMOS_SESSION_RETRY_MAXIMUM_BACKOFF is invalid.  Value: {0}", (object) result);
      }
      this.currentBackoffInMilliSeconds = this.initialBackoffTimeInMilliseconds;
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
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
          backoffTime = TimeSpan.FromMilliseconds((double) Math.Min(this.currentBackoffInMilliSeconds, val2));
          this.currentBackoffInMilliSeconds = Math.Min(this.currentBackoffInMilliSeconds * 2, this.maximumBackoffTimeInMilliseconds);
        }
        ++this.retryCount;
        DefaultTrace.TraceInformation("SessionTokenMismatchRetryPolicy will retry. Retry count = {0}.  Backoff time = {1} ms", (object) this.retryCount, (object) backoffTime.Milliseconds);
        return ShouldRetryResult.RetryAfter(backoffTime);
      }
      this.durationTimer.Stop();
      DefaultTrace.TraceInformation("SessionTokenMismatchRetryPolicy not retrying because StatusCode or SubStatusCode not found.");
      return ShouldRetryResult.NoRetry();
    }
  }
}
