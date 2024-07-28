// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.QueryIterator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.ExecutionContext;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query
{
  internal sealed class QueryIterator : FeedIteratorInternal
  {
    private static readonly string CorrelatedActivityIdKeyName = "Query Correlated ActivityId";
    private static readonly IReadOnlyList<CosmosElement> EmptyPage = (IReadOnlyList<CosmosElement>) new List<CosmosElement>();
    private readonly CosmosQueryContextCore cosmosQueryContext;
    private readonly IQueryPipelineStage queryPipelineStage;
    private readonly CosmosSerializationFormatOptions cosmosSerializationFormatOptions;
    private readonly RequestOptions requestOptions;
    private readonly CosmosClientContext clientContext;
    private readonly Guid correlatedActivityId;
    private bool hasMoreResults;

    private QueryIterator(
      CosmosQueryContextCore cosmosQueryContext,
      IQueryPipelineStage cosmosQueryExecutionContext,
      CosmosSerializationFormatOptions cosmosSerializationFormatOptions,
      RequestOptions requestOptions,
      CosmosClientContext clientContext,
      Guid correlatedActivityId,
      ContainerInternal container)
    {
      this.cosmosQueryContext = cosmosQueryContext ?? throw new ArgumentNullException(nameof (cosmosQueryContext));
      this.queryPipelineStage = cosmosQueryExecutionContext ?? throw new ArgumentNullException(nameof (cosmosQueryExecutionContext));
      this.cosmosSerializationFormatOptions = cosmosSerializationFormatOptions;
      this.requestOptions = requestOptions;
      this.clientContext = clientContext ?? throw new ArgumentNullException(nameof (clientContext));
      this.hasMoreResults = true;
      this.correlatedActivityId = correlatedActivityId;
      this.container = container;
    }

    public static QueryIterator Create(
      ContainerCore containerCore,
      CosmosQueryClient client,
      CosmosClientContext clientContext,
      SqlQuerySpec sqlQuerySpec,
      string continuationToken,
      FeedRangeInternal feedRangeInternal,
      QueryRequestOptions queryRequestOptions,
      string resourceLink,
      bool isContinuationExpected,
      bool allowNonValueAggregateQuery,
      bool forcePassthrough,
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      if (queryRequestOptions == null)
        queryRequestOptions = new QueryRequestOptions();
      Guid correlatedActivityId1 = Guid.NewGuid();
      CosmosQueryClient client1 = client;
      Type resourceType = typeof (QueryResponseCore);
      string resourceLink1 = resourceLink;
      bool flag1 = isContinuationExpected;
      bool flag2 = allowNonValueAggregateQuery;
      bool flag3 = QueryIterator.IsSystemPrefixExpected(queryRequestOptions);
      Guid correlatedActivityId2 = correlatedActivityId1;
      int num1 = flag1 ? 1 : 0;
      int num2 = flag2 ? 1 : 0;
      int num3 = flag3 ? 1 : 0;
      CosmosQueryContextCore cosmosQueryContext = new CosmosQueryContextCore(client1, ResourceType.Document, OperationType.Query, resourceType, resourceLink1, correlatedActivityId2, num1 != 0, num2 != 0, num3 != 0);
      DocumentContainer documentContainer = new DocumentContainer((IMonadicDocumentContainer) new NetworkAttachedDocumentContainer((ContainerInternal) containerCore, client, correlatedActivityId1, queryRequestOptions));
      ExecutionEnvironment? executionEnvironment = queryRequestOptions.ExecutionEnvironment;
      CosmosElement initialUserContinuationToken;
      switch (executionEnvironment.GetValueOrDefault(ExecutionEnvironment.Client))
      {
        case ExecutionEnvironment.Client:
          if (continuationToken != null)
          {
            TryCatch<CosmosElement> tryCatch = CosmosElement.Monadic.Parse(continuationToken);
            if (tryCatch.Failed)
              return new QueryIterator(cosmosQueryContext, (IQueryPipelineStage) new FaultedQueryPipelineStage((Exception) new MalformedContinuationTokenException("Malformed Continuation Token: " + continuationToken, tryCatch.Exception)), queryRequestOptions.CosmosSerializationFormatOptions, (RequestOptions) queryRequestOptions, clientContext, correlatedActivityId1, (ContainerInternal) containerCore);
            initialUserContinuationToken = tryCatch.Result;
            break;
          }
          initialUserContinuationToken = (CosmosElement) null;
          break;
        case ExecutionEnvironment.Compute:
          initialUserContinuationToken = queryRequestOptions.CosmosElementContinuationToken;
          break;
        default:
          executionEnvironment = queryRequestOptions.ExecutionEnvironment;
          throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) "ExecutionEnvironment", (object) executionEnvironment.Value));
      }
      CosmosQueryExecutionContextFactory.InputParameters inputParameters = new CosmosQueryExecutionContextFactory.InputParameters(sqlQuerySpec, initialUserContinuationToken, feedRangeInternal, queryRequestOptions.MaxConcurrency, queryRequestOptions.MaxItemCount, queryRequestOptions.MaxBufferedItemCount, queryRequestOptions.PartitionKey, queryRequestOptions.Properties, partitionedQueryExecutionInfo, queryRequestOptions.ExecutionEnvironment, queryRequestOptions.ReturnResultsInDeterministicOrder, forcePassthrough, queryRequestOptions.TestSettings);
      return new QueryIterator(cosmosQueryContext, CosmosQueryExecutionContextFactory.Create(documentContainer, (CosmosQueryContext) cosmosQueryContext, inputParameters, (ITrace) NoOpTrace.Singleton), queryRequestOptions.CosmosSerializationFormatOptions, (RequestOptions) queryRequestOptions, clientContext, correlatedActivityId1, (ContainerInternal) containerCore);
    }

    public override bool HasMoreResults => this.hasMoreResults;

    public override Task<ResponseMessage> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ReadNextAsync((ITrace) NoOpTrace.Singleton, cancellationToken);

    public override async Task<ResponseMessage> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      object obj;
      Guid correlatedActivityId;
      if (trace.Data.TryGetValue(QueryIterator.CorrelatedActivityIdKeyName, out obj))
      {
        List<string> list = ((IEnumerable<string>) obj.ToString().Split(',')).ToList<string>();
        List<string> stringList1 = list;
        correlatedActivityId = this.correlatedActivityId;
        string str1 = correlatedActivityId.ToString();
        if (!stringList1.Contains(str1))
        {
          List<string> stringList2 = list;
          correlatedActivityId = this.correlatedActivityId;
          string str2 = correlatedActivityId.ToString();
          stringList2.Add(str2);
          trace.AddOrUpdateDatum(QueryIterator.CorrelatedActivityIdKeyName, (object) string.Join(",", (IEnumerable<string>) list));
        }
      }
      else
        trace.AddDatum(QueryIterator.CorrelatedActivityIdKeyName, (object) this.correlatedActivityId.ToString());
      TryCatch<QueryPage> current;
      try
      {
        this.queryPipelineStage.SetCancellationToken(cancellationToken);
        if (!await this.queryPipelineStage.MoveNextAsync(trace))
        {
          this.hasMoreResults = false;
          IReadOnlyList<CosmosElement> emptyPage = QueryIterator.EmptyPage;
          int count = QueryIterator.EmptyPage.Count;
          CosmosSerializationFormatOptions serializationFormatOptions = this.cosmosSerializationFormatOptions;
          CosmosQueryResponseMessageHeaders responseHeaders = new CosmosQueryResponseMessageHeaders((string) null, (string) null, this.cosmosQueryContext.ResourceTypeEnum, this.cosmosQueryContext.ContainerResourceId);
          responseHeaders.RequestCharge = 0.0;
          correlatedActivityId = this.correlatedActivityId;
          responseHeaders.ActivityId = correlatedActivityId.ToString();
          responseHeaders.SubStatusCode = SubStatusCodes.Unknown;
          CosmosSerializationFormatOptions serializationOptions = serializationFormatOptions;
          ITrace trace1 = trace;
          return (ResponseMessage) QueryResponse.CreateSuccess(emptyPage, count, 0L, responseHeaders, serializationOptions, trace1);
        }
        current = this.queryPipelineStage.Current;
      }
      catch (OperationCanceledException ex) when (!(ex is CosmosOperationCanceledException))
      {
        throw new CosmosOperationCanceledException(ex, trace);
      }
      if (current.Succeeded)
      {
        if (current.Result.State == null && current.Result.DisallowContinuationTokenMessage == null)
          this.hasMoreResults = false;
        CosmosQueryResponseMessageHeaders responseMessageHeaders1 = new CosmosQueryResponseMessageHeaders(current.Result.State?.Value.ToString(), current.Result.DisallowContinuationTokenMessage, this.cosmosQueryContext.ResourceTypeEnum, this.cosmosQueryContext.ContainerResourceId);
        responseMessageHeaders1.RequestCharge = current.Result.RequestCharge;
        responseMessageHeaders1.ActivityId = current.Result.ActivityId;
        responseMessageHeaders1.SubStatusCode = SubStatusCodes.Unknown;
        CosmosQueryResponseMessageHeaders responseMessageHeaders2 = responseMessageHeaders1;
        foreach (KeyValuePair<string, string> additionalHeader in (IEnumerable<KeyValuePair<string, string>>) current.Result.AdditionalHeaders)
          responseMessageHeaders2[additionalHeader.Key] = additionalHeader.Value;
        IReadOnlyList<CosmosElement> documents = current.Result.Documents;
        int count = current.Result.Documents.Count;
        long responseLengthInBytes = current.Result.ResponseLengthInBytes;
        CosmosSerializationFormatOptions serializationFormatOptions = this.cosmosSerializationFormatOptions;
        CosmosQueryResponseMessageHeaders responseHeaders = responseMessageHeaders2;
        CosmosSerializationFormatOptions serializationOptions = serializationFormatOptions;
        ITrace trace2 = trace;
        return (ResponseMessage) QueryResponse.CreateSuccess(documents, count, responseLengthInBytes, responseHeaders, serializationOptions, trace2);
      }
      CosmosException cosmosException1;
      if (!ExceptionToCosmosException.TryCreateFromException(current.Exception, trace, out cosmosException1))
        throw current.Exception;
      if (!FeedIteratorInternal.IsRetriableException(cosmosException1))
        this.hasMoreResults = false;
      HttpStatusCode statusCode = cosmosException1.StatusCode;
      CosmosException cosmosException2 = cosmosException1;
      return (ResponseMessage) QueryResponse.CreateFailure(CosmosQueryResponseMessageHeaders.ConvertToQueryHeaders(cosmosException1.Headers, this.cosmosQueryContext.ResourceTypeEnum, this.cosmosQueryContext.ContainerResourceId, new int?(cosmosException1.SubStatusCode), cosmosException1.ActivityId), statusCode, (RequestMessage) null, cosmosException2, trace);
    }

    public override CosmosElement GetCosmosElementContinuationToken() => this.queryPipelineStage.Current.Result.State?.Value;

    protected override void Dispose(bool disposing)
    {
      this.queryPipelineStage.DisposeAsync();
      base.Dispose(disposing);
    }

    internal static bool IsSystemPrefixExpected(QueryRequestOptions queryRequestOptions)
    {
      object obj;
      bool result;
      return queryRequestOptions != null && queryRequestOptions.Properties != null && queryRequestOptions.Properties.TryGetValue("x-ms-query-disableSystemPrefix", out obj) && bool.TryParse(obj.ToString(), out result) && !result;
    }
  }
}
