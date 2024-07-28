// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.PartitionKeyRangeCache
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal class PartitionKeyRangeCache : IRoutingMapProvider, ICollectionRoutingMapCache
  {
    private const string PageSizeString = "-1";
    private readonly AsyncCacheNonBlocking<string, CollectionRoutingMap> routingMapCache;
    private readonly ICosmosAuthorizationTokenProvider authorizationTokenProvider;
    private readonly IStoreModel storeModel;
    private readonly CollectionCache collectionCache;

    public PartitionKeyRangeCache(
      ICosmosAuthorizationTokenProvider authorizationTokenProvider,
      IStoreModel storeModel,
      CollectionCache collectionCache)
    {
      this.routingMapCache = new AsyncCacheNonBlocking<string, CollectionRoutingMap>(keyEqualityComparer: (IEqualityComparer<string>) StringComparer.Ordinal);
      this.authorizationTokenProvider = authorizationTokenProvider;
      this.storeModel = storeModel;
      this.collectionCache = collectionCache;
    }

    public virtual async Task<IReadOnlyList<PartitionKeyRange>> TryGetOverlappingRangesAsync(
      string collectionRid,
      Range<string> range,
      ITrace trace,
      bool forceRefresh = false)
    {
      using (ITrace childTrace = trace.StartChild("Try Get Overlapping Ranges", TraceComponent.Routing, TraceLevel.Info))
      {
        CollectionRoutingMap previousValue = await this.TryLookupAsync(collectionRid, (CollectionRoutingMap) null, (DocumentServiceRequest) null, childTrace);
        if (forceRefresh && previousValue != null)
          previousValue = await this.TryLookupAsync(collectionRid, previousValue, (DocumentServiceRequest) null, childTrace);
        if (previousValue != null)
          return previousValue.GetOverlappingRanges(range);
        DefaultTrace.TraceWarning(string.Format("Routing Map Null for collection: {0} for range: {1}, forceRefresh:{2}", (object) collectionRid, (object) range.ToString(), (object) forceRefresh));
        return (IReadOnlyList<PartitionKeyRange>) null;
      }
    }

    public virtual async Task<PartitionKeyRange> TryGetPartitionKeyRangeByIdAsync(
      string collectionResourceId,
      string partitionKeyRangeId,
      ITrace trace,
      bool forceRefresh = false)
    {
      CollectionRoutingMap previousValue = await this.TryLookupAsync(collectionResourceId, (CollectionRoutingMap) null, (DocumentServiceRequest) null, trace);
      if (forceRefresh && previousValue != null)
        previousValue = await this.TryLookupAsync(collectionResourceId, previousValue, (DocumentServiceRequest) null, trace);
      if (previousValue != null)
        return previousValue.TryGetRangeByPartitionKeyRangeId(partitionKeyRangeId);
      DefaultTrace.TraceInformation(string.Format("Routing Map Null for collection: {0}, PartitionKeyRangeId: {1}, forceRefresh:{2}", (object) collectionResourceId, (object) partitionKeyRangeId, (object) forceRefresh));
      return (PartitionKeyRange) null;
    }

    public virtual async Task<CollectionRoutingMap> TryLookupAsync(
      string collectionRid,
      CollectionRoutingMap previousValue,
      DocumentServiceRequest request,
      ITrace trace)
    {
      try
      {
        return await this.routingMapCache.GetAsync(collectionRid, (Func<CollectionRoutingMap, Task<CollectionRoutingMap>>) (_ => this.GetRoutingMapForCollectionAsync(collectionRid, previousValue, trace, request?.RequestContext?.ClientRequestStatistics)), (Func<CollectionRoutingMap, bool>) (currentValue => PartitionKeyRangeCache.ShouldForceRefresh(previousValue, currentValue)));
      }
      catch (DocumentClientException ex)
      {
        if (previousValue != null)
        {
          StringBuilder stringBuilder = new StringBuilder();
          foreach (PartitionKeyRange partitionKeyRange in (IEnumerable<PartitionKeyRange>) previousValue.OrderedPartitionKeyRanges)
          {
            stringBuilder.Append(partitionKeyRange.ToRange().ToString());
            stringBuilder.Append(", ");
          }
          DefaultTrace.TraceInformation(string.Format("DocumentClientException in TryLookupAsync Collection: {0}, previousValue: {1} Exception: {2}", (object) collectionRid, (object) stringBuilder.ToString(), (object) ex.ToString()));
        }
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
        if (statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue)
          return (CollectionRoutingMap) null;
        throw;
      }
    }

    private static bool ShouldForceRefresh(
      CollectionRoutingMap previousValue,
      CollectionRoutingMap currentValue)
    {
      return previousValue != null && currentValue != null && previousValue.ChangeFeedNextIfNoneMatch == currentValue.ChangeFeedNextIfNoneMatch;
    }

    public async Task<PartitionKeyRange> TryGetRangeByPartitionKeyRangeIdAsync(
      string collectionRid,
      string partitionKeyRangeId,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics)
    {
      try
      {
        return (await this.routingMapCache.GetAsync(collectionRid, (Func<CollectionRoutingMap, Task<CollectionRoutingMap>>) (_ => this.GetRoutingMapForCollectionAsync(collectionRid, (CollectionRoutingMap) null, trace, clientSideRequestStatistics)), (Func<CollectionRoutingMap, bool>) (_ => false))).TryGetRangeByPartitionKeyRangeId(partitionKeyRangeId);
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
        if (statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue)
          return (PartitionKeyRange) null;
        throw;
      }
    }

    private async Task<CollectionRoutingMap> GetRoutingMapForCollectionAsync(
      string collectionRid,
      CollectionRoutingMap previousRoutingMap,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics)
    {
      List<PartitionKeyRange> ranges = new List<PartitionKeyRange>();
      string changeFeedNextIfNoneMatch = previousRoutingMap?.ChangeFeedNextIfNoneMatch;
      HttpStatusCode httpStatusCode = HttpStatusCode.OK;
      do
      {
        INameValueCollection headers = (INameValueCollection) new RequestNameValueCollection();
        headers.Set("x-ms-max-item-count", "-1");
        headers.Set("A-IM", "Incremental Feed");
        if (changeFeedNextIfNoneMatch != null)
          headers.Set("If-None-Match", changeFeedNextIfNoneMatch);
        RetryOptions retryOptions = new RetryOptions();
        using (DocumentServiceResponse documentServiceResponse = await BackoffRetryUtility<DocumentServiceResponse>.ExecuteAsync((Func<Task<DocumentServiceResponse>>) (() => this.ExecutePartitionKeyRangeReadChangeFeedAsync(collectionRid, headers, trace, clientSideRequestStatistics)), (IRetryPolicy) new ResourceThrottleRetryPolicy(retryOptions.MaxRetryAttemptsOnThrottledRequests, retryOptions.MaxRetryWaitTimeInSeconds)))
        {
          httpStatusCode = documentServiceResponse.StatusCode;
          changeFeedNextIfNoneMatch = documentServiceResponse.Headers["etag"];
          FeedResource<PartitionKeyRange> resource = documentServiceResponse.GetResource<FeedResource<PartitionKeyRange>>();
          if (resource != null)
            ranges.AddRange((IEnumerable<PartitionKeyRange>) resource);
        }
      }
      while (httpStatusCode != HttpStatusCode.NotModified);
      IEnumerable<Tuple<PartitionKeyRange, ServiceIdentity>> tuples = ranges.Select<PartitionKeyRange, Tuple<PartitionKeyRange, ServiceIdentity>>((Func<PartitionKeyRange, Tuple<PartitionKeyRange, ServiceIdentity>>) (range => Tuple.Create<PartitionKeyRange, ServiceIdentity>(range, (ServiceIdentity) null)));
      CollectionRoutingMap collectionRoutingMap;
      if (previousRoutingMap == null)
      {
        HashSet<string> goneRanges = new HashSet<string>(ranges.SelectMany<PartitionKeyRange, string>((Func<PartitionKeyRange, IEnumerable<string>>) (range => (IEnumerable<string>) range.Parents ?? Enumerable.Empty<string>())));
        collectionRoutingMap = CollectionRoutingMap.TryCreateCompleteRoutingMap(tuples.Where<Tuple<PartitionKeyRange, ServiceIdentity>>((Func<Tuple<PartitionKeyRange, ServiceIdentity>, bool>) (tuple => !goneRanges.Contains(tuple.Item1.Id))), string.Empty, changeFeedNextIfNoneMatch);
      }
      else
        collectionRoutingMap = previousRoutingMap.TryCombine(tuples, changeFeedNextIfNoneMatch);
      if (collectionRoutingMap == null)
        throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": GetRoutingMapForCollectionAsync(collectionRid: " + collectionRid + "), Range information either doesn't exist or is not complete.");
      trace.AddDatum("PKRangeCache Info(" + previousRoutingMap?.ChangeFeedNextIfNoneMatch + "#" + DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ")", (TraceDatum) new PartitionKeyRangeCacheTraceDatum(previousRoutingMap?.ChangeFeedNextIfNoneMatch, collectionRoutingMap.ChangeFeedNextIfNoneMatch));
      CollectionRoutingMap forCollectionAsync = collectionRoutingMap;
      ranges = (List<PartitionKeyRange>) null;
      return forCollectionAsync;
    }

    private async Task<DocumentServiceResponse> ExecutePartitionKeyRangeReadChangeFeedAsync(
      string collectionRid,
      INameValueCollection headers,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics)
    {
      DocumentServiceResponse documentServiceResponse;
      using (ITrace childTrace = trace.StartChild("Read PartitionKeyRange Change Feed", TraceComponent.Transport, TraceLevel.Info))
      {
        using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.ReadFeed, collectionRid, ResourceType.PartitionKeyRange, AuthorizationTokenType.PrimaryMasterKey, headers))
        {
          string authorizationToken = (string) null;
          try
          {
            authorizationToken = await this.authorizationTokenProvider.GetUserAuthorizationTokenAsync(request.ResourceAddress, PathsHelper.GetResourcePath(request.ResourceType), "GET", request.Headers, AuthorizationTokenType.PrimaryMasterKey, childTrace);
          }
          catch (UnauthorizedException ex)
          {
          }
          request.Headers["authorization"] = authorizationToken != null ? authorizationToken : throw new NotSupportedException("Resource tokens are not supported");
          request.RequestContext.ClientRequestStatistics = clientSideRequestStatistics ?? (IClientSideRequestStatistics) new ClientSideRequestStatisticsTraceDatum(DateTime.UtcNow, trace.Summary);
          if (clientSideRequestStatistics == null)
            childTrace.AddDatum("Client Side Request Stats", (object) request.RequestContext.ClientRequestStatistics);
          using (new ActivityScope(Guid.NewGuid()))
          {
            try
            {
              documentServiceResponse = await this.storeModel.ProcessMessageAsync(request);
            }
            catch (DocumentClientException ex)
            {
              childTrace.AddDatum("Exception Message", (object) ex.Message);
              throw;
            }
          }
        }
      }
      return documentServiceResponse;
    }
  }
}
