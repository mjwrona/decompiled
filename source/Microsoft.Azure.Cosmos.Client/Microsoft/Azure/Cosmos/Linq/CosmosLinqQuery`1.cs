// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.CosmosLinqQuery`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class CosmosLinqQuery<T> : 
    IDocumentQuery<T>,
    IDocumentQuery,
    IDisposable,
    IOrderedQueryable<T>,
    IEnumerable<T>,
    IEnumerable,
    IOrderedQueryable,
    IQueryable,
    IQueryable<T>
  {
    private readonly CosmosLinqQueryProvider queryProvider;
    private readonly Guid correlatedActivityId;
    private readonly ContainerInternal container;
    private readonly CosmosQueryClientCore queryClient;
    private readonly CosmosResponseFactoryInternal responseFactory;
    private readonly QueryRequestOptions cosmosQueryRequestOptions;
    private readonly bool allowSynchronousQueryExecution;
    private readonly string continuationToken;
    private readonly CosmosLinqSerializerOptions linqSerializationOptions;

    public CosmosLinqQuery(
      ContainerInternal container,
      CosmosResponseFactoryInternal responseFactory,
      CosmosQueryClientCore queryClient,
      string continuationToken,
      QueryRequestOptions cosmosQueryRequestOptions,
      Expression expression,
      bool allowSynchronousQueryExecution,
      CosmosLinqSerializerOptions linqSerializationOptions = null)
    {
      this.container = container ?? throw new ArgumentNullException(nameof (container));
      this.responseFactory = responseFactory ?? throw new ArgumentNullException(nameof (responseFactory));
      this.queryClient = queryClient ?? throw new ArgumentNullException(nameof (queryClient));
      this.continuationToken = continuationToken;
      this.cosmosQueryRequestOptions = cosmosQueryRequestOptions;
      this.Expression = expression ?? (Expression) Expression.Constant((object) this);
      this.allowSynchronousQueryExecution = allowSynchronousQueryExecution;
      this.correlatedActivityId = Guid.NewGuid();
      this.linqSerializationOptions = linqSerializationOptions;
      this.queryProvider = new CosmosLinqQueryProvider(container, responseFactory, queryClient, this.continuationToken, cosmosQueryRequestOptions, this.allowSynchronousQueryExecution, this.queryClient.OnExecuteScalarQueryCallback, this.linqSerializationOptions);
    }

    public CosmosLinqQuery(
      ContainerInternal container,
      CosmosResponseFactoryInternal responseFactory,
      CosmosQueryClientCore queryClient,
      string continuationToken,
      QueryRequestOptions cosmosQueryRequestOptions,
      bool allowSynchronousQueryExecution,
      CosmosLinqSerializerOptions linqSerializerOptions = null)
      : this(container, responseFactory, queryClient, continuationToken, cosmosQueryRequestOptions, (Expression) null, allowSynchronousQueryExecution, linqSerializerOptions)
    {
    }

    public Type ElementType => typeof (T);

    public Expression Expression { get; }

    public IQueryProvider Provider => (IQueryProvider) this.queryProvider;

    public bool HasMoreResults => throw new NotImplementedException();

    public IEnumerator<T> GetEnumerator()
    {
      if (!this.allowSynchronousQueryExecution)
        throw new NotSupportedException("To execute LINQ query please set allowSynchronousQueryExecution true or use GetItemQueryIterator to execute asynchronously");
      FeedIterator<T> localFeedIterator = this.CreateFeedIterator(false);
      while (localFeedIterator.HasMoreResults)
      {
        foreach (T obj in TaskHelper.InlineIfPossible<FeedResponse<T>>((Func<Task<FeedResponse<T>>>) (() => localFeedIterator.ReadNextAsync(CancellationToken.None)), (IRetryPolicy) null).GetAwaiter().GetResult())
          yield return obj;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public override string ToString()
    {
      SqlQuerySpec sqlQuerySpec = DocumentQueryEvaluator.Evaluate(this.Expression, this.linqSerializationOptions);
      return sqlQuerySpec != null ? JsonConvert.SerializeObject((object) sqlQuerySpec) : this.container.LinkUri.ToString();
    }

    public QueryDefinition ToQueryDefinition(IDictionary<object, string> parameters = null) => QueryDefinition.CreateFromQuerySpec(DocumentQueryEvaluator.Evaluate(this.Expression, this.linqSerializationOptions, parameters));

    public FeedIterator<T> ToFeedIterator() => (FeedIterator<T>) new FeedIteratorInlineCore<T>(this.CreateFeedIterator(true), this.container.ClientContext);

    public FeedIterator ToStreamIterator() => (FeedIterator) new FeedIteratorInlineCore(this.CreateStreamIterator(true), this.container.ClientContext);

    public void Dispose()
    {
    }

    Task<DocumentFeedResponse<TResult>> IDocumentQuery<T>.ExecuteNextAsync<TResult>(
      CancellationToken token)
    {
      throw new NotImplementedException();
    }

    Task<DocumentFeedResponse<object>> IDocumentQuery<T>.ExecuteNextAsync(CancellationToken token) => throw new NotImplementedException();

    internal async Task<Response<T>> AggregateResultAsync(CancellationToken cancellationToken = default (CancellationToken))
    {
      List<T> result = new List<T>();
      Microsoft.Azure.Cosmos.Headers headers = new Microsoft.Azure.Cosmos.Headers();
      FeedIterator<T> localFeedIterator = this.CreateFeedIterator(false);
      FeedIteratorInternal<T> localFeedIteratorInternal = (FeedIteratorInternal<T>) localFeedIterator;
      ITrace rootTrace;
      using (rootTrace = (ITrace) Trace.GetRootTrace("Aggregate LINQ Operation"))
      {
        while (localFeedIterator.HasMoreResults)
        {
          FeedResponse<T> collection = await localFeedIteratorInternal.ReadNextAsync(rootTrace, cancellationToken);
          headers.RequestCharge += collection.RequestCharge;
          result.AddRange((IEnumerable<T>) collection);
        }
      }
      Response<T> response = (Response<T>) new ItemResponse<T>(HttpStatusCode.OK, headers, result.FirstOrDefault<T>(), (CosmosDiagnostics) new CosmosTraceDiagnostics(rootTrace), (RequestMessage) null);
      result = (List<T>) null;
      headers = (Microsoft.Azure.Cosmos.Headers) null;
      localFeedIterator = (FeedIterator<T>) null;
      localFeedIteratorInternal = (FeedIteratorInternal<T>) null;
      rootTrace = (ITrace) null;
      return response;
    }

    private FeedIteratorInternal CreateStreamIterator(bool isContinuationExcpected) => this.container.GetItemQueryStreamIteratorInternal(DocumentQueryEvaluator.Evaluate(this.Expression, this.linqSerializationOptions), isContinuationExcpected, this.continuationToken, (FeedRangeInternal) null, this.cosmosQueryRequestOptions);

    private FeedIterator<T> CreateFeedIterator(bool isContinuationExpected)
    {
      DocumentQueryEvaluator.Evaluate(this.Expression, this.linqSerializationOptions);
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>((FeedIteratorInternal<T>) new FeedIteratorCore<T>(this.CreateStreamIterator(isContinuationExpected), new Func<ResponseMessage, FeedResponse<T>>(this.responseFactory.CreateQueryFeedUserTypeResponse<T>)), this.container.ClientContext);
    }
  }
}
