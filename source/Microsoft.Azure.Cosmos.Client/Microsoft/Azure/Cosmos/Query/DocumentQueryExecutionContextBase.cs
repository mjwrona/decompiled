// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.DocumentQueryExecutionContextBase
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
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

namespace Microsoft.Azure.Cosmos.Query
{
  internal abstract class DocumentQueryExecutionContextBase : 
    IDocumentQueryExecutionContext,
    IDisposable
  {
    public static readonly DocumentFeedResponse<object> EmptyFeedResponse = new DocumentFeedResponse<object>(Enumerable.Empty<object>(), Enumerable.Empty<object>().Count<object>(), (INameValueCollection) new RequestNameValueCollection());
    protected SqlQuerySpec querySpec;
    private readonly Expression expression;
    private readonly FeedOptions feedOptions;
    private readonly bool getLazyFeedResponse;
    private bool isExpressionEvaluated;
    private DocumentFeedResponse<CosmosElement> lastPage;

    protected DocumentQueryExecutionContextBase(
      DocumentQueryExecutionContextBase.InitParams initParams)
    {
      this.Client = initParams.Client;
      this.ResourceTypeEnum = initParams.ResourceTypeEnum;
      this.ResourceType = initParams.ResourceType;
      this.expression = initParams.Expression;
      this.feedOptions = initParams.FeedOptions;
      this.ResourceLink = initParams.ResourceLink;
      this.getLazyFeedResponse = initParams.GetLazyFeedResponse;
      this.CorrelatedActivityId = initParams.CorrelatedActivityId;
      this.isExpressionEvaluated = false;
    }

    public bool ShouldExecuteQueryRequest => this.QuerySpec != null;

    public IDocumentQueryClient Client { get; }

    public Type ResourceType { get; }

    public Microsoft.Azure.Documents.ResourceType ResourceTypeEnum { get; }

    public string ResourceLink { get; }

    public int? MaxItemCount => this.feedOptions.MaxItemCount;

    protected SqlQuerySpec QuerySpec
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

    protected virtual string ContinuationToken => this.lastPage != null ? this.lastPage.ResponseContinuation : this.feedOptions.RequestContinuationToken;

    public virtual bool IsDone => this.lastPage != null && string.IsNullOrEmpty(this.lastPage.ResponseContinuation);

    public Guid CorrelatedActivityId { get; }

    public async Task<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo> GetPartitionedQueryExecutionInfoAsync(
      PartitionKeyDefinition partitionKeyDefinition,
      bool requireFormattableOrderByQuery,
      bool isContinuationExpected,
      bool allowNonValueAggregateQuery,
      bool hasLogicalPartitionKey,
      bool allowDCount,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      TryCatch<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo> queryExecutionInfo = (await this.Client.GetQueryPartitionProviderAsync()).TryGetPartitionedQueryExecutionInfo(JsonConvert.SerializeObject((object) this.QuerySpec), partitionKeyDefinition, requireFormattableOrderByQuery, isContinuationExpected, allowNonValueAggregateQuery, hasLogicalPartitionKey, allowDCount, false);
      return queryExecutionInfo.Succeeded ? queryExecutionInfo.Result : throw new BadRequestException(queryExecutionInfo.Exception);
    }

    public virtual async Task<DocumentFeedResponse<CosmosElement>> ExecuteNextFeedResponseAsync(
      CancellationToken cancellationToken)
    {
      if (this.IsDone)
        throw new InvalidOperationException(RMResources.DocumentQueryExecutionContextIsDone);
      this.lastPage = await this.ExecuteInternalAsync(cancellationToken);
      return this.lastPage;
    }

    public FeedOptions GetFeedOptions(string continuationToken) => new FeedOptions(this.feedOptions)
    {
      RequestContinuationToken = continuationToken
    };

    public async Task<INameValueCollection> CreateCommonHeadersAsync(FeedOptions feedOptions)
    {
      INameValueCollection requestHeaders = (INameValueCollection) new RequestNameValueCollection();
      Microsoft.Azure.Cosmos.ConsistencyLevel defaultConsistencyLevel = (Microsoft.Azure.Cosmos.ConsistencyLevel) await this.Client.GetDefaultConsistencyLevelAsync();
      Microsoft.Azure.Documents.ConsistencyLevel? consistencyLevelAsync = await this.Client.GetDesiredConsistencyLevelAsync();
      Microsoft.Azure.Cosmos.ConsistencyLevel? nullable1 = consistencyLevelAsync.HasValue ? new Microsoft.Azure.Cosmos.ConsistencyLevel?((Microsoft.Azure.Cosmos.ConsistencyLevel) consistencyLevelAsync.GetValueOrDefault()) : new Microsoft.Azure.Cosmos.ConsistencyLevel?();
      if (!string.IsNullOrEmpty(feedOptions.SessionToken) && !ReplicatedResourceClient.IsReadingFromMaster(this.ResourceTypeEnum, OperationType.ReadFeed) && (defaultConsistencyLevel == Microsoft.Azure.Cosmos.ConsistencyLevel.Session || nullable1.HasValue && nullable1.Value == Microsoft.Azure.Cosmos.ConsistencyLevel.Session))
        requestHeaders["x-ms-session-token"] = feedOptions.SessionToken;
      requestHeaders["x-ms-continuation"] = feedOptions.RequestContinuationToken;
      requestHeaders["x-ms-documentdb-isquery"] = bool.TrueString;
      int? nullable2;
      if (feedOptions.MaxItemCount.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        nullable2 = feedOptions.MaxItemCount;
        string str = nullable2.ToString();
        nameValueCollection["x-ms-max-item-count"] = str;
      }
      requestHeaders["x-ms-documentdb-query-enablecrosspartition"] = feedOptions.EnableCrossPartitionQuery.ToString();
      if (feedOptions.MaxDegreeOfParallelism != 0)
        requestHeaders["x-ms-documentdb-query-parallelizecrosspartitionquery"] = bool.TrueString;
      bool? nullable3;
      if (this.feedOptions.EnableScanInQuery.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        nullable3 = this.feedOptions.EnableScanInQuery;
        string str = nullable3.ToString();
        nameValueCollection["x-ms-documentdb-query-enable-scan"] = str;
      }
      nullable3 = this.feedOptions.EmitVerboseTracesInQuery;
      if (nullable3.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        nullable3 = this.feedOptions.EmitVerboseTracesInQuery;
        string str = nullable3.ToString();
        nameValueCollection["x-ms-documentdb-query-emit-traces"] = str;
      }
      nullable3 = this.feedOptions.EnableLowPrecisionOrderBy;
      if (nullable3.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        nullable3 = this.feedOptions.EnableLowPrecisionOrderBy;
        string str = nullable3.ToString();
        nameValueCollection["x-ms-documentdb-query-enable-low-precision-order-by"] = str;
      }
      if (!string.IsNullOrEmpty(this.feedOptions.FilterBySchemaResourceId))
        requestHeaders["x-ms-documentdb-filterby-schema-rid"] = this.feedOptions.FilterBySchemaResourceId;
      nullable2 = this.feedOptions.ResponseContinuationTokenLimitInKb;
      if (nullable2.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        nullable2 = this.feedOptions.ResponseContinuationTokenLimitInKb;
        string str = nullable2.ToString();
        nameValueCollection["x-ms-documentdb-responsecontinuationtokenlimitinkb"] = str;
      }
      if (this.feedOptions.ConsistencyLevel.HasValue)
      {
        await this.Client.EnsureValidOverwriteAsync((Microsoft.Azure.Documents.ConsistencyLevel) feedOptions.ConsistencyLevel.Value, OperationType.ReadFeed, this.ResourceTypeEnum);
        requestHeaders.Set("x-ms-consistency-level", this.feedOptions.ConsistencyLevel.Value.ToString());
      }
      else if (nullable1.HasValue)
        requestHeaders.Set("x-ms-consistency-level", nullable1.Value.ToString());
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
      if (this.feedOptions.MergeStaticId != null)
        requestHeaders.Set("x-ms-cosmos-merge-static-id", this.feedOptions.MergeStaticId);
      if (this.feedOptions.CosmosSerializationFormatOptions != null)
        requestHeaders["x-ms-documentdb-content-serialization-format"] = this.feedOptions.CosmosSerializationFormatOptions.ContentSerializationFormat;
      else if (this.feedOptions.ContentSerializationFormat.HasValue)
        requestHeaders["x-ms-documentdb-content-serialization-format"] = this.feedOptions.ContentSerializationFormat.Value.ToString();
      INameValueCollection commonHeadersAsync = requestHeaders;
      requestHeaders = (INameValueCollection) null;
      return commonHeadersAsync;
    }

    public DocumentServiceRequest CreateDocumentServiceRequest(
      INameValueCollection requestHeaders,
      SqlQuerySpec querySpec,
      PartitionKeyInternal partitionKey)
    {
      DocumentServiceRequest documentServiceRequest = this.CreateDocumentServiceRequest(requestHeaders, querySpec);
      this.PopulatePartitionKeyInfo(documentServiceRequest, partitionKey);
      documentServiceRequest.Properties = this.feedOptions.Properties;
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
      documentServiceRequest.Properties = this.feedOptions.Properties;
      return documentServiceRequest;
    }

    public async Task<DocumentFeedResponse<CosmosElement>> ExecuteRequestLazyAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      DocumentServiceResponse documentServiceResponse = await this.ExecuteQueryRequestInternalAsync(request, retryPolicyInstance, cancellationToken);
      return this.GetFeedResponse(request, documentServiceResponse);
    }

    public async Task<DocumentFeedResponse<CosmosElement>> ExecuteRequestAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return await (this.ShouldExecuteQueryRequest ? this.ExecuteQueryRequestAsync(request, retryPolicyInstance, cancellationToken) : this.ExecuteReadFeedRequestAsync(request, retryPolicyInstance, cancellationToken));
    }

    public async Task<DocumentFeedResponse<T>> ExecuteRequestAsync<T>(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return await (this.ShouldExecuteQueryRequest ? this.ExecuteQueryRequestAsync<T>(request, retryPolicyInstance, cancellationToken) : this.ExecuteReadFeedRequestAsync<T>(request, retryPolicyInstance, cancellationToken));
    }

    public async Task<DocumentFeedResponse<CosmosElement>> ExecuteQueryRequestAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      DocumentServiceRequest documentServiceRequest = request;
      DocumentServiceResponse documentServiceResponse = await this.ExecuteQueryRequestInternalAsync(request, retryPolicyInstance, cancellationToken);
      return this.GetFeedResponse(documentServiceRequest, documentServiceResponse);
    }

    public async Task<DocumentFeedResponse<T>> ExecuteQueryRequestAsync<T>(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return this.GetFeedResponse<T>(await this.ExecuteQueryRequestInternalAsync(request, retryPolicyInstance, cancellationToken));
    }

    public async Task<DocumentFeedResponse<CosmosElement>> ExecuteReadFeedRequestAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      DocumentServiceRequest documentServiceRequest = request;
      DocumentServiceResponse documentServiceResponse = await this.Client.ReadFeedAsync(request, retryPolicyInstance, cancellationToken);
      return this.GetFeedResponse(documentServiceRequest, documentServiceResponse);
    }

    public async Task<DocumentFeedResponse<T>> ExecuteReadFeedRequestAsync<T>(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return this.GetFeedResponse<T>(await this.Client.ReadFeedAsync(request, retryPolicyInstance, cancellationToken));
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
      if (!this.ResourceTypeEnum.IsPartitioned())
        return;
      request.RouteTo(new PartitionKeyRangeIdentity(collectionRid, range.Id));
    }

    public async Task<PartitionKeyRange> GetTargetPartitionKeyRangeByIdAsync(
      string collectionResourceId,
      string partitionKeyRangeId)
    {
      PartitionKeyRange range = await (await this.Client.GetRoutingMapProviderAsync()).TryGetPartitionKeyRangeByIdAsync(collectionResourceId, partitionKeyRangeId, (ITrace) NoOpTrace.Singleton);
      if (range == null && PathsHelper.IsNameBased(this.ResourceLink))
        (await this.Client.GetCollectionCacheAsync()).Refresh(this.ResourceLink);
      PartitionKeyRange keyRangeByIdAsync = range != null ? range : throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": GetTargetPartitionKeyRangeById(collectionResourceId:" + collectionResourceId + ", partitionKeyRangeId: " + partitionKeyRangeId + ") failed due to stale cache");
      range = (PartitionKeyRange) null;
      return keyRangeByIdAsync;
    }

    internal Task<List<PartitionKeyRange>> GetTargetPartitionKeyRangesByEpkStringAsync(
      string collectionResourceId,
      string effectivePartitionKeyString)
    {
      return this.GetTargetPartitionKeyRangesAsync(collectionResourceId, new List<Range<string>>()
      {
        Range<string>.GetPointRange(effectivePartitionKeyString)
      });
    }

    internal async Task<List<PartitionKeyRange>> GetTargetPartitionKeyRangesAsync(
      string collectionResourceId,
      List<Range<string>> providedRanges)
    {
      if (string.IsNullOrEmpty(nameof (collectionResourceId)))
        throw new ArgumentNullException();
      if (providedRanges == null || !providedRanges.Any<Range<string>>())
        throw new ArgumentNullException(nameof (providedRanges));
      List<PartitionKeyRange> ranges = await (await this.Client.GetRoutingMapProviderAsync()).TryGetOverlappingRangesAsync(collectionResourceId, (IEnumerable<Range<string>>) providedRanges, (ITrace) NoOpTrace.Singleton);
      if (ranges == null && PathsHelper.IsNameBased(this.ResourceLink))
        (await this.Client.GetCollectionCacheAsync()).Refresh(this.ResourceLink);
      List<PartitionKeyRange> partitionKeyRangesAsync = ranges != null ? ranges : throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": GetTargetPartitionKeyRanges(collectionResourceId:" + collectionResourceId + ", providedRanges: " + string.Join<Range<string>>(",", (IEnumerable<Range<string>>) providedRanges) + " failed due to stale cache");
      ranges = (List<PartitionKeyRange>) null;
      return partitionKeyRangesAsync;
    }

    public abstract void Dispose();

    protected abstract Task<DocumentFeedResponse<CosmosElement>> ExecuteInternalAsync(
      CancellationToken cancellationToken);

    protected async Task<List<PartitionKeyRange>> GetReplacementRangesAsync(
      PartitionKeyRange targetRange,
      string collectionRid)
    {
      List<PartitionKeyRange> list = (await (await this.Client.GetRoutingMapProviderAsync()).TryGetOverlappingRangesAsync(collectionRid, targetRange.ToRange(), (ITrace) NoOpTrace.Singleton, true)).ToList<PartitionKeyRange>();
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
        documentServiceResponse = await this.Client.ExecuteQueryAsync(request, retryPolicyInstance, cancellationToken);
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
      switch (this.Client.QueryCompatibilityMode)
      {
        case QueryCompatibilityMode.SqlQuery:
          if (querySpec.Parameters != null && querySpec.Parameters.Count > 0)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unsupported argument in query compatibility mode '{0}'", (object) this.Client.QueryCompatibilityMode), "querySpec.Parameters");
          documentServiceRequest = DocumentServiceRequest.Create(OperationType.SqlQuery, this.ResourceTypeEnum, this.ResourceLink, AuthorizationTokenType.PrimaryMasterKey, requestHeaders);
          documentServiceRequest.Headers["Content-Type"] = "application/sql";
          s = querySpec.QueryText;
          break;
        default:
          documentServiceRequest = DocumentServiceRequest.Create(OperationType.Query, this.ResourceTypeEnum, this.ResourceLink, AuthorizationTokenType.PrimaryMasterKey, requestHeaders);
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
      return this.ResourceTypeEnum == Microsoft.Azure.Documents.ResourceType.Database || this.ResourceTypeEnum == Microsoft.Azure.Documents.ResourceType.Offer || this.ResourceTypeEnum == Microsoft.Azure.Documents.ResourceType.Snapshot ? DocumentServiceRequest.Create(OperationType.ReadFeed, (string) null, this.ResourceTypeEnum, AuthorizationTokenType.PrimaryMasterKey, requestHeaders) : DocumentServiceRequest.Create(OperationType.ReadFeed, this.ResourceTypeEnum, this.ResourceLink, AuthorizationTokenType.PrimaryMasterKey, requestHeaders);
    }

    private void PopulatePartitionKeyInfo(
      DocumentServiceRequest request,
      PartitionKeyInternal partitionKey)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (!this.ResourceTypeEnum.IsPartitioned() || partitionKey == null)
        return;
      request.Headers["x-ms-documentdb-partitionkey"] = partitionKey.ToJsonString();
    }

    private DocumentFeedResponse<T> GetFeedResponse<T>(DocumentServiceResponse response)
    {
      long length = response.ResponseBody.CanSeek ? response.ResponseBody.Length : 0L;
      int itemCount;
      return new DocumentFeedResponse<T>(response.GetQueryResponse<T>(this.ResourceType, this.getLazyFeedResponse, out itemCount), itemCount, response.Headers, response.RequestStats, length);
    }

    private DocumentFeedResponse<CosmosElement> GetFeedResponse(
      DocumentServiceRequest documentServiceRequest,
      DocumentServiceResponse documentServiceResponse)
    {
      MemoryStream destination = new MemoryStream();
      documentServiceResponse.ResponseBody.CopyTo((Stream) destination);
      long length = destination.Length;
      ArraySegment<byte> buffer;
      ReadOnlyMemory<byte> readOnlyMemory = !destination.TryGetBuffer(out buffer) ? (ReadOnlyMemory<byte>) destination.ToArray() : (ReadOnlyMemory<byte>) buffer;
      IJsonNavigator jsonNavigator;
      if (this.feedOptions.CosmosSerializationFormatOptions != null)
      {
        jsonNavigator = this.feedOptions.CosmosSerializationFormatOptions.CreateCustomNavigatorCallback(readOnlyMemory);
        if (jsonNavigator == null)
          throw new InvalidOperationException("The CosmosSerializationOptions did not return a JSON navigator.");
      }
      else
        jsonNavigator = JsonNavigator.Create(readOnlyMemory);
      string rootNodeName = this.GetRootNodeName(documentServiceRequest.ResourceType);
      ObjectProperty objectProperty;
      if (!jsonNavigator.TryGetObjectProperty(jsonNavigator.GetRootNode(), rootNodeName, out objectProperty))
        throw new InvalidOperationException("Response Body Contract was violated. QueryResponse did not have property: " + rootNodeName);
      IJsonNavigatorNode valueNode = objectProperty.ValueNode;
      if (!(CosmosElement.Dispatch(jsonNavigator, valueNode) is CosmosArray result))
        throw new InvalidOperationException("QueryResponse did not have an array of : " + rootNodeName);
      return new DocumentFeedResponse<CosmosElement>((IEnumerable<CosmosElement>) result, result.Count, documentServiceResponse.Headers, documentServiceResponse.RequestStats, length);
    }

    private string GetRootNodeName(Microsoft.Azure.Documents.ResourceType resourceType) => resourceType == Microsoft.Azure.Documents.ResourceType.Collection ? "DocumentCollections" : resourceType.ToResourceTypeString() + "s";

    public readonly struct InitParams
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
        if (resourceType == (Type) null)
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
