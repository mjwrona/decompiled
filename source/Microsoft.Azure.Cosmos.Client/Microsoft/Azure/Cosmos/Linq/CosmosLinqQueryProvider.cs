// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.CosmosLinqQueryProvider
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class CosmosLinqQueryProvider : IQueryProvider
  {
    private readonly ContainerInternal container;
    private readonly CosmosQueryClientCore queryClient;
    private readonly CosmosResponseFactoryInternal responseFactory;
    private readonly QueryRequestOptions cosmosQueryRequestOptions;
    private readonly bool allowSynchronousQueryExecution;
    private readonly Action<IQueryable> onExecuteScalarQueryCallback;
    private readonly string continuationToken;
    private readonly CosmosLinqSerializerOptions linqSerializerOptions;

    public CosmosLinqQueryProvider(
      ContainerInternal container,
      CosmosResponseFactoryInternal responseFactory,
      CosmosQueryClientCore queryClient,
      string continuationToken,
      QueryRequestOptions cosmosQueryRequestOptions,
      bool allowSynchronousQueryExecution,
      Action<IQueryable> onExecuteScalarQueryCallback = null,
      CosmosLinqSerializerOptions linqSerializerOptions = null)
    {
      this.container = container;
      this.responseFactory = responseFactory;
      this.queryClient = queryClient;
      this.continuationToken = continuationToken;
      this.cosmosQueryRequestOptions = cosmosQueryRequestOptions;
      this.allowSynchronousQueryExecution = allowSynchronousQueryExecution;
      this.onExecuteScalarQueryCallback = onExecuteScalarQueryCallback;
      this.linqSerializerOptions = linqSerializerOptions;
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => (IQueryable<TElement>) new CosmosLinqQuery<TElement>(this.container, this.responseFactory, this.queryClient, this.continuationToken, this.cosmosQueryRequestOptions, expression, this.allowSynchronousQueryExecution, this.linqSerializerOptions);

    public IQueryable CreateQuery(Expression expression)
    {
      Type elementType = TypeSystem.GetElementType(expression.Type);
      return (IQueryable) Activator.CreateInstance(typeof (CosmosLinqQuery<bool>).GetGenericTypeDefinition().MakeGenericType(elementType), (object) this.container, (object) this.responseFactory, (object) this.queryClient, (object) this.continuationToken, (object) this.cosmosQueryRequestOptions, (object) expression, (object) this.allowSynchronousQueryExecution, (object) this.linqSerializerOptions);
    }

    public TResult Execute<TResult>(Expression expression)
    {
      CosmosLinqQuery<TResult> instance = (CosmosLinqQuery<TResult>) Activator.CreateInstance(typeof (CosmosLinqQuery<bool>).GetGenericTypeDefinition().MakeGenericType(typeof (TResult)), (object) this.container, (object) this.responseFactory, (object) this.queryClient, (object) this.continuationToken, (object) this.cosmosQueryRequestOptions, (object) expression, (object) this.allowSynchronousQueryExecution, (object) this.linqSerializerOptions);
      Action<IQueryable> scalarQueryCallback = this.onExecuteScalarQueryCallback;
      if (scalarQueryCallback != null)
        scalarQueryCallback((IQueryable) instance);
      return instance.ToList<TResult>().FirstOrDefault<TResult>();
    }

    public object Execute(Expression expression)
    {
      CosmosLinqQuery<object> instance = (CosmosLinqQuery<object>) Activator.CreateInstance(typeof (CosmosLinqQuery<bool>).GetGenericTypeDefinition().MakeGenericType(typeof (object)), (object) this.container, (object) this.responseFactory, (object) this.queryClient, (object) this.continuationToken, (object) this.cosmosQueryRequestOptions, (object) this.allowSynchronousQueryExecution, (object) this.linqSerializerOptions);
      Action<IQueryable> scalarQueryCallback = this.onExecuteScalarQueryCallback;
      if (scalarQueryCallback != null)
        scalarQueryCallback((IQueryable) instance);
      return instance.ToList<object>().FirstOrDefault<object>();
    }

    public Task<Response<TResult>> ExecuteAggregateAsync<TResult>(
      Expression expression,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CosmosLinqQuery<TResult> cosmosLINQQuery = (CosmosLinqQuery<TResult>) Activator.CreateInstance(typeof (CosmosLinqQuery<bool>).GetGenericTypeDefinition().MakeGenericType(typeof (TResult)), (object) this.container, (object) this.responseFactory, (object) this.queryClient, (object) this.continuationToken, (object) this.cosmosQueryRequestOptions, (object) expression, (object) this.allowSynchronousQueryExecution, (object) this.linqSerializerOptions);
      return TaskHelper.RunInlineIfNeededAsync<Response<TResult>>((Func<Task<Response<TResult>>>) (() => cosmosLINQQuery.AggregateResultAsync(cancellationToken)));
    }
  }
}
