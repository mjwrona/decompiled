// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureTableStorageProviderExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class AzureTableStorageProviderExtensions
  {
    private const string c_area = "AzureTableStorageProviderExtensions";
    private const string c_layer = "AzureTableStorageProviderExtensions";

    public static List<T> QueryTable<T>(
      this IAzureTableStorageProvider storageProvider,
      IVssRequestContext requestContext,
      string tableName,
      TableQuery<T> query)
      where T : ITableEntity, new()
    {
      List<T> objList = new List<T>();
      TableContinuationToken continuationToken = (TableContinuationToken) null;
      do
      {
        objList.AddRange((IEnumerable<T>) storageProvider.QueryTable<T>(requestContext, tableName, query, ref continuationToken));
      }
      while (continuationToken != null);
      return objList;
    }

    public static List<T> QueryTableByPartitionKey<T>(
      this IAzureTableStorageProvider storageProvider,
      IVssRequestContext requestContext,
      string tableName,
      string partitionKeyValue)
      where T : ITableEntity, new()
    {
      ArgumentUtility.CheckStringForNullOrEmpty(partitionKeyValue, nameof (partitionKeyValue));
      TableQuery<T> query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", "eq", partitionKeyValue));
      return storageProvider.QueryTable<T>(requestContext, tableName, query);
    }

    public static TableResult Retrieve<TEntity>(
      this IAzureTableStorageProvider storageProvider,
      IVssRequestContext requestContext,
      string tableName,
      ITableEntityKeys entityKeys)
      where TEntity : ITableEntity
    {
      ArgumentUtility.CheckForNull<ITableEntityKeys>(entityKeys, nameof (entityKeys));
      ArgumentUtility.CheckForNull<string>(entityKeys.PartitionKey, "PartitionKey");
      ArgumentUtility.CheckForNull<string>(entityKeys.RowKey, "RowKey");
      return storageProvider.ExecuteTableOperation(requestContext, tableName, TableOperation.Retrieve<TEntity>(entityKeys.PartitionKey, entityKeys.RowKey));
    }

    public static TableResult Upsert(
      this IAzureTableStorageProvider storageProvider,
      IVssRequestContext requestContext,
      string tableName,
      ITableEntity entity)
    {
      ArgumentUtility.CheckForNull<ITableEntity>(entity, nameof (entity));
      requestContext.CheckWriteAccess();
      return storageProvider.ExecuteTableOperation(requestContext, tableName, TableOperation.InsertOrReplace(entity));
    }

    public static IList<TableResult> Upsert(
      this IAzureTableStorageProvider storageProvider,
      IVssRequestContext requestContext,
      string tableName,
      IEnumerable<ITableEntity> entities)
    {
      requestContext.CheckWriteAccess();
      TableBatchOperation batch = new TableBatchOperation();
      return storageProvider.PrepareAndExecuteBatchOperation(requestContext, tableName, entities, batch, new Action<ITableEntity>(batch.InsertOrReplace));
    }

    public static IList<TableResult> NonAtomicUpsert(
      this IAzureTableStorageProvider storageProvider,
      IVssRequestContext requestContext,
      string tableName,
      IEnumerable<ITableEntity> entities)
    {
      requestContext.CheckWriteAccess();
      TableBatchOperation batch = new TableBatchOperation();
      return storageProvider.PrepareAndExecuteNonAtomicBatchOperation(requestContext, tableName, entities, batch, new Action<ITableEntity>(batch.InsertOrReplace));
    }

    public static TableResult Delete(
      this IAzureTableStorageProvider storageProvider,
      IVssRequestContext requestContext,
      string tableName,
      ITableEntity entity)
    {
      ArgumentUtility.CheckForNull<ITableEntity>(entity, nameof (entity));
      requestContext.CheckWriteAccess();
      return storageProvider.ExecuteTableOperation(requestContext, tableName, TableOperation.Delete(entity));
    }

    public static IList<TableResult> Delete(
      this IAzureTableStorageProvider storageProvider,
      IVssRequestContext requestContext,
      string tableName,
      IEnumerable<ITableEntity> entities)
    {
      requestContext.CheckWriteAccess();
      TableBatchOperation batch = new TableBatchOperation();
      return storageProvider.PrepareAndExecuteBatchOperation(requestContext, tableName, entities, batch, new Action<ITableEntity>(batch.Delete));
    }

    private static IList<TableResult> PrepareAndExecuteBatchOperation(
      this IAzureTableStorageProvider storageProvider,
      IVssRequestContext requestContext,
      string tableName,
      IEnumerable<ITableEntity> entities,
      TableBatchOperation batch,
      Action<ITableEntity> batchMethod)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) entities, nameof (entities));
      ArgumentUtility.CheckForNull<TableBatchOperation>(batch, nameof (batch));
      ArgumentUtility.CheckForNull<Action<ITableEntity>>(batchMethod, nameof (batchMethod));
      string a = (string) null;
      foreach (ITableEntity entity in entities)
      {
        if (a == null)
          a = entity.PartitionKey;
        else if (!string.Equals(a, entity.PartitionKey, StringComparison.Ordinal))
          throw new ArgumentException("All table entities within the batch must belong to the same partition");
        batchMethod(entity);
        if (batch.Count > AzureTableStorageProvider.MaxTableOperationsPerBatch)
          throw new ArgumentException("Exceeding maximum batch size");
      }
      return storageProvider.ExecuteBatchOperation(requestContext, tableName, batch);
    }

    private static IList<TableResult> PrepareAndExecuteNonAtomicBatchOperation(
      this IAzureTableStorageProvider storageProvider,
      IVssRequestContext requestContext,
      string tableName,
      IEnumerable<ITableEntity> entities,
      TableBatchOperation batch,
      Action<ITableEntity> batchMethod)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) entities, nameof (entities));
      ArgumentUtility.CheckForNull<TableBatchOperation>(batch, nameof (batch));
      ArgumentUtility.CheckForNull<Action<ITableEntity>>(batchMethod, nameof (batchMethod));
      List<TableResult> tableResultList = new List<TableResult>();
      foreach (IEnumerable<ITableEntity> tableEntities in (IEnumerable<IGrouping<string, ITableEntity>>) entities.ToLookup<ITableEntity, string>((Func<ITableEntity, string>) (entity => entity.PartitionKey), (IEqualityComparer<string>) StringComparer.Ordinal))
      {
        foreach (ITableEntity tableEntity in tableEntities)
        {
          if (batch.Count + 1 > AzureTableStorageProvider.MaxTableOperationsPerBatch)
          {
            tableResultList.AddRange((IEnumerable<TableResult>) storageProvider.ExecuteBatchOperation(requestContext, tableName, batch));
            batch.Clear();
          }
          batchMethod(tableEntity);
        }
        if (batch.Count > 0)
        {
          tableResultList.AddRange((IEnumerable<TableResult>) storageProvider.ExecuteBatchOperation(requestContext, tableName, batch));
          batch.Clear();
        }
      }
      return (IList<TableResult>) tableResultList;
    }
  }
}
