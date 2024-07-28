// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableQuery
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table
{
  public class TableQuery
  {
    private int? takeCount;

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

    public TableQuery OrderBy(string propertyName)
    {
      this.ValidateOrderBy();
      this.OrderByEntities.Add(new OrderByItem(propertyName));
      return this;
    }

    public TableQuery OrderByDesc(string propertyName)
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

    internal List<OrderByItem> OrderByEntities { get; set; } = new List<OrderByItem>();

    public static T Project<T>(T entity, params string[] columns) => entity;

    internal IEnumerable<DynamicTableEntity> Execute(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      TableRequestOptions modifiedOptions = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      return CommonUtility.LazyEnumerable<DynamicTableEntity>((Func<TableContinuationToken, ResultSegment<DynamicTableEntity>>) (continuationToken =>
      {
        TableQuerySegment<DynamicTableEntity> tableQuerySegment = this.ExecuteQuerySegmented(continuationToken, client, table, modifiedOptions, operationContext);
        return new ResultSegment<DynamicTableEntity>(tableQuerySegment.Results)
        {
          ContinuationToken = tableQuerySegment.ContinuationToken
        };
      }), this.takeCount.HasValue ? (long) this.takeCount.Value : long.MaxValue);
    }

    internal IEnumerable<TResult> Execute<TResult>(
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      TableRequestOptions modifiedOptions = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      return CommonUtility.LazyEnumerable<TResult>((Func<TableContinuationToken, ResultSegment<TResult>>) (continuationToken =>
      {
        TableQuerySegment<TResult> tableQuerySegment = this.ExecuteQuerySegmented<TResult>(continuationToken, client, table, resolver, modifiedOptions, operationContext);
        return new ResultSegment<TResult>(tableQuerySegment.Results)
        {
          ContinuationToken = tableQuerySegment.ContinuationToken
        };
      }), this.takeCount.HasValue ? (long) this.takeCount.Value : long.MaxValue);
    }

    internal TableQuerySegment<DynamicTableEntity> ExecuteQuerySegmented(
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      return client.Executor.ExecuteQuerySegmented<DynamicTableEntity>(this, token, client, table, (EntityResolver<DynamicTableEntity>) null, requestOptions1, operationContext);
    }

    internal TableQuerySegment<TResult> ExecuteQuerySegmented<TResult>(
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
      return client.Executor.ExecuteQuerySegmented<TResult>(this, token, client, table, resolver, requestOptions1, operationContext);
    }

    internal Task<TableQuerySegment<DynamicTableEntity>> ExecuteQuerySegmentedAsync(
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
      return client.Executor.ExecuteQuerySegmentedAsync<DynamicTableEntity>(this, token, client, table, (EntityResolver<DynamicTableEntity>) null, requestOptions1, operationContext, cancellationToken);
    }

    internal Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      return client.Executor.ExecuteQuerySegmentedAsync<TResult>(this, token, client, table, resolver, requestOptions1, operationContext, cancellationToken);
    }

    public static string GenerateFilterCondition(
      string propertyName,
      string operation,
      string givenValue)
    {
      givenValue = givenValue ?? string.Empty;
      return TableQuery.GenerateFilterCondition(propertyName, operation, givenValue, EdmType.String);
    }

    public static string GenerateFilterConditionForBool(
      string propertyName,
      string operation,
      bool givenValue)
    {
      return TableQuery.GenerateFilterCondition(propertyName, operation, givenValue ? "true" : "false", EdmType.Boolean);
    }

    public static string GenerateFilterConditionForBinary(
      string propertyName,
      string operation,
      byte[] givenValue)
    {
      CommonUtility.AssertNotNull("value", (object) givenValue);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in givenValue)
        stringBuilder.AppendFormat("{0:x2}", (object) num);
      return TableQuery.GenerateFilterCondition(propertyName, operation, stringBuilder.ToString(), EdmType.Binary);
    }

    public static string GenerateFilterConditionForDate(
      string propertyName,
      string operation,
      DateTimeOffset givenValue)
    {
      return TableQuery.GenerateFilterCondition(propertyName, operation, givenValue.UtcDateTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), EdmType.DateTime);
    }

    public static string GenerateFilterConditionForDouble(
      string propertyName,
      string operation,
      double givenValue)
    {
      return TableQuery.GenerateFilterCondition(propertyName, operation, Convert.ToString(givenValue, (IFormatProvider) CultureInfo.InvariantCulture), EdmType.Double);
    }

    public static string GenerateFilterConditionForInt(
      string propertyName,
      string operation,
      int givenValue)
    {
      return TableQuery.GenerateFilterCondition(propertyName, operation, Convert.ToString(givenValue, (IFormatProvider) CultureInfo.InvariantCulture), EdmType.Int32);
    }

    public static string GenerateFilterConditionForLong(
      string propertyName,
      string operation,
      long givenValue)
    {
      return TableQuery.GenerateFilterCondition(propertyName, operation, Convert.ToString(givenValue, (IFormatProvider) CultureInfo.InvariantCulture), EdmType.Int64);
    }

    public static string GenerateFilterConditionForGuid(
      string propertyName,
      string operation,
      Guid givenValue)
    {
      CommonUtility.AssertNotNull("value", (object) givenValue);
      return TableQuery.GenerateFilterCondition(propertyName, operation, givenValue.ToString(), EdmType.Guid);
    }

    private static string GenerateFilterCondition(
      string propertyName,
      string operation,
      string givenValue,
      EdmType edmType)
    {
      string str;
      switch (edmType)
      {
        case EdmType.Binary:
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "X'{0}'", (object) givenValue);
          break;
        case EdmType.Boolean:
        case EdmType.Int32:
          str = givenValue;
          break;
        case EdmType.DateTime:
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "datetime'{0}'", (object) givenValue);
          break;
        case EdmType.Double:
          str = int.TryParse(givenValue, out int _) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.0", (object) givenValue) : givenValue;
          break;
        case EdmType.Guid:
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "guid'{0}'", (object) givenValue);
          break;
        case EdmType.Int64:
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}L", (object) givenValue);
          break;
        default:
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", (object) givenValue.Replace("'", "''"));
          break;
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2}", (object) propertyName, (object) operation, (object) str);
    }

    public static string CombineFilters(string filterA, string operatorString, string filterB) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0}) {1} ({2})", (object) filterA, (object) operatorString, (object) filterB);

    public TableQuery Select(IList<string> columns)
    {
      this.SelectColumns = columns;
      return this;
    }

    public TableQuery Take(int? take)
    {
      this.TakeCount = take;
      return this;
    }

    public TableQuery Where(string filter)
    {
      this.FilterString = filter;
      return this;
    }

    public TableQuery Copy() => new TableQuery()
    {
      TakeCount = this.TakeCount,
      FilterString = this.FilterString,
      SelectColumns = this.SelectColumns,
      OrderByEntities = this.OrderByEntities
    };
  }
}
