// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableQuery`1
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.Queryable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table
{
  public class TableQuery<TElement> : 
    IQueryable<TElement>,
    IEnumerable<TElement>,
    IEnumerable,
    IQueryable
  {
    private readonly Expression queryExpression;
    private readonly TableQueryProvider queryProvider;
    private int? takeCount;

    public TableQuery()
    {
      if (typeof (TElement).GetTypeInfo().GetInterface(typeof (ITableEntity).FullName, false) == (Type) null)
        throw new NotSupportedException("TableQuery Generic Type must implement the ITableEntity Interface");
      if (typeof (TElement).GetTypeInfo().GetConstructor(Type.EmptyTypes) == (ConstructorInfo) null)
        throw new NotSupportedException("TableQuery Generic Type must provide a default parameterless constructor.");
    }

    internal TableQuery(CloudTable table)
    {
      this.queryProvider = new TableQueryProvider(table);
      this.queryExpression = (Expression) new ResourceSetExpression(typeof (IOrderedQueryable<TElement>), (Expression) null, (Expression) Expression.Constant((object) "0"), typeof (TElement), (List<string>) null, CountOption.None, (Dictionary<ConstantExpression, ConstantExpression>) null, (ProjectionQueryOptionExpression) null);
    }

    internal TableQuery(Expression queryExpression, TableQueryProvider queryProvider)
    {
      this.queryProvider = queryProvider;
      this.queryExpression = queryExpression;
    }

    public int? TakeCount
    {
      get => this.takeCount;
      set
      {
        if (value.HasValue && value.Value <= 0)
          throw new ArgumentException("Take count must be positive and greater than 0.");
        this.takeCount = value;
      }
    }

    public string FilterString { get; set; }

    public IList<string> SelectColumns { get; set; }

    public Type ElementType => typeof (TElement);

    public Expression Expression => this.queryExpression;

    public IQueryProvider Provider => (IQueryProvider) this.queryProvider;

    public TableQuery<TElement> Select(IList<string> columns)
    {
      if (this.Expression != null)
        throw new NotSupportedException(TableResources.TableQueryFluentMethodNotAllowed);
      this.SelectColumns = columns;
      return this;
    }

    public TableQuery<TElement> Take(int? take)
    {
      if (this.Expression != null)
        throw new NotSupportedException(TableResources.TableQueryFluentMethodNotAllowed);
      this.TakeCount = take;
      return this;
    }

    public TableQuery<TElement> Where(string filter)
    {
      if (this.Expression != null)
        throw new NotSupportedException(TableResources.TableQueryFluentMethodNotAllowed);
      this.FilterString = filter;
      return this;
    }

    public TableQuery<TElement> OrderBy(string propertyName)
    {
      this.ValidateOrderBy();
      this.OrderByEntities.Add(new OrderByItem(propertyName));
      return this;
    }

    public TableQuery<TElement> OrderByDesc(string propertyName)
    {
      this.ValidateOrderBy();
      this.OrderByEntities.Add(new OrderByItem(propertyName, "desc"));
      return this;
    }

    private void ValidateOrderBy()
    {
      if (this.OrderByEntities.Count >= 1)
        throw new NotSupportedException("Only single order by is supported");
    }

    internal List<OrderByItem> OrderByEntities { get; } = new List<OrderByItem>();

    public TableQuery<TElement> Copy() => new TableQuery<TElement>()
    {
      TakeCount = this.TakeCount,
      FilterString = this.FilterString,
      SelectColumns = this.SelectColumns
    };

    public virtual IEnumerable<TElement> Execute(
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      if (this.queryProvider == null)
        throw new InvalidOperationException("Unknown Table. The TableQuery does not have an associated CloudTable Reference. Please execute the query via the CloudTable ExecuteQuery APIs.");
      TableQuery<TElement>.ExecutionInfo executionInfo = this.Bind();
      executionInfo.RequestOptions = requestOptions ?? executionInfo.RequestOptions;
      executionInfo.OperationContext = operationContext ?? executionInfo.OperationContext;
      return executionInfo.Resolver != null ? this.ExecuteInternal<TElement>(this.queryProvider.Table.ServiceClient, this.queryProvider.Table, executionInfo.Resolver, executionInfo.RequestOptions, executionInfo.OperationContext) : this.ExecuteInternal(this.queryProvider.Table.ServiceClient, this.queryProvider.Table, executionInfo.RequestOptions, executionInfo.OperationContext);
    }

    public virtual Task<TableQuerySegment<TElement>> ExecuteSegmentedAsync(
      TableContinuationToken currentToken)
    {
      return this.ExecuteSegmentedAsync(currentToken, CancellationToken.None);
    }

    public virtual Task<TableQuerySegment<TElement>> ExecuteSegmentedAsync(
      TableContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ExecuteSegmentedAsync(currentToken, (TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual Task<TableQuerySegment<TElement>> ExecuteSegmentedAsync(
      TableContinuationToken currentToken,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.ExecuteSegmentedAsync(currentToken, requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<TableQuerySegment<TElement>> ExecuteSegmentedAsync(
      TableContinuationToken currentToken,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      if (this.queryProvider == null)
        throw new InvalidOperationException("Unknown Table. The TableQuery does not have an associated CloudTable Reference. Please execute the query via the CloudTable ExecuteQuery APIs.");
      TableQuery<TElement>.ExecutionInfo executionInfo = this.Bind();
      executionInfo.RequestOptions = requestOptions == null ? executionInfo.RequestOptions : requestOptions;
      executionInfo.OperationContext = operationContext == null ? executionInfo.OperationContext : operationContext;
      return executionInfo.Resolver != null ? this.ExecuteQuerySegmentedInternalAsync<TElement>(currentToken, this.queryProvider.Table.ServiceClient, this.queryProvider.Table, executionInfo.Resolver, executionInfo.RequestOptions, executionInfo.OperationContext, cancellationToken) : this.ExecuteQuerySegmentedInternalAsync(currentToken, this.queryProvider.Table.ServiceClient, this.queryProvider.Table, executionInfo.RequestOptions, executionInfo.OperationContext, cancellationToken);
    }

    public virtual TableQuerySegment<TElement> ExecuteSegmented(
      TableContinuationToken continuationToken,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      if (this.queryProvider == null)
        throw new InvalidOperationException("Unknown Table. The TableQuery does not have an associated CloudTable Reference. Please execute the query via the CloudTable ExecuteQuery APIs.");
      TableQuery<TElement>.ExecutionInfo executionInfo = this.Bind();
      executionInfo.RequestOptions = requestOptions == null ? executionInfo.RequestOptions : requestOptions;
      executionInfo.OperationContext = operationContext == null ? executionInfo.OperationContext : operationContext;
      return executionInfo.Resolver != null ? this.ExecuteQuerySegmentedInternal<TElement>(continuationToken, this.queryProvider.Table.ServiceClient, this.queryProvider.Table, executionInfo.Resolver, executionInfo.RequestOptions, executionInfo.OperationContext) : this.ExecuteQuerySegmentedInternal(continuationToken, this.queryProvider.Table.ServiceClient, this.queryProvider.Table, executionInfo.RequestOptions, executionInfo.OperationContext);
    }

    public virtual IEnumerator<TElement> GetEnumerator()
    {
      if (this.Expression == null)
        return this.ExecuteInternal(this.queryProvider.Table.ServiceClient, this.queryProvider.Table, TableRequestOptions.ApplyDefaults((TableRequestOptions) null, this.queryProvider.Table.ServiceClient), (OperationContext) null).GetEnumerator();
      TableQuery<TElement>.ExecutionInfo executionInfo = this.Bind();
      return executionInfo.Resolver != null ? this.ExecuteInternal<TElement>(this.queryProvider.Table.ServiceClient, this.queryProvider.Table, executionInfo.Resolver, executionInfo.RequestOptions, executionInfo.OperationContext).GetEnumerator() : this.ExecuteInternal(this.queryProvider.Table.ServiceClient, this.queryProvider.Table, executionInfo.RequestOptions, executionInfo.OperationContext).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    internal TableQuery<TElement>.ExecutionInfo Bind()
    {
      TableQuery<TElement>.ExecutionInfo executionInfo = new TableQuery<TElement>.ExecutionInfo();
      if (this.Expression != null)
      {
        Dictionary<Expression, Expression> rewrites = new Dictionary<Expression, Expression>((IEqualityComparer<Expression>) ReferenceEqualityComparer<Expression>.Instance);
        Expression e = ResourceBinder.Bind(ExpressionNormalizer.Normalize(Evaluator.PartialEval(this.Expression), rewrites));
        ExpressionParser parser = this.queryProvider.Table.ServiceClient.GetExpressionParser();
        parser.Translate(e);
        this.TakeCount = parser.TakeCount;
        this.FilterString = parser.FilterString;
        this.SelectColumns = parser.SelectColumns;
        executionInfo.RequestOptions = parser.RequestOptions;
        executionInfo.OperationContext = parser.OperationContext;
        if (parser.Resolver == null)
        {
          if (parser.Projection != null && parser.Projection.Selector != ProjectionQueryOptionExpression.DefaultLambda)
          {
            Type intermediateType = parser.Projection.Selector.Parameters[0].Type;
            ParameterExpression parameterExpression;
            Func<object, TElement> projectorFunc = Expression.Lambda<Func<object, TElement>>((Expression) Expression.Invoke((Expression) parser.Projection.Selector, (Expression) Expression.Convert((Expression) parameterExpression, intermediateType)), parameterExpression).Compile();
            executionInfo.Resolver = (EntityResolver<TElement>) ((pk, rk, ts, props, etag) =>
            {
              ITableEntity tableEntity = (ITableEntity) EntityUtilities.InstantiateEntityFromType(intermediateType);
              tableEntity.PartitionKey = pk;
              tableEntity.RowKey = rk;
              tableEntity.Timestamp = ts;
              tableEntity.ReadEntity(props, parser.OperationContext);
              tableEntity.ETag = etag;
              return projectorFunc((object) tableEntity);
            });
          }
        }
        else
          executionInfo.Resolver = (EntityResolver<TElement>) parser.Resolver.Value;
      }
      executionInfo.RequestOptions = TableRequestOptions.ApplyDefaults(executionInfo.RequestOptions, this.queryProvider.Table.ServiceClient);
      executionInfo.OperationContext = executionInfo.OperationContext ?? new OperationContext();
      return executionInfo;
    }

    internal IEnumerable<TElement> ExecuteInternal(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      TableRequestOptions modifiedOptions = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      Func<TableContinuationToken, ResultSegment<TElement>> segmentGenerator = (Func<TableContinuationToken, ResultSegment<TElement>>) (continuationToken =>
      {
        try
        {
          TableQuerySegment<TElement> tableQuerySegment = this.ExecuteQuerySegmentedInternal(continuationToken, client, table, modifiedOptions, operationContext);
          return new ResultSegment<TElement>(tableQuerySegment.Results)
          {
            ContinuationToken = tableQuerySegment.ContinuationToken
          };
        }
        catch (StorageException ex)
        {
          int num1;
          if (ex == null)
          {
            num1 = 0;
          }
          else
          {
            int? httpStatusCode = ex.RequestInformation?.HttpStatusCode;
            int num2 = 404;
            num1 = httpStatusCode.GetValueOrDefault() == num2 & httpStatusCode.HasValue ? 1 : 0;
          }
          if (num1 != 0)
            return new ResultSegment<TElement>(new TableQuerySegment<TElement>(new List<TElement>()).Results);
          throw;
        }
      });
      int? takeCount = this.TakeCount;
      long maxValue;
      if (!takeCount.HasValue)
      {
        maxValue = long.MaxValue;
      }
      else
      {
        takeCount = this.TakeCount;
        maxValue = (long) takeCount.Value;
      }
      return CommonUtility.LazyEnumerable<TElement>(segmentGenerator, maxValue);
    }

    internal IEnumerable<TResult> ExecuteInternal<TResult>(
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      CommonUtility.AssertNotNull(nameof (resolver), (object) resolver);
      TableRequestOptions modifiedOptions = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      return CommonUtility.LazyEnumerable<TResult>((Func<TableContinuationToken, ResultSegment<TResult>>) (continuationToken =>
      {
        TableQuerySegment<TResult> tableQuerySegment = this.ExecuteQuerySegmentedInternal<TResult>(continuationToken, client, table, resolver, modifiedOptions, operationContext);
        return new ResultSegment<TResult>(tableQuerySegment.Results)
        {
          ContinuationToken = tableQuerySegment.ContinuationToken
        };
      }), this.takeCount.HasValue ? (long) this.takeCount.Value : long.MaxValue);
    }

    internal TableQuerySegment<TElement> ExecuteQuerySegmentedInternal(
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      try
      {
        CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
        TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, client);
        operationContext = operationContext ?? new OperationContext();
        return client.Executor.ExecuteQuerySegmented<TElement, TElement>(this, token, client, table, (EntityResolver<TElement>) null, requestOptions1, operationContext);
      }
      catch (StorageException ex)
      {
        int num1;
        if (ex == null)
        {
          num1 = 0;
        }
        else
        {
          int? httpStatusCode = ex.RequestInformation?.HttpStatusCode;
          int num2 = 404;
          num1 = httpStatusCode.GetValueOrDefault() == num2 & httpStatusCode.HasValue ? 1 : 0;
        }
        if (num1 != 0)
          return new TableQuerySegment<TElement>(new TableQuerySegment<TElement>(new List<TElement>()).Results);
        throw;
      }
    }

    internal TableQuerySegment<TResult> ExecuteQuerySegmentedInternal<TResult>(
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      CommonUtility.AssertNotNull(nameof (resolver), (object) resolver);
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      return client.Executor.ExecuteQuerySegmented<TResult, TElement>(this, token, client, table, resolver, requestOptions1, operationContext);
    }

    internal Task<TableQuerySegment<TElement>> ExecuteQuerySegmentedInternalAsync(
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      return client.Executor.ExecuteQuerySegmentedAsync<TElement, TElement>(this, token, client, table, (EntityResolver<TElement>) null, requestOptions1, operationContext, cancellationToken);
    }

    internal Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedInternalAsync<TResult>(
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      CommonUtility.AssertNotNull(nameof (resolver), (object) resolver);
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      return client.Executor.ExecuteQuerySegmentedAsync<TResult, TElement>(this, token, client, table, resolver, requestOptions1, operationContext, cancellationToken);
    }

    internal class ExecutionInfo
    {
      public OperationContext OperationContext { get; set; }

      public TableRequestOptions RequestOptions { get; set; }

      public EntityResolver<TElement> Resolver { get; set; }
    }
  }
}
