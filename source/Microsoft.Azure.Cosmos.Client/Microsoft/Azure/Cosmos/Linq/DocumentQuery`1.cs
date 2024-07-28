// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.DocumentQuery`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Metrics;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class DocumentQuery<T> : 
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
    public static readonly DocumentFeedResponse<object> EmptyFeedResponse = new DocumentFeedResponse<object>(Enumerable.Empty<object>(), 0, (INameValueCollection) new StoreResponseNameValueCollection());
    private readonly IDocumentQueryClient client;
    private readonly ResourceType resourceTypeEnum;
    private readonly Type resourceType;
    private readonly string documentsFeedOrDatabaseLink;
    private readonly FeedOptions feedOptions;
    private readonly object partitionKey;
    private readonly DocumentQueryProvider queryProvider;
    private readonly SchedulingStopwatch executeNextAysncMetrics;
    private IDocumentQueryExecutionContext queryExecutionContext;
    private bool tracedFirstExecution;
    private bool tracedLastExecution;

    public DocumentQuery(
      IDocumentQueryClient client,
      ResourceType resourceTypeEnum,
      Type resourceType,
      string documentsFeedOrDatabaseLink,
      Expression expression,
      FeedOptions feedOptions,
      object partitionKey = null)
    {
      this.client = client != null ? client : throw new ArgumentNullException(nameof (client));
      this.resourceTypeEnum = resourceTypeEnum;
      this.resourceType = resourceType;
      this.documentsFeedOrDatabaseLink = documentsFeedOrDatabaseLink;
      this.feedOptions = feedOptions == null ? new FeedOptions() : new FeedOptions(feedOptions);
      if (this.feedOptions.MaxBufferedItemCount < 0)
        this.feedOptions.MaxBufferedItemCount = int.MaxValue;
      if (this.feedOptions.MaxDegreeOfParallelism < 0)
        this.feedOptions.MaxDegreeOfParallelism = int.MaxValue;
      int? maxItemCount = this.feedOptions.MaxItemCount;
      int num = 0;
      if (maxItemCount.GetValueOrDefault() < num & maxItemCount.HasValue)
        this.feedOptions.MaxItemCount = new int?(int.MaxValue);
      this.partitionKey = partitionKey;
      this.Expression = expression ?? (Expression) Expression.Constant((object) this);
      this.queryProvider = new DocumentQueryProvider(client, resourceTypeEnum, resourceType, documentsFeedOrDatabaseLink, feedOptions, partitionKey, this.client.OnExecuteScalarQueryCallback);
      this.executeNextAysncMetrics = new SchedulingStopwatch();
      this.executeNextAysncMetrics.Ready();
      this.CorrelatedActivityId = Guid.NewGuid();
    }

    public DocumentQuery(
      DocumentClient client,
      ResourceType resourceTypeEnum,
      Type resourceType,
      string documentsFeedOrDatabaseLink,
      Expression expression,
      FeedOptions feedOptions,
      object partitionKey = null)
      : this((IDocumentQueryClient) new DocumentQueryClient(client), resourceTypeEnum, resourceType, documentsFeedOrDatabaseLink, expression, feedOptions, partitionKey)
    {
    }

    public DocumentQuery(
      IDocumentQueryClient client,
      ResourceType resourceTypeEnum,
      Type resourceType,
      string documentsFeedOrDatabaseLink,
      FeedOptions feedOptions,
      object partitionKey = null)
      : this(client, resourceTypeEnum, resourceType, documentsFeedOrDatabaseLink, (Expression) null, feedOptions, partitionKey)
    {
    }

    public DocumentQuery(
      DocumentClient client,
      ResourceType resourceTypeEnum,
      Type resourceType,
      string documentsFeedOrDatabaseLink,
      FeedOptions feedOptions,
      object partitionKey = null)
      : this((IDocumentQueryClient) new DocumentQueryClient(client), resourceTypeEnum, resourceType, documentsFeedOrDatabaseLink, (Expression) null, feedOptions, partitionKey)
    {
    }

    public Type ElementType => typeof (T);

    public Expression Expression { get; }

    public IQueryProvider Provider => (IQueryProvider) this.queryProvider;

    public bool HasMoreResults => this.queryExecutionContext == null || !this.queryExecutionContext.IsDone;

    public Guid CorrelatedActivityId { get; }

    public void Dispose()
    {
      if (this.queryExecutionContext == null)
        return;
      this.queryExecutionContext.Dispose();
      DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, CorrelatedActivityId: {1} | Disposing DocumentQuery", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.CorrelatedActivityId));
    }

    public Task<DocumentFeedResponse<object>> ExecuteNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ExecuteNextAsync<object>(cancellationToken);

    public Task<DocumentFeedResponse<TResponse>> ExecuteNextAsync<TResponse>(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      try
      {
        if (!this.tracedFirstExecution)
        {
          DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, CorrelatedActivityId: {1} | First ExecuteNextAsync", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.CorrelatedActivityId));
          this.tracedFirstExecution = true;
        }
        this.executeNextAysncMetrics.Start();
        return TaskHelper.InlineIfPossible<DocumentFeedResponse<TResponse>>((Func<Task<DocumentFeedResponse<TResponse>>>) (() => this.ExecuteNextPrivateAsync<TResponse>(cancellationToken)), (IRetryPolicy) null, cancellationToken);
      }
      finally
      {
        this.executeNextAysncMetrics.Stop();
        if (!this.HasMoreResults && !this.tracedLastExecution)
        {
          DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, CorrelatedActivityId: {1} | Last ExecuteNextAsync with ExecuteNextAsyncMetrics: [{2}]", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.CorrelatedActivityId, (object) this.executeNextAysncMetrics));
          this.tracedLastExecution = true;
        }
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      DocumentQuery<T> documentQuery = this;
      // ISSUE: reference to a compiler-generated method
      using (IDocumentQueryExecutionContext localQueryExecutionContext = TaskHelper.InlineIfPossible<IDocumentQueryExecutionContext>(new Func<Task<IDocumentQueryExecutionContext>>(documentQuery.\u003CGetEnumerator\u003Eb__31_0), (IRetryPolicy) null).Result)
      {
        while (!localQueryExecutionContext.IsDone)
        {
          foreach (T obj in FeedResponseBinder.ConvertCosmosElementFeed<T>(TaskHelper.InlineIfPossible<DocumentFeedResponse<CosmosElement>>((Func<Task<DocumentFeedResponse<CosmosElement>>>) (() => localQueryExecutionContext.ExecuteNextFeedResponseAsync(CancellationToken.None)), (IRetryPolicy) null).Result, documentQuery.resourceTypeEnum, documentQuery.feedOptions.JsonSerializerSettings))
            yield return obj;
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public override string ToString()
    {
      SqlQuerySpec sqlQuerySpec = DocumentQueryEvaluator.Evaluate(this.Expression);
      return sqlQuerySpec != null ? JsonConvert.SerializeObject((object) sqlQuerySpec) : new Uri(this.client.ServiceEndpoint, this.documentsFeedOrDatabaseLink).ToString();
    }

    private Task<IDocumentQueryExecutionContext> CreateDocumentQueryExecutionContextAsync(
      bool isContinuationExpected,
      CancellationToken cancellationToken)
    {
      return DocumentQueryExecutionContextFactory.CreateDocumentQueryExecutionContextAsync(this.client, this.resourceTypeEnum, this.resourceType, this.Expression, this.feedOptions, this.documentsFeedOrDatabaseLink, isContinuationExpected, cancellationToken, this.CorrelatedActivityId);
    }

    internal async Task<List<T>> ExecuteAllAsync(CancellationToken cancellationToken = default (CancellationToken))
    {
      List<T> result = new List<T>();
      // ISSUE: reference to a compiler-generated field
      using (IDocumentQueryExecutionContext localQueryExecutionContext = await TaskHelper.InlineIfPossible<IDocumentQueryExecutionContext>((Func<Task<IDocumentQueryExecutionContext>>) (() => this.\u003C\u003E4__this.CreateDocumentQueryExecutionContextAsync(false, cancellationToken)), (IRetryPolicy) null, cancellationToken))
      {
        while (!localQueryExecutionContext.IsDone)
        {
          // ISSUE: reference to a compiler-generated field
          if (DocumentQuery<T>.\u003C\u003Eo__35.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentQuery<T>.\u003C\u003Eo__35.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, DocumentFeedResponse<T>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (DocumentFeedResponse<T>), typeof (DocumentQuery<T>)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, DocumentFeedResponse<T>> func = DocumentQuery<T>.\u003C\u003Eo__35.\u003C\u003Ep__0.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, DocumentFeedResponse<T>>> callSite = DocumentQuery<T>.\u003C\u003Eo__35.\u003C\u003Ep__0;
          // ISSUE: reference to a compiler-generated field
          if (DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "GetAwaiter", (IEnumerable<Type>) null, typeof (DocumentQuery<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj1 = DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__0.Target((CallSite) DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__0, (object) TaskHelper.InlineIfPossible<DocumentFeedResponse<CosmosElement>>((Func<Task<DocumentFeedResponse<CosmosElement>>>) (() => localQueryExecutionContext.ExecuteNextFeedResponseAsync(cancellationToken)), (IRetryPolicy) null, cancellationToken));
          // ISSUE: reference to a compiler-generated field
          if (DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (bool), typeof (DocumentQuery<T>)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target = DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__2.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p2 = DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__2;
          // ISSUE: reference to a compiler-generated field
          if (DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "IsCompleted", typeof (DocumentQuery<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj2 = DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__1.Target((CallSite) DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__1, obj1);
          if (!target((CallSite) p2, obj2))
          {
            int num;
            // ISSUE: explicit reference operation
            // ISSUE: reference to a compiler-generated field
            (^this).\u003C\u003E1__state = num = 1;
            object obj = obj1;
            if (!(obj1 is ICriticalNotifyCompletion awaiter1))
            {
              INotifyCompletion awaiter = (INotifyCompletion) obj1;
              // ISSUE: explicit reference operation
              // ISSUE: reference to a compiler-generated field
              (^this).\u003C\u003Et__builder.AwaitOnCompleted<INotifyCompletion, DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35>(ref awaiter, this);
            }
            else
            {
              // ISSUE: explicit reference operation
              // ISSUE: reference to a compiler-generated field
              (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<ICriticalNotifyCompletion, DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35>(ref awaiter1, this);
            }
            awaiter1 = (ICriticalNotifyCompletion) null;
            return;
          }
          // ISSUE: reference to a compiler-generated field
          if (DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__3 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "GetResult", (IEnumerable<Type>) null, typeof (DocumentQuery<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          DocumentFeedResponse<T> collection = func((CallSite) callSite, DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__3.Target((CallSite) DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__3, obj1));
          func = (Func<CallSite, object, DocumentFeedResponse<T>>) null;
          callSite = (CallSite<Func<CallSite, object, DocumentFeedResponse<T>>>) null;
          result.AddRange((IEnumerable<T>) collection);
        }
      }
      List<T> objList = result;
      result = (List<T>) null;
      return objList;
    }

    private async Task<DocumentFeedResponse<TResponse>> ExecuteNextPrivateAsync<TResponse>(
      CancellationToken cancellationToken)
    {
      if (this.queryExecutionContext == null)
        this.queryExecutionContext = await this.CreateDocumentQueryExecutionContextAsync(true, cancellationToken);
      else if (this.queryExecutionContext.IsDone)
      {
        this.queryExecutionContext.Dispose();
        this.queryExecutionContext = await this.CreateDocumentQueryExecutionContextAsync(true, cancellationToken);
      }
      DocumentFeedResponse<TResponse> documentFeedResponse = FeedResponseBinder.ConvertCosmosElementFeed<TResponse>(await this.queryExecutionContext.ExecuteNextFeedResponseAsync(cancellationToken), this.resourceTypeEnum, this.feedOptions.JsonSerializerSettings);
      if (!this.HasMoreResults && !this.tracedLastExecution)
      {
        DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, CorrelatedActivityId: {1} | Last ExecuteNextAsync with ExecuteNextAsyncMetrics: [{2}]", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.CorrelatedActivityId, (object) this.executeNextAysncMetrics));
        this.tracedLastExecution = true;
      }
      return documentFeedResponse;
    }
  }
}
