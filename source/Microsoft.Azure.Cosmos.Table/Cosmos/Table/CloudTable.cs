// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.CloudTable
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.Protocol;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table
{
  public class CloudTable
  {
    public CloudTable(Uri tableAddress, TableClientConfiguration configuration = null)
      : this(tableAddress, (StorageCredentials) null, configuration)
    {
    }

    public CloudTable(
      Uri tableAbsoluteUri,
      StorageCredentials credentials,
      TableClientConfiguration configuration = null)
      : this(new StorageUri(tableAbsoluteUri), credentials, configuration)
    {
    }

    public CloudTable(
      StorageUri tableAddress,
      StorageCredentials credentials,
      TableClientConfiguration configuration = null)
    {
      this.ParseQueryAndVerify(tableAddress, credentials, configuration);
    }

    internal CloudTable(string tableName, CloudTableClient client)
    {
      CommonUtility.AssertNotNull(nameof (tableName), (object) tableName);
      CommonUtility.AssertNotNull(nameof (client), (object) client);
      this.Name = tableName;
      this.StorageUri = NavigationHelper.AppendPathToUri(client.StorageUri, tableName);
      this.ServiceClient = client;
    }

    public CloudTableClient ServiceClient { get; private set; }

    public string Name { get; private set; }

    public Uri Uri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; private set; }

    public override string ToString() => this.Name;

    public virtual TableResult Execute(
      TableOperation operation,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (operation), (object) operation);
      return operation.Execute(this.ServiceClient, this, requestOptions, operationContext);
    }

    public virtual Task<TableResult> ExecuteAsync(TableOperation operation) => this.ExecuteAsync(operation, CancellationToken.None);

    public virtual Task<TableResult> ExecuteAsync(
      TableOperation operation,
      CancellationToken cancellationToken)
    {
      return this.ExecuteAsync(operation, (TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual Task<TableResult> ExecuteAsync(
      TableOperation operation,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.ExecuteAsync(operation, requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<TableResult> ExecuteAsync(
      TableOperation operation,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return operation.ExecuteAsync(this.ServiceClient, this, requestOptions, operationContext, cancellationToken);
    }

    public virtual TableBatchResult ExecuteBatch(
      TableBatchOperation batch,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (batch), (object) batch);
      return batch.Execute(this.ServiceClient, this, requestOptions, operationContext);
    }

    public virtual Task<TableBatchResult> ExecuteBatchAsync(TableBatchOperation batch) => this.ExecuteBatchAsync(batch, CancellationToken.None);

    public virtual Task<TableBatchResult> ExecuteBatchAsync(
      TableBatchOperation batch,
      CancellationToken cancellationToken)
    {
      return this.ExecuteBatchAsync(batch, (TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual Task<TableBatchResult> ExecuteBatchAsync(
      TableBatchOperation batch,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.ExecuteBatchAsync(batch, requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<TableBatchResult> ExecuteBatchAsync(
      TableBatchOperation batch,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (batch), (object) batch);
      return batch.ExecuteAsync(this.ServiceClient, this, requestOptions, operationContext, cancellationToken);
    }

    public virtual IEnumerable<DynamicTableEntity> ExecuteQuery(
      TableQuery query,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      return query.Execute(this.ServiceClient, this, requestOptions, operationContext);
    }

    public virtual TableQuerySegment<DynamicTableEntity> ExecuteQuerySegmented(
      TableQuery query,
      TableContinuationToken token,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      return query.ExecuteQuerySegmented(token, this.ServiceClient, this, requestOptions, operationContext);
    }

    public virtual Task<TableQuerySegment<DynamicTableEntity>> ExecuteQuerySegmentedAsync(
      TableQuery query,
      TableContinuationToken token)
    {
      return this.ExecuteQuerySegmentedAsync(query, token, CancellationToken.None);
    }

    public virtual Task<TableQuerySegment<DynamicTableEntity>> ExecuteQuerySegmentedAsync(
      TableQuery query,
      TableContinuationToken token,
      CancellationToken cancellationToken)
    {
      return this.ExecuteQuerySegmentedAsync(query, token, (TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual Task<TableQuerySegment<DynamicTableEntity>> ExecuteQuerySegmentedAsync(
      TableQuery query,
      TableContinuationToken token,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.ExecuteQuerySegmentedAsync(query, token, requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<TableQuerySegment<DynamicTableEntity>> ExecuteQuerySegmentedAsync(
      TableQuery query,
      TableContinuationToken token,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      return query.ExecuteQuerySegmentedAsync(token, this.ServiceClient, this, requestOptions, operationContext, cancellationToken);
    }

    public virtual IEnumerable<TResult> ExecuteQuery<TResult>(
      TableQuery query,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      CommonUtility.AssertNotNull(nameof (resolver), (object) resolver);
      return query.Execute<TResult>(this.ServiceClient, this, resolver, requestOptions, operationContext);
    }

    public virtual TableQuerySegment<TResult> ExecuteQuerySegmented<TResult>(
      TableQuery query,
      EntityResolver<TResult> resolver,
      TableContinuationToken token,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      return query.ExecuteQuerySegmented<TResult>(token, this.ServiceClient, this, resolver, requestOptions, operationContext);
    }

    public virtual Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(
      TableQuery query,
      EntityResolver<TResult> resolver,
      TableContinuationToken token)
    {
      return this.ExecuteQuerySegmentedAsync<TResult>(query, resolver, token, CancellationToken.None);
    }

    public virtual Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(
      TableQuery query,
      EntityResolver<TResult> resolver,
      TableContinuationToken token,
      CancellationToken cancellationToken)
    {
      return this.ExecuteQuerySegmentedAsync<TResult>(query, resolver, token, (TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(
      TableQuery query,
      EntityResolver<TResult> resolver,
      TableContinuationToken token,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.ExecuteQuerySegmentedAsync<TResult>(query, resolver, token, requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(
      TableQuery query,
      EntityResolver<TResult> resolver,
      TableContinuationToken token,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      CommonUtility.AssertNotNull(nameof (resolver), (object) resolver);
      return query.ExecuteQuerySegmentedAsync<TResult>(token, this.ServiceClient, this, resolver, requestOptions, operationContext, cancellationToken);
    }

    public virtual TableQuery<TElement> CreateQuery<TElement>() where TElement : ITableEntity, new() => new TableQuery<TElement>(this);

    public virtual IEnumerable<TElement> ExecuteQuery<TElement>(
      TableQuery<TElement> query,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
      where TElement : ITableEntity, new()
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      return query.Provider != null ? query.Execute(requestOptions, operationContext) : query.ExecuteInternal(this.ServiceClient, this, requestOptions, operationContext);
    }

    public virtual TableQuerySegment<TElement> ExecuteQuerySegmented<TElement>(
      TableQuery<TElement> query,
      TableContinuationToken token,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
      where TElement : ITableEntity, new()
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      return query.Provider != null ? query.ExecuteSegmented(token, requestOptions, operationContext) : query.ExecuteQuerySegmentedInternal(token, this.ServiceClient, this, requestOptions, operationContext);
    }

    public virtual Task<TableQuerySegment<TElement>> ExecuteQuerySegmentedAsync<TElement>(
      TableQuery<TElement> query,
      TableContinuationToken token)
      where TElement : ITableEntity, new()
    {
      return this.ExecuteQuerySegmentedAsync<TElement>(query, token, CancellationToken.None);
    }

    public virtual Task<TableQuerySegment<TElement>> ExecuteQuerySegmentedAsync<TElement>(
      TableQuery<TElement> query,
      TableContinuationToken token,
      CancellationToken cancellationToken)
      where TElement : ITableEntity, new()
    {
      return this.ExecuteQuerySegmentedAsync<TElement>(query, token, (TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual Task<TableQuerySegment<TElement>> ExecuteQuerySegmentedAsync<TElement>(
      TableQuery<TElement> query,
      TableContinuationToken token,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
      where TElement : ITableEntity, new()
    {
      return this.ExecuteQuerySegmentedAsync<TElement>(query, token, requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<TableQuerySegment<TElement>> ExecuteQuerySegmentedAsync<TElement>(
      TableQuery<TElement> query,
      TableContinuationToken token,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
      where TElement : ITableEntity, new()
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      return query.Provider != null ? query.ExecuteSegmentedAsync(token, requestOptions, operationContext, cancellationToken) : query.ExecuteQuerySegmentedInternalAsync(token, this.ServiceClient, this, requestOptions, operationContext, cancellationToken);
    }

    public virtual IEnumerable<TResult> ExecuteQuery<TElement, TResult>(
      TableQuery<TElement> query,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
      where TElement : ITableEntity, new()
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      CommonUtility.AssertNotNull(nameof (resolver), (object) resolver);
      return query.Provider != null ? query.Resolve<TElement, TResult>(resolver).Execute(requestOptions, operationContext) : query.ExecuteInternal<TResult>(this.ServiceClient, this, resolver, requestOptions, operationContext);
    }

    public virtual TableQuerySegment<TResult> ExecuteQuerySegmented<TElement, TResult>(
      TableQuery<TElement> query,
      EntityResolver<TResult> resolver,
      TableContinuationToken token,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
      where TElement : ITableEntity, new()
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      CommonUtility.AssertNotNull(nameof (resolver), (object) resolver);
      return query.Provider != null ? query.Resolve<TElement, TResult>(resolver).ExecuteSegmented(token, requestOptions, operationContext) : query.ExecuteQuerySegmentedInternal<TResult>(token, this.ServiceClient, this, resolver, requestOptions, operationContext);
    }

    public virtual Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TElement, TResult>(
      TableQuery<TElement> query,
      EntityResolver<TResult> resolver,
      TableContinuationToken token)
      where TElement : ITableEntity, new()
    {
      return this.ExecuteQuerySegmentedAsync<TElement, TResult>(query, resolver, token, CancellationToken.None);
    }

    public virtual Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TElement, TResult>(
      TableQuery<TElement> query,
      EntityResolver<TResult> resolver,
      TableContinuationToken token,
      CancellationToken cancellationToken)
      where TElement : ITableEntity, new()
    {
      return this.ExecuteQuerySegmentedAsync<TElement, TResult>(query, resolver, token, (TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TElement, TResult>(
      TableQuery<TElement> query,
      EntityResolver<TResult> resolver,
      TableContinuationToken token,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
      where TElement : ITableEntity, new()
    {
      return this.ExecuteQuerySegmentedAsync<TElement, TResult>(query, resolver, token, requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TElement, TResult>(
      TableQuery<TElement> query,
      EntityResolver<TResult> resolver,
      TableContinuationToken token,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
      where TElement : ITableEntity, new()
    {
      CommonUtility.AssertNotNull(nameof (query), (object) query);
      CommonUtility.AssertNotNull(nameof (resolver), (object) resolver);
      return query.Provider != null ? query.Resolve<TElement, TResult>(resolver).ExecuteSegmentedAsync(token, requestOptions, operationContext, cancellationToken) : query.ExecuteQuerySegmentedInternalAsync<TResult>(token, this.ServiceClient, this, resolver, requestOptions, operationContext, cancellationToken);
    }

    public virtual void Create(Microsoft.Azure.Cosmos.IndexingMode? indexingMode, int? throughput = null) => this.Create(serializedIndexingPolicy: CloudTable.ToSerializedIndexingPolicy(indexingMode), throughput: throughput);

    public virtual void Create(
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null,
      string serializedIndexingPolicy = null,
      int? throughput = null)
    {
      this.ServiceClient.AssertPremiumFeaturesOnlyToCosmosTables(throughput, serializedIndexingPolicy);
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
      dynamicTableEntity.SetCosmosTableName(this.Name);
      dynamicTableEntity.SetCosmosTableThroughput(throughput);
      dynamicTableEntity.SetCosmosTableIndexingPolicy(serializedIndexingPolicy);
      new TableOperation((ITableEntity) dynamicTableEntity, TableOperationType.Insert, false)
      {
        IsTableEntity = true
      }.Execute(this.ServiceClient, this.ServiceClient.GetTableReference("Tables"), requestOptions, operationContext);
    }

    public virtual Task CreateAsync() => this.CreateAsync(CancellationToken.None);

    public virtual Task CreateAsync(CancellationToken cancellationToken) => this.CreateAsync(new int?(), (string) null, cancellationToken);

    public virtual Task CreateAsync(
      int? throughput,
      Microsoft.Azure.Cosmos.IndexingMode indexingMode,
      CancellationToken cancellationToken)
    {
      return this.CreateAsync(throughput, CloudTable.ToSerializedIndexingPolicy(new Microsoft.Azure.Cosmos.IndexingMode?(indexingMode)), cancellationToken);
    }

    public virtual Task CreateAsync(
      int? throughput,
      string serializedIndexingPolicy,
      CancellationToken cancellationToken)
    {
      return this.CreateAsync((TableRequestOptions) null, (OperationContext) null, serializedIndexingPolicy, throughput, cancellationToken);
    }

    public virtual Task CreateAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.CreateAsync(requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task CreateAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.CreateAsync(requestOptions, operationContext, (string) null, new int?(), cancellationToken);
    }

    public virtual Task CreateAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      string serializedIndexingPolicy,
      int? throughput,
      CancellationToken cancellationToken)
    {
      this.ServiceClient.AssertPremiumFeaturesOnlyToCosmosTables(throughput, serializedIndexingPolicy);
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
      dynamicTableEntity.SetCosmosTableName(this.Name);
      dynamicTableEntity.SetCosmosTableThroughput(throughput);
      dynamicTableEntity.SetCosmosTableIndexingPolicy(serializedIndexingPolicy);
      return (Task) new TableOperation((ITableEntity) dynamicTableEntity, TableOperationType.Insert, false)
      {
        IsTableEntity = true
      }.ExecuteAsync(this.ServiceClient, this.ServiceClient.GetTableReference("Tables"), requestOptions, operationContext, cancellationToken);
    }

    public virtual bool CreateIfNotExists(Microsoft.Azure.Cosmos.IndexingMode indexingMode, int? throughput = null) => this.CreateIfNotExists(serializedIndexingPolicy: CloudTable.ToSerializedIndexingPolicy(new Microsoft.Azure.Cosmos.IndexingMode?(indexingMode)), throughput: throughput);

    public virtual bool CreateIfNotExists(
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null,
      string serializedIndexingPolicy = null,
      int? throughput = null)
    {
      try
      {
        this.Create(requestOptions, operationContext, serializedIndexingPolicy, throughput);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 409 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == TableErrorCodeStrings.TableAlreadyExists))
          return false;
        throw;
      }
    }

    public virtual Task<bool> CreateIfNotExistsAsync() => this.CreateIfNotExistsAsync(CancellationToken.None);

    public virtual Task<bool> CreateIfNotExistsAsync(CancellationToken cancellationToken) => this.CreateIfNotExistsAsync((TableRequestOptions) null, (OperationContext) null, cancellationToken);

    public virtual Task<bool> CreateIfNotExistsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.CreateIfNotExistsAsync(requestOptions, operationContext, (string) null, new int?(), CancellationToken.None);
    }

    public virtual Task<bool> CreateIfNotExistsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.CreateIfNotExistsAsync(requestOptions, operationContext, (string) null, new int?(), cancellationToken);
    }

    public virtual Task<bool> CreateIfNotExistsAsync(
      Microsoft.Azure.Cosmos.IndexingMode indexingMode,
      int? throughput,
      CancellationToken cancellationToken)
    {
      return this.CreateIfNotExistsAsync((TableRequestOptions) null, (OperationContext) null, CloudTable.ToSerializedIndexingPolicy(new Microsoft.Azure.Cosmos.IndexingMode?(indexingMode)), throughput, cancellationToken);
    }

    public virtual async Task<bool> CreateIfNotExistsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      string serializedIndexingPolicy,
      int? throughput,
      CancellationToken cancellationToken)
    {
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        await this.CreateAsync(requestOptions1, operationContext, serializedIndexingPolicy, throughput, cancellationToken);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 409 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == TableErrorCodeStrings.TableAlreadyExists))
          return false;
        throw;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public virtual void Delete(
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      new TableOperation((ITableEntity) new DynamicTableEntity()
      {
        Properties = {
          {
            "TableName",
            new EntityProperty(this.Name)
          }
        }
      }, TableOperationType.Delete)
      {
        IsTableEntity = true
      }.Execute(this.ServiceClient, this.ServiceClient.GetTableReference("Tables"), requestOptions, operationContext);
    }

    public virtual Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public virtual Task DeleteAsync(CancellationToken cancellationToken) => this.DeleteAsync((TableRequestOptions) null, (OperationContext) null, cancellationToken);

    public virtual Task DeleteAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.DeleteAsync(requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task DeleteAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) new TableOperation((ITableEntity) new DynamicTableEntity()
      {
        Properties = {
          {
            "TableName",
            new EntityProperty(this.Name)
          }
        }
      }, TableOperationType.Delete)
      {
        IsTableEntity = true
      }.ExecuteAsync(this.ServiceClient, this.ServiceClient.GetTableReference("Tables"), requestOptions, operationContext, cancellationToken);
    }

    public virtual bool DeleteIfExists(
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        if (!this.Exists(requestOptions1, operationContext))
          return false;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode != 403)
          throw;
      }
      try
      {
        this.Delete(requestOptions1, operationContext);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
        {
          if (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == StorageErrorCodeStrings.ResourceNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    public virtual Task<bool> DeleteIfExistsAsync() => this.DeleteIfExistsAsync(CancellationToken.None);

    public virtual Task<bool> DeleteIfExistsAsync(CancellationToken cancellationToken) => this.DeleteIfExistsAsync((TableRequestOptions) null, (OperationContext) null, cancellationToken);

    public virtual Task<bool> DeleteIfExistsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.DeleteIfExistsAsync(requestOptions, operationContext, CancellationToken.None);
    }

    public virtual async Task<bool> DeleteIfExistsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        if (!await this.ExistsAsync(requestOptions, operationContext, cancellationToken))
          return false;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation != null)
        {
          if (ex.RequestInformation.HttpStatusCode != 403)
            throw;
        }
      }
      catch (Exception ex)
      {
        throw;
      }
      cancellationToken.ThrowIfCancellationRequested();
      try
      {
        await this.DeleteAsync(requestOptions, operationContext, cancellationToken);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == StorageErrorCodeStrings.ResourceNotFound))
          return false;
        throw;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public bool Exists(TableRequestOptions requestOptions = null, OperationContext operationContext = null)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return new TableOperation((ITableEntity) new DynamicTableEntity()
      {
        Properties = {
          {
            "TableName",
            new EntityProperty(this.Name)
          }
        }
      }, TableOperationType.Retrieve)
      {
        IsTableEntity = true
      }.Execute(this.ServiceClient, this.ServiceClient.GetTableReference("Tables"), requestOptions, operationContext).HttpStatusCode == 200;
    }

    public virtual Task<bool> ExistsAsync() => this.ExistsAsync(CancellationToken.None);

    public virtual Task<bool> ExistsAsync(CancellationToken cancellationToken) => this.ExistsAsync((TableRequestOptions) null, (OperationContext) null, cancellationToken);

    public virtual Task<bool> ExistsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.ExistsAsync(requestOptions, operationContext, CancellationToken.None);
    }

    public virtual async Task<bool> ExistsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (await new TableOperation((ITableEntity) new DynamicTableEntity()
      {
        Properties = {
          {
            "TableName",
            new EntityProperty(this.Name)
          }
        }
      }, TableOperationType.Retrieve)
      {
        IsTableEntity = true
      }.ExecuteAsync(this.ServiceClient, this.ServiceClient.GetTableReference("Tables"), requestOptions, operationContext, cancellationToken)).HttpStatusCode == 200;
    }

    private static string ToSerializedIndexingPolicy(Microsoft.Azure.Cosmos.IndexingMode? mode)
    {
      IndexingPolicy indexingPolicy = new IndexingPolicy();
      if (mode.HasValue)
        indexingPolicy.IndexingMode = (Microsoft.Azure.Documents.IndexingMode) mode.Value;
      return JsonConvert.SerializeObject((object) indexingPolicy);
    }

    private void ParseQueryAndVerify(
      StorageUri address,
      StorageCredentials credentials,
      TableClientConfiguration configuration = null)
    {
      StorageCredentials parsedCredentials;
      this.StorageUri = NavigationHelper.ParseTableQueryAndVerify(address, out parsedCredentials);
      if (parsedCredentials != null && credentials != null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot provide credentials as part of the address and as constructor parameter. Either pass in the address or use a different constructor."));
      this.ServiceClient = new CloudTableClient(NavigationHelper.GetServiceClientBaseAddress(this.StorageUri, new bool?()), credentials ?? parsedCredentials, configuration);
      this.Name = NavigationHelper.GetTableNameFromUri(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris));
    }

    public virtual TablePermissions GetPermissions(
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return this.ServiceClient.Executor.GetTablePermissions(this.ServiceClient, this, requestOptions, operationContext);
    }

    public virtual Task<TablePermissions> GetPermissionsAsync() => this.GetPermissionsAsync(CancellationToken.None);

    public virtual Task<TablePermissions> GetPermissionsAsync(CancellationToken cancellationToken) => this.GetPermissionsAsync((TableRequestOptions) null, (OperationContext) null, cancellationToken);

    public virtual Task<TablePermissions> GetPermissionsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.GetPermissionsAsync(requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<TablePermissions> GetPermissionsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return this.ServiceClient.Executor.GetTablePermissionsAsync(this.ServiceClient, this, requestOptions, operationContext, cancellationToken);
    }

    public virtual void SetPermissions(
      TablePermissions permissions,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      this.ServiceClient.Executor.SetTablePermissions(permissions, this.ServiceClient, this, requestOptions, operationContext);
    }

    public virtual Task SetPermissionsAsync(TablePermissions permissions) => this.SetPermissionsAsync(permissions, CancellationToken.None);

    public virtual Task SetPermissionsAsync(
      TablePermissions permissions,
      CancellationToken cancellationToken)
    {
      return this.SetPermissionsAsync(permissions, (TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual Task SetPermissionsAsync(
      TablePermissions permissions,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.SetPermissionsAsync(permissions, requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task SetPermissionsAsync(
      TablePermissions permissions,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return this.ServiceClient.Executor.SetTablePermissionsAsync(permissions, this.ServiceClient, this, requestOptions, operationContext, cancellationToken);
    }

    public string GetSharedAccessSignature(SharedAccessTablePolicy policy) => this.GetSharedAccessSignature(policy, (string) null, (string) null, (string) null, (string) null, (string) null);

    public string GetSharedAccessSignature(
      SharedAccessTablePolicy policy,
      string accessPolicyIdentifier)
    {
      return this.GetSharedAccessSignature(policy, accessPolicyIdentifier, (string) null, (string) null, (string) null, (string) null);
    }

    public string GetSharedAccessSignature(
      SharedAccessTablePolicy policy,
      string accessPolicyIdentifier,
      string startPartitionKey,
      string startRowKey,
      string endPartitionKey,
      string endRowKey)
    {
      return this.GetSharedAccessSignature(policy, accessPolicyIdentifier, startPartitionKey, startRowKey, endPartitionKey, endRowKey, new SharedAccessProtocol?(), (IPAddressOrRange) null);
    }

    public string GetSharedAccessSignature(
      SharedAccessTablePolicy policy,
      string accessPolicyIdentifier,
      string startPartitionKey,
      string startRowKey,
      string endPartitionKey,
      string endRowKey,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange)
    {
      if (!this.ServiceClient.Credentials.IsSharedKey)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot create Shared Access Signature unless Account Key credentials are used."));
      string canonicalName = this.GetCanonicalName();
      StorageCredentials credentials = this.ServiceClient.Credentials;
      string hash = SharedAccessSignatureHelper.GetHash(policy, accessPolicyIdentifier, startPartitionKey, startRowKey, endPartitionKey, endRowKey, canonicalName, "2017-07-29", protocols, ipAddressOrRange, credentials.Key);
      return SharedAccessSignatureHelper.GetSignature(policy, this.Name, accessPolicyIdentifier, startPartitionKey, startRowKey, endPartitionKey, endRowKey, hash, credentials.KeyName, "2017-07-29", protocols, ipAddressOrRange).ToString();
    }

    private string GetCanonicalName() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}", (object) "table", (object) this.ServiceClient.Credentials.AccountName, (object) this.Name.ToLower());
  }
}
