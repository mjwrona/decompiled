// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.PartitionKeyRangeCache
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Routing
{
  internal class PartitionKeyRangeCache : IRoutingMapProvider, ICollectionRoutingMapCache
  {
    private const string PageSizeString = "-1";
    private readonly AsyncCache<string, CollectionRoutingMap> routingMapCache;
    private readonly IAuthorizationTokenProvider authorizationTokenProvider;
    private readonly IStoreModel storeModel;
    private readonly CollectionCache collectionCache;

    public PartitionKeyRangeCache(
      IAuthorizationTokenProvider authorizationTokenProvider,
      IStoreModel storeModel,
      CollectionCache collectionCache)
    {
      this.routingMapCache = new AsyncCache<string, CollectionRoutingMap>((IEqualityComparer<CollectionRoutingMap>) EqualityComparer<CollectionRoutingMap>.Default, (IEqualityComparer<string>) StringComparer.Ordinal);
      this.authorizationTokenProvider = authorizationTokenProvider;
      this.storeModel = storeModel;
      this.collectionCache = collectionCache;
    }

    public async Task<IReadOnlyList<PartitionKeyRange>> TryGetOverlappingRangesAsync(
      string collectionRid,
      Range<string> range,
      bool forceRefresh = false)
    {
      CollectionRoutingMap previousValue = await this.TryLookupAsync(collectionRid, (CollectionRoutingMap) null, (DocumentServiceRequest) null, CancellationToken.None);
      if (forceRefresh && previousValue != null)
        previousValue = await this.TryLookupAsync(collectionRid, previousValue, (DocumentServiceRequest) null, CancellationToken.None);
      if (previousValue != null)
        return previousValue.GetOverlappingRanges(range);
      DefaultTrace.TraceInformation(string.Format("Routing Map Null for collection: {0} for range: {1}, forceRefresh:{2}", (object) collectionRid, (object) range.ToString(), (object) forceRefresh));
      return (IReadOnlyList<PartitionKeyRange>) null;
    }

    public async Task<PartitionKeyRange> TryGetPartitionKeyRangeByIdAsync(
      string collectionResourceId,
      string partitionKeyRangeId,
      bool forceRefresh = false)
    {
      CollectionRoutingMap previousValue = await this.TryLookupAsync(collectionResourceId, (CollectionRoutingMap) null, (DocumentServiceRequest) null, CancellationToken.None);
      if (forceRefresh && previousValue != null)
        previousValue = await this.TryLookupAsync(collectionResourceId, previousValue, (DocumentServiceRequest) null, CancellationToken.None);
      if (previousValue != null)
        return previousValue.TryGetRangeByPartitionKeyRangeId(partitionKeyRangeId);
      DefaultTrace.TraceInformation(string.Format("Routing Map Null for collection: {0}, PartitionKeyRangeId: {1}, forceRefresh:{2}", (object) collectionResourceId, (object) partitionKeyRangeId, (object) forceRefresh));
      return (PartitionKeyRange) null;
    }

    public async Task<CollectionRoutingMap> TryLookupAsync(
      string collectionRid,
      CollectionRoutingMap previousValue,
      DocumentServiceRequest request,
      CancellationToken cancellationToken)
    {
      try
      {
        return await this.routingMapCache.GetAsync(collectionRid, previousValue, (Func<Task<CollectionRoutingMap>>) (() => this.GetRoutingMapForCollectionAsync(collectionRid, previousValue, cancellationToken)), CancellationToken.None);
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

    public async Task<PartitionKeyRange> TryGetRangeByPartitionKeyRangeId(
      string collectionRid,
      string partitionKeyRangeId)
    {
      try
      {
        return (await this.routingMapCache.GetAsync(collectionRid, (CollectionRoutingMap) null, (Func<Task<CollectionRoutingMap>>) (() => this.GetRoutingMapForCollectionAsync(collectionRid, (CollectionRoutingMap) null, CancellationToken.None)), CancellationToken.None)).TryGetRangeByPartitionKeyRangeId(partitionKeyRangeId);
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
      CancellationToken cancellationToken)
    {
      List<PartitionKeyRange> ranges = new List<PartitionKeyRange>();
      string changeFeedNextIfNoneMatch = previousRoutingMap == null ? (string) null : previousRoutingMap.ChangeFeedNextIfNoneMatch;
      HttpStatusCode httpStatusCode = HttpStatusCode.OK;
      do
      {
        INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
        headers.Set("x-ms-max-item-count", "-1");
        headers.Set("A-IM", "Incremental Feed");
        if (changeFeedNextIfNoneMatch != null)
          headers.Set("If-None-Match", changeFeedNextIfNoneMatch);
        RetryOptions retryOptions = new RetryOptions();
        using (DocumentServiceResponse documentServiceResponse = await BackoffRetryUtility<DocumentServiceResponse>.ExecuteAsync((Func<Task<DocumentServiceResponse>>) (() => this.ExecutePartitionKeyRangeReadChangeFeed(collectionRid, headers)), (IRetryPolicy) new ResourceThrottleRetryPolicy(retryOptions.MaxRetryAttemptsOnThrottledRequests, retryOptions.MaxRetryWaitTimeInSeconds), cancellationToken))
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
      return collectionRoutingMap != null ? collectionRoutingMap : throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": GetRoutingMapForCollectionAsync(collectionRid: " + collectionRid + "), Range information either doesn't exist or is not complete.");
    }

    private async Task<DocumentServiceResponse> ExecutePartitionKeyRangeReadChangeFeed(
      string collectionRid,
      INameValueCollection headers)
    {
      DocumentServiceResponse documentServiceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.ReadFeed, collectionRid, ResourceType.PartitionKeyRange, AuthorizationTokenType.PrimaryMasterKey, headers))
      {
        string str = (string) null;
        try
        {
          str = this.authorizationTokenProvider.GetUserAuthorizationToken(request.ResourceAddress, PathsHelper.GetResourcePath(request.ResourceType), "GET", request.Headers, AuthorizationTokenType.PrimaryMasterKey, out string _);
        }
        catch (UnauthorizedException ex)
        {
        }
        if (str == null)
          str = this.authorizationTokenProvider.GetUserAuthorizationToken((await this.collectionCache.ResolveCollectionAsync(request, CancellationToken.None)).AltLink, PathsHelper.GetResourcePath(request.ResourceType), "GET", request.Headers, AuthorizationTokenType.PrimaryMasterKey, out string _);
        request.Headers["authorization"] = str;
        using (new ActivityScope(Guid.NewGuid()))
          documentServiceResponse = await this.storeModel.ProcessMessageAsync(request);
      }
      return documentServiceResponse;
    }
  }
}
