// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableOperation
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class TableOperation
  {
    private Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, object> retrieveResolver;

    internal TableOperation(ITableEntity entity, TableOperationType operationType)
      : this(entity, operationType, true)
    {
    }

    internal TableOperation(
      ITableEntity entity,
      TableOperationType operationType,
      bool echoContent)
    {
      this.Entity = entity != null || operationType == TableOperationType.Retrieve ? entity : throw new ArgumentNullException(nameof (entity));
      this.OperationType = operationType;
      this.EchoContent = echoContent;
    }

    public ITableEntity Entity { get; private set; }

    public TableOperationType OperationType { get; private set; }

    internal bool IsTableEntity { get; set; }

    internal bool IsPrimaryOnlyRetrieve { get; set; }

    internal string RetrievePartitionKey { get; set; }

    internal string RetrieveRowKey { get; set; }

    internal string PartitionKey => this.OperationType == TableOperationType.Retrieve ? this.RetrievePartitionKey : this.Entity.PartitionKey;

    internal string RowKey => this.OperationType == TableOperationType.Retrieve ? this.RetrieveRowKey : this.Entity.RowKey;

    internal string ETag
    {
      get
      {
        int operationType = (int) this.OperationType;
        return this.Entity.ETag;
      }
    }

    internal Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, object> RetrieveResolver
    {
      get
      {
        if (this.retrieveResolver == null)
          this.retrieveResolver = new Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, object>(TableOperation.DynamicEntityResolver);
        return this.retrieveResolver;
      }
      set => this.retrieveResolver = value;
    }

    internal Type PropertyResolverType { get; set; }

    internal bool EchoContent { get; set; }

    internal List<string> SelectColumns { get; set; }

    public static TableOperation Delete(ITableEntity entity)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      return !string.IsNullOrEmpty(entity.ETag) ? new TableOperation(entity, TableOperationType.Delete) : throw new ArgumentException("Delete requires an ETag (which may be the '*' wildcard).");
    }

    public static TableOperation Insert(ITableEntity entity) => TableOperation.Insert(entity, false);

    public static TableOperation Insert(ITableEntity entity, bool echoContent)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      return new TableOperation(entity, TableOperationType.Insert, echoContent);
    }

    public static TableOperation InsertOrMerge(ITableEntity entity)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      return new TableOperation(entity, TableOperationType.InsertOrMerge);
    }

    public static TableOperation InsertOrReplace(ITableEntity entity)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      return new TableOperation(entity, TableOperationType.InsertOrReplace);
    }

    public static TableOperation Merge(ITableEntity entity)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      return !string.IsNullOrEmpty(entity.ETag) ? new TableOperation(entity, TableOperationType.Merge) : throw new ArgumentException("Merge requires an ETag (which may be the '*' wildcard).");
    }

    public static TableOperation Replace(ITableEntity entity)
    {
      CommonUtility.AssertNotNull(nameof (entity), (object) entity);
      return !string.IsNullOrEmpty(entity.ETag) ? new TableOperation(entity, TableOperationType.Replace) : throw new ArgumentException("Replace requires an ETag (which may be the '*' wildcard).");
    }

    public static TableOperation Retrieve<TElement>(
      string partitionKey,
      string rowkey,
      List<string> selectColumns = null)
      where TElement : ITableEntity
    {
      CommonUtility.AssertNotNull(nameof (partitionKey), (object) partitionKey);
      CommonUtility.AssertNotNull(nameof (rowkey), (object) rowkey);
      return new TableOperation((ITableEntity) null, TableOperationType.Retrieve)
      {
        RetrievePartitionKey = partitionKey,
        RetrieveRowKey = rowkey,
        SelectColumns = selectColumns,
        RetrieveResolver = (Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, object>) ((pk, rk, ts, prop, etag) => (object) EntityUtilities.ResolveEntityByType<TElement>(pk, rk, ts, prop, etag)),
        PropertyResolverType = typeof (TElement)
      };
    }

    public static TableOperation Retrieve<TResult>(
      string partitionKey,
      string rowkey,
      EntityResolver<TResult> resolver,
      List<string> selectedColumns = null)
    {
      CommonUtility.AssertNotNull(nameof (partitionKey), (object) partitionKey);
      CommonUtility.AssertNotNull(nameof (rowkey), (object) rowkey);
      return new TableOperation((ITableEntity) null, TableOperationType.Retrieve)
      {
        RetrievePartitionKey = partitionKey,
        RetrieveRowKey = rowkey,
        RetrieveResolver = (Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, object>) ((pk, rk, ts, prop, etag) => (object) resolver(pk, rk, ts, prop, etag)),
        SelectColumns = selectedColumns
      };
    }

    public static TableOperation Retrieve(
      string partitionKey,
      string rowkey,
      List<string> selectedColumns = null)
    {
      CommonUtility.AssertNotNull(nameof (partitionKey), (object) partitionKey);
      CommonUtility.AssertNotNull(nameof (rowkey), (object) rowkey);
      return new TableOperation((ITableEntity) null, TableOperationType.Retrieve)
      {
        RetrievePartitionKey = partitionKey,
        RetrieveRowKey = rowkey,
        SelectColumns = selectedColumns
      };
    }

    internal TableResult Execute(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      return client.Executor.ExecuteTableOperation<TableResult>(this, client, table, requestOptions1, operationContext);
    }

    internal Task<TableResult> ExecuteAsync(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      TableRequestOptions requestOptions1 = TableRequestOptions.ApplyDefaults(requestOptions, client);
      operationContext = operationContext ?? new OperationContext();
      CommonUtility.AssertNotNullOrEmpty("tableName", table.Name);
      return client.Executor.ExecuteTableOperationAsync<TableResult>(this, client, table, requestOptions1, operationContext, cancellationToken);
    }

    private static object DynamicEntityResolver(
      string partitionKey,
      string rowKey,
      DateTimeOffset timestamp,
      IDictionary<string, EntityProperty> properties,
      string etag)
    {
      DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
      dynamicTableEntity.PartitionKey = partitionKey;
      dynamicTableEntity.RowKey = rowKey;
      dynamicTableEntity.Timestamp = timestamp;
      dynamicTableEntity.ReadEntity(properties, (OperationContext) null);
      dynamicTableEntity.ETag = etag;
      return (object) dynamicTableEntity;
    }
  }
}
