// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl.MemoryTableAdapter
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl
{
  internal class MemoryTableAdapter : ITable
  {
    private readonly MemoryTableStorage storage;
    public IClock Clock;

    internal Action<TableBatchOperationDescriptor> PreExecuteBatchAsync { private get; set; }

    internal MemoryTableAdapter(string tableName, MemoryTableStorage storage)
    {
      this.Name = tableName;
      this.storage = storage;
      this.Clock = this.storage.testClock;
    }

    public MemoryTable Table
    {
      get
      {
        MemoryTable table;
        return this.storage.TryGetTable(this.Name, out table) ? table : (MemoryTable) null;
      }
    }

    public string StorageAccountName => this.storage.AccountName;

    public IRetryPolicy RetryPolicy
    {
      set
      {
      }
    }

    public int MaxSegmentSize => 100;

    public string Name { get; }

    public Task<PreauthenticatedUri> GetRowSasUriAsync(
      VssRequestPump.Processor processor,
      string partition,
      string row)
    {
      return Task.FromResult<PreauthenticatedUri>(new PreauthenticatedUri(new Uri("about:blank"), EdgeType.NotEdge));
    }

    public Task<bool> CreateIfNotExistsAsync(
      VssRequestPump.Processor processor,
      TableRequestOptions options = null,
      OperationContext context = null)
    {
      return Task.FromResult<bool>(this.storage.CreateTableIfNotExists(this.Name));
    }

    public Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      TableRequestOptions options = null,
      OperationContext context = null)
    {
      return Task.FromResult<bool>(this.storage.DeleteTableIfExists(this.Name));
    }

    private static HttpStatusCode CheckInternal(
      MemoryPartition partition,
      TableOperationDescriptor operation)
    {
      MemoryRow memoryRow;
      if (!partition.Rows.TryGetValue(operation.RowKey, out memoryRow))
        memoryRow = (MemoryRow) null;
      TableEntityTableOperationDescriptor operationDescriptor1 = operation as TableEntityTableOperationDescriptor;
      TableRowTableOperationDescriptor operationDescriptor2 = operation as TableRowTableOperationDescriptor;
      if (operationDescriptor1 != null)
      {
        string etag = operationDescriptor1.TableEntity.ETag;
        bool flag = etag == memoryRow?.Etag;
        switch (operationDescriptor1.OperationType)
        {
          case TableOperationType.Insert:
            return memoryRow != null ? HttpStatusCode.Conflict : HttpStatusCode.OK;
          case TableOperationType.Delete:
          case TableOperationType.Replace:
          case TableOperationType.Merge:
            if (memoryRow == null)
              return HttpStatusCode.NotFound;
            if (etag == "*" | flag)
              return HttpStatusCode.OK;
            return etag == null ? HttpStatusCode.Conflict : HttpStatusCode.PreconditionFailed;
          case TableOperationType.InsertOrReplace:
          case TableOperationType.InsertOrMerge:
            return HttpStatusCode.OK;
          default:
            throw new InvalidOperationException();
        }
      }
      else
      {
        if (operationDescriptor2 == null)
          throw new InvalidOperationException();
        if (operationDescriptor2.OperationType != TableOperationType.Retrieve)
          throw new InvalidOperationException();
        return memoryRow == null ? HttpStatusCode.NotFound : HttpStatusCode.OK;
      }
    }

    private MemoryRow ExecuteInteralEntity(
      MemoryPartition partition,
      TableEntityTableOperationDescriptor operation)
    {
      MemoryRow memoryRow;
      if (!partition.Rows.TryGetValue(operation.RowKey, out memoryRow))
        memoryRow = (MemoryRow) null;
      TableOperationType tableOperationType = operation.OperationType;
      if (tableOperationType != TableOperationType.InsertOrReplace)
      {
        if (tableOperationType == TableOperationType.InsertOrMerge)
          tableOperationType = memoryRow == null ? TableOperationType.Insert : TableOperationType.Merge;
      }
      else
        tableOperationType = memoryRow == null ? TableOperationType.Insert : TableOperationType.Replace;
      switch (tableOperationType)
      {
        case TableOperationType.Insert:
          memoryRow = new MemoryRow(operation.TableEntity);
          memoryRow.Etag = partition.GetNextEtag();
          memoryRow.Timestamp = this.Clock.Now;
          partition.Rows.Add(operation.RowKey, memoryRow);
          break;
        case TableOperationType.Delete:
          partition.Rows.Remove(operation.RowKey);
          partition.GetNextEtag();
          break;
        case TableOperationType.Replace:
          memoryRow = new MemoryRow(operation.TableEntity);
          memoryRow.Etag = partition.GetNextEtag();
          memoryRow.Timestamp = this.Clock.Now;
          partition.Rows[operation.RowKey] = memoryRow;
          break;
        case TableOperationType.Merge:
          foreach (KeyValuePair<string, EntityProperty> keyValuePair in (IEnumerable<KeyValuePair<string, EntityProperty>>) operation.TableEntity.WriteEntity((OperationContext) null))
          {
            if (keyValuePair.Value.PropertyAsObject != null)
              memoryRow.Properties[keyValuePair.Key] = keyValuePair.Value;
          }
          memoryRow.Etag = partition.GetNextEtag();
          memoryRow.Timestamp = this.Clock.Now;
          partition.Rows[operation.RowKey] = memoryRow;
          break;
        default:
          throw new InvalidOperationException();
      }
      operation.TableEntity.ETag = memoryRow.Etag;
      return memoryRow;
    }

    private TableOperationResult ExecuteInternal(
      MemoryPartition partition,
      TableOperationDescriptor operation)
    {
      TableEntityTableOperationDescriptor operation1 = operation as TableEntityTableOperationDescriptor;
      TableRowTableOperationDescriptor operationDescriptor = operation as TableRowTableOperationDescriptor;
      ITableEntity result;
      if (operation1 != null)
      {
        MemoryRow memoryRow = this.ExecuteInteralEntity(partition, operation1);
        if (memoryRow == null)
        {
          result = (ITableEntity) null;
        }
        else
        {
          result = operation1.TableEntity;
          result.ReadEntity((IDictionary<string, EntityProperty>) memoryRow.Properties, new OperationContext());
        }
      }
      else
      {
        if (operationDescriptor == null)
          throw new InvalidOperationException();
        if (operationDescriptor.OperationType != TableOperationType.Retrieve)
          throw new InvalidOperationException();
        MemoryRow memoryRow;
        partition.Rows.TryGetValue(operation.RowKey, out memoryRow);
        result = (ITableEntity) new TableEntity(memoryRow.PartitionKey, memoryRow.RowKey);
        result.ETag = memoryRow.Etag;
        result.Timestamp = memoryRow.Timestamp;
        result.ReadEntity((IDictionary<string, EntityProperty>) memoryRow.Properties, new OperationContext());
      }
      return new TableOperationResult(MemoryTableAdapter.GetHttpSuccessCode(operation.OperationType), (object) result);
    }

    public Task<TableOperationResult> ExecuteAsync(
      VssRequestPump.Processor processor,
      TableOperationDescriptor operation,
      TableRequestOptions options = null,
      OperationContext context = null)
    {
      MemoryTable table;
      return Task.FromResult<TableOperationResult>(!this.storage.TryGetTable(this.Name, out table) ? new TableOperationResult(HttpStatusCode.NotFound, (object) "TableNotFound") : table.UsePartition<TableOperationResult>(operation.PartitionKey, (Func<MemoryPartition, TableOperationResult>) (partition =>
      {
        HttpStatusCode httpStatusCode = MemoryTableAdapter.CheckInternal(partition, operation);
        return httpStatusCode == HttpStatusCode.OK ? this.ExecuteInternal(partition, operation) : new TableOperationResult(httpStatusCode, (object) httpStatusCode.ToString());
      })));
    }

    public TableBatchOperationResult ExecuteBatchInternal(TableBatchOperationDescriptor operations)
    {
      string partitionKey = operations.Count != 0 ? operations.First<TableOperationDescriptor>().PartitionKey : throw new NotImplementedException();
      if (operations.All<TableOperationDescriptor>((Func<TableOperationDescriptor, bool>) (op => op.PartitionKey != partitionKey)))
        throw new ArgumentException("Partition keys are not all the same.");
      if (operations.Select<TableOperationDescriptor, string>((Func<TableOperationDescriptor, string>) (op => op.RowKey)).Distinct<string>().Count<string>() != operations.Count)
        throw new ArgumentException("Duplicate row keys are not allowed in a single batch.");
      if (operations.Count<TableOperationDescriptor>((Func<TableOperationDescriptor, bool>) (op => op.OperationType == TableOperationType.Retrieve)) > 1)
        return TableBatchOperationResult.FromError(new int?(), (TableOperationDescriptor) null, "No more than one Retrieve per batch", HttpStatusCode.BadRequest, (StorageException) null);
      MemoryTable table;
      return this.storage.TryGetTable(this.Name, out table) ? table.UsePartition<TableBatchOperationResult>(partitionKey, (Func<MemoryPartition, TableBatchOperationResult>) (partition =>
      {
        for (int index = 0; index < operations.Count; ++index)
        {
          HttpStatusCode statusCode = MemoryTableAdapter.CheckInternal(partition, operations[index]);
          if (statusCode != HttpStatusCode.OK)
            return TableBatchOperationResult.FromError(new int?(index), operations[index], statusCode.ToString(), statusCode, (StorageException) null);
        }
        List<TableOperationResult> results = new List<TableOperationResult>();
        foreach (TableOperationDescriptor operation in (List<TableOperationDescriptor>) operations)
          results.Add(this.ExecuteInternal(partition, operation));
        return TableBatchOperationResult.FromSuccess((IList<TableOperationResult>) results);
      })) : TableBatchOperationResult.FromError(new int?(0), (TableOperationDescriptor) null, "TableNotFound", HttpStatusCode.NotFound, (StorageException) null);
    }

    public virtual Task<TableBatchOperationResult> ExecuteBatchAsync(
      VssRequestPump.Processor processor,
      TableBatchOperationDescriptor operations,
      TableRequestOptions options,
      OperationContext context)
    {
      Action<TableBatchOperationDescriptor> executeBatchAsync = this.PreExecuteBatchAsync;
      if (executeBatchAsync != null)
        executeBatchAsync(operations);
      return Task.FromResult<TableBatchOperationResult>(this.ExecuteBatchInternal(operations));
    }

    public Task<IResultSegment<TEntity>> ExecuteQuerySegmentedAsync<TEntity>(
      VssRequestPump.Processor processor,
      Query<TEntity> query,
      ITableQueryContinuationToken token)
      where TEntity : ITableEntity, new()
    {
      MemoryTableAdapter.ResultSegment<TEntity> segment = new MemoryTableAdapter.ResultSegment<TEntity>();
      MemoryTable table;
      if (this.storage.TryGetTable(this.Name, out table))
      {
        segment.TableExists = true;
        foreach (string partitionName1 in table.PartitionNames)
        {
          string partitionName = partitionName1;
          if ((query.PartitionKeyMin == null || string.CompareOrdinal(query.PartitionKeyMin, partitionName) <= 0) && (query.PartitionKeyMax == null || string.CompareOrdinal(query.PartitionKeyMax, partitionName) >= 0) && (token == null || string.CompareOrdinal(token.NextPartitionKey, partitionName) <= 0))
            table.UsePartition<int>(partitionName, (Func<MemoryPartition, int>) (partition =>
            {
              foreach (MemoryRow memoryRow in query.filter.Evaluate<IColumn, MemoryRow>(partition.Rows.Where<KeyValuePair<string, MemoryRow>>(closure_0 ?? (closure_0 = (Func<KeyValuePair<string, MemoryRow>, bool>) (row => token == null || string.CompareOrdinal(token.NextRowKey, row.Key) <= 0))).Select<KeyValuePair<string, MemoryRow>, MemoryRow>((Func<KeyValuePair<string, MemoryRow>, MemoryRow>) (kvp => kvp.Value))))
              {
                TEntity entity1 = new TEntity();
                entity1.PartitionKey = partitionName;
                ref TEntity local1 = ref entity1;
                TEntity entity2 = default (TEntity);
                if ((object) entity2 == null)
                {
                  entity2 = local1;
                  local1 = ref entity2;
                }
                string rowKey = memoryRow.RowKey;
                local1.RowKey = rowKey;
                entity1.ETag = memoryRow.Etag;
                ref TEntity local2 = ref entity1;
                entity2 = default (TEntity);
                if ((object) entity2 == null)
                {
                  entity2 = local2;
                  local2 = ref entity2;
                }
                DateTimeOffset timestamp = memoryRow.Timestamp;
                local2.Timestamp = timestamp;
                entity1.ReadEntity((IDictionary<string, EntityProperty>) memoryRow.Properties, (OperationContext) null);
                segment.Results.Add(entity1);
              }
              return 0;
            }));
        }
      }
      else
        segment.TableExists = false;
      segment.Results.ValidateQueryResult<TEntity>(query);
      return Task.FromResult<IResultSegment<TEntity>>((IResultSegment<TEntity>) segment);
    }

    public Task<IResultSegment<TEntity>> ExecuteQuerySegmentedAsync<TEntity>(
      VssRequestPump.Processor processor,
      Query<TEntity> query,
      ITableQueryContinuationToken token,
      TableRequestOptions options,
      OperationContext context)
      where TEntity : ITableEntity, new()
    {
      return this.ExecuteQuerySegmentedAsync<TEntity>(processor, query, token);
    }

    private static HttpStatusCode GetHttpSuccessCode(TableOperationType opType)
    {
      switch (opType)
      {
        case TableOperationType.Insert:
        case TableOperationType.Delete:
        case TableOperationType.Replace:
        case TableOperationType.Merge:
        case TableOperationType.InsertOrReplace:
        case TableOperationType.InsertOrMerge:
          return HttpStatusCode.NoContent;
        case TableOperationType.Retrieve:
          return HttpStatusCode.OK;
        default:
          throw new InvalidOperationException("unknown opType " + opType.ToString());
      }
    }

    private class ResultSegment<TEntity> : IResultSegment<TEntity> where TEntity : ITableEntity, new()
    {
      public ITableQueryContinuationToken ContinuationToken { get; set; }

      public List<TEntity> Results { get; } = new List<TEntity>();

      public bool TableExists { get; set; }
    }
  }
}
