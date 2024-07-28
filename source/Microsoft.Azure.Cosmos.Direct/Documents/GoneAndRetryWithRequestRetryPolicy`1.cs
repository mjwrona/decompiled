// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GoneAndRetryWithRequestRetryPolicy`1
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class GoneAndRetryWithRequestRetryPolicy<TResponse> : 
    IRequestRetryPolicy<GoneAndRetryRequestRetryPolicyContext, DocumentServiceRequest, TResponse>,
    IRequestRetryPolicy<DocumentServiceRequest, TResponse>
    where TResponse : IRetriableResponse
  {
    private static readonly ThreadLocal<System.Random> Random = new ThreadLocal<System.Random>((Func<System.Random>) (() => new System.Random()));
    private const int defaultWaitTimeInMilliSeconds = 30000;
    private const int minExecutionTimeInMilliSeconds = 5000;
    private const int initialBackoffMilliSeconds = 1000;
    private const int backoffMultiplier = 2;
    private const int defaultMaximumBackoffTimeInMilliSeconds = 15000;
    private const int defaultInitialBackoffTimeForRetryWithInMilliseconds = 10;
    private const int defaultMaximumBackoffTimeForRetryWithInMilliseconds = 1000;
    private const int defaultRandomSaltForRetryWithInMilliseconds = 5;
    private const int minFailedReplicaCountToConsiderConnectivityIssue = 3;
    private readonly int maximumBackoffTimeInMilliSeconds;
    private readonly int maximumBackoffTimeInMillisecondsForRetryWith;
    private readonly int initialBackoffTimeInMilliSeconds;
    private readonly int initialBackoffTimeInMillisecondsForRetryWith;
    private readonly int? randomSaltForRetryWithMilliseconds;
    private Stopwatch durationTimer = new Stopwatch();
    private TimeSpan minBackoffForRegionReroute;
    private int attemptCount = 1;
    private int attemptCountInvalidPartition = 1;
    private int regionRerouteAttemptCount;
    private int? currentBackoffMilliseconds;
    private int? currentBackoffMillisecondsForRetryWith;
    private RetryWithException lastRetryWithException;
    private Exception previousException;
    private readonly int waitTimeInMilliseconds;
    private readonly int waitTimeInMillisecondsForRetryWith;
    private readonly bool detectConnectivityIssues;
    private readonly bool disableRetryWithPolicy;

    public GoneAndRetryWithRequestRetryPolicy(
      bool disableRetryWithPolicy,
      int? waitTimeInSecondsOverride = null,
      TimeSpan minBackoffForRegionReroute = default (TimeSpan),
      bool detectConnectivityIssues = false,
      RetryWithConfiguration retryWithConfiguration = null)
    {
      this.waitTimeInMilliseconds = !waitTimeInSecondsOverride.HasValue ? 30000 : waitTimeInSecondsOverride.Value * 1000;
      this.disableRetryWithPolicy = disableRetryWithPolicy;
      this.detectConnectivityIssues = detectConnectivityIssues;
      this.minBackoffForRegionReroute = minBackoffForRegionReroute;
      this.ExecuteContext.RemainingTimeInMsOnClientRequest = TimeSpan.FromMilliseconds((double) this.waitTimeInMilliseconds);
      this.ExecuteContext.TimeoutForInBackoffRetryPolicy = TimeSpan.Zero;
      this.initialBackoffTimeInMilliSeconds = 1000;
      int? nullable = (int?) retryWithConfiguration?.InitialRetryIntervalMilliseconds;
      this.initialBackoffTimeInMillisecondsForRetryWith = nullable ?? 10;
      this.maximumBackoffTimeInMilliSeconds = 15000;
      nullable = (int?) retryWithConfiguration?.MaximumRetryIntervalMilliseconds;
      this.maximumBackoffTimeInMillisecondsForRetryWith = nullable ?? 1000;
      nullable = (int?) retryWithConfiguration?.TotalWaitTimeMilliseconds;
      this.waitTimeInMillisecondsForRetryWith = nullable ?? this.waitTimeInMilliseconds;
      nullable = (int?) retryWithConfiguration?.RandomSaltMaxValueMilliseconds;
      this.randomSaltForRetryWithMilliseconds = new int?(nullable ?? 5);
      if (this.randomSaltForRetryWithMilliseconds.HasValue)
      {
        nullable = this.randomSaltForRetryWithMilliseconds;
        int num = 1;
        if (nullable.GetValueOrDefault() < num & nullable.HasValue)
          throw new ArgumentException("RandomSaltMaxValueMilliseconds must be a number greater than 1 or null");
      }
      this.durationTimer.Start();
    }

    public void OnBeforeSendRequest(DocumentServiceRequest request)
    {
    }

    public GoneAndRetryRequestRetryPolicyContext ExecuteContext { get; } = new GoneAndRetryRequestRetryPolicyContext();

    public bool TryHandleResponseSynchronously(
      DocumentServiceRequest request,
      TResponse response,
      Exception exception,
      out ShouldRetryResult shouldRetryResult)
    {
      Exception exception1 = (Exception) null;
      TimeSpan backoffTime = TimeSpan.FromSeconds(0.0);
      TimeSpan.FromSeconds(0.0);
      request.RequestContext.IsRetry = true;
      bool flag1 = false;
      if (!GoneAndRetryWithRequestRetryPolicy<TResponse>.IsBaseGone(response, exception) && !(exception is RetryWithException) && (!GoneAndRetryWithRequestRetryPolicy<TResponse>.IsPartitionIsMigrating(response, exception) || request.ServiceIdentity != null && !request.ServiceIdentity.IsMasterService) && (!GoneAndRetryWithRequestRetryPolicy<TResponse>.IsInvalidPartition(response, exception) || request.PartitionKeyRangeIdentity != null && request.PartitionKeyRangeIdentity.CollectionRid != null) && (!GoneAndRetryWithRequestRetryPolicy<TResponse>.IsPartitionKeySplitting(response, exception) || request.ServiceIdentity != null))
      {
        this.durationTimer.Stop();
        shouldRetryResult = ShouldRetryResult.NoRetry();
        return true;
      }
      if (exception is RetryWithException)
      {
        if (this.disableRetryWithPolicy)
        {
          DefaultTrace.TraceWarning("The GoneAndRetryWithRequestRetryPolicy is configured with disableRetryWithPolicy to true. Retries on 449(RetryWith) exceptions has been disabled. This is by design to allow users to handle the exception: {0}", (object) exception.ToStringWithMessageAndData());
          this.durationTimer.Stop();
          shouldRetryResult = ShouldRetryResult.NoRetry();
          return true;
        }
        flag1 = true;
        this.lastRetryWithException = exception as RetryWithException;
      }
      int num1 = !flag1 ? this.waitTimeInMilliseconds - Convert.ToInt32(this.durationTimer.Elapsed.TotalMilliseconds) : this.waitTimeInMillisecondsForRetryWith - Convert.ToInt32(this.durationTimer.Elapsed.TotalMilliseconds);
      int val2 = num1 > 0 ? num1 : 0;
      int attemptCount = this.attemptCount;
      if (this.attemptCount++ > 1)
      {
        if (val2 <= 0)
        {
          if (GoneAndRetryWithRequestRetryPolicy<TResponse>.IsBaseGone(response, exception) || GoneAndRetryWithRequestRetryPolicy<TResponse>.IsPartitionIsMigrating(response, exception) || GoneAndRetryWithRequestRetryPolicy<TResponse>.IsInvalidPartition(response, exception) || GoneAndRetryWithRequestRetryPolicy<TResponse>.IsPartitionKeyRangeGone(response, exception) || GoneAndRetryWithRequestRetryPolicy<TResponse>.IsPartitionKeySplitting(response, exception))
          {
            string str1 = exception?.GetType().Name;
            HttpStatusCode statusCode;
            if (str1 == null)
            {
              if ((object) response == null)
              {
                str1 = (string) null;
              }
              else
              {
                statusCode = response.StatusCode;
                str1 = statusCode.ToString();
              }
            }
            string str2 = string.Format("Received {0} after backoff/retry", (object) str1);
            if (this.lastRetryWithException != null)
            {
              object[] objArray = new object[3]
              {
                (object) str2,
                null,
                null
              };
              string str3 = exception != null ? exception.ToStringWithData() : (string) null;
              if (str3 == null)
              {
                if ((object) response == null)
                {
                  str3 = (string) null;
                }
                else
                {
                  statusCode = response.StatusCode;
                  str3 = statusCode.ToString();
                }
              }
              objArray[1] = (object) str3;
              objArray[2] = (object) this.lastRetryWithException.ToStringWithData();
              DefaultTrace.TraceError("{0} including at least one RetryWithException. Will fail the request with RetryWithException. Exception: {1}. RetryWithException: {2}", objArray);
              exception1 = (Exception) this.lastRetryWithException;
            }
            else
            {
              object[] objArray = new object[2]
              {
                (object) str2,
                null
              };
              string str4 = exception != null ? exception.ToStringWithData() : (string) null;
              if (str4 == null)
              {
                if ((object) response == null)
                {
                  str4 = (string) null;
                }
                else
                {
                  statusCode = response.StatusCode;
                  str4 = statusCode.ToString();
                }
              }
              objArray[1] = (object) str4;
              DefaultTrace.TraceError("{0}. Will fail the request. {1}", objArray);
              SubStatusCodes forGoneRetryPolicy = DocumentClientException.GetExceptionSubStatusForGoneRetryPolicy(exception);
              if (forGoneRetryPolicy == SubStatusCodes.TimeoutGenerated410 && this.previousException != null)
                forGoneRetryPolicy = DocumentClientException.GetExceptionSubStatusForGoneRetryPolicy(this.previousException);
              exception1 = !this.detectConnectivityIssues || request.RequestContext.ClientRequestStatistics == null || !request.RequestContext.ClientRequestStatistics.IsCpuHigh.GetValueOrDefault(false) ? (!this.detectConnectivityIssues || request.RequestContext.ClientRequestStatistics == null || !request.RequestContext.ClientRequestStatistics.IsCpuThreadStarvation.GetValueOrDefault(false) ? (!this.detectConnectivityIssues || request.RequestContext.ClientRequestStatistics == null || request.RequestContext.ClientRequestStatistics.FailedReplicas.Count < 3 ? (Exception) ServiceUnavailableException.Create(new SubStatusCodes?(forGoneRetryPolicy), exception) : (Exception) new ServiceUnavailableException(string.Format(RMResources.ClientUnavailable, (object) request.RequestContext.ClientRequestStatistics.FailedReplicas.Count, (object) (request.RequestContext.ClientRequestStatistics.RegionsContacted.Count == 0 ? 1 : request.RequestContext.ClientRequestStatistics.RegionsContacted.Count)), exception, forGoneRetryPolicy)) : (Exception) new ServiceUnavailableException(string.Format(RMResources.ClientCpuThreadStarvation, (object) request.RequestContext.ClientRequestStatistics.FailedReplicas.Count, (object) (request.RequestContext.ClientRequestStatistics.RegionsContacted.Count == 0 ? 1 : request.RequestContext.ClientRequestStatistics.RegionsContacted.Count)), SubStatusCodes.Client_ThreadStarvation)) : (Exception) new ServiceUnavailableException(string.Format(RMResources.ClientCpuOverload, (object) request.RequestContext.ClientRequestStatistics.FailedReplicas.Count, (object) (request.RequestContext.ClientRequestStatistics.RegionsContacted.Count == 0 ? 1 : request.RequestContext.ClientRequestStatistics.RegionsContacted.Count)), SubStatusCodes.Client_CPUOverload);
            }
          }
          else
            DefaultTrace.TraceError("Received retry with exception after backoff/retry. Will fail the request. {0}", (object) ((exception != null ? exception.ToStringWithData() : (string) null) ?? response?.StatusCode.ToString()));
          this.durationTimer.Stop();
          shouldRetryResult = ShouldRetryResult.NoRetry(exception1);
          return true;
        }
        int? nullable = this.currentBackoffMillisecondsForRetryWith;
        this.currentBackoffMillisecondsForRetryWith = new int?(nullable ?? this.initialBackoffTimeInMillisecondsForRetryWith);
        nullable = this.currentBackoffMilliseconds;
        this.currentBackoffMilliseconds = new int?(nullable ?? this.initialBackoffTimeInMilliSeconds);
        if (flag1)
        {
          int val1 = this.currentBackoffMillisecondsForRetryWith.Value;
          if (this.randomSaltForRetryWithMilliseconds.HasValue)
            val1 += GoneAndRetryWithRequestRetryPolicy<TResponse>.Random.Value.Next(1, this.randomSaltForRetryWithMilliseconds.Value);
          backoffTime = TimeSpan.FromMilliseconds((double) Math.Min(Math.Min(val1, val2), this.maximumBackoffTimeInMillisecondsForRetryWith));
          this.currentBackoffMillisecondsForRetryWith = new int?(Math.Min(this.currentBackoffMillisecondsForRetryWith.Value * 2, this.maximumBackoffTimeInMillisecondsForRetryWith));
        }
        else
        {
          backoffTime = TimeSpan.FromMilliseconds((double) Math.Min(Math.Min(this.currentBackoffMilliseconds.Value, val2), this.maximumBackoffTimeInMilliSeconds));
          this.currentBackoffMilliseconds = new int?(Math.Min(this.currentBackoffMilliseconds.Value * 2, this.maximumBackoffTimeInMilliSeconds));
        }
      }
      double num2 = (double) val2 - backoffTime.TotalMilliseconds;
      TimeSpan timeSpan = num2 > 0.0 ? TimeSpan.FromMilliseconds(num2) : TimeSpan.FromMilliseconds(5000.0);
      if (backoffTime >= this.minBackoffForRegionReroute)
        ++this.regionRerouteAttemptCount;
      bool flag2;
      if (GoneAndRetryWithRequestRetryPolicy<TResponse>.IsBaseGone(response, exception))
        flag2 = true;
      else if (GoneAndRetryWithRequestRetryPolicy<TResponse>.IsPartitionIsMigrating(response, exception))
      {
        GoneAndRetryWithRequestRetryPolicy<TResponse>.ClearRequestContext(request);
        request.ForceCollectionRoutingMapRefresh = true;
        request.ForceMasterRefresh = true;
        flag2 = false;
      }
      else if (GoneAndRetryWithRequestRetryPolicy<TResponse>.IsInvalidPartition(response, exception))
      {
        GoneAndRetryWithRequestRetryPolicy<TResponse>.ClearRequestContext(request);
        request.RequestContext.GlobalCommittedSelectedLSN = -1L;
        if (this.attemptCountInvalidPartition++ > 2)
        {
          SubStatusCodes forGoneRetryPolicy = DocumentClientException.GetExceptionSubStatusForGoneRetryPolicy(exception);
          DefaultTrace.TraceCritical("Received second InvalidPartitionException after backoff/retry. Will fail the request. {0}", (object) ((exception != null ? exception.ToStringWithData() : (string) null) ?? response?.StatusCode.ToString()));
          shouldRetryResult = ShouldRetryResult.NoRetry((Exception) ServiceUnavailableException.Create(new SubStatusCodes?(forGoneRetryPolicy), exception));
          return true;
        }
        if (request != null)
        {
          request.ForceNameCacheRefresh = true;
          flag2 = false;
        }
        else
        {
          DefaultTrace.TraceCritical("Received unexpected invalid collection exception, request should be non-null. {0}", (object) ((exception != null ? exception.ToStringWithData() : (string) null) ?? response?.StatusCode.ToString()));
          shouldRetryResult = ShouldRetryResult.NoRetry((Exception) new InternalServerErrorException(exception));
          return true;
        }
      }
      else if (GoneAndRetryWithRequestRetryPolicy<TResponse>.IsPartitionKeySplitting(response, exception))
      {
        GoneAndRetryWithRequestRetryPolicy<TResponse>.ClearRequestContext(request);
        request.ForcePartitionKeyRangeRefresh = true;
        flag2 = false;
      }
      else
        flag2 = false;
      DefaultTrace.TraceWarning("GoneAndRetryWithRequestRetryPolicy Received exception, will retry, attempt: {0}, regionRerouteAttempt: {1}, backoffTime: {2}, Timeout: {3}, Exception: {4}", (object) this.attemptCount, (object) this.regionRerouteAttemptCount, (object) backoffTime, (object) timeSpan, (object) ((exception != null ? exception.ToStringWithData() : (string) null) ?? response?.StatusCode.ToString()));
      shouldRetryResult = ShouldRetryResult.RetryAfter(backoffTime);
      this.previousException = exception;
      this.ExecuteContext.ForceRefresh = flag2;
      this.ExecuteContext.IsInRetry = true;
      this.ExecuteContext.RemainingTimeInMsOnClientRequest = timeSpan;
      this.ExecuteContext.ClientRetryCount = attemptCount;
      this.ExecuteContext.RegionRerouteAttemptCount = this.regionRerouteAttemptCount;
      this.ExecuteContext.TimeoutForInBackoffRetryPolicy = backoffTime;
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

    private static bool IsBaseGone(TResponse response, Exception exception)
    {
      if (exception is GoneException)
        return true;
      if (((object) response != null ? (response.StatusCode == HttpStatusCode.Gone ? 1 : 0) : 0) == 0)
        return false;
      if (((object) response != null ? (response.SubStatusCode == SubStatusCodes.Unknown ? 1 : 0) : 0) != 0)
        return true;
      return (object) response != null && response.SubStatusCode.IsSDKGeneratedSubStatus();
    }

    private static bool IsPartitionIsMigrating(TResponse response, Exception exception)
    {
      if (exception is PartitionIsMigratingException)
        return true;
      return ((object) response != null ? (response.StatusCode == HttpStatusCode.Gone ? 1 : 0) : 0) != 0 && (object) response != null && response.SubStatusCode == SubStatusCodes.CompletingPartitionMigration;
    }

    private static bool IsInvalidPartition(TResponse response, Exception exception)
    {
      if (exception is InvalidPartitionException)
        return true;
      return ((object) response != null ? (response.StatusCode == HttpStatusCode.Gone ? 1 : 0) : 0) != 0 && (object) response != null && response.SubStatusCode == SubStatusCodes.NameCacheIsStale;
    }

    private static bool IsPartitionKeySplitting(TResponse response, Exception exception)
    {
      if (exception is PartitionKeyRangeIsSplittingException)
        return true;
      return ((object) response != null ? (response.StatusCode == HttpStatusCode.Gone ? 1 : 0) : 0) != 0 && (object) response != null && response.SubStatusCode == SubStatusCodes.CompletingSplit;
    }

    private static bool IsPartitionKeyRangeGone(TResponse response, Exception exception)
    {
      if (exception is PartitionKeyRangeGoneException)
        return true;
      return ((object) response != null ? (response.StatusCode == HttpStatusCode.Gone ? 1 : 0) : 0) != 0 && (object) response != null && response.SubStatusCode == SubStatusCodes.PartitionKeyRangeGone;
    }

    private static void ClearRequestContext(DocumentServiceRequest request)
    {
      request.RequestContext.TargetIdentity = (ServiceIdentity) null;
      request.RequestContext.ResolvedPartitionKeyRange = (PartitionKeyRange) null;
      request.RequestContext.QuorumSelectedLSN = -1L;
      request.RequestContext.UpdateQuorumSelectedStoreResponse((StoreResult) null);
    }
  }
}
