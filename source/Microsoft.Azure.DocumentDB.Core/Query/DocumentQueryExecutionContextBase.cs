// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.DocumentQueryExecutionContextBase
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal abstract class DocumentQueryExecutionContextBase : 
    IDocumentQueryExecutionContext,
    IDisposable
  {
    public static readonly FeedResponse<object> EmptyFeedResponse = new FeedResponse<object>(Enumerable.Empty<object>(), Enumerable.Empty<object>().Count<object>(), (INameValueCollection) new DictionaryNameValueCollection());
    protected SqlQuerySpec querySpec;
    private readonly IDocumentQueryClient client;
    private readonly Microsoft.Azure.Documents.ResourceType resourceTypeEnum;
    private readonly Type resourceType;
    private readonly Expression expression;
    private readonly FeedOptions feedOptions;
    private readonly string resourceLink;
    private readonly bool getLazyFeedResponse;
    private bool isExpressionEvaluated;
    private FeedResponse<object> lastPage;
    private readonly Guid correlatedActivityId;

    protected DocumentQueryExecutionContextBase(
      DocumentQueryExecutionContextBase.InitParams initParams)
    {
      this.client = initParams.Client;
      this.resourceTypeEnum = initParams.ResourceTypeEnum;
      this.resourceType = initParams.ResourceType;
      this.expression = initParams.Expression;
      this.feedOptions = initParams.FeedOptions;
      this.resourceLink = initParams.ResourceLink;
      this.getLazyFeedResponse = initParams.GetLazyFeedResponse;
      this.correlatedActivityId = initParams.CorrelatedActivityId;
      this.isExpressionEvaluated = false;
    }

    public bool ShouldExecuteQueryRequest => this.QuerySpec != null;

    public IDocumentQueryClient Client => this.client;

    public Type ResourceType => this.resourceType;

    public Microsoft.Azure.Documents.ResourceType ResourceTypeEnum => this.resourceTypeEnum;

    public string ResourceLink => this.resourceLink;

    public int? MaxItemCount => this.feedOptions.MaxItemCount;

    public SqlQuerySpec QuerySpec
    {
      get
      {
        if (!this.isExpressionEvaluated)
        {
          this.querySpec = DocumentQueryEvaluator.Evaluate(this.expression);
          this.isExpressionEvaluated = true;
        }
        return this.querySpec;
      }
    }

    protected PartitionKeyInternal PartitionKeyInternal => this.feedOptions.PartitionKey != null ? this.feedOptions.PartitionKey.InternalKey : (PartitionKeyInternal) null;

    protected int MaxBufferedItemCount => this.feedOptions.MaxBufferedItemCount;

    protected int MaxDegreeOfParallelism => this.feedOptions.MaxDegreeOfParallelism;

    protected string PartitionKeyRangeId => this.feedOptions.PartitionKeyRangeId;

    protected virtual string ContinuationToken => this.lastPage != null ? this.lastPage.ResponseContinuation : this.feedOptions.RequestContinuation;

    public virtual bool IsDone => this.lastPage != null && string.IsNullOrEmpty(this.lastPage.ResponseContinuation);

    public Guid CorrelatedActivityId => this.correlatedActivityId;

    public async Task<PartitionedQueryExecutionInfo> GetPartitionedQueryExecutionInfoAsync(
      PartitionKeyDefinition partitionKeyDefinition,
      bool requireFormattableOrderByQuery,
      bool isContinuationExpected,
      bool allowNonValueAggregateQuery,
      bool hasLogicalPartitionKey,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return (await this.client.GetQueryPartitionProviderAsync(cancellationToken)).GetPartitionedQueryExecutionInfo(this.QuerySpec, partitionKeyDefinition, requireFormattableOrderByQuery, isContinuationExpected, allowNonValueAggregateQuery, hasLogicalPartitionKey);
    }

    public virtual async Task<FeedResponse<object>> ExecuteNextAsync(
      CancellationToken cancellationToken)
    {
      if (this.IsDone)
        throw new InvalidOperationException(RMResources.DocumentQueryExecutionContextIsDone);
      this.lastPage = await this.ExecuteInternalAsync(cancellationToken);
      return this.lastPage;
    }

    public FeedOptions GetFeedOptions(string continuationToken) => new FeedOptions(this.feedOptions)
    {
      RequestContinuation = continuationToken
    };

    public async Task<INameValueCollection> CreateCommonHeadersAsync(FeedOptions feedOptions)
    {
      INameValueCollection requestHeaders = (INameValueCollection) new DictionaryNameValueCollection();
      ConsistencyLevel defaultConsistencyLevel = await this.client.GetDefaultConsistencyLevelAsync();
      ConsistencyLevel? consistencyLevelAsync = await this.client.GetDesiredConsistencyLevelAsync();
      if (!string.IsNullOrEmpty(feedOptions.SessionToken) && !ReplicatedResourceClient.IsReadingFromMaster(this.resourceTypeEnum, Microsoft.Azure.Documents.OperationType.ReadFeed) && (defaultConsistencyLevel == ConsistencyLevel.Session || consistencyLevelAsync.HasValue && consistencyLevelAsync.Value == ConsistencyLevel.Session))
        requestHeaders["x-ms-session-token"] = feedOptions.SessionToken;
      requestHeaders["x-ms-continuation"] = feedOptions.RequestContinuation;
      requestHeaders["x-ms-documentdb-isquery"] = bool.TrueString;
      int? nullable1;
      if (feedOptions.MaxItemCount.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        nullable1 = feedOptions.MaxItemCount;
        string str = nullable1.ToString();
        nameValueCollection["x-ms-max-item-count"] = str;
      }
      requestHeaders["x-ms-documentdb-query-enablecrosspartition"] = feedOptions.EnableCrossPartitionQuery.ToString();
      if (feedOptions.MaxDegreeOfParallelism != 0)
        requestHeaders["x-ms-documentdb-query-parallelizecrosspartitionquery"] = bool.TrueString;
      bool? nullable2;
      if (this.feedOptions.EnableScanInQuery.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        nullable2 = this.feedOptions.EnableScanInQuery;
        string str = nullable2.ToString();
        nameValueCollection["x-ms-documentdb-query-enable-scan"] = str;
      }
      nullable2 = this.feedOptions.EmitVerboseTracesInQuery;
      if (nullable2.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        nullable2 = this.feedOptions.EmitVerboseTracesInQuery;
        string str = nullable2.ToString();
        nameValueCollection["x-ms-documentdb-query-emit-traces"] = str;
      }
      nullable2 = this.feedOptions.EnableLowPrecisionOrderBy;
      if (nullable2.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        nullable2 = this.feedOptions.EnableLowPrecisionOrderBy;
        string str = nullable2.ToString();
        nameValueCollection["x-ms-documentdb-query-enable-low-precision-order-by"] = str;
      }
      if (!string.IsNullOrEmpty(this.feedOptions.FilterBySchemaResourceId))
        requestHeaders["x-ms-documentdb-filterby-schema-rid"] = this.feedOptions.FilterBySchemaResourceId;
      nullable1 = this.feedOptions.ResponseContinuationTokenLimitInKb;
      if (nullable1.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        nullable1 = this.feedOptions.ResponseContinuationTokenLimitInKb;
        string str = nullable1.ToString();
        nameValueCollection["x-ms-documentdb-responsecontinuationtokenlimitinkb"] = str;
      }
      if (this.feedOptions.DisableRUPerMinuteUsage)
        requestHeaders["x-ms-documentdb-disable-ru-per-minute-usage"] = bool.TrueString;
      if (this.feedOptions.ConsistencyLevel.HasValue)
      {
        await this.client.EnsureValidOverwrite(feedOptions.ConsistencyLevel.Value);
        requestHeaders.Set("x-ms-consistency-level", this.feedOptions.ConsistencyLevel.Value.ToString());
      }
      else if (consistencyLevelAsync.HasValue)
        requestHeaders.Set("x-ms-consistency-level", consistencyLevelAsync.Value.ToString());
      if (this.feedOptions.EnumerationDirection.HasValue)
        requestHeaders.Set("x-ms-enumeration-direction", this.feedOptions.EnumerationDirection.Value.ToString());
      if (this.feedOptions.ReadFeedKeyType.HasValue)
        requestHeaders.Set("x-ms-read-key-type", this.feedOptions.ReadFeedKeyType.Value.ToString());
      if (this.feedOptions.StartId != null)
        requestHeaders.Set("x-ms-start-id", this.feedOptions.StartId);
      if (this.feedOptions.EndId != null)
        requestHeaders.Set("x-ms-end-id", this.feedOptions.EndId);
      if (this.feedOptions.StartEpk != null)
        requestHeaders.Set("x-ms-start-epk", this.feedOptions.StartEpk);
      if (this.feedOptions.EndEpk != null)
        requestHeaders.Set("x-ms-end-epk", this.feedOptions.EndEpk);
      if (this.feedOptions.PopulateQueryMetrics)
        requestHeaders["x-ms-documentdb-populatequerymetrics"] = bool.TrueString;
      if (this.feedOptions.ForceQueryScan)
        requestHeaders["x-ms-documentdb-force-query-scan"] = bool.TrueString;
      if (this.feedOptions.ContentSerializationFormat.HasValue)
        requestHeaders["x-ms-documentdb-content-serialization-format"] = this.feedOptions.ContentSerializationFormat.Value.ToString();
      if (this.feedOptions.MergeStaticId != null)
        requestHeaders.Set("x-ms-cosmos-merge-static-id", this.feedOptions.MergeStaticId);
      return requestHeaders;
    }

    public DocumentServiceRequest CreateDocumentServiceRequest(
      INameValueCollection requestHeaders,
      SqlQuerySpec querySpec,
      PartitionKeyInternal partitionKey)
    {
      DocumentServiceRequest documentServiceRequest = this.CreateDocumentServiceRequest(requestHeaders, querySpec);
      this.PopulatePartitionKeyInfo(documentServiceRequest, partitionKey);
      return documentServiceRequest;
    }

    public DocumentServiceRequest CreateDocumentServiceRequest(
      INameValueCollection requestHeaders,
      SqlQuerySpec querySpec,
      PartitionKeyRange targetRange,
      string collectionRid)
    {
      DocumentServiceRequest documentServiceRequest = this.CreateDocumentServiceRequest(requestHeaders, querySpec);
      this.PopulatePartitionKeyRangeInfo(documentServiceRequest, targetRange, collectionRid);
      return documentServiceRequest;
    }

    public Task<FeedResponse<object>> ExecuteRequestAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return !this.ShouldExecuteQueryRequest ? this.ExecuteReadFeedRequestAsync(request, retryPolicyInstance, cancellationToken) : this.ExecuteQueryRequestAsync(request, retryPolicyInstance, cancellationToken);
    }

    public Task<FeedResponse<T>> ExecuteRequestAsync<T>(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return !this.ShouldExecuteQueryRequest ? this.ExecuteReadFeedRequestAsync<T>(request, retryPolicyInstance, cancellationToken) : this.ExecuteQueryRequestAsync<T>(request, retryPolicyInstance, cancellationToken);
    }

    public async Task<FeedResponse<object>> ExecuteQueryRequestAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return this.GetFeedResponse(await this.ExecuteQueryRequestInternalAsync(request, retryPolicyInstance, cancellationToken));
    }

    public async Task<FeedResponse<T>> ExecuteQueryRequestAsync<T>(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return this.GetFeedResponse<T>(await this.ExecuteQueryRequestInternalAsync(request, retryPolicyInstance, cancellationToken));
    }

    public async Task<FeedResponse<object>> ExecuteReadFeedRequestAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return this.GetFeedResponse(await this.client.ReadFeedAsync(request, retryPolicyInstance, cancellationToken));
    }

    public async Task<FeedResponse<T>> ExecuteReadFeedRequestAsync<T>(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return this.GetFeedResponse<T>(await this.client.ReadFeedAsync(request, retryPolicyInstance, cancellationToken));
    }

    public void PopulatePartitionKeyRangeInfo(
      DocumentServiceRequest request,
      PartitionKeyRange range,
      string collectionRid)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (range == null)
        throw new ArgumentNullException(nameof (range));
      if (!this.resourceTypeEnum.IsPartitioned())
        return;
      request.RouteTo(new PartitionKeyRangeIdentity(collectionRid, range.Id));
    }

    public async Task<PartitionKeyRange> GetTargetPartitionKeyRangeById(
      string collectionResourceId,
      string partitionKeyRangeId)
    {
      PartitionKeyRange range = await (await this.client.GetRoutingMapProviderAsync()).TryGetPartitionKeyRangeByIdAsync(collectionResourceId, partitionKeyRangeId);
      if (range == null && PathsHelper.IsNameBased(this.resourceLink))
        (await this.Client.GetCollectionCacheAsync()).Refresh(this.resourceLink);
      return range != null ? range : throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": GetTargetPartitionKeyRangeById(collectionResourceId:" + collectionResourceId + ", partitionKeyRangeId: " + partitionKeyRangeId + ") failed due to stale cache");
    }

    public async Task<List<PartitionKeyRange>> GetTargetPartitionKeyRanges(
      string collectionResourceId,
      List<Range<string>> providedRanges)
    {
      List<PartitionKeyRange> ranges = await (await this.client.GetRoutingMapProviderAsync()).TryGetOverlappingRangesAsync(collectionResourceId, (IEnumerable<Range<string>>) providedRanges);
      if (ranges == null && PathsHelper.IsNameBased(this.resourceLink))
        (await this.Client.GetCollectionCacheAsync()).Refresh(this.resourceLink);
      return ranges != null ? ranges : throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": GetTargetPartitionKeyRanges(collectionResourceId:" + collectionResourceId + ", providedRanges: " + string.Join<Range<string>>(",", (IEnumerable<Range<string>>) providedRanges) + " failed due to stale cache");
    }

    public abstract void Dispose();

    protected abstract Task<FeedResponse<object>> ExecuteInternalAsync(
      CancellationToken cancellationToken);

    protected async Task<List<PartitionKeyRange>> GetReplacementRanges(
      PartitionKeyRange targetRange,
      string collectionRid)
    {
      List<PartitionKeyRange> list = (await (await this.client.GetRoutingMapProviderAsync()).TryGetOverlappingRangesAsync(collectionRid, targetRange.ToRange(), true)).ToList<PartitionKeyRange>();
      string minInclusive = list.First<PartitionKeyRange>().MinInclusive;
      string maxExclusive = list.Last<PartitionKeyRange>().MaxExclusive;
      if (!minInclusive.Equals(targetRange.MinInclusive, StringComparison.Ordinal) || !maxExclusive.Equals(targetRange.MaxExclusive, StringComparison.Ordinal))
        throw new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Target range and Replacement range has mismatched min/max. Target range: [{0}, {1}). Replacement range: [{2}, {3}).", (object) targetRange.MinInclusive, (object) targetRange.MaxExclusive, (object) minInclusive, (object) maxExclusive));
      return list;
    }

    protected bool NeedPartitionKeyRangeCacheRefresh(DocumentClientException ex)
    {
      HttpStatusCode? statusCode = ex.StatusCode;
      HttpStatusCode httpStatusCode = HttpStatusCode.Gone;
      return statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue && ex.GetSubStatus() == SubStatusCodes.PartitionKeyRangeGone;
    }

    private async Task<DocumentServiceResponse> ExecuteQueryRequestInternalAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      DocumentServiceResponse documentServiceResponse;
      try
      {
        documentServiceResponse = await this.client.ExecuteQueryAsync(request, retryPolicyInstance, cancellationToken);
      }
      finally
      {
        request.Body.Position = 0L;
      }
      return documentServiceResponse;
    }

    private DocumentServiceRequest CreateDocumentServiceRequest(
      INameValueCollection requestHeaders,
      SqlQuerySpec querySpec)
    {
      DocumentServiceRequest documentServiceRequest = querySpec != null ? this.CreateQueryDocumentServiceRequest(requestHeaders, querySpec) : this.CreateReadFeedDocumentServiceRequest(requestHeaders);
      if (this.feedOptions.JsonSerializerSettings != null)
        documentServiceRequest.SerializerSettings = this.feedOptions.JsonSerializerSettings;
      return documentServiceRequest;
    }

    private DocumentServiceRequest CreateQueryDocumentServiceRequest(
      INameValueCollection requestHeaders,
      SqlQuerySpec querySpec)
    {
      DocumentServiceRequest documentServiceRequest;
      string s;
      switch (this.client.QueryCompatibilityMode)
      {
        case QueryCompatibilityMode.SqlQuery:
          if (querySpec.Parameters != null && querySpec.Parameters.Count > 0)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unsupported argument in query compatibility mode '{0}'", (object) this.client.QueryCompatibilityMode), "querySpec.Parameters");
          documentServiceRequest = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.SqlQuery, this.resourceTypeEnum, this.resourceLink, AuthorizationTokenType.PrimaryMasterKey, requestHeaders);
          documentServiceRequest.Headers["Content-Type"] = "application/sql";
          s = querySpec.QueryText;
          break;
        default:
          documentServiceRequest = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Query, this.resourceTypeEnum, this.resourceLink, AuthorizationTokenType.PrimaryMasterKey, requestHeaders);
          documentServiceRequest.Headers["Content-Type"] = "application/query+json";
          s = JsonConvert.SerializeObject((object) querySpec);
          break;
      }
      documentServiceRequest.Body = (Stream) new MemoryStream(Encoding.UTF8.GetBytes(s));
      return documentServiceRequest;
    }

    private DocumentServiceRequest CreateReadFeedDocumentServiceRequest(
      INameValueCollection requestHeaders)
    {
      return this.resourceTypeEnum == Microsoft.Azure.Documents.ResourceType.Database || this.resourceTypeEnum == Microsoft.Azure.Documents.ResourceType.Offer || this.resourceTypeEnum == Microsoft.Azure.Documents.ResourceType.Snapshot ? DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.ReadFeed, (string) null, this.resourceTypeEnum, AuthorizationTokenType.PrimaryMasterKey, requestHeaders) : DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.ReadFeed, this.resourceTypeEnum, this.resourceLink, AuthorizationTokenType.PrimaryMasterKey, requestHeaders);
    }

    private void PopulatePartitionKeyInfo(
      DocumentServiceRequest request,
      PartitionKeyInternal partitionKey)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (!this.resourceTypeEnum.IsPartitioned() || partitionKey == null)
        return;
      request.Headers["x-ms-documentdb-partitionkey"] = partitionKey.ToJsonString();
    }

    private FeedResponse<object> GetFeedResponse(DocumentServiceResponse response) => this.GetFeedResponse<object>(response);

    private FeedResponse<T> GetFeedResponse<T>(DocumentServiceResponse response)
    {
      int itemCount = 0;
      long length = response.ResponseBody.CanSeek ? response.ResponseBody.Length : 0L;
      return new FeedResponse<T>(response.GetQueryResponse<T>(this.resourceType, this.getLazyFeedResponse, out itemCount), itemCount, response.Headers, partitionedClientSideRequestStatistics: response.RequestStats != null ? PartitionedClientSideRequestStatistics.CreateFromSingleRequest(string.Empty, response.RequestStats) : (PartitionedClientSideRequestStatistics) null, responseLengthBytes: length);
    }

    public struct InitParams
    {
      public IDocumentQueryClient Client { get; }

      public Microsoft.Azure.Documents.ResourceType ResourceTypeEnum { get; }

      public Type ResourceType { get; }

      public Expression Expression { get; }

      public FeedOptions FeedOptions { get; }

      public string ResourceLink { get; }

      public bool GetLazyFeedResponse { get; }

      public Guid CorrelatedActivityId { get; }

      public InitParams(
        IDocumentQueryClient client,
        Microsoft.Azure.Documents.ResourceType resourceTypeEnum,
        Type resourceType,
        Expression expression,
        FeedOptions feedOptions,
        string resourceLink,
        bool getLazyFeedResponse,
        Guid correlatedActivityId)
      {
        if (client == null)
          throw new ArgumentNullException("client can not be null.");
        if ((object) resourceType == null)
          throw new ArgumentNullException("resourceType can not be null.");
        if (expression == null)
          throw new ArgumentNullException("expression can not be null.");
        if (feedOptions == null)
          throw new ArgumentNullException("feedOptions can not be null.");
        if (correlatedActivityId == Guid.Empty)
          throw new ArgumentException("correlatedActivityId can not be empty.");
        this.Client = client;
        this.ResourceTypeEnum = resourceTypeEnum;
        this.ResourceType = resourceType;
        this.Expression = expression;
        this.FeedOptions = feedOptions;
        this.ResourceLink = resourceLink;
        this.GetLazyFeedResponse = getLazyFeedResponse;
        this.CorrelatedActivityId = correlatedActivityId;
      }
    }
  }
}
