// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GoneAndRetryWithRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class GoneAndRetryWithRetryPolicy : 
    IRetryPolicy<bool>,
    IRetryPolicy<Tuple<bool, bool, TimeSpan>>,
    IRetryPolicy<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>
  {
    private const int defaultWaitTimeInSeconds = 30;
    private const int minExecutionTimeInSeconds = 5;
    private const int initialBackoffSeconds = 1;
    private const int backoffMultiplier = 2;
    private const int maximumBackoffTimeInSeconds = 15;
    private const int minFailedReplicaCountToConsiderConnectivityIssue = 3;
    private Stopwatch durationTimer = new Stopwatch();
    private int attemptCount = 1;
    private int attemptCountInvalidPartition = 1;
    private int regionRerouteAttemptCount;
    private TimeSpan minBackoffForRegionReroute;
    private RetryWithException lastRetryWithException;
    private readonly int waitTimeInSeconds;
    private readonly bool detectConnectivityIssues;
    private int currentBackoffSeconds = 1;
    private DocumentServiceRequest request;

    public GoneAndRetryWithRetryPolicy(
      DocumentServiceRequest request = null,
      int? waitTimeInSecondsOverride = null,
      TimeSpan minBackoffForRegionReroute = default (TimeSpan),
      bool detectConnectivityIssues = false)
    {
      this.waitTimeInSeconds = !waitTimeInSecondsOverride.HasValue ? 30 : waitTimeInSecondsOverride.Value;
      this.request = request;
      this.detectConnectivityIssues = detectConnectivityIssues;
      this.minBackoffForRegionReroute = minBackoffForRegionReroute;
      this.durationTimer.Start();
    }

    bool IRetryPolicy<bool>.InitialArgumentValue => false;

    Tuple<bool, bool, TimeSpan> IRetryPolicy<Tuple<bool, bool, TimeSpan>>.InitialArgumentValue => Tuple.Create<bool, bool, TimeSpan>(false, false, TimeSpan.FromSeconds((double) this.waitTimeInSeconds));

    Tuple<bool, bool, TimeSpan, int, int, TimeSpan> IRetryPolicy<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>.InitialArgumentValue => Tuple.Create<bool, bool, TimeSpan, int, int, TimeSpan>(false, false, TimeSpan.FromSeconds((double) this.waitTimeInSeconds), 0, 0, TimeSpan.Zero);

    async Task<ShouldRetryResult<bool>> IRetryPolicy<bool>.ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>> shouldRetryResult = await ((IRetryPolicy<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>) this).ShouldRetryAsync(exception, cancellationToken);
      return !shouldRetryResult.ShouldRetry ? ShouldRetryResult<bool>.NoRetry(shouldRetryResult.ExceptionToThrow) : ShouldRetryResult<bool>.RetryAfter(shouldRetryResult.BackoffTime, shouldRetryResult.PolicyArg1.Item1);
    }

    async Task<ShouldRetryResult<Tuple<bool, bool, TimeSpan>>> IRetryPolicy<Tuple<bool, bool, TimeSpan>>.ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>> shouldRetryResult = await ((IRetryPolicy<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>) this).ShouldRetryAsync(exception, cancellationToken);
      return !shouldRetryResult.ShouldRetry ? ShouldRetryResult<Tuple<bool, bool, TimeSpan>>.NoRetry(shouldRetryResult.ExceptionToThrow) : ShouldRetryResult<Tuple<bool, bool, TimeSpan>>.RetryAfter(shouldRetryResult.BackoffTime, Tuple.Create<bool, bool, TimeSpan>(shouldRetryResult.PolicyArg1.Item1, shouldRetryResult.PolicyArg1.Item2, shouldRetryResult.PolicyArg1.Item3));
    }

    Task<ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>> IRetryPolicy<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>.ShouldRetryAsync(
      Exception exception,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Exception exception1 = (Exception) null;
      TimeSpan backoffTime = TimeSpan.FromSeconds(0.0);
      TimeSpan.FromSeconds(0.0);
      if (!(exception is GoneException) && !(exception is RetryWithException) && (!(exception is PartitionIsMigratingException) || this.request.ServiceIdentity != null && !this.request.ServiceIdentity.IsMasterService) && (!(exception is InvalidPartitionException) || this.request.PartitionKeyRangeIdentity != null && this.request.PartitionKeyRangeIdentity.CollectionRid != null) && (!(exception is PartitionKeyRangeIsSplittingException) || this.request.ServiceIdentity != null))
      {
        this.durationTimer.Stop();
        return Task.FromResult<ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>>(ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>.NoRetry((Exception) null));
      }
      if (exception is RetryWithException)
        this.lastRetryWithException = exception as RetryWithException;
      int num1 = this.waitTimeInSeconds - Convert.ToInt32(this.durationTimer.Elapsed.TotalSeconds);
      int val2 = num1 > 0 ? num1 : 0;
      int attemptCount = this.attemptCount;
      if (this.attemptCount++ > 1)
      {
        if (val2 <= 0)
        {
          switch (exception)
          {
            case GoneException _:
            case PartitionIsMigratingException _:
            case InvalidPartitionException _:
            case PartitionKeyRangeGoneException _:
            case PartitionKeyRangeIsSplittingException _:
              string str = string.Format("Received {0} after backoff/retry", (object) exception.GetType().Name);
              if (this.lastRetryWithException != null)
              {
                DefaultTrace.TraceError("{0} including at least one RetryWithException. Will fail the request with RetryWithException. Exception: {1}. RetryWithException: {2}", (object) str, (object) exception.ToStringWithData(), (object) this.lastRetryWithException.ToStringWithData());
                exception1 = (Exception) this.lastRetryWithException;
                break;
              }
              DefaultTrace.TraceError("{0}. Will fail the request. {1}", (object) str, (object) exception.ToStringWithData());
              SubStatusCodes forGoneRetryPolicy = DocumentClientException.GetExceptionSubStatusForGoneRetryPolicy(exception);
              bool? nullable;
              if (this.detectConnectivityIssues && this.request.RequestContext.ClientRequestStatistics != null)
              {
                nullable = this.request.RequestContext.ClientRequestStatistics.IsCpuHigh;
                if (nullable.GetValueOrDefault(false))
                {
                  Exception exception2 = (Exception) new ServiceUnavailableException(string.Format(RMResources.ClientCpuOverload, (object) this.request.RequestContext.ClientRequestStatistics.FailedReplicas.Count, (object) (this.request.RequestContext.ClientRequestStatistics.RegionsContacted.Count == 0 ? 1 : this.request.RequestContext.ClientRequestStatistics.RegionsContacted.Count)), SubStatusCodes.Client_CPUOverload);
                }
              }
              if (this.detectConnectivityIssues && this.request.RequestContext.ClientRequestStatistics != null)
              {
                nullable = this.request.RequestContext.ClientRequestStatistics.IsCpuThreadStarvation;
                if (nullable.GetValueOrDefault(false))
                {
                  exception1 = (Exception) new ServiceUnavailableException(string.Format(RMResources.ClientCpuThreadStarvation, (object) this.request.RequestContext.ClientRequestStatistics.FailedReplicas.Count, (object) (this.request.RequestContext.ClientRequestStatistics.RegionsContacted.Count == 0 ? 1 : this.request.RequestContext.ClientRequestStatistics.RegionsContacted.Count)), SubStatusCodes.Client_ThreadStarvation);
                  break;
                }
              }
              exception1 = !this.detectConnectivityIssues || this.request.RequestContext.ClientRequestStatistics == null || this.request.RequestContext.ClientRequestStatistics.FailedReplicas.Count < 3 ? (Exception) ServiceUnavailableException.Create(new SubStatusCodes?(forGoneRetryPolicy), exception) : (Exception) new ServiceUnavailableException(string.Format(RMResources.ClientUnavailable, (object) this.request.RequestContext.ClientRequestStatistics.FailedReplicas.Count, (object) (this.request.RequestContext.ClientRequestStatistics.RegionsContacted.Count == 0 ? 1 : this.request.RequestContext.ClientRequestStatistics.RegionsContacted.Count)), exception, forGoneRetryPolicy);
              break;
            default:
              DefaultTrace.TraceError("Received retrywith exception after backoff/retry. Will fail the request. {0}", (object) exception.ToStringWithData());
              break;
          }
          this.durationTimer.Stop();
          return Task.FromResult<ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>>(ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>.NoRetry(exception1));
        }
        backoffTime = TimeSpan.FromSeconds((double) Math.Min(Math.Min(this.currentBackoffSeconds, val2), 15));
        this.currentBackoffSeconds *= 2;
      }
      double num2 = (double) val2 - backoffTime.TotalSeconds;
      TimeSpan timeSpan = num2 > 0.0 ? TimeSpan.FromSeconds(num2) : TimeSpan.FromSeconds(5.0);
      if (backoffTime >= this.minBackoffForRegionReroute)
        ++this.regionRerouteAttemptCount;
      bool flag;
      switch (exception)
      {
        case GoneException _:
          flag = true;
          break;
        case PartitionIsMigratingException _:
          this.ClearRequestContext();
          this.request.ForceCollectionRoutingMapRefresh = true;
          this.request.ForceMasterRefresh = true;
          flag = false;
          break;
        case InvalidPartitionException _:
          this.ClearRequestContext();
          this.request.RequestContext.GlobalCommittedSelectedLSN = -1L;
          if (this.attemptCountInvalidPartition++ > 2)
          {
            SubStatusCodes forGoneRetryPolicy = DocumentClientException.GetExceptionSubStatusForGoneRetryPolicy(exception);
            DefaultTrace.TraceCritical("Received second InvalidPartitionException after backoff/retry. Will fail the request. {0}", (object) exception.ToStringWithData());
            return Task.FromResult<ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>>(ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>.NoRetry((Exception) ServiceUnavailableException.Create(new SubStatusCodes?(forGoneRetryPolicy), exception)));
          }
          if (this.request != null)
          {
            this.request.ForceNameCacheRefresh = true;
            flag = false;
            break;
          }
          DefaultTrace.TraceCritical("Received unexpected invalid collection exception, request should be non-null.", (object) exception.ToStringWithData());
          return Task.FromResult<ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>>(ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>.NoRetry((Exception) new InternalServerErrorException(exception)));
        case PartitionKeyRangeIsSplittingException _:
          this.ClearRequestContext();
          this.request.ForcePartitionKeyRangeRefresh = true;
          flag = false;
          break;
        default:
          flag = false;
          break;
      }
      DefaultTrace.TraceWarning("GoneAndRetryWithRetryPolicy Received exception, will retry, attempt: {0}, regionRerouteAttempt: {1}, backoffTime: {2}, Timeout: {3}, Exception: {4}", (object) this.attemptCount, (object) this.regionRerouteAttemptCount, (object) backoffTime, (object) timeSpan, (object) exception.ToStringWithData());
      return Task.FromResult<ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>>(ShouldRetryResult<Tuple<bool, bool, TimeSpan, int, int, TimeSpan>>.RetryAfter(backoffTime, Tuple.Create<bool, bool, TimeSpan, int, int, TimeSpan>(flag, true, timeSpan, attemptCount, this.regionRerouteAttemptCount, backoffTime)));
    }

    private void ClearRequestContext()
    {
      this.request.RequestContext.TargetIdentity = (ServiceIdentity) null;
      this.request.RequestContext.ResolvedPartitionKeyRange = (PartitionKeyRange) null;
      this.request.RequestContext.QuorumSelectedLSN = -1L;
      this.request.RequestContext.UpdateQuorumSelectedStoreResponse((StoreResult) null);
    }
  }
}
