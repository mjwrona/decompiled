// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.TableOperationExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class TableOperationExtensions
  {
    private const int MaxOperationsPerTableBatch = 100;

    public static IEnumerable<TableBatchOperation> ToTableBatchOperations(
      this IEnumerable<ITableEntity> entities,
      TableOperationType operationType)
    {
      return entities.GroupBy<ITableEntity, string>((Func<ITableEntity, string>) (x => x.PartitionKey)).SelectMany<IGrouping<string, ITableEntity>, TableBatchOperation>((Func<IGrouping<string, ITableEntity>, IEnumerable<TableBatchOperation>>) (partition => partition.Batch<ITableEntity>(100).Select<IList<ITableEntity>, TableBatchOperation>((Func<IList<ITableEntity>, TableBatchOperation>) (batch => batch.ToTableBatchOperation(operationType)))));
    }

    private static TableBatchOperation ToTableBatchOperation(
      this IEnumerable<ITableEntity> entities,
      TableOperationType operationType)
    {
      TableBatchOperation collection = new TableBatchOperation();
      collection.AddRange<TableOperation, TableBatchOperation>(entities.Select<ITableEntity, TableOperation>((Func<ITableEntity, TableOperation>) (item => item.ToTableOperation(operationType))));
      return collection;
    }

    private static TableOperation ToTableOperation(
      this ITableEntity item,
      TableOperationType operationType)
    {
      switch (operationType)
      {
        case TableOperationType.Insert:
          return TableOperation.Insert(item);
        case TableOperationType.Delete:
          return TableOperation.Delete(item);
        case TableOperationType.Replace:
          return TableOperation.Replace(item);
        case TableOperationType.Merge:
          return TableOperation.Merge(item);
        case TableOperationType.InsertOrReplace:
          return TableOperation.InsertOrReplace(item);
        case TableOperationType.InsertOrMerge:
          return TableOperation.InsertOrMerge(item);
        default:
          throw new ArgumentOutOfRangeException(nameof (operationType), (object) operationType, (string) null);
      }
    }
  }
}
