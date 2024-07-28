// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableBatchOperation
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class TableBatchOperation : 
    IList<TableOperation>,
    ICollection<TableOperation>,
    IEnumerable<TableOperation>,
    IEnumerable
  {
    private bool hasQuery;
    internal List<TableOperation> operations = new List<TableOperation>();
    internal string batchPartitionKey;

    internal bool ContainsWrites { get; private set; }

    public int Count => this.operations.Count;

    public bool IsReadOnly => false;

    public TableOperation this[int index]
    {
      get => this.operations[index];
      set => throw new NotSupportedException();
    }

    public void Delete(ITableEntity entity)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      if (string.IsNullOrEmpty(entity.ETag))
        throw new ArgumentException("Delete requires an ETag (which may be the '*' wildcard).");
      this.Add(new TableOperation(entity, TableOperationType.Delete));
    }

    public void Insert(ITableEntity entity) => this.Insert(entity, false);

    public void Insert(ITableEntity entity, bool echoContent)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      this.Add(new TableOperation(entity, TableOperationType.Insert, echoContent));
    }

    public void InsertOrMerge(ITableEntity entity)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      this.Add(new TableOperation(entity, TableOperationType.InsertOrMerge));
    }

    public void InsertOrReplace(ITableEntity entity)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      this.Add(new TableOperation(entity, TableOperationType.InsertOrReplace));
    }

    public void Merge(ITableEntity entity)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      if (string.IsNullOrEmpty(entity.ETag))
        throw new ArgumentException("Merge requires an ETag (which may be the '*' wildcard).");
      this.Add(new TableOperation(entity, TableOperationType.Merge));
    }

    public void Replace(ITableEntity entity)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      if (string.IsNullOrEmpty(entity.ETag))
        throw new ArgumentException("Replace requires an ETag (which may be the '*' wildcard).");
      this.Add(new TableOperation(entity, TableOperationType.Replace));
    }

    public void Retrieve(string partitionKey, string rowKey)
    {
      CommonUtility.AssertNotNull(nameof (partitionKey), (object) partitionKey);
      CommonUtility.AssertNotNull("rowkey", (object) rowKey);
      this.Add(new TableOperation((ITableEntity) null, TableOperationType.Retrieve)
      {
        RetrievePartitionKey = partitionKey,
        RetrieveRowKey = rowKey
      });
    }

    public int IndexOf(TableOperation item) => this.operations.IndexOf(item);

    public void Insert(int index, TableOperation item)
    {
      CommonUtility.AssertNotNull(nameof (item), (object) item);
      this.CheckSingleQueryPerBatch(item);
      this.LockToPartitionKey(item.PartitionKey);
      TableBatchOperation.CheckPartitionKeyRowKeyPresent(item);
      this.operations.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      this.operations.RemoveAt(index);
      if (this.operations.Count != 0)
        return;
      this.batchPartitionKey = (string) null;
      this.hasQuery = false;
    }

    public void Add(TableOperation item)
    {
      CommonUtility.AssertNotNull(nameof (item), (object) item);
      this.CheckSingleQueryPerBatch(item);
      this.LockToPartitionKey(item.PartitionKey);
      TableBatchOperation.CheckPartitionKeyRowKeyPresent(item);
      this.operations.Add(item);
    }

    public void Clear()
    {
      this.operations.Clear();
      this.batchPartitionKey = (string) null;
      this.hasQuery = false;
    }

    public bool Contains(TableOperation item) => this.operations.Contains(item);

    public void CopyTo(TableOperation[] array, int arrayIndex) => this.operations.CopyTo(array, arrayIndex);

    public bool Remove(TableOperation item)
    {
      CommonUtility.AssertNotNull(nameof (item), (object) item);
      int num = this.operations.Remove(item) ? 1 : 0;
      if (this.operations.Count != 0)
        return num != 0;
      this.batchPartitionKey = (string) null;
      this.hasQuery = false;
      return num != 0;
    }

    public IEnumerator<TableOperation> GetEnumerator() => (IEnumerator<TableOperation>) this.operations.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.operations.GetEnumerator();

    public void Retrieve<TElement>(
      string partitionKey,
      string rowKey,
      List<string> selectedColumns = null)
      where TElement : ITableEntity
    {
      CommonUtility.AssertNotNull(nameof (partitionKey), (object) partitionKey);
      CommonUtility.AssertNotNull("rowkey", (object) rowKey);
      this.Add(new TableOperation((ITableEntity) null, TableOperationType.Retrieve)
      {
        RetrievePartitionKey = partitionKey,
        RetrieveRowKey = rowKey,
        SelectColumns = selectedColumns,
        RetrieveResolver = (Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, object>) ((pk, rk, ts, prop, etag) => (object) EntityUtilities.ResolveEntityByType<TElement>(pk, rk, ts, prop, etag))
      });
    }

    public void Retrieve<TResult>(
      string partitionKey,
      string rowKey,
      EntityResolver<TResult> resolver,
      List<string> selectedColumns = null)
    {
      CommonUtility.AssertNotNull(nameof (partitionKey), (object) partitionKey);
      CommonUtility.AssertNotNull("rowkey", (object) rowKey);
      this.Add(new TableOperation((ITableEntity) null, TableOperationType.Retrieve)
      {
        RetrievePartitionKey = partitionKey,
        RetrieveRowKey = rowKey,
        SelectColumns = selectedColumns,
        RetrieveResolver = (Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, object>) ((pk, rk, ts, prop, etag) => (object) resolver(pk, rk, ts, prop, etag))
      });
    }

    internal TableBatchResult Execute(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      if (this.operations.Count == 0)
        throw new InvalidOperationException("Cannot execute an empty batch operation");
      if (this.operations.Count > 100)
        throw new InvalidOperationException("The maximum number of operations allowed in one batch has been exceeded.");
      return client.Executor.ExecuteTableBatchOperation<TableBatchResult>(this, client, table, requestOptions1, operationContext);
    }

    internal Task<TableBatchResult> ExecuteAsync(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      if (this.operations.Count == 0)
        throw new InvalidOperationException("Cannot execute an empty batch operation");
      if (this.operations.Count > 100)
        throw new InvalidOperationException("The maximum number of operations allowed in one batch has been exceeded.");
      return client.Executor.ExecuteTableBatchOperationAsync<TableBatchResult>(this, client, table, requestOptions1, operationContext, cancellationToken);
    }

    private void CheckSingleQueryPerBatch(TableOperation item)
    {
      if (this.hasQuery)
        throw new ArgumentException("A batch transaction with a retrieve operation cannot contain any other operations.");
      if (item.OperationType == TableOperationType.Retrieve)
      {
        if (this.operations.Count > 0)
          throw new ArgumentException("A batch transaction with a retrieve operation cannot contain any other operations.");
        this.hasQuery = true;
      }
      this.ContainsWrites = item.OperationType != TableOperationType.Retrieve;
    }

    private void LockToPartitionKey(string partitionKey)
    {
      if (this.batchPartitionKey == null)
        this.batchPartitionKey = partitionKey;
      else if (partitionKey != this.batchPartitionKey)
        throw new ArgumentException("All entities in a given batch must have the same partition key.");
    }

    private static void CheckPartitionKeyRowKeyPresent(TableOperation item)
    {
      if (item.OperationType != TableOperationType.Retrieve && (item.PartitionKey == null || item.RowKey == null))
        throw new ArgumentNullException(nameof (item), "A batch non-retrieve operation requires a non-null partition key and row key.");
    }
  }
}
