// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.WebExceptionRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class WebExceptionRetryPolicy : IRetryPolicy
  {
    private const int waitTimeInSeconds = 30;
    private const int initialBackoffSeconds = 1;
    private const int backoffMultiplier = 2;
    private ValueStopwatch durationTimer;
    private int attemptCount = 1;
    private int currentBackoffSeconds = 1;

    public WebExceptionRetryPolicy() => this.durationTimer.Start();

    public Task<ShouldRetryResult> ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      TimeSpan backoffTime = TimeSpan.FromSeconds(0.0);
      if (!WebExceptionUtility.IsWebExceptionRetriable(exception))
      {
        this.durationTimer.Stop();
        return Task.FromResult<ShouldRetryResult>(ShouldRetryResult.NoRetry());
      }
      if (this.attemptCount++ > 1)
      {
        int val2 = 30 - this.durationTimer.Elapsed.Seconds;
        if (val2 <= 0)
        {
          this.durationTimer.Stop();
          return Task.FromResult<ShouldRetryResult>(ShouldRetryResult.NoRetry());
        }
        backoffTime = TimeSpan.FromSeconds((double) Math.Min(this.currentBackoffSeconds, val2));
        this.currentBackoffSeconds *= 2;
      }
      DefaultTrace.TraceWarning("Received retriable web exception, will retry, {0}", (object) exception);
      return Task.FromResult<ShouldRetryResult>(ShouldRetryResult.RetryAfter(backoffTime));
    }

    public Task<ShouldRetryResult> ShouldRetryAsync(
      ResponseMessage cosmosResponseMessage,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}
