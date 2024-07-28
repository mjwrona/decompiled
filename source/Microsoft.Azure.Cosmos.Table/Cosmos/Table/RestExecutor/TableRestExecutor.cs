// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableRestExecutor
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor
{
  internal sealed class TableRestExecutor : IExecutor
  {
    public TResult ExecuteTableOperation<TResult>(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
      where TResult : class
    {
      return Executor.ExecuteSync<TResult>(TableOperationRESTCommandGenerator.GenerateCMDForTableOperation(operation, client, table, requestOptions) as RESTCommand<TResult>, requestOptions.RetryPolicy, operationContext);
    }

    public TResult ExecuteTableBatchOperation<TResult>(
      TableBatchOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
      where TResult : class
    {
      return Executor.ExecuteSync<TResult>(TableBatchOperationRESTCommandGenerator.GenerateCMDForTableBatchOperation(operation, client, table, requestOptions) as RESTCommand<TResult>, requestOptions.RetryPolicy, operationContext);
    }

    public ServiceProperties GetServicePropertiesOperation(
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return Executor.ExecuteSync<ServiceProperties>(TableServicePropertiesRESTCommandGenerator.GetServicePropertiesImpl(client, requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    public void SetServicePropertiesOperation(
      ServiceProperties properties,
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      Executor.ExecuteSync<NullType>(TableServicePropertiesRESTCommandGenerator.SetServicePropertiesImpl(properties, client, requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    public ServiceStats GetServiceStats(
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return Executor.ExecuteSync<ServiceStats>(TableServiceStatsRESTCommandGenerator.GenerateCMDForGetServiceStats(client, requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    public TablePermissions GetTablePermissions(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return Executor.ExecuteSync<TablePermissions>(TablePermissionsRESTCommandGenerator.GetAclImpl(client, table, requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    public void SetTablePermissions(
      TablePermissions permissions,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      Executor.ExecuteSync<NullType>(TablePermissionsRESTCommandGenerator.SetAclImpl(permissions, client, table, requestOptions), requestOptions.RetryPolicy, operationContext);
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
      return Executor.ExecuteAsync<TResult>(TableOperationRESTCommandGenerator.GenerateCMDForTableOperation(operation, client, table, requestOptions) as RESTCommand<TResult>, requestOptions.RetryPolicy, operationContext, cancellationToken);
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
      return Executor.ExecuteAsync<TResult>(TableBatchOperationRESTCommandGenerator.GenerateCMDForTableBatchOperation(operation, client, table, requestOptions) as RESTCommand<TResult>, requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    public Task<ServiceProperties> GetServicePropertiesOperationAsync(
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return Executor.ExecuteAsync<ServiceProperties>(TableServicePropertiesRESTCommandGenerator.GetServicePropertiesImpl(client, requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    public Task SetServicePropertiesOperationAsync(
      ServiceProperties properties,
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return (Task) Executor.ExecuteAsync<NullType>(TableServicePropertiesRESTCommandGenerator.SetServicePropertiesImpl(properties, client, requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    public Task<ServiceStats> GetServiceStatsAsync(
      CloudTableClient client,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return Executor.ExecuteAsync<ServiceStats>(TableServiceStatsRESTCommandGenerator.GenerateCMDForGetServiceStats(client, requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    public Task<TablePermissions> GetTablePermissionsAsync(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return Executor.ExecuteAsync<TablePermissions>(TablePermissionsRESTCommandGenerator.GetAclImpl(client, table, requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    public Task SetTablePermissionsAsync(
      TablePermissions permissions,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return (Task) Executor.ExecuteAsync<NullType>(TablePermissionsRESTCommandGenerator.SetAclImpl(permissions, client, table, requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
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
      return Executor.ExecuteSync<TableQuerySegment<TResult>>(TableQueryRESTCommandGenerator.GenerateCMDForTableQuery<TInput, TResult>(query, token, client, table, resolver ?? new EntityResolver<TResult>(EntityUtilities.ResolveEntityByType<TResult>), requestOptions), requestOptions.RetryPolicy, operationContext);
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
      return Executor.ExecuteSync<TableQuerySegment<TResult>>(typeof (TResult) == typeof (DynamicTableEntity) ? TableQueryRESTCommandGenerator.GenerateCMDForTableQuery(query, token, client, table, requestOptions) as RESTCommand<TableQuerySegment<TResult>> : TableQueryRESTCommandGenerator.GenerateCMDForTableQuery<TResult>(query, token, client, table, resolver ?? new EntityResolver<TResult>(EntityUtilities.ResolveEntityByType<TResult>), requestOptions), requestOptions.RetryPolicy, operationContext);
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
      return Executor.ExecuteAsync<TableQuerySegment<TResult>>(TableQueryRESTCommandGenerator.GenerateCMDForTableQuery<TInput, TResult>(query, token, client, table, resolver ?? new EntityResolver<TResult>(EntityUtilities.ResolveEntityByType<TResult>), requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
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
      return Executor.ExecuteAsync<TableQuerySegment<TResult>>(typeof (TResult) == typeof (DynamicTableEntity) ? TableQueryRESTCommandGenerator.GenerateCMDForTableQuery(query, token, client, table, requestOptions) as RESTCommand<TableQuerySegment<TResult>> : TableQueryRESTCommandGenerator.GenerateCMDForTableQuery<TResult>(query, token, client, table, resolver ?? new EntityResolver<TResult>(EntityUtilities.ResolveEntityByType<TResult>), requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }
  }
}
