// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.DocumentQuery`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Query;
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

namespace Microsoft.Azure.Documents.Linq
{
  internal sealed class DocumentQuery<T> : 
    IOrderedQueryableDocumentQuery<T>,
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
    public static readonly FeedResponse<object> EmptyFeedResponse = new FeedResponse<object>(Enumerable.Empty<object>(), 0, (INameValueCollection) new DictionaryNameValueCollection());
    private readonly IDocumentQueryClient client;
    private readonly ResourceType resourceTypeEnum;
    private readonly Type resourceType;
    private readonly string documentsFeedOrDatabaseLink;
    private readonly FeedOptions feedOptions;
    private readonly object partitionKey;
    private readonly Expression expression;
    private readonly DocumentQueryProvider queryProvider;
    private readonly SchedulingStopwatch executeNextAysncMetrics;
    private readonly Guid correlatedActivityId;
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
      this.expression = expression ?? (Expression) Expression.Constant((object) this);
      this.queryProvider = new DocumentQueryProvider(client, resourceTypeEnum, resourceType, documentsFeedOrDatabaseLink, feedOptions, partitionKey, this.client.OnExecuteScalarQueryCallback);
      this.executeNextAysncMetrics = new SchedulingStopwatch();
      this.executeNextAysncMetrics.Ready();
      this.correlatedActivityId = Guid.NewGuid();
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

    public Expression Expression => this.expression;

    public IQueryProvider Provider => (IQueryProvider) this.queryProvider;

    public bool HasMoreResults => this.queryExecutionContext == null || !this.queryExecutionContext.IsDone;

    public Guid CorrelatedActivityId => this.correlatedActivityId;

    public void Dispose()
    {
      if (this.queryExecutionContext == null)
        return;
      this.queryExecutionContext.Dispose();
      DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, CorrelatedActivityId: {1} | Disposing DocumentQuery", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.CorrelatedActivityId));
    }

    public Task<FeedResponse<object>> ExecuteNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ExecuteNextAsync<object>(cancellationToken);

    public Task<FeedResponse<TResponse>> ExecuteNextAsync<TResponse>(
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
        return TaskHelper.InlineIfPossible<FeedResponse<TResponse>>((Func<Task<FeedResponse<TResponse>>>) (() => this.ExecuteNextPrivateAsync<TResponse>(cancellationToken)), (IRetryPolicy) null, cancellationToken);
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
          // ISSUE: reference to a compiler-generated field
          if (DocumentQuery<T>.\u003C\u003Eo__31.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentQuery<T>.\u003C\u003Eo__31.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, IEnumerable<T>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable<T>), typeof (DocumentQuery<T>)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          foreach (T obj in DocumentQuery<T>.\u003C\u003Eo__31.\u003C\u003Ep__0.Target((CallSite) DocumentQuery<T>.\u003C\u003Eo__31.\u003C\u003Ep__0, (object) TaskHelper.InlineIfPossible<FeedResponse<object>>((Func<Task<FeedResponse<object>>>) (() => localQueryExecutionContext.ExecuteNextAsync(CancellationToken.None)), (IRetryPolicy) null).Result))
            yield return obj;
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public override string ToString()
    {
      SqlQuerySpec sqlQuerySpec = DocumentQueryEvaluator.Evaluate(this.expression);
      return sqlQuerySpec != null ? JsonConvert.SerializeObject((object) sqlQuerySpec) : new Uri(this.client.ServiceEndpoint, this.documentsFeedOrDatabaseLink).ToString();
    }

    private Task<IDocumentQueryExecutionContext> CreateDocumentQueryExecutionContextAsync(
      bool isContinuationExpected,
      CancellationToken cancellationToken)
    {
      IPartitionResolver partitionResolver;
      return this.documentsFeedOrDatabaseLink != null && this.client.PartitionResolvers.TryGetValue(this.documentsFeedOrDatabaseLink, out partitionResolver) && (object) this.resourceType == (object) typeof (Document) ? DocumentQueryExecutionContextFactory.CreateDocumentQueryExecutionContextAsync(this.client, this.resourceTypeEnum, this.resourceType, this.expression, this.feedOptions, (IEnumerable<string>) partitionResolver.ResolveForRead(this.partitionKey).ToArray<string>(), isContinuationExpected, cancellationToken, this.CorrelatedActivityId) : DocumentQueryExecutionContextFactory.CreateDocumentQueryExecutionContextAsync(this.client, this.resourceTypeEnum, this.resourceType, this.expression, this.feedOptions, this.documentsFeedOrDatabaseLink, isContinuationExpected, cancellationToken, this.CorrelatedActivityId);
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
            DocumentQuery<T>.\u003C\u003Eo__35.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, FeedResponse<T>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (FeedResponse<T>), typeof (DocumentQuery<T>)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, FeedResponse<T>> func = DocumentQuery<T>.\u003C\u003Eo__35.\u003C\u003Ep__0.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, FeedResponse<T>>> callSite = DocumentQuery<T>.\u003C\u003Eo__35.\u003C\u003Ep__0;
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
          object obj1 = DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__0.Target((CallSite) DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__0, (object) TaskHelper.InlineIfPossible<FeedResponse<object>>((Func<Task<FeedResponse<object>>>) (() => localQueryExecutionContext.ExecuteNextAsync(cancellationToken)), (IRetryPolicy) null, cancellationToken));
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
          FeedResponse<T> collection = func((CallSite) callSite, DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__3.Target((CallSite) DocumentQuery<T>.\u003CExecuteAllAsync\u003Ed__35.\u003C\u003Eo__35.\u003C\u003Ep__3, obj1));
          func = (Func<CallSite, object, FeedResponse<T>>) null;
          callSite = (CallSite<Func<CallSite, object, FeedResponse<T>>>) null;
          result.AddRange((IEnumerable<T>) collection);
        }
      }
      return result;
    }

    private async Task<FeedResponse<TResponse>> ExecuteNextPrivateAsync<TResponse>(
      CancellationToken cancellationToken)
    {
      if (this.queryExecutionContext == null)
        this.queryExecutionContext = await this.CreateDocumentQueryExecutionContextAsync(true, cancellationToken);
      else if (this.queryExecutionContext.IsDone)
      {
        this.queryExecutionContext.Dispose();
        this.queryExecutionContext = await this.CreateDocumentQueryExecutionContextAsync(true, cancellationToken);
      }
      FeedResponse<object> feedResponse1 = await this.queryExecutionContext.ExecuteNextAsync(cancellationToken);
      // ISSUE: reference to a compiler-generated field
      if (DocumentQuery<T>.\u003C\u003Eo__36<TResponse>.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DocumentQuery<T>.\u003C\u003Eo__36<TResponse>.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, FeedResponse<TResponse>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (FeedResponse<TResponse>), typeof (DocumentQuery<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      FeedResponse<TResponse> result = DocumentQuery<T>.\u003C\u003Eo__36<TResponse>.\u003C\u003Ep__0.Target((CallSite) DocumentQuery<T>.\u003C\u003Eo__36<TResponse>.\u003C\u003Ep__0, (object) feedResponse1);
      FeedResponse<TResponse> feedResponse2 = new FeedResponse<TResponse>((IEnumerable<TResponse>) result, result.Count, result.Headers, result.UseETagAsContinuation, result.QueryMetrics, result.PartitionedClientSideRequestStatistics, feedResponse1.DisallowContinuationTokenMessage, result.ResponseLengthBytes);
      if (!this.HasMoreResults && !this.tracedLastExecution)
      {
        DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, CorrelatedActivityId: {1} | Last ExecuteNextAsync with ExecuteNextAsyncMetrics: [{2}]", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.CorrelatedActivityId, (object) this.executeNextAysncMetrics));
        this.tracedLastExecution = true;
      }
      return feedResponse2;
    }
  }
}
