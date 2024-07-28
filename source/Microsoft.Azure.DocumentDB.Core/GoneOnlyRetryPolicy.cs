// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GoneOnlyRetryPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class GoneOnlyRetryPolicy : IRetryPolicy<Tuple<bool, bool, TimeSpan>>
  {
    private const int backoffMultiplier = 2;
    private const int initialBackoffTimeInSeconds = 1;
    private Stopwatch durationTimer = new Stopwatch();
    private readonly TimeSpan retryTimeout;
    private DocumentServiceRequest request;
    private int currentBackoffTimeInSeconds;
    private bool isInRetry;

    public GoneOnlyRetryPolicy(DocumentServiceRequest request, TimeSpan retryTimeout)
    {
      this.request = request;
      this.retryTimeout = retryTimeout;
      this.currentBackoffTimeInSeconds = 1;
      this.isInRetry = false;
      this.durationTimer.Start();
    }

    Tuple<bool, bool, TimeSpan> IRetryPolicy<Tuple<bool, bool, TimeSpan>>.InitialArgumentValue => Tuple.Create<bool, bool, TimeSpan>(false, false, this.retryTimeout);

    Task<ShouldRetryResult<Tuple<bool, bool, TimeSpan>>> IRetryPolicy<Tuple<bool, bool, TimeSpan>>.ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (!(exception is GoneException))
      {
        this.durationTimer.Stop();
        return Task.FromResult<ShouldRetryResult<Tuple<bool, bool, TimeSpan>>>(ShouldRetryResult<Tuple<bool, bool, TimeSpan>>.NoRetry((Exception) null));
      }
      TimeSpan elapsed = this.durationTimer.Elapsed;
      if (elapsed >= this.retryTimeout)
      {
        DefaultTrace.TraceInformation("GoneOnlyRetryPolicy - timeout {0}, elapsed {1}", (object) this.retryTimeout, (object) elapsed);
        this.durationTimer.Stop();
        return Task.FromResult<ShouldRetryResult<Tuple<bool, bool, TimeSpan>>>(ShouldRetryResult<Tuple<bool, bool, TimeSpan>>.NoRetry((Exception) new ServiceUnavailableException(exception)));
      }
      TimeSpan timeSpan = this.retryTimeout - elapsed;
      TimeSpan backoffTime = TimeSpan.Zero;
      if (this.isInRetry)
      {
        backoffTime = TimeSpan.FromSeconds((double) this.currentBackoffTimeInSeconds);
        this.currentBackoffTimeInSeconds *= 2;
        if (backoffTime > timeSpan)
        {
          DefaultTrace.TraceInformation("GoneOnlyRetryPolicy - timeout {0}, elapsed {1}, backoffTime {2}", (object) this.retryTimeout, (object) elapsed, (object) backoffTime);
          this.durationTimer.Stop();
          return Task.FromResult<ShouldRetryResult<Tuple<bool, bool, TimeSpan>>>(ShouldRetryResult<Tuple<bool, bool, TimeSpan>>.NoRetry((Exception) new ServiceUnavailableException(exception)));
        }
      }
      else
        this.isInRetry = true;
      DefaultTrace.TraceInformation("GoneOnlyRetryPolicy - timeout {0}, elapsed {1}, backoffTime {2}, remainingTime {3}", (object) this.retryTimeout, (object) elapsed, (object) backoffTime, (object) timeSpan);
      return Task.FromResult<ShouldRetryResult<Tuple<bool, bool, TimeSpan>>>(ShouldRetryResult<Tuple<bool, bool, TimeSpan>>.RetryAfter(backoffTime, Tuple.Create<bool, bool, TimeSpan>(true, true, timeSpan)));
    }
  }
}
