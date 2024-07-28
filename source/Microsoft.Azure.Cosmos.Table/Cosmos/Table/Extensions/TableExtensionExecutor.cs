// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.TableExtensionExecutor
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.Extensions
{
  internal sealed class TableExtensionExecutor : IExecutor
  {
    internal string TombstoneFieldName { get; set; }

    public TableExtensionExecutor() => this.TombstoneFieldName = (string) null;

    public TResult ExecuteTableOperation<TResult>(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
      where TResult : class
    {
      return TaskHelper.InlineIfPossible<TResult>((Func<Task<TResult>>) (() => this.ExecuteTableOperationAsync<TResult>(operation, client, table, requestOptions, operationContext, CancellationToken.None)), (Microsoft.Azure.Documents.IRetryPolicy) null).GetAwaiter().GetResult();
    }

    public TResult ExecuteTableBatchOperation<TResult>(
      TableBatchOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
      where TResult : class
    {
      return TaskHelper.InlineIfPossible<TResult>((Func<Task<TResult>>) (() => this.ExecuteTableBatchOperationAsync<TResult>(operation, client, table, requestOptions, operationContext, CancellationToken.None)), (Microsoft.Azure.Documents.IRetryPolicy) null).GetAwaiter().GetResult();
    }

    public ServiceProperties GetServicePropertiesOperation(
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      throw new NotSupportedException("The operation is not supported by Azure Cosmos Table endpoints. ");
    }

    public void SetServicePropertiesOperation(
      ServiceProperties properties,
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      throw new NotSupportedException("The operation is not supported by Azure Cosmos Table endpoints. ");
    }

    public ServiceStats GetServiceStats(
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      throw new NotSupportedException("The operation is not supported by Azure Cosmos Table endpoints. ");
    }

    public TablePermissions GetTablePermissions(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      throw new NotSupportedException("The operation is not supported by Azure Cosmos Table endpoints. ");
    }

    public void SetTablePermissions(
      TablePermissions permissions,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      throw new NotSupportedException("The operation is not supported by Azure Cosmos Table endpoints. ");
    }

    public Task<TResult> ExecuteTableOperationAsync<TResult>(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
      where TResult : class
    {
      operationContext = operationContext ?? new OperationContext();
      return TableExtensionRetryPolicy.Execute<TResult>((Func<Task<TResult>>) (() => TableExtensionOperationHelper.ExecuteOperationAsync<TResult>(operation, client, table, requestOptions, operationContext, cancellationToken)), cancellationToken, operationContext, requestOptions);
    }

    public Task<TResult> ExecuteTableBatchOperationAsync<TResult>(
      TableBatchOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
      where TResult : class
    {
      operationContext = operationContext ?? new OperationContext();
      return TableExtensionRetryPolicy.Execute<TResult>((Func<Task<TResult>>) (() => TableExtensionOperationHelper.ExecuteBatchOperationAsync<TResult>(operation, client, table, requestOptions, operationContext, cancellationToken)), cancellationToken, operationContext, requestOptions);
    }

    public Task<ServiceProperties> GetServicePropertiesOperationAsync(
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException("The operation is not supported by Azure Cosmos Table endpoints. ");
    }

    public Task SetServicePropertiesOperationAsync(
      ServiceProperties properties,
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException("The operation is not supported by Azure Cosmos Table endpoints. ");
    }

    public Task<ServiceStats> GetServiceStatsAsync(
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException("The operation is not supported by Azure Cosmos Table endpoints. ");
    }

    public Task<TablePermissions> GetTablePermissionsAsync(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException("The operation is not supported by Azure Cosmos Table endpoints. ");
    }

    public Task SetTablePermissionsAsync(
      TablePermissions permissions,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException("The operation is not supported by Azure Cosmos Table endpoints. ");
    }

    public TableQuerySegment<TResult> ExecuteQuerySegmented<TResult, TInput>(
      TableQuery<TInput> query,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return TaskHelper.InlineIfPossible<TableQuerySegment<TResult>>((Func<Task<TableQuerySegment<TResult>>>) (() => this.ExecuteQuerySegmentedInternalAsync<TResult, TInput>(query, token, client, table, resolver ?? new EntityResolver<TResult>(EntityUtilities.ResolveEntityByType<TResult>), requestOptions, operationContext, CancellationToken.None, query.Expression != null)), (Microsoft.Azure.Documents.IRetryPolicy) null).GetAwaiter().GetResult();
    }

    public TableQuerySegment<TResult> ExecuteQuerySegmented<TResult>(
      TableQuery query,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return TaskHelper.InlineIfPossible<TableQuerySegment<TResult>>((Func<Task<TableQuerySegment<TResult>>>) (() => this.ExecuteQuerySegmentedInternalAsync<TResult>(query, token, client, table, resolver ?? new EntityResolver<TResult>(EntityUtilities.ResolveEntityByType<TResult>), requestOptions, operationContext, CancellationToken.None, false)), (Microsoft.Azure.Documents.IRetryPolicy) null).GetAwaiter().GetResult();
    }

    public Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult, TInput>(
      TableQuery<TInput> query,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.ExecuteQuerySegmentedInternalAsync<TResult, TInput>(query, token, client, table, resolver ?? new EntityResolver<TResult>(EntityUtilities.ResolveEntityByType<TResult>), requestOptions, operationContext, cancellationToken, query.Expression != null);
    }

    public Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(
      TableQuery query,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.ExecuteQuerySegmentedInternalAsync<TResult>(query, token, client, table, resolver ?? new EntityResolver<TResult>(EntityUtilities.ResolveEntityByType<TResult>), requestOptions, operationContext, cancellationToken, false);
    }

    private Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedInternalAsync<TResult>(
      TableQuery query,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken,
      bool isLinq)
    {
      operationContext = operationContext ?? new OperationContext();
      return TableExtensionRetryPolicy.Execute<TableQuerySegment<TResult>>((Func<Task<TableQuerySegment<TResult>>>) (async () =>
      {
        TableQuerySegment<TResult> tableQuerySegment;
        try
        {
          tableQuerySegment = await TableExtensionQueryHelper.QueryDocumentsAsync<TResult>(query.TakeCount, string.IsNullOrEmpty(query.FilterString) ? query.FilterString : ODataV3Translator.TranslateFilter(query.FilterString, false), query.SelectColumns, token, client, table, resolver, requestOptions, operationContext, isLinq, (IList<OrderByItem>) query.OrderByEntities, this.TombstoneFieldName);
        }
        catch (Exception ex)
        {
          StorageException resultFromException = EntityHelpers.GetTableResultFromException(ex);
          operationContext.RequestResults.Add(resultFromException.RequestInformation);
          throw resultFromException;
        }
        return tableQuerySegment;
      }), cancellationToken, operationContext, requestOptions);
    }

    private Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedInternalAsync<TResult, TInput>(
      TableQuery<TInput> query,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken,
      bool isLinq)
    {
      operationContext = operationContext ?? new OperationContext();
      return TableExtensionRetryPolicy.Execute<TableQuerySegment<TResult>>((Func<Task<TableQuerySegment<TResult>>>) (async () =>
      {
        try
        {
          return string.Equals(table.Name, "Tables", StringComparison.OrdinalIgnoreCase) ? await TableExtensionQueryHelper.QueryCollectionsAsync<TResult>(query.TakeCount, string.IsNullOrEmpty(query.FilterString) ? query.FilterString : ODataV3Translator.TranslateFilter(query.FilterString, false), token, client, table, requestOptions, operationContext) : await TableExtensionQueryHelper.QueryDocumentsAsync<TResult>(query.TakeCount, string.IsNullOrEmpty(query.FilterString) ? query.FilterString : ODataV3Translator.TranslateFilter(query.FilterString, false), query.SelectColumns, token, client, table, resolver, requestOptions, operationContext, isLinq, (IList<OrderByItem>) query.OrderByEntities, this.TombstoneFieldName);
        }
        catch (Exception ex)
        {
          StorageException resultFromException = EntityHelpers.GetTableResultFromException(ex);
          operationContext.RequestResults.Add(resultFromException.RequestInformation);
          throw resultFromException;
        }
      }), cancellationToken, operationContext, requestOptions);
    }
  }
}
